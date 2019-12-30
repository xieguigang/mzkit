#Region "Microsoft.VisualBasic::ea288fefea017f055315967acf32e7d0, DATA\XrefEngine\ChemOntClassify.vb"

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

    ' Class ChemOntClassify
    ' 
    '     Properties: [class], kingdom, molecularFramework, subClass, superClass
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FilterByLevel, GetLineages, termsByLevel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.foundation.OBO_Foundry
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.genomics.foundation.OBO_Foundry.Tree

Public Class ChemOntClassify

    ReadOnly oboNodeList As GenericTree()
    ReadOnly oboTable As Dictionary(Of String, GenericTree)

    ''' <summary>
    ''' level 1
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property kingdom As GenericTree()
        Get
            Return termsByLevel(1)
        End Get
    End Property

    ''' <summary>
    ''' level 2
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property superClass As GenericTree()
        Get
            Return termsByLevel(2)
        End Get
    End Property

    ''' <summary>
    ''' level 3
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [class] As GenericTree()
        Get
            Return termsByLevel(3)
        End Get
    End Property

    ''' <summary>
    ''' level 4
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property subClass As GenericTree()
        Get
            Return termsByLevel(4)
        End Get
    End Property

    Public ReadOnly Property molecularFramework As GenericTree()
        Get
            Return termsByLevel(5)
        End Get
    End Property

    Default Public ReadOnly Property Item(id As String) As GenericTree
        Get
            Return oboTable(id)
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="obo">The file path of ``ChemOnt_2_1.obo``</param>
    Sub New(obo As String)
        oboTable = New OBOFile(file:=obo) _
            .GetRawTerms _
            .BuildTree
        oboNodeList = oboTable _
            .Values _
            .ToArray
    End Sub

    ''' <summary>
    ''' 将对应分类层次等级的注释分类信息取出来
    ''' </summary>
    ''' <param name="anno"></param>
    ''' <param name="level%"></param>
    ''' <returns></returns>
    Public Iterator Function FilterByLevel(anno As IEnumerable(Of ClassyfireAnnotation), level%) As IEnumerable(Of ClassyfireAnnotation)
        Dim levelIndex As Index(Of String) = termsByLevel(level).TermIndex

        For Each item As ClassyfireAnnotation In anno
            If item.ChemOntID Like levelIndex Then
                Yield item
            End If
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetLineages(term_id As String) As NamedCollection(Of GenericTree)()
        Return oboTable(term_id).TermLineages.ToArray
    End Function

    ''' <summary>
    ''' Get terms by level on tree
    ''' </summary>
    ''' <param name="level"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function termsByLevel(level As Integer) As GenericTree()
        Return oboNodeList _
            .Select(Function(node)
                        Return node.GetTermsByLevel(level)
                    End Function) _
            .Where(Function(a) Not a.IsNullOrEmpty) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(o) o.name) _
            .ToArray
    End Function
End Class

