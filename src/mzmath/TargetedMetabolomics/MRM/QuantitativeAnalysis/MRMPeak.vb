#Region "Microsoft.VisualBasic::1052968558d4935714a462b717dd83e8, src\mzmath\TargetedMetabolomics\MRM\QuantitativeAnalysis\MRMPeak.vb"

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

'     Module MRMPeak
' 
'         Function: MRMPeak
'         Enum MRMPeakExtensionMethods
' 
'             BaselineMatch, TriangleMatch
' 
' 
' 
'  
' 
'     Function: MakeBaselineExtension, MakeExtension
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting

Namespace MRM

    Public Module MRMPeak

        ''' <summary>
        ''' Returns time range for the peak where the max intensity was represented.
        ''' </summary>
        ''' <param name="chromatogram">Should be order by time asceding.(应该是按照时间升序排序了的)</param>
        ''' <param name="threshold">
        ''' Meaning changes base on the <paramref name="method"/> value:
        ''' 
        ''' + <see cref="MRMPeakExtensionMethods.TriangleMatch"/>: Unit in degree, values in range ``[0-90]``
        ''' + <see cref="MRMPeakExtensionMethods.BaselineMatch"/>: Unit in percentage, value in range ``[0%-100%]``
        ''' </param>
        ''' <returns>The retention time range: ``[rtmin, rtmax]``</returns>
        <Extension>
        Public Function MRMPeak(chromatogram As IVector(Of ChromatogramTick),
                                Optional threshold# = 30%,
                                Optional winSize% = 5,
                                Optional baselineQuantile# = 0.65,
                                Optional method As MRMPeakExtensionMethods = MRMPeakExtensionMethods.BaselineMatch) As DoubleRange

            ' 先找到最高的信号，然后逐步分别往两边延伸
            ' 直到下降的速率小于阈值
            ' 因为MRM方法在一个色谱之中只出一个峰，所以在这里仅仅实现一个非常简单的峰的边界检测的算法

            Dim maxIndex%
            Dim timeRange#()

            ' 2018-1-18 如果事先将基线移除的话，或导致峰的范围扩大
            ' 取消掉
            ' removes all of the ticks that intensity value less than baseline.
            ' chromatogram = chromatogram(chromatogram!Intensity >= chromatogram.Base(baselineQuantile))
            maxIndex = Which.Max(chromatogram!Intensity)

            With chromatogram.ToArray

                If method = MRMPeakExtensionMethods.BaselineMatch Then
                    Dim baselineRange As DoubleRange

                    If threshold > 1 Then
                        ' normalize to 0-1
                        threshold /= 100
                    End If

                    baselineQuantile = chromatogram.Baseline(baselineQuantile)
                    baselineRange = {
                        baselineQuantile - baselineQuantile * threshold,
                        baselineQuantile + baselineQuantile * threshold
                    }
                    ' split
                    timeRange = {
                        .Take(maxIndex).Reverse.ToArray.MakeBaselineExtension(baselineRange, winSize),   ' left
                        .Skip(maxIndex).ToArray.MakeBaselineExtension(baselineRange, winSize)            ' right
                    }
                Else
                    threshold = Cos(threshold.ToRadians)
                    ' split
                    timeRange = {
                        .Take(maxIndex).Reverse.ToArray.MakeExtension(threshold, winSize),   ' left
                        .Skip(maxIndex).ToArray.MakeExtension(threshold, winSize)            ' right
                    }
                End If
            End With

            Return New DoubleRange(timeRange)
        End Function

        Public Enum MRMPeakExtensionMethods As Integer
            BaselineMatch
            TriangleMatch
        End Enum

        ''' <summary>
        ''' Detect a peak by using a slide window, when the average signal intensity is approximately equals to the <paramref name="baseline"/>,
        ''' Then found a peak boundary.
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="baseline#"></param>
        ''' <returns></returns>
        <Extension>
        Private Function MakeBaselineExtension(chromatogram As ChromatogramTick(), baseline As DoubleRange, winSize%) As Double
            ' 构建一个滑窗，如果滑窗的信号量平均值接近于baseline，则认为到达了峰的边界
            Dim windows = chromatogram _
                .SlideWindows(winSize) _
                .ToArray

            For Each block As SlideWindow(Of ChromatogramTick) In windows
                Dim average# = block.IntensityArray.Average

                If average <= baseline.Min OrElse baseline.IsInside(average) Then
                    ' 已经到达边界了，返回时间点
                    Return block.First.Time
                End If
            Next

            ' using the entire area???
            Return chromatogram.Last.Time
        End Function

        ''' <summary>
        ''' Returns the MRM signal peaks' time range boundary: ``t1 -> t2``
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="threshold#">cos value</param>
        ''' <param name="winSize%"></param>
        ''' <returns></returns>
        <Extension>
        Private Function MakeExtension(chromatogram As ChromatogramTick(), threshold#, winSize%) As Double
            Dim vector = chromatogram.Shadows
            Dim timeRange As DoubleRange = vector!Time
            Dim normInto As Vector = vector _
                !Intensity _
                .RangeTransform(timeRange) _
                .AsVector
            Dim windows = chromatogram _
                .Select(Function(c, i)
                            Return New ChromatogramTick With {
                                .Time = c.Time,
                                .Intensity = normInto(i)
                            }
                        End Function) _
                .SlideWindows(winSize) _
                .ToArray

            For Each block As SlideWindow(Of ChromatogramTick) In windows

                '      A
                '     /|
                '    / |
                '   /  |
                '  /   |
                ' ------
                ' t0   t1
                ' 
                ' cos(threshold) = (t1 - t0) / ( distance((t0, 0), (t1, A)) )
                '

                Dim t10 = block.Shadows!Time.Range.Length
                Dim t1 = block.First
                Dim t0 = block.Last
                Dim A = Abs(t0.Intensity - t1.Intensity)

                Dim C = (t1.Time, A).Distance(t0.Time, 0R)
                Dim cos# = t10 / C

                If threshold <= cos Then
                    Return t1.Time
                End If
            Next

            ' using the entire area???
            Return chromatogram.Last.Time
        End Function
    End Module
End Namespace
