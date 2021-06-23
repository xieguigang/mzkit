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

        Dim memoryCache As ms2()

        Sub New(ibd As ibdReader, pixel As ScanData)
            i = pixel
            raw = ibd
        End Sub

        Public Overrides Function GetMs() As ms2()
            If memoryCache.IsNullOrEmpty Then
                memoryCache = raw.GetMSMS(i)
            End If

            Return memoryCache
        End Function

        Public Function ReadMz() As Double()
            If memoryCache.IsNullOrEmpty Then
                Return raw.ReadArray(i.MzPtr)
            Else
                Return (From m As ms2 In memoryCache Select m.mz).ToArray
            End If
        End Function

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Dim mzlist As Double() = ReadMz()

            Return mz _
                .Any(Function(mzi)
                         Return mzlist.Any(Function(zzz) tolerance(zzz, mzi))
                     End Function)
        End Function

        Protected Friend Overrides Sub release()
            Erase memoryCache
        End Sub
    End Class
End Namespace