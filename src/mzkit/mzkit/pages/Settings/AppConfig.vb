Public Class AppConfig : Implements ISaveSettings, IPageSettings

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings

    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Throw New NotImplementedException()
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call DirectCast(ParentForm, frmMain).ShowPage(DirectCast(ParentForm, frmMain).mzkitSettings)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Globals.Settings.ui.rememberWindowsLocation = CheckBox1.Checked
    End Sub
End Class

