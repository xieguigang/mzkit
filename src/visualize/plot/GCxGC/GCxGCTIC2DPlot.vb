#Region "Microsoft.VisualBasic::9a73fc179e98243c119643fde4fe96f1, mzkit\src\visualize\plot\GCxGC\GCxGCTIC2DPlot.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 164
    '    Code Lines: 142
    ' Comment Lines: 5
    '   Blank Lines: 17
    '     File Size: 7.12 KB


    ' Class GCxGCTIC2DPlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: cutSignal
    ' 
    '     Sub: FillHeatMap, PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS

''' <summary>
''' GCxGC Imaging
''' </summary>
Public Class GCxGCTIC2DPlot : Inherits Plot

    ReadOnly TIC2D As D2Chromatogram()
    ReadOnly mapLevels As Integer

    Public Sub New(TIC2D As D2Chromatogram(), ql As Double, qh As Double, mapLevels As Integer, theme As Theme)
        Call MyBase.New(theme)

        Me.TIC2D = cutSignal(TIC2D, ql, qh, mapLevels).ToArray
        Me.mapLevels = mapLevels
    End Sub

    Private Shared Iterator Function cutSignal(gcxgc As D2Chromatogram(), ql As Double, qh As Double, levels As Integer) As IEnumerable(Of D2Chromatogram)
        Dim rawData As IEnumerable(Of Double) = gcxgc _
            .Select(Function(t) t.chromatogram) _
            .IteratesALL _
            .Select(Function(t) t.Intensity) _
            .ToArray
        Dim qcut As Double = If(qh >= 1, rawData.Max, TrIQ.FindThreshold(rawData, qh, N:=levels))
        Dim cutLow As Double = If(ql <= 0, rawData.Min, TrIQ.FindThreshold(rawData, ql, N:=levels))

        For Each scan As D2Chromatogram In gcxgc
            Yield New D2Chromatogram With {
                .intensity = scan.intensity,
                .scan_time = scan.scan_time,
                .chromatogram = scan _
                    .chromatogram _
                    .Select(Function(d)
                                Dim into As Double = If(d.Intensity > qcut, qcut, If(d.Intensity < cutLow, cutLow, d.Intensity))

                                Return New ChromatogramTick With {
                                    .Time = d.Time,
                                    .Intensity = into
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim xTicks As Vector = TIC2D.Select(Function(t) t.scan_time).CreateAxisTicks.AsVector
        Dim yTicks As Vector = TIC2D.Select(Function(t) t.chromatogram) _
            .IteratesALL _
            .TimeArray _
            .CreateAxisTicks _
            .AsVector
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(values:=xTicks).range(integers:={rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(values:=yTicks).range(integers:={rect.Top, rect.Bottom})
        Dim scale As New DataScaler() With {
            .AxisTicks = (xTicks, yTicks),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim colors As Color() = Designer.GetColors(theme.colorSet, mapLevels)
        Dim dw As Double = rect.Width / TIC2D.Length
        Dim dh As Double = rect.Height / TIC2D(Scan0).chromatogram.Length
        Dim allIntensity As Vector = TIC2D _
            .Select(Function(t) t.chromatogram) _
            .IteratesALL _
            .IntensityArray
        Dim intensityRange As New DoubleRange(allIntensity)

        ' allIntensity = (allIntensity * 10 ^ 40).CreateAxisTicks.AsVector / (10 ^ 40)
        allIntensity = allIntensity.Range.CreateAxisTicks.AsVector

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
            Call g.DrawImage(FillHeatMap(TIC2D, rect.Size, scale, theme.colorSet, mapLevels, dw, dh, colors.First), rect.Location)
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
            designer:=colors _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray,
            ticks:=allIntensity,
            titleFont:=CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi),
            title:="Intensity Scale",
            tickFont:=CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi),
            tickAxisStroke:=Stroke.TryParse(theme.axisTickStroke).GDIObject,
            format:="G3"
        )
    End Sub

    Public Shared Function FillHeatMap(TIC2D As IEnumerable(Of D2Chromatogram),
                                       size As Size,
                                       scale As DataScaler,
                                       Colors As String,
                                       mapLevels As Integer,
                                       dw As Double, dh As Double,
                                       background As Color) As Image

        Dim render As New PixelRender(Colors, mapLevels, defaultColor:=background)
        Dim pixels As MsImaging.PixelData() = TIC2D _
            .Select(Iterator Function(line) As IEnumerable(Of MsImaging.PixelData)
                        Dim xi As Integer = scale.TranslateX(line.scan_time)

                        For Each cel As ChromatogramTick In line.chromatogram
                            Yield New MsImaging.PixelData(xi, scale.TranslateY(cel.Time), cel.Intensity)
                        Next
                    End Function) _
            .IteratesALL _
            .ToArray

        Return render.RenderRasterImage(pixels, size, fillRect:=True, cw:=dw, ch:=dh)
    End Function

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
                ' i = index.Max - i

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
