Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Namespace Imaging

    Public Class RectangleRender : Inherits Renderer

        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Size,
                                                      Optional dimSize As Size = Nothing,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear) As Bitmap

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim raw As New Bitmap(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height, PixelFormat.Format32bppArgb)
            Dim Rchannel = GetPixelChannelReader(R)
            Dim Gchannel = GetPixelChannelReader(G)
            Dim Bchannel = GetPixelChannelReader(B)

            Using gr As Graphics2D = New Size(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height).CreateGDIDevice
                For x As Integer = 1 To dimension.Width
                    For y As Integer = 1 To dimension.Height
                        Dim bR As Byte = Rchannel(x, y)
                        Dim bG As Byte = Gchannel(x, y)
                        Dim bB As Byte = Bchannel(x, y)
                        Dim color As Color = Color.FromArgb(bR, bG, bB)
                        Dim rect As New Rectangle(New Point(x - 1, y - 1), dimSize)

                        ' imzXML里面的坐标是从1开始的
                        ' 需要减一转换为.NET中从零开始的位置
                        Call gr.FillRectangle(New SolidBrush(color), rect)
                    Next
                Next

                Return gr.ImageResource
            End Using
        End Function

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, dimSize As Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels As Integer = 25,
                                               Optional logE As Boolean = False,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As Bitmap
            Dim color As SolidBrush
            Dim colors As SolidBrush() = Designer.GetColors(colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim index As Integer
            Dim level As Double
            Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
            Dim levelRange As DoubleRange = New Double() {0, 1}
            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim rect As Rectangle

            Using gr As Graphics2D = New Size(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height).CreateGDIDevice
                For Each point As PixelData In PixelData.ScalePixels(pixels, cutoff, logE)
                    level = point.level
                    rect = New Rectangle(New Point(point.x - 1, point.y - 1), dimSize)

                    If level <= 0.0 Then
                        color = defaultColor
                    Else
                        index = levelRange.ScaleMapping(level, indexrange)

                        If index < 0 Then
                            index = 0
                        End If

                        color = colors(index)
                    End If

                    ' imzXML里面的坐标是从1开始的
                    ' 需要减一转换为.NET中从零开始的位置
                    Call gr.FillRectangle(color, rect)
                Next

                Return gr.ImageResource
            End Using
        End Function
    End Class
End Namespace