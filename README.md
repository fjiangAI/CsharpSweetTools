# CsharpSweetTools
这是C#的一些常用的小工具类
这是在日常的编码中的自己写的一些C#通用工具类，将会不断的增加。下面附上介绍，按照字母顺序排序：
## DataBaseOpt
这是一个重量级的数据库操作类，提供了包括事务在内的一系列数据库操作方法。更提供了增删查改的操作，以及对于按行读和按表读等多种读写手段。
## FileHelper
这是一个文件文本的输入输出类，仿照的是Python的风格。目前的缺点在于不能按行读，按行写。
## JsonHelper
这是一个Json实体转换类，但是需要依靠Newtonsoft.Json库，速度较快。
## JsonToObject
这是也是一个Json实体转换类，使用的是内置的转换，速度较慢。
## mailProcess
这是一个邮件处理类，专门用于发送邮件的。原本是一个静态工具类，现在改写成实体对象类，更易配置和频繁使用。
## PythonOperator
这是一个C#操纵Python的工具类，其实质是用来执行命令行的，因此不仅可以用来执行Python,还可以执行其他使用命令行模式启动的程序。
## WordCount
这是一个统计词频的工具类，专门用来统计词频。
## XmlSerializer
这是Xml序列化帮助类，用来序列化Xml文件的。