Imports System.ComponentModel

Public Class frmDockDocument

    Friend pages As New List(Of Control)

    Public Sub addPage(ParamArray pageList As Control())
        For Each page As Control In pageList
            Controls.Add(page)
            pages.Add(page)
            page.Dock = DockStyle.Fill
        Next
    End Sub

    Private Sub frmDockDocument_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = WeifenLuo.WinFormsUI.Docking.DockState.Document
    End Sub

    Private Sub frmDockDocument_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "BioNovoGene M/Z Data Toolkit"
    End Sub
End Class