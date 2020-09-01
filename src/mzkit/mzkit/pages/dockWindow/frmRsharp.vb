Imports System.ComponentModel

Public Class frmRsharp

    Friend Routput As New TextBox

    Private Sub frmRsharp_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmRsharp_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(Routput)

        Routput.Multiline = True
        Routput.Dock = DockStyle.Fill
        Routput.ReadOnly = True
        Routput.Font = New Font("Consolas", 10, FontStyle.Regular)
        Routput.ScrollBars = ScrollBars.Vertical

        TabText = "R# Terminal"
        Me.Icon = My.Resources.Rscript

        Me.ShowIcon = True
        '  Me.ShowInTaskbar = True
    End Sub
End Class
