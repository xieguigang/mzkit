Namespace Infer

    Public Class CandidateInfer

        Public Property kegg_id As String
        Public Property infers As Candidate()

        Public Overrides Function ToString() As String
            Return kegg_id
        End Function

    End Class

    Public Class Candidate

        Public Property precursorType As String
        Public Property ppm As Double
        Public Property score As Double
        Public Property pvalue As Double

        Public Property infer As InferLink

    End Class
End Namespace