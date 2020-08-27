Public Class AppConfig : Implements ISaveSettings, IPageSettings

    Dim WithEvents colorPicker As New ThemeColorPicker

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings With {.rememberWindowsLocation = True}
        End If

        CheckBox1.Checked = Globals.Settings.ui.rememberWindowsLocation
    End Sub

    Public Sub SaveSettings() Implements ISaveSettings.SaveSettings
        Globals.Settings.Save()
    End Sub

    Public Sub ShowPage() Implements IPageSettings.ShowPage
        Call DirectCast(ParentForm, frmMain).ShowPage(DirectCast(ParentForm, frmMain).mzkitSettings)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Globals.Settings.ui.rememberWindowsLocation = CheckBox1.Checked
    End Sub

    Private Sub AppConfig_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddHandler colorPicker.ColorSelected, AddressOf selectColor
    End Sub

    Private Sub selectColor(sender As Object, e As ColorSelectedArg)

    End Sub
End Class

