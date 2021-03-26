Imports System.ComponentModel
Imports WeifenLuo.WinFormsUI.Docking

Public Class AdjustParameters

    Dim applyNewParameters As Action(Of Object)

    Public Sub SetParameterObject(Of T As Class)(args As T, refresh As Action(Of T))
        applyNewParameters = Sub(obj) Call refresh(DirectCast(obj, T))
        propertyGrid.SelectedObject = args
        propertyGrid.Refresh()
    End Sub

    Private Sub propertyGrid_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles propertyGrid.PropertyValueChanged
        Call applyNewParameters(propertyGrid.SelectedObject)
    End Sub

    Private Sub AdjustParameters_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub
End Class