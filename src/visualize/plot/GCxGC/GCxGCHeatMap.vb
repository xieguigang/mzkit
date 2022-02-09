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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS

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
        Dim t1 As New DoubleRange(point.X - w_rt1, point.X + w_rt1)
        Dim t2 As New DoubleRange(point.Y - w_rt2, point.Y + w_rt2)
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
                scaleX = d3js.scale.linear.domain(values:=col.Select(Function(d) d.scan_time)).range({x, x + wx})
                scaleY = d3js.scale.linear.domain(values:=col.Select(Function(d) d.times).IteratesALL).range({y, y + wy})
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

            Call g.DrawString(points(i).Name, rowLabelFont, Brushes.Black, New PointF(rect.Right, y + (wy - fontHeight) / 2))

            y += wy + dy
        Next

        ' draw sample labels
        x = rect.Left
        y = rect.Bottom

        For Each sample In Me.gcxgc
            Dim fontSize As SizeF = g.MeasureString(sample.name, rowLabelFont)
            Dim pos As New PointF With {
                .X = x + (wx - fontSize.Width) / 2,
                .Y = y
            }

            Call g.DrawString(sample.name, rowLabelFont, Brushes.Black, pos)
        Next
    End Sub
End Class
