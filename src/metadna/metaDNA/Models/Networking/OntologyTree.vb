Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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

    End Function
End Class
