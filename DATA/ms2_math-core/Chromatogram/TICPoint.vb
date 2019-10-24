Imports stdNum = System.Math

Namespace Chromatogram

    Public Class TICPoint : Implements ITimePoint

        Public Property mz As Double
        Public Property time As Double Implements ITimePoint.time
        Public Property intensity As Double Implements ITimePoint.intensity

        Public Overrides Function ToString() As String
            Return $"Dim [{mz.ToString("F3")}, {stdNum.Round(time, 1)}] = {intensity.ToString("G4")}"
        End Function

    End Class
End Namespace