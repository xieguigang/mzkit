#Region "Microsoft.VisualBasic::1bc4d5ad34062b66064f9324eaa58bd7, E:/mzkit/src/visualize/MsImaging//Blender/Renderer/RectangleRender.vb"

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


    ' Code Statistics:

    '   Total Lines: 212
    '    Code Lines: 166
    ' Comment Lines: 16
    '   Blank Lines: 30
    '     File Size: 9.98 KB


    '     Class RectangleRender
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ChannelCompositions, (+2 Overloads) LayerOverlaps, (+2 Overloads) RenderPixels
    ' 
    '         Sub: ChannelCompositions, FillLayerInternal, RenderPixels
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

        <DebuggerStepThrough>
        Public Sub New(driver As Drivers, heatmapRender As Boolean)
            MyBase.New(heatmapRender)
            Me.driver = driver
        End Sub

        Public Overloads Sub ChannelCompositions(gr As IGraphics, region As GraphicsRegion,
                                                 R() As PixelData, G() As PixelData, B() As PixelData,
                                                 dimension As Size,
                                                 Optional background As String = "black")

            Dim defaultBackground As Color = background.TranslateColor
            Dim rgb As New RenderRGB(defaultBackground, heatmapMode) With {
                .Bchannel = GetPixelChannelReader(B),
                .Rchannel = GetPixelChannelReader(R),
                .Gchannel = GetPixelChannelReader(G),
                .dimension = dimension
            }

            Call rgb.Render(gr, region)
        End Sub

        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Size,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor
            Dim Rchannel = GetPixelChannelReader(R)
            Dim Gchannel = GetPixelChannelReader(G)
            Dim Bchannel = GetPixelChannelReader(B)
            Dim w As Integer = dimension.Width
            Dim h As Integer = dimension.Height
            Dim rgb As New RenderRGB(defaultBackground, heatmapMode) With {
                .Bchannel = Bchannel,
                .dimension = dimension,
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

        Public Overloads Sub RenderPixels(g As IGraphics, offset As Point, pixels() As PixelData, colorSet() As SolidBrush)
            Call FillLayerInternal(g, pixels, colorSet.First, colorSet, offset)
        End Sub

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, colorSet() As SolidBrush,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent") As GraphicsData

            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim w = dimension.Width
            Dim h = dimension.Height

            Return g.GraphicsPlots(
                size:=New Size(w, h),
                padding:=New Padding,
                bg:=defaultFill,
                driver:=driver,
                plotAPI:=Sub(ByRef g, region)
                             Call FillLayerInternal(g, pixels, defaultColor, colorSet, Nothing)
                         End Sub)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">
        ''' the ms-imaging canvas size
        ''' </param>
        ''' <param name="colorSet"></param>
        ''' <param name="mapLevels"></param>
        ''' <param name="scale"></param>
        ''' <param name="defaultFill">
        ''' the background of the MS-imaging chartting.
        ''' </param>
        ''' <returns></returns>
        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels As Integer = 25,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent") As GraphicsData

            Dim colors As SolidBrush() = Designer.GetColors(colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray

            Return RenderPixels(pixels, dimension, colors, scale, defaultFill)
        End Function

        Private Sub FillLayerInternal(gr As IGraphics,
                                      pixels() As PixelData,
                                      defaultColor As SolidBrush,
                                      colors As SolidBrush(),
                                      Offset As Point)
            Dim color As SolidBrush
            Dim index As Integer
            Dim levelRange As DoubleRange = New Double() {0, 1}
            Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
            Dim dimSize As New SizeF(1, 1)

            For Each point As PixelData In PixelData.ScalePixels(pixels)
                Dim level As Double = point.level
                Dim pos As New PointF With {
                    .X = (point.x - 1) + Offset.X,
                    .Y = (point.y - 1) + Offset.Y
                }
                Dim rect As New RectangleF(pos, dimSize)

                If level <= 0.0 Then
                    color = defaultColor
                Else
                    index = levelRange.ScaleMapping(level, indexrange)

                    If index <= 0 Then
                        color = defaultColor
                    Else
                        color = colors(index)
                    End If
                End If

                ' imzXML里面的坐标是从1开始的
                ' 需要减一转换为.NET中从零开始的位置
                Call gr.FillRectangle(color, rect)
            Next
        End Sub

        Public Overrides Function LayerOverlaps(layers()() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim i As i32 = Scan0
            Dim w = dimension.Width
            Dim h = dimension.Height

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

                                 Call FillLayerInternal(g, layer, defaultColor, colors, Nothing)
                             Next
                         End Sub)
        End Function

        Public Overloads Function LayerOverlaps(pixels() As PixelData, dimension As Size, colorSet As MzLayerColorSet,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Dim layers = colorSet.SelectGroup(pixels).ToArray
            Dim defaultColor As SolidBrush = defaultFill.GetBrush
            Dim w = dimension.Width
            Dim h = dimension.Height

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

                                 Call FillLayerInternal(g, layer.value, defaultColor, colors, Nothing)
                             Next
                         End Sub)
        End Function
    End Class
End Namespace
