#Region "Microsoft.VisualBasic::600482317d0216098af090b975713fe5, visualize\plot\PlotMassWindowXIC.vb"

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

    '   Total Lines: 202
    '    Code Lines: 155 (76.73%)
    ' Comment Lines: 26 (12.87%)
    '    - Xml Docs: 92.31%
    ' 
    '   Blank Lines: 21 (10.40%)
    '     File Size: 8.29 KB


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

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

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
    ReadOnly rtmin As Double
    ReadOnly rtmax As Double

    ''' <summary>
    ''' Construct a plot that combined of XIC with the scatter density heatmap
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
        Dim rt = pool.Select(Function(a) a.scan_time).ToArray

        If rt.Length > 0 Then
            Me.rtmin = rt.Min
            Me.rtmax = rt.Max
        Else
            Me.rtmin = 0
            Me.rtmax = 1
        End If

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

    Private Iterator Function SplineLine(intensity As DoubleRange, index As DoubleRange, heatColors As String()) As IEnumerable(Of PointData)
        Dim spline As IEnumerable(Of PointF) = xic _
            .Select(Function(ci) New PointF(ci.Time, ci.Intensity)) _
            .BSpline(RESOLUTION:=2) _
            .ToArray

        Yield New PointData(rtmin, 0) With {.color = heatColors(0)}

        For Each ti As PointF In spline
            Dim i As Integer = intensity.ScaleMapping(ti.Y, index)

            If i >= index.Max Then
                i = index.Max - 1
            End If
            If i < 0 Then
                i = 0
            End If

            Yield New PointData(ti.X, ti.Y) With {
                .color = heatColors(i)
            }
        Next

        Yield New PointData(rtmax, 0) With {.color = heatColors(0)}
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim rect As Rectangle = canvas.PlotRegion(css)
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
            .pts = SplineLine(intensity, index, heatColors).ToArray,
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

        If Not xic_dat.pts.IsNullOrEmpty Then
            Call New Scatter2D({xic_dat}, theme) With {
                .xlabel = "Retention Time(s)",
                .ylabel = "Intensity"
            }.Plot(g, layout:=part1)
        Else
            Call "No Xic data points!".Warning
        End If

        Call g.DrawLine(axisLine, New PointF(part1.Left, part1.Bottom), New PointF(part1.Right, part1.Bottom))

        theme.xAxisLayout = Axis.XAxisLayoutStyles.Bottom

        If mass_scatter.Select(Function(s) s.pts).IteratesALL.Any Then
            Call New Scatter2D(mass_scatter, theme) With {
                .xlabel = "Retention Time(s)",
                .ylabel = "M/Z"
            }.Plot(g, layout:=part2)
        Else
            Call "No mass trace scatter points!".Warning
        End If
    End Sub
End Class
