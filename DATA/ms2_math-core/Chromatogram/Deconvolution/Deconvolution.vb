Imports System.Runtime.CompilerServices

Namespace Chromatogram

    Public Module Deconvolution

        ''' <summary>
        ''' All of the mz value in <paramref name="mzpoints"/> should be equals
        ''' </summary>
        ''' <param name="mzpoints"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetPeakGroups(mzpoints As IEnumerable(Of TICPoint)) As IEnumerable(Of PeakFeature)

        End Function
    End Module

    Public Class PeakFeature : Inherits ROI

        Public Property mz As Double

    End Class
End Namespace