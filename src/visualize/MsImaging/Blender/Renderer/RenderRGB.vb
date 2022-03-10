Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Blender

    Public Class RenderRGB

        ReadOnly defaultBackground As Color
        ReadOnly heatmapMode As Boolean

        Public Property dimension As Size
        Public Property dimSize As Size

        Public Property Rchannel As Func(Of Integer, Integer, Byte)
        Public Property Gchannel As Func(Of Integer, Integer, Byte)
        Public Property Bchannel As Func(Of Integer, Integer, Byte)

        Sub New(defaultBackground As Color, Optional heatmapMode As Boolean = False)
            Me.heatmapMode = heatmapMode
            Me.defaultBackground = defaultBackground
        End Sub

        Public Sub Render(ByRef gr As IGraphics, region As GraphicsRegion)
            Dim plotOffset As Point = region.PlotRegion.Location
            Dim pos As Point
            Dim rect As Rectangle

            For x As Integer = 1 To dimension.Width
                For y As Integer = 1 To dimension.Height
                    Dim bR As Byte = Rchannel(x, y)
                    Dim bG As Byte = Gchannel(x, y)
                    Dim bB As Byte = Bchannel(x, y)
                    Dim color As Color

                    pos = New Point With {
                        .X = (x - 1) * dimSize.Width + plotOffset.X,
                        .Y = (y - 1) * dimSize.Height + plotOffset.Y
                    }
                    rect = New Rectangle(pos, dimSize)

                    If bR = 0 AndAlso bG = 0 AndAlso bB = 0 Then
                        ' missing a pixel at here?
                        If heatmapMode Then
                            bR = CByte(New Integer() {
                                Rchannel(x - 1, y - 1), Rchannel(x, y - 1), Rchannel(x + 1, y - 1),
                                Rchannel(x - 1, y), Rchannel(x, y), Rchannel(x + 1, y),
                                Rchannel(x - 1, y + 1), Rchannel(x, y + 1), Rchannel(x + 1, y + 1)
                            }.Average)
                            bB = CByte(New Integer() {
                                Bchannel(x - 1, y - 1), Bchannel(x, y - 1), Bchannel(x + 1, y - 1),
                                Bchannel(x - 1, y), Bchannel(x, y), Bchannel(x + 1, y),
                                Bchannel(x - 1, y + 1), Bchannel(x, y + 1), Bchannel(x + 1, y + 1)
                            }.Average)
                            bG = CByte(New Integer() {
                                Gchannel(x - 1, y - 1), Gchannel(x, y - 1), Gchannel(x + 1, y - 1),
                                Gchannel(x - 1, y), Gchannel(x, y), Gchannel(x + 1, y),
                                Gchannel(x - 1, y + 1), Gchannel(x, y + 1), Gchannel(x + 1, y + 1)
                            }.Average)

                            color = Color.FromArgb(bR, bG, bB)
                        Else
                            color = defaultBackground
                        End If
                    Else
                        color = Color.FromArgb(bR, bG, bB)
                    End If

                    ' imzXML里面的坐标是从1开始的
                    ' 需要减一转换为.NET中从零开始的位置
                    Call gr.FillRectangle(New SolidBrush(color), rect)
                Next
            Next
        End Sub
    End Class
End Namespace