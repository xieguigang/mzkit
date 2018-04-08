Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM.Dumping

Public Module Extensions

    ''' <summary>
    ''' Quantile summary of the chromatogram tick <see cref="ChromatogramTick.Intensity"/>
    ''' </summary>
    ''' <param name="chromatogram"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Summary(chromatogram As IEnumerable(Of ChromatogramTick), Optional delta# = 0.1) As IEnumerable(Of Quantile)
        Dim quantile = chromatogram.Shadows!Intensity.GKQuantile

        For q As Double = 0 To 1 Step delta
            Yield New Quantile With {
                .Percentage = q,
                .Quantile = quantile.Query(q)
            }
        Next
    End Function
End Module
