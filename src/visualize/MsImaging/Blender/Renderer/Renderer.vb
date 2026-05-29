#Region "Microsoft.VisualBasic::ef86af696b3d13c1c92acb5ddaf1bc40, visualize\MsImaging\Blender\Renderer\Renderer.vb"

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

    '   Total Lines: 238
    '    Code Lines: 145 (60.92%)
    ' Comment Lines: 62 (26.05%)
    '    - Xml Docs: 90.32%
    ' 
    '   Blank Lines: 31 (13.03%)
    '     File Size: 9.56 KB


    '     Class Renderer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DrawBackground, GetPixelChannelReader
    ' 
    '     Class PixelChannelRaster
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetPixelChannelReader
    ' 
    '         Sub: setRange
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports RasterPixel = Microsoft.VisualBasic.Imaging.Pixel

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
Imports PixelFormat = System.Drawing.Imaging.PixelFormat
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
Imports PixelFormat = Microsoft.VisualBasic.Imaging.PixelFormat
#End If

Namespace Blender

    ''' <summary>
    ''' abstract model for pixel based heatmap render or the rectangle based pixel heatmap render
    ''' </summary>
    Public MustInherit Class Renderer

        Protected heatmapMode As Boolean
        Protected gauss As Integer = 8
        Protected sigma As Integer = 32

        ''' <summary>
        ''' overlaps raster image as background
        ''' </summary>
        Protected ReadOnly overlaps As Image

        <DebuggerStepThrough>
        Sub New(heatmapRender As Boolean, overlaps As Image)
            Me.heatmapMode = heatmapRender
            Me.overlaps = overlaps
        End Sub

        ''' <summary>
        ''' draw background
        ''' </summary>
        ''' <remarks>
        ''' <paramref name="dimension"/> defines the image size directly
        ''' </remarks>
        Protected Function DrawBackground(dimension As Size, defaultBackground As Color) As Bitmap
            Dim raw As New Bitmap(dimension.Width, dimension.Height, PixelFormat.Format32bppArgb)

#If NET8_0_OR_GREATER Then
            Dim buffer As Byte() = raw.MemoryBuffer.RawBuffer

            For i As Integer = 0 To buffer.Length - 1 Step 4
                ' set background color manually
                buffer(i + 3) = defaultBackground.A
                buffer(i + 2) = defaultBackground.R
                buffer(i + 1) = defaultBackground.G
                buffer(i + 0) = defaultBackground.B
            Next
#End If

            Using g As IGraphics = DriverLoad.CreateGraphicsDevice(raw)
#If NET48 Then
                Call g.Clear(defaultBackground)
#End If
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
        ''' 每一种离子一种对应的颜色生成多个图层，然后叠在在一块进行可视化
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension"></param>
        ''' <param name="colorSet">
        ''' [mz(F4) => color]
        ''' </param>
        ''' <returns></returns>
        Public MustOverride Function LayerOverlaps(pixels As PixelData()(), dimension As Size, colorSet As MzLayerColorSet,
                                                   Optional defaultFill As String = "Transparent",
                                                   Optional mapLevels As Integer = 25) As GraphicsData

        ''' <summary>
        ''' 最多只支持三种离子（R,G,B）
        ''' </summary>
        ''' <param name="R"></param>
        ''' <param name="G"></param>
        ''' <param name="B"></param>
        ''' <param name="dimension"></param>
        ''' <returns></returns>
        Public MustOverride Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                         dimension As Size,
                                                         Optional background As String = "black") As GraphicsData

        ''' <summary>
        ''' 最多只支持四种离子（C,M,Y,K）
        ''' </summary>
        ''' <param name="dimension"></param>
        ''' <returns></returns>
        Public MustOverride Function ChannelCompositions(C As PixelData(), M As PixelData(), Y As PixelData(), K As PixelData(),
                                                         dimension As Size,
                                                         Optional background As String = "black") As GraphicsData

        ''' <summary>
        ''' 将所有的离子混合叠加再一个图层中可视化
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="HeatMapParameters.defaultFill"/> configs of the background of the MS-imaging chartting.
        ''' </remarks>
        Public MustOverride Function RenderPixels(pixels As PixelData(), dimension As Size, heatmap As HeatMapParameters) As GraphicsData

        ''' <summary>
        ''' 将所有的离子混合叠加再一个图层中可视化
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <returns></returns>
        Public MustOverride Function RenderPixels(pixels As PixelData(), dimension As Size, heatmap As HeatMapBrushes) As GraphicsData

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="channel"></param>
        ''' <returns></returns>
        Protected Function GetPixelChannelReader(channel As PixelData()) As Func(Of Integer, Integer, Byte)
            If channel.IsNullOrEmpty Then
                Return Function(x, y) CByte(0)
            Else
                Return AddressOf New PixelChannelRaster(gauss, sigma, channel).GetPixelChannelReader
            End If
        End Function

    End Class

    ''' <summary>
    ''' A single signal channel data spatial reader helper
    ''' </summary>
    Friend Class PixelChannelRaster

        Dim raster As RasterPixel()
        Dim intensityRange As DoubleRange
        Dim xy As Dictionary(Of Integer, Dictionary(Of Integer, Double))
        Dim byteRange As DoubleRange = New Double() {8, 255}
        Dim delta As Double

        Sub New(gauss As Integer, sigma As Integer, channel As PixelData())
            Me.raster = New HeatMapRaster(Of PixelData)(gauss, sigma) _
                .SetDatas(channel.ToList) _
                .GetRasterPixels _
                .ToArray
            Me.raster = channel
            Me.xy = raster _
                .GroupBy(Function(p) p.X) _
                .ToDictionary(Function(p) p.Key,
                              Function(x)
                                  Return x _
                                      .GroupBy(Function(p) p.Y) _
                                      .ToDictionary(Function(p) p.Key,
                                                    Function(p)
                                                        Return Aggregate pm As RasterPixel
                                                               In p
                                                               Into Average(pm.Scale)
                                                    End Function)
                              End Function)
            Call setRange()
        End Sub

        Private Sub setRange()
            Me.intensityRange = raster _
                .Select(Function(p) p.Scale) _
                .ToArray
            Me.delta = intensityRange.Length
        End Sub

        Public Function GetPixelChannelReader(x As Integer, y As Integer) As Byte
            If Not xy.ContainsKey(x) Then
                Return 0
            End If

            Dim ylist = xy.Item(x)

            If Not ylist.ContainsKey(y) Then
                Return 0
            End If

            ' 20250123 just a single pixel spot data
            ' so scale range is zero
            ' returns 255 directly
            If delta = 0.0 Then
                Return 255
            End If

            Dim into As Double = ylist.Item(y)

            If into <= intensityRange.Min Then
                into = intensityRange.Min
            ElseIf into >= intensityRange.Max Then
                into = intensityRange.Max
            Else
                ' do nothing
            End If

            Return CByte(intensityRange.ScaleMapping(into, byteRange))
        End Function
    End Class
End Namespace
