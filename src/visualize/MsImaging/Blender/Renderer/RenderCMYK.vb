Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

Namespace Blender

    Public Class RenderCMYK : Inherits CompositionBlender

        ''' <summary>
        ''' Cyan
        ''' </summary>
        ''' <returns></returns>
        Public Property Cchannel As Func(Of Integer, Integer, Byte)
        ''' <summary>
        ''' Magenta
        ''' </summary>
        ''' <returns></returns>
        Public Property Mchannel As Func(Of Integer, Integer, Byte)
        ''' <summary>
        ''' Yellow
        ''' </summary>
        ''' <returns></returns>
        Public Property Ychannel As Func(Of Integer, Integer, Byte)
        ''' <summary>
        ''' Key/Black
        ''' </summary>
        ''' <returns></returns>
        Public Property Kchannel As Func(Of Integer, Integer, Byte)

        Public Sub New(defaultBackground As Color, Optional heatmapMode As Boolean = False)
            MyBase.New(defaultBackground, heatmapMode)
        End Sub

        Public Overrides Sub Render(ByRef g As IGraphics, region As GraphicsRegion)
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim plotOffset As Point = region.PlotRegion(css).Location
            Dim pos As PointF
            Dim rect As RectangleF
            Dim pixel_size As New SizeF(1, 1)

            For x As Integer = 1 To dimension.Width
                For y As Integer = 1 To dimension.Height
                    Dim bC As Byte = Cchannel(x, y)
                    Dim bM As Byte = Mchannel(x, y)
                    Dim bY As Byte = Ychannel(x, y)
                    Dim bK As Byte = Kchannel(x, y)
                    Dim color As Color

                    pos = New PointF With {
                        .X = (x - 1) + plotOffset.X,
                        .Y = (y - 1) + plotOffset.Y
                    }
                    rect = New RectangleF(pos, pixel_size)

                    If bC = 0 AndAlso bM = 0 AndAlso bY = 0 AndAlso bK = 0 Then
                        ' missing a pixel at here?
                        If heatmapMode Then
                            bC = HeatmapBlending(Cchannel, x, y)
                            bM = HeatmapBlending(Mchannel, x, y)
                            bY = HeatmapBlending(Ychannel, x, y)
                            bK = HeatmapBlending(Kchannel, x, y)

                            color = New CMYKColor(bC, bM, bY, bK).ToRGB
                        Else
                            color = defaultBackground
                        End If
                    Else
                        color = New CMYKColor(bC, bM, bY, bK).ToRGB
                    End If

                    ' imzXML里面的坐标是从1开始的
                    ' 需要减一转换为.NET中从零开始的位置
                    Call g.FillRectangle(New SolidBrush(color), rect)
                Next
            Next
        End Sub
    End Class
End Namespace