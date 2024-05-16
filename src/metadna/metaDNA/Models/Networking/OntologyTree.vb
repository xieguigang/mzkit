#Region "Microsoft.VisualBasic::1b55b780b1f4be62f86082fd67923b5e, metadna\metaDNA\Models\Networking\OntologyTree.vb"

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

    '   Total Lines: 86
    '    Code Lines: 54
    ' Comment Lines: 22
    '   Blank Lines: 10
    '     File Size: 3.10 KB


    ' Class OntologyTree
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CheckLineage, FindPartners, FindReactions, GetChilds
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.genomics.foundation.OBO_Foundry.Tree

''' <summary>
''' 
''' </summary>
''' <remarks>
''' working based on the generic ontology <see cref="GenericTree"/> node.
''' </remarks>
Public Class OntologyTree : Inherits Networking

    ReadOnly depth_distance As Integer
    ReadOnly ontology As Dictionary(Of String, GenericTree)

    Sub New(file As OBOFile, Optional depth_distance As Integer = 3)
        Me.depth_distance = depth_distance
        Me.ontology = file.GetRawTerms.BuildTree
    End Sub

    ''' <summary>
    ''' find all related nodes which is nearby the given node in a specific search depth distance.
    ''' </summary>
    ''' <param name="kegg_id"></param>
    ''' <returns></returns>
    Public Overrides Iterator Function FindPartners(kegg_id As String) As IEnumerable(Of String)
        For Each parent In ontology(kegg_id).is_a.SafeQuery
            ' get childs in the same parent
            If Not parent.direct_childrens Is Nothing Then
                For Each child In parent.direct_childrens.Values
                    If child.ID <> kegg_id Then
                        Yield child.ID
                    End If
                Next
            End If
        Next

        ' populate all childs
        For Each child As GenericTree In GetChilds(node:=ontology(kegg_id))
            If child.ID <> kegg_id Then
                Yield child.ID
            End If
        Next
    End Function

    Private Iterator Function GetChilds(node As GenericTree) As IEnumerable(Of GenericTree)
        Yield node

        If Not node.direct_childrens Is Nothing Then
            For Each child As GenericTree In node.direct_childrens.Values
                For Each c As GenericTree In GetChilds(child)
                    Yield c
                Next
            Next
        End If
    End Function

    ''' <summary>
    ''' try to find the common term between two node
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' find common term in a given search depth
    ''' </remarks>
    Public Overrides Function FindReactions(a As String, b As String) As NamedValue(Of String)()
        Dim p1 = ontology(a)
        Dim p2 = ontology(b)

        Return CheckLineage(p1, p2).JoinIterates(CheckLineage(p2, p1))
    End Function

    Private Iterator Function CheckLineage(ancestors As GenericTree, child As GenericTree) As IEnumerable(Of NamedValue(Of String))
        For Each parent In child.is_a.SafeQuery
            If parent.ID = ancestors.ID Then
                Yield New NamedValue(Of String)(parent.ID, parent.name)
            Else
                For Each hit As NamedValue(Of String) In CheckLineage(ancestors, parent)
                    Yield hit
                Next
            End If
        Next
    End Function
End Class
