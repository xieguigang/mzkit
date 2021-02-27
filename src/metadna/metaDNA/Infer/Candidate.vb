Namespace Infer

    Public Class Candidate

        Public Property precursorType As String
        Public Property ppm As Double
        Public Property score As Double
        Public Property pvalue As Double

        Public Property infer As InferLink

        Public Overrides Function ToString() As String
            Return pvalue
        End Function

    End Class
End Namespace