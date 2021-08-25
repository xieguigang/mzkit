Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace Imaging

    Public MustInherit Class Renderer

        Public MustOverride Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                         dimension As Size,
                                                         Optional dimSize As Size = Nothing,
                                                         Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                         Optional cut As DoubleRange = Nothing) As Bitmap

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pixels"></param>
        ''' <param name="dimension">the scan size</param>
        ''' <param name="dimSize">pixel size</param>
        ''' <param name="colorSet"></param>
        ''' <param name="mapLevels"></param>
        ''' <param name="logE"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <paramref name="dimSize"/> value set to nothing for returns the raw image
        ''' </remarks>
        Public MustOverride Function RenderPixels(pixels As PixelData(), dimension As Size, dimSize As Size,
                                                  Optional colorSet As String = "YlGnBu:c8",
                                                  Optional mapLevels% = 25,
                                                  Optional logE As Boolean = False,
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
                                                        Return p.Select(Function(pm) pm.intensity).Max
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