﻿#Region "Microsoft.VisualBasic::d512de7281d1f8e0a04a79e8a5c4a387, visualize\plot\ChromatogramPlot\ScanVisual3D.vb"

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

    '   Total Lines: 282
    '    Code Lines: 211 (74.82%)
    ' Comment Lines: 32 (11.35%)
    '    - Xml Docs: 15.62%
    ' 
    '   Blank Lines: 39 (13.83%)
    '     File Size: 11.37 KB


    ' Class ScanVisual3D
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: evalDc, evalDx, evalDy, GetScanCollection
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports std = System.Math

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

' intensity
'  ^ y   z
'  |   / m/z
'  |  /  
'  | /
'  |/          retention time
'  ------------------> X 

Public Class ScanVisual3D : Inherits Plot

    ReadOnly scans As NamedCollection(Of ChromatogramTick)()
    ReadOnly angle As Double = 60
    ReadOnly height As Double = 0.3
    ReadOnly fillCurve As Boolean
    ReadOnly fillAlpha As Integer
    ReadOnly drawParallelAxis As Boolean

#Region "constructor"
    Public Sub New(scans As IEnumerable(Of ms1_scan), tolerance As Tolerance, angle As Double, fillCurve As Boolean, fillAlpha As Integer, drawParallelAxis As Boolean, theme As Theme)
        Call Me.New(GetScanCollection(scans, tolerance), angle, fillCurve, fillAlpha, drawParallelAxis, theme)
    End Sub

    Public Sub New(scans As IEnumerable(Of NamedCollection(Of ChromatogramTick)), angle As Double, fillCurve As Boolean, fillAlpha As Integer, drawParallelAxis As Boolean, theme As Theme)
        MyBase.New(theme)

        Me.fillCurve = fillCurve
        Me.fillAlpha = fillAlpha
        Me.angle = angle
        Me.scans = scans.ToArray
        Me.drawParallelAxis = drawParallelAxis
    End Sub
#End Region

    Private Shared Iterator Function GetScanCollection(points As IEnumerable(Of ms1_scan), tolerance As Tolerance) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each scan In points _
             .GroupBy(Function(p) p.mz, tolerance) _
             .Select(Function(mz)
                         Return (Val(mz.name), mz.OrderBy(Function(t) t.scan_time).ToArray)
                     End Function) _
             .OrderBy(Function(mz) mz.Item1)

            Yield New NamedCollection(Of ChromatogramTick) With {
                .name = scan.Item1.ToString("F4"),
                .value = scan.Item2 _
                    .Select(Function(t)
                                Return New ChromatogramTick With {
                                    .Intensity = t.intensity,
                                    .Time = t.scan_time
                                }
                            End Function) _
                    .ToArray
            }
        Next
    End Function

    Private Function evalDx(canvas As GraphicsRegion) As Double
        ' cos(a) = dx / dc
        ' dx = cos(a) * dc
        Dim dc As Double = evalDc(canvas)
        Dim dx As Double = std.Cos(angle.ToRadians) * dc

        Return dx / (scans.Length + 1)
    End Function

    ''' <summary>
    ''' 计算出直角三角形的斜边长度
    ''' </summary>
    ''' <param name="canvas"></param>
    ''' <returns></returns>
    Private Function evalDc(canvas As GraphicsRegion) As Double
        Dim rect As Rectangle = canvas.PlotRegion(New CSSEnvirnment(canvas.Size))
        Dim height As Double = 0 ' Me.height * rect.Height
        Dim y1 As Double = rect.Bottom - height
        Dim y2 As Double = rect.Top
        Dim dh As Double = y1 - y2

        ' tan(a) = dh / dw
        ' dw = dh / tan(a)

        Dim dw As Double = dh / std.Tan(a:=angle.ToRadians)

        ' c/|
        ' /a|
        ' ---
        ' dw

        ' cos(a) = dw / c
        ' c = dw / cos(a)

        Dim c As Double = dw / std.Cos(d:=angle.ToRadians)

        Return c
    End Function

    Private Function evalDy(canvas As GraphicsRegion) As Double
        ' sin(a) = dy / dc
        ' dy = sin(a) * dc
        Dim dc As Double = evalDc(canvas)
        Dim dy As Double = std.Sin(angle.ToRadians) * dc

        Return dy / (scans.Length + 1)
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim plotRect = canvas.PlotRegion(css)
        Dim dx As Double = evalDx(canvas)
        Dim dy As Double = plotRect.Height * Me.height / (scans.Length + 1)  ' evalDy(canvas)
        Dim theme As Theme = Me.theme.Clone
        Dim colors As String() = Designer _
            .GetColors(theme.colorSet, scans.Length) _
            .Select(AddressOf ToHtmlColor) _
            .ToArray
        Dim padding As PaddingLayout = PaddingLayout.EvaluateFromCSS(css, canvas.Padding)
        Dim parallelCanvas As New GraphicsRegion With {
            .Size = canvas.Size,
            .Padding = New Padding With {
                .Top = padding.Top,
                .Left = padding.Left + dx * scans.Length,
                .Right = padding.Right,
                .Bottom = padding.Bottom + Me.height * plotRect.Height
            }
        }
        Dim labelList As New List(Of Label)
        Dim legendList As New List(Of LegendObject)

        theme.background = "transparent"
        theme.gridFill = "transparent"
        theme.drawGrid = False
        theme.drawLegend = False
        theme.drawAxis = False
        theme.drawLabels = False

        parallelCanvas = parallelCanvas.Offset2D(dx, -dy)

        Dim firstFrame As GraphicsRegion
        Dim lastFrame As GraphicsRegion
        Dim parallelXAxisPen As Pen = CSS.GetPen(Stroke.TryParse(theme.gridStrokeX))
        Dim maxinto As Double = Aggregate scan As NamedCollection(Of ChromatogramTick)
                                In scans
                                Let into As Double = scan _
                                    .Select(Function(tick) tick.Intensity) _
                                    .Max
                                Into Max(into)
        Dim rtAll As (min#, max#) = scans _
            .Select(Function(chr) chr.Select(Function(t) t.Time)) _
            .IteratesALL _
            .Range

        For i As Integer = 0 To scans.Length - 1
            Dim labels As Label() = Nothing
            Dim legends As LegendObject() = Nothing

            parallelCanvas = parallelCanvas.Offset2D(-dx, dy)
            theme.colorSet = colors(i)

            If i = 0 Then
                theme.drawGrid = True
                theme.gridFill = Me.theme.gridFill
                theme.drawAxis = True
                theme.yAxisLayout = YAxisLayoutStyles.None
                theme.xAxisLayout = XAxisLayoutStyles.None

                firstFrame = parallelCanvas
            Else
                theme.gridFill = "transparent"
                theme.drawGrid = False
                theme.drawAxis = False

                If i = scans.Length - 1 Then
                    theme.drawAxis = True
                    theme.yAxisLayout = YAxisLayoutStyles.Left
                    theme.xAxisLayout = XAxisLayoutStyles.Bottom

                    lastFrame = parallelCanvas
                Else
                    theme.drawAxis = False
                End If
            End If

            If drawParallelAxis Then
                Dim x0 = css.GetWidth(parallelCanvas.Padding.Left)
                Dim y0 = canvas.Height - css.GetHeight(parallelCanvas.Padding.Bottom)
                Dim x1 = canvas.Width - css.GetWidth(parallelCanvas.Padding.Right)
                Dim y1 = canvas.Height - css.GetHeight(parallelCanvas.Padding.Bottom)

                Call g.DrawLine(parallelXAxisPen, x0, y0, x1, y1)
            End If

            Call New TICplot(
                ionData:={scans(i)},
                timeRange:={rtAll.min, rtAll.max},
                intensityMax:=maxinto,
                isXIC:=False,
                fillCurve:=fillCurve,
                fillAlpha:=fillAlpha,
                labelLayoutTicks:=-1,
                theme:=theme,
                bspline:=0
            ) With {
                .xlabel = xlabel,
                .ylabel = ylabel,
                .main = ""
            }.RunPlot(g, parallelCanvas, labels, legends)

            Call labelList.AddRange(labels)
            Call legendList.AddRange(legends)
        Next

        '     a
        '    /|
        '   / |
        ' d | |
        '   | / b --------/ e
        '   |/           /
        '   c ----------/
        '               f

        Dim a As New PointF(css.GetWidth(firstFrame.Padding.Left), css.GetHeight(firstFrame.Padding.Top))
        Dim b As New PointF(css.GetWidth(firstFrame.Padding.Left), canvas.Height - css.GetHeight(firstFrame.Padding.Bottom))
        Dim c As New PointF(css.GetWidth(lastFrame.Padding.Left), canvas.Height - css.GetHeight(lastFrame.Padding.Bottom))
        Dim d As New PointF(css.GetWidth(lastFrame.Padding.Left), css.GetHeight(lastFrame.Padding.Top))
        Dim e As New PointF(canvas.Width - css.GetWidth(firstFrame.Padding.Right), canvas.Height - css.GetHeight(firstFrame.Padding.Bottom))
        Dim f As New PointF(canvas.Width - css.GetWidth(lastFrame.Padding.Right), canvas.Height - css.GetHeight(lastFrame.Padding.Bottom))

        Dim axisPen As Pen = CSS.GetPen(Stroke.TryParse(theme.axisStroke))

        Call g.DrawLine(axisPen, a, d)
        Call g.DrawLine(axisPen, a, b)
        Call g.DrawLine(axisPen, c, b)
        Call g.DrawLine(axisPen, e, f)

        If Me.theme.drawLabels Then Call TICplot.DrawLabels(g, canvas.PlotRegion(css), labelList.ToArray, theme, 1500)
        If Me.theme.drawLegend Then Call TICplot.DrawTICLegends(g, canvas, legendList.ToArray, 100, outside:=True)

        Call DrawMainTitle(g, plotRegion:=canvas.PlotRegion(css))
    End Sub
End Class
