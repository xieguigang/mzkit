Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Module MessageParser

    Public Function ParseMessage(returns As String) As JsonObject
        Return New JsonParser().OpenJSON(returns)
    End Function

    <Extension>
    Public Function success(msg As JsonObject) As Boolean
        Return msg.ContainsKey("code") AndAlso msg!code.AsString = "0"
    End Function

    <Extension>
    Public Function getMsgString(msg As JsonObject) As String
        Return msg.ContainsKey("info") AndAlso msg!info.AsString
    End Function
End Module
