Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class PeakTablePlot : Inherits Plot

    ReadOnly peakSet As PeakSet
    ReadOnly mapLevels As Integer = 64

    Public Sub New(peakSet As PeakSet, theme As Theme)
        MyBase.New(theme)

        Me.peakSet = peakSet
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim xTicks As Double() = peakSet.peaks.Select(Function(pk) pk.rt).Range.CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(values:=xTicks).range(integers:={rect.Left, rect.Right})
        Dim sampleNames As String() = peakSet.sampleNames
        Dim dy As Double = rect.Height / sampleNames.Length
        Dim idFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
        Dim lbSize As SizeF
        Dim y As Double = rect.Top
        Dim x As Double
        Dim dot As RectangleF
        Dim colors As SolidBrush() = Designer.GetColors(theme.colorSet, mapLevels).Select(Function(c) New SolidBrush(c)).ToArray
        Dim indexRange As New DoubleRange(0, mapLevels)
        Dim valueRange As DoubleRange = peakSet.peaks.Select(Function(pk) pk.Properties.Values).IteratesALL.Range
        Dim color As Integer

        ' for each sample as matrix row
        For Each sampleId As String In sampleNames
            lbSize = g.MeasureString(sampleId, idFont)
            g.DrawString(sampleId, idFont, Brushes.Black, New PointF(rect.Left - lbSize.Width - 10, y + (dy - lbSize.Height) / 2))

            For Each peak In peakSet.peaks
                x = scaleX(peak.rt)
                dot = New RectangleF With {
                    .X = x,
                    .Y = y,
                    .Width = 5,
                    .Height = dy
                }
                color = CInt(valueRange.ScaleMapping(peak(sampleId), indexRange))

                If color < 0 Then color = 0
                If color >= colors.Length Then color = colors.Length - 1

                Call g.FillRectangle(colors(color), rect:=dot)
            Next

            y += dy
        Next
    End Sub
End Class
