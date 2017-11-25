Namespace TMIC.HMDB

    Public Class disease

        Public Property name As String
        Public Property omim_id As String
        Public Property references As reference()

    End Class

    Public Structure reference
        Public Property reference_text As String
        Public Property pubmed_id As String

        Public Overrides Function ToString() As String
            Return reference_text
        End Function
    End Structure
End Namespace