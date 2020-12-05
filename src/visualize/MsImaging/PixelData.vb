Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Public Class PixelData

    Public Property x As Integer
    Public Property y As Integer
    Public Property intensity As Double
    Public Property level As Double

    Public Overrides Function ToString() As String
        Return $"Dim [{x},{y}] as intensity = {intensity}"
    End Function

    Public Shared Function ScalePixels(pixels As PixelData()) As PixelData()
        Dim intensityRange As DoubleRange = pixels _
            .Select(Function(p) p.intensity) _
            .Range
        Dim level As Double
        Dim levelRange As DoubleRange = New Double() {0, 1}

        For Each point As PixelData In pixels
            level = intensityRange.ScaleMapping(point.intensity, levelRange)
            point.level = level
        Next

        Return pixels
    End Function
End Class
