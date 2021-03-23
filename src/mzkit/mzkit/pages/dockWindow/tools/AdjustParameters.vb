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
End Class