using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EasyFPSViewer.Core
{
    public static class I18N
    {
        private static readonly string LanguageString = System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToLower();
        private static Dictionary<string, string> _languageDic = new Dictionary<string, string>();
        static I18N()
        {
            string languageData;
            if(LanguageString.Contains("chinese"))
            {
                languageData = Properties.Resources.Zh_Cn;
            }
            else
            {
                languageData = "";
            }

            InitLanguageDic(languageData);
        }

        private static void InitLanguageDic(string langData)
        {
            string[] rules = langData.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            foreach(string rule in rules)
            {
                if(rule.StartsWith("##"))
                {
                    continue;
                }

                string[] split = rule.Split('=');
                if(split.Length == 2)
                {
                    _languageDic[split[0]] = split[1];
                }
            }
        }

        public static string GetStr(string str)
        {
            if (_languageDic.ContainsKey(str))
            {
                return _languageDic[str];
            }

            return str;
        }

        public static void InitControl(Control control)
        {
            control.Text = GetStr(control.Text);
            foreach (Control child in control.Controls)
            {
                if(child is ListView)
                {
                    InitListView(child as ListView);
                }
                else if(child is MenuStrip)
                {
                    InitMenuStrip(child as MenuStrip);
                }

                InitControl(child);
            }
        }

        public static void InitMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.Text = GetStr(menuStrip.Text);
            foreach(ToolStripMenuItem child in menuStrip.Items)
            {
                if(child is ToolStripMenuItem)
                {
                    InitToolStripMenuItem(child);
                }
            }
        }

        public static void InitToolStripMenuItem(ToolStripMenuItem stripMenuItem)
        {
            stripMenuItem.Text = GetStr(stripMenuItem.Text);
            foreach(ToolStripItem child in stripMenuItem.DropDownItems)
            {
                if(child is ToolStripMenuItem)
                {
                    child.Text = GetStr(child.Text);
                    InitToolStripMenuItem(child as ToolStripMenuItem);
                }
            }
        }

        public static void InitContextMenuStrip(ContextMenuStrip contextMenuStrip)
        {
            contextMenuStrip.Text = GetStr(contextMenuStrip.Text);
            foreach(ToolStripItem child in contextMenuStrip.Items)
            {
                if(child is ToolStripMenuItem)
                {
                    child.Text = GetStr(child.Text);
                    InitToolStripMenuItem(child as ToolStripMenuItem);
                }
            }
        }


        public static void InitListView(ListView listView)
        {
            foreach(ColumnHeader col in listView.Columns)
            {
                col.Text = GetStr(col.Text);
            }
        }
    }
}
