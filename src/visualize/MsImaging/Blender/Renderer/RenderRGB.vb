#Region "Microsoft.VisualBasic::2cd3eb204c67d9359a2cf6560e5116f8, G:/mzkit/src/visualize/MsImaging//Blender/Renderer/RenderRGB.vb"

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

    '   Total Lines: 76
    '    Code Lines: 61
    ' Comment Lines: 3
    '   Blank Lines: 12
    '     File Size: 3.43 KB


    '     Class RenderRGB
    ' 
    '         Properties: Bchannel, dimension, Gchannel, Rchannel
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

Namespace Blender

    Public Class RenderRGB

        ReadOnly defaultBackground As Color
        ReadOnly heatmapMode As Boolean

        Public Property dimension As Size

        Public Property Rchannel As Func(Of Integer, Integer, Byte)
        Public Property Gchannel As Func(Of Integer, Integer, Byte)
        Public Property Bchannel As Func(Of Integer, Integer, Byte)

        Sub New(defaultBackground As Color, Optional heatmapMode As Boolean = False)
            Me.heatmapMode = heatmapMode
            Me.defaultBackground = defaultBackground
        End Sub

        Public Sub Render(ByRef gr As IGraphics, region As GraphicsRegion)
            Dim plotOffset As Point = region.PlotRegion.Location
            Dim pos As PointF
            Dim rect As RectangleF
            Dim pixel_size As New SizeF(1, 1)

            For x As Integer = 1 To dimension.Width
                For y As Integer = 1 To dimension.Height
                    Dim bR As Byte = Rchannel(x, y)
                    Dim bG As Byte = Gchannel(x, y)
                    Dim bB As Byte = Bchannel(x, y)
                    Dim color As Color

                    pos = New PointF With {
                        .X = (x - 1) + plotOffset.X,
                        .Y = (y - 1) + plotOffset.Y
                    }
                    rect = New RectangleF(pos, pixel_size)

                    If bR = 0 AndAlso bG = 0 AndAlso bB = 0 Then
                        ' missing a pixel at here?
                        If heatmapMode Then
                            bR = CByte(New Integer() {
                                Rchannel(x - 1, y - 1), Rchannel(x, y - 1), Rchannel(x + 1, y - 1),
                                Rchannel(x - 1, y), Rchannel(x, y), Rchannel(x + 1, y),
                                Rchannel(x - 1, y + 1), Rchannel(x, y + 1), Rchannel(x + 1, y + 1)
                            }.Average)
                            bB = CByte(New Integer() {
                                Bchannel(x - 1, y - 1), Bchannel(x, y - 1), Bchannel(x + 1, y - 1),
                                Bchannel(x - 1, y), Bchannel(x, y), Bchannel(x + 1, y),
                                Bchannel(x - 1, y + 1), Bchannel(x, y + 1), Bchannel(x + 1, y + 1)
                            }.Average)
                            bG = CByte(New Integer() {
                                Gchannel(x - 1, y - 1), Gchannel(x, y - 1), Gchannel(x + 1, y - 1),
                                Gchannel(x - 1, y), Gchannel(x, y), Gchannel(x + 1, y),
                                Gchannel(x - 1, y + 1), Gchannel(x, y + 1), Gchannel(x + 1, y + 1)
                            }.Average)

                            color = Color.FromArgb(bR, bG, bB)
                        Else
                            color = defaultBackground
                        End If
                    Else
                        color = Color.FromArgb(bR, bG, bB)
                    End If

                    ' imzXML里面的坐标是从1开始的
                    ' 需要减一转换为.NET中从零开始的位置
                    Call gr.FillRectangle(New SolidBrush(color), rect)
                Next
            Next
        End Sub
    End Class
End Namespace
