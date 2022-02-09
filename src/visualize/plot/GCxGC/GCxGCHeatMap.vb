Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq

Public Class GCxGCHeatMap : Inherits Plot

    ReadOnly gcxgc As NamedCollection(Of D2Chromatogram)()
    ReadOnly w_rt1 As Double
    ReadOnly w_rt2 As Double
    ReadOnly points As NamedValue(Of PointF)()

    Public Sub New(gcxgc As IEnumerable(Of NamedCollection(Of D2Chromatogram)), points As IEnumerable(Of NamedValue(Of PointF)), rt1 As Double, rt2 As Double, theme As Theme)
        MyBase.New(theme)

        Me.gcxgc = gcxgc.ToArray
        Me.w_rt1 = rt1
        Me.w_rt2 = rt2
        Me.points = points.ToArray
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
        Dim dx As Double = 5
        Dim dy As Double = 5
        Dim wx As Double = (rect.Width - dx * (ncols - 1)) / ncols
        Dim wy As Double = (rect.Height - dy * (nrows - 1)) / nrows
        Dim matrix = points.Select(Function(cpd)
                                       Return gcxgc.Select(Function(c) GetRectangle(c.value, cpd)).ToArray
                                   End Function).ToArray
        Dim valueRange As DoubleRange = matrix.IteratesALL.IteratesALL.Select(Function(t) t.chromatogram).IteratesALL.Select(Function(d) d.Intensity).Range

    End Sub
End Class
