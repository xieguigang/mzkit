Public Class frmUserAgreement

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK

        Globals.Settings.licensed = True
        Globals.Settings.Save()

        Call Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
        Close()
    End Sub
End Class