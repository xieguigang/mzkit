Namespace NCBI.MeSH

    Public Class Term

        Public Property term As String
        Public Property tree As String()

        Public Overrides Function ToString() As String
            Return $"[{tree.JoinBy("->")}] {term}"
        End Function

    End Class
End Namespace