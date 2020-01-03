#Region "Microsoft.VisualBasic::5dacf526ebb19a7da3620db7ed7a1776, src\visualize\plot\ChromatogramPlot.vb"

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

    ' Module ChromatogramPlot
    ' 
    '     Function: MRMChromatogramPlot, (+2 Overloads) TICplot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM

Public Module ChromatogramPlot

    ''' <summary>
    ''' 将所有离子对所捕获的色谱曲线数据都绘制在同一张图上面
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="mzML$"></param>
    ''' <param name="size$"></param>
    ''' <param name="margin$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="colorsSchema$"></param>
    ''' <param name="penStyle$"></param>
    ''' <param name="labelFontStyle$"></param>
    ''' <param name="labelConnectorStroke$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MRMChromatogramPlot(ions As IonPair(),
                                        mzML$,
                                        Optional size$ = "1600,1000",
                                        Optional margin$ = g.DefaultLargerPadding,
                                        Optional bg$ = "white",
                                        Optional colorsSchema$ = "scibasic.category31()",
                                        Optional penStyle$ = Stroke.ScatterLineStroke,
                                        Optional labelFontStyle$ = CSSFont.Win7Normal,
                                        Optional labelConnectorStroke$ = Stroke.StrongHighlightStroke) As GraphicsData
        Return ions.ExtractIonData(mzML, Function(ion) ion.name) _
                   .TICplot(
            size:=size,
            bg:=bg,
            colorsSchema:=colorsSchema,
            labelConnectorStroke:=labelConnectorStroke,
            labelFontStyle:=labelFontStyle,
            margin:=margin,
            penStyle:=penStyle,
            fillCurve:=False
        )
    End Function

    ''' <summary>
    ''' 从mzML文件之中解析出色谱数据之后，将所有的色谱峰都绘制在一张图之中进行可视化
    ''' </summary>
    ''' <param name="size$"></param>
    ''' <param name="margin$"></param>
    ''' <param name="bg$"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function TICplot(ionData As NamedCollection(Of ChromatogramTick),
                            Optional size$ = "1600,1000",
                            Optional margin$ = g.DefaultLargerPadding,
                            Optional bg$ = "white",
                            Optional colorsSchema$ = "scibasic.category31()",
                            Optional penStyle$ = Stroke.ScatterLineStroke,
                            Optional labelFontStyle$ = CSSFont.Win7Normal,
                            Optional labelConnectorStroke$ = Stroke.StrongHighlightStroke,
                            Optional labelLayoutTicks% = 500,
                            Optional showLabels As Boolean = True,
                            Optional fillCurve As Boolean = True,
                            Optional axisLabelFont$ = CSSFont.Win7Large,
                            Optional axisTickFont$ = CSSFont.Win10NormalLarger,
                            Optional showLegends As Boolean = True,
                            Optional legendFontCSS$ = CSSFont.Win10Normal) As GraphicsData

        Return {ionData}.TICplot(
            size:=size,
            axisLabelFont:=axisLabelFont,
            axisTickFont:=axisTickFont,
            bg:=bg,
            penStyle:=penStyle,
            labelFontStyle:=labelFontStyle,
            labelConnectorStroke:=labelConnectorStroke,
            colorsSchema:=colorsSchema,
            margin:=margin,
            labelLayoutTicks:=labelLayoutTicks,
            showLabels:=showLabels,
            showLegends:=showLegends,
            fillCurve:=fillCurve,
            legendFontCSS:=legendFontCSS
        )
    End Function

    ''' <summary>
    ''' 从mzML文件之中解析出色谱数据之后，将所有的色谱峰都绘制在一张图之中进行可视化
    ''' </summary>
    ''' <param name="size"></param>
    ''' <param name="margin"></param>
    ''' <param name="bg"></param>
    ''' <param name="deln">legend每一列有多少个进行显示</param>
    ''' <param name="labelColor"></param>
    <Extension>
    Public Function TICplot(ionData As NamedCollection(Of ChromatogramTick)(),
                            Optional size$ = "1600,1000",
                            Optional margin$ = g.DefaultLargerPadding,
                            Optional bg$ = "white",
                            Optional colorsSchema$ = "scibasic.category31()",
                            Optional penStyle$ = Stroke.ScatterLineStroke,
                            Optional labelFontStyle$ = CSSFont.Win7Small,
                            Optional labelConnectorStroke$ = Stroke.StrongHighlightStroke,
                            Optional labelLayoutTicks% = 1000,
                            Optional labelColor$ = "black",
                            Optional showLabels As Boolean = True,
                            Optional fillCurve As Boolean = True,
                            Optional axisLabelFont$ = CSSFont.Win7Large,
                            Optional axisTickFont$ = CSSFont.Win10NormalLarger,
                            Optional showLegends As Boolean = True,
                            Optional legendFontCSS$ = CSSFont.Win10Normal,
                            Optional deln% = 10,
                            Optional isXIC As Boolean = False) As GraphicsData

        Dim labelFont As Font = CSSFont.TryParse(labelFontStyle)
        Dim labelConnector As Pen = Stroke.TryParse(labelConnectorStroke)

        For Each ion As NamedCollection(Of ChromatogramTick) In ionData
            Dim base = ion.value.Baseline(quantile:=0.65)
            Dim max# = ion.value.Shadows!Intensity.Max

            Call $"{ion.name}: {base}/{max} = {(100 * base / max).ToString("F2")}%".__DEBUG_ECHO
        Next

        Dim colors As LoopArray(Of Pen)
        Dim newPen As Func(Of Color, Pen) =
            Function(c As Color) As Pen
                Dim style As Stroke = Stroke.TryParse(penStyle)
                style.fill = c.ARGBExpression
                Return style.GDIObject
            End Function

        If ionData.Length = 1 Then
            colors = {
                newPen(colorsSchema.TranslateColor(False) Or Color.DeepSkyBlue.AsDefault)
            }
        Else
            colors = Designer _
                .GetColors(colorsSchema) _
                .Select(newPen) _
                .ToArray
        End If

        Dim XTicks = ionData _
            .Select(Function(ion)
                        Return ion.value.TimeArray
                    End Function) _
            .IteratesALL _
            .AsVector _
            .Range _
            .CreateAxisTicks  ' time
        Dim YTicks = ionData _
            .Select(Function(ion)
                        Return ion.value.IntensityArray
                    End Function) _
            .IteratesALL _
            .AsVector _
            .Range _
            .CreateAxisTicks ' intensity

        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                Dim rect As Rectangle = region.PlotRegion
                Dim X = d3js.scale.linear.domain(XTicks).range(integers:={rect.Left, rect.Right})
                Dim Y = d3js.scale.linear.domain(YTicks).range(integers:={rect.Top, rect.Bottom})
                Dim scaler As New DataScaler With {
                    .AxisTicks = (XTicks, YTicks),
                    .region = rect,
                    .X = X,
                    .Y = Y
                }

                Call g.DrawAxis(
                    region, scaler, showGrid:=False,
                    xlabel:=If(isXIC, "m/z ratio", "Time (s)"),
                    ylabel:="Intensity",
                    htmlLabel:=False,
                    YtickFormat:="G2",
                    labelFont:=axisLabelFont,
                    tickFontStyle:=axisTickFont
                )

                Dim legends As New List(Of Legend)
                Dim peakTimes As New List(Of NamedValue(Of ChromatogramTick))

                For i As Integer = 0 To ionData.Length - 1
                    Dim curvePen As Pen = colors.Next
                    Dim line = ionData(i)
                    Dim chromatogram = line.value

                    legends += New Legend With {
                        .title = line.name,
                        .color = curvePen.Color.ToHtmlColor,
                        .fontstyle = legendFontCSS,
                        .style = LegendStyles.Rectangle
                    }
                    peakTimes += New NamedValue(Of ChromatogramTick) With {
                        .Name = line.name,
                        .Value = chromatogram(Which.Max(chromatogram.Shadows!Intensity))
                    }

                    Dim A, B As PointF
                    Dim polygon As New List(Of PointF)

                    For Each signal As SlideWindow(Of PointF) In chromatogram _
                        .Select(Function(c)
                                    Return New PointF(c.Time, c.Intensity)
                                End Function) _
                        .SlideWindows(winSize:=2)

                        A = scaler.Translate(signal.First)
                        B = scaler.Translate(signal.Last)

                        Call g.DrawLine(curvePen, A, B)

                        If polygon = 0 Then
                            polygon.Add(A)
                        End If

                        polygon.Add(B)
                    Next

                    Dim bottom% = region.Bottom - 6

                    polygon.Insert(0, New PointF(polygon(0).X, bottom))
                    polygon.Add(New PointF(polygon.Last.X, bottom))

                    If fillCurve Then
                        Dim color As Color = Color.FromArgb(200, curvePen.Color)

                        Call g.FillPolygon(New SolidBrush(color), polygon)
                    End If
                Next

                If showLabels Then
                    ' labeling 
                    Dim canvas = g
                    Dim labels As Label() = peakTimes _
                        .Select(Function(ion)
                                    Dim labelSize As SizeF = canvas.MeasureString(ion.Name, labelFont)
                                    Dim location As PointF = scaler.Translate(ion.Value)

                                    Return New Label With {
                                        .height = labelSize.Height,
                                        .width = labelSize.Width,
                                        .text = ion.Name,
                                        .X = location.X,
                                        .Y = location.Y
                                    }
                                End Function) _
                        .ToArray
                    Dim anchors As Anchor() = labels.GetLabelAnchors(r:=3)

                    Call d3js.labeler(maxAngle:=5, maxMove:=300, w_lab2:=100, w_lab_anc:=100) _
                        .Labels(labels) _
                        .Anchors(anchors) _
                        .Width(rect.Width) _
                        .Height(rect.Height) _
                        .Start(showProgress:=False, nsweeps:=labelLayoutTicks)

                    Dim labelBrush As Brush = labelColor.GetBrush

                    For Each i As SeqValue(Of Label) In labels.SeqIterator
                        Call g.DrawLine(labelConnector, i.value.GetTextAnchor(anchors(i)), anchors(i))
                        Call g.DrawString(i.value.text, labelFont, labelBrush, i.value)
                    Next
                End If

                If showLegends Then

                    ' 如果离子数量非常多的话,则会显示不完
                    ' 这时候每20个换一列
                    Dim cols = legends.Count / deln

                    If cols > Fix(cols) Then
                        ' 有余数,说明还有剩下的,增加一列
                        cols += 1
                    End If

                    ' 计算在右上角的位置
                    Dim maxSize = legends.MaxLegendSize(g)
                    Dim top = region.PlotRegion.Top + maxSize.Height + 5
                    Dim maxLen = maxSize.Width
                    Dim legendShapeWidth% = 70
                    Dim left = region.PlotRegion.Right - (maxLen + legendShapeWidth) * cols
                    Dim position As New Point With {
                        .X = left,
                        .Y = top
                    }

                    For Each block As Legend() In legends.Split(deln)
                        g.DrawLegends(position, block, $"{legendShapeWidth},10", d:=0)
                        position = New Point With {
                            .X = position.X + maxLen + legendShapeWidth,
                            .Y = position.Y
                        }
                    Next
                End If
            End Sub

        Return g.GraphicsPlots(
            size.SizeParser,
            margin,
            bg,
            plotInternal
        )
    End Function
End Module
