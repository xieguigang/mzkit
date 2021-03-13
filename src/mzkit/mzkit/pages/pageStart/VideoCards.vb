Public Class VideoCards

    Public Property url As String

    Private Sub LinkLabel1_LinkClicked() Handles LinkLabel1.LinkClicked, PictureBox1.Click
        Call Process.Start(url)
    End Sub
End Class
