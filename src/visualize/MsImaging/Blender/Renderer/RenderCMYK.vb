#Region "Microsoft.VisualBasic::51e0b28a0a9f39818f355e82f72e7820, visualize\MsImaging\Blender\Renderer\RenderCMYK.vb"

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

    '   Total Lines: 80
    '    Code Lines: 51 (63.75%)
    ' Comment Lines: 19 (23.75%)
    '    - Xml Docs: 84.21%
    ' 
    '   Blank Lines: 10 (12.50%)
    '     File Size: 3.12 KB


    '     Class RenderCMYK
    ' 
    '         Properties: Cchannel, Kchannel, Mchannel, Ychannel
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Render
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
