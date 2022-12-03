Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class NMRSpectrum : Inherits Plot

    ''' <summary>
    ''' the x axis
    ''' </summary>
    ReadOnly ppm As Double()
    ''' <summary>
    ''' the y axis
    ''' </summary>
    ReadOnly intensity As Double()
    ''' <summary>
    ''' the title of the spectrum line
    ''' </summary>
    ReadOnly title As String

    Public Sub New(nmr As LibraryMatrix, theme As Theme)
        MyBase.New(theme)

        ppm = nmr.Select(Function(m) m.mz).ToArray
        intensity = nmr.Select(Function(m) m.intensity).ToArray
        title = nmr.name
        theme.yAxisLayout = YAxisLayoutStyles.None
        theme.xAxisReverse = True
        theme.drawLegend = False
        xlabel = "ppm"
        ylabel = "absorb"
        main = "NMR spectrum"
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim points As PointData() = ppm _
            .Select(Function(ppmi, i) New PointData(ppmi, intensity(i))) _
            .ToArray
        Dim lineStyle As Pen = Stroke.TryParse(theme.lineStroke).GDIObject
        Dim line As New SerialData With {
            .color = lineStyle.Color,
            .lineType = DashStyle.Solid,
            .pointSize = theme.pointSize,
            .pts = points,
            .shape = LegendStyles.SolidLine,
            .title = title,
            .width = lineStyle.Width
        }
        Dim app As New LinePlot2D(
            data:={line},
            theme:=theme,
            fill:=False,
            fillPie:=False,
            interplot:=Splines.None
        ) With {
            .legendTitle = legendTitle,
            .main = main,
            .xlabel = xlabel,
            .ylabel = ylabel,
            .zlabel = zlabel
        }

        Call app.Plot(g, canvas)
    End Sub
End Class
