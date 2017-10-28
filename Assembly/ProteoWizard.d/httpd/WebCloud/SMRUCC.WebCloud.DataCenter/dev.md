# MySQL development docs
Mysql database field attributes notes:

> AI: Auto Increment; B: Binary; NN: Not Null; PK: Primary Key; UQ: Unique; UN: Unsigned; ZF: Zero Fill

## app
The analysis application that running the task

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|name|VarChar (128)|``NN``||
|description|Text||功能的详细描述|
|catagory|VarChar (45)||功能分类|

```SQL
CREATE TABLE `app` (
  `uid` int(11) NOT NULL,
  `name` varchar(128) NOT NULL,
  `description` longtext COMMENT '功能的详细描述',
  `catagory` varchar(45) DEFAULT NULL COMMENT '功能分类',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='The analysis application that running the task';
```



## subscription
向订阅了网站更新的用户发送产品的更新信息

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``AI``, ``NN``||
|email|VarChar (128)|``NN``||
|hash|VarChar (64)|``NN``||
|app|Int64 (11)|``NN``||
|active|Int64 (11)|``NN``|1(active) OR 0(inactive)|
|add_time|DateTime|``NN``||

```SQL
CREATE TABLE `subscription` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(128) NOT NULL,
  `hash` varchar(64) NOT NULL,
  `app` int(11) NOT NULL,
  `active` int(11) NOT NULL DEFAULT '0' COMMENT '1(active) OR 0(inactive)',
  `add_time` datetime NOT NULL,
  PRIMARY KEY (`email`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  KEY `fk_subscription_app1_idx` (`app`),
  CONSTRAINT `fk_subscription_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='向订阅了网站更新的用户发送产品的更新信息';
```



## sys_config
系统设置

|field|type|attributes|description|
|-----|----|----------|-----------|
|variable|VarChar (128)|``NN``||
|value|VarChar (128)|||
|set_time|DateTime|``NN``||
|set_by|VarChar (128)|||

```SQL
CREATE TABLE `sys_config` (
  `variable` varchar(128) NOT NULL,
  `value` varchar(128) DEFAULT NULL,
  `set_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `set_by` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`variable`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统设置';
```



## sys_updates
网站更新记录

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``AI``, ``NN``||
|date|DateTime|``NN``||
|title|VarChar (45)|``NN``||
|details|Text|``NN``||
|app|Int64 (11)|``NN``|如果这个字段不为-1，则表示更新的内容为某一个app的内容更新|

```SQL
CREATE TABLE `sys_updates` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  `title` varchar(45) NOT NULL,
  `details` mediumtext NOT NULL,
  `app` int(11) NOT NULL DEFAULT '-1' COMMENT '如果这个字段不为-1，则表示更新的内容为某一个app的内容更新',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='网站更新记录';
```



## task_errors
Task executing errors log

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``NN``||
|app|Int64 (11)|``NN``|The task app name|
|task|Int64 (11)|``NN``|The task uid|
|exception|Text|``NN``|The exception message|
|type|VarChar (45)|``NN``|GetType.ToString|
|stack-trace|VarChar (45)|``NN``||
|solved|Int64 (11)|``NN``|这个bug是否已经解决了？ 默认是0未解决，1为已经解决了|

```SQL
CREATE TABLE `task_errors` (
  `uid` int(11) NOT NULL,
  `app` int(11) NOT NULL COMMENT 'The task app name',
  `task` int(11) NOT NULL COMMENT 'The task uid',
  `exception` longtext NOT NULL COMMENT 'The exception message',
  `type` varchar(45) NOT NULL COMMENT 'GetType.ToString',
  `stack-trace` varchar(45) NOT NULL,
  `solved` int(11) NOT NULL DEFAULT '0' COMMENT '这个bug是否已经解决了？ 默认是0未解决，1为已经解决了',
  PRIMARY KEY (`uid`,`app`),
  KEY `fk_task_errors_app1_idx` (`app`),
  CONSTRAINT `error_task` FOREIGN KEY (`app`) REFERENCES `task_pool` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_task_errors_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Task executing errors log';
```



## task_pool
这个数据表之中只存放已经完成的用户任务信息

|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``AI``, ``NN``||
|md5|VarChar (32)|``NN``|用户查询任务状态结果所使用的唯一标识符字符串|
|workspace|Text||保存临时上传数据以及结果报告文件的工作区文件夹|
|time_create|DateTime||这个用户任务所创建的时间|
|time_complete|DateTime||这个用户任务所完成的时间，只有用户的任务完成了之后（无论是否出现错误），这个属性才会被赋值。这个属性值也是计算工作区的临时数据的清除时间锁需要的，一般是24小时之后任务才会过期，工作区的临时数据才会被自动清除|
|result_url|Text||结果页面的url|
|email|VarChar (45)||任务完成之后通知的目标对象的e-mail,如果不存在，则不发送email|
|title|VarChar (128)||任务的标题（可选）|
|description|Text||任务的描述(可选)|
|status|Int64 (11)||任务的结果状态<br /><br />-100 任务执行失败<br />1 任务成功执行完毕<br />0 任务未执行或者执行中未完毕|
|app|Int64 (11)|``NN``|The task app id|
|parameters|Text|``NN``|使用json保存着当前的这个任务对象的所有的构造函数所需要的参数信息|

```SQL
CREATE TABLE `task_pool` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `md5` varchar(32) NOT NULL COMMENT '用户查询任务状态结果所使用的唯一标识符字符串',
  `workspace` mediumtext COMMENT '保存临时上传数据以及结果报告文件的工作区文件夹',
  `time_create` datetime DEFAULT NULL COMMENT '这个用户任务所创建的时间',
  `time_complete` datetime DEFAULT NULL COMMENT '这个用户任务所完成的时间，只有用户的任务完成了之后（无论是否出现错误），这个属性才会被赋值。这个属性值也是计算工作区的临时数据的清除时间锁需要的，一般是24小时之后任务才会过期，工作区的临时数据才会被自动清除',
  `result_url` mediumtext COMMENT '结果页面的url',
  `email` varchar(45) DEFAULT NULL COMMENT '任务完成之后通知的目标对象的e-mail,如果不存在，则不发送email',
  `title` varchar(128) DEFAULT NULL COMMENT '任务的标题（可选）',
  `description` mediumtext COMMENT '任务的描述(可选)',
  `status` int(11) DEFAULT NULL COMMENT '任务的结果状态\n\n-100 任务执行失败\n1 任务成功执行完毕\n0 任务未执行或者执行中未完毕',
  `app` int(11) NOT NULL COMMENT 'The task app id',
  `parameters` longtext NOT NULL COMMENT '使用json保存着当前的这个任务对象的所有的构造函数所需要的参数信息',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `md5_UNIQUE` (`md5`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  KEY `fk_task_pool_app1_idx` (`app`),
  CONSTRAINT `fk_task_pool_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个数据表之中只存放已经完成的用户任务信息';
```



## visitor_stat


|field|type|attributes|description|
|-----|----|----------|-----------|
|uid|Int64 (11)|``AI``, ``NN``||
|time|DateTime|``NN``||
|ip|VarChar (45)|``NN``||
|url|Text|``NN``|Url that going to visit this web site|
|success|Int64 (11)|``NN``||
|method|VarChar (45)||GET/POST/PUT.....|
|ua|VarChar (1024)||User agent|
|ref|Text||reference url, Referer|
|data|Text||additional data notes|
|app|Int64 (11)|``NN``|用户所访问的url所属的app的编号|

```SQL
CREATE TABLE `visitor_stat` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `time` datetime NOT NULL,
  `ip` varchar(45) NOT NULL,
  `url` tinytext NOT NULL COMMENT 'Url that going to visit this web site',
  `success` int(11) NOT NULL,
  `method` varchar(45) DEFAULT NULL COMMENT 'GET/POST/PUT.....',
  `ua` varchar(1024) DEFAULT NULL COMMENT 'User agent',
  `ref` mediumtext COMMENT 'reference url, Referer',
  `data` mediumtext COMMENT 'additional data notes',
  `app` int(11) NOT NULL COMMENT '用户所访问的url所属的app的编号',
  PRIMARY KEY (`ip`,`time`,`app`),
  UNIQUE KEY `uid_UNIQUE` (`u

