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
    ReadOnly mapLevels As Integer

    Public Sub New(TIC2D As D2Chromatogram(), q As Double, mapLevels As Integer, theme As Theme)
        Call MyBase.New(theme)

        Me.TIC2D = cutSignal(TIC2D, q, mapLevels).ToArray
        Me.mapLevels = mapLevels
    End Sub

    Private Shared Iterator Function cutSignal(gcxgc As D2Chromatogram(), q As Double, levels As Integer) As IEnumerable(Of D2Chromatogram)
        Dim qcut As Double = TrIQ.FindThreshold(gcxgc.Select(Function(t) t.chromatogram).IteratesALL.Select(Function(t) t.Intensity), q, N:=levels)

        For Each scan As D2Chromatogram In gcxgc
            Yield New D2Chromatogram With {
                .intensity = scan.intensity,
                .scan_time = scan.scan_time,
                .chromatogram = scan _
                    .chromatogram _
                    .Select(Function(d)
                                Return New ChromatogramTick With {
                                    .Time = d.Time,
                                    .Intensity = If(d.Intensity > qcut, qcut, d.Intensity)
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim xTicks As Vector = TIC2D.Select(Function(t) t.scan_time).CreateAxisTicks.AsVector
        Dim yTicks As Vector = TIC2D.Select(Function(t) t.chromatogram).IteratesALL.TimeArray.CreateAxisTicks.AsVector
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(values:=xTicks).range(integers:={rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(values:=yTicks).range(integers:={rect.Top, rect.Bottom})
        Dim scale As New DataScaler() With {
            .AxisTicks = (xTicks, yTicks),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim colors As SolidBrush() = Designer _
            .GetColors(theme.colorSet, mapLevels) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim dw As Double = rect.Width / TIC2D.Length + 1
        Dim dh As Double = rect.Height / TIC2D(Scan0).chromatogram.Length + 1
        Dim index As New DoubleRange(0, colors.Length - 1)
        Dim allIntensity As Vector = TIC2D.Select(Function(t) t.chromatogram).IteratesALL.IntensityArray
        Dim intensityRange As New DoubleRange(allIntensity)

        allIntensity = (allIntensity * 10 ^ 40).CreateAxisTicks.AsVector / (10 ^ 40)

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

        If intensityRange.Length > 0.0 Then
            Call FillHeatMap(g, TIC2D, dw, dh, scale, intensityRange, index, colors)
        End If

        Dim width = canvas.Width * 0.1
        Dim legendLayout As New Rectangle(
            x:=canvas.Width - width - canvas.Padding.Right / 3,
            y:=canvas.Padding.Top,
            width:=width,
            height:=canvas.Height * 0.3
        )

        Call DrawMainTitle(g, canvas.PlotRegion)
        Call g.ColorMapLegend(
            layout:=legendLayout,
            designer:=colors,
            ticks:=allIntensity,
            titleFont:=CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi),
            title:="Intensity Scale",
            tickFont:=CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi),
            tickAxisStroke:=Stroke.TryParse(theme.axisTickStroke).GDIObject,
            format:="G3"
        )
    End Sub

    Public Shared Sub FillHeatMap(g As IGraphics,
                                  TIC2D As IEnumerable(Of D2Chromatogram),
                                  dw As Double,
                                  dh As Double,
                                  scale As DataScaler,
                                  intensityRange As DoubleRange,
                                  index As DoubleRange,
                                  colors As SolidBrush())

        For Each col As D2Chromatogram In TIC2D
            Dim x As Double = scale.TranslateX(col.scan_time)
            Dim i As Integer
            Dim rect As RectangleF

            For Each cell As ChromatogramTick In col.chromatogram
                rect = New RectangleF() With {
                    .X = x,
                    .Y = scale.TranslateY(cell.Time),
                    .Width = dw,
                    .Height = dh
                }
                i = intensityRange.ScaleMapping(If(cell.Intensity > intensityRange.Max, intensityRange.Max, cell.Intensity), index)
                i = index.Max - i

                If i >= colors.Length Then
                    Call g.FillRectangle(colors.Last, rect)
                ElseIf i <= 0 Then
                    Call g.FillRectangle(colors(Scan0), rect)
                Else
                    Call g.FillRectangle(colors(i), rect)
                End If
            Next
        Next
    End Sub
End Class
