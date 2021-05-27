Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

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
End Class
