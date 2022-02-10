Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class PeakTablePlot : Inherits Plot

    ReadOnly peakSet As PeakSet
    ReadOnly mapLevels As Integer = 64
    ReadOnly cut As Double = 0.65

    Public Sub New(peakSet As PeakSet, theme As Theme)
        MyBase.New(theme)

        Me.peakSet = peakSet.Norm
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
        Dim allIntensity As Double() = peakSet.peaks.Select(Function(pk) pk.Properties.Values).IteratesALL.ToArray
        Dim qcut As Double = TrIQ.FindThreshold(allIntensity, Me.cut, N:=mapLevels)
        Dim valueRange As DoubleRange = New Double() {allIntensity.Min, qcut}
        Dim color As Integer
        Dim strokePen As Pen = Stroke.TryParse(theme.axisStroke).GDIObject
        Dim scaler As New DataScaler With {
            .region = rect,
            .X = scaleX,
            .Y = d3js.scale.linear.domain(values:={0.0, 1.0}).range(integers:={rect.Top, rect.Bottom})
        }
        Dim tickFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)

        Call Axis.DrawX(g, strokePen, "Retention Time(s)", scaler, XAxisLayoutStyles.Bottom, 0, Nothing, theme.axisLabelCSS, Brushes.Black, tickFont, Brushes.Black)

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
