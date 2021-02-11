#Region "Microsoft.VisualBasic::c34bed20cb1a5e2d7d8e8020d4f765ba, PubChem.MySql\mysql\synonym.vb"

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

    ' Class synonym
    ' 
    '     Properties: cid, synonym_name
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

REM  Dump @2019/9/5 12:56:55


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace mysql

''' <summary>
''' ```SQL
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("synonym", Database:="pubchem", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `pubchem`.`synonym` (
  `cid` INT NOT NULL,
  `synonym_name` VARCHAR(1024) NOT NULL,
  PRIMARY KEY (`cid`))
ENGINE = InnoDB;
")>
Public Class synonym: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="cid"), XmlAttribute> Public Property cid As Long
    <DatabaseField("synonym_name"), NotNull, DataType(MySqlDbType.VarChar, "1024"), Column(Name:="synonym_name")> Public Property synonym_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `synonym` WHERE `cid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `synonym` SET `cid`='{0}', `synonym_name`='{1}' WHERE `cid` = '{2}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `synonym` WHERE `cid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cid, synonym_name)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, cid, synonym_name)
        Else
        Return String.Format(INSERT_SQL, cid, synonym_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{cid}', '{synonym_name}')"
        Else
            Return $"('{cid}', '{synonym_name}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cid, synonym_name)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `synonym` (`cid`, `synonym_name`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, cid, synonym_name)
        Else
        Return String.Format(REPLACE_SQL, cid, synonym_name)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `synonym` SET `cid`='{0}', `synonym_name`='{1}' WHERE `cid` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cid, synonym_name, cid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As synonym
                         Return DirectCast(MyClass.MemberwiseClone, synonym)
                     End Function
End Class


End Namespace
