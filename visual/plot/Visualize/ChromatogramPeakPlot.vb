Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.MassSpectrum.Math.Chromatogram

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
    ''' <param name="baselineQuantile">
    ''' 如果基线过高的话，显示累加线会在基线处出现斜边的情况，会需要这个参数来计算基线然后减去基线值来修正累加曲线
    ''' </param>
    ''' <param name="showAccumulateLine">
    ''' 这个类似于对峰面积积分的结果
    ''' </param>
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
                         Optional accumulateLineStyleCss$ = "stroke: blue; stroke-width: 2px; stroke-dash: dash;",
                         Optional showAccumulateLine As Boolean = False,
                         Optional baselineQuantile# = 0.65) As GraphicsData

        Dim timeTicks#() = chromatogram.TimeArray.CreateAxisTicks
        Dim intoTicks#() = chromatogram.IntensityArray.CreateAxisTicks
        Dim accumulate# = 0
        Dim base = chromatogram.Baseline(baselineQuantile)
        Dim sumAll = (chromatogram.IntensityArray.AsVector - base).Sum()
        Dim maxInto = intoTicks.Max - base
        Dim ay = Function(into As Double) As Double
                     into = into - base
                     accumulate += If(into < 0, 0, into)
                     Return (accumulate / sumAll) * maxInto
                 End Function
        Dim curvePen As Pen = Stroke.TryParse(curveStyle).GDIObject
        Dim titleFont As Font = CSSFont.TryParse(titleFontCSS)
        Dim ROIpen As Pen = Stroke.TryParse(ROI_styleCSS).GDIObject
        Dim baselinePen As Pen = Stroke.TryParse(baseLine_styleCSS).GDIObject
        Dim accumulateLine As Pen = Stroke.TryParse(accumulateLineStyleCss).GDIObject
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
                    htmlLabel:=False,
                    XtickFormat:="G3",
                    YtickFormat:="G3"
                )

                Dim A, B As PointF
                ' 累加线
                Dim ac1 As New PointF
                Dim ac2 As PointF

                ' 在这里绘制色谱曲线
                For Each signal As SlideWindow(Of PointF) In chromatogram _
                    .Select(Function(c)
                                Return New PointF(c.Time, c.Intensity)
                            End Function) _
                    .SlideWindows(slideWindowSize:=2)

                    A = scaler.Translate(signal.First)
                    B = scaler.Translate(signal.Last)

                    Call g.DrawLine(curvePen, A, B)

                    If showAccumulateLine Then
                        ac2 = New PointF(signal.First.X, ay(signal.First.Y))
                        g.DrawLine(accumulateLine, scaler.Translate(ac1), scaler.Translate(ac2))
                        ac1 = ac2
                    End If
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

                    A = New PointF(timeTicks.Min, base)
                    B = New PointF(timeTicks.Max, base)
                    ROIpen = baselinePen

                    drawLine(A, B)
                End If

                Dim left = rect.Left + (rect.Width - g.MeasureString(title, titleFont).Width) / 2
                Dim top = (rect.Top - titleFont.Height) / 2 - 10

                Call g.DrawString(title, titleFont, Brushes.Black, left, top)
            End Sub

        Return g.GraphicsPlots(
            size.SizeParser,
            padding,
            bg,
            plotInternal)
    End Function
End Module
