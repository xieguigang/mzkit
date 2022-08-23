Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

''' <summary>
''' 针对分子网络的cluster，取出每一个cluster中的成员的保留时间，在保留时间维度上，
''' 对每一个cluster的分布数量做直方图，绘制出直方图曲线的叠加图
''' </summary>
Public Class PlotMNHist : Inherits Plot

    ReadOnly g As NetworkGraph

    Public Sub New(g As NetworkGraph, theme As Theme)
        MyBase.New(theme)
        Me.g = g
    End Sub

    Private Function createClusterHistogram() As HistogramGroup

    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim hist As HistogramGroup = createClusterHistogram()

    End Sub
End Class
