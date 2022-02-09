Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Device
Imports Microsoft.VisualBasic.Data.ChartPlots.Plot3D.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS

''' <summary>
''' plot GCxGC TIC in 3D peaks style
''' </summary>
Public Class GCxGCTIC3DPeaks : Inherits Plot

    ReadOnly raw As D2Chromatogram()
    ReadOnly sampling As Integer
    ReadOnly mapLevels As Integer

    Public Sub New(gcxgc As D2Chromatogram(), sampling As Integer, mapLevels As Integer, theme As Theme)
        MyBase.New(theme)

        Me.raw = gcxgc
        Me.sampling = sampling
        Me.mapLevels = mapLevels
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim mesh3D As Polygon() = MeshGrid(gcxgc:=raw, sampling:=sampling).ToArray
        Dim colors As SolidBrush() = Designer.GetColors(theme.colorSet, mapLevels).Select(Function(c) New SolidBrush(c)).ToArray
        Dim z As Double() = mesh3D.Select(Function(s) s.Path.Select(Function(p) p.Z).Average).ToArray
        Dim range As New DoubleRange(z)
        Dim indexRange As New DoubleRange(0, mapLevels - 1)
        Dim plotSize = canvas.PlotRegion.Size

        ' render color
        For i As Integer = 0 To mesh3D.Length - 1
            mesh3D(i).Brush = colors(CInt(range.ScaleMapping(z(i), indexRange)))
        Next

        Dim camera As New Camera With {
            .screen = plotSize,
            .fov = 100000,
            .viewDistance = -75,
            .angleX = 31.5,
            .angleY = 65,
            .angleZ = 125
        }
        Dim ppi As Integer = g.Dpi
        Dim xTicks = raw.Select(Function(s) s.scan_time).Range.CreateAxisTicks
        Dim yTicks = raw.Select(Function(s) s.chromatogram.Select(Function(t) t.Time)).IteratesALL.Range.CreateAxisTicks
        Dim zTicks = mesh3D.Select(Function(s) s.Path.Select(Function(p) p.Z)).IteratesALL.Range.CreateAxisTicks
        Dim tickCss As String = CSSFont.TryParse(theme.axisTickCSS).SetFontColor(theme.mainTextColor).ToString
        Dim models As New List(Of Element3D)

        ' 然后生成底部的网格
        Call Grids.Grid1(xTicks, yTicks, (xTicks(1) - xTicks(0), yTicks(1) - yTicks(0)), zTicks.Min, showTicks:=Not theme.axisTickCSS.StringEmpty, strokeCSS:=theme.gridStrokeX, tickCSS:=tickCss).DoCall(AddressOf models.AddRange)
        Call Grids.Grid2(xTicks, zTicks, (xTicks(1) - xTicks(0), zTicks(1) - zTicks(0)), yTicks.Min, showTicks:=Not theme.axisTickCSS.StringEmpty, strokeCSS:=theme.gridStrokeX, tickCSS:=tickCss).DoCall(AddressOf models.AddRange)
        Call Grids.Grid3(yTicks, zTicks, (yTicks(1) - yTicks(0), zTicks(1) - zTicks(0)), xTicks.Max, showTicks:=Not theme.axisTickCSS.StringEmpty, strokeCSS:=theme.gridStrokeX, tickCSS:=tickCss).DoCall(AddressOf models.AddRange)
        Call AxisDraw.Axis(
            xrange:=xTicks, yrange:=yTicks, zrange:=zTicks,
            labelFontCss:=theme.axisLabelCSS,
            labels:=(xlabel, ylabel, zlabel),
            strokeCSS:=theme.axisStroke,
            arrowFactor:="1,2",
            labelColorVal:=theme.mainTextColor
        ).DoCall(AddressOf models.AddRange)

        Call models.AddRange(mesh3D)

        Call models.RenderAs3DChart(
            canvas:=g,
            camera:=camera,
            region:=canvas,
            theme:=theme
        )
    End Sub

    Public Shared Iterator Function MeshGrid(gcxgc As D2Chromatogram(), sampling As Integer) As IEnumerable(Of Polygon)
        Dim height As Integer = gcxgc _
            .Select(Function(d) d.size) _
            .GroupBy(Function(n) n) _
            .OrderByDescending(Function(d) d.Count) _
            .First _
            .Key

        gcxgc = (From scan As D2Chromatogram
                 In gcxgc
                 Where scan.size = height
                 Order By scan.scan_time).ToArray

        Dim left As D2Chromatogram = gcxgc(Scan0)

        For i As Integer = sampling To gcxgc.Length - 1 Step sampling
            Dim right As D2Chromatogram = gcxgc(i)
            Dim bottom1 As ChromatogramTick = left(Scan0)
            Dim bottom2 As ChromatogramTick = right(Scan0)

            For j As Integer = sampling To height - 1 Step sampling
                Dim top1 = left(j)
                Dim top2 = right(j)

                ' top1, top2, bottom2, bottom1
                Yield New Polygon With {
                    .Path = {
                        New Point3D(left.scan_time, top1.Time, top1.Intensity),
                        New Point3D(right.scan_time, top2.Time, top2.Intensity),
                        New Point3D(right.scan_time, bottom2.Time, bottom2.Intensity),
                        New Point3D(left.scan_time, bottom1.Time, bottom1.Intensity)
                    }
                }
            Next

            left = right
        Next
    End Function
End Class
