Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel

Public Module Extensions

    ''' <summary>
    ''' scale to new size for reduce data for downstream analysis
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="scaleTo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function JoinPixels(pixels As IEnumerable(Of PixelScan), scaleTo As Size) As mzPackPixel

    End Function

    ''' <summary>
    ''' scale to new size for reduce data size for downstream analysis by
    ''' pixels sampling
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="samplingSize"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Sampling(pixels As IEnumerable(Of PixelScan), samplingSize As Size) As IEnumerable(Of PixelScan)
        For Each pixel As PixelScan In pixels
            If pixel.X Mod samplingSize.Width <> 0 Then
                Continue For
            ElseIf pixel.Y Mod samplingSize.Height <> 0 Then
                Continue For
            End If

            Yield pixel
        Next
    End Function
End Module
