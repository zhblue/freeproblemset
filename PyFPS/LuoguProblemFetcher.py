from FpsGenTool import FpsGenTool
import requests
from bs4 import BeautifulSoup
import base64
from selenium import webdriver
from selenium.webdriver.common.by import By
import re 
import urllib.parse
import json 

# https://www.luogu.com.cn/problem/P1043

class LuoguProblemFetcher:
    def __init__(self):
        self.base_headers={'user-agent':'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36'}

        options = webdriver.ChromeOptions()
        options.add_argument('--headless')
        self.browser = webdriver.Chrome(options=options)

    def _gather_img(self, parent, img_list):
        for img in parent.find_all('img', recursive=True):
            img_list.append(img.get('src'))

    def fetch(self,id):
        problem_url='https://www.luogu.com.cn/problem/%s'%id
        self.browser.get(problem_url)
        #self.browser.implicitly_wait(3)
        # 展开标签
        tag_btn=self.browser.find_element(By.CSS_SELECTOR, '.expand-tip')
        tag_btn.click()
        html=self.browser.page_source

        html=html.replace('class="katex"','class="katex katex-no-js"')

        json_data=re.search('JSON.parse\\(decodeURIComponent\\("(.+)"',html).group(1)
        json_data=json.loads(urllib.parse.unquote(json_data))
        limits=json_data['currentData']['problem']['limits']

        soup=BeautifulSoup(html,features='lxml')
        # 标题
        title=soup.find('head').find('title').string[len(id):]
        title=title[:title.find('- 洛谷')].strip() + '(洛谷 - %s)' % id

        # 核心区域
        pdetail=soup.find(class_='problem-card')

        time_limit=max(limits['time'])//1000
        memory_limit=max(limits['memory'])//1024
        description=None
        input_=None 
        output=None
        sample_input=None
        sample_output=None
        test_input=[]
        test_output=[]
        hint=None # 提示
        source=[] # 来源
        img_list=[]#涉及图片

        # 标签
        tags_wrap=soup.find('div',class_='tags-wrap',recursive=True)
        if tags_wrap:
            for tag_div in soup.find('div',class_='tags-wrap').find_all('a'):
                source.append(tag_div.find('span').string)
            source=' '.join(source)
        
        # 扫描problem-card第2个div
        key=''
        for item in pdetail.find_all('div')[1].contents:
            if item.name not in ['h2','div']:
                continue
            if item.name=='h2':
                key=item.string.strip()
            elif item.name=='div':
                if key=='题目描述':
                    description=str(item)
                    self._gather_img(item,img_list)
                    #description='[md]'+json_data['currentData']['problem']['description']+'[/md]'
                elif key=='输入格式':
                    input_=str(item)
                    self._gather_img(item,img_list)
                    #input_='[md]'+json_data['currentData']['problem']['inputFormat']+'[/md]'
                elif key=='输出格式':
                    output=str(item)
                    self._gather_img(item,img_list)
                    #output='[md]'+json_data['currentData']['problem']['outputFormat']+'[/md]'
                elif key=='说明/提示':
                    hint=str(item)
                    self._gather_img(item,img_list)
                    #hint='[md]'+json_data['currentData']['problem']['hint']+'[/md]'
                elif key=='输入输出样例':
                    sample_input=item.find(class_='input').find('pre').string
                    sample_output=item.find(class_='output').find('pre').string
                    test_input.append(sample_input)
                    test_output.append(sample_output)
                    self._gather_img(item,img_list)
                key=''

        # 图片内联
        for i in range(len(img_list)):
            url=img_list[i] 
            if not url.startswith('http'):
                url='https://cdn.luogu.com.cn/%s'%img_list[i]
            img_content=requests.get(url).content
            img_b64=base64.b64encode(img_content).decode('utf-8')
            img_list[i]=(img_list[i],img_b64)

        return {
            'id': id,
            'title': title,
            'time_limit':time_limit,
            'memory_limit':memory_limit,
            'description': description,
            'input_': input_,
            'output': output,
            'sample_input': sample_input,
            'sample_output': sample_output,
            'test_input': test_input,
            'test_output': test_output,
            'source': source,
            'hint': hint,
            'url': problem_url,
            'img':img_list,
        }
if __name__=='__main__':
    fetcher=LuoguProblemFetcher()
    problem=fetcher.fetch('P1371')
    #print(problem)
    hint_href="<a href='%s'>原题链接</a>"%problem['url']
    
    tool=FpsGenTool()
    tool.add_problem(problem['title'],
                    problem['time_limit'],
                    problem['memory_limit'],
                    problem['description'],
                    input_=problem['input_'],
                    output=problem['output'],
                    sample_input=problem['sample_input'],
                    sample_output=problem['sample_output'],
                    test_input=problem['test_input'],
                    test_output=problem['test_output'],
                    hint=problem['hint']+'<br/>'+hint_href if problem['hint'] else hint_href,
                    source=problem['source'],
                    img=problem['img'])
    tool.dump('luogu.xml')
    print(tool)