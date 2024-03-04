Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class OntologyTree : Inherits Networking

    ReadOnly depth_distance As Integer

    Sub New(Optional depth_distance As Integer = 3)
        Me.depth_distance = depth_distance
    End Sub

    ''' <summary>
    ''' find all related nodes which is nearby the given node in a specific search depth distance.
    ''' </summary>
    ''' <param name="kegg_id"></param>
    ''' <returns></returns>
    Public Overrides Function FindPartners(kegg_id As String) As IEnumerable(Of String)

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
