Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Chromatogram

    Public Module ChromatogramMath

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TimeArray(chromatogram As IEnumerable(Of ChromatogramTick)) As Vector
            Return chromatogram.Select(Function(c) c.Time).AsVector
        End Function

        ''' <summary>
        ''' <see cref="ChromatogramTick.Intensity"/>
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function IntensityArray(chromatogram As IEnumerable(Of ChromatogramTick)) As Vector
            Return chromatogram.Select(Function(c) c.Intensity).AsVector
        End Function

        ''' <summary>
        ''' Detection of the signal base line based on the quantile method.
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="quantile#"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Baseline(chromatogram As IEnumerable(Of ChromatogramTick), Optional quantile# = 0.65) As Double
            Dim q As QuantileEstimationGK = chromatogram.Shadows!Intensity.GKQuantile
            Dim baseValue = q.Query(quantile)

            Return baseValue
        End Function
    End Module
End Namespace
