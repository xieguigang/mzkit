Imports System.Drawing
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
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Class TICplot : Inherits Plot

    ReadOnly ionData As NamedCollection(Of ChromatogramTick)()
    ReadOnly timeRange As Double() = Nothing
    ReadOnly intensityMax As Double = 0
    ReadOnly isXIC As Boolean
    ReadOnly fillCurve As Boolean
    ReadOnly fillAlpha As Integer
    ReadOnly labelLayoutTicks As Integer = 100
    ReadOnly deln As Integer = 10

    Public Sub New(ionData As NamedCollection(Of ChromatogramTick)(),
                   timeRange As Double(),
                   intensityMax As Double,
                   isXIC As Boolean,
                   fillCurve As Boolean,
                   fillAlpha As Integer,
                   labelLayoutTicks As Integer,
                   deln As Integer,
                   theme As Theme)

        MyBase.New(theme)

        Me.isXIC = isXIC
        Me.intensityMax = intensityMax
        Me.ionData = ionData
        Me.fillCurve = fillCurve
        Me.fillAlpha = fillAlpha
        Me.labelLayoutTicks = labelLayoutTicks
        Me.deln = deln

        If timeRange Is Nothing Then
            Me.timeRange = {}
        Else
            Me.timeRange = timeRange
        End If
    End Sub

    Private Function newPen(c As Color) As Pen
        Dim style As Stroke = Stroke.TryParse(theme.lineStroke)
        style.fill = c.ARGBExpression
        Return style.GDIObject
    End Function

    Private Function colorProvider() As LoopArray(Of Pen)
        If ionData.Length = 1 Then
            Return {newPen(theme.colorSet.TranslateColor(False) Or Color.DeepSkyBlue.AsDefault)}
        Else
            Return Designer _
                .GetColors(theme.colorSet) _
                .Select(AddressOf newPen) _
                .ToArray
        End If
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim colors As LoopArray(Of Pen) = colorProvider()
        Dim XTicks As Double() = ionData _
            .Select(Function(ion)
                        Return ion.value.TimeArray
                    End Function) _
            .IteratesALL _
            .JoinIterates(timeRange) _
            .AsVector _
            .Range _
            .CreateAxisTicks  ' time

        Dim YTicks = ionData _
            .Select(Function(ion)
                        Return ion.value.IntensityArray
                    End Function) _
            .IteratesALL _
            .JoinIterates({intensityMax}) _
            .AsVector _
            .Range _
            .CreateAxisTicks ' intensity

        Dim rect As Rectangle = canvas.PlotRegion
        Dim X = d3js.scale.linear.domain(XTicks).range(integers:={rect.Left, rect.Right})
        Dim Y = d3js.scale.linear.domain(YTicks).range(integers:={rect.Top, rect.Bottom})
        Dim scaler As New DataScaler With {
            .AxisTicks = (XTicks, YTicks),
            .region = rect,
            .X = X,
            .Y = Y
        }

        If theme.drawAxis Then
            Call g.DrawAxis(
                canvas, scaler, showGrid:=theme.drawGrid,
                xlabel:="Time (s)",
                ylabel:="Intensity",
                htmlLabel:=False,
                XtickFormat:=If(isXIC, "F2", "F0"),
                YtickFormat:="G2",
                labelFont:=theme.axisLabelCSS,
                tickFontStyle:=theme.axisTickCSS,
                gridFill:=theme.gridFill
            )
        End If

        Dim legends As New List(Of LegendObject)
        Dim peakTimes As New List(Of NamedValue(Of ChromatogramTick))
        Dim fillColor As Brush

        For i As Integer = 0 To ionData.Length - 1
            Dim curvePen As Pen = colors.Next
            Dim line = ionData(i)
            Dim chromatogram = line.value

            If chromatogram.IsNullOrEmpty Then
                Call $"ion not found in raw file: '{line.name}'".Warning
                Continue For
            End If

            legends += New LegendObject With {
                .title = line.name,
                .color = curvePen.Color.ToHtmlColor,
                .fontstyle = theme.legendLabelCSS,
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

            Dim bottom% = canvas.Bottom - 6

            polygon.Insert(0, New PointF(polygon(0).X, bottom))
            polygon.Add(New PointF(polygon.Last.X, bottom))

            If fillCurve Then
                fillColor = New SolidBrush(Color.FromArgb(fillAlpha, curvePen.Color))
                g.FillPolygon(fillColor, polygon)
            End If
        Next

        If theme.drawLabels Then Call DrawLabels(g, rect, scaler, peakTimes)
        If theme.drawLegend Then Call DrawTICLegends(g, canvas, legends)
    End Sub

    Private Sub DrawLabels(g As IGraphics, rect As Rectangle,
                           scaler As DataScaler,
                           peakTimes As IEnumerable(Of NamedValue(Of ChromatogramTick)))

        Dim labelFont As Font = CSSFont.TryParse(theme.tagCSS)
        Dim labelConnector As Pen = Stroke.TryParse(theme.tagLinkStroke)
        ' labeling 
        Dim labels As Label() = peakTimes _
            .Select(Function(ion)
                        Dim labelSize As SizeF = g.MeasureString(ion.Name, labelFont)
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

        Dim labelBrush As Brush = theme.tagColor.GetBrush

        For Each i As SeqValue(Of Label) In labels.SeqIterator
            Call g.DrawLine(labelConnector, i.value.GetTextAnchor(anchors(i)), anchors(i))
            Call g.DrawString(i.value.text, labelFont, labelBrush, i.value)
        Next
    End Sub

    Private Sub DrawTICLegends(g As IGraphics, canvas As GraphicsRegion, legends As List(Of LegendObject))
        ' 如果离子数量非常多的话,则会显示不完
        ' 这时候每20个换一列
        Dim cols = legends.Count / deln

        If cols > Fix(cols) Then
            ' 有余数,说明还有剩下的,增加一列
            cols += 1
        End If

        ' 计算在右上角的位置
        Dim maxSize As SizeF = legends.MaxLegendSize(g)
        Dim top = canvas.PlotRegion.Top + maxSize.Height + 5
        Dim maxLen = maxSize.Width
        Dim legendShapeWidth% = 70
        Dim left As Double = canvas.PlotRegion.Right - (maxLen + legendShapeWidth) * cols
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
