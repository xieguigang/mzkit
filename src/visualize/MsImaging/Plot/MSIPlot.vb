#Region "Microsoft.VisualBasic::8836c35f899c4ddd5de27b578b48a54e, visualize\MsImaging\Plot\MSIPlot.vb"

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

    '   Total Lines: 112
    '    Code Lines: 95 (84.82%)
    ' Comment Lines: 1 (0.89%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 16 (14.29%)
    '     File Size: 4.98 KB


    ' Class MSIPlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: (+2 Overloads) MeasureSize
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

''' <summary>
''' do ms-imaging plot with axis [x,y]
''' </summary>
Public Class MSIPlot : Inherits Plot

    ReadOnly ion As SingleIonLayer
    ReadOnly pixelScale As Size
    ReadOnly pixelDrawer As Boolean
    ReadOnly driver As Drivers = Drivers.Default

    Public Sub New(ion As SingleIonLayer,
                   pixelScale As Size,
                   pixelDrawer As Boolean,
                   theme As Theme,
                   driver As Drivers)

        Call MyBase.New(theme)

        Me.pixelDrawer = pixelDrawer
        Me.ion = ion
        Me.pixelScale = pixelScale
        Me.driver = driver
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function MeasureSize() As Size
        Return MeasureSize(ion, pixelScale, theme)
    End Function

    Public Shared Function MeasureSize(ion As SingleIonLayer, pixelScale As Size, theme As Theme) As Size
        Dim padding As Padding = Padding.TryParse(theme.padding)
        Dim size As Size = ion.DimensionSize

        size = New Size(size.Width * pixelScale.Width, size.Height * pixelScale.Height)
        size = New Size(size.Width + padding.Left + padding.Right, size.Height + padding.Top + padding.Bottom)

        Return size
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim Xtick As Double() = New DoubleRange({0, ion.DimensionSize.Width}).CreateAxisTicks()
        Dim Ytick As Double() = New DoubleRange({0, ion.DimensionSize.Height}).CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(values:=Xtick).range(values:=New Double() {rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(values:=Ytick).range(values:=New Double() {rect.Top, rect.Bottom})
        Dim scale As New DataScaler With {
            .AxisTicks = (Xtick.AsVector, Ytick.AsVector),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim engine As New RectangleRender(Drivers.Default, heatmapRender:=False)
        Dim colorScale = Designer.GetColors(theme.colorSet, 25).Select(Function(c) New SolidBrush(c)).ToArray
        Dim scaleSize As New Size With {
            .Width = rect.Width / ion.DimensionSize.Width,
            .Height = rect.Height / ion.DimensionSize.Height
        }

        Call g.DrawAxis(canvas, scale,
                        showGrid:=True,
                        gridFill:=theme.gridFill,
                        xlabel:=xlabel,
                        ylabel:=ylabel,
                        XtickFormat:="F0",
                        YtickFormat:="F0",
                        htmlLabel:=False,
                        driver:=driver)

        Call engine.RenderPixels(g, rect.Location, ion.MSILayer, colorScale)

        ' draw ion m/z
        Dim labelFont As Font = css.GetFont(CSSFont.TryParse(theme.legendLabelCSS))
        Dim label As String = ion.IonMz
        Dim labelSize As SizeF = g.MeasureString(label, labelFont)
        Dim pos As New Point(rect.Right + canvas.Padding.Right * 0.05, rect.Top + labelSize.Height)
        Dim mzLegend As New LegendObject With {
            .color = "black",
            .fontstyle = theme.legendLabelCSS,
            .style = LegendStyles.Square,
            .title = label
        }

        Call Legend.DrawLegends(g, pos, {mzLegend}, $"{labelSize.Height},{labelSize.Height}")

        Dim colors = Designer.GetColors(theme.colorSet, 120).Select(Function(c) New SolidBrush(c)).ToArray
        Dim intensityTicks As Double() = New DoubleRange(ion.GetIntensity).CreateAxisTicks
        Dim layout As New Rectangle(
            x:=pos.X - canvas.Padding.Right / 5,
            y:=pos.Y + labelSize.Height * 2,
            width:=canvas.Padding.Right * 0.8,
            height:=rect.Height * 0.5
        )
        Dim tickFont As Font = css.GetFont(CSSFont.TryParse(theme.legendTickCSS))
        Dim tickPen As Pen = css.GetPen(Stroke.TryParse(theme.legendTickAxisStroke))

        Call g.ColorMapLegend(layout, colors, intensityTicks, labelFont, "Intensity", tickFont, tickPen, format:="G3")
    End Sub
End Class
