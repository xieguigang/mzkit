Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Class mzPackPixel : Inherits PixelScan

    Public Overrides ReadOnly Property X As Integer
        Get
            Return pixel.X
        End Get
    End Property

    Public Overrides ReadOnly Property Y As Integer
        Get
            Return pixel.Y
        End Get
    End Property

    ReadOnly scan As ScanMS1
    ReadOnly pixel As Point

    Sub New(scan As ScanMS1)
        Me.scan = scan
        Me.pixel = Casting.PointParser(scan.scan_id.Match("\[\d+,\d+\]").GetStackValue("[", "]"))
    End Sub

    Public Overrides Function GetMs() As ms2()
        Return scan.GetMs.ToArray
    End Function
End Class
