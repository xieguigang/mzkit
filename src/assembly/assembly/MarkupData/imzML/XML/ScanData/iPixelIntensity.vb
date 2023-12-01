Namespace MarkupData.imzML

    ''' <summary>
    ''' A pixel spot scan summary data in MSI
    ''' </summary>
    Public Class iPixelIntensity : Inherits PixelScanIntensity

        ''' <summary>
        ''' The max intensity in current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property basePeakIntensity As Double
        ''' <summary>
        ''' The average intensity in current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property average As Double
        ''' <summary>
        ''' The ions m/z which it is the max intensity value inside current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property basePeakMz As Double
        ''' <summary>
        ''' the min intensity value of current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property min As Double
        ''' <summary>
        ''' the 50% quantile intensity of current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property median As Double
        ''' <summary>
        ''' the number of the ions in current spot
        ''' </summary>
        ''' <returns></returns>
        Public Property numIons As Integer

        Public Overrides Function ToString() As String
            Return $"[{x}, {y}] {totalIon.ToString("G3")}"
        End Function

    End Class

End Namespace