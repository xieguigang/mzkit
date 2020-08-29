Public Class frmSettings

    Friend mzkitSettings As New PageSettings With {.Text = "Settings"}

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(mzkitSettings)
        mzkitSettings.Dock = DockStyle.Fill
    End Sub
End Class