Imports stdNum = System.Math

Namespace Chromatogram

    Public Class TICPoint

        Public Property mz As Double
        Public Property time As Double
        Public Property intensity As Double

        Public Overrides Function ToString() As String
            Return $"Dim [{mz.ToString("F3")}, {stdNum.Round(time, 1)}] = {intensity.ToString("G4")}"
        End Function

    End Class
End Namespace