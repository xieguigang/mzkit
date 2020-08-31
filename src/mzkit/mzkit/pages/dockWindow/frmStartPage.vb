Public Class frmStartPage

    Dim startPage As New PageStart

    Private Sub frmStartPage_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(startPage)
        startPage.Dock = DockStyle.Fill
        Me.Icon = My.Resources.chemistry

        Me.ShowIcon = True
        '    Me.ShowInTaskbar = True
    End Sub
End Class