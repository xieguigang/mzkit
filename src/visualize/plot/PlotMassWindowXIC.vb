#Region "Microsoft.VisualBasic::bef756c153ddca76f66674917ed60775, visualize\plot\PlotMassWindowXIC.vb"

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

    '   Total Lines: 110
    '    Code Lines: 86
    ' Comment Lines: 12
    '   Blank Lines: 12
    '     File Size: 4.64 KB


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
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

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

    Sub New(xic As IEnumerable(Of ms1_scan), theme As Theme, Optional mzdiff As Double = 0.001)
        Call MyBase.New(theme)

        Dim pool As ms1_scan() = xic.ToArray

        Me.xic = loadXIC(pool).ToArray
        Me.mass_windows = pool _
            .GroupBy(Function(m) m.mz, offsets:=mzdiff) _
            .Select(Function(m)
                        Return New DoubleTagged(Of ms1_scan())(Val(m.name), m.value)
                    End Function) _
            .OrderBy(Function(m) m.Tag) _
            .ToArray
    End Sub

    Private Iterator Function loadXIC(pool As ms1_scan()) As IEnumerable(Of ChromatogramTick)
        Dim rt_ticks = pool _
            .OrderBy(Function(a) a.scan_time).GroupBy(Function(t) t.scan_time, offsets:=0.5) _
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
        Dim part2 As New Rectangle(New Point(rect.Left, rect.Top + part1.Height), New Size(rect.Width, rect.Height / 2))
        Dim heatColors As String() = Designer.GetColors("jet", 30).Select(Function(c) c.ToHtmlColor).ToArray
        Dim index As New DoubleRange(0, 30)
        Dim intensity As New DoubleRange(xic.Select(Function(ti) ti.Intensity))
        Dim xic_dat As New SerialData With {
            .color = Color.Blue,
            .lineType = DashStyle.Dot,
            .pointSize = theme.pointSize,
            .width = 2,
            .pts = xic _
                .Select(Function(ti)
                            Return New PointData(ti.Time, ti.Intensity) With {
                                .color = heatColors(CInt(intensity.ScaleMapping(ti.Intensity, index)))
                            }
                        End Function) _
                .ToArray,
            .shape = LegendStyles.Circle
        }
        Dim mass_scatter As New List(Of SerialData)

        intensity = New DoubleRange(mass_windows.Select(Function(m) m.Value).IteratesALL.Select(Function(m) m.intensity))

        For Each mass As DoubleTagged(Of ms1_scan()) In mass_windows
            mass_scatter += New SerialData With {
                .pointSize = theme.pointSize,
                .lineType = DashStyle.Dot,
                .shape = LegendStyles.Circle,
                .width = 2,
                .pts = mass.Value _
                    .Select(Function(mi)
                                Return New PointData(mi.scan_time, mi.mz) With {
                                    .color = heatColors(CInt(intensity.ScaleMapping(mi.intensity, index)))
                                }
                            End Function) _
                    .ToArray
            }
        Next

        Call New Scatter2D({xic_dat}, theme).Plot(g, layout:=part1)
        Call New Scatter2D(mass_scatter, theme).Plot(g, layout:=part2)
    End Sub
End Class

