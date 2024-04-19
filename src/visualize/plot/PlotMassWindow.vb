Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports std = System.Math

''' <summary>
''' show the mass windows histogram in scatter plot view
''' </summary>
Public Class PlotMassWindow : Inherits Plot

    ReadOnly mz As Double()
    ReadOnly hist As Double()
    ReadOnly bins As MassWindow()

    Sub New(mz As IEnumerable(Of Double), theme As Theme, Optional mzdiff As Double = 0.001)
        Call MyBase.New(theme)

        With MzBins.GetScatter(mz, mzdiff)
            Me.mz = .x
            Me.hist = .y
        End With

        xlabel = "m/z"
        ylabel = "histogram"

        bins = MzBins.GetMzBins(Me.mz, Me.hist).ToArray
    End Sub

    Sub New(mz As Double(), hist As Double(), theme As Theme)
        Call Me.New(mz, hist, MzBins.GetMzBins(mz, hist).ToArray, theme)
    End Sub

    Sub New(mz As Double(), hist As Double(), windows As MassWindow(), theme As Theme)
        Call MyBase.New(theme)

        xlabel = "m/z"
        ylabel = "histogram"

        Me.bins = windows.ToArray
        Me.mz = mz
        Me.hist = hist
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        ' colors for the annotation points
        Dim colors As LoopArray(Of Color) = Designer.GetColors(theme.colorSet)
        Dim pen As Stroke = Stroke.TryParse(theme.lineStroke)
        Dim xy As PointData() = mz _
            .Select(Function(mzi, i) New PointData(mzi, hist(i))) _
            .OrderBy(Function(pti) pti.pt.X) _
            .ToArray
        Dim histogram As New SerialData With {
            .color = pen.fill.TranslateColor,
            .lineType = pen.dash,
            .pointSize = theme.pointSize,
            .shape = LegendStyles.Circle,
            .title = mz.Average.ToString("F4"),
            .width = pen.width,
            .pts = xy,
            .DataAnnotations = bins _
                .Select(Function(win, i)
                            Dim nearest As PointData = xy _
                                .OrderBy(Function(pi) std.Abs(pi.pt.X - win.mass)) _
                                .First

                            Return New Annotation With {
                                .X = win.mass,
                                .Y = nearest.pt.Y,
                                .color = (++colors).ToHtmlColor,
                                .Font = theme.tagCSS,
                                .Legend = LegendStyles.Triangle,
                                .size = New SizeF(20, 20),
                                .Text = win.ToString
                            }
                        End Function) _
                .ToArray
        }
        Dim plot As New Plots.LinePlot2D(data:={histogram},
                                         theme:=theme,
                                         fill:=False,
                                         interplot:=Splines.B_Spline) With {
            .xlabel = xlabel,
            .ylabel = ylabel,
            .main = main
        }

        Call plot.Plot(g, canvas)
    End Sub
End Class
