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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing.COW

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
    ''' Make peak alignment via COW alignment algorithm.
    ''' </summary>
    ''' <param name="samples">the peak collection for each sample file, a sample </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateMatrix(samples As IEnumerable(Of NamedCollection(Of PeakFeature))) As IEnumerable(Of xcms2)
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
                .npeaks = 0,
                .Properties = New Dictionary(Of String, Double) From {
                    {refer.name, point.area}
                }
            })
        Next

        For Each sample As NamedCollection(Of PeakFeature) In targets
            Dim aligns = cow.CorrelationOptimizedWarping(refer.AsList, sample.AsList).ToArray

            For Each point As PeakFeature In aligns
                Call peaktable(point.xcms_id).Add(sample.name, point.area)
            Next
        Next

        Return peaktable.Values
    End Function
End Module
