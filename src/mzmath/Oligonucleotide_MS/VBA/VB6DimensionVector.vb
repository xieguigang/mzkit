Imports Microsoft.VisualBasic.Serialization.JSON

Friend Class VB6DimensionVector

    Public Cells As Long()

    Default Public Property Item(i As Integer) As Long
        Get
            Return Cells(i)
        End Get
        Set(value As Long)
            Cells(i) = value
        End Set
    End Property

    Sub New(n As Integer)
        Cells = New Long(n) {}
    End Sub

    Public Overrides Function ToString() As String
        Return Cells.GetJson
    End Function
End Class
