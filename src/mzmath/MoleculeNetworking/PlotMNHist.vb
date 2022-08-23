Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Distributions.BinBox

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

    Private Iterator Function createClusterHistogram() As IEnumerable(Of SerialData)
        Dim clusters = g.vertex _
            .GroupBy(Function(v)
                         Return v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function) _
            .ToArray
        Dim colors As Color() = Designer.GetColors(theme.colorSet, clusters.Length)

        For i As Integer = 0 To clusters.Length - 1
            Yield getLineData(clusters(i), colors(i))
        Next
    End Function

    Private Function getLineData(cluster As IGrouping(Of String, Node), color As Color) As SerialData
        Dim rt As Double() = cluster _
            .Select(Function(v) Val(v.data("rt"))) _
            .ToArray
        Dim range As New DoubleRange(rt)
        Dim hist As DataBinBox(Of Double)() = rt _
            .Hist((range.Max - range.Min) / 30) _
            .ToArray
        Dim data As PointData() = hist _
            .Select(Function(bin)
                        Return New PointData With {
                            .pt = New PointF(If(bin.Count = 0, 0.0, bin.Raw.Average), bin.Count)
                        }
                    End Function) _
            .ToArray

        Return New SerialData With {
            .color = color,
            .lineType = DashStyle.Solid,
            .pointSize = theme.pointSize,
            .pts = data,
            .shape = LegendStyles.Diamond,
            .title = cluster.Key
        }
    End Function

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim hist As SerialData() = createClusterHistogram().ToArray
        Dim transform As New LinePlot2D(hist, theme, fill:=False)

        Call transform.Plot(g, canvas)
    End Sub
End Class
