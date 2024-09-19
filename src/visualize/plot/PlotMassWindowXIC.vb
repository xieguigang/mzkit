#Region "Microsoft.VisualBasic::95e4e9513a9dd773a640c5a197b1d5b6, visualize\plot\PlotMassWindowXIC.vb"

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

    '   Total Lines: 109
    '    Code Lines: 86 (78.90%)
    ' Comment Lines: 12 (11.01%)
    '    - Xml Docs: 91.67%
    ' 
    '   Blank Lines: 11 (10.09%)
    '     File Size: 4.63 KB


    ' Class PlotMassWindowXIC
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: loadXIC
    ' 
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

''' <summary>
''' Mass window plot combine with XIC plot
''' </summary>
''' <remarks>
''' the plot figure combine with two chart:
''' 
''' 1. top: XIC plot
''' 2. bottom: mass window plot
''' </remarks>
Public Class PlotMassWindowXIC : Inherits Plot

    ''' <summary>
    ''' mass window data re-order by mz asc
    ''' </summary>
    ReadOnly mass_windows As DoubleTagged(Of ms1_scan())()
    ReadOnly xic As ChromatogramTick()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="xic"></param>
    ''' <param name="theme"></param>
    ''' <param name="mz">
    ''' the target ion m/z value for extract the XIC data
    ''' </param>
    ''' <param name="mzerr">
    ''' the mass tolerance error for extract the XIC data
    ''' </param>
    ''' <param name="mzdiff">
    ''' the mass window size for plot the density scatters
    ''' </param>
    Sub New(xic As IEnumerable(Of ms1_scan), mz As Double, mzerr As Tolerance,
            theme As Theme,
            Optional mzdiff As Double = 0.001)

        Call MyBase.New(theme)

        Dim pool As ms1_scan() = xic.ToArray

        Me.xic = loadXIC(pool, mz, mzerr).ToArray
        Me.mass_windows = pool _
            .GroupBy(Function(m) m.mz, offsets:=mzdiff) _
            .Select(Function(m)
                        Return New DoubleTagged(Of ms1_scan())(Val(m.name), m.value)
                    End Function) _
            .OrderBy(Function(m) m.Tag) _
            .ToArray
    End Sub

    Private Iterator Function loadXIC(pool As ms1_scan(), mz As Double, mzdiff As Tolerance) As IEnumerable(Of ChromatogramTick)
        Dim rt_ticks = pool _
            .AsParallel _
            .Where(Function(a) mzdiff(a.mz, mz)) _
            .OrderBy(Function(a) a.scan_time) _
            .GroupBy(Function(t) t.scan_time, offsets:=0.25) _
            .ToArray

        For Each scatter As NamedCollection(Of ms1_scan) In rt_ticks
            Yield New ChromatogramTick(
                time:=Val(scatter.name),
                into:=Aggregate ti In scatter.value Into Average(ti.intensity)
            )
        Next
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim rect As Rectangle = canvas.PlotRegion
        Dim part1 As New Rectangle(rect.Location, New Size(rect.Width, rect.Height / 2))
        Dim part2 As New Rectangle(New Point(rect.Left, rect.Top + part1.Height + 10), New Size(rect.Width, rect.Height / 2 - 10))
        Dim heatColors As String() = Designer.GetColors(theme.colorSet, 100).Select(Function(c) c.ToHtmlColor).ToArray
        Dim index As New DoubleRange(0, heatColors.Length - 1)
        Dim intensity As New DoubleRange(xic.Select(Function(ti) ti.Intensity))
        Dim xic_dat As New SerialData With {
            .color = Color.Blue,
            .lineType = DashStyle.Dot,
            .pointSize = theme.pointSize,
            .width = 2,
            .pts = xic _
                .Select(Function(ci) New PointF(ci.Time, ci.Intensity)) _
                .BSpline(RESOLUTION:=2) _
                .Select(Function(ti)
                            Dim i As Integer = intensity.ScaleMapping(ti.Y, index)

                            If i >= index.Max Then
                                i = index.Max - 1
                            End If
                            If i < 0 Then
                                i = 0
                            End If

                            Return New PointData(ti.X, ti.Y) With {
                                .color = heatColors(i)
                            }
                        End Function) _
                .ToArray,
            .shape = LegendStyles.Circle
        }
        Dim mass_scatter As New List(Of SerialData)
        Dim axisLine As Pen = g.LoadEnvironment.GetPen(Stroke.TryParse(theme.axisStroke))

        intensity = New DoubleRange(mass_windows.Select(Function(m) m.Value).IteratesALL.Select(Function(m) m.intensity))

        For Each mass As DoubleTagged(Of ms1_scan()) In mass_windows
            mass_scatter += New SerialData With {
                .pointSize = theme.pointSize,
                .lineType = DashStyle.Dot,
                .shape = LegendStyles.Circle,
                .width = 2,
                .pts = mass.Value _
                    .Select(Function(mi)
                                Dim i As Integer = intensity.ScaleMapping(mi.intensity, index)

                                If i >= index.Max Then
                                    i = index.Max - 1
                                End If
                                If i < 0 Then
                                    i = 0
                                End If

                                Return New PointData(mi.scan_time, mi.mz) With {
                                    .color = heatColors(i)
                                }
                            End Function) _
                    .ToArray
            }
        Next

        theme.xAxisLayout = Axis.XAxisLayoutStyles.None

        Call New Scatter2D({xic_dat}, theme) With {.xlabel = "Retention Time(s)", .ylabel = "Intensity"}.Plot(g, layout:=part1)
        Call g.DrawLine(axisLine, New PointF(part1.Left, part1.Bottom), New PointF(part1.Right, part1.Bottom))

        theme.xAxisLayout = Axis.XAxisLayoutStyles.Bottom

        Call New Scatter2D(mass_scatter, theme) With {.xlabel = "Retention Time(s)", .ylabel = "M/Z"}.Plot(g, layout:=part2)
    End Sub
End Class
