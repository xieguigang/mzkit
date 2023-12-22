Imports Microsoft.VisualBasic.Serialization.JSON

Friend Class VB6DimensionVector(Of T)

    Public Cells As T()

    Default Public Property Item(i As Integer) As T
        Get
            Return Cells(i)
        End Get
        Set
            Cells(i) = Value
        End Set
    End Property

    Sub New(n As Integer)
        Cells = New T(n) {}
    End Sub

    Public Overrides Function ToString() As String
        Return Cells.GetJson
    End Function
End Class
