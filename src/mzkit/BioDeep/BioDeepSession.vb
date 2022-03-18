#Region "Microsoft.VisualBasic::80ff1c3b1ebd257e7a67ec82bc675568, mzkit\src\mzkit\BioDeep\BioDeepSession.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 112
    '    Code Lines: 80
    ' Comment Lines: 9
    '   Blank Lines: 23
    '     File Size: 4.09 KB


    ' Class BioDeepSession
    ' 
    '     Properties: cookieName, ssid
    ' 
    '     Function: CheckSession, GetSessionInfo, headerProvider, Login, Request
    '               RequestStream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.IO
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Net.Http

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
            Return DirectCast(result!info, JsonObject).CreateObject(Of SessionInfo)
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
        Dim buffer As New MemoryStream

        If Not headers.IsNullOrEmpty Then
            For Each item In headers
                sessionHeader(item.Key) = item.Value
            Next
        End If

        Using webResponse As Stream = api.GetRequestRaw(headers:=sessionHeader)
            Dim chunk As Byte() = New Byte(4096 - 1) {}
            Dim nread As i32 = 0

            Do While True
                If (nread = webResponse.Read(chunk, Scan0, chunk.Length)) <= 0 Then
                    Exit Do
                Else
                    buffer.Write(chunk, Scan0, nread)
                End If
            Loop

            Call buffer.Seek(Scan0, SeekOrigin.Begin)
        End Using

        Return buffer '.UnGzipStream
    End Function

    Public Shared Function Login(account As String, passwordMd5 As String) As String
        Dim post As New NameValueCollection

        Call post.Add("account", account)
        Call post.Add("password", passwordMd5)

        Dim result As WebResponseResult = $"http://passport.biodeep.cn/passport/verify.vbs".POST(params:=post)
        Dim json As JsonObject = New JsonParser().OpenJSON(result.html)

        If json!code.AsString <> 0 Then
            Call MessageBox.Show("Account not found or incorrect password...", "BioDeep Login", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            ' session_id
            ' cookie_name
            json = json!debug

            SingletonHolder(Of BioDeepSession).Instance.cookieName = json!cookie_name.AsString
            SingletonHolder(Of BioDeepSession).Instance.ssid = json!session_id.AsString

            Return SingletonHolder(Of BioDeepSession).Instance.ssid
        End If

        Return Nothing
    End Function
End Class
