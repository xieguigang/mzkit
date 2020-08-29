Imports System.ComponentModel

Public Class frmRsharp

    Dim console As New ConsoleControl.ConsoleControl

    Private Sub frmRsharp_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmRsharp_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(console)
        console.Dock = DockStyle.Fill
    End Sub
End Class