#Region "Microsoft.VisualBasic::326aece1fcf2ccc67c24fe1d89b4c745, Massbank\Public\TMIC\FooDB\mysql\nutrients.vb"

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

    ' Class nutrients
    ' 
    '     Properties: annotation_quality, comments, compound_source, created_at, creator_id
    '                 description, dfc_id, dfc_name, duke_id, eafus_id
    '                 export, general_citations, id, legacy_id, metabolism
    '                 name, public_id, state, synthesis_citations, type
    '                 updated_at, updater_id, wikipedia_id
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
''' DROP TABLE IF EXISTS `nutrients`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `nutrients` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `legacy_id` int(11) DEFAULT NULL,
'''   `type` varchar(255) NOT NULL,
'''   `public_id` varchar(9) NOT NULL,
'''   `name` varchar(255) NOT NULL,
'''   `export` tinyint(1) DEFAULT '0',
'''   `state` varchar(255) DEFAULT NULL,
'''   `annotation_quality` varchar(255) DEFAULT NULL,
'''   `description` mediumtext,
'''   `wikipedia_id` varchar(255) DEFAULT NULL,
'''   `comments` mediumtext,
'''   `dfc_id` varchar(255) DEFAULT NULL,
'''   `duke_id` varchar(255) DEFAULT NULL,
'''   `eafus_id` varchar(255) DEFAULT NULL,
'''   `dfc_name` mediumtext,
'''   `compound_source` varchar(255) DEFAULT NULL,
'''   `metabolism` mediumtext,
'''   `synthesis_citations` mediumtext,
'''   `general_citations` mediumtext,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime DEFAULT NULL,
'''   `updated_at` datetime DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `index_nutrients_on_name` (`name`),
'''   UNIQUE KEY `index_nutrients_on_public_id` (`public_id`),
'''   UNIQUE KEY `index_nutrients_on_name_and_public_id` (`name`,`public_id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("nutrients", Database:="foodb", SchemaSQL:="
CREATE TABLE `nutrients` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `legacy_id` int(11) DEFAULT NULL,
  `type` varchar(255) NOT NULL,
  `public_id` varchar(9) NOT NULL,
  `name` varchar(255) NOT NULL,
  `export` tinyint(1) DEFAULT '0',
  `state` varchar(255) DEFAULT NULL,
  `annotation_quality` varchar(255) DEFAULT NULL,
  `description` mediumtext,
  `wikipedia_id` varchar(255) DEFAULT NULL,
  `comments` mediumtext,
  `dfc_id` varchar(255) DEFAULT NULL,
  `duke_id` varchar(255) DEFAULT NULL,
  `eafus_id` varchar(255) DEFAULT NULL,
  `dfc_name` mediumtext,
  `compound_source` varchar(255) DEFAULT NULL,
  `metabolism` mediumtext,
  `synthesis_citations` mediumtext,
  `general_citations` mediumtext,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_nutrients_on_name` (`name`),
  UNIQUE KEY `index_nutrients_on_public_id` (`public_id`),
  UNIQUE KEY `index_nutrients_on_name_and_public_id` (`name`,`public_id`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;")>
Public Class nutrients: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("legacy_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="legacy_id")> Public Property legacy_id As Long
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="type")> Public Property type As String
    <DatabaseField("public_id"), NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="public_id")> Public Property public_id As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="name")> Public Property name As String
    <DatabaseField("export"), DataType(MySqlDbType.Boolean, "1"), Column(Name:="export")> Public Property export As Boolean
    <DatabaseField("state"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="state")> Public Property state As String
    <DatabaseField("annotation_quality"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="annotation_quality")> Public Property annotation_quality As String
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
    <DatabaseField("wikipedia_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="wikipedia_id")> Public Property wikipedia_id As String
    <DatabaseField("comments"), DataType(MySqlDbType.Text), Column(Name:="comments")> Public Property comments As String
    <DatabaseField("dfc_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="dfc_id")> Public Property dfc_id As String
    <DatabaseField("duke_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="duke_id")> Public Property duke_id As String
    <DatabaseField("eafus_id"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="eafus_id")> Public Property eafus_id As String
    <DatabaseField("dfc_name"), DataType(MySqlDbType.Text), Column(Name:="dfc_name")> Public Property dfc_name As String
    <DatabaseField("compound_source"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="compound_source")> Public Property compound_source As String
    <DatabaseField("metabolism"), DataType(MySqlDbType.Text), Column(Name:="metabolism")> Public Property metabolism As String
    <DatabaseField("synthesis_citations"), DataType(MySqlDbType.Text), Column(Name:="synthesis_citations")> Public Property synthesis_citations As String
    <DatabaseField("general_citations"), DataType(MySqlDbType.Text), Column(Name:="general_citations")> Public Property general_citations As String
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `nutrients` (`legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `nutrients` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `nutrients` (`legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `nutrients` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `nutrients` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `nutrients` SET `id`='{0}', `legacy_id`='{1}', `type`='{2}', `public_id`='{3}', `name`='{4}', `export`='{5}', `state`='{6}', `annotation_quality`='{7}', `description`='{8}', `wikipedia_id`='{9}', `comments`='{10}', `dfc_id`='{11}', `duke_id`='{12}', `eafus_id`='{13}', `dfc_name`='{14}', `compound_source`='{15}', `metabolism`='{16}', `synthesis_citations`='{17}', `general_citations`='{18}', `creator_id`='{19}', `updater_id`='{20}', `created_at`='{21}', `updated_at`='{22}' WHERE `id` = '{23}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `nutrients` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `nutrients` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `nutrients` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        Else
        Return String.Format(INSERT_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{legacy_id}', '{type}', '{public_id}', '{name}', '{export}', '{state}', '{annotation_quality}', '{description}', '{wikipedia_id}', '{comments}', '{dfc_id}', '{duke_id}', '{eafus_id}', '{dfc_name}', '{compound_source}', '{metabolism}', '{synthesis_citations}', '{general_citations}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
        Else
            Return $"('{legacy_id}', '{type}', '{public_id}', '{name}', '{export}', '{state}', '{annotation_quality}', '{description}', '{wikipedia_id}', '{comments}', '{dfc_id}', '{duke_id}', '{eafus_id}', '{dfc_name}', '{compound_source}', '{metabolism}', '{synthesis_citations}', '{general_citations}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `nutrients` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `nutrients` (`id`, `legacy_id`, `type`, `public_id`, `name`, `export`, `state`, `annotation_quality`, `description`, `wikipedia_id`, `comments`, `dfc_id`, `duke_id`, `eafus_id`, `dfc_name`, `compound_source`, `metabolism`, `synthesis_citations`, `general_citations`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        Else
        Return String.Format(REPLACE_SQL, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `nutrients` SET `id`='{0}', `legacy_id`='{1}', `type`='{2}', `public_id`='{3}', `name`='{4}', `export`='{5}', `state`='{6}', `annotation_quality`='{7}', `description`='{8}', `wikipedia_id`='{9}', `comments`='{10}', `dfc_id`='{11}', `duke_id`='{12}', `eafus_id`='{13}', `dfc_name`='{14}', `compound_source`='{15}', `metabolism`='{16}', `synthesis_citations`='{17}', `general_citations`='{18}', `creator_id`='{19}', `updater_id`='{20}', `created_at`='{21}', `updated_at`='{22}' WHERE `id` = '{23}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, legacy_id, type, public_id, name, export, state, annotation_quality, description, wikipedia_id, comments, dfc_id, duke_id, eafus_id, dfc_name, compound_source, metabolism, synthesis_citations, general_citations, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As nutrients
                         Return DirectCast(MyClass.MemberwiseClone, nutrients)
                     End Function
End Class


End Namespace
