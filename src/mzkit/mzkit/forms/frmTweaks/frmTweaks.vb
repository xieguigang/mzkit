Imports Task

Public Class frmTweaks

    Dim params As New PlotProperty

    Public Property draw As Action

    Private Sub frmTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        PropertyGrid1.SelectedObject = params
        PropertyGrid1.Refresh()

        Me.TabText = "Plot Styles"
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        ' 进行重绘？
        If Not draw Is Nothing Then
            Call _draw()
        End If
    End Sub
End Class