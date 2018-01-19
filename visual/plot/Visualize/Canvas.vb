Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.Visualization.DATA.SpectrumJSON
Imports SMRUCC.proteomics.PNL.OMICS.MwtWinDll
Imports SMRUCC.proteomics.PNL.OMICS.MwtWinDll.Extensions

''' <summary>
''' m/z -> into
''' </summary>
Public Module Canvas

    Public Const Padding$ = "padding: 250px 100px 200px 200px"

    <Extension> Private Function __possibleFormula(mz#, Optional chargeRange$ = "-1,1") As String
        Dim candidates As FormulaFinderResult() = IFormulaFinder.CommonAtoms.SearchByMZAndLimitCharges(chargeRange, mz, 20)
        candidates = candidates.OrderBy(Function(m) Abs(m.DeltaMass)).ToArray
        Return candidates.FirstOrDefault?.EmpiricalFormula
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="MS_spectrum"></param>
    ''' <param name="size"></param>
    ''' <param name="padding$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="mzAxis$">Y轴的数据永远是0到100</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Plot(MS_spectrum As SpectrumData,
                         Optional size$ = "2000,1600",
                         Optional padding$ = Canvas.Padding,
                         Optional bg$ = "white",
                         Optional title$ = "<span style=""color:blue"">MS/MS spectra</span>",
                         Optional mzAxis$ = "(0,800),tick=100",
                         Optional signalStroke$ = "stroke: green; stroke-width: 10px; stroke-dash: solid;",
                         Optional titleFontCSS$ = CSSFont.Win7Large,
                         Optional axisTickFont$ = CSSFont.Win10NormalLarger,
                         Optional axisLabelFont$ = CSSFont.Win7Large,
                         Optional legendFontCSS$ = CSSFont.PlotSmallTitle,
                         Optional showPossibleFormula As Boolean = False,
                         Optional labelCSS$ = CSSFont.Win7LargerNormal,
                         Optional showLegend As Boolean = True) As GraphicsData

        Dim maxInto# = MS_spectrum _
            .data _
            .Select(Function(l) l.y) _
            .Max
        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                Dim plotRect As Rectangle = region.PlotRegion
                Dim ticks = (
                    x:=MS_spectrum.data.Select(Function(mz) mz.x).Range.CreateAxisTicks,
                    y:={0R, 100.0R}.Range.CreateAxisTicks)
                Dim y As d3js.scale.LinearScale = d3js.scale.linear().domain(ticks.y).range(New Integer() {plotRect.Top, plotRect.Bottom})
                Dim x As d3js.scale.LinearScale = d3js.scale.linear().domain(ticks.x).range(New Integer() {plotRect.Left, plotRect.Right})
                Dim mapper As New DataScaler With {
                    .AxisTicks = ticks,
                    .Region = plotRect,
                    .X = x,
                    .Y = y
                }

                Call mapper.DrawYGrid(g, region, New Pen(Color.Black, 2), "Intensity (%)",)
                Call g.DrawX(New Pen(Color.Black, 2), "Mass (m/z)",
                             mapper,
                             XAxisLayoutStyles.Bottom,
                             Nothing,
                             axisLabelFont,
                             CSSFont.TryParse(axisTickFont),
                             overridesTickLine:=20,
                             noTicks:=True)

                Dim signalPen As Pen = Stroke.TryParse(signalStroke)
                Dim bottom! = region.Bottom
                Dim labelFont As Font = CSSFont.TryParse(labelCSS).GDIObject
                Dim linkPen As New Pen(Color.Black) With {.Width = 2, .DashStyle = DashStyle.Dot}

                For Each hit As IntensityCoordinate In MS_spectrum.data
                    Dim point As PointF = mapper.Translate(x:=hit.x, y:=hit.y)
                    Dim low As New PointF(point.X, bottom)
                    Dim percentage# = hit.y / maxInto
                    Dim label$ = Nothing

                    Call g.DrawLine(signalPen, point, low)  ' 二级碎片

                    ' 绘制X轴的mz标签
                    '
                    ' mz(into%)
                    ' formula

                    If percentage >= 0.4 Then

                        label = hit.x.ToString("F2") & $"({(percentage * 100).ToString("F2")}%)"

                        ' Call g.DrawString(hit.x, labelFont, Brushes.Black, point)

                        If showPossibleFormula AndAlso percentage >= 0.7 Then
                            Dim formula$ = hit.x.__possibleFormula
#If DEBUG Then
                            Call formula.__DEBUG_ECHO
#End If
                            If Not formula.StringEmpty Then
                                'Dim formulaLabel As Image = TextRender _
                                '    .DrawHtmlText(formula, CSSFont.Win10Normal,) _
                                '    .RotateImage(-90.0!)

                                'point = New PointF(point.X - formulaLabel.Width / 2,
                                '                   point.Y - formulaLabel.Height)
                                'g.DrawImageUnscaled(formulaLabel, point.ToPoint)
                                label = label & ASCII.LF & formula
                            End If
                        End If
                    End If

                    If Not label.StringEmpty Then
                        Dim position As Point
                        Dim s = label.lTokens
                        Dim maxlen = g.MeasureString(s.MaxLengthString, labelFont)

                        If percentage >= 0.95 Then
                            ' 绘制在旁边
                            position = New Point(point.X + 20, point.Y + 50)
                            Call g.DrawLine(linkPen, New Point(position.X + maxlen.Width / 2, position.Y), point)
                        Else
                            ' 直接绘制在顶部
                        End If

                        For Each label In s
                            Dim ss = g.MeasureString(label, labelFont).Width
                            Dim offsets = (maxlen.Width - ss) / 2
                            point = New PointF(position.X + offsets, position.Y)
                            Call g.DrawString(label, labelFont, Brushes.Black, point)
                            position = New Point(position.X, position.Y + labelFont.Height + 5)
                        Next
                    End If
                Next

                If showLegend Then
                    Dim legendFont As Font = CSSFont.TryParse(legendFontCSS)
                    Dim name$ = MS_spectrum.name.Trim
                    Dim legendShapeSize As New Size(60, 45)
                    Dim legendPoint As New Point(
                        region.PlotRegion.Right - legendShapeSize.Width * 2 - g.MeasureString(name, legendFont).Width,
                        region.Padding.Top * 1.5)
                    Dim legend As New Legend With {
                        .color = signalPen.Color.RGBExpression,
                        .fontstyle = legendFontCSS,
                        .style = LegendStyles.RoundRectangle,
                        .title = name
                    }

                    Call g.DrawLegends(
                        legendPoint, {legend}, legendShapeSize.ScriptValue,
                        regionBorder:=New Stroke With {
                            .dash = DashStyle.Solid,
                            .fill = "black",
                            .width = 2
                        })
                End If

                Dim titleImage As Image = TextRender.DrawHtmlText(title, titleFontCSS)
                Call g.DrawImageUnscaled(titleImage, region.TopCentra(titleImage.Size))
            End Sub

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            plotInternal)
    End Function
End Module
