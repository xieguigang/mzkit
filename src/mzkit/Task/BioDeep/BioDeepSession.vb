Imports Microsoft.VisualBasic.Linq

Public Class BioDeepSession

    Public Property cookieName As String
    Public Property ssid As String

    Private Function headerProvider() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {{"Cookie", $"BIODEEP_USER_SESSION={ssid};"}}
    End Function

    Public Function CheckSession() As Boolean
        Dim url$ = "http://my.biodeep.cn/services/ping.vbs"
        Dim result As Boolean = url _
            .GET(headers:=headerProvider) _
            .DoCall(AddressOf MessageParser.ParseMessage) _
            .success

        Return result
    End Function

End Class
