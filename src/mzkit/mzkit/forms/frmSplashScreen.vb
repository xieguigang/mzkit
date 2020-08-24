Public Class frmSplashScreen

    Public Property isAboutScreen As Boolean = False

    Private Sub frmSplashScreen_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If isAboutScreen Then
            Call Me.Close()
        End If
    End Sub

    Private Sub frmSplashScreen_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus

    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://mzkit.org/")
    End Sub

    Private Sub frmSplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub frmSplashScreen_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawRectangle(New Pen(Color.Black, 1), New Rectangle(0, 0, Width, Height))
    End Sub
End Class
