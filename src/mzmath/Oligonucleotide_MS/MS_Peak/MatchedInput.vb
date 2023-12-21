Imports Microsoft.VisualBasic.Serialization.JSON

Public Class MatchedInput

    Public Property ObservedMass As Double
    Public Property Match As String()

    Public Overrides Function ToString() As String
        Return $"{ObservedMass}: {Match.GetJson}"
    End Function

End Class