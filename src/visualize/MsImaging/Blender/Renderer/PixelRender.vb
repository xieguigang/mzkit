#Region "Microsoft.VisualBasic::45bd68b5bbe970b074f85ce80a6c3d9c, src\visualize\MsImaging\Imaging\PixelRender.vb"

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

'     Class PixelRender
' 
'         Function: ChannelCompositions, LayerOverlaps, (+2 Overloads) RenderPixels
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq

Namespace Blender

    Public Class PixelRender : Inherits Renderer

        Public Sub New(heatmapRender As Boolean)
            MyBase.New(heatmapRender)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="R"></param>
        ''' <param name="G"></param>
        ''' <param name="B"></param>
        ''' <param name="dimension"></param>
        ''' <param name="dimSize">
        ''' set this parameter to value nothing to returns
        ''' the raw image without any scale operation.
        ''' </param>
        ''' <param name="scale"></param>
        ''' <returns></returns>
        Public Overrides Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                      dimension As Size,
                                                      Optional dimSize As Size = Nothing,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                      Optional cut As (r As DoubleRange, g As DoubleRange, b As DoubleRange) = Nothing,
                                                      Optional background As String = "black") As GraphicsData

            Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)
            Dim defaultBackground As Color = background.TranslateColor

            Dim Rchannel = GetPixelChannelReader(R, cut.r)
            Dim Gchannel = GetPixelChannelReader(G, cut.g)
            Dim Bchannel = GetPixelChannelReader(B, cut.b)

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw, ImageLockMode.WriteOnly)
                For x As Integer = 1 To dimension.Width
                    For y As Integer = 1 To dimension.Height
                        Dim bR As Byte = Rchannel(x, y)
                        Dim bG As Byte = Gchannel(x, y)
                        Dim bB As Byte = Bchannel(x, y)
                        Dim color As Color

                        If bR = 0 AndAlso bG = 0 AndAlso bB = 0 Then
                            color = defaultBackground
                        Else
                            color = Color.FromArgb(bR, bG, bB)
                        End If

                        ' imzXML里面的坐标是从1开始的
                        ' 需要减一转换为.NET中从零开始的位置
                        Call buffer.SetPixel(x - 1, y - 1, color)
                    Next
                Next
            End Using

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                Return raw
            Else
                Return Drawer.ScaleLayer(raw, dimension, dimSize, scale)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="dimSize">pixel size</param>
        ''' <param name="colorSet"></param>
        ''' <param name="mapLevels"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <paramref name="dimSize"/> value set to nothing for returns the raw image
        ''' </remarks>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels% = 25,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As GraphicsData

            Dim brushColors As SolidBrush() = Designer _
                .GetColors(colorSet, mapLevels) _
                .Select(Function(c)
                            Return New SolidBrush(c)
                        End Function) _
                .ToArray

            Return RenderPixels(pixels, dimension, dimSize, brushColors, scale, defaultFill, cutoff)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="dimSize">pixel size</param>
        ''' <param name="colorSet"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <paramref name="dimSize"/> value set to nothing for returns the raw image
        ''' </remarks>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size, colorSet As SolidBrush(),
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As GraphicsData
            Dim color As Color
            Dim colors As Color() = colorSet.Select(Function(br) br.Color).ToArray
            Dim index As Integer
            Dim level As Double
            Dim indexrange As DoubleRange = New Double() {0, colors.Length - 1}
            Dim levelRange As DoubleRange = New Double() {0, 1}
            Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)
            Dim defaultColor As Color = defaultFill.TranslateColor

            Call raw.CreateCanvas2D(directAccess:=True).FillRectangle(Brushes.Transparent, New Rectangle(0, 0, raw.Width, raw.Height))

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw, ImageLockMode.WriteOnly)
                For Each point As PixelData In PixelData.ScalePixels(pixels, cutoff)
                    level = point.level

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
                    Call buffer.SetPixel(point.x - 1, point.y - 1, color)
                Next
            End Using

            If dimSize.Width = 0 OrElse dimSize.Height = 0 Then
                Return raw
            Else
                Return Drawer.ScaleLayer(raw, dimension, dimSize, scale)
            End If
        End Function

        Public Overrides Function LayerOverlaps(pixels As PixelData()(),
                                                dimension As Size,
                                                colorSet As MzLayerColorSet,
                                                Optional dimSize As Size = Nothing,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
