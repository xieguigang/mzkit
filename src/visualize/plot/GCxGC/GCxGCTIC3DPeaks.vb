Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing3D

''' <summary>
''' plot GCxGC TIC in 3D peaks style
''' </summary>
Public Class GCxGCTIC3DPeaks : Inherits Plot

    ReadOnly raw As D2Chromatogram()
    ReadOnly sampling As Integer

    Public Sub New(gcxgc As D2Chromatogram(), sampling As Integer, theme As Theme)
        MyBase.New(theme)

        Me.raw = gcxgc
        Me.sampling = sampling
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim mesh3D As Surface() = MeshGrid(gcxgc:=raw, sampling:=sampling).ToArray
    End Sub

    Public Shared Iterator Function MeshGrid(gcxgc As D2Chromatogram(), sampling As Integer) As IEnumerable(Of Surface)
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
                Yield New Surface With {
                    .vertices = {
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
