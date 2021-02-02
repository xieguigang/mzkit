#Region "Microsoft.VisualBasic::78af973eada1d1b7a274e0d275da0e6e, ms2_math-core\Chromatogram\Chromatogram.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Module ChromatogramMath
    ' 
    '         Function: Baseline, IntensityArray, Summary, TimeArray, TimeRange
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Namespace Chromatogram

    Public Module ChromatogramMath

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TimeRange(chromatogram As IEnumerable(Of ChromatogramTick)) As DoubleRange
            Return New DoubleRange(chromatogram.Select(Function(c) c.Time))
        End Function

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
End Namespace
