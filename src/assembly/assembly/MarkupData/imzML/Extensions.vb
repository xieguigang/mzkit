Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

Namespace MarkupData.imzML

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' Make conversion of the pixel data model
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="shape"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ExtractPixels(Of T As IMSIPixel)(shape As IEnumerable(Of T)) As IEnumerable(Of PixelScanIntensity)
            If shape Is Nothing Then
                Return
            End If

            For Each pixel As T In shape
                Yield New PixelScanIntensity With {.x = pixel.x, .y = pixel.y, .totalIon = pixel.intensity}
            Next
        End Function

        ''' <summary>
        ''' Make conversion of the pixel data model
        ''' </summary>
        ''' <typeparam name="PixelData"></typeparam>
        ''' <param name="layer"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function CreatePixelData(Of PixelData As {New, Pixel})(layer As IEnumerable(Of PixelScanIntensity)) As IEnumerable(Of PixelData)
            If layer Is Nothing Then
                Return
            End If

            For Each pixel As PixelScanIntensity In layer
                Yield New PixelData With {.X = pixel.x, .Y = pixel.y, .Scale = pixel.totalIon}
            Next
        End Function

    End Module
End Namespace