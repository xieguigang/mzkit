#Region "Microsoft.VisualBasic::af1b674222bd08ad16f447037daa0854, DATA\Massbank\MetaLib\Match\TreeSearch.vb"

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

    '     Class TreeSearch
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: BuildTree, CompareAnnotation, Search
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.MassSpectrum.DATA.MetaLib.Models

Namespace MetaLib

    ''' <summary>
    ''' 在数据库的编号定义比较模糊的情况下, 会需要使用这个模块进行快速匹配搜索
    ''' </summary>
    Public Class TreeSearch

        ReadOnly metaTree As AVLTree(Of MetaInfo, MetaInfo)
        ReadOnly cutoff#
        ReadOnly metaEquals As New MetaEquals

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="score">认为两个物质注释指的是相同的物质的最低得分</param>
        Sub New(Optional score# = 0.4)
            metaTree = New AVLTree(Of MetaInfo, MetaInfo)(AddressOf CompareAnnotation, Function(meta) meta.name)
            cutoff = score
        End Sub

        Public Function CompareAnnotation(a As MetaInfo, b As MetaInfo) As Integer
            Dim score = metaEquals.Agreement(a, b)

            If score >= cutoff Then
                Return 0
            ElseIf score <= 0 Then
                Return -1
            Else
                Return 1
            End If
        End Function

        Public Function BuildTree(pubchem As IEnumerable(Of MetaInfo)) As TreeSearch
            For Each meta As MetaInfo In pubchem
                Call metaTree.Add(meta, meta, False)
            Next

            Return Me
        End Function

        Public Function Search(term As MetaInfo) As MetaInfo()
            Dim result = metaTree.Find(term)

            If result Is Nothing Then
                Return {}
            Else
                Return result.Members
            End If
        End Function

    End Class
End Namespace
