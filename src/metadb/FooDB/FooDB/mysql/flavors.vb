#Region "Microsoft.VisualBasic::ae0c2fcb1ba9c0d8aaf36fd41cb07adb, FooDB\FooDB\mysql\flavors.vb"

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

    ' Class flavors
    ' 
    '     Properties: category, created_at, creator_id, flavor_group, id
    '                 name, updated_at, updater_id
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
''' DROP TABLE IF EXISTS `flavors`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `flavors` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `flavor_group` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `category` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_flavors_on_name` (`name`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=857 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("flavors", Database:="foodb", SchemaSQL:="
CREATE TABLE `flavors` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `flavor_group` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `category` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_flavors_on_name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=857 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class flavors: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="name")> Public Property name As String
    <DatabaseField("flavor_group"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="flavor_group")> Public Property flavor_group As String
    <DatabaseField("category"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="category")> Public Property category As String
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `flavors` (`name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `flavors` (`id`, `name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `flavors` (`name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `flavors` (`id`, `name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `flavors` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `flavors` SET `id`='{0}', `name`='{1}', `flavor_group`='{2}', `category`='{3}', `created_at`='{4}', `updated_at`='{5}', `creator_id`='{6}', `updater_id`='{7}' WHERE `id` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `flavors` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `flavors` (`id`, `name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `flavors` (`id`, `name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id)
        Else
        Return String.Format(INSERT_SQL, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{name}', '{flavor_group}', '{category}', '{created_at}', '{updated_at}', '{creator_id}', '{updater_id}')"
        Else
            Return $"('{name}', '{flavor_group}', '{category}', '{created_at}', '{updated_at}', '{creator_id}', '{updater_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `flavors` (`id`, `name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `flavors` (`id`, `name`, `flavor_group`, `category`, `created_at`, `updated_at`, `creator_id`, `updater_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id)
        Else
        Return String.Format(REPLACE_SQL, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `flavors` SET `id`='{0}', `name`='{1}', `flavor_group`='{2}', `category`='{3}', `created_at`='{4}', `updated_at`='{5}', `creator_id`='{6}', `updater_id`='{7}' WHERE `id` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, name, flavor_group, category, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As flavors
                         Return DirectCast(MyClass.MemberwiseClone, flavors)
                     End Function
End Class


End Namespace
