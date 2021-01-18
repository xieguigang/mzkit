Imports Microsoft.VisualBasic.Math.SignalProcessing

Namespace UV

    Public Class UVScanPoint

        Public Property wavelength As Double
        Public Property intensity As Double

        Public Shared Iterator Function FromSignal(signal As GeneralSignal) As IEnumerable(Of UVScanPoint)
            Dim x As Double() = signal.Measures
            Dim y As Double() = signal.Strength

            For i As Integer = 0 To x.Length - 1
                Yield New UVScanPoint With {
                    .intensity = y(i),
                    .wavelength = x(i)
                }
            Next
        End Function

    End Class
End Namespace