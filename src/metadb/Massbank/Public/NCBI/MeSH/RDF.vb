Namespace NCBI.MeSH

    Public Class Term

        Public Property term As String
        Public Property tree As String()

        Public Overrides Function ToString() As String
            Return $"[{tree.JoinBy("->")}] {term}"
        End Function

    End Class

    Public Class Tree

        Public Property term As Term
        Public Property childs As New Dictionary(Of String, Tree)

        Public Overrides Function ToString() As String
            Return term.ToString
        End Function

    End Class
End Namespace