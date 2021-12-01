Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS

''' <summary>
''' GCxGC Imaging
''' </summary>
Public Class GCxGCTIC2DPlot : Inherits Plot

    ReadOnly TIC2D As D2Chromatogram()

    Public Sub New(TIC2D As D2Chromatogram(), theme As Theme)
        Call MyBase.New(theme)

        Me.TIC2D = TIC2D
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim xTicks As Vector = TIC2D.Select(Function(t) t.scan_time).CreateAxisTicks.AsVector
        Dim yTicks As Vector = TIC2D.Select(Function(t) t.d2chromatogram).IteratesALL.TimeArray.CreateAxisTicks.AsVector
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(xTicks).range(integers:={rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(yTicks).range(integers:={rect.Top, rect.Bottom})
        Dim scale As New DataScaler() With {
            .AxisTicks = (xTicks, yTicks),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim colors As SolidBrush() = Designer _
            .GetColors(theme.colorSet, 50) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim dw As Double = rect.Width / TIC2D.Length
        Dim dh As Double = rect.Height / TIC2D(Scan0).d2chromatogram.Length
        Dim i As Integer
        Dim index As New DoubleRange(0, colors.Length - 1)
        Dim allIntensity As Vector = TIC2D.Select(Function(t) t.d2chromatogram).IteratesALL.IntensityArray
        Dim intensityRange As New DoubleRange(allIntensity.Min, TrIQ.FindThreshold(allIntensity, 0.9))

        Call Axis.DrawAxis(g, canvas, scale,
                           showGrid:=theme.drawGrid,
                           xlabel:=xlabel,
                           ylabel:=ylabel,
                           labelFont:=theme.axisLabelCSS,
                           axisStroke:=theme.axisStroke,
                           gridFill:=theme.gridFill,
                           gridX:=theme.gridStrokeX,
                           gridY:=theme.gridStrokeY,
                           XtickFormat:=theme.XaxisTickFormat,
                           YtickFormat:=theme.YaxisTickFormat,
                           tickFontStyle:=theme.axisTickCSS
        )

        For Each col As D2Chromatogram In TIC2D
            Dim x As Double = scaleX(col.scan_time)

            For Each cell As ChromatogramTick In col.d2chromatogram
                rect = New Rectangle() With {
                    .X = x,
                    .Y = scale.TranslateY(cell.Time),
                    .Width = dw,
                    .Height = dh
                }
                i = intensityRange.ScaleMapping(cell.Intensity, index)

                If i >= colors.Length Then
                    Call g.FillRectangle(colors.Last, rect)
                Else
                    Call g.FillRectangle(colors(i), rect)
                End If
            Next
        Next

        Dim width = canvas.Width * 0.1
        Dim legendLayout As New Rectangle(canvas.Width - width - canvas.Padding.Right / 3, canvas.Padding.Top, width, canvas.Height * 0.3)

        Call DrawMainTitle(g, canvas.PlotRegion)
        Call g.ColorMapLegend(
            layout:=legendLayout,
            designer:=colors,
            ticks:=allIntensity.CreateAxisTicks,
            titleFont:=CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi),
            title:="Intensity Scale",
            tickFont:=CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi),
            tickAxisStroke:=Stroke.TryParse(theme.axisTickStroke).GDIObject,
            format:="G2"
        )
    End Sub
End Class
