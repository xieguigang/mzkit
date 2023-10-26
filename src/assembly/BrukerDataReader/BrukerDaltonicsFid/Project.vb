Imports System.Xml.Serialization

Public Class Project

    Public Property sptype As String
    Public Property pdata As pdata()
    Public Property AnalysisParameter As AnalysisParameter

    Public Shared Function FromResultFolder(dir As String) As Project

    End Function
End Class

