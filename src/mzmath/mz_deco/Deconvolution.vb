#Region "Microsoft.VisualBasic::e71cf6d4b1a75e156afd6dbbfbab8bef, mzkit\src\mzmath\mz_deco\Deconvolution.vb"

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

    '   Total Lines: 172
    '    Code Lines: 125
    ' Comment Lines: 28
    '   Blank Lines: 19
    '     File Size: 6.75 KB


    ' Module Deconvolution
    ' 
    '     Function: DecoMzGroups, (+2 Overloads) GetMzGroups, GetPeakGroups, localMax, localMin
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports scan = BioNovoGene.Analytical.MassSpectrometry.Math.IMs1Scan
Imports stdNum = System.Math

''' <summary>
''' #### 解卷积计算步骤
''' 
''' 1. 首先对每一个原始数据点按照mz进行数据分组
''' 2. 对每一个mz数据分组按照rt进行升序排序
''' 3. 对每一个mz数据分组进行解卷积，得到峰列表
''' 4. 输出peaktable结果，完成解卷积操作
''' </summary>
Public Module Deconvolution

    ''' <summary>
    ''' All of the mz value in <paramref name="mzpoints"/> should be equals
    ''' </summary>
    ''' <param name="mzpoints"></param>
    ''' <returns></returns>
    ''' <remarks>实际的解卷积操作步骤：应用于处理复杂的样本数据</remarks>
    <Extension>
    Public Iterator Function GetPeakGroups(mzpoints As MzGroup, peakwidth As DoubleRange,
                                           Optional quantile# = 0.65,
                                           Optional sn_threshold As Double = 3) As IEnumerable(Of PeakFeature)

        For Each ROI As ROI In mzpoints.XIC.Shadows.PopulateROI(
            peakwidth:=peakwidth,
            baselineQuantile:=quantile,
            snThreshold:=sn_threshold
        )
            Yield New PeakFeature With {
                .mz = stdNum.Round(mzpoints.mz, 4),
                .baseline = ROI.baseline,
                .integration = ROI.integration,
                .maxInto = ROI.maxInto,
                .noise = ROI.noise,
                .rt = ROI.rt,
                .rtmax = ROI.time.Max,
                .rtmin = ROI.time.Min,
                .nticks = ROI.ticks.Length,
                .area = ROI.ticks.Select(Function(t) t.Intensity).Sum
            }
        Next
    End Function


    <Extension>
    Private Function localMax(window As IEnumerable(Of ChromatogramTick)) As ChromatogramTick
        Return window.OrderByDescending(Function(t) t.Intensity).First
    End Function

    <Extension>
    Private Function localMin(window As IEnumerable(Of ChromatogramTick)) As ChromatogramTick
        Return window.OrderBy(Function(t) t.Intensity).First
    End Function

    ''' <summary>
    ''' 1. Separation of mass signals, generate XIC sequence data.
    ''' (进行原始数据的mz分组操作，然后进行rt的升序排序)
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function GetMzGroups(Of T As scan)(scans As IEnumerable(Of T),
                                                       Optional rtwin As Double = 0.05,
                                                       Optional mzdiff As Tolerance = Nothing) As IEnumerable(Of MzGroup)
        ' group by m/z at first
        For Each group As NamedCollection(Of T) In scans _
            .GroupBy(
                evaluate:=Function(ti) ti.mz,
                equals:=mzdiff Or Tolerance.DefaultTolerance
             )

            Yield group.GetMzGroups(rtwin)
        Next
    End Function

    <Extension>
    Private Function GetMzGroups(Of T As scan)(group As NamedCollection(Of T), rtwin As Double) As MzGroup
        Dim rawGroup As T() = group.ToArray
        Dim timePoints As NamedCollection(Of T)() = rawGroup _
            .GroupBy(Function(ti) ti.rt,
                     Function(a, b)
                         Return stdNum.Abs(a - b) <= rtwin
                     End Function) _
            .ToArray
        Dim xic As ChromatogramTick() = timePoints _
            .Select(Function(ti)
                        Dim rt As Double = Aggregate p As scan In ti Into Average(p.rt)
                        Dim into As Double = Aggregate p As scan
                                             In ti
                                             Into Max(p.intensity)

                        Return New ChromatogramTick With {
                            .Time = rt,
                            .Intensity = into
                        }
                    End Function) _
            .OrderBy(Function(ti) ti.Time) _
            .ToArray
        Dim mzPoint As T = rawGroup _
            .OrderByDescending(Function(d) d.intensity) _
            .First

        Return New MzGroup With {
            .mz = mzPoint.mz,
            .XIC = xic
        }
    End Function

    ''' <summary>
    ''' 2. 对得到的XIC进行峰查找
    ''' </summary>
    ''' <param name="mzgroups"></param>
    ''' <param name="quantile#"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function DecoMzGroups(mzgroups As IEnumerable(Of MzGroup), peakwidth As DoubleRange,
                                          Optional quantile# = 0.65,
                                          Optional sn As Double = 3,
                                          Optional nticks As Integer = 6,
                                          Optional parallel As Boolean = False) As IEnumerable(Of PeakFeature)

        Dim groupData As MzGroup() = mzgroups.ToArray
        Dim features As IGrouping(Of String, PeakFeature)() = groupData _
            .Populate(parallel) _
            .Select(Function(mz)
                        Return mz.GetPeakGroups(peakwidth, quantile, sn)
                    End Function) _
            .IteratesALL _
            .Where(Function(peak) peak.nticks >= nticks) _
            .GroupBy(Function(m)
                         ' 产生xcms id编号的Mxx部分
                         Return stdNum.Round(m.mz).ToString
                     End Function) _
            .ToArray
        Dim guid As New Dictionary(Of String, Counter)
        Dim uid As String

        For Each mzId As IGrouping(Of String, PeakFeature) In features
            Dim mId As String = mzId.Key
            Dim rtIdgroup = mzId _
                .GroupBy(Function(m) stdNum.Round(m.rt).ToString) _
                .ToArray

            For Each rtgroup As IGrouping(Of String, PeakFeature) In rtIdgroup
                uid = $"M{mId}T{rtgroup.Key}"
                guid(uid) = 0

                For Each feature As PeakFeature In rtgroup
                    If guid(uid).Value = 0 Then
                        feature.xcms_id = uid
                    Else
                        feature.xcms_id = uid & "_" & guid(uid).ToString
                    End If

                    Call guid(uid).Hit()

                    Yield feature
                Next
            Next
        Next
    End Function
End Module
