Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Net.Http
Imports Task

Public Class frmLogin

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Call Process.Start("http://passport.biodeep.cn/register?lang=enUS")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call Close()
    End Sub

    ''' <summary>
    ''' login
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim account As String = TextBox1.Text
        Dim password As String = TextBox2.Text.MD5
        Dim post As New NameValueCollection

        Call post.Add("account", account)
        Call post.Add("password", password)

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

            Call Close()
        End If
    End Sub
End Class