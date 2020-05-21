#Region "Microsoft.VisualBasic::e2daa309bb3f7a77f2878635eb4deeee, src\mzmath\ms2_math-core\Chromatogram\AccumulateROI.vb"

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

'     Module AccumulateROI
' 
'         Function: getAccumulateLine, PopulateROI, SplitGCMSPeaks, SplitMRMPeaks
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Math.SignalProcessing.PeakFinding

Namespace Chromatogram

    ''' <summary>
    ''' 根据累加线来查找色谱峰的ROI
    ''' </summary>
    Public Module AccumulateROI

        ''' <summary>
        ''' The input data parameter <paramref name="chromatogram"/> for this function should be 
        ''' sort in asc order at first!
        ''' 
        ''' (在这个函数之中，只是查找出了色谱峰的时间范围，但是并未对峰面积做积分计算)
        ''' </summary>
        ''' <param name="angleThreshold">
        ''' The higher of this value it is, the more sensible it its.
        ''' (区分色谱峰的累加线切线角度的阈值，单位为度)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个方法对于MRM的数据的处理结果比较可靠，但是对于GCMS的实验数据，
        ''' 由于GCMS实验数据的峰比较窄，这个函数不太适合处理GCMS的峰
        ''' </remarks>
        <Extension>
        Public Iterator Function PopulateROI(chromatogram As IVector(Of ChromatogramTick), peakwidth As DoubleRange,
                                             Optional angleThreshold# = 5,
                                             Optional baselineQuantile# = 0.65,
                                             Optional snThreshold As Double = 3) As IEnumerable(Of ROI)
            ' 先计算出基线和累加线
            Dim baseline# = chromatogram.Baseline(baselineQuantile)
            Dim time As Vector = chromatogram!time
            Dim peaks As SignalPeak() = New ElevationAlgorithm(angleThreshold, baselineQuantile) _
                .FindAllSignalPeaks(chromatogram.OfType(Of ITimeSignal)) _
                .Triming(peakwidth) _
                .ToArray

            For Each window As SignalPeak In peaks
                Dim rtmin# = window.rtmin
                Dim rtmax# = window.rtmax
                Dim peak As ChromatogramTick() = window.region
                Dim max# = peak.Max(Function(a) a.Intensity)
                Dim rt# = window(Which.Max(window.region.Select(Function(a) a.intensity))).time
                Dim ROI As New ROI With {
                    .ticks = peak,
                    .maxInto = max,
                    .baseline = baseline,
                    .time = {rtmin, rtmax},
                    .integration = window.integration,
                    .rt = rt,
                    .noise = peak.Length * baseline
                }

                If ROI.snRatio >= snThreshold Then
                    Yield ROI
                End If
            Next
        End Function
    End Module
End Namespace
