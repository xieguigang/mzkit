Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime

Public Class TICplot : Inherits Plot

    Public Sub New(theme As Theme)
        MyBase.New(theme)
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim labelFont As Font = CSSFont.TryParse(labelFontStyle)
        Dim labelConnector As Pen = Stroke.TryParse(labelConnectorStroke)

        'For Each ion As NamedCollection(Of ChromatogramTick) In ionData
        '    Dim base = ion.value.Baseline(quantile:=0.65)
        '    Dim max# = ion.value.Shadows!Intensity.Max

        '    Call $"{ion.name}: {base}/{max} = {(100 * base / max).ToString("F2")}%".__DEBUG_ECHO
        'Next

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

        Dim XTicks As Double() = ionData _
            .Select(Function(ion)
                        Return ion.value.TimeArray
                    End Function) _
            .IteratesALL _
            .JoinIterates(TimeRange) _
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

        Dim ZTicks As Double()

        If Parallel Then
            ZTicks = ionData _
                .Sequence _
                .Select(Function(i) CDbl(i)) _
                .AsVector _
                .Range _
                .CreateAxisTicks
        End If

        Dim rect As Rectangle = Region.PlotRegion
        Dim X = d3js.scale.linear.domain(XTicks).range(integers:={rect.Left, rect.Right})
        Dim Y = d3js.scale.linear.domain(YTicks).range(integers:={rect.Top, rect.Bottom})
        Dim scaler As New DataScaler With {
            .AxisTicks = (XTicks, YTicks),
            .region = rect,
            .X = X,
            .Y = Y
        }

        Call g.DrawAxis(
            Region, scaler, showGrid:=showGrid,
            xlabel:="Time (s)",
            ylabel:="Intensity",
            htmlLabel:=False,
            XtickFormat:=If(isXIC, "F2", "F0"),
            YtickFormat:="G2",
            labelFont:=axisLabelFont,
            tickFontStyle:=axisTickFont,
            gridFill:=gridFill
        )

        If Parallel Then
            ' draw Z axis

        End If

        Dim legends As New List(Of LegendObject)
        Dim peakTimes As New List(Of NamedValue(Of ChromatogramTick))
        Dim fillColor As Brush
        Dim parallelOffset As New PointF

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
                .fontstyle = legendFontCSS,
                .style = LegendStyles.Rectangle
            }
            peakTimes += New NamedValue(Of ChromatogramTick) With {
                .Name = line.name,
                .Value = chromatogram(Which.Max(chromatogram.Shadows!Intensity))
            }

            Dim A, B As PointF
            Dim polygon As New List(Of PointF)

            If Parallel Then
                ' add [x,y] offset for current data
                parallelOffset = New PointF With {
                    .X = parallelOffset.X + rect.Height / (ionData.Length + 2),
                    .Y = parallelOffset.Y - rect.Height / (ionData.Length + 2)
                }
            End If

            For Each signal As SlideWindow(Of PointF) In chromatogram _
                .Select(Function(c)
                            Return New PointF(c.Time, c.Intensity)
                        End Function) _
                .SlideWindows(winSize:=2)

                A = scaler.Translate(signal.First).OffSet2D(parallelOffset)
                B = scaler.Translate(signal.Last).OffSet2D(parallelOffset)

                Call g.DrawLine(curvePen, A, B)

                If polygon = 0 Then
                    polygon.Add(A)
                End If

                polygon.Add(B)
            Next

            Dim bottom% = Region.Bottom - 6

            polygon.Insert(0, New PointF(polygon(0).X, bottom))
            polygon.Add(New PointF(polygon.Last.X, bottom))

            If fillCurve Then
                fillColor = New SolidBrush(Color.FromArgb(fillAlpha, curvePen.Color))
                g.FillPolygon(fillColor, polygon)
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
            Dim maxSize As SizeF = legends.MaxLegendSize(g)
            Dim top = Region.PlotRegion.Top + maxSize.Height + 5
            Dim maxLen = maxSize.Width
            Dim legendShapeWidth% = 70
            Dim left As Double = Region.PlotRegion.Right - (maxLen + legendShapeWidth) * cols
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
        End If
    End Sub
End Class
