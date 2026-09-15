#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
通用 FPS (freeproblemset) XML 转换器

读取一组 hydro 格式题目目录，输出 FPS 1.4 / 1.6 XML。

题目目录约定 (hydro 格式):
  <problem_dir>/
    problem.yaml            题面元数据 (pid/title/source/tag/time_limit/memory_limit)
    problem_zh.md           题面 markdown (描述 / 输入 / 输出 / 样例 / 提示)
    std.cpp / std.py / ...  标准解法（语言从扩展名推断；可选）
    testdata/
      config.yaml           hydro 配置（time / memory / subtasks）
      N.in / dataNN.in      第 N 组输入
      N.ans / dataNN.ans    第 N 组输出

用法:
  # 命令行
  python3 convert_hydro_to_fps.py <source_dir> [output_dir] [options]

  # 交互模式（提示输入）
  python3 convert_hydro_to_fps.py --interactive

  # 管道（输出到 stdout）
  python3 convert_hydro_to_fps.py <source_dir> - | less

FPS 格式参考: https://github.com/zhblue/freeproblemset/blob/master/fps.current.dtd
"""

from __future__ import annotations

import argparse
import os
import re
import sys
from pathlib import Path
from typing import Iterable, Optional
from xml.sax.saxutils import escape

import markdown
import yaml

# ============================================================================
# 常量
# ============================================================================

FPS_DTD_URL = "http://hustoj.com/fps.current.dtd"
FPS_REPO_URL = "https://github.com/zhblue/freeproblemset/"

# 中文标题 → FPS 字段分类
SECTION_PATTERNS = {
    "description": [r"^描述$", r"^题目描述$"],
    "input":       [r"^输入格式$", r"^输入$", r"^输入说明$"],
    "output":      [r"^输出格式$", r"^输出$", r"^输出说明$"],
    "hint":        [r"^提示$", r"^数据范围$", r"^样例解释$", r"^说明$"],
}

LANG_BY_EXT = {
    ".cpp": "C++", ".cc": "C++", ".cxx": "C++",
    ".c":   "C",
    ".py":  "Python",
    ".java": "Java",
    ".pas": "Pascal",
    ".js":  "JavaScript",
    ".rb":  "Ruby",
    ".go":  "Go",
    ".rs":  "Rust",
    ".cs":  "C#",
    ".php": "PHP",
}


# ============================================================================
# Markdown 解析
# ============================================================================

def classify_section(name: str) -> Optional[str]:
    """把中文标题归到 fps 的字段。"""
    for key, patterns in SECTION_PATTERNS.items():
        for p in patterns:
            if re.match(p, name.strip()):
                return key
    return None


def parse_markdown(md_text: str):
    """
    解析 problem_zh.md，返回:
      title: 字符串
      sections: { description, input, output, hint, samples:[(in,out,explain)] }

    支持的样例写法:
      ## 样例 #1 + ### 样例输入 #1 + ### 样例输出 #1  （推荐）
      ## 样例     + ### 输入样例 1   + ### 输出样例 1   （7级-1/2 风格）
      ### 样例     + 两个代码块自动按 in→out 交替           （3级-1 风格）
      ### 样例 1/2 + 两个代码块自动按 in→out 交替           （3级-2 风格）
    """
    lines = md_text.splitlines()

    title = None
    current_section = None
    sections = {"description": [], "input": [], "output": [], "hint": []}
    samples = []
    cur_sample = None
    sample_auto = False
    sample_phase = None
    code_buffer = []
    in_code = False
    code_lang = ""
    collect_for = None
    saved_collect = None
    raw_buffer = []

    def submit_to(target, text):
        nonlocal cur_sample
        if target == "sample_in" and cur_sample is not None:
            cur_sample["in"] = text
        elif target == "sample_out" and cur_sample is not None:
            cur_sample["out"] = text
        elif target == "sample_explain" and cur_sample is not None:
            cur_sample["explain"] = text
        elif target in sections:
            sections[target].append(text)

    def flush_raw():
        if not raw_buffer:
            return
        text = "\n".join(raw_buffer).strip("\n")
        if text and collect_for is not None:
            submit_to(collect_for, text)
        raw_buffer.clear()

    def start_sample(idx_zero_based: int, auto: bool):
        nonlocal cur_sample, sample_auto, sample_phase, current_section, collect_for
        while len(samples) <= idx_zero_based:
            samples.append({"in": "", "out": "", "explain": ""})
        cur_sample = samples[idx_zero_based]
        sample_auto = auto
        sample_phase = "sample_in"
        current_section = "sample"
        collect_for = None

    i = 0
    while i < len(lines):
        line = lines[i]
        stripped = line.strip()

        # 围栏代码块
        if stripped.startswith("```"):
            flush_raw()
            if not in_code:
                in_code = True
                code_lang = stripped[3:].strip()
                code_buffer = []
                saved_collect = collect_for
                if cur_sample is not None and sample_auto and saved_collect is None:
                    saved_collect = sample_phase
            else:
                in_code = False
                content = "\n".join(code_buffer)
                if saved_collect is not None:
                    submit_to(saved_collect, content)
                code_buffer = []
                code_lang = ""
                if cur_sample is not None and sample_auto:
                    if saved_collect == "sample_in":
                        sample_phase = "sample_out"
                        collect_for = "sample_out"
                    else:
                        sample_phase = None
                        collect_for = None
                else:
                    if saved_collect == "sample_out":
                        collect_for = "sample_explain"
                    else:
                        collect_for = saved_collect
                saved_collect = None
            i += 1
            continue

        if in_code:
            code_buffer.append(line)
            i += 1
            continue

        # 标题
        m = re.match(r"^(#+)\s+(.+)$", line)
        if m:
            flush_raw()
            level = len(m.group(1))
            name = m.group(2).strip()

            if level == 1 and title is None:
                title = name
                current_section = None
                collect_for = None
                i += 1
                continue

            classified = classify_section(name)

            # ## 样例 / ## 样例 #1 / ## 样例 1
            if level == 2 and re.match(r"^样例(\s*#?\s*\d+)?$", name):
                mnum = re.search(r"(\d+)", name)
                idx = (int(mnum.group(1)) - 1) if mnum else len(samples)
                start_sample(idx, auto=False)
                i += 1
                continue

            # ### 样例输入 / 样例输出 / 输入样例 / 输出样例 / 样例解释
            if level == 3 and current_section == "sample" and not sample_auto:
                mnum = re.search(r"(\d+)", name)
                idx = (int(mnum.group(1)) - 1) if mnum else None
                if idx is not None:
                    while len(samples) <= idx:
                        samples.append({"in": "", "out": "", "explain": ""})
                    cur_sample = samples[idx]
                if re.match(r"^样例?输入", name) or name.startswith("输入样例"):
                    collect_for = "sample_in"
                elif re.match(r"^样例?输出", name) or name.startswith("输出样例"):
                    collect_for = "sample_out"
                elif name.startswith("样例解释"):
                    collect_for = "sample_explain"
                else:
                    collect_for = None
                i += 1
                continue

            # ### 样例 N / ### 样例 (h3 auto 模式)
            if level == 3 and re.match(r"^样例(\s*\d+)?$", name):
                mnum = re.search(r"(\d+)", name)
                idx = (int(mnum.group(1)) - 1) if mnum else len(samples)
                start_sample(idx, auto=True)
                i += 1
                continue

            # 普通 ## 段
            if level == 2:
                if classified == "hint" and current_section == "sample":
                    collect_for = "sample_explain"
                elif classified:
                    current_section = classified
                    collect_for = classified
                    sample_auto = False
                else:
                    current_section = None
                    collect_for = None
                i += 1
                continue

            # 普通 ### 段
            if level == 3:
                if classified:
                    current_section = classified
                    collect_for = classified
                else:
                    current_section = None
                    collect_for = None
                i += 1
                continue

        raw_buffer.append(line)
        i += 1

    flush_raw()

    hint_parts = []
    if sections["hint"]:
        hint_parts.append("\n\n".join(sections["hint"]))
    for idx, s in enumerate(samples, 1):
        if s["explain"]:
            hint_parts.append(f"**样例 {idx} 解释**\n\n{s['explain']}")
    hint_text = "\n\n".join(hint_parts).strip()

    return title, {
        "description": "\n\n".join(sections["description"]).strip(),
        "input":       "\n\n".join(sections["input"]).strip(),
        "output":      "\n\n".join(sections["output"]).strip(),
        "hint":        hint_text,
        "samples":     [(s["in"], s["out"]) for s in samples if s["in"] or s["out"]],
        "raw_markdown": md_text,  # 保留原文，供 hustoj 风格使用
    }


# ============================================================================
# markdown → HTML
# ============================================================================

_md_engine = markdown.Markdown(extensions=['fenced_code', 'tables', 'sane_lists'])


def md_to_html(text: str) -> str:
    if not text:
        return ""
    _md_engine.reset()
    return _md_engine.convert(text).strip()


# ============================================================================
# YAML 字段归一化
# ============================================================================

def parse_time(s) -> Optional[float]:
    """'1s' / '2000ms' / '1000' / '1.0s' → 秒（浮点）"""
    if s is None:
        return None
    s = str(s).strip().lower()
    m = re.match(r"^([0-9.]+)\s*(ms|s|秒|毫秒)?$", s)
    if not m:
        return None
    val = float(m.group(1))
    unit = m.group(2) or "s"
    if unit in ("ms", "毫秒"):
        return val / 1000.0
    return val


def parse_memory(s) -> Optional[int]:
    """'512MB' / '524288' / '512' → MB（整数）"""
    if s is None:
        return None
    s = str(s).strip().upper().replace(" ", "")
    m = re.match(r"^([0-9.]+)\s*(MB|M|KB|K|GB|G|B)?$", s)
    if not m:
        return None
    val = float(m.group(1))
    unit = m.group(2) or "MB"
    if unit in ("GB", "G"):
        return int(val * 1024)
    if unit in ("KB", "K"):
        return max(1, int(val / 1024))
    if unit == "B":
        return max(1, int(val / 1024 / 1024))
    return int(val)


def fmt_time(t: Optional[float]) -> str:
    """秒 → 字符串。整数显示为 '1'，小数显示为 '1.000'。"""
    if t is None:
        return "1"
    if t == int(t):
        return str(int(t))
    return f"{t:.3f}"


# ============================================================================
# 测试数据发现
# ============================================================================

def discover_test_pairs(testdata_dir: Path):
    """
    在 testdata_dir 下找所有 .in/.ans 配对，按数字大小排序。

    兼容命名:
      1.in / 1.ans
      data01.in / data01.ans
      test_1.in / test_1.ans
      foo.in / foo.ans（兜底：成对出现）
    """
    pairs = []
    for p in sorted(testdata_dir.glob("*.in")):
        ans = testdata_dir / f"{p.stem}.ans"
        if not ans.exists():
            continue
        m = re.search(r"(\d+)", p.stem)
        idx = int(m.group(1)) if m else len(pairs) + 1
        pairs.append((idx, p, ans))
    pairs.sort(key=lambda x: x[0])
    return pairs


# ============================================================================
# 标准解法发现
# ============================================================================

def discover_solution(problem_dir: Path):
    """
    在题目目录下找标程。优先级:
      std.{cpp,py,java,...} > solution.{cpp,py,...} > answer.*

    返回 (language, code) 或 (None, None)
    """
    candidates = ["std", "solution", "answer"]
    exts = list(LANG_BY_EXT.keys())
    for c in candidates:
        for ext in exts:
            p = problem_dir / f"{c}{ext}"
            if p.exists():
                lang = LANG_BY_EXT[ext]
                return lang, p.read_text(encoding="utf-8")
    return None, None


# ============================================================================
# 单题 XML 构建
# ============================================================================

def build_test_blocks(test_pairs, *, preserve_bytes: bool = False) -> list[str]:
    """生成 <test_input> / <test_output> 字符串列表。"""
    blocks = []
    for idx, in_f, ans_f in test_pairs:
        in_data = in_f.read_text(encoding="utf-8")
        out_data = ans_f.read_text(encoding="utf-8")
        if not preserve_bytes:
            # 默认补换行（保持与 HUSTOJ 输出兼容）
            if in_data and not in_data.endswith("\n"):
                in_data += "\n"
            if out_data and not out_data.endswith("\n"):
                out_data += "\n"
        blocks.append(
            f'<test_input name="test_{idx}"><![CDATA[{in_data}]]></test_input>\n'
            f'<test_output name="test_{idx}"><![CDATA[{out_data}]]></test_output>\n'
        )
    return blocks


def build_sample_blocks(samples, *, preserve_bytes: bool = False) -> list[str]:
    """生成 <sample_input> / <sample_output> 字符串列表。"""
    blocks = []
    for idx, (sin, sout) in enumerate(samples, 1):
        if not preserve_bytes:
            if sin and not sin.endswith("\n"):
                sin += "\n"
            if sout and not sout.endswith("\n"):
                sout += "\n"
        blocks.append(
            f'<sample_input><![CDATA[{sin}]]></sample_input>\n'
            f'<sample_output><![CDATA[{sout}]]></sample_output>\n'
        )
    return blocks


def build_source_field(meta: dict, pid: str) -> str:
    """从 problem.yaml 构造 <source> 内容。"""
    tags = meta.get("tag") or []
    if isinstance(tags, str):
        tags = [tags]
    parts = []
    if meta.get("source"):
        parts.append(str(meta["source"]))
    if tags:
        parts.append(", ".join(tags))
    return "; ".join(parts)


def build_url(url_base: Optional[str], problem_index: int, url_start: int) -> str:
    """
    根据 base + 起始 id 构造 <url>。

    支持的 base 写法:
      http://demo.hustoj.com/problem.php?id=     （末尾以 id= 收尾） → 直接拼数字
      http://demo.hustoj.com/problem.php?id={id} → 替换 {id}
      http://demo.hustoj.com/problem.php          （无 query）       → 补 ?id=N
      http://demo.hustoj.com/problem?pid=        （其他 query）      → 补 &id=N
    """
    if not url_base:
        return ""
    pid_value = str(url_start + problem_index)
    if "{id}" in url_base:
        return url_base.replace("{id}", pid_value)
    # 兼容 base 末尾是 id= 的情况
    if url_base.endswith("id=") or url_base.endswith("pid="):
        return url_base + pid_value
    # 否则按是否有 ? 拼接
    sep = "&" if "?" in url_base else "?"
    # 优先用 pid（与 HUSTOJ 一致），如需 id 可改
    return f"{url_base}{sep}pid={pid_value}"


def build_item(
    problem_dir: Path,
    *,
    style: str = "fields",
    include_solution: bool = True,
    url_base: Optional[str] = None,
    url_start: int = 1000,
    problem_index: int = 0,
    add_test_newline: bool = True,
) -> str:
    """
    为单个题目目录生成 <item> XML 字符串。

    style:
      'fields'  - 把 markdown 按字段拆分（description/input/output/hint/sample_input/sample_output）
      'hustoj'  - 整段 markdown 包 <span class="md"> 塞 description，其他字段空
    """
    meta_path = problem_dir / "problem.yaml"
    md_path = problem_dir / "problem_zh.md"
    if not meta_path.exists():
        raise FileNotFoundError(f"missing problem.yaml in {problem_dir}")
    if not md_path.exists():
        raise FileNotFoundError(f"missing problem_zh.md in {problem_dir}")

    meta = yaml.safe_load(meta_path.read_text(encoding="utf-8"))
    md_text = md_path.read_text(encoding="utf-8")
    md_title, parsed = parse_markdown(md_text)

    title = meta.get("title") or md_title or problem_dir.name
    pid = meta.get("pid", problem_dir.name)

    # 时限 / 内存
    config_path = problem_dir / "testdata" / "config.yaml"
    config = {}
    if config_path.exists():
        try:
            config = yaml.safe_load(config_path.read_text(encoding="utf-8")) or {}
        except yaml.YAMLError:
            config = {}

    time_limit = parse_time(meta.get("time_limit")) or parse_time(config.get("time")) or 1.0
    mem_limit = parse_memory(meta.get("memory_limit")) or parse_memory(config.get("memory")) or 512

    # 测试数据
    testdata_dir = problem_dir / "testdata"
    test_pairs = discover_test_pairs(testdata_dir) if testdata_dir.exists() else []
    test_blocks = build_test_blocks(test_pairs, preserve_bytes=not add_test_newline)

    # 标准解法
    sol_lang, sol_code = discover_solution(problem_dir) if include_solution else (None, None)

    # source / url
    source = build_source_field(meta, pid)
    url = build_url(url_base, problem_index, url_start)

    parts = [f'<item>\n']
    parts.append(f'<title><![CDATA[{title}]]></title>\n')
    if url:
        parts.append(f'<url><![CDATA[{url}]]></url>\n')
    parts.append(f'<time_limit unit="s"><![CDATA[{fmt_time(time_limit)}]]></time_limit>\n')
    parts.append(f'<memory_limit unit="mb"><![CDATA[{mem_limit}]]></memory_limit>\n')

    if style == "hustoj":
        # HUSTOJ 风格：原始 markdown 包 <span class="md">，其他字段空
        parts.append(f'<description><![CDATA[<span class="md">{md_to_html_for_hustoj(md_text)}</span>]]></description>\n')
        # input/output/hint/sample 全空
        parts.append('<input><![CDATA[]]></input>\n')
        parts.append('<output><![CDATA[]]></output>\n')
        # sample_input/output 也空
        # remote_oj/remote_id 占位（与 HUSTOJ 一致）
        parts.append('<remote_oj><![CDATA[]]></remote_oj>\n')
        parts.append('<remote_id><![CDATA[]]></remote_id>\n')
    else:
        # fields 风格：按字段拆分
        desc_html = md_to_html(parsed["description"])
        in_html = md_to_html(parsed["input"])
        out_html = md_to_html(parsed["output"])
        hint_html = md_to_html(parsed["hint"])
        sample_blocks = build_sample_blocks(parsed["samples"], preserve_bytes=not add_test_newline)

        if desc_html:
            parts.append(f'<description><![CDATA[{desc_html}]]></description>\n')
        if in_html:
            parts.append(f'<input><![CDATA[{in_html}]]></input>\n')
        if out_html:
            parts.append(f'<output><![CDATA[{out_html}]]></output>\n')
        parts.extend(sample_blocks)

    parts.extend(test_blocks)

    if style != "hustoj":
        if 'hint_html' in dir() and hint_html:
            parts.append(f'<hint><![CDATA[{hint_html}]]></hint>\n')

    if source:
        parts.append(f'<source><![CDATA[{source}]]></source>\n')

    if style != "hustoj" and include_solution and sol_code:
        parts.append(f'<solution language="{sol_lang}"><![CDATA[{sol_code}]]></solution>\n')

    parts.append('</item>\n')
    return "".join(parts)


def md_to_html_for_hustoj(md_text: str) -> str:
    """
    HUSTOJ 风格：把整段 markdown 转 HTML 后嵌入 <span class="md">。
    与 md_to_html 不同：保留所有 ## 标题、不剥 h1（OJ 前端会自己处理）。
    """
    _md_engine.reset()
    # extensions already configured
    return _md_engine.convert(md_text).strip()


# ============================================================================
# 题目目录发现
# ============================================================================

def discover_problems(source_dir: Path) -> list[Path]:
    """在 source_dir 下找所有含 problem.yaml 的子目录。"""
    return sorted([
        d for d in source_dir.iterdir()
        if d.is_dir() and (d / "problem.yaml").exists()
    ])


# ============================================================================
# 顶层生成
# ============================================================================

def build_fps_xml(
    problem_dirs: list[Path],
    *,
    style: str = "fields",
    version: str = "1.4",
    generator_name: str = "FPS-Converter",
    generator_url: str = FPS_REPO_URL,
    include_solution: bool = True,
    url_base: Optional[str] = None,
    url_start: int = 1000,
    add_test_newline: bool = True,
) -> str:
    """构造完整的 FPS XML 文档。"""
    header = (
        '<?xml version="1.0" encoding="UTF-8"?>\n'
        '<!DOCTYPE fps PUBLIC\n'
        '  "-//freeproblemset//An opensource XML standard for AlgorithmContest Problem Set//EN"\n'
        f'  "{FPS_DTD_URL}" >\n'
        f'<fps version="{version}" url="{FPS_REPO_URL}">\n'
        f'<generator name="{escape(generator_name, {chr(34): "&quot;"})}" '
        f'url="{escape(generator_url, {chr(34): "&quot;"})}"/>\n'
    )
    items = []
    for i, d in enumerate(problem_dirs):
        items.append(build_item(
            d,
            style=style,
            include_solution=include_solution,
            url_base=url_base,
            url_start=url_start,
            problem_index=i,
            add_test_newline=add_test_newline,
        ))
    return header + "".join(items) + "</fps>\n"


# ============================================================================
# CLI
# ============================================================================

def prompt(msg: str, default: Optional[str] = None) -> str:
    """交互式提示输入，回车取默认值。"""
    if default:
        s = input(f"{msg} [{default}]: ").strip()
        return s or default
    return input(f"{msg}: ").strip()


def main(argv: Optional[list[str]] = None) -> int:
    parser = argparse.ArgumentParser(
        prog="convert_to_fps",
        description="把 hydro 格式题目目录批量转 FPS (freeproblemset) XML。",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""\
示例:
  # 默认 (fields 风格，输出到 <source_dir>/fps/)
  python3 convert_hydro_to_fps.py ./GESP202609-C++编程题

  # HUSTOJ 风格（description 装整段 markdown，前端 JS 渲染）
  python3 convert_hydro_to_fps.py ./problems --style hustoj

  # 指定输出目录 + 给所有题目加上 OJ url
  python3 convert_hydro_to_fps.py ./problems ./out \\
      --url-base "http://demo.hustoj.com/problem.php?id=" --url-start 9569

  # 交互模式
  python3 convert_to_fps.py --interactive
""",
    )
    parser.add_argument("source_dir", nargs="?", help="题目根目录（含子目录，每个子目录一题）")
    parser.add_argument("output_dir", nargs="?", help="输出目录（默认 <source_dir>/fps/）")
    parser.add_argument("-i", "--interactive", action="store_true",
                        help="交互模式：依次提示输入路径与参数")
    parser.add_argument("--style", choices=["fields", "hustoj"], default="fields",
                        help="输出风格：fields=按字段拆分；hustoj=整段 markdown 塞 description（默认 fields）")
    parser.add_argument("--version", default="1.4",
                        help="FPS version 属性（默认 1.4，可选 1.6）")
    parser.add_argument("--generator-name", default="FPS-Converter",
                        help="<generator name> 值")
    parser.add_argument("--generator-url", default=FPS_REPO_URL,
                        help="<generator url> 值")
    parser.add_argument("--no-solution", action="store_true",
                        help="不导出标程（与 hustoj 导出行为一致）")
    parser.add_argument("--url-base", default=None,
                        help="OJ 题目链接前缀，如 http://demo.hustoj.com/problem.php?id=")
    parser.add_argument("--url-start", type=int, default=1000,
                        help="OJ 起始题目编号，配合 --url-base 使用（默认 1000）")
    parser.add_argument("--no-test-newline", action="store_true",
                        help="测试数据保持原始字节，不强制补末尾换行")
    parser.add_argument("--single", action="store_true",
                        help="同时输出每题单独的 XML 文件（默认只输出合并文件）")
    parser.add_argument("--merged-name", default="all.xml",
                        help="合并文件名（默认 all.xml）")
    parser.add_argument("--quiet", action="store_true", help="安静模式：只输出错误信息")

    args = parser.parse_args(argv)

    # ---------- 参数解析 ----------
    if args.interactive:
        print("=== FPS 转换器 交互模式 ===\n")
        args.source_dir = prompt("题目源目录 (含 problem.yaml 的子目录集合)", args.source_dir)
        args.output_dir = prompt("输出目录", args.output_dir or f"{args.source_dir}/fps")
        if not args.quiet:
            print(f"  风格: {args.style}")
            print(f"  FPS version: {args.version}")
            print(f"  包含标程: {not args.no_solution}")
            if args.url_base:
                print(f"  OJ url: {args.url_base} (起始 {args.url_start})")
        print()

    if not args.source_dir:
        parser.error("缺少 source_dir（或用 -i 进入交互模式）")

    source_dir = Path(args.source_dir).expanduser().resolve()
    if not source_dir.is_dir():
        print(f"❌ 源目录不存在: {source_dir}", file=sys.stderr)
        return 2

    output_dir = Path(args.output_dir).expanduser().resolve() if args.output_dir \
                 else source_dir / "fps"

    # ---------- 发现题目 ----------
    problems = discover_problems(source_dir)
    if not problems:
        print(f"⚠ {source_dir} 下未找到任何题目（需 problem.yaml）", file=sys.stderr)
        return 1

    if not args.quiet:
        print(f"📂 源目录: {source_dir}")
        print(f"📁 输出目录: {output_dir}")
        print(f"🎨 风格: {args.style}  |  FPS v{args.version}  |  题目数: {len(problems)}")
        print(f"🔧 包含标程: {not args.no_solution}  |  补 test 换行: {not args.no_test_newline}")
        if args.url_base:
            print(f"🔗 OJ url: {args.url_base} (起始 {args.url_start})")
        print()

    # ---------- 生成 XML ----------
    output_dir.mkdir(parents=True, exist_ok=True)

    merged_xml = build_fps_xml(
        problems,
        style=args.style,
        version=args.version,
        generator_name=args.generator_name,
        generator_url=args.generator_url,
        include_solution=not args.no_solution,
        url_base=args.url_base,
        url_start=args.url_start,
        add_test_newline=not args.no_test_newline,
    )

    # 合并文件
    merged_path = output_dir / args.merged_name
    merged_path.write_text(merged_xml, encoding="utf-8")

    # 单题文件（可选）
    if args.single:
        for i, d in enumerate(problems):
            single_xml = build_fps_xml(
                [d],
                style=args.style,
                version=args.version,
                generator_name=args.generator_name,
                generator_url=args.generator_url,
                include_solution=not args.no_solution,
                url_base=args.url_base,
                url_start=args.url_start,
                add_test_newline=not args.no_test_newline,
            )
            (output_dir / f"{d.name}.xml").write_text(single_xml, encoding="utf-8")

    # ---------- 报告 ----------
    if not args.quiet:
        print(f"题目列表:")
        for i, d in enumerate(problems, 1):
            meta = yaml.safe_load((d / "problem.yaml").read_text(encoding="utf-8"))
            pid = meta.get("pid", d.name)
            title = meta.get("title", d.name)
            print(f"  {i:2d}. {pid:20s} {title}")

        print(f"\n✅ 已生成: {merged_path}")
        print(f"   ({len(merged_xml)/1024:.1f} KB, {len(problems)} 题)")
        if args.single:
            n_single = len(list(output_dir.glob("*.xml"))) - 1  # 减去 merged
            print(f"   + {n_single} 个单题文件")

    return 0


if __name__ == "__main__":
    sys.exit(main())
