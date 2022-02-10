Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class PeakTablePlot : Inherits Plot

    ReadOnly peakSet As PeakSet

    Public Sub New(peakSet As PeakSet, theme As Theme)
        MyBase.New(theme)

        Me.peakSet = peakSet
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)

    End Sub
End Class
