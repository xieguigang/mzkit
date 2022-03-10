#Region "Microsoft.VisualBasic::3011348f9de04107109dbdd3ec43dcdb, src\visualize\MsImaging\Imaging\RectangleRender.vb"

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

'     Enum RenderingModes
' 
'         LayerOverlaps, MixIons, RGBComposition
' 
'  
' 
' 
' 
'     Class RectangleRender
' 
'         Function: ChannelCompositions, (+2 Overloads) LayerOverlaps, (+2 Overloads) RenderPixels
' 
'         Sub: FillLayerInternal
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace Blender

    Public Class RectangleRender : Inherits Renderer

        ReadOnly driver As Drivers

        Public Sub New(driver As Drivers, heatmapRender As Boolean)
            MyBase.New(heatmapRender)

            Me.driver = driver
        End Sub

        Public Overloads Sub ChannelCompositions(gr As IGraphics, region As GraphicsRegion,
                                                 R() As PixelData, G() As PixelData, B() As PixelData,
                                                 dimension As Size,
                                                 Optional dimSize As SizeF = Nothing,
                                                 Optional cut As (r As DoubleRange, g As DoubleRange, b As DoubleRange) = Nothing,
                                                 Optional background As String = "black")

            Dim defaultBackground As Color = background.TranslateColor

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim rgb As New RenderRGB(defaultBackground, heatmapMode) With {
                .Bchannel = GetPixelChannelReader(B, cut.b),
                .Rchannel = GetPixelChannelReader(R, cut.r),
                .Gchannel = GetPixelChannelReader(G, cut.g),
                .dimension = dimension,
                .dimSize = dimSize
            }

            Call rgb.Render(gr, region)
        End Sub

        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Size,
                                                      Optional dimSize As Size = Nothing,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                      Optional cut As (r As DoubleRange, g As DoubleRange, b As DoubleRange) = Nothing,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim Rchannel = GetPixelChannelReader(R, cut.r)
            Dim Gchannel = GetPixelChannelReader(G, cut.g)
            Dim Bchannel = GetPixelChannelReader(B, cut.b)
            Dim w As Integer = dimension.Width * dimSize.Width
            Dim h As Integer = dimension.Height * dimSize.Height
            Dim rgb As New RenderRGB(defaultBackground, heatmapMode) With {
                .Bchannel = Bchannel,
                .dimension = dimension,
                .dimSize = dimSize,
                .Gchannel = Gchannel,
                .Rchannel = Rchannel
            }

            Return Drawing2D.g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=NameOf(Color.Transparent),
                driver:=driver,
                plotAPI:=AddressOf rgb.Render
            )
        End Function

        Public Overloads Sub RenderPixels(g As IGraphics, offset As Point, pixels() As PixelData, dimSize As Size, colorSet() As SolidBrush,
                                          Optional cutoff As DoubleRange = Nothing)

            Call FillLayerInternal(g, pixels, colorSet.First, colorSet, cutoff, dimSize, offset)
        End Sub

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, dimSize As Size, colorSet() As SolidBrush,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As GraphicsData

            Dim defaultColor As SolidBrush = defaultFill.GetBrush

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim w = dimension.Width * dimSize.Width
            Dim h = dimension.Height * dimSize.Height

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=defaultFill,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             Call FillLayerInternal(g, pixels, defaultColor, colorSet, cutoff, dimSize, Nothing)
                         End Sub)
        End Function

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, dimSize As Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels As Integer = 25,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As GraphicsData

            Dim colors As SolidBrush() = Designer.GetColors(colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray

            Return RenderPixels(pixels, dimension, dimSize, colors, scale, defaultFill, cutoff)
        End Function

        Private Shared Sub FillLayerInternal(gr As IGraphics,
                                             pixels() As PixelData,
                                             defaultColor As SolidBrush,
                                             colors As SolidBrush(),
                                             cutoff As DoubleRange,
                                             dimSize As Size,
                                             offset As Point)
            Dim color As SolidBrush
            Dim index As Integer
            Dim levelRange As DoubleRange = New Double() {0, 1}
            Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}

            For Each point As PixelData In PixelData.ScalePixels(pixels, cutoff)
                Dim level As Double = point.level
                Dim pos As New Point With {
                    .X = (point.x - 1) * dimSize.Width + offset.X,
                    .Y = (point.y - 1) * dimSize.Height + offset.Y
                }
                Dim rect As New Rectangle(pos, dimSize)

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
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim i As i32 = Scan0

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim w = dimension.Width * dimSize.Width
            Dim h = dimension.Height * dimSize.Height

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=defaultColor.Color.ToHtmlColor,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             For Each layer As PixelData() In layers
                                 Dim baseColor As Color = colorSet(++i)
                                 Dim colors As SolidBrush() = seq(50, 255, (255 - 30) / mapLevels) _
                                     .Select(Function(a) New SolidBrush(baseColor.Alpha(a))) _
                                     .ToArray

                                 Call FillLayerInternal(g, layer, defaultColor, colors, cut, dimSize, Nothing)
                             Next
                         End Sub)
        End Function

        Public Overloads Function LayerOverlaps(pixels() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional dimSize As Size = Nothing,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Dim layers = colorSet.SelectGroup(pixels).ToArray
            Dim defaultColor As SolidBrush = defaultFill.GetBrush

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                dimSize = New Size(1, 1)
            End If

            Dim w = dimension.Width * dimSize.Width
            Dim h = dimension.Height * dimSize.Height

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=defaultColor.Color.ToHtmlColor,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             For Each layer As NamedCollection(Of PixelData) In layers
                                 Dim baseColor As Color = colorSet.FindColor(Val(layer.name))
                                 Dim colors As SolidBrush() = seq(0, 255, 255 / mapLevels) _
                                      .Select(Function(a)
                                                  Return New SolidBrush(baseColor.Alpha(a))
                                              End Function) _
                                      .ToArray

                                 Call FillLayerInternal(g, layer.value, defaultColor, colors, cut, dimSize, Nothing)
                             Next
                         End Sub)
        End Function
    End Class
End Namespace
