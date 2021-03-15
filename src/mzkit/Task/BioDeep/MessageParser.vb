Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Module MessageParser

    Public Function ParseMessage(returns As String) As JsonObject
        Return New JsonParser().OpenJSON(returns)
    End Function

    <Extension>
    Public Function success(msg As JsonObject) As Boolean
        If msg Is Nothing Then
            Return False
        Else
            Return msg.ContainsKey("code") AndAlso msg!code.AsString = "0"
        End If
    End Function

    <Extension>
    Public Function getMsgString(msg As JsonObject) As String
        If msg.ContainsKey("info") Then
            Return msg!info.AsString
        Else
            Return Nothing
        End If
    End Function
End Module
