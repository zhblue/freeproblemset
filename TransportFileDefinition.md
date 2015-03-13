# a transport file format to store all information about problems

# Introduction #

To transport data from one OJ to another, a transport file format is needed.


# Details #

Format should be defined cross-platform, so we consider XML is obviously a good choice:
  * XML, define a new XML format
  * RSS2.0, reuse RSS 2.0 to synchronize problems between OJs someday :)
  * SQL, problem only, without test data, not good enough
  * CSV, standard text file , no better than XML when trying make extends
  * SPJ, special judger is running with 3 parameters, input-file output-file user-output

Example of A+B problem in FPS format
```

<?xml version="1.0" encoding="UTF-8"?>
 <fps version="1.2">
   <item>
     <title>A+B</title>
     <time_limit unit="s">1</time_limit>
     <memory_limit unit="mb">64</memory_limit>
     <img>
        <src>JudgeOnline/upload/201009/Screenshot.png</src>
        <base64>iVBORw0K[BASE64_ENCODED_IMAGE_BINARY]RU5ErkJggg==</base64>
     </img>
     <description><![CDATA[print the sum of two integer<img src=JudgeOnline/upload/201009/Screenshot.png>]]></description>
     <input>two integer a and b</input>
     <output>the sum of a and b</output>
     <sample_input>1 2</sample_input>
     <sample_output>3</sample_output>
     <test_input><![CDATA[2 3]]></test_input>
     <test_output><![CDATA[5]]></test_output>
     <hint>use scanf and printf in stdio.h</hint>
     <source>http://code.google.com/p/freeproblemset</source>
     <solution language="C++"><![CDATA[
             #include <iostream>

             using namespace std;

             int  main()

             {

             int a,b;

             cin >> a >> b;

             cout << a+b << endl;

             return 0;

             }
     ]]></solution>
     <spj language="C"></spj>
   </item>
 </fps>

```

[DTD](http://code.google.com/p/freeproblemset/source/browse/trunk/fps.current.dtd)