Namespace Infer

    Public Class CandidateInfer

        Public Property kegg_id As String
        Public Property infers As Candidate()

        Public Overrides Function ToString() As String
            Return $"{kegg_id}: {infers.Select(Function(c) c.pvalue).Min}"
        End Function
    End Class
End Namespace