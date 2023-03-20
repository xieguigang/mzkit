#Region "Microsoft.VisualBasic::3a5a6220b5e1c32f99585f9738c19b74, mzkit\src\mzmath\mz_deco\PeakAlignment.vb"

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

    '   Total Lines: 67
    '    Code Lines: 55
    ' Comment Lines: 6
    '   Blank Lines: 6
    '     File Size: 2.78 KB


    ' Module PeakAlignment
    ' 
    '     Function: CreateMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

''' <summary>
''' 峰对齐操作主要是针对保留时间漂移进行矫正
''' 
''' 在峰对齐操作之中所处理的对象就是不同的样本原始数据文件之间的在给定误差下相同``m/z``的峰之间的保留时间矫正的操作
''' 峰对齐的一个基础的操作为比较峰的相似度
''' </summary>
Public Module PeakAlignment

    <Extension>
    Public Iterator Function CreateMatrix(samples As IEnumerable(Of NamedCollection(Of PeakFeature)),
                                          mzdiff As Tolerance,
                                          Optional rt_win As Double = 30) As IEnumerable(Of xcms2)
        Dim tag_peaks = samples _
            .Select(Iterator Function(peaks) As IEnumerable(Of (sample As String, peak As PeakFeature))
                        For Each peak As PeakFeature In peaks
                            Yield (peaks.name, peak)
                        Next
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.peak.mz, mzdiff) _
            .ToArray
        Dim rt_groups = tag_peaks _
            .AsParallel _
            .Select(Function(mz_group)
                        Return mz_group.GroupBy(Function(i) i.peak.rt, offsets:=rt_win).ToArray
                    End Function) _
            .ToArray

        For Each row In rt_groups.IteratesALL
            Dim mzRange As Double() = row.Select(Function(i) i.peak.mz).ToArray
            Dim rtRange As Double() = row _
                .Select(Function(i) {i.peak.rt, i.peak.rtmax, i.peak.rtmin}) _
                .IteratesALL _
                .ToArray
            Dim peakAreas As New Dictionary(Of String, Double)

            For Each sample In row
                If peakAreas.ContainsKey(sample.sample) Then
                    peakAreas(sample.sample) += sample.peak.area
                Else
                    peakAreas(sample.sample) = sample.peak.area
                End If
            Next

            Dim peak As New xcms2 With {
                .mz = stdNum.Round(mzRange.Average, 4),
                .mzmin = mzRange.Min,
                .mzmax = mzRange.Max,
                .rt = stdNum.Round(rtRange.Average),
                .rtmin = stdNum.Round(rtRange.Min),
                .rtmax = stdNum.Round(rtRange.Max),
                .npeaks = row.Length,
                .Properties = peakAreas,
                .ID = $"M{stdNum.Round(.mz)}T{stdNum.Round(.rt)}"
            }

            Yield peak
        Next
    End Function
End Module
