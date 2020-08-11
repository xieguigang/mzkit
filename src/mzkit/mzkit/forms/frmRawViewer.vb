Imports Task

Public Class frmRawViewer

    Public rawFile As Raw

    Private Sub frmRawViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub frmRawViewer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        DirectCast(ParentForm, frmMain).ToolStripStatusLabel.Text = rawFile.source
    End Sub
End Class