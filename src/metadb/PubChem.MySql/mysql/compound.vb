#Region "Microsoft.VisualBasic::48f955585ff0e8a5954df76b34684970, src\metadb\PubChem.MySql\mysql\compound.vb"

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

    ' Class compound
    ' 
    '     Properties: canonicalized, chebi, cid, common_name, hmdb
    '                 inchi_key, kegg
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("compound", Database:="pubchem", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `pubchem`.`compound` (
  `cid` INT NOT NULL COMMENT 'The pubchem cid',
  `canonicalized` INT NOT NULL DEFAULT 0,
  `common_name` VARCHAR(256) NULL,
  `kegg` VARCHAR(8) NULL,
  `hmdb` VARCHAR(16) NULL,
  `chebi` VARCHAR(16) NULL,
  `inchi_key` CHAR(32) NOT NULL,
  PRIMARY KEY (`cid`),
  UNIQUE INDEX `cid_UNIQUE` (`cid` ASC))
ENGINE = InnoDB
COMMENT = 'table for query database. 这个表比较精简，主要是为了方便进行根据id或者inchi_key以及其他的数据库编号来进行的快速查询操作';
")>
Public Class compound: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' The pubchem cid
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("cid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="cid"), XmlAttribute> Public Property cid As Long
    <DatabaseField("canonicalized"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="canonicalized")> Public Property canonicalized As Long
    <DatabaseField("common_name"), DataType(MySqlDbType.VarChar, "256"), Column(Name:="common_name")> Public Property common_name As String
    <DatabaseField("kegg"), DataType(MySqlDbType.VarChar, "8"), Column(Name:="kegg")> Public Property kegg As String
    <DatabaseField("hmdb"), DataType(MySqlDbType.VarChar, "16"), Column(Name:="hmdb")> Public Property hmdb As String
    <DatabaseField("chebi"), DataType(MySqlDbType.VarChar, "16"), Column(Name:="chebi")> Public Property chebi As String
    <DatabaseField("inchi_key"), NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="inchi_key")> Public Property inchi_key As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `compound` WHERE `cid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `compound` SET `cid`='{0}', `canonicalized`='{1}', `common_name`='{2}', `kegg`='{3}', `hmdb`='{4}', `chebi`='{5}', `inchi_key`='{6}' WHERE `cid` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `compound` WHERE `cid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key)
        Else
        Return String.Format(INSERT_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{cid}', '{canonicalized}', '{common_name}', '{kegg}', '{hmdb}', '{chebi}', '{inchi_key}')"
        Else
            Return $"('{cid}', '{canonicalized}', '{common_name}', '{kegg}', '{hmdb}', '{chebi}', '{inchi_key}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `compound` (`cid`, `canonicalized`, `common_name`, `kegg`, `hmdb`, `chebi`, `inchi_key`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key)
        Else
        Return String.Format(REPLACE_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `compound` SET `cid`='{0}', `canonicalized`='{1}', `common_name`='{2}', `kegg`='{3}', `hmdb`='{4}', `chebi`='{5}', `inchi_key`='{6}' WHERE `cid` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cid, canonicalized, common_name, kegg, hmdb, chebi, inchi_key, cid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As compound
                         Return DirectCast(MyClass.MemberwiseClone, compound)
                     End Function
End Class


End Namespace
