#Region "Microsoft.VisualBasic::95d3e0e41db9f24619e03920fcab0e38, Massbank\Public\TMIC\FooDB\mysql\contents.vb"

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

    ' Class contents
    ' 
    '     Properties: citation, citation_type, created_at, creator_id, food_id
    '                 id, orig_citation, orig_content, orig_food_common_name, orig_food_id
    '                 orig_food_part, orig_food_scientific_name, orig_max, orig_method, orig_min
    '                 orig_source_id, orig_source_name, orig_unit, orig_unit_expression, source_id
    '                 source_type, standard_content, updated_at, updater_id
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
''' DROP TABLE IF EXISTS `contents`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `contents` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `source_id` int(11) DEFAULT NULL,
'''   `source_type` varchar(255) DEFAULT NULL,
'''   `food_id` int(11) NOT NULL,
'''   `orig_food_id` varchar(255) DEFAULT NULL,
'''   `orig_food_common_name` varchar(255) DEFAULT NULL,
'''   `orig_food_scientific_name` varchar(255) DEFAULT NULL,
'''   `orig_food_part` varchar(255) DEFAULT NULL,
'''   `orig_source_id` varchar(255) DEFAULT NULL,
'''   `orig_source_name` varchar(255) DEFAULT NULL,
'''   `orig_content` decimal(15,9) DEFAULT NULL,
'''   `orig_min` decimal(15,9) DEFAULT NULL,
'''   `orig_max` decimal(15,9) DEFAULT NULL,
'''   `orig_unit` varchar(255) DEFAULT NULL,
'''   `orig_citation` mediumtext,
'''   `citation` mediumtext NOT NULL,
'''   `citation_type` varchar(255) NOT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `orig_method` varchar(255) DEFAULT NULL,
'''   `orig_unit_expression` varchar(255) DEFAULT NULL,
'''   `standard_content` decimal(15,9) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `content_source_and_food_index` (`source_id`,`source_type`,`food_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=1682258 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("contents", Database:="foodb", SchemaSQL:="
CREATE TABLE `contents` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `source_id` int(11) DEFAULT NULL,
  `source_type` varchar(255) DEFAULT NULL,
  `food_id` int(11) NOT NULL,
  `orig_food_id` varchar(255) DEFAULT NULL,
  `orig_food_common_name` varchar(255) DEFAULT NULL,
  `orig_food_scientific_name` varchar(255) DEFAULT NULL,
  `orig_food_part` varchar(255) DEFAULT NULL,
  `orig_source_id` varchar(255) DEFAULT NULL,
  `orig_source_name` varchar(255) DEFAULT NULL,
  `orig_content` decimal(15,9) DEFAULT NULL,
  `orig_min` decimal(15,9) DEFAULT NULL,
  `orig_max` decimal(15,9) DEFAULT NULL,
  `orig_unit` varchar(255) DEFAULT NULL,
  `orig_citation` mediumtext,
  `citation` mediumtext NOT NULL,
  `citation_type` varchar(255) NOT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `orig_method` varchar(255) DEFAULT NULL,
  `orig_unit_expression` varchar(255) DEFAULT NULL,
  `standard_content` decimal(15,9) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `content_source_and_food_index` (`source_id`,`source_type`,`food_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1682258 DEFAULT CHARSET=utf8;")>
Public Class contents: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("source_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="source_id")> Public Property source_id As Long
    <DatabaseField("source_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="source_type")> Public Property source_type As String
    <DatabaseField("food_id"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="food_id")> Public Property food_id As Long
    <DatabaseField("orig_food_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_food_id")> Public Property orig_food_id As String
    <DatabaseField("orig_food_common_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_food_common_name")> Public Property orig_food_common_name As String
    <DatabaseField("orig_food_scientific_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_food_scientific_name")> Public Property orig_food_scientific_name As String
    <DatabaseField("orig_food_part"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_food_part")> Public Property orig_food_part As String
    <DatabaseField("orig_source_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_source_id")> Public Property orig_source_id As String
    <DatabaseField("orig_source_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_source_name")> Public Property orig_source_name As String
    <DatabaseField("orig_content"), DataType(MySqlDbType.Decimal), Column(Name:="orig_content")> Public Property orig_content As Decimal
    <DatabaseField("orig_min"), DataType(MySqlDbType.Decimal), Column(Name:="orig_min")> Public Property orig_min As Decimal
    <DatabaseField("orig_max"), DataType(MySqlDbType.Decimal), Column(Name:="orig_max")> Public Property orig_max As Decimal
    <DatabaseField("orig_unit"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_unit")> Public Property orig_unit As String
    <DatabaseField("orig_citation"), DataType(MySqlDbType.Text), Column(Name:="orig_citation")> Public Property orig_citation As String
    <DatabaseField("citation"), NotNull, DataType(MySqlDbType.Text), Column(Name:="citation")> Public Property citation As String
    <DatabaseField("citation_type"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="citation_type")> Public Property citation_type As String
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("orig_method"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_method")> Public Property orig_method As String
    <DatabaseField("orig_unit_expression"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="orig_unit_expression")> Public Property orig_unit_expression As String
    <DatabaseField("standard_content"), DataType(MySqlDbType.Decimal), Column(Name:="standard_content")> Public Property standard_content As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `contents` (`source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `contents` (`id`, `source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `contents` (`source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `contents` (`id`, `source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `contents` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `contents` SET `id`='{0}', `source_id`='{1}', `source_type`='{2}', `food_id`='{3}', `orig_food_id`='{4}', `orig_food_common_name`='{5}', `orig_food_scientific_name`='{6}', `orig_food_part`='{7}', `orig_source_id`='{8}', `orig_source_name`='{9}', `orig_content`='{10}', `orig_min`='{11}', `orig_max`='{12}', `orig_unit`='{13}', `orig_citation`='{14}', `citation`='{15}', `citation_type`='{16}', `creator_id`='{17}', `updater_id`='{18}', `created_at`='{19}', `updated_at`='{20}', `orig_method`='{21}', `orig_unit_expression`='{22}', `standard_content`='{23}' WHERE `id` = '{24}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `contents` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `contents` (`id`, `source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `contents` (`id`, `source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content)
        Else
        Return String.Format(INSERT_SQL, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{source_id}', '{source_type}', '{food_id}', '{orig_food_id}', '{orig_food_common_name}', '{orig_food_scientific_name}', '{orig_food_part}', '{orig_source_id}', '{orig_source_name}', '{orig_content}', '{orig_min}', '{orig_max}', '{orig_unit}', '{orig_citation}', '{citation}', '{citation_type}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}', '{orig_method}', '{orig_unit_expression}', '{standard_content}')"
        Else
            Return $"('{source_id}', '{source_type}', '{food_id}', '{orig_food_id}', '{orig_food_common_name}', '{orig_food_scientific_name}', '{orig_food_part}', '{orig_source_id}', '{orig_source_name}', '{orig_content}', '{orig_min}', '{orig_max}', '{orig_unit}', '{orig_citation}', '{citation}', '{citation_type}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}', '{orig_method}', '{orig_unit_expression}', '{standard_content}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `contents` (`id`, `source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `contents` (`id`, `source_id`, `source_type`, `food_id`, `orig_food_id`, `orig_food_common_name`, `orig_food_scientific_name`, `orig_food_part`, `orig_source_id`, `orig_source_name`, `orig_content`, `orig_min`, `orig_max`, `orig_unit`, `orig_citation`, `citation`, `citation_type`, `creator_id`, `updater_id`, `created_at`, `updated_at`, `orig_method`, `orig_unit_expression`, `standard_content`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content)
        Else
        Return String.Format(REPLACE_SQL, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `contents` SET `id`='{0}', `source_id`='{1}', `source_type`='{2}', `food_id`='{3}', `orig_food_id`='{4}', `orig_food_common_name`='{5}', `orig_food_scientific_name`='{6}', `orig_food_part`='{7}', `orig_source_id`='{8}', `orig_source_name`='{9}', `orig_content`='{10}', `orig_min`='{11}', `orig_max`='{12}', `orig_unit`='{13}', `orig_citation`='{14}', `citation`='{15}', `citation_type`='{16}', `creator_id`='{17}', `updater_id`='{18}', `created_at`='{19}', `updated_at`='{20}', `orig_method`='{21}', `orig_unit_expression`='{22}', `standard_content`='{23}' WHERE `id` = '{24}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, source_id, source_type, food_id, orig_food_id, orig_food_common_name, orig_food_scientific_name, orig_food_part, orig_source_id, orig_source_name, orig_content, orig_min, orig_max, orig_unit, orig_citation, citation, citation_type, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), orig_method, orig_unit_expression, standard_content, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As contents
                         Return DirectCast(MyClass.MemberwiseClone, contents)
                     End Function
End Class


End Namespace
