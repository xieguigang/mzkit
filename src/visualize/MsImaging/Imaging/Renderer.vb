Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace Imaging

    Public MustInherit Class Renderer

        Public MustOverride Function ChannelCompositions(R As PixelData(), G As PixelData(), B As PixelData(),
                                                         dimension As Size,
                                                         Optional dimSize As Size = Nothing,
                                                         Optional scale As InterpolationMode = InterpolationMode.Bilinear) As Bitmap

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

    End Class
End Namespace