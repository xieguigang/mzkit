Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class Project

    Public Property sptype As String
    Public Property pdata As pdata()
    Public Property acqu As NamedValue(Of String())()
    Public Property acqus As NamedValue(Of String())()
    Public Property AnalysisParameter As AnalysisParameter

    Public Shared Function FromResultFolder(dir As String) As Project
        Return New Project With {
            .sptype = $"{dir}/sptype".ReadAllText
        }
    End Function
End Class

