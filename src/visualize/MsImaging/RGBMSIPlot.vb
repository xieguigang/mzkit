Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class RGBMSIPlot : Inherits Plot

    ReadOnly R, G, B As SingleIonLayer
    ReadOnly dimensionSize As Size

    Public Sub New(R As SingleIonLayer, G As SingleIonLayer, B As SingleIonLayer, theme As Theme)
        MyBase.New(theme)

        Me.R = R
        Me.G = G
        Me.B = B
        Me.dimensionSize = R.DimensionSize
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim Xtick As Double() = New DoubleRange({0, dimensionSize.Width}).CreateAxisTicks()
        Dim Ytick As Double() = New DoubleRange({0, dimensionSize.Height}).CreateAxisTicks
        Dim rect As Rectangle = canvas.PlotRegion
        Dim scaleX = d3js.scale.linear.domain(Xtick).range(New Double() {rect.Left, rect.Right})
        Dim scaleY = d3js.scale.linear.domain(Ytick).range(New Double() {rect.Top, rect.Bottom})
        Dim scale As New DataScaler With {
            .AxisTicks = (Xtick.AsVector, Ytick.AsVector),
            .region = rect,
            .X = scaleX,
            .Y = scaleY
        }

        Dim MSI As Image

        MSI = Drawer.ChannelCompositions(Me.R.MSILayer, Me.G?.MSILayer, Me.B?.MSILayer, dimensionSize, Nothing)
        MSI = Drawer.ScaleLayer(MSI, rect.Width, rect.Height, InterpolationMode.Bilinear)

        Call g.DrawAxis(canvas, scale, showGrid:=False, xlabel:=xlabel, ylabel:=ylabel, XtickFormat:="F0", YtickFormat:="F0", htmlLabel:=False)
        Call g.DrawImageUnscaled(MSI, rect)

        ' draw ion m/z
        Dim labelFont As Font = CSSFont.TryParse(theme.legendLabelCSS).GDIObject(g.Dpi)
        Dim labelSize As SizeF = g.MeasureString(Me.R.IonMz.ToString("F4"), labelFont)
        Dim pos As New Point(rect.Right + canvas.Padding.Right * 0.05, rect.Top + labelSize.Height)
        Dim mzR As New LegendObject With {
            .color = "red",
            .fontstyle = theme.legendLabelCSS,
            .style = LegendStyles.Square,
            .title = Me.R.IonMz.ToString("F4")
        }
        Dim mzG As LegendObject = Nothing
        Dim mzB As LegendObject = Nothing

        If Not Me.G Is Nothing Then
            mzG = New LegendObject With {
                .color = "green",
                .fontstyle = theme.legendLabelCSS,
                .style = LegendStyles.Square,
                .title = Me.G.IonMz.ToString("F4")
            }
        End If
        If Not Me.B Is Nothing Then
            mzB = New LegendObject With {
                .color = "blue",
                .fontstyle = theme.legendLabelCSS,
                .style = LegendStyles.Square,
                .title = Me.B.IonMz.ToString("F4")
            }
        End If

        Dim legends As LegendObject() = {mzR, mzG, mzB}.Where(Function(a) Not a Is Nothing).ToArray

        Call Legend.DrawLegends(g, pos, legends, $"{labelSize.Height},{labelSize.Height}")
    End Sub
End Class
