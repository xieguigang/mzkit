Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class GCxGCTIC2DPlot : Inherits Plot

    ReadOnly TIC2D As D2Chromatogram()

    Public Sub New(TIC2D As D2Chromatogram(), theme As Theme)
        Call MyBase.New(theme)

        Me.TIC2D = TIC2D
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Throw New NotImplementedException()
    End Sub
End Class
