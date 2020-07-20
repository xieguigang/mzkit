Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Module UVsignalPlot

    Public Function Plot(signals As IEnumerable(Of GeneralSignal), legendTitle As Func(Of Dictionary(Of String, String), String),
                         Optional size As String = "1600,1200",
                         Optional padding As String = "padding:125px 50px 150px 200px;",
                         Optional colorSet As String = "Set1:c8",
                         Optional rtLine As Double? = Nothing,
                         Optional annotations As NamedValue(Of PointF)() = Nothing,
                         Optional pt_size As Single = 8,
                         Optional line_width As Single = 5,
                         Optional title$ = "UV absorption",
                         Optional xlabel$ = "time (sec)",
                         Optional ylabel$ = "intensity") As GraphicsData

        Dim colors As LoopArray(Of Color) = Designer.GetColors(colorSet)
        Dim data As SerialData() = signals _
            .Select(Function(line)
                        Return New SerialData With {
                            .color = colors.Next,
                            .lineType = DashStyle.Solid,
                            .pointSize = pt_size,
                            .shape = LegendStyles.Triangle,
                            .width = line_width,
                            .title = legendTitle(line.meta),
                            .pts = line _
                                .PopulatePoints _
                                .Select(Function(p)
                                            Return New PointData(p)
                                        End Function) _
                                .ToArray,
                            .DataAnnotations = annotations _
                                .SafeQuery _
                                .Select(Function(a)
                                            Return New Annotation With {
                                                .color = "blue",
                                                .Font = CSSFont.Win10NormalLarge,
                                                .Legend = LegendStyles.Pentacle,
                                                .size = New SizeF(200, 64),
                                                .Text = a.Name,
                                                .X = a.Value.X,
                                                .Y = a.Value.Y
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
        Dim ablines As Line() = {}

        If Not rtLine Is Nothing Then
            Dim min = data.Select(Function(a) a.pts.Select(Function(b) b.pt.Y).Min).Min
            Dim max = data.Select(Function(a) a.pts.Select(Function(b) b.pt.Y).Max).Max

            ablines = {
               New Line(New PointF(rtLine, min), New PointF(rtLine, max), New Pen(Color.Black, 3) With {.DashStyle = DashStyle.Dash})
            }
        End If

        Return Scatter.Plot(
            c:=data,
            size:=size,
            padding:=padding,
            Xlabel:=xlabel,
            Ylabel:=ylabel,
            XtickFormat:="F0",
            YtickFormat:="G2",
            title:=title,
            ablines:=ablines,
            titleFontCSS:=CSSFont.Win7LargeBold
        )
    End Function
End Module
