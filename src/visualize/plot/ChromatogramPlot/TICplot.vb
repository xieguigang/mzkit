#Region "Microsoft.VisualBasic::4828f560467967bd92d747ff4c7d3df4, visualize\plot\ChromatogramPlot\TICplot.vb"

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

    '   Total Lines: 429
    '    Code Lines: 343 (79.95%)
    ' Comment Lines: 26 (6.06%)
    '    - Xml Docs: 69.23%
    ' 
    '   Blank Lines: 60 (13.99%)
    '     File Size: 17.18 KB


    ' Class TICplot
    ' 
    '     Properties: sampleColors
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: colorProvider, GetLabels, newPen
    ' 
    '     Sub: DrawLabels, DrawTICLegends, PlotInternal, RunPlot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

''' <summary>
''' Chromatogram overlaps plot
''' </summary>
Public Class TICplot : Inherits Plot

    ReadOnly ionData As NamedCollection(Of ChromatogramTick)()
    ReadOnly timeRange As Double() = Nothing
    ReadOnly intensityMax As Double = 0
    ReadOnly isXIC As Boolean
    ReadOnly fillCurve As Boolean
    ReadOnly fillAlpha As Integer
    ReadOnly labelLayoutTicks As Integer = 100
    ReadOnly bspline As BSpline
    ''' <summary>
    ''' 当两个滑窗点的时间距离过长的时候，就不进行连接线的绘制操作了
    ''' （插入两个零值的点）
    ''' </summary>
    ReadOnly leapTimeWinSize As Double = 30

    Public Property sampleColors As Dictionary(Of String, Pen)
    Public Property ROIFill As Brush = Nothing
    Public Property ROI As PeakFeature

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tic"></param>
    ''' <param name="timeRange"></param>
    ''' <param name="intensityMax"></param>
    ''' <param name="isXIC">affects of the x axis tick format</param>
    ''' <param name="fillCurve"></param>
    ''' <param name="fillAlpha"></param>
    ''' <param name="labelLayoutTicks"></param>
    ''' <param name="bspline"></param>
    ''' <param name="theme"></param>
    Sub New(tic As NamedCollection(Of ChromatogramTick),
            timeRange As Double(),
            intensityMax As Double,
            isXIC As Boolean,
            fillCurve As Boolean,
            fillAlpha As Integer,
            labelLayoutTicks As Integer,
            bspline As BSpline,
            theme As Theme)

        Call Me.New({tic},
                    timeRange:=timeRange,
                    intensityMax:=intensityMax,
                    isXIC:=isXIC,
                    fillCurve:=fillCurve,
                    fillAlpha:=fillAlpha,
                    labelLayoutTicks:=labelLayoutTicks,
                    bspline:=bspline,
                    theme:=theme)
    End Sub

    Public Sub New(ionData As NamedCollection(Of ChromatogramTick)(),
                   timeRange As Double(),
                   intensityMax As Double,
                   isXIC As Boolean,
                   fillCurve As Boolean,
                   fillAlpha As Integer,
                   labelLayoutTicks As Integer,
                   bspline As BSpline,
                   theme As Theme)

        MyBase.New(theme)

        Me.isXIC = isXIC
        Me.intensityMax = intensityMax
        Me.ionData = ionData
        Me.fillCurve = fillCurve
        Me.fillAlpha = fillAlpha
        Me.labelLayoutTicks = labelLayoutTicks
        Me.bspline = bspline
        Me.xlabel = "Retention Time(sec)"
        Me.ylabel = "Intensity"

        If timeRange Is Nothing Then
            Me.timeRange = {}
        Else
            Me.timeRange = timeRange
        End If
    End Sub

    Private Function newPen(css As CSSEnvirnment, c As Color) As Pen
        Dim style As Stroke = Stroke.TryParse(theme.lineStroke)
        style.fill = c.ARGBExpression
        Return css.GetPen(style)
    End Function

    Private Function colorProvider(css As CSSEnvirnment) As LoopArray(Of Pen)
        If ionData.Length = 1 Then
            Return {newPen(css, theme.colorSet.TranslateColor(False) Or Color.DeepSkyBlue.AsDefault)}
        Else
            Dim palette As String = theme.colorSet

            If palette.StringEmpty(, True) Then
                palette = "paper"
                Call $"the color set for TIC overlaps plot is empty, use the default color set 'paper' for the plot rendering.".warning
            End If

            Return Designer _
                .GetColors(exp:=palette) _
                .Select(Function(c) newPen(css, c)) _
                .ToArray
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Call RunPlot(g, canvas, Nothing, Nothing)
    End Sub

    Friend Sub RunPlot(ByRef g As IGraphics, canvas As GraphicsRegion, ByRef labels As Label(), ByRef legends As LegendObject())
        Dim colors As LoopArray(Of Pen) = colorProvider(g.LoadEnvironment)
        Dim defaultPen As Pen = newPen(g.LoadEnvironment, theme.colorSet.TranslateColor(False) Or Color.DeepSkyBlue.AsDefault)
        Dim rawTime = ionData _
            .Select(Function(ion)
                        Return ion.value.TimeArray
                    End Function) _
            .IteratesALL _
            .OrderBy(Function(xi) xi) _
            .ToArray
        Dim timeRange = Me.timeRange
        timeRange = New DoubleRange(rawTime.JoinIterates(timeRange)).MinMax
        Dim XTicks As Double() = timeRange.CreateAxisTicks  ' time
        Dim YTicks = ionData _
            .Select(Function(ion)
                        Return ion.value.IntensityArray
                    End Function) _
            .IteratesALL _
            .JoinIterates({intensityMax}) _
            .AsVector _
            .Range _
            .CreateAxisTicks ' intensity

        ' make bugs fixed of all intensity value is zero
        If YTicks.Length = 0 Then
            YTicks = {0, 100}
        End If

        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim rect As Rectangle = canvas.PlotRegion(css)
        Dim X = d3js.scale.linear.domain(values:=XTicks).range(integers:={rect.Left, rect.Right})
        Dim Y = d3js.scale.linear.domain(values:=YTicks).range(integers:={rect.Top, rect.Bottom})
        Dim scaler As New DataScaler With {
            .AxisTicks = (XTicks, YTicks),
            .region = rect,
            .X = X,
            .Y = Y
        }

        If theme.drawAxis Then
            Call g.DrawAxis(
                canvas, scaler, showGrid:=theme.drawGrid,
                xlabel:=Me.xlabel,
                ylabel:=Me.ylabel,
                htmlLabel:=False,
                XtickFormat:=If(isXIC, "F2", "F0"),
                YtickFormat:="G2",
                labelFont:=theme.axisLabelCSS,
                tickFontStyle:=theme.axisTickCSS,
                gridFill:=theme.gridFill,
                xlayout:=theme.xAxisLayout,
                ylayout:=theme.yAxisLayout,
                axisStroke:=theme.axisStroke
            )
        End If

        Dim peakTimes As New List(Of NamedValue(Of ChromatogramTick))
        Dim fillColor As Brush
        Dim legendList As New List(Of LegendObject)
        Dim curvePen As Pen

        If Not ROI Is Nothing Then
            Dim left As Single = scaler.X(ROI.rtmin)
            Dim right As Single = scaler.X(ROI.rtmax)
            Dim top As Single = rect.Top
            Dim bottom As Single = scaler.TranslateY(0)
            Dim roi_region As New RectangleF(left, top, right - left, bottom - top)

            ' 20251022 fix the bug of multi-thread brush resource conflict
            ' when do drawing on windows form graphic canvas
            If ROIFill IsNot Nothing Then
                SyncLock ROIFill
                    Call g.FillRectangle(ROIFill, roi_region)
                End SyncLock
            Else
                Dim fill As Brush = New SolidBrush(Color.Blue.Alpha(150))

                SyncLock fill
                    Call g.FillRectangle(fill, roi_region)
                End SyncLock
            End If
        End If

        For i As Integer = 0 To ionData.Length - 1
            Dim line As NamedCollection(Of ChromatogramTick) = ionData(i)
            Dim chromatogram = line.value

            If sampleColors IsNot Nothing Then
                ' rendering color by sample group or manual config
                If sampleColors.ContainsKey(line.name) Then
                    curvePen = sampleColors(line.name)
                Else
                    curvePen = defaultPen
                End If
            Else
                curvePen = colors.Next
            End If

            If chromatogram.IsNullOrEmpty Then
                Call $"ion not found in raw file: '{line.name}'".Warning
                Continue For
            ElseIf bspline IsNot Nothing AndAlso bspline.degree > 1 Then
                chromatogram = ChromatogramTick.Bspline(chromatogram, bspline.degree, bspline.resolution)
            End If

            legendList += New LegendObject With {
                .title = line.name,
                .color = curvePen.Color.ToHtmlColor,
                .fontstyle = theme.legendLabelCSS,
                .style = LegendStyles.Rectangle
            }

            If theme.drawLabels Then
                Dim data As New MzGroup With {.mz = 0, .XIC = chromatogram}
                Dim peaks = data.GetPeakGroups(New Double() {2, 60}).ToArray

                peakTimes += From ROI As PeakFeature
                             In peaks
                             Select New NamedValue(Of ChromatogramTick) With {
                                 .Name = ROI.rt.ToString("F0") & $"({(ROI.rt / 60).ToString("F1")}min) {ROI.maxInto.ToString("G4")}",
                                 .Value = New ChromatogramTick With {.Intensity = ROI.maxInto, .Time = ROI.rt}
                             }
            End If

            Dim bottom% = canvas.Bottom(css) - 6
            Dim viz = g
            Dim polygon As New List(Of PointF)
            Dim draw = Sub(t1 As ChromatogramTick, t2 As ChromatogramTick)
                           Dim A = scaler.Translate(t1.Time, t1.Intensity)
                           Dim B = scaler.Translate(t2.Time, t2.Intensity)

                           Call viz.DrawLine(curvePen, A, B)

                           If polygon = 0 Then
                               polygon.Add(New PointF(A.X, bottom))
                               polygon.Add(A)
                           End If

                           polygon.Add(B)
                       End Sub

            For Each signal As SlideWindow(Of ChromatogramTick) In chromatogram.SlideWindows(winSize:=2)
                Dim dt As Double = signal.Last.Time - signal.First.Time

                If dt > leapTimeWinSize Then
                    ' add zero point
                    If dt > leapTimeWinSize Then
                        dt = leapTimeWinSize / 2
                    Else
                        dt = dt / 2
                    End If

                    Dim i1 As New ChromatogramTick With {.Time = signal.First.Time + dt, .Intensity = 0}
                    Dim i2 As New ChromatogramTick With {.Time = signal.Last.Time - dt, .Intensity = 0}

                    Call draw(signal.First, i1)

                    polygon.Add(New PointF(polygon.Last.X, bottom))

                    If fillCurve Then
                        fillColor = New SolidBrush(Color.FromArgb(fillAlpha, curvePen.Color))
                        g.FillPolygon(fillColor, polygon)
                    End If

                    polygon.Clear()

                    Call draw(i2, signal.Last)
                Else
                    Call draw(signal.First, signal.Last)
                End If
            Next

            polygon.Add(New PointF(polygon.Last.X, bottom))

            If fillCurve Then
                fillColor = New SolidBrush(Color.FromArgb(fillAlpha, curvePen.Color))
                g.FillPolygon(fillColor, polygon)
            End If
        Next

        legends = legendList
        labels = GetLabels(g, scaler, peakTimes).ToArray

        If theme.drawLabels Then Call DrawLabels(g, rect, labels, theme, labelLayoutTicks)
        If theme.drawLegend Then Call DrawTICLegends(g, canvas, legends, theme.legendSplitSize, outside:=False)

        If Not main.StringEmpty() Then
            Call DrawMainTitle(g, canvas.PlotRegion(css))
        End If
    End Sub

    Private Iterator Function GetLabels(g As IGraphics, scaler As DataScaler, peakTimes As IEnumerable(Of NamedValue(Of ChromatogramTick))) As IEnumerable(Of Label)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim labelFont As Font = css.GetFont(CSSFont.TryParse(theme.tagCSS))

        For Each ion As NamedValue(Of ChromatogramTick) In peakTimes
            Dim labelSize As SizeF = g.MeasureString(ion.Name, labelFont)
            Dim tick As ChromatogramTick = ion.Value
            Dim location As PointF = scaler.Translate(tick.Time, tick.Intensity)

            location = New PointF With {
                .X = location.X - labelSize.Width / 2,
                .Y = location.Y - labelSize.Height * 1.0125
            }

            Yield New Label With {
                .height = labelSize.Height,
                .width = labelSize.Width,
                .text = ion.Name,
                .X = location.X,
                .Y = location.Y
            }
        Next
    End Function

    Friend Shared Sub DrawLabels(g As IGraphics, rect As Rectangle, labels As Label(), theme As Theme, labelLayoutTicks As Integer)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim labelFont As Font = css.GetFont(CSSFont.TryParse(theme.tagCSS))
        Dim labelConnector As Pen = css.GetPen(Stroke.TryParse(theme.tagLinkStroke))
        Dim anchors As Anchor() = labels.GetLabelAnchors(r:=3)

        If labelLayoutTicks > 0 Then
            Call d3js.labeler(maxAngle:=5, maxMove:=300, w_lab2:=100, w_lab_anc:=100) _
                .Labels(labels) _
                .Anchors(anchors) _
                .Width(rect.Width) _
                .Height(rect.Height) _
                .Start(showProgress:=False, nsweeps:=labelLayoutTicks)
        End If

        Dim labelBrush As Brush = theme.tagColor.GetBrush

        For Each i As SeqValue(Of Label) In labels.SeqIterator
            If labelLayoutTicks > 0 Then
                Call g.DrawLine(labelConnector, i.value.GetTextAnchor(anchors(i)), anchors(i))
            End If

            Call g.DrawString(i.value.text, labelFont, labelBrush, i.value.location)
        Next
    End Sub

    Friend Shared Sub DrawTICLegends(g As IGraphics, canvas As GraphicsRegion, legends As LegendObject(), split_size As Integer, outside As Boolean)
        ' 如果离子数量非常多的话,则会显示不完
        ' 这时候每20个换一列
        Dim deln As Integer = If(split_size <= 0, 16, split_size)
        Dim cols = legends.Length / deln
        Dim css As CSSEnvirnment = g.LoadEnvironment

        If cols > Fix(cols) Then
            ' 有余数,说明还有剩下的,增加一列
            cols += 1
        End If

        ' 计算在右上角的位置
        Dim plotRect = canvas.PlotRegion(css)
        Dim maxSize As SizeF = legends.MaxLegendSize(g)
        Dim top = plotRect.Top + maxSize.Height + 5
        Dim maxLen = maxSize.Width
        Dim legendShapeWidth% = 70
        Dim left As Double

        If outside Then
            left = plotRect.Right + g.MeasureString("A", legends(Scan0).GetFont(css)).Width
        Else
            left = plotRect.Right - (maxLen + legendShapeWidth) * cols
        End If

        Dim position As New Point With {
            .X = left,
            .Y = top
        }

        For Each block As LegendObject() In legends.Split(deln)
            g.DrawLegends(position, block, $"{legendShapeWidth},10", d:=0)
            position = New Point With {
                .X = position.X + maxLen + legendShapeWidth,
                .Y = position.Y
            }
        Next
    End Sub
End Class
