Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public MustInherit Class Networking

    Public MustOverride Function FindPartners(kegg_id As String) As IEnumerable(Of String)
    ''' <summary>
    ''' find reaction list for export report table
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Public MustOverride Function FindReactions(a As String, b As String) As NamedValue(Of String)()

End Class
