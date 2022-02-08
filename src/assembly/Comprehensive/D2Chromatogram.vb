Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Class D2Chromatogram

    Public Property scan_time As Double
    Public Property intensity As Double
    Public Property d2chromatogram As ChromatogramTick()

    Public Overrides Function ToString() As String
        Return $"{intensity.ToString("G3")}@{scan_time.ToString("F2")}"
    End Function

    Public Shared Function EncodeCDF(file As Stream) As Boolean

    End Function

    Public Shared Iterator Function DecodeCDF(file As Stream) As IEnumerable(Of D2Chromatogram)

    End Function

End Class
