from FpsGenTool import FpsGenTool
import requests
from bs4 import BeautifulSoup
from LuoguProblemFetcher import  LuoguProblemFetcher
import json 
import time 

# problem下载器
fetcher=LuoguProblemFetcher()
tool=FpsGenTool()

# 生成题单JSON
problemset=[]

# index翻页
for page in range(39,40): # 1,162
    url='https://www.luogu.com.cn/problem/list?page=%d'%page
    index_html=requests.get(url,headers={'user-agent':'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36'}).text 
    soup=BeautifulSoup(index_html,features='lxml')
    problem_table=soup.find('div',id='app')
    for problem_li in problem_table.find('ul',recursive=True).find_all('li'):
        problem_id=problem_li.find('a').get('href')
        while True: # 洛谷有限速，失败了重试就好
            try:
                problem=fetcher.fetch(problem_id)
                print(page,problem_id)
            except:
                print('失败', problem_id)
                tool.dump('luogu.xml')
                time.sleep(15)
            else:
                break
        hint_href="<a href='%s'>原题链接</a>"%problem['url']
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
        problemset.append([problem_id,problem['title']])
        time.sleep(1)
tool.dump('luogu.xml')