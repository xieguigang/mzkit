﻿#Region "Microsoft.VisualBasic::d40da8d06cf12797a3d1951183b5ff23, mzmath\mz_deco\Signals\Chromatogram\AccumulateROI.vb"

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


    ' Code Statistics:

    '   Total Lines: 211
    '    Code Lines: 137 (64.93%)
    ' Comment Lines: 47 (22.27%)
    '    - Xml Docs: 74.47%
    ' 
    '   Blank Lines: 27 (12.80%)
    '     File Size: 8.50 KB


    '     Module AccumulateROI
    ' 
    '         Function: dt, JointPeak, JointPeaks, (+2 Overloads) PopulateROI
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Math.SignalProcessing.PeakFinding
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports std = System.Math

Namespace Chromatogram

    ''' <summary>
    ''' 根据累加线来查找色谱峰的ROI
    ''' </summary>
    Public Module AccumulateROI

        ''' <summary>
        ''' cut chromatogram via rt range and then populate all ROI from <see cref="PopulateROI"/>
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="rt"></param>
        ''' <param name="peakwidth"></param>
        ''' <param name="angleThreshold#"></param>
        ''' <param name="baselineQuantile#"></param>
        ''' <param name="snThreshold"></param>
        ''' <returns></returns>
        <Extension>
        Public Function PopulateROI(chromatogram As IVector(Of ChromatogramTick), rt As DoubleRange, peakwidth As DoubleRange,
                                    Optional angleThreshold# = 3,
                                    Optional baselineQuantile# = 0.65,
                                    Optional snThreshold As Double = 3) As IEnumerable(Of ROI)

            Return chromatogram((chromatogram!time >= rt.Min) & (chromatogram!time <= rt.Max)) _
                .PopulateROI(
                    peakwidth:=peakwidth,
                    angleThreshold:=angleThreshold,
                    baselineQuantile:=baselineQuantile,
                    snThreshold:=snThreshold
                )
        End Function

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
        ''' <param name="snThreshold">
        ''' negative value means no threshold cutoff
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个方法对于MRM的数据的处理结果比较可靠，但是对于GCMS的实验数据，
        ''' 由于GCMS实验数据的峰比较窄，这个函数不太适合处理GCMS的峰
        ''' </remarks>
        <Extension>
        Public Iterator Function PopulateROI(chromatogram As IVector(Of ChromatogramTick), peakwidth As DoubleRange,
                                             Optional angleThreshold# = 3,
                                             Optional baselineQuantile# = 0.65,
                                             Optional snThreshold As Double = 3,
                                             Optional joint As Boolean = False,
                                             Optional nticks As Integer = 6) As IEnumerable(Of ROI)
            ' 先计算出基线和累加线
            Dim baseline# = chromatogram.SignalBaseline(baselineQuantile)
            Dim time As Vector = chromatogram!time
            Dim peaks As SignalPeak() = New ElevationAlgorithm(angleThreshold, baselineQuantile) _
                .FindAllSignalPeaks(chromatogram.As(Of ITimeSignal)) _
                .Triming(peakwidth) _
                .ToArray

            If joint Then
                peaks = peaks.JointPeaks(peakwidth.Max).ToArray
            End If

            For Each window As SignalPeak In peaks
                Dim rtmin# = window.rtmin
                Dim rtmax# = window.rtmax
                Dim peak As ChromatogramTick() = window.region.As(Of ChromatogramTick).ToArray

                If peak.Length < nticks Then
                    Continue For
                End If

                Dim max# = peak.Max(Function(a) a.Intensity) - window.baseline
                Dim rt# = window(which.Max(window.region.Select(Function(a) a.intensity))).time
                Dim ROI As New ROI With {
                    .ticks = peak,
                    .maxInto = max,
                    .baseline = baseline,
                    .time = {rtmin, rtmax},
                    .integration = window.integration,
                    .rt = rt,
                    .noise = peak.Length * baseline
                }

                If snThreshold <= 0 OrElse ROI.snRatio >= snThreshold Then
                    Yield ROI
                End If
            Next
        End Function

        <Extension>
        Private Iterator Function JointPeaks(raw As SignalPeak(), max_width As Double) As IEnumerable(Of SignalPeak)
            Dim q2 As Double

            If raw.IsNullOrEmpty Then
                Return
            Else
                ' re-order data by rt asc
                raw = raw _
                    .OrderBy(Function(t) t.rtmin) _
                    .ToArray
            End If

            Dim dt As Double() = raw _
                .SlideWindows(winSize:=2, offset:=1) _
                .Select(Function(twoPeak)
                            Return AccumulateROI.dt(p0:=twoPeak.First, p1:=twoPeak.Last)
                        End Function) _
                .OrderBy(Function(a) a) _
                .ToArray
            Dim jointPeak As New List(Of SignalPeak) From {
                raw(0)
            }
            Dim quar As DataQuartile = dt.Quartile

            'If dt.Length > 2 Then
            '    q2 = dt(dt.Length * (3 / 4)) * 1.25
            'Else
            '    q2 = 0
            'End If

            ' q2 = dt.Average * (3 / 4)
            q2 = quar.Outlier(dt).normal.Average

            If q2 > max_width Then
                q2 = max_width * 3 / 4
            End If

            Dim left As Double = raw(0).rtmin
            Dim right As Double = 0

            For i As Integer = 1 To raw.Length - 1
                right = raw(i).rtmax

                If (right - left) <= max_width AndAlso AccumulateROI.dt(raw(i), raw(i - 1)) <= q2 Then
                    jointPeak.Add(raw(i))
                Else
                    If jointPeak.Count > 0 Then
                        ' break
                        Yield AccumulateROI.JointPeak(jointPeak)
                    End If

                    left = raw(i).rtmin

                    jointPeak.Clear()
                    jointPeak.Add(raw(i))
                End If
            Next

            If jointPeak.Count > 0 Then
                Yield AccumulateROI.JointPeak(jointPeak)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p1">p(i)</param>
        ''' <param name="p0">p(i-1)</param>
        ''' <returns></returns>
        Private Function dt(p1 As SignalPeak, p0 As SignalPeak) As Double
            Dim d1 = std.Abs(p1.rtmin - p0.rtmax)
            Dim d2 = std.Abs(p1.rtmin - p0.rtmin)
            Dim d3 = std.Abs(p1.rtmax - p0.rtmax)
            Dim d4 = std.Abs(p1.rtmax - p0.rtmin)

            Return {d1, d2, d3, d4}.Min
        End Function

        Private Function JointPeak(peaks As List(Of SignalPeak)) As SignalPeak
            If peaks.Count = 1 Then
                Return peaks.First
            End If

            Dim base = Aggregate part In peaks Into Average(part.baseline)
            Dim a = Aggregate part In peaks Into Sum(part.integration)
            ' may includes the duplicated points?
            Dim points As ITimeSignal() = peaks _
                .Select(Function(i) i.region) _
                .IteratesALL _
                .GroupBy(Function(ai) ai.time) _
                .Select(Function(ag) ag.First) _
                .OrderBy(Function(ti) ti.time) _
                .ToArray

            Return New SignalPeak With {
                .baseline = base,
                .integration = a,
                .region = points
            }
        End Function
    End Module
End Namespace
