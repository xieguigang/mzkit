﻿#Region "Microsoft.VisualBasic::4c88f027e87cbb96958d5211a4de19b8, visualize\TissueMorphology\Scatter\LayerRender.vb"

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

    '   Total Lines: 84
    '    Code Lines: 55 (65.48%)
    ' Comment Lines: 15 (17.86%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 14 (16.67%)
    '     File Size: 3.05 KB


    ' Class LayerRender
    ' 
    '     Properties: alphaLevel, dotSize, highlights
    ' 
    '     Function: CheckGdiSizeParameter, Draw
    ' 
    '     Sub: Rendering
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq

Public Class LayerRender

    Public Property alphaLevel As Double = 0.8
    Public Property dotSize As New SizeF(3, 3)

    ''' <summary>
    ''' a list of the region labels for highlights
    ''' </summary>
    ''' <returns></returns>
    Public Property highlights As String()

    Public Sub Rendering(g As IGraphics, regions As IEnumerable(Of TissueRegion))
        Dim highlights As Index(Of String) = Me.highlights.SafeQuery.Distinct.Indexing
        Dim pt As PointF

        For Each region As TissueRegion In regions
            Dim fill As New SolidBrush(region.color.Alpha(255 * alphaLevel))

            If region.label Like highlights Then
                fill = New SolidBrush(region.color.Darken(0.25))
            End If

            For Each p As Point In region.points
                pt = New PointF(p.X * dotSize.Width, p.Y * dotSize.Height)
                g.FillRectangle(fill, New RectangleF(pt, dotSize))
            Next
        Next
    End Sub

    Const gdi2GB As Long = 2 * ByteSize.GB

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="layerSize"></param>
    ''' <param name="dotSize"></param>
    ''' <returns>
    ''' true means the gdi+ data is less than 2GB, false means invalid gdi+ parameter due to the reason of
    ''' byte size of the image is greater than 2GB.
    ''' </returns>
    Public Shared Function CheckGdiSizeParameter(layerSize As Size, dotSize As Single) As Boolean
        Dim w = layerSize.Width * dotSize
        Dim h = layerSize.Height * dotSize

        Return (w * h * 4) < gdi2GB
    End Function

    Public Shared Function Draw(regions As IEnumerable(Of TissueRegion),
                                layerSize As Size,
                                Optional alphaLevel As Double = 0.8,
                                Optional dotSize As Single = 3,
                                Optional highlights As String() = Nothing) As Bitmap

        Dim layer As New Bitmap(
            width:=layerSize.Width * dotSize,
            height:=layerSize.Height * dotSize,
            format:=PixelFormat.Format32bppArgb
        )
        Dim g As IGraphics = DriverLoad.CreateGraphicsDevice(layer)
        Dim blender As New LayerRender With {
            .alphaLevel = alphaLevel,
            .dotSize = New SizeF(dotSize, dotSize),
            .highlights = highlights
        }

        'g.CompositingQuality = CompositingQuality.HighQuality
        'g.InterpolationMode = InterpolationMode.HighQualityBilinear
        g.Clear(Color.Transparent)
        blender.Rendering(g, regions)
        g.Flush()
        g.Dispose()

        Return layer
    End Function
End Class
