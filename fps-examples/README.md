部分题面导入后题面是markdown的，需要增加[md] [/md] 或者 
```
  <span class="md" >
   .....
  </span>
```

如果有新搭建的OJ，想测试性能，可以导入这个文件：PerformanceTest.xml

最后全部判完，这个AWT越小，说明判题速度越快

![image](https://github.com/zhblue/freeproblemset/assets/3926566/788482f8-cb30-4e2c-9913-f853d103555c)







fps-my-1000-1128.xml  用于演示service.php的使用。
先复制include/remote_service.php 为 include/remote_my.php
修改其中末尾的
```
$remote_oj="my";   // problem表的remote_oj字段设my，本文件复制一份改名成remote_my.php，并在../remote.php中增加扫描项。
$remote_site="http://my.hustoj.com/service.php";  // 需要远程服务器运行开启service_port的最新版本HUSTOJ
$remote_user='service';    //  远程系统具有 service_port 的可用状态(正常登录、未到期，有权限)账号
$remote_pass='123456';      //账号、密码 注意保存，更新时可能覆盖此文件
```
修改remote.php
增加项
```
$remote_ojs=array(
                 "my"       // "pku","hdu"     //使用一本通启蒙设为："bas"
        );
```
和
```
  "my" => "http://my.hustoj.com/",
```
最后启用db_info.inc.php中的
`$OJ_REMOTE_JUDGE=true;`

并导入本文件。
