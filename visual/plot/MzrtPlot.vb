#Region "Microsoft.VisualBasic::979a0ce6170b4b30e6041707237eb636, plot\MzrtPlot.vb"

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

    ' Module MzrtPlot
    ' 
    '     Function: Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.MassSpectrum.Math

''' <summary>
''' 横坐标为rt，纵坐标为m/z的散点图绘制
''' </summary>
Public Module MzrtPlot

    ''' <summary>
    ''' The scatter plots of the samples ``m/z`` and ``rt``.
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <param name="size$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="margin$"></param>
    ''' <param name="ptSize!"></param>
    ''' <returns></returns>
    Public Function Plot(samples As IEnumerable(Of NamedValue(Of IMs1())),
                         Optional size$ = "8000,3000",
                         Optional bg$ = "white",
                         Optional margin$ = Resolution2K.PaddingWithTopTitleAndRightLegend,
                         Optional ptSize! = 8,
                         Optional sampleColors$ = "Set1:c8") As GraphicsData

        ' 先转换为散点图的数据系列
        Dim colors = Designer.GetColors(sampleColors).AsLoop
        Dim serials = samples _
            .Select(Function(sample)
                        Dim points = sample.Value _
                            .Select(Function(compound)
                                        Return New PointData() With {
                                            .pt = New PointF(compound.rt, compound.mz)
                                        }
                                    End Function) _
                            .ToArray
                        Return New SerialData With {
                            .title = sample.Name,
                            .pts = points,
                            .PointSize = ptSize,
                            .color = colors.Next
                        }
                    End Function) _
            .ToArray

        Return Scatter.Plot(
            serials,
            size:=size, padding:=margin, bg:=bg,
            showGrid:=True,
            drawLine:=False,
            Xlabel:="rt in seconds",
            Ylabel:="m/z",
            htmlLabel:=False
        )
    End Function
End Module
