Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace Imaging

    Public Class RectangleRender : Inherits Renderer

        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Size,
                                                      Optional dimSize As Size = Nothing,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear) As Bitmap
            Throw New NotImplementedException()
        End Function

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Size, dimSize As Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels As Integer = 25,
                                               Optional logE As Boolean = False,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As Bitmap
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace