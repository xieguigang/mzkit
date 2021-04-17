Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class ScanContour : Inherits Plot

    Dim scans As ms1_scan()

    Public Sub New(scans As ms1_scan(), theme As Theme)
        MyBase.New(theme)

        Me.scans = scans
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)

    End Sub
End Class
