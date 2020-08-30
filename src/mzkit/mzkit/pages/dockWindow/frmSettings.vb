Imports System.ComponentModel

Public Class frmSettings

    Friend mzkitSettings As New PageSettings With {.Text = "Settings"}

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(mzkitSettings)
        mzkitSettings.Dock = DockStyle.Fill
        Me.Text = "Application Settings"
        Me.Icon = My.Resources.settings

        Me.ShowIcon = True
        '  Me.ShowInTaskbar = True
    End Sub

    Private Sub frmSettings_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Hidden
    End Sub
End Class