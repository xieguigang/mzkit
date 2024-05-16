#Region "Microsoft.VisualBasic::f761d23e8bd1fe1cff4cc07794bdc949, visualize\plot\GCxGC\GCxGCHeatMap.vb"

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

    '   Total Lines: 138
    '    Code Lines: 114
    ' Comment Lines: 3
    '   Blank Lines: 21
    '     File Size: 5.76 KB


    ' Class GCxGCHeatMap
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetRectangle
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports stdNum = System.Math

Public Class GCxGCHeatMap : Inherits Plot

    ReadOnly gcxgc As NamedCollection(Of D2Chromatogram)()
    ReadOnly w_rt1 As Double
    ReadOnly w_rt2 As Double
    ReadOnly points As NamedValue(Of PointF)()
    ReadOnly mapLevels As Integer

    Dim dx As Double = 5
    Dim dy As Double = 5

    Public Sub New(gcxgc As IEnumerable(Of NamedCollection(Of D2Chromatogram)),
                   points As IEnumerable(Of NamedValue(Of PointF)),
                   rt1 As Double,
                   rt2 As Double,
                   mapLevels As Integer,
                   marginX As Integer,
                   marginY As Integer,
                   theme As Theme)

        MyBase.New(theme)

        Me.gcxgc = gcxgc.ToArray
        Me.w_rt1 = rt1
        Me.w_rt2 = rt2
        Me.points = points.ToArray
        Me.mapLevels = mapLevels
        Me.dx = marginX
        Me.dy = marginY
    End Sub

    Private Function GetRectangle(gcxgc As D2Chromatogram(), point As PointF) As D2Chromatogram()
        Dim t1 As New DoubleRange(point.X - w_rt1 / 2, point.X + w_rt1 / 2)
        Dim t2 As New DoubleRange(point.Y - w_rt2 / 2, point.Y + w_rt2 / 2)
        Dim scans = gcxgc.Where(Function(i) t1.IsInside(i.scan_time)).OrderBy(Function(i) i.scan_time).ToArray
        Dim matrix = scans _
            .Select(Function(i)
                        Return New D2Chromatogram With {
                            .scan_time = i.scan_time,
                            .intensity = i.intensity,
                            .chromatogram = i(t2)
                        }
                    End Function) _
            .ToArray

        Return matrix
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim ncols As Integer = gcxgc.Length
        Dim nrows As Integer = points.Length
        Dim rect As Rectangle = canvas.PlotRegion
        Dim wx As Double = (rect.Width - dx * (ncols - 1)) / ncols
        Dim wy As Double = (rect.Height - dy * (nrows - 1)) / nrows
        Dim matrix = points.Select(Function(cpd)
                                       Return gcxgc.Select(Function(c) GetRectangle(c.value, cpd)).ToArray
                                   End Function).ToArray
        Dim valueRange As DoubleRange = matrix.IteratesALL.IteratesALL.Select(Function(t) t.chromatogram).IteratesALL.Select(Function(d) d.Intensity).Range
        Dim indexRange As New DoubleRange(0, mapLevels)
        Dim colors As SolidBrush() = Designer.GetColors(theme.colorSet, mapLevels).Select(Function(c) New SolidBrush(c)).ToArray
        Dim x As Double = rect.Left
        Dim y As Double = rect.Top
        Dim scaleX As d3js.scale.LinearScale
        Dim scaleY As d3js.scale.LinearScale
        Dim scale As DataScaler
        Dim n As Double
        Dim rowLabelFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
        Dim fontHeight As Double = g.MeasureString("H", rowLabelFont).Height

        For i As Integer = 0 To matrix.Length - 1
            ' for each metabolite row
            x = rect.Left

            For Each col As D2Chromatogram() In matrix(i)
                scaleX = d3js.scale.linear.domain(values:=col.Select(Function(d) d.scan_time)).range(values:={x, x + wx})
                scaleY = d3js.scale.linear.domain(values:=col.Select(Function(d) d.times).IteratesALL).range(values:={y, y + wy})
                n = col.Select(Function(d) d.size).Average
                scale = New DataScaler() With {
                    .X = scaleX,
                    .Y = scaleY,
                    .region = New Rectangle With {
                        .X = x,
                        .Y = y,
                        .Width = wx,
                        .Height = wy
                    }
                }

                Call GCxGCTIC2DPlot.FillHeatMap(g, col, wx / col.Length, wy / n, scale, valueRange, indexRange, colors)

                x += wx + dx
            Next

            Call g.DrawString(points(i).Name, rowLabelFont, Brushes.Black, New PointF(rect.Right + dx, y + (wy - fontHeight) / 2))

            y += wy + dy
        Next

        ' draw sample labels
        Dim tickFont = rowLabelFont
        Dim tickColor As Brush = Brushes.Black

        x = rect.Left
        y = rect.Bottom

        For Each sample In Me.gcxgc
            Dim fontSize As SizeF = g.MeasureString(sample.name, rowLabelFont)
            Dim labelText = sample.name
            Dim pos As New Point With {
                .X = x + wx / 2,
                .Y = y
            }

            x += wx + dx
            ' g.DrawString(sample.name, rowLabelFont, Brushes.Black, pos)

            Dim text As New GraphicsText(g)
            Dim xRotate As Double = 45

            Call text.DrawString(labelText, tickFont, tickColor, New Point(pos.X, pos.Y + fontSize.Height * stdNum.Sin(xRotate * 180 / stdNum.PI)), angle:=xRotate)
        Next
    End Sub
End Class
