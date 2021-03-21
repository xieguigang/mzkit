#Region "Microsoft.VisualBasic::473502dc25680b2249108ceb59d1d90f, src\metadb\FooDB\FooDB\mysql\references.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class references
    ' 
    '     Properties: created_at, creator_id, id, link, pubmed_id
    '                 ref_type, source_id, source_type, text, title
    '                 updated_at, updater_id
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 11:01:41


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace TMIC.FooDB.mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `references`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `references` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `ref_type` varchar(255) DEFAULT NULL,
'''   `text` text,
'''   `pubmed_id` varchar(255) DEFAULT NULL,
'''   `link` varchar(255) DEFAULT NULL,
'''   `title` varchar(255) DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime NOT NULL,
'''   `updated_at` datetime NOT NULL,
'''   `source_id` int(11) DEFAULT NULL,
'''   `source_type` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `index_references_on_source_type_and_source_id` (`source_type`,`source_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=31792 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'foodb'
''' --
''' 
''' --
''' -- Dumping routines for database 'foodb'
''' --
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
''' -- Dump completed on 2018-02-08 13:24:46
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("references", Database:="foodb", SchemaSQL:="
CREATE TABLE `references` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ref_type` varchar(255) DEFAULT NULL,
  `text` text,
  `pubmed_id` varchar(255) DEFAULT NULL,
  `link` varchar(255) DEFAULT NULL,
  `title` varchar(255) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_references_on_source_type_and_source_id` (`source_type`,`source_id`)
) ENGINE=InnoDB AUTO_INCREMENT=31792 DEFAULT CHARSET=utf8;")>
Public Class references: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("ref_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="ref_type")> Public Property ref_type As String
    <DatabaseField("text"), DataType(MySqlDbType.Text), Column(Name:="text")> Public Property text As String
    <DatabaseField("pubmed_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="pubmed_id")> Public Property pubmed_id As String
    <DatabaseField("link"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="link")> Public Property link As String
    <DatabaseField("title"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="title")> Public Property title As String
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("source_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="source_id")> Public Property source_id As Long
    <DatabaseField("source_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="source_type")> Public Property source_type As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `references` (`ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `references` (`id`, `ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `references` (`ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `references` (`id`, `ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `references` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `references` SET `id`='{0}', `ref_type`='{1}', `text`='{2}', `pubmed_id`='{3}', `link`='{4}', `title`='{5}', `creator_id`='{6}', `updater_id`='{7}', `created_at`='{8}', `updated_at`='{9}', `source_id`='{10}', `source_type`='{11}' WHERE `id` = '{12}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `references` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `references` (`id`, `ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `references` (`id`, `ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type)
        Else
        Return String.Format(INSERT_SQL, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{ref_type}', '{text}', '{pubmed_id}', '{link}', '{title}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}', '{source_id}', '{source_type}')"
        Else
            Return $"('{ref_type}', '{text}', '{pubmed_id}', '{link}', '{title}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}', '{source_id}', '{source_type}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `references` (`id`, `ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `references` (`id`, `ref_type`, `text`, `pubmed_id`, `link`, `title`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `source_id`, `source_type`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type)
        Else
        Return String.Format(REPLACE_SQL, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `references` SET `id`='{0}', `ref_type`='{1}', `text`='{2}', `pubmed_id`='{3}', `link`='{4}', `title`='{5}', `creator_id`='{6}', `updater_id`='{7}', `created_at`='{8}', `updated_at`='{9}', `source_id`='{10}', `source_type`='{11}' WHERE `id` = '{12}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, ref_type, text, pubmed_id, link, title, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), source_id, source_type, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As references
                         Return DirectCast(MyClass.MemberwiseClone, references)
                     End Function
End Class


End Namespace
