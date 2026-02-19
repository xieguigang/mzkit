#Region "Microsoft.VisualBasic::d3c6c0cebd9f39bd5b130b71472d9dd7, metadb\Massbank\MetaLib\Models\MetaLib.vb"

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


' Code Statistics:

'   Total Lines: 126
'    Code Lines: 89 (70.63%)
' Comment Lines: 21 (16.67%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 16 (12.70%)
'     File Size: 5.05 KB


'     Class MetaLib
' 
'         Properties: chemical, keywords, organism, pathways, samples
' 
'         Constructor: (+3 Overloads) Sub New
'         Function: Equals, ToString, (+2 Overloads) Union
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Metabolite.CrossReference
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Linq

Namespace Metabolite

    ''' <summary>
    ''' 对``chebi/kegg/pubchem/HMDB/metlin``的物质注释信息整合库，这个数据库只要为了生成编号，名称之类的注释信息而构建的
    ''' </summary>
    Public Class MetaLib : Inherits MetaInfo
        Implements IEquatable(Of MetaLib)
        Implements ICompoundClass

        <Field(13)> Public Property chemical As ChemicalDescriptor

        <Field(14)> Public Property organism As String()

        ''' <summary>
        ''' 包含有这个物质的KEGG pathway的编号的集合，只有当<see cref="xref.KEGG"/>
        ''' 存在值的时候才会存在这个属性
        ''' </summary>
        ''' <returns></returns>
        <Field(15)> Public Property pathways As String()
        <Field(16)> Public Property samples As BiosampleSource()

        <Field(17)> Public Property keywords As String()

        Sub New()
        End Sub

        ''' <summary>
        ''' creator a metabolite annotation data with compound class assigned
        ''' </summary>
        ''' <param name="class">the compound class data</param>
        Sub New([class] As ICompoundClass)
            If Not [class] Is Nothing Then
                Me.kingdom = [class].kingdom
                Me.super_class = [class].super_class
                Me.[class] = [class].class
                Me.sub_class = [class].sub_class
                Me.molecular_framework = [class].molecular_framework
            End If
        End Sub

        ''' <summary>
        ''' make value copy
        ''' </summary>
        ''' <param name="clone"></param>
        Sub New(clone As MetaLib)
            ID = clone.ID
            formula = clone.formula
            exact_mass = clone.exact_mass
            name = clone.name
            IUPACName = clone.IUPACName
            description = clone.description
            synonym = clone.synonym.SafeQuery.ToArray
            kingdom = clone.kingdom
            super_class = clone.super_class
            [class] = clone.class
            sub_class = clone.sub_class
            molecular_framework = clone.molecular_framework
            organism = clone.organism.SafeQuery.ToArray
            pathways = clone.pathways.SafeQuery.ToArray
            samples = clone.samples.SafeQuery.Select(Function(a) New BiosampleSource(a)).ToArray
            keywords = clone.keywords.SafeQuery.ToArray

            If Not clone.xref Is Nothing Then
                xref = New xref(clone.xref)
            End If
            If Not clone.chemical Is Nothing Then
                chemical = New ChemicalDescriptor(clone.chemical)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return name
        End Function

        ''' <summary>
        ''' 相同的物质可能在数据库之间有好几个编号?
        ''' </summary>
        ''' <param name="other"></param>
        ''' <returns></returns>
        Public Overloads Function Equals(other As MetaLib) As Boolean Implements IEquatable(Of MetaLib).Equals
            Static metaEquals As MetaEquals

            If metaEquals Is Nothing Then
                metaEquals = New MetaEquals
            End If

            If other Is Nothing Then
                Return False
            Else
                Return metaEquals.Equals(Me, other)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Union(metabolite As IEnumerable(Of MetaInfo)) As MetaInfo
            Return CrossReferenceData.UnionData(Of xref, MetaInfo)(
                group:=metabolite,
                setMeta:=Function(ByRef m, id, name, formula, exact_mass)
                             m.ID = id
                             m.name = name
                             m.formula = formula
                             m.exact_mass = exact_mass
                             Return m
                         End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Union(metabolite As IEnumerable(Of MetaLib)) As MetaInfo
            Return CrossReferenceData.UnionData(Of xref, MetaInfo)(
                group:=metabolite,
                setMeta:=Function(ByRef m, id, name, formula, exact_mass)
                             m.ID = id
                             m.name = name
                             m.formula = formula
                             m.exact_mass = exact_mass
                             Return m
                         End Function)
        End Function
    End Class
End Namespace
