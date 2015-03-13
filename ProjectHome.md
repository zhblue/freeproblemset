如果您愿意分享原创题目，请联系我，或直接将导出为fps格式的题目发我，我将上传到Downloads中供ACMer们下载使用。newsclan(a)gmail.com

  * 上传题目前请做好测试，用hustoj的liveCD导入，正常AC的再上传。
  * Downloads中已有400余题，下载时请注意标签，避免重复导入。最好先在HUSTOJ-LiveCD做测试。
  * 新增Java编写的FPS文件题目查看器，需要安装JRE，Windows中暂不支持中文路径。

There is many free ACM/ICPC Online Judge System available.
But not so much problemset available.

This project is trying to provide free problemset for managers of ACM/ICPC Online Jugde.
```
FPS is definied as:
1.problem
2.test data
3.special judger(optional)
4.standard AC answer(optional)
```
the latest defination can be view and discussed on [TransportFileDefinition](TransportFileDefinition.md)

For now it's designed for hustoj (on googlecode),but it should be usable for other OJs.

目前有许多开源的ACM/ICPC Online Judge 系统，都各有特色，他们都有自己独特的体系结构。

但似乎比较难找的是相应的练习题目，各大学校的OJ都对自己的题目数据严加保密，不轻易与人分享。

出题是很辛苦的事情，抄题也是如此，而且抄来的题目数据很难制作，标程就更难寻找，特别裁判更加难上加难。

这个项目希望建立一个交流平台，使得学校之间交流题目更加容易。

所以这个项目的目标是，建立一种通用题目交换格式，基于XML来实现。
```
其中包含了：
1、题目
2、图片(可选)
   =1.0 URL
   >=1.1 内置
3、测试数据
4、裁判(可选)
5、标程(可选)
```

最新格式可以在[TransportFileDefinition](TransportFileDefinition.md)中看到，欢迎讨论和建议。

因为个人的原因，以[HUSTOJ](HUSTOJ.md)为原型系统，建立导入/导出功能，因此本项目的主要工作是制定数据格式，相应的代码工作在[HUSTOJ](HUSTOJ.md)中完成。

如果您的OJ系统不是[HUSTOJ](HUSTOJ.md),也没关系，只要您有修改源码的权限，增加基于XML的导入导出功能是理论上一定可以实现的。

一旦您的系统有了导入功能，您就可以从本站下载题目数据；一旦您有导出功能，您就可以导出题目与兄弟院校进行交流，如果您愿意，当然更希望您能参与本项目，将导出的数据共享出来，这样我们就可以共同创建世界上第一个开源OJ数据中心。

目前[HUSTOJ](HUSTOJ.md)已经可以进行题目和测试数据的导出。

本项目期待您的加入…… 联系我newsclan=Gmail
