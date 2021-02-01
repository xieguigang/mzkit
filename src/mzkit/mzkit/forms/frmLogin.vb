Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.Net.Http

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

        Call post.Add("", account)
        Call post.Add("", password)

        Dim result As WebResponseResult = $"http://passport.biodeep.cn/passport/verify.vbs".POST(params:=post)

    End Sub
End Class