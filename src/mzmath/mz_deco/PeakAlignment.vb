﻿#Region "Microsoft.VisualBasic::75ca11d2aef1a9dea5778288f0523521, mzmath\mz_deco\PeakAlignment.vb"

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

    '   Total Lines: 190
    '    Code Lines: 145 (76.32%)
    ' Comment Lines: 22 (11.58%)
    '    - Xml Docs: 81.82%
    ' 
    '   Blank Lines: 23 (12.11%)
    '     File Size: 8.14 KB


    ' Module PeakAlignment
    ' 
    '     Function: CowAlignment, CreatePeak, PickReferenceSampleMaxIntensity, RIAlignment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.SignalProcessing.COW
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports std = System.Math

''' <summary>
''' 峰对齐操作主要是针对保留时间漂移进行矫正
''' 
''' 在峰对齐操作之中所处理的对象就是不同的样本原始数据文件之间的在给定误差下相同``m/z``的峰之间的保留时间矫正的操作
''' 峰对齐的一个基础的操作为比较峰的相似度
''' </summary>
Public Module PeakAlignment

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function CreatePeak(id As String, mz As Double, rt As Double, intensity As Double) As PeakFeature
        Return New PeakFeature With {
            .xcms_id = id,
            .mz = mz,
            .rt = rt,
            .area = intensity,
            .maxInto = intensity
        }
    End Function

    <Extension>
    Public Function PickReferenceSampleMaxIntensity(samples As IEnumerable(Of NamedCollection(Of PeakFeature))) As NamedCollection(Of PeakFeature)
        Dim maxinto As Double = Double.MinValue
        Dim refer As NamedCollection(Of PeakFeature) = Nothing

        For Each sample As NamedCollection(Of PeakFeature) In samples
            Dim into As Double = Aggregate peak In sample Into Average(peak.maxInto)
            Dim area As Double = Aggregate peak In sample Into Sum(peak.area)
            Dim rank As Double = into * area

            If rank > maxinto Then
                maxinto = rank
                refer = sample
            End If
        Next

        Return refer
    End Function

    ''' <summary>
    ''' create peaktable matrix by retention index alignment.
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <param name="top_ion">
    ''' use the top intensity/area ion its m/z value as peak ion m/z.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function RIAlignment(samples As IEnumerable(Of NamedCollection(Of PeakFeature)),
                                         Optional rt_shift As List(Of RtShift) = Nothing,
                                         Optional mzdiff As Double = 0.005,
                                         Optional ri_offset As Double = 1,
                                         Optional top_ion As Boolean = False,
                                         Optional aggregate As Aggregates = Aggregates.Sum) As IEnumerable(Of xcms2)
        Dim allData = samples.ToArray
        ' make data bins by RI
        Dim RI_rawdata = allData.IteratesAll.GroupBy(Function(i) i.RI, offsets:=ri_offset).ToArray
        Dim unique_id As New Dictionary(Of String, Counter)
        Dim refer As String = allData.PickReferenceSampleMaxIntensity.name
        Dim mz_bin As New GroupBins(Of PeakFeature)(Function(i) i.mz, Function(a, b) std.Abs(a - b) < mzdiff, left_margin_bin:=True)
        Dim ion_mz As Double
        Dim f As Func(Of Double, Double, Double) = aggregate.GetAggregateFunction2

        If rt_shift Is Nothing Then
            rt_shift = New List(Of RtShift)
        End If

        For Each ri_point As NamedCollection(Of PeakFeature) In Tqdm.Wrap(RI_rawdata, wrap_console:=App.EnableTqdm)
            ' make data bins by mz
            ' where the given data all has the same RI value
            Dim mz_group As NamedCollection(Of PeakFeature)() = mz_bin _
                .GroupBy(ri_point) _
                .ToArray

            For Each peak As NamedCollection(Of PeakFeature) In mz_group
                Dim ri As Double = peak.Average(Function(a) a.RI)
                Dim mzri As String = $"M{CInt(Val(peak.name))}RI{CInt(ri)}"
                Dim refer_rt As PeakFeature = peak.Where(Function(p) p.rawfile = refer).FirstOrDefault
                Dim mz_set = peak.Select(Function(a) a.mz).ToArray

                If top_ion Then
                    ion_mz = peak _
                       .OrderByDescending(Function(i) i.area) _
                       .First.mz
                Else
                    ion_mz = mz_set.TabulateMode(topBin:=True, bags:=10)
                End If

                Dim peak1 As New xcms2 With {
                    .ID = mzri,
                    .mz = ion_mz,
                    .RI = ri,
                    .rt = peak.OrderByDescending(Function(pi) pi.maxInto).First.rt,
                    .mzmin = peak.Select(Function(pi) pi.mz).Min,
                    .mzmax = peak.Select(Function(pi) pi.mz).Max,
                    .rtmax = peak.Select(Function(pi) pi.rt).Max,
                    .rtmin = peak.Select(Function(pi) pi.rt).Min,
                    .RImin = peak.Select(Function(pi) pi.RI).Min,
                    .RImax = peak.Select(Function(pi) pi.RI).Max,
                    .groups = peak.Length
                }

                If refer_rt Is Nothing Then
                    refer_rt = peak _
                        .OrderByDescending(Function(p) p.maxInto) _
                        .First
                End If

                For Each sample As PeakFeature In peak
                    If peak1.HasProperty(sample.rawfile) Then
                        peak1(sample.rawfile) = f(peak1(sample.rawfile), sample.area)
                    Else
                        peak1(sample.rawfile) = sample.area
                    End If

                    rt_shift.Add(New RtShift() With {
                        .refer_rt = refer_rt.rt,
                        .RI = ri,
                        .sample = sample.rawfile,
                        .sample_rt = sample.rt,
                        .xcms_id = mzri
                    })
                Next

                Yield peak1
            Next
        Next
    End Function

    ''' <summary>
    ''' Make peak alignment via COW alignment algorithm.
    ''' </summary>
    ''' <param name="samples">the peak collection for each sample file, a sample </param>
    ''' <returns></returns>
    <Extension>
    Public Function CowAlignment(samples As IEnumerable(Of NamedCollection(Of PeakFeature))) As IEnumerable(Of xcms2)
        Dim cow As New CowAlignment(Of PeakFeature)(AddressOf CreatePeak)
        Dim rawdata = samples.ToArray
        Dim refer = rawdata.PickReferenceSampleMaxIntensity
        Dim targets = rawdata.Where(Function(sample) sample.name <> refer.name).ToArray
        Dim peaktable As New Dictionary(Of String, xcms2)

        For Each point As PeakFeature In refer
            peaktable.Add(point.xcms_id, New xcms2 With {
                .ID = point.xcms_id,
                .mz = point.mz,
                .mzmin = point.mz,
                .mzmax = point.mz,
                .rt = point.rt,
                .rtmin = point.rtmin,
                .rtmax = point.rtmax,
                .Properties = New Dictionary(Of String, Double) From {
                    {refer.name, point.area}
                }
            })
        Next

        Dim peak As xcms2

        For Each sample As NamedCollection(Of PeakFeature) In targets
            Dim aligns = cow.CorrelationOptimizedWarping(refer.AsList, sample.AsList).ToArray

            For Each point As PeakFeature In aligns
                peak = peaktable(point.xcms_id)
                peak.Add(sample.name, point.area)

                If point.mz < peak.mzmin Then
                    peak.mzmin = point.mz
                End If
                If point.mz > peak.mzmax Then
                    peak.mzmax = point.mz
                End If
            Next
        Next

        Return peaktable.Values
    End Function
End Module
