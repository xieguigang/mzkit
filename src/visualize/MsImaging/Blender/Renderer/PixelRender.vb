#Region "Microsoft.VisualBasic::ca1af9ac0b4ac70e8750ffccdcefe428, visualize\MsImaging\Blender\Renderer\PixelRender.vb"

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

'   Total Lines: 234
'    Code Lines: 144 (61.54%)
' Comment Lines: 53 (22.65%)
'    - Xml Docs: 67.92%
' 
'   Blank Lines: 37 (15.81%)
'     File Size: 10.31 KB


'     Class PixelRender
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: (+2 Overloads) ChannelCompositions, DrawBackground, LayerOverlaps, (+2 Overloads) RenderPixels
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq

Namespace Blender

    ''' <summary>
    ''' Do ms-imaging rendering via pixel drawing
    ''' </summary>
    ''' <remarks>
    ''' this component is working on windows for the mzkit_win32 
    ''' desktop workbench program.
    ''' </remarks>
    Public Class PixelRender : Inherits Renderer

        ReadOnly overlaps As Image
        ReadOnly transparentCutoff As Integer = 5

        Public Sub New(heatmapRender As Boolean, Optional overlaps As Image = Nothing, Optional transparent As Integer = 5)
            MyBase.New(heatmapRender)

            Me.overlaps = overlaps
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
        ''' draw background
        ''' </summary>
        ''' <remarks>
        ''' <paramref name="dimension"/> defines the image size directly
        ''' </remarks>
        Private Function DrawBackground(dimension As Size, defaultBackground As Color) As Bitmap
            Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)

            Using g As IGraphics = DriverLoad.CreateGraphicsDevice(raw)
                Call g.Clear(defaultBackground)

                If Not overlaps Is Nothing Then
                    Call g.DrawImage(overlaps, New Rectangle(New Point, raw.Size))
                End If

                Call g.Flush()

#If NETCOREAPP Then
                Return New Bitmap(DirectCast(g, GdiRasterGraphics).ImageResource)
#End If
            End Using

            Return raw
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <returns></returns>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, heatmap As HeatMapParameters) As GraphicsData
            Return RenderPixels(pixels, dimension, heatmap.GetBrushes, heatmap.defaultFill.ToHtmlColor)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="colorSet"></param>
        ''' <returns>a gdi+ image obejct with size which is specified by
        ''' <paramref name="dimension"/> parameter.</returns>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, colorSet As SolidBrush(),
                                               Optional defaultFill As String = "Transparent") As GraphicsData
            Dim color As Color
            Dim colors As Color() = colorSet.Select(Function(br) br.Color).ToArray
            Dim index As Integer
            Dim level As Double
            Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
            Dim levelRange As DoubleRange = New Double() {0, 1}
            ' create a blank canvas image
            Dim raw As Bitmap = DrawBackground(dimension, Color.Transparent)
            Dim defaultColor As Color = defaultFill.TranslateColor
            Dim isTransparentFill As Boolean = defaultFill.TextEquals("Transparent")
            Dim skipTransparent As Boolean = (Not overlaps Is Nothing) OrElse isTransparentFill

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw)
                For Each point As PixelData In PixelData.ScalePixels(pixels)
                    level = point.level

                    If level <= 0.000005 OrElse level.IsNaNImaginary Then
                        color = defaultColor

                        ' skip draw on this pixel for missing pixel
                        If skipTransparent OrElse isTransparentFill Then
                            Continue For
                        End If
                    Else
                        index = levelRange.ScaleMapping(level, indexrange)

                        If index <= 0 Then
                            index = 0

                            If skipTransparent OrElse isTransparentFill Then
                                Continue For
                            End If
                        End If

                        color = colors(index)
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
