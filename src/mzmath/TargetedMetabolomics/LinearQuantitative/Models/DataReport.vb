Namespace LinearQuantitative

    ''' <summary>
    ''' table model for the linear quantification result
    ''' </summary>
    Public Class DataReport

        Public Property ID As String
        Public Property name As String
        Public Property ISTD As String
        Public Property linear As String
        Public Property R2 As Double
        Public Property samples As Dictionary(Of String, Double)

        Default Public Property Value(name As String) As Double
            Get
                Return samples.TryGetValue(name)
            End Get
            Set(value As Double)
                samples(name) = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"f({ID}) = {linear}"
        End Function

    End Class
End Namespace