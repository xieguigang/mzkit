Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.MassSpectrum.Math

''' <summary>
''' time -> into
''' </summary>
Public Module ChromatogramPeakPlot

    Public Const DefaultPadding$ = "padding: 350px 100px 250px 200px"

    ''' <summary>
    ''' 绘制一个单独的峰
    ''' </summary>
    ''' <param name="chromatogram"></param>
    ''' <param name="size$"></param>
    ''' <param name="padding$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="title$"></param>
    ''' <param name="curveStyle$"></param>
    ''' <param name="titleFontCSS$"></param>
    ''' <param name="showMRMRegion">Debug usage</param>
    ''' <param name="ROI_styleCSS$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(chromatogram As ChromatogramTick(),
                         Optional size$ = "2100,1600",
                         Optional padding$ = DefaultPadding,
                         Optional bg$ = "white",
                         Optional title$ = "NULL",
                         Optional curveStyle$ = Stroke.ScatterLineStroke,
                         Optional titleFontCSS$ = CSSFont.Win7VeryLarge,
                         Optional showMRMRegion As Boolean = False,
                         Optional ROI_styleCSS$ = "stroke: red; stroke-width: 2px; stroke-dash: dash;",
                         Optional baseLine_styleCSS$ = "stroke: green; stroke-width: 2px; stroke-dash: dash;",
                         Optional debug As Boolean = False) As GraphicsData

        Dim timeTicks#() = chromatogram.TimeArray.CreateAxisTicks
        Dim intoTicks#() = chromatogram.IntensityArray.CreateAxisTicks
        Dim curvePen As Pen = Stroke.TryParse(curveStyle).GDIObject
        Dim titleFont As Font = CSSFont.TryParse(titleFontCSS)
        Dim ROIpen As Pen = Stroke.TryParse(ROI_styleCSS).GDIObject
        Dim baselinePen As Pen = Stroke.TryParse(baseLine_styleCSS).GDIObject
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

                Dim A, B As PointF

                For Each signal As SlideWindow(Of PointF) In chromatogram _
                    .Select(Function(c)
                                Return New PointF(c.Time, c.Intensity)
                            End Function) _
                    .SlideWindows(slideWindowSize:=2)

                    A = scaler.Translate(signal.First)
                    B = scaler.Translate(signal.Last)

                    Call g.DrawLine(curvePen, A, B)
                Next

                If showMRMRegion Then
                    Dim vector = chromatogram.Shadows
                    Dim MRM_ROI As DoubleRange = vector.MRMPeak
                    Dim maxIntensity# = vector!Intensity.Max
                    Dim canvas = g
                    Dim drawLine =
                        Sub(x1, x2)
                            x1 = scaler.Translate(x1)
                            x2 = scaler.Translate(x2)

                            Call canvas.DrawLine(ROIpen, x1, x2)
                        End Sub

                    A = New PointF(MRM_ROI.Min, 0)
                    B = New PointF(MRM_ROI.Min, maxIntensity)
                    drawLine(A, B)

                    A = New PointF(MRM_ROI.Max, 0)
                    B = New PointF(MRM_ROI.Max, maxIntensity)
                    drawLine(A, B)

                    Dim base = chromatogram.Base

                    A = New PointF(timeTicks.Min, base)
                    B = New PointF(timeTicks.Max, base)
                    ROIpen = baselinePen

                    drawLine(A, B)
                End If

                Dim left = rect.Left + (rect.Width - g.MeasureString(title, titleFont).Width) / 2
                Dim top = rect.Top + 10

                Call g.DrawString(title, titleFont, Brushes.Black, left, top)
            End Sub

        Return g.GraphicsPlots(
            size.SizeParser,
            padding,
            bg,
            plotInternal)
    End Function
End Module
