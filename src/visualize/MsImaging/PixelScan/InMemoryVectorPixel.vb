Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public Class InMemoryVectorPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
        Public Overrides ReadOnly Property Y As Integer

        Public ReadOnly Property mz As Double()
        Public ReadOnly Property intensity As Double()

        Sub New(x As Integer, y As Integer, mz As Double(), into As Double())
            Me.X = x
            Me.Y = y
            Me.mz = mz
            Me.intensity = into
        End Sub

        Protected Friend Overrides Sub release()
            Erase _mz
            Erase _intensity
        End Sub

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Return mz.Any(Function(mzi) Me.mz.Any(Function(mz2) tolerance(mzi, mz2)))
        End Function

        Protected Friend Overrides Iterator Function GetMsPipe() As IEnumerable(Of ms2)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms2 With {
                    .mz = mz(i),
                    .intensity = intensity(i)
                }
            Next
        End Function
    End Class

End Namespace