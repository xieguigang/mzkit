Friend Structure IonIndex

    Public mz As Double
    Public node As Integer

    Public Overrides Function ToString() As String
        Return $"[{mz.ToString("F4")}] -> {node}"
    End Function

End Structure
