#Region "Microsoft.VisualBasic::512edcc1b20ecbb90baa4dc116b47df1, src\metadb\FooDB\FooDB\mysql\compounds_pathways.vb"

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

    ' Class compounds_pathways
    ' 
    '     Properties: compound_id, created_at, creator_id, id, pathway_id
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
''' DROP TABLE IF EXISTS `compounds_pathways`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `compounds_pathways` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `compound_id` int(11) DEFAULT NULL,
'''   `pathway_id` int(11) DEFAULT NULL,
'''   `creator_id` int(11) DEFAULT NULL,
'''   `updater_id` int(11) DEFAULT NULL,
'''   `created_at` datetime NOT NULL,
'''   `updated_at` datetime NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `index_compounds_pathways_on_compound_id` (`compound_id`),
'''   KEY `index_compounds_pathways_on_pathway_id` (`pathway_id`),
'''   CONSTRAINT `fk_rails_14c02acb79` FOREIGN KEY (`pathway_id`) REFERENCES `pathways` (`id`),
'''   CONSTRAINT `fk_rails_34b0bf14de` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=1605 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compounds_pathways", Database:="foodb", SchemaSQL:="
CREATE TABLE `compounds_pathways` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `compound_id` int(11) DEFAULT NULL,
  `pathway_id` int(11) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL,
  `updater_id` int(11) DEFAULT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_compounds_pathways_on_compound_id` (`compound_id`),
  KEY `index_compounds_pathways_on_pathway_id` (`pathway_id`),
  CONSTRAINT `fk_rails_14c02acb79` FOREIGN KEY (`pathway_id`) REFERENCES `pathways` (`id`),
  CONSTRAINT `fk_rails_34b0bf14de` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1605 DEFAULT CHARSET=utf8;")>
Public Class compounds_pathways: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="id"), XmlAttribute> Public Property id As Long
    <DatabaseField("compound_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="compound_id")> Public Property compound_id As Long
    <DatabaseField("pathway_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pathway_id")> Public Property pathway_id As Long
    <DatabaseField("creator_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="creator_id")> Public Property creator_id As Long
    <DatabaseField("updater_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="updater_id")> Public Property updater_id As Long
    <DatabaseField("created_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="created_at")> Public Property created_at As Date
    <DatabaseField("updated_at"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="updated_at")> Public Property updated_at As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `compounds_pathways` (`compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `compounds_pathways` (`compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `compounds_pathways` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `compounds_pathways` SET `id`='{0}', `compound_id`='{1}', `pathway_id`='{2}', `creator_id`='{3}', `updater_id`='{4}', `created_at`='{5}', `updated_at`='{6}' WHERE `id` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `compounds_pathways` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        Else
        Return String.Format(INSERT_SQL, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{compound_id}', '{pathway_id}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
        Else
            Return $"('{compound_id}', '{pathway_id}', '{creator_id}', '{updater_id}', '{created_at}', '{updated_at}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `compounds_pathways` (`id`, `compound_id`, `pathway_id`, `creator_id`, `updater_id`, `created_at`, `updated_at`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        Else
        Return String.Format(REPLACE_SQL, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `compounds_pathways` SET `id`='{0}', `compound_id`='{1}', `pathway_id`='{2}', `creator_id`='{3}', `updater_id`='{4}', `created_at`='{5}', `updated_at`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, pathway_id, creator_id, updater_id, MySqlScript.ToMySqlDateTimeString(created_at), MySqlScript.ToMySqlDateTimeString(updated_at), id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As compounds_pathways
                         Return DirectCast(MyClass.MemberwiseClone, compounds_pathways)
                     End Function
End Class


End Namespace
