Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class ScanContour : Inherits Plot

    Dim scans As ms1_scan()

    Public Sub New(scans As ms1_scan(), theme As Theme)
        MyBase.New(theme)

        Me.scans = scans
    End Sub

    ''' <summary>
    ''' group by mz/rt
    ''' </summary>
    ''' <returns></returns>
    Private Iterator Function CreateMatrix() As IEnumerable(Of DataSet)
        Dim mzRange As Double() = scans.Select(Function(x) x.mz).CreateAxisTicks
        Dim rtRange As Double() = scans.Select(Function(x) x.scan_time).CreateAxisTicks
        Dim rtLeft As Double = 0
        Dim mzLeft As Double = 0
        Dim mzi As Double

        For rt As Double = rtRange.Min To rtRange.Max Step (rtRange.Max - rtRange.Min) / 20
            Dim rti As Double = rt
            Dim rtRow As ms1_scan() = scans _
                .Where(Function(x) x.scan_time >= rtLeft AndAlso x.scan_time < rti) _
                .ToArray
            Dim row As New DataSet With {
                .ID = rti.ToString
            }

            For mz As Double = mzRange.Min To mzRange.Max Step (mzRange.Max - mzRange.Min) / 20
                mzi = mz
                row(mz.ToString) = rtRow _
                    .Where(Function(x) x.mz >= mzLeft AndAlso x.mz < mzi) _
                    .OrderByDescending(Function(x) x.intensity) _
                    .FirstOrDefault _
                   ?.intensity

                mzLeft = mz
            Next

            mzLeft = 0
            rtLeft = rt
        Next
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim matrix As DataSet() = CreateMatrix.ToArray

        Call Contour.CreatePlot(
            matrix:=matrix,
            legendTitle:="Scan Contour",
            xlabel:="scan_time(seconds)",
            ylabel:="m/z"
        ).Plot(g, canvas)
    End Sub
End Class
