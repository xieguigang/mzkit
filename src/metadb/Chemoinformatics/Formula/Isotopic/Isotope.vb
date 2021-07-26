Namespace Formula.IsotopicPatterns

    Public Class Isotope

        Public Property Mass As Double
        Public Property Prob As Double
        Public Property NumNeutrons As Integer

        Public Overrides Function ToString() As String
            Return $"[{NumNeutrons}] {Prob}"
        End Function

    End Class
End Namespace