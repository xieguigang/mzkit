#Region "Microsoft.VisualBasic::3a9f9cdfb080ccf3dd34154891a6b5e4, ..\httpd\WebCloud\VisitStat\visitor_stat.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @1/19/2016 4:41:44 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes


''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `visitor_stat`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `visitor_stat` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `time` datetime NOT NULL,
'''   `ip` varchar(45) NOT NULL,
'''   `url` tinytext,
'''   `success` int(11) DEFAULT NULL,
'''   `method` varchar(45) DEFAULT NULL,
'''   `ua` varchar(1024) DEFAULT NULL,
'''   PRIMARY KEY (`ip`,`time`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping data for table `visitor_stat`
''' --
''' 
''' LOCK TABLES `visitor_stat` WRITE;
''' /*!40000 ALTER TABLE `visitor_stat` DISABLE KEYS */;
''' /*!40000 ALTER TABLE `visitor_stat` ENABLE KEYS */;
''' UNLOCK TABLES;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2016-01-19 16:36:03
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("visitor_stat")>
Public Class visitor_stat: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property uid As Long
    <DatabaseField("time"), PrimaryKey, NotNull, DataType(MySqlDbType.DateTime)> Public Property time As Date
    <DatabaseField("ip"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property ip As String
    <DatabaseField("url"), DataType(MySqlDbType.Text)> Public Property url As String
    <DatabaseField("success"), DataType(MySqlDbType.Int64, "11")> Public Property success As Long
    <DatabaseField("method"), DataType(MySqlDbType.VarChar, "45")> Public Property method As String
    <DatabaseField("ua"), DataType(MySqlDbType.VarChar, "1024")> Public Property ua As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `visitor_stat` (`time`, `ip`, `url`, `success`, `method`, `ua`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `visitor_stat` (`time`, `ip`, `url`, `success`, `method`, `ua`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `visitor_stat` WHERE `ip`='{0}' and `time`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `visitor_stat` SET `uid`='{0}', `time`='{1}', `ip`='{2}', `url`='{3}', `success`='{4}', `method`='{5}', `ua`='{6}' WHERE `ip`='{7}' and `time`='{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ip, DataType.ToMySqlDateTimeString(time))
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DataType.ToMySqlDateTimeString(time), ip, url, success, method, ua)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DataType.ToMySqlDateTimeString(time), ip, url, success, method, ua)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, DataType.ToMySqlDateTimeString(time), ip, url, success, method, ua, ip, DataType.ToMySqlDateTimeString(time))
    End Function
#End Region
End Class
