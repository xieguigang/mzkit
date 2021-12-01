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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

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
        Dim intensityRange As DoubleRange = TIC2D.Select(Function(t) t.d2chromatogram).IteratesALL.IntensityArray.Range

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

                Call g.FillRectangle(colors(i), rect)
            Next
        Next
    End Sub
End Class
