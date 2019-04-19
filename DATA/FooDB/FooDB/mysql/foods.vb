#Region "Microsoft.VisualBasic::ff60c585024fee96a3dc07f0b920d270, FooDB\FooDB\mysql\foods.vb"

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

    ' Class foods
    ' 
    '     Properties: category, created_at, creator_id, description, export_to_afcdb
    '                 export_to_foodb, food_group, food_subgroup, food_type, id
    '                 itis_id, legacy_id, name, name_scientific, ncbi_taxonomy_id
    '                 picture_content_type, picture_file_name, picture_file_size, picture_updated_at, updated_at
    '                 updater_id, wikipedia_id
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
''' DROP TABLE IF EXISTS `foods`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `foods` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `name_scientific` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `description` mediumtext COLLATE utf8_unicode_ci,
'''   `itis_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `wikipedia_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `picture_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `picture_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `picture_file_size` int(11) DEFAULT NULL,
'''   `picture_updated_at` datetime DEFAULT NULL,
'''   `legacy_id` int(11) DEFAULT NULL,
'''   `food_group` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `food_subgroup` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `food_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `export_to_afcdb` tinyint(1) NOT NULL DEFAULT '0',
'''   `category` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
'''   `ncbi_taxonomy_id` int(11) DEFAULT NULL,
'''   `export_to_foodb` tinyint(1) DEFAULT '1',
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_foods_on_name` (`name`),
'''   KEY `index_foods_on_name_scientific` (`name_scientific`),
'''   KEY `index_foods_on_export_to_afcdb` (`export_to_afcdb`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=925 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("foods", Database:="foodb", SchemaSQL:="
CREATE TABLE `foods` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `name_scientific` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` mediumtext COLLATE utf8_unicode_ci,
  `itis_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `wikipedia_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture_file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture_content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture_file_size` int(11) DEFAULT NULL,
  `picture_updated_at` datetime DEFAULT NULL,
  `legacy_id` int(11) DEFAULT NULL,
  `food_group` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `food_subgroup` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `food_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `export_to_afcdb` tinyint(1) NOT NULL DEFAULT '0',
  `category` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `ncbi_taxonomy_id` int(11) DEFAULT NULL,
  `export_to_foodb` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_foods_on_name` (`name`),
  KEY `index_foods_on_name_scientific` (`name_scientific`),
  KEY `index_foods_on_export_to_afcdb` (`export_to_afcdb`)
) ENGINE=InnoDB AUTO_INCREMENT=925 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;")>
Public Class foods: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="name")> Public Property name As String
    <DatabaseField("name_scientific"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="name_scientific")> Public Property name_scientific As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("itis_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="itis_id")> Public Property itis_id As String
    <DatabaseField("wikipedia_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="wikipedia_id")> Public Property wikipedia_id As String
    <DatabaseField("picture_file_name"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="picture_file_name")> Public Property picture_file_name As String
    <DatabaseField("picture_content_type"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="picture_content_type")> Public Property picture_content_type As String
    <DatabaseField("picture_file_size"), DataType(MySqlDbType.Int64, "11"), Column(Name:="picture_file_size")> Public Property picture_file_size As Long
    <DatabaseField("picture_updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="picture_updated_at")> Public Property picture_updated_at As Date
    <DatabaseField("legacy_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="legacy_id")> Public Property legacy_id As Long
    <DatabaseField("food_group"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="food_group")> Public Property food_group As String
    <DatabaseField("food_subgroup"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="food_subgroup")> Public Property food_subgroup As String
    <DatabaseField("food_type"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="food_type")> Public Property food_type As String
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("export_to_afcdb"), NotNull, DataType(MySqlDbType.Boolean, "1"), Column(Name:="export_to_afcdb")> Public Property export_to_afcdb As Boolean
    <DatabaseField("category"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="category")> Public Property category As String
    <DatabaseField("ncbi_taxonomy_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="ncbi_taxonomy_id")> Public Property ncbi_taxonomy_id As Long
    <DatabaseField("export_to_foodb"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="export_to_foodb")> Public Property export_to_foodb As Boolean
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `foods` (`name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `foods` (`id`, `name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `foods` (`name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `foods` (`id`, `name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `foods` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `foods` SET `id`='{0}', `name`='{1}', `name_scientific`='{2}', `description`='{3}', `itis_id`='{4}', `wikipedia_id`='{5}', `picture_file_name`='{6}', `picture_content_type`='{7}', `picture_file_size`='{8}', `picture_updated_at`='{9}', `legacy_id`='{10}', `food_group`='{11}', `food_subgroup`='{12}', `food_type`='{13}', `created_at`='{14}', `updated_at`='{15}', `creator_id`='{16}', `updater_id`='{17}', `export_to_afcdb`='{18}', `category`='{19}', `ncbi_taxonomy_id`='{20}', `export_to_foodb`='{21}' WHERE `id` = '{22}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `foods` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `foods` (`id`, `name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `foods` (`id`, `name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb)
        Else
        Return String.Format(INSERT_SQL, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{name}', '{name_scientific}', '{description}', '{itis_id}', '{wikipedia_id}', '{picture_file_name}', '{picture_content_type}', '{picture_file_size}', '{picture_updated_at}', '{legacy_id}', '{food_group}', '{food_subgroup}', '{food_type}', '{created_at}', '{updated_at}', '{creator_id}', '{updater_id}', '{export_to_afcdb}', '{category}', '{ncbi_taxonomy_id}', '{export_to_foodb}')"
        Else
            Return $"('{name}', '{name_scientific}', '{description}', '{itis_id}', '{wikipedia_id}', '{picture_file_name}', '{picture_content_type}', '{picture_file_size}', '{picture_updated_at}', '{legacy_id}', '{food_group}', '{food_subgroup}', '{food_type}', '{created_at}', '{updated_at}', '{creator_id}', '{updater_id}', '{export_to_afcdb}', '{category}', '{ncbi_taxonomy_id}', '{export_to_foodb}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `foods` (`id`, `name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `foods` (`id`, `name`, `name_scientific`, `description`, `itis_id`, `wikipedia_id`, `picture_file_name`, `picture_content_type`, `picture_file_size`, `picture_updated_at`, `legacy_id`, `food_group`, `food_subgroup`, `food_type`, `created_at`, `updated_at`, `creator_id`, `updater_id`, `export_to_afcdb`, `category`, `ncbi_taxonomy_id`, `export_to_foodb`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb)
        Else
        Return String.Format(REPLACE_SQL, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `foods` SET `id`='{0}', `name`='{1}', `name_scientific`='{2}', `description`='{3}', `itis_id`='{4}', `wikipedia_id`='{5}', `picture_file_name`='{6}', `picture_content_type`='{7}', `picture_file_size`='{8}', `picture_updated_at`='{9}', `legacy_id`='{10}', `food_group`='{11}', `food_subgroup`='{12}', `food_type`='{13}', `created_at`='{14}', `updated_at`='{15}', `creator_id`='{16}', `updater_id`='{17}', `export_to_afcdb`='{18}', `category`='{19}', `ncbi_taxonomy_id`='{20}', `export_to_foodb`='{21}' WHERE `id` = '{22}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, name, name_scientific, description, itis_id, wikipedia_id, picture_file_name, picture_content_type, picture_file_size, MySqlScript.ToMySqlDateTimeString(picture_updated_at), legacy_id, food_group, food_subgroup, food_type, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), creator_id, updater_id, export_to_afcdb, category, ncbi_taxonomy_id, export_to_foodb, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As foods
                         Return DirectCast(MyClass.MemberwiseClone, foods)
                     End Function
End Class


End Namespace
