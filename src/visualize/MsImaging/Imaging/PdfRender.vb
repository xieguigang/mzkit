Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace Imaging

    Public Class PdfRender : Inherits Renderer

        Public Sub New(heatmapRender As Boolean)
            MyBase.New(heatmapRender)
        End Sub

        Public Overrides Function LayerOverlaps(pixels As PixelData()(),
                                                dimension As Drawing.Size,
                                                colorSet As MzLayerColorSet,
                                                Optional dimSize As Drawing.Size = Nothing,
                                                Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                Optional cut As DoubleRange = Nothing,
                                                Optional defaultFill As String = "Transparent",
                                                Optional mapLevels As Integer = 25) As Drawing.Bitmap

            Throw New NotImplementedException()
        End Function

        Public Overrides Function ChannelCompositions(R() As PixelData, G() As PixelData, B() As PixelData,
                                                      dimension As Drawing.Size,
                                                      Optional dimSize As Drawing.Size = Nothing,
                                                      Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                                      Optional cut As (r As DoubleRange, g As DoubleRange, b As DoubleRange) = Nothing,
                                                      Optional background As String = "black") As Drawing.Bitmap
            Throw New NotImplementedException()
        End Function

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Drawing.Size, dimSize As Drawing.Size,
                                               Optional colorSet As String = "YlGnBu:c8",
                                               Optional mapLevels As Integer = 25,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As Drawing.Bitmap

            Throw New NotImplementedException()
        End Function

        Public Overrides Function RenderPixels(pixels() As PixelData, dimension As Drawing.Size, dimSize As Drawing.Size,
                                               colorSet() As Drawing.SolidBrush,
                                               Optional scale As InterpolationMode = InterpolationMode.Bilinear,
                                               Optional defaultFill As String = "Transparent",
                                               Optional cutoff As DoubleRange = Nothing) As Drawing.Bitmap

            Throw New NotImplementedException()
        End Function
    End Class
End Namespace