#Region "Microsoft.VisualBasic::64cec1de180d95b2287c2a2348d36c2c, visualize\MsImaging\Blender\Renderer\PixelRender.vb"

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

    '   Total Lines: 192
    '    Code Lines: 117
    ' Comment Lines: 45
    '   Blank Lines: 30
    '     File Size: 8.44 KB


    '     Class PixelRender
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ChannelCompositions, DrawBackground, LayerOverlaps, (+2 Overloads) RenderPixels
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
        ''' <param name="scale"></param>
        ''' <returns></returns>
        Public Overrides Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                      dimension As Size,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                      Optional background As String = "black") As GraphicsData

            Dim defaultBackground As Color = background.TranslateColor
            ' rendering via raw dimesnion size
            Dim raw As Bitmap = DrawBackground(dimension, defaultBackground)
            Dim Rchannel = GetPixelChannelReader(R)
            Dim Gchannel = GetPixelChannelReader(G)
            Dim Bchannel = GetPixelChannelReader(B)
            Dim skipTransparent As Boolean = Not overlaps Is Nothing

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw, ImageLockMode.WriteOnly)
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

        ''' <summary>
        ''' draw background
        ''' </summary>
        Private Function DrawBackground(dimension As Size, defaultBackground As Color) As Bitmap
            Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(raw)
                Call g.Clear(defaultBackground)

                If Not overlaps Is Nothing Then
                    Call g.DrawImage(overlaps, New Rectangle(New Point, raw.Size))
                End If
            End Using

            Return raw
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="colorSet"></param>
        ''' <param name="mapLevels"></param>
        ''' <returns></returns>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels% = 25,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent") As GraphicsData

            Dim brushColors As SolidBrush() = Designer _
                .GetColors(colorSet, mapLevels) _
                .Select(Function(c)
                            Return New SolidBrush(c)
                        End Function) _
                .ToArray

            Return RenderPixels(pixels, dimension, brushColors, scale, defaultFill)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="colorSet"></param>
        ''' <returns></returns>
        Public Overrides Function RenderPixels(pixels As PixelData(), dimension As Size, colorSet As SolidBrush(),
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
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
            Dim skipTransparent As Boolean = Not overlaps Is Nothing

            Using buffer As BitmapBuffer = BitmapBuffer.FromBitmap(raw, ImageLockMode.WriteOnly)
                For Each point As PixelData In PixelData.ScalePixels(pixels)
                    level = point.level

                    If level <= 0.000005 Then
                        color = defaultColor

                        If skipTransparent Then
                            Continue For
                        End If
                    Else
                        index = levelRange.ScaleMapping(level, indexrange)

                        If index <= 0 Then
                            index = 0

                            If skipTransparent Then
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
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As GraphicsData

            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
