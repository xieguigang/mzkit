#Region "Microsoft.VisualBasic::d960262894f705c886c6e6c20b7fa11f, DATA\PubChem.MySql\mysql\structure.vb"

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

    ' Class [structure]
    ' 
    '     Properties: bond_annotations, checksum, cid, coordinate_type, model_base64
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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("structure", Database:="pubchem", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `pubchem`.`structure` (
  `cid` INT NOT NULL,
  `coordinate_type` VARCHAR(1024) NULL,
  `bond_annotations` VARCHAR(1024) NULL,
  `checksum` VARCHAR(32) NULL COMMENT 'md5',
  `model_base64` LONGTEXT NULL,
  PRIMARY KEY (`cid`),
  UNIQUE INDEX `cid_UNIQUE` (`cid` ASC))
ENGINE = InnoDB;
")>
Public Class [structure]: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="cid"), XmlAttribute> Public Property cid As Long
    <DatabaseField("coordinate_type"), DataType(MySqlDbType.VarChar, "1024"), Column(Name:="coordinate_type")> Public Property coordinate_type As String
    <DatabaseField("bond_annotations"), DataType(MySqlDbType.VarChar, "1024"), Column(Name:="bond_annotations")> Public Property bond_annotations As String
''' <summary>
''' md5
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("checksum"), DataType(MySqlDbType.VarChar, "32"), Column(Name:="checksum")> Public Property checksum As String
    <DatabaseField("model_base64"), DataType(MySqlDbType.Text), Column(Name:="model_base64")> Public Property model_base64 As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `structure` WHERE `cid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `structure` SET `cid`='{0}', `coordinate_type`='{1}', `bond_annotations`='{2}', `checksum`='{3}', `model_base64`='{4}' WHERE `cid` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `structure` WHERE `cid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, cid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64)
        Else
        Return String.Format(INSERT_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{cid}', '{coordinate_type}', '{bond_annotations}', '{checksum}', '{model_base64}')"
        Else
            Return $"('{cid}', '{coordinate_type}', '{bond_annotations}', '{checksum}', '{model_base64}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `structure` (`cid`, `coordinate_type`, `bond_annotations`, `checksum`, `model_base64`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64)
        Else
        Return String.Format(REPLACE_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `structure` SET `cid`='{0}', `coordinate_type`='{1}', `bond_annotations`='{2}', `checksum`='{3}', `model_base64`='{4}' WHERE `cid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, cid, coordinate_type, bond_annotations, checksum, model_base64, cid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As [structure]
                         Return DirectCast(MyClass.MemberwiseClone, [structure])
                     End Function
End Class


End Namespace

