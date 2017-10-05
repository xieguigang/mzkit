CREATE DATABASE  IF NOT EXISTS `smrucc-cloud` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `smrucc-cloud`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: smrucc-cloud
-- ------------------------------------------------------
-- Server version	5.7.18-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `app`
--

DROP TABLE IF EXISTS `app`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `app` (
  `uid` int(11) NOT NULL,
  `name` varchar(128) NOT NULL,
  `description` longtext COMMENT '功能的详细描述',
  `catagory` varchar(45) DEFAULT NULL COMMENT '功能分类',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='The analysis application that running the task';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `subscription`
--

DROP TABLE IF EXISTS `subscription`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_config`
--

DROP TABLE IF EXISTS `sys_config`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_config` (
  `variable` varchar(128) NOT NULL,
  `value` varchar(128) DEFAULT NULL,
  `set_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `set_by` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`variable`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统设置';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_updates`
--

DROP TABLE IF EXISTS `sys_updates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_updates` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  `title` varchar(45) NOT NULL,
  `details` mediumtext NOT NULL,
  `app` int(11) NOT NULL DEFAULT '-1' COMMENT '如果这个字段不为-1，则表示更新的内容为某一个app的内容更新',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='网站更新记录';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_errors`
--

DROP TABLE IF EXISTS `task_errors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `task_pool`
--

DROP TABLE IF EXISTS `task_pool`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `visitor_stat`
--

DROP TABLE IF EXISTS `visitor_stat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  KEY `fk_visitor_stat_app1_idx` (`app`),
  CONSTRAINT `fk_visitor_stat_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'smrucc-cloud'
--

--
-- Dumping routines for database 'smrucc-cloud'
--
/*!50003 DROP FUNCTION IF EXISTS `task_expired` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ALLOW_INVALID_DATES,ERROR_FOR_DIVISION_BY_ZERO,TRADITIONAL,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `task_expired`(time_complete datetime) RETURNS tinyint(1)
BEGIN

   /* 
    * 用户的任务执行结果数据只保存24个小时，则在这个函数之中需要进行判断的是
    * 任务的完成时间和现在的时间差是否大于24个小时？ 
    *
    * 如果是，则说明任务已经过期了，则会返回true删除数据
    * 如果不是，则返回false
    */
   DECLARE val integer;

   SET val = TIMESTAMPDIFF(HOUR, time_complete, now()) ;
   RETURN NOT val <= 24;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-09-10  3:50:20
