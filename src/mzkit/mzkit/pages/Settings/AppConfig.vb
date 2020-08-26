Public Class AppConfig
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Globals.Settings.ui.rememberWindowsLocation = CheckBox1.Checked
    End Sub
End Class

