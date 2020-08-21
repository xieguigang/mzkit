Public Class frmSplashScreen

    Public Property isAboutScreen As Boolean = False

    Private Sub frmSplashScreen_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate

    End Sub

    Private Sub frmSplashScreen_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        If isAboutScreen Then
            Call Me.Close()
        End If
    End Sub
End Class
