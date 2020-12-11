Imports Task

Public Class frmTweaks

    Dim params As New PlotProperty

    Private Sub frmTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        PropertyGrid1.SelectedObject = params
    End Sub
End Class