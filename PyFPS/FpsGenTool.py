from lxml import etree
import os

class FpsGenTool:
    def __init__(self):
        #self.tree=etree.parse(os.path.dirname(__file__)+'fps-framework.xml')
        self.problem_cnt=0
        self.tree=etree.parse('fps-framework.xml')
        # item
        self.tpl_item=self.tree.getroot().find('item')
        self.tree.getroot().remove(self.tpl_item)
        # test_input
        self.tpl_test_input=self.tpl_item.find('test_input')
        self.tpl_item.remove(self.tpl_test_input)
        # test_output
        self.tpl_test_output=self.tpl_item.find('test_output')
        self.tpl_item.remove(self.tpl_test_output)
        # solution
        self.tpl_solution=self.tpl_item.find('solution')
        self.tpl_item.remove(self.tpl_solution)
        
        self.tpl_item_str=etree.tostring(self.tpl_item)
        self.tpl_test_input_str=etree.tostring(self.tpl_test_input)
        self.tpl_test_output_str=etree.tostring(self.tpl_test_output)
        self.tpl_solution_str=etree.tostring(self.tpl_solution)
    def add_problem(self,
                    title,
                    time_limit,
                    memory_limit,
                    description,
                    input_=None,
                    output=None,
                    sample_input=None,
                    sample_output=None,
                    test_input=None,
                    test_output=None,
                    hint=None,
                    source=None,
                    solution=None,
                    img=None):
        new_item=etree.fromstring(self.tpl_item_str)
        new_item.find('title').text=title
        new_item.find('time_limit').text=str(time_limit)
        new_item.find('memory_limit').text=str(memory_limit)
        new_item.find('description').text=description
        if input_:
            new_item.find('input').text=input_
        if output:
            new_item.find('output').text=output
        if sample_input: # HustOj只支持单sample
            new_item.find('sample_input').text=sample_input
        if sample_output:
            new_item.find('sample_output').text=sample_output
        if test_input:
            for i in range(len(test_input)):
                elem=etree.fromstring(self.tpl_test_input_str)
                elem.set('name','test%d'%i)
                elem.text=test_input[i]
                new_item.append(elem)
        if test_output:
            for i in range(len(test_output)):
                elem=etree.fromstring(self.tpl_test_output_str)
                elem.set('name','test%d'%i)
                elem.text=test_output[i]
                new_item.append(elem)
        if hint:
            new_item.find('hint').text=hint
        if source: # 题目来源
            new_item.find('source').text=source
        if solution: # 标准程序
            elem=etree.fromstring(self.tpl_solution_str)
            elem.text=solution
            new_item.append(elem)
        if img: # 图片内联替换
            for img_item in img:
                elem=etree.fromstring('''<img><src/><base64/></img>''')
                elem.find('src').text=img_item[0]
                elem.find('base64').text=img_item[1]
                new_item.append(elem)
        self.tree.getroot().append(new_item)
        self.problem_cnt+=1
    def count(self):
        return self.problem_cnt
    def dump(self,filename):
        self.tree.write(filename,pretty_print=True)
    def __str__(self):
        return etree.tostring(self.tree,pretty_print=True).decode('utf-8')

if __name__ == '__main__':
    tool=FpsGenTool()
    tool.add_problem('FPS验证导入功能',
                    '1',
                    '32',
                    '<p>这是题目描述</p>',
                    '输入2个整数a和b',
                    '输出a和b的和',
                    '1 2',
                    '3',
                    ['4 5'],
                    ['9'],
                    '注意整数是64位的',
                    '采集自东方博宜',
                    )
    tool.dump('result.xml')
    print(tool)