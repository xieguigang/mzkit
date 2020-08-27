Imports RibbonLib

Public Class AppConfig : Implements ISaveSettings, IPageSettings

    Dim WithEvents colorPicker As New ThemeColorPicker

    Public Sub LoadSettings() Implements ISaveSettings.LoadSettings
        If Globals.Settings.ui Is Nothing Then
            Globals.Settings.ui = New UISettings With {.rememberWindowsLocation = True}
        End If

        CheckBox1.Checked = Globals.Settings.ui.rememberWindowsLocation

        If Globals.Settings.ui.background.IsNullOrEmpty Then
            Dim colors As RibbonColors = DirectCast(ParentForm, frmMain).Ribbon1.GetColors

            PictureBox1.BackColor = colors.BackgroundColor
            PictureBox2.BackColor = colors.HighlightColor
            PictureBox3.BackColor = colors.TextColor

            Globals.Settings.ui.background = {colors.BackgroundColor.R, colors.BackgroundColor.G, colors.BackgroundColor.B}
            Globals.Settings.ui.highlight = {colors.HighlightColor.R, colors.HighlightColor.G, colors.HighlightColor.B}
            Globals.Settings.ui.text = {colors.TextColor.R, colors.TextColor.G, colors.TextColor.B}
        End If
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
        Controls.Add(colorPicker)

        colorPicker.Location = New Point(50, 50)
        PictureBox1.BorderStyle = BorderStyle.FixedSingle

        AddHandler colorPicker.ColorSelected, AddressOf selectColor
    End Sub

    Private Sub selectColor(sender As Object, e As ColorSelectedArg) Handles colorPicker.ColorSelected
        Dim color As Integer() = {e.R, e.G, e.B}

        If PictureBox1.BorderStyle = BorderStyle.FixedSingle Then
            Globals.Settings.ui.background = color
            PictureBox1.BackColor = e.Color
        ElseIf PictureBox2.BorderStyle = BorderStyle.FixedSingle Then
            Globals.Settings.ui.highlight = color
            PictureBox2.BackColor = e.Color
        ElseIf PictureBox3.BorderStyle = BorderStyle.FixedSingle Then
            Globals.Settings.ui.text = color
            PictureBox3.BackColor = e.Color
        End If

        Globals.Settings.ui.setColors(DirectCast(ParentForm, frmMain).Ribbon1)
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        PictureBox1.BorderStyle = BorderStyle.FixedSingle
        PictureBox2.BorderStyle = BorderStyle.None
        PictureBox3.BorderStyle = BorderStyle.None
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        PictureBox1.BorderStyle = BorderStyle.None
        PictureBox2.BorderStyle = BorderStyle.FixedSingle
        PictureBox3.BorderStyle = BorderStyle.None
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        PictureBox1.BorderStyle = BorderStyle.None
        PictureBox2.BorderStyle = BorderStyle.None
        PictureBox3.BorderStyle = BorderStyle.FixedSingle
    End Sub
End Class

