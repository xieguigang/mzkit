#Region "Microsoft.VisualBasic::8744caf92b6d90d05a3b7838a690d521, Massbank\MetaLib\MetaLib.vb"

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

'     Class MetaInfo
' 
'         Properties: formula, ID, mass, name
' 
'         Function: ToString
' 
'     Class MetaLib
' 
'         Properties: biofluid_locations, compound_class, pathways, tissue_locations, xref
' 
'         Function: Equals, ToString
' 
'     Class xref
' 
'         Properties: CAS, chebi, HMDB, InChI, InChIkey
'                     KEGG, metlin, pubchem
' 
'         Function: IsCAS, IsChEBI, IsHMDB, IsKEGG, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetaLib.Models

    Public Class MetaInfo : Implements INamedValue
        Implements IEquatable(Of MetaInfo)

        ''' <summary>
        ''' 该物质在整合库之中的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property formula As String
        <XmlAttribute> Public Property exact_mass As Double

        Public Property name As String
        Public Property xref As xref

        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Overloads Function Equals(other As MetaInfo) As Boolean Implements IEquatable(Of MetaInfo).Equals
            If other Is Nothing Then
                Return False
            Else
                Return MetaEquals.Equals(Me, other)
            End If
        End Function
    End Class

    ''' <summary>
    ''' 对``chebi/kegg/pubchem/HMDB/metlin``的物质注释信息整合库，这个数据库只要为了生成编号，名称之类的注释信息而构建的
    ''' </summary>
    Public Class MetaLib : Inherits MetaInfo
        Implements IEquatable(Of MetaLib)
        Implements ICompoundClass

#Region "化合物分类"

        Public Property kingdom As String Implements ICompoundClass.kingdom
        Public Property super_class As String Implements ICompoundClass.super_class
        Public Property [class] As String Implements ICompoundClass.class
        Public Property sub_class As String Implements ICompoundClass.sub_class
        Public Property molecular_framework As String Implements ICompoundClass.molecular_framework

#End Region

        ''' <summary>
        ''' 仅限于人体内环境，这个化合物所存在的组织列表
        ''' </summary>
        ''' <returns></returns>
        Public Property tissue_locations As String()
        ''' <summary>
        ''' 仅限于人体内环境，这个化合物所存在的生物体液列表
        ''' </summary>
        ''' <returns></returns>
        Public Property biofluid_locations As String()

        ''' <summary>
        ''' 包含有这个物质的KEGG pathway的编号的集合，只有当<see cref="xref.KEGG"/>
        ''' 存在值的时候才会存在这个属性
        ''' </summary>
        ''' <returns></returns>
        Public Property pathways As String()

        Public Overrides Function ToString() As String
            Return name
        End Function

        ''' <summary>
        ''' 相同的物质可能在数据库之间有好几个编号?
        ''' </summary>
        ''' <param name="other"></param>
        ''' <returns></returns>
        Public Overloads Function Equals(other As MetaLib) As Boolean Implements IEquatable(Of MetaLib).Equals
            If other Is Nothing Then
                Return False
            Else
                Return MetaEquals.Equals(Me, other)
            End If
        End Function
    End Class
End Namespace
