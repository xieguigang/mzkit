#Region "Microsoft.VisualBasic::dcf20c9153a0857496bbead1d8b69425, src\visualize\plot\UVsignalPlot.vb"

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

    ' Module UVsignalPlot
    ' 
    '     Function: Plot
    ' 
    ' /********************************************************************************/

#End Region

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
Imports Microsoft.VisualBasic.MIME.Html.CSS

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
                         Optional ylabel$ = "intensity",
                         Optional showLegend As Boolean = True,
                         Optional showGrid As Boolean = True,
                         Optional gridFill$ = "rgb(245,245,245)") As GraphicsData

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
            titleFontCSS:=CSSFont.Win7LargeBold,
            showLegend:=showLegend,
            showGrid:=showGrid,
            gridFill:=gridFill
        )
    End Function
End Module
