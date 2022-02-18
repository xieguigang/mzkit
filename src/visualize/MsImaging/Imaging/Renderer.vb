#Region "Microsoft.VisualBasic::f7c8148ae8ec0dc6b20abc3a7ce8fe22, src\visualize\MsImaging\Imaging\Renderer.vb"

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

'     Class MzLayerColorSet
' 
'         Properties: colorSet, mz, tolerance
' 
'         Function: FindColor, SelectGroup
' 
'     Class Renderer
' 
'         Function: AutoCheckCutMax, GetPixelChannelReader
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Imaging

    Public MustInherit Class Renderer

        Protected heatmapMode As Boolean

        Sub New(heatmapRender As Boolean)
            heatmapMode = heatmapRender
        End Sub

        ''' <summary>
        ''' 每一种离子一种对应的颜色生成多个图层，然后叠在在一块进行可视化
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension"></param>
        ''' <param name="colorSet">
        ''' [mz(F4) => color]
        ''' </param>
        ''' <param name="dimSize"></param>
        ''' <param name="scale"></param>
        ''' <param name="cut"></param>
        ''' <returns></returns>
        Public MustOverride Function LayerOverlaps(pixels As PixelData()(), dimension As Size, colorSet As MzLayerColorSet,
                                                   Optional dimSize As Size = Nothing,
                                                   Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                   Optional cut As DoubleRange = Nothing,
                                                   Optional defaultFill As String = "Transparent",
                                                   Optional mapLevels As Integer = 25) As Bitmap

        ''' <summary>
        ''' 最多只支持三种离子（R,G,B）
        ''' </summary>
        ''' <param name="R"></param>
        ''' <param name="G"></param>
        ''' <param name="B"></param>
        ''' <param name="dimension"></param>
        ''' <param name="dimSize"></param>
        ''' <param name="scale"></param>
        ''' <param name="cut"></param>
        ''' <returns></returns>
        Public MustOverride Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                         dimension As Size,
                                                         Optional dimSize As Size = Nothing,
                                                         Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                         Optional cut As (r As DoubleRange, g As DoubleRange, b As DoubleRange) = Nothing,
                                                         Optional background As String = "black") As Bitmap

        ''' <summary>
        ''' 将所有的离子混合叠加再一个图层中可视化
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
        Public MustOverride Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size,
                                                  Optional colorSet As String = "YlGnBu:c8",
                                                  Optional mapLevels% = 25,
                                                  Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                  Optional defaultFill As String = "Transparent",
                                                  Optional cutoff As DoubleRange = Nothing) As Bitmap

        ''' <summary>
        ''' 将所有的离子混合叠加再一个图层中可视化
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="dimSize">pixel size</param>
        ''' <param name="colorSet"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <paramref name="dimSize"/> value set to nothing for returns the raw image
        ''' </remarks>
        Public MustOverride Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size, colorSet As SolidBrush(),
                                                  Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                  Optional defaultFill As String = "Transparent",
                                                  Optional cutoff As DoubleRange = Nothing) As Bitmap

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="channel"></param>
        ''' <param name="cut">
        ''' [0,1]
        ''' </param>
        ''' <returns></returns>
        Protected Function GetPixelChannelReader(channel As PixelData(), cut As DoubleRange) As Func(Of Integer, Integer, Byte)
            If channel.IsNullOrEmpty Then
                Return Function(x, y) CByte(0)
            End If

            Dim intensityRange As DoubleRange = channel.Select(Function(p) p.intensity).ToArray
            Dim byteRange As DoubleRange = {0, 255}
            Dim xy = channel _
                .GroupBy(Function(p) p.x) _
                .ToDictionary(Function(p) p.Key,
                              Function(x)
                                  Return x _
                                      .GroupBy(Function(p) p.y) _
                                      .ToDictionary(Function(p) p.Key,
                                                    Function(p)
                                                        Return Aggregate pm As PixelData
                                                               In p
                                                               Into Max(pm.intensity)
                                                    End Function)
                              End Function)

            If Not cut Is Nothing Then
                Dim length As Double = intensityRange.Length
                Dim dmin = intensityRange.Min + cut.Min * length
                Dim dmax = intensityRange.Min + cut.Max * length

                intensityRange = New DoubleRange(dmin, dmax)
            End If

            Return Function(x, y) As Byte
                       If Not xy.ContainsKey(x) Then
                           Return 0
                       End If

                       Dim ylist = xy.Item(x)

                       If Not ylist.ContainsKey(y) Then
                           Return 0
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
        End Function

    End Class
End Namespace
