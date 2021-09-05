Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class DualRegionUnionPlot : Inherits Plot

    ReadOnly region1 As SingleIonLayer
    ReadOnly region2 As SingleIonLayer
    ReadOnly pixelScale As Size
    ReadOnly cutoff As DoubleRange
    ReadOnly pixelDrawer As Boolean
    ReadOnly colorSet1 As SolidBrush()
    ReadOnly colorSet2 As SolidBrush()
    ReadOnly intensityTicks As Double()

    Public Sub New(region1 As SingleIonLayer, region2 As SingleIonLayer,
                   pixelScale As Size,
                   cutoff As DoubleRange,
                   pixelDrawer As Boolean,
                   colorSet1 As Color(),
                   colorSet2 As Color(),
                   theme As Theme)

        MyBase.New(theme)

        Me.region1 = region1
        Me.region2 = region2
        Me.pixelDrawer = pixelDrawer
        Me.cutoff = cutoff
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
           .title = ion.IonMz.ToString("F4")
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
        Dim Xtick As Double() = New DoubleRange({0, MSIsize.Width}).CreateAxisTicks()
        Dim Ytick As Double() = New DoubleRange({0, MSIsize.Height}).CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(Xtick).range(New Double() {rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(Ytick).range(New Double() {rect.Top, rect.Bottom})
        Dim scale As New DataScaler With {
            .AxisTicks = (Xtick.AsVector, Ytick.AsVector),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim MSI1 As Image, MSI2 As Image
        Dim engine As Renderer = If(pixelDrawer, New PixelRender, New RectangleRender)

        MSI1 = engine.RenderPixels(region1.MSILayer, MSIsize, Nothing, cutoff:=cutoff, colorSet:=colorSet1)
        MSI1 = Drawer.ScaleLayer(MSI1, rect.Width, rect.Height, InterpolationMode.Bilinear)

        MSI2 = engine.RenderPixels(region2.MSILayer, MSIsize, Nothing, cutoff:=cutoff, colorSet:=colorSet2)
        MSI2 = Drawer.ScaleLayer(MSI2, rect.Width, rect.Height, InterpolationMode.Bilinear)

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
        Dim labelFont As Font = CSSFont.TryParse(theme.legendLabelCSS).GDIObject(g.Dpi)
        Dim labelSize As SizeF = g.MeasureString(region1.IonMz.ToString("F4"), labelFont)
        Dim pos As New Point(rect.Right + canvas.Padding.Right * 0.05, rect.Top)
        Dim mz1 = GetMzLegend(region1, colorSet1.Last.Color)
        Dim mz2 = GetMzLegend(region2, colorSet2.Last.Color)

        Call Legend.DrawLegends(g, pos, {mz1, mz2}, $"{labelSize.Height},{labelSize.Height}")

        Dim layout As New Rectangle(
            x:=canvas.PlotRegion.Right + 10,
            y:=pos.Y + labelSize.Height * 4,
            width:=canvas.Padding.Right * 0.5,
            height:=rect.Height * 0.5
        )
        Dim tickFont As Font = CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi)
        Dim tickPen As Pen = Stroke.TryParse(theme.legendTickAxisStroke)
        Dim axisPen As Pen = Stroke.TryParse(theme.axisStroke)

        Call g.DrawDualColorBar(colorSet1, colorSet2, layout, intensityTicks, axisPen, tickPen, "Intensity", labelFont, tickFont, "G3")
    End Sub
End Class
