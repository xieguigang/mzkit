Public Class PixelData

    Public Property x As Integer
    Public Property y As Integer
    Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return $"Dim [{x},{y}] as intensity = {intensity}"
    End Function

End Class
