Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public Class InMemoryPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
        Public Overrides ReadOnly Property Y As Integer

        Dim data As ms2()

        Sub New(x As Integer, y As Integer, data As ms2())
            Me.X = x
            Me.Y = y
            Me.data = data
        End Sub

        Protected Friend Overrides Sub release()
            Erase data
        End Sub

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return data.Any(Function(x) mz.Any(Function(mzi) tolerance(mzi, x.mz)))
        End Function

        Public Overrides Function GetMs() As ms2()
            Return data
        End Function

        Protected Friend Overrides Function GetMsPipe() As IEnumerable(Of ms2)
            Return data
        End Function
    End Class
End Namespace