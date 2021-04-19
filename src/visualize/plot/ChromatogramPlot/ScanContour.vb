Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Public Class ScanContour : Inherits Plot

    Dim scans As ms1_scan()
    Dim mzRange As DoubleRange
    Dim rtRange As DoubleRange
    Dim xsteps As Double
    Dim ysteps As Double

    Public Sub New(scans As ms1_scan(), theme As Theme)
        MyBase.New(theme)

        Me.scans = scans
        Me.mzRange = scans.Select(Function(x) x.mz).CreateAxisTicks
        Me.rtRange = scans.Select(Function(x) x.scan_time).CreateAxisTicks
        Me.xsteps = (rtRange.Max - rtRange.Min) / 800
        Me.ysteps = (mzRange.Max - mzRange.Min) / 800
    End Sub

    ''' <summary>
    ''' group by mz/rt
    ''' </summary>
    ''' <returns></returns>
    Private Iterator Function CreateMatrix() As IEnumerable(Of DataSet)
        Dim rtLeft As Double = 0
        Dim mzLeft As Double = 0
        Dim mzi As Double

        For rt As Double = rtRange.Min To rtRange.Max Step xsteps
            Dim rti As Double = rt
            Dim rtRow As ms1_scan() = scans _
                .Where(Function(x) x.scan_time >= rtLeft AndAlso x.scan_time < rti) _
                .ToArray
            Dim row As New DataSet With {
                .ID = rti.ToString
            }

            If rtRow.Length = 0 Then
                Yield row
                Continue For
            End If

            For mz As Double = mzRange.Min To mzRange.Max Step ysteps
                mzi = mz

                With rtRow _
                    .Where(Function(x)
                               Return x.mz >= mzLeft AndAlso x.mz < mzi
                           End Function) _
                    .ToArray

                    If .Length = 0 Then
                        row(mz.ToString) = 0.0
                    Else
                        row(mz.ToString) =
                            .OrderByDescending(Function(x) x.intensity) _
                            .First _
                            .intensity _
                            .DoCall(AddressOf stdNum.Log10)
                    End If
                End With

                mzLeft = mz
            Next

            mzLeft = 0
            rtLeft = rt

            Yield row
        Next
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim matrix As DataSet() = CreateMatrix.ToArray

        Call Contour.CreatePlot(
            matrix:=matrix,
            legendTitle:="Scan Contour",
            xlabel:="scan_time(seconds)",
            ylabel:="m/z",
            unit:=stdNum.Max(xsteps, ysteps),
            legendTickFormat:="G2",
            colorMap:=theme.colorSet,
            xsteps:=xsteps,
            ysteps:=ysteps
        ).Plot(g, canvas)
    End Sub

    Public Overloads Shared Function Plot(data As IEnumerable(Of ms1_scan),
                                          Optional size$ = "3600,2400",
                                          Optional padding$ = "padding: 100px 900px 250px 300px;",
                                          Optional bg$ = "white",
                                          Optional colorSet$ = "darkblue,blue,skyblue,green,orange,red,darkred") As GraphicsData
        Dim theme As New Theme With {
            .colorSet = colorSet,
            .padding = padding,
            .background = bg
        }
        Dim app As New ScanContour(data.ToArray, theme) With {
            .xlabel = "scan_time (seconds)",
            .ylabel = "M/z"
        }

        Return app.Plot(size:=size)
    End Function
End Class
