#Region "Microsoft.VisualBasic::06274a9f454bf8df4d24c5cf575c80d1, visualize\MsImaging\Plot\DualRegionUnionPlot.vb"

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

'   Total Lines: 143
'    Code Lines: 123 (86.01%)
' Comment Lines: 1 (0.70%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 19 (13.29%)
'     File Size: 6.27 KB


' Class DualRegionUnionPlot
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: GetMzLegend, UnionScale
' 
'     Sub: PlotInternal
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Plots
Imports Microsoft.VisualBasic.Data.Plots.Canvas
Imports Microsoft.VisualBasic.Data.Plots.Legacy.Plots
Imports Microsoft.VisualBasic.Data.Plots.Plot3D
Imports Microsoft.VisualBasic.Data.Plots.Plot3D.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports HeatMapBrushes = Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.HeatMapBrushes

Public Class DualRegionUnionPlot : Inherits Plot

    ReadOnly region1 As SingleIonLayer
    ReadOnly region2 As SingleIonLayer
    ReadOnly pixelScale As Size
    ReadOnly pixelDrawer As Boolean
    ReadOnly colorSet1 As SolidBrush()
    ReadOnly colorSet2 As SolidBrush()
    ReadOnly intensityTicks As Double()

    Public Sub New(region1 As SingleIonLayer, region2 As SingleIonLayer,
                   pixelScale As Size,
                   pixelDrawer As Boolean,
                   colorSet1 As Color(),
                   colorSet2 As Color(),
                   theme As Theme)

        MyBase.New(theme)

        Me.region1 = region1
        Me.region2 = region2
        Me.pixelDrawer = pixelDrawer
        Me.pixelScale = pixelScale
        Me.colorSet1 = colorSet1.Select(Function(c) New SolidBrush(c)).ToArray
        Me.colorSet2 = colorSet2.Select(Function(c) New SolidBrush(c)).ToArray

        intensityTicks = region1 _
            .GetIntensity _
            .JoinIterates(region2.GetIntensity) _
            .Range _
            .CreateAxisTicks

        Me.region1 = UnionScale(region1, colorSet1.Length)
        Me.region2 = UnionScale(region2, colorSet2.Length)
    End Sub

    Private Function GetMzLegend(ion As SingleIonLayer, color As Color) As LegendObject
        Return New LegendObject With {
           .color = color.ToHtmlColor,
           .fontstyle = theme.legendLabelCSS,
           .style = LegendStyles.Square,
           .title = Double.Parse(ion.IonMz).ToString("F4")
        }
    End Function

    Private Function UnionScale(layer As SingleIonLayer, n As Integer) As SingleIonLayer
        Dim scaleRange As New DoubleRange(0, n)
        Dim intoRange As New DoubleRange(intensityTicks)
        Dim pixels As PixelData() = layer.MSILayer _
            .Select(Function(p)
                        Return New PixelData() With {
                            .x = p.x,
                            .y = p.y,
                            .mz = p.mz,
                            .intensity = intoRange.ScaleMapping(p.intensity, scaleRange),
                            .level = .intensity
                        }
                    End Function) _
            .ToArray

        Return New SingleIonLayer With {
            .DimensionSize = layer.DimensionSize,
            .IonMz = layer.IonMz,
            .MSILayer = pixels
        }
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim MSIsize As Size = region1.DimensionSize
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim Xtick As Double() = New DoubleRange({0, MSIsize.Width}).CreateAxisTicks()
        Dim Ytick As Double() = New DoubleRange({0, MSIsize.Height}).CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion(css)
        Dim scaleX = d3js.scale.linear.domain(values:=Xtick).range(values:=New Double() {rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(values:=Ytick).range(values:=New Double() {rect.Top, rect.Bottom})
        Dim scale As New DataScaler With {
            .AxisTicks = (Xtick.AsVector, Ytick.AsVector),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim MSI1 As Image, MSI2 As Image
        Dim engine As New PixelRender(heatmapRender:=False)
        Dim heatmap As HeatMapBrushes

        heatmap = New HeatMapBrushes(colorSet1, NameOf(Color.Transparent))
        MSI1 = engine.RenderPixels(region1.MSILayer, MSIsize, heatmap).AsGDIImage
        MSI1 = Drawer.ScaleLayer(MSI1, rect.Width, rect.Height)

        heatmap = New HeatMapBrushes(colorSet2, NameOf(Color.Transparent))
        MSI2 = engine.RenderPixels(region2.MSILayer, MSIsize, heatmap).AsGDIImage
        MSI2 = Drawer.ScaleLayer(MSI2, rect.Width, rect.Height)

        Call g.DrawAxis(canvas, scale,
                        showGrid:=True,
                        gridFill:=theme.gridFill,
                        xlabel:=xlabel,
                        ylabel:=ylabel,
                        XtickFormat:="F0",
                        YtickFormat:="F0",
                        htmlLabel:=False)

        Call g.DrawImage(MSI1, rect)
        Call g.DrawImage(MSI2, rect)

        ' draw ion m/z
        Dim labelFont As Font = css.GetFont(CSSFont.TryParse(theme.legendLabelCSS))
        Dim label As String = Double.Parse(region1.IonMz).ToString("F4")
        Dim labelSize As SizeF = g.MeasureString(label, labelFont)
        Dim pos As New Point(rect.Right + canvas.Padding.Right * 0.05, rect.Top)
        Dim mz1 = GetMzLegend(region1, colorSet1.Last.Color)
        Dim mz2 = GetMzLegend(region2, colorSet2.Last.Color)

        Call Legend.DrawLegends(g, pos, {mz1, mz2}, $"{labelSize.Height},{labelSize.Height}")

        Dim layout As New Rectangle(
            x:=canvas.PlotRegion(css).Right + 10,
            y:=pos.Y + labelSize.Height * 4,
            width:=canvas.Padding.Right * 0.5,
            height:=rect.Height * 0.5
        )
        Dim tickFont As Font = css.GetFont(CSSFont.TryParse(theme.legendTickCSS))
        Dim tickPen As Pen = css.GetPen(Stroke.TryParse(theme.legendTickAxisStroke))
        Dim axisPen As Pen = css.GetPen(Stroke.TryParse(theme.axisStroke))

        Call DrawDualColorBar(g, colorSet1, colorSet2, layout, intensityTicks, axisPen, tickPen, "Intensity", labelFont, tickFont, "G3")
    End Sub

    ''' <summary>
    ''' Draws two vertical color bars (colorSet1 on the left, colorSet2 on the
    ''' right) inside <paramref name="layout"/>. This reimplements the former
    ''' <c>IGraphics.DrawDualColorBar</c> extension that was removed during the
    ''' DataPlot refactor.
    ''' </summary>
    Private Sub DrawDualColorBar(g As IGraphics,
                                  colorSet1 As SolidBrush(),
                                  colorSet2 As SolidBrush(),
                                  layout As Rectangle,
                                  intensityTicks As Double(),
                                  axisPen As Pen,
                                  tickPen As Pen,
                                  title As String,
                                  labelFont As Font,
                                  tickFont As Font,
                                  format As String)
        If colorSet1 Is Nothing OrElse colorSet1.Length = 0 Then Return

        Dim halfWidth = layout.Width \ 2
        Dim barW = halfWidth - 4
        Dim leftRect As New Rectangle(layout.X, layout.Y, barW, layout.Height)
        Dim rightRect As New Rectangle(layout.X + halfWidth + 4, layout.Y, barW, layout.Height)

        Call DrawColorBar(g, colorSet1, leftRect, intensityTicks, tickPen, tickFont, format, True)
        Call DrawColorBar(g, colorSet2, rightRect, intensityTicks, tickPen, tickFont, format, False)

        If Not String.IsNullOrEmpty(title) Then
            Dim titleSize = g.MeasureString(title, labelFont)
            Dim titlePt As New PointF(layout.X + (layout.Width - titleSize.Width) / 2.0F, layout.Y - titleSize.Height - 2.0F)
            g.DrawString(title, labelFont, New SolidBrush(Color.Black), titlePt)
        End If

        g.DrawRectangle(axisPen, layout)
    End Sub

    Private Sub DrawColorBar(g As IGraphics,
                              colors As SolidBrush(),
                              rect As Rectangle,
                              intensityTicks As Double(),
                              tickPen As Pen,
                              tickFont As Font,
                              format As String,
                              ticksOnLeft As Boolean)
        Dim n = colors.Length
        Dim segH = rect.Height / n

        For i As Integer = 0 To n - 1
            g.FillRectangle(colors(i), New Rectangle(rect.X, rect.Y + CInt(i * segH), rect.Width, CInt(segH) + 1))
        Next

        If intensityTicks IsNot Nothing AndAlso intensityTicks.Length > 0 Then
            Dim tmin = intensityTicks.Min()
            Dim tmax = intensityTicks.Max()
            Dim span = tmax - tmin

            For Each t In intensityTicks
                Dim frac = If(span <= 0, 0.5, (t - tmin) / span)
                Dim y = rect.Y + rect.Height * (1.0F - CSng(frac))
                Dim label = t.ToString(format)
                Dim sz = g.MeasureString(label, tickFont)
                Dim x = If(ticksOnLeft, rect.X - sz.Width - 2.0F, rect.Right + 2.0F)
                Dim pt As New PointF(x, y - sz.Height / 2.0F)
                g.DrawString(label, tickFont, New SolidBrush(Color.Black), pt)

                Dim tickFrom = New PointF(If(ticksOnLeft, rect.X - 4.0F, rect.Right), y)
                Dim tickTo = New PointF(If(ticksOnLeft, rect.X, rect.Right + 4.0F), y)
                g.DrawLine(tickPen, tickFrom, tickTo)
            Next
        End If
    End Sub
End Class
