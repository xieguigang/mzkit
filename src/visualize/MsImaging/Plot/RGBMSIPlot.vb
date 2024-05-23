#Region "Microsoft.VisualBasic::38f8649ad9641356a2b12eaf715a1fb9, visualize\MsImaging\Plot\RGBMSIPlot.vb"

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

    '   Total Lines: 98
    '    Code Lines: 85 (86.73%)
    ' Comment Lines: 1 (1.02%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (12.24%)
    '     File Size: 4.16 KB


    ' Class RGBMSIPlot
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class RGBMSIPlot : Inherits Plot

    ReadOnly R, G, B As SingleIonLayer
    ReadOnly dimensionSize As Size
    ReadOnly pixelDrawer As Boolean
    ReadOnly maxCut As Double = 0.75
    ReadOnly threshold As IThreshold

    Public Sub New(R As SingleIonLayer, G As SingleIonLayer, B As SingleIonLayer,
                   pixelDrawer As Boolean,
                   threshold As QuantizationThreshold,
                   theme As Theme
        )

        Call MyBase.New(theme)

        Me.pixelDrawer = pixelDrawer
        Me.R = R
        Me.G = G
        Me.B = B
        Me.dimensionSize = R.DimensionSize
        Me.threshold = AddressOf threshold.ThresholdValue
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim Xtick As Double() = New DoubleRange({0, dimensionSize.Width}).CreateAxisTicks()
        Dim Ytick As Double() = New DoubleRange({0, dimensionSize.Height}).CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(values:=Xtick).range(values:=New Double() {rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(values:=Ytick).range(values:=New Double() {rect.Top, rect.Bottom})
        Dim scale As New DataScaler With {
            .AxisTicks = (Xtick.AsVector, Ytick.AsVector),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }
        Dim MSI As Image
        Dim engine As New PixelRender(heatmapRender:=False)
        Dim iR = Me.R.MSILayer
        Dim iG = Me.G?.MSILayer
        Dim iB = Me.B?.MSILayer

        MSI = engine.ChannelCompositions(Me.R.MSILayer, Me.G?.MSILayer, Me.B?.MSILayer, dimensionSize, background:=theme.background).AsGDIImage
        MSI = Drawer.ScaleLayer(MSI, rect.Width, rect.Height, InterpolationMode.Bilinear)

        Call g.DrawAxis(canvas, scale, showGrid:=False, xlabel:=xlabel, ylabel:=ylabel, XtickFormat:="F0", YtickFormat:="F0", htmlLabel:=False)
        Call g.DrawImageUnscaled(MSI, rect)

        ' draw ion m/z
        Dim labelFont As Font = CSSFont.TryParse(theme.legendLabelCSS).GDIObject(g.Dpi)
        Dim label As String = SingleIonLayer.ToString(Me.R)
        Dim labelSize As SizeF = g.MeasureString(label, labelFont)
        Dim pos As New Point(rect.Right + canvas.Padding.Right * 0.05, rect.Top + labelSize.Height)
        Dim mzR As New LegendObject With {
            .color = "red",
            .fontstyle = theme.legendLabelCSS,
            .style = LegendStyles.Square,
            .title = label
        }
        Dim mzG As LegendObject = Nothing
        Dim mzB As LegendObject = Nothing

        If Not Me.G Is Nothing Then
            mzG = New LegendObject With {
                .color = "green",
                .fontstyle = theme.legendLabelCSS,
                .style = LegendStyles.Square,
                .title = SingleIonLayer.ToString(Me.G)
            }
        End If
        If Not Me.B Is Nothing Then
            mzB = New LegendObject With {
                .color = "blue",
                .fontstyle = theme.legendLabelCSS,
                .style = LegendStyles.Square,
                .title = SingleIonLayer.ToString(Me.B)
            }
        End If

        Dim legends As LegendObject() = {mzR, mzG, mzB}.Where(Function(a) Not a Is Nothing).ToArray

        Call Legend.DrawLegends(g, pos, legends, $"{labelSize.Height},{labelSize.Height}")
    End Sub
End Class
