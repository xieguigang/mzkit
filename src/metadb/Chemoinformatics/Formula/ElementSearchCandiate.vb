Namespace Formula

    Public Class ElementSearchCandiate

        Public Property Element As String
        Public Property MinCount As Integer = 0
        Public Property MaxCount As Integer = 100

        Public Overrides Function ToString() As String
            Return $"{Element} [{MinCount}, {MaxCount}]"
        End Function

    End Class
End Namespace