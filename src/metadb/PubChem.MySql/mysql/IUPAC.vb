#Region "Microsoft.VisualBasic::ad65c7094ef6a8998a4fb2818ee582b3, src\metadb\PubChem.MySql\mysql\IUPAC.vb"

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

    ' Class IUPAC
    ' 
    '     Properties: cas_name, cid, inchi, name, name_markup
    '                 openeye_name, systematic_name, traditional_name
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("IUPAC", Database:="pubchem", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `pubchem`.`IUPAC` (
  `cid` INT NOT NULL,
  `openeye_name` VARCHAR(256) NULL,
  `cas_name` VARCHAR(256) NULL,
  `name_markup` LONGTEXT NULL,
  `name` VARCHAR(256) NULL,
  `systematic_name` LONGTEXT NULL,
  `traditional_name` VARCHAR(256) NULL,
  `inchi` LONGTEXT NULL,
  PRIMARY KEY (`cid`),
  UNIQUE INDEX `cid_UNIQUE` (`cid` ASC))
ENGINE = InnoDB;
")>
Public Class IUPAC: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="cid"), XmlAttribute> Public Property cid As Long
    <DatabaseField("openeye_name"), DataType(MySqlDbType.VarChar, "256"), Column(Name:="openeye_name")> Public Property openeye_name As String
    <DatabaseField("cas_name"), DataType(MySqlDbType.VarChar, "256"), Column(Name:="cas_name")> Public Property cas_name As String
    <DatabaseField("name_markup"), DataType(MySqlDbType.Text), Column(Name:="name_markup")> Public Property name_markup As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "256"), Column(Name:="name")> Public Property name As String
    <DatabaseField("systematic_name"), DataType(MySqlDbType.Text), Column(Name:="systematic_name")> Public Property systematic_name As String
    <DatabaseField("traditional_name"), DataType(MySqlDbType.VarChar, "256"), Column(Name:="traditional_name")> Public Property traditional_name As String
    <DatabaseField("inchi"), DataType(MySqlDbType.Text), Column(Name:="inchi")> Public Property inchi As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `IUPAC` WHERE `cid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `IUPAC` SET `cid`='{0}', `openeye_name`='{1}', `cas_name`='{2}', `name_markup`='{3}', `name`='{4}', `systematic_name`='{5}', `traditional_name`='{6}', `inchi`='{7}' WHERE `cid` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `IUPAC` WHERE `cid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi)
        Else
        Return String.Format(INSERT_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{cid}', '{openeye_name}', '{cas_name}', '{name_markup}', '{name}', '{systematic_name}', '{traditional_name}', '{inchi}')"
        Else
            Return $"('{cid}', '{openeye_name}', '{cas_name}', '{name_markup}', '{name}', '{systematic_name}', '{traditional_name}', '{inchi}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `IUPAC` (`cid`, `openeye_name`, `cas_name`, `name_markup`, `name`, `systematic_name`, `traditional_name`, `inchi`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi)
        Else
        Return String.Format(REPLACE_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `IUPAC` SET `cid`='{0}', `openeye_name`='{1}', `cas_name`='{2}', `name_markup`='{3}', `name`='{4}', `systematic_name`='{5}', `traditional_name`='{6}', `inchi`='{7}' WHERE `cid` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cid, openeye_name, cas_name, name_markup, name, systematic_name, traditional_name, inchi, cid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As IUPAC
                         Return DirectCast(MyClass.MemberwiseClone, IUPAC)
                     End Function
End Class


End Namespace
