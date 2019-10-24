Imports System.Runtime.CompilerServices

Namespace Chromatogram

    Public Module Deconvolution

        ''' <summary>
        ''' All of the mz value in <paramref name="mzpoints"/> should be equals
        ''' </summary>
        ''' <param name="mzpoints"></param>
        ''' <returns></returns>
        ''' <remarks>应用于处理复杂的样本数据</remarks>
        <Extension>
        Public Function GetPeakGroups(mzpoints As IEnumerable(Of TICPoint)) As IEnumerable(Of PeakFeature)
            Dim timepoints = mzpoints.OrderBy(Function(p) p.time).ToArray
        End Function


    End Module

    Public Class PeakFeature : Inherits ROI

        Public Property mz As Double

    End Class
End Namespace