Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Vip.Notification

<Package("notification")>
Module Notification

    <ExportAPI("success")>
    Public Sub success(message As String)
        Call Alert.ShowSucess(message)
    End Sub
End Module
