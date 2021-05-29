Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public Class ibdPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
            Get
                Return i.x
            End Get
        End Property

        Public Overrides ReadOnly Property Y As Integer
            Get
                Return i.y
            End Get
        End Property

        ReadOnly i As ScanData
        ReadOnly raw As ibdReader

        Sub New(ibd As ibdReader, pixel As ScanData)
            i = pixel
            raw = ibd
        End Sub

        Public Overrides Function GetMs() As ms2()
            Return raw.GetMSMS(i)
        End Function

        Public Function ReadMz() As Double()
            Return raw.ReadArray(i.MzPtr)
        End Function

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Dim mzlist As Double() = raw.ReadArray(i.MzPtr)

            Return mz _
                .Any(Function(mzi)
                         Return mzlist.Any(Function(zzz) tolerance(zzz, mzi))
                     End Function)
        End Function
    End Class
End Namespace