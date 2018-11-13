#Region "Microsoft.VisualBasic::118b7c93c6079c83fda2eda1f0de07e5, ms2_math-core\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: RetentionIndex, ToTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToTable(ROIlist As IEnumerable(Of ROI), Optional getTitle As Func(Of ROI, String) = Nothing) As ROITable()
        Return ROIlist _
            .SafeQuery _
            .Select(Function(ROI, i)
                        Return ROI _
                            .GetROITable(Function(region)
                                             If getTitle Is Nothing Then
                                                 Return "#" & (i + 1)
                                             Else
                                                 Return getTitle(region)
                                             End If
                                         End Function)
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 根据保留时间来计算出保留指数
    ''' </summary>
    ''' <param name="rt"></param>
    ''' <param name="A"></param>
    ''' <param name="B"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RetentionIndex(rt As IRetentionTime, A As (rt#, ri#), B As (rt#, ri#)) As Double
        Dim rtScale = (rt.rt - A.rt) / (B.rt - A.rt)
        Dim riScale = (B.ri - A.ri) * rtScale
        Dim ri = A.ri + riScale
        Return ri
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GroupByMz(mz As IEnumerable(Of ms1_scan), Optional tolerance As Tolerance = Nothing) As ms1_scan()
        Return ms1_scan.GroupByMz(mz, tolerance)
    End Function
End Module
