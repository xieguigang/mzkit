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
Imports Microsoft.VisualBasic.Math.SignalProcessing.COW
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
    Public Function PickReferenceSample(samples As IEnumerable(Of NamedCollection(Of PeakFeature))) As NamedCollection(Of PeakFeature)

    End Function

    ''' <summary>
    ''' Make peak alignment via COW alignment algorithm.
    ''' </summary>
    ''' <param name="samples">the peak collection for each sample file, a sample </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function CreateMatrix(samples As IEnumerable(Of NamedCollection(Of PeakFeature))) As IEnumerable(Of xcms2)
        Dim cow As New CowAlignment(Of PeakFeature)(AddressOf CreatePeak)
        Dim rawdata = samples.ToArray
        Dim refer = rawdata.PickReferenceSample
        Dim targets = rawdata.Where(Function(sample) sample.name <> refer.name).ToArray

        For Each sample As NamedCollection(Of PeakFeature) In targets
            Dim aligns = cow.CorrelationOptimizedWarping(0, 0, 0, refer.AsList, sample.AsList, BorderLimit.Gaussian)

        Next
    End Function
End Module
