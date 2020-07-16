Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module UVsignalPlot

    Public Function Plot(signals As IEnumerable(Of GeneralSignal),
                         Optional size As String = "1600,1200",
                         Optional padding As String = "padding:125px 50px 150px 200px;",
                         Optional colorSet As String = "Set1:c8",
                         Optional pt_size As Single = 8,
                         Optional line_width As Single = 5) As GraphicsData

        Dim colors As LoopArray(Of Color) = Designer.GetColors(colorSet)
        Dim data As SerialData() = signals _
            .Select(Function(line)
                        Return New SerialData With {
                            .color = colors.Next,
                            .lineType = DashStyle.Solid,
                            .pointSize = pt_size,
                            .shape = LegendStyles.Triangle,
                            .width = line_width,
                            .title = line.meta!wavelength & "nm",
                            .pts = line _
                                .PopulatePoints _
                                .Select(Function(p)
                                            Return New PointData(p)
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return Scatter.Plot(
            c:=data,
            size:=size,
            padding:=padding,
            Xlabel:="time(sec)",
            Ylabel:="intensity",
            XtickFormat:="F0",
            YtickFormat:="G2",
            title:="UV absorption"
        )
    End Function
End Module
