Public Class Properties

    Public Property Key As String
    Public Property Value As String

    Public Overrides Function ToString() As String
        Return $"[{Key}] {Value}"
    End Function
End Class
