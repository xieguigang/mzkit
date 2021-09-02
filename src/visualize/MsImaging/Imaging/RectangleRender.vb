Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.SVG.XML
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Imaging

    Public Enum RenderingModes

        RGBComposition
        MixIons
        LayerOverlaps

    End Enum

    Public Class RectangleRender : Inherits Renderer

        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Size,
                                                      Optional dimSize As Size = Nothing,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                      Optional cut As DoubleRange = Nothing) As Bitmap

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim Rchannel = GetPixelChannelReader(R, cut)
            Dim Gchannel = GetPixelChannelReader(G, cut)
            Dim Bchannel = GetPixelChannelReader(B, cut)

            Using gr As Graphics2D = New Size(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height).CreateGDIDevice(filled:=Color.Transparent)
                For x As Integer = 1 To dimension.Width
                    For y As Integer = 1 To dimension.Height
                        Dim bR As Byte = Rchannel(x, y)
                        Dim bG As Byte = Gchannel(x, y)
                        Dim bB As Byte = Bchannel(x, y)
                        Dim color As Color = Color.FromArgb(bR, bG, bB)
                        Dim rect As New Rectangle(New Point((x - 1) * dimSize.Width, (y - 1) * dimSize.Height), dimSize)

                        ' imzXML里面的坐标是从1开始的
                        ' 需要减一转换为.NET中从零开始的位置
                        Call gr.FillRectangle(New SolidBrush(color), rect)
                    Next
                Next

                Return gr.ImageResource
            End Using
        End Function

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, dimSize As Size, colorSet() As SolidBrush,
                                               Optional logE As Boolean = False,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As Bitmap

            Dim defaultColor As SolidBrush = defaultFill.GetBrush

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Using gr As Graphics2D = New Size(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height).CreateGDIDevice(defaultColor.Color)
                Call FillLayerInternal(gr, pixels, defaultColor, colorSet, cutoff, logE, dimSize)
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

            Dim colors As SolidBrush() = Designer.GetColors(colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray

            Return RenderPixels(pixels, dimension, dimSize, colors, logE, scale, defaultFill, cutoff)
        End Function

        Private Shared Sub FillLayerInternal(gr As Graphics2D, pixels() As PixelData, defaultColor As SolidBrush, colors As SolidBrush(), cutoff As DoubleRange, logE As Boolean, dimSize As Size)
            Dim color As SolidBrush
            Dim index As Integer
            Dim levelRange As DoubleRange = New Double() {0, 1}
            Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}

            For Each point As PixelData In PixelData.ScalePixels(pixels, cutoff, logE)
                Dim level As Double = point.level
                Dim rect As New Rectangle(New Point((point.x - 1) * dimSize.Width, (point.y - 1) * dimSize.Height), dimSize)

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
        End Sub

        Public Overrides Function LayerOverlaps(layers()() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional dimSize As Size = Nothing,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As Bitmap

            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim i As i32 = Scan0

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Using gr As Graphics2D = New Size(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height).CreateGDIDevice(defaultColor.Color)
                For Each layer As PixelData() In layers
                    Dim baseColor As Color = colorSet(++i)
                    Dim colors As SolidBrush() = seq(50, 255, (255 - 30) / mapLevels).Select(Function(a) New SolidBrush(baseColor.Alpha(a))).ToArray

                    Call FillLayerInternal(gr, layer, defaultColor, colors, cut, False, dimSize)
                Next

                Return gr.ImageResource
            End Using
        End Function

        Public Overloads Function LayerOverlaps(pixels() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional dimSize As Size = Nothing,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As Bitmap

            Dim layers = colorSet.SelectGroup(pixels).ToArray
            Dim defaultColor As SolidBrush = defaultFill.GetBrush

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Using gr As Graphics2D = New Size(dimension.Width * dimSize.Width, dimension.Height * dimSize.Height).CreateGDIDevice(defaultColor.Color)
                For Each layer As NamedCollection(Of PixelData) In layers
                    Dim baseColor As Color = colorSet.FindColor(Val(layer.name))
                    Dim colors As SolidBrush() = seq(0, 255, 255 / mapLevels).Select(Function(a) New SolidBrush(baseColor.Alpha(a))).ToArray

                    Call FillLayerInternal(gr, layer.value, defaultColor, colors, cut, False, dimSize)
                Next

                Return gr.ImageResource
            End Using
        End Function
    End Class
End Namespace