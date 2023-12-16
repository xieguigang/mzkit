Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' helper for simulate vb6 array
''' </summary>
Friend Class Dim4

    Public Cells As Long()

    Default Public Property Item(i As Integer) As Long
        Get
            Return Cells(i)
        End Get
        Set(value As Long)
            Cells(i) = value
        End Set
    End Property

    Sub New()
        Cells = New Long(4) {}
    End Sub

    Public Overrides Function ToString() As String
        Return Cells.GetJson
    End Function
End Class