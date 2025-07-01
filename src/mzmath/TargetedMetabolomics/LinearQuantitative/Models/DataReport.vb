Namespace LinearQuantitative

    ''' <summary>
    ''' table model for the linear quantification result
    ''' </summary>
    Public Class DataReport

        Public Property ID As String
        Public Property name As String
        Public Property linear As String
        Public Property R2 As Double
        Public Property samples As Dictionary(Of String, Double)

    End Class
End Namespace