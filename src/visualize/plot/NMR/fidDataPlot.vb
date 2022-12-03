Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.NMR
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class fidDataPlot : Inherits Plot

    ReadOnly fidData As fidData

    Public Sub New(fidData As fidData, theme As Theme)
        MyBase.New(theme)

        Me.fidData = fidData
        Me.xlabel = "Time"
        Me.ylabel = "Amplitude"
        Me.main = "NMR fidData Plot"
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim points As PointData() = fidData.time _
            .Select(Function(t, i) New PointData(t, fidData.amplitude(i))) _
            .ToArray
        Dim lineStyle As Pen = Stroke.TryParse(theme.lineStroke).GDIObject
        Dim line As New SerialData With {
            .color = lineStyle.Color,
            .lineType = DashStyle.Solid,
            .pointSize = theme.pointSize,
            .pts = points,
            .shape = LegendStyles.SolidLine,
            .title = "NMR fidData",
            .width = lineStyle.Width
        }
        Dim app As New LinePlot2D({line}, theme, fill:=False, fillPie:=False, interplot:=Splines.None) With {
            .main = main,
            .ylabel = ylabel,
            .xlabel = xlabel,
            .legendTitle = legendTitle,
            .zlabel = zlabel
        }

        Call app.Plot(g, canvas)
    End Sub
End Class
