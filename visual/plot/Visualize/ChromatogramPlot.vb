Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime

''' <summary>
''' time -> into
''' </summary>
Public Module ChromatogramPlot

    <Extension>
    Public Function Plot(chromatogram As PointF(),
                         Optional size$ = "2000,1800",
                         Optional padding$ = g.DefaultPadding,
                         Optional bg$ = "white",
                         Optional title$ = "NULL",
                         Optional curveStyle$ = Stroke.AxisStroke) As GraphicsData

        Dim timeTicks#() = chromatogram.X.CreateAxisTicks
        Dim intoTicks#() = chromatogram.Y.CreateAxisTicks
        Dim curvePen As Pen = Stroke.TryParse(curveStyle).GDIObject
        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                Dim rect As Rectangle = region.PlotRegion
                Dim X = d3js.scale.linear.domain(timeTicks).range(integers:={rect.Left, rect.Right})
                Dim Y = d3js.scale.linear.domain(intoTicks).range(integers:={rect.Top, rect.Bottom})
                Dim scaler As New DataScaler With {
                    .X = X,
                    .Y = Y,
                    .Region = rect,
                    .AxisTicks = (timeTicks, intoTicks)
                }
                Dim ZERO = scaler.TranslateY(0)

                Call g.DrawAxis(
                    region, scaler, showGrid:=False,
                    xlabel:="Time (s)",
                    ylabel:="Intensity",
                    htmlLabel:=False
                )

                For Each signal In chromatogram.SlideWindows(slideWindowSize:=2)
                    Dim A = scaler.Translate(signal.First)
                    Dim B = scaler.Translate(signal.Last)

                    Call g.DrawLine(curvePen, A, B)
                Next
            End Sub

        Return g.GraphicsPlots(
            size.SizeParser,
            padding,
            bg,
            plotInternal)
    End Function
End Module
