Public Class frmUserAgreement

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Globals.Settings.licensed = True
        Globals.Settings.Save()

        Call Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        End
    End Sub
End Class