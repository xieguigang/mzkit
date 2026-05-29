#Region "Microsoft.VisualBasic::1d4d280c9b2a7f8780b81fb0d203a19f, visualize\MsImaging\Blender\Renderer\PixelRender.vb"

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

    '   Total Lines: 197
    '    Code Lines: 114 (57.87%)
    ' Comment Lines: 54 (27.41%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 29 (14.72%)
    '     File Size: 9.08 KB


    '     Class PixelRender
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) ChannelCompositions, LayerOverlaps, (+2 Overloads) RenderPixels
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Driver

Namespace Blender

    ''' <summary>
    ''' Do ms-imaging rendering via setpixel method in bitmap drawing
    ''' </summary>
    ''' <remarks>
    ''' this component is working on windows for the mzkit_win32 
    ''' desktop workbench program.
    ''' </remarks>
    Public Class PixelRender : Inherits Renderer

        ReadOnly transparentCutoff As Integer = 5

        ''' <summary>
        ''' Construct an ms-imaging data render via the bitmap buffer set pixel method
        ''' </summary>
        ''' <param name="heatmapRender"></param>
        ''' <param name="overlaps"></param>
        ''' <param name="transparent"></param>
        ''' <remarks>
        ''' only works for the gdi+ bitmap image
        ''' </remarks>
        Public Sub New(heatmapRender As Boolean, Optional overlaps As Image = Nothing, Optional transparent As Integer = 5)
            MyBase.New(heatmapRender, overlaps)

            Me.transparentCutoff = transparent
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="R"></param>
        ''' <param name="G"></param>
        ''' <param name="B"></param>
        ''' <param name="dimension">
        ''' the raw dimension size of the ms-imaging raw data file
        ''' </param>
        ''' <returns></returns>
        Public Overrides Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                      dimension As Size,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor
            ' rendering via raw dimesnion size
            Dim raw As Bitmap = DrawBackground(dimension, defaultBackground)
            Dim Rchannel = GetPixelChannelReader(R)
            Dim Gchannel = GetPixelChannelReader(G)
            Dim Bchannel = GetPixelChannelReader(B)
            Dim skipTransparent As Boolean = Not overlaps Is Nothing

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw)
                For x As Integer = 1 To dimension.Width
                    For y As Integer = 1 To dimension.Height
                        Dim bR As Byte = Rchannel(x, y)
                        Dim bG As Byte = Gchannel(x, y)
                        Dim bB As Byte = Bchannel(x, y)
                        Dim color As Color

                        If bR < transparentCutoff AndAlso bG < transparentCutoff AndAlso bB < transparentCutoff Then
                            color = defaultBackground

                            If skipTransparent Then
                                Continue For
                            End If
                        Else
                            color = Color.FromArgb(bR, bG, bB)
                        End If

                        ' imzXML里面的坐标是从1开始的
                        ' 需要减一转换为.NET中从零开始的位置
                        ' 但是经过scale之后，已经变换为0为底的结果了
                        Call buffer.SetPixel(x - 1, y - 1, color)
                    Next
                Next
            End Using

            ' no scale
            Return New ImageData(raw)
        End Function

        Public Overrides Function ChannelCompositions(Cdata() As PixelData, Mdata() As PixelData, Ydata() As PixelData, Kdata() As PixelData,
                                                      dimension As Size,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor
            ' rendering via raw dimesnion size
            Dim raw As Bitmap = DrawBackground(dimension, defaultBackground)
            Dim Cchannel = GetPixelChannelReader(Cdata)
            Dim Mchannel = GetPixelChannelReader(Mdata)
            Dim Ychannel = GetPixelChannelReader(Ydata)
            Dim Kchannel = GetPixelChannelReader(Kdata)
            Dim skipTransparent As Boolean = Not overlaps Is Nothing

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw)
                For x As Integer = 1 To dimension.Width
                    For y As Integer = 1 To dimension.Height
                        Dim bC As Byte = Cchannel(x, y)
                        Dim bM As Byte = Mchannel(x, y)
                        Dim bY As Byte = Ychannel(x, y)
                        Dim bK As Byte = Kchannel(x, y)
                        Dim color As Color

                        If bC < transparentCutoff AndAlso bM < transparentCutoff AndAlso bY < transparentCutoff AndAlso bK < transparentCutoff Then
                            color = defaultBackground

                            If skipTransparent Then
                                Continue For
                            End If
                        Else
                            color = New CMYKColor(bC, bM, bY, bK).ToRGB
                        End If

                        ' imzXML里面的坐标是从1开始的
                        ' 需要减一转换为.NET中从零开始的位置
                        ' 但是经过scale之后，已经变换为0为底的结果了
                        Call buffer.SetPixel(x - 1, y - 1, color)
                    Next
                Next
            End Using

            ' no scale
            Return New ImageData(raw)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <returns></returns>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, heatmap As HeatMapParameters) As GraphicsData
            Return RenderPixels(pixels, dimension, New HeatMapBrushes(heatmap))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <returns>a gdi+ image obejct with size which is specified by
        ''' <paramref name="dimension"/> parameter.</returns>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, heatmap As HeatMapBrushes) As GraphicsData
            Dim color As Color
            Dim colors As ValueScaleColorProfile = heatmap.GetMapping(pixels.Select(Function(c) c.intensity))
            Dim index As Integer
            Dim level As Double
            ' create a blank canvas image
            Dim defaultColor As Color = heatmap.defaultFill.TranslateColor
            Dim raw As Bitmap = DrawBackground(dimension, defaultColor)
            Dim isTransparentFill As Boolean = heatmap.defaultFill.TextEquals("Transparent")
            Dim skipTransparent As Boolean = (Not overlaps Is Nothing) OrElse isTransparentFill
            Dim intensityRange As Double() = colors.ValueMinMax

            colors = colors.ReScaleToValueRange(0, 1)

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw)
                For Each point As PixelData In PixelData.ScalePixels(pixels, setRange:=intensityRange)
                    level = point.level
                    color = colors.GetColor(level, index)

                    If level <= 0.000005 OrElse level <= 0 OrElse level.IsNaNImaginary Then
                        color = defaultColor

                        ' skip draw on this pixel for missing pixel
                        If skipTransparent OrElse isTransparentFill Then
                            Continue For
                        End If
                    End If

                    ' imzXML里面的坐标是从1开始的
                    ' 需要减一转换为.NET中从零开始的位置
                    ' 但是经过scale之后，已经变换为0为底的结果了
                    Call buffer.SetPixel(point.x - 1, point.y - 1, color)
                Next
            End Using

            Return New ImageData(raw)
        End Function

        Public Overrides Function LayerOverlaps(pixels As PixelData()(),
                                                dimension As Size,
                                                colorSet As MzLayerColorSet,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
