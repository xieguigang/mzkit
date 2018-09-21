Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.MassSpectrum.Math

''' <summary>
''' 横坐标为rt，纵坐标为m/z的散点图绘制
''' </summary>
Public Module MzrtPlot

    Public Function Plot(samples As IEnumerable(Of NamedValue(Of IMs1())),
                         Optional size$ = "3300,2700",
                         Optional bg$ = "white",
                         Optional margin$ = Resolution2K.PaddingWithTopTitleAndRightLegend) As GraphicsData

    End Function
End Module
