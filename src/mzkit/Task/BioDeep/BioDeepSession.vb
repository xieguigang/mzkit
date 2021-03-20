Imports System.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.My

''' <summary>
''' 登录状态信息使用<see cref="SingletonHolder(Of BioDeepSession)"/>进行保存
''' </summary>
Public Class BioDeepSession

    Public Property cookieName As String
    Public Property ssid As String

    Private Function headerProvider() As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {{"Cookie", $"BIODEEP_USER_SESSION={ssid};"}}
    End Function

    ''' <summary>
    ''' 检查是否处于登录状态
    ''' </summary>
    ''' <returns></returns>
    Public Function CheckSession() As Boolean
        Dim url$ = "http://my.biodeep.cn/services/ping.vbs"
        Dim text As String = url.GET(headers:=headerProvider)
        Dim json As JsonObject = MessageParser.ParseMessage(text)
        Dim result As Boolean = json.success

        Return result
    End Function

    Public Function GetSessionInfo() As SessionInfo
        Dim result As JsonObject = Request(api:="http://my.biodeep.cn/services/session_info.vbs")

        If result.success Then
            Return result.CreateObject(Of SessionInfo)
        Else
            Return Nothing
        End If
    End Function

    Public Function Request(api As String, Optional headers As Dictionary(Of String, String) = Nothing) As JsonObject
        Dim sessionHeader As Dictionary(Of String, String) = headerProvider()

        If Not headers.IsNullOrEmpty Then
            For Each item In headers
                sessionHeader(item.Key) = item.Value
            Next
        End If

        Return api _
            .GET(headers:=sessionHeader) _
            .DoCall(AddressOf MessageParser.ParseMessage)
    End Function

    Public Function RequestStream(api As String, Optional headers As Dictionary(Of String, String) = Nothing) As Stream
        Dim sessionHeader As Dictionary(Of String, String) = headerProvider()

        If Not headers.IsNullOrEmpty Then
            For Each item In headers
                sessionHeader(item.Key) = item.Value
            Next
        End If

        Return api.GetRequestRaw(headers:=headers)
    End Function

End Class

