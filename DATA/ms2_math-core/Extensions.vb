#Region "Microsoft.VisualBasic::9dc653ec473428c258076f8161d375d3, ms2_math-core\Extensions.vb"

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
    '     Function: CreateLibraryMatrix, GroupByMz, RetentionIndex, Trim
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.MassSpectrum.Math.Ms1
Imports SMRUCC.MassSpectrum.Math.Spectra

Public Module Extensions

    ''' <summary>
    ''' 将响应强度低于一定值的碎片进行删除
    ''' </summary>
    ''' <param name="library"></param>
    ''' <param name="intoCutoff">相对相应强度的删除阈值, 值范围为``[0, 1]``</param>
    ''' <returns></returns>
    <Extension>
    Public Function Trim(ByRef library As LibraryMatrix, intoCutoff#) As LibraryMatrix
        library = library / library.Max
        library = library(library!intensity >= intoCutoff)
        library = library * 100

        Return library
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

    ''' <summary>
    ''' 在一定的误差范围内按照m/z对碎片进行分组操作，并取出该分组内的信号响应值最大值作为该分组的信号响应
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GroupByMz(mz As IEnumerable(Of ms1_scan), Optional tolerance As Tolerance = Nothing) As ms1_scan()
        Return ms1_scan.GroupByMz(mz, tolerance)
    End Function

    ''' <summary>
    ''' 这个拓展适用于``GC/MS``的标准品图谱，当然也适用于LC/MS的时间窗口采样结果
    ''' </summary>
    ''' <param name="fragments"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateLibraryMatrix(fragments As IEnumerable(Of ms1_scan), Optional name$ = "GC/MS Mass Scan") As LibraryMatrix
        Dim ms2 = fragments _
            .SafeQuery _
            .Select(Function(scan)
                        Return New ms2 With {
                            .mz = scan.mz,
                            .intensity = scan.intensity,
                            .quantity = scan.intensity
                        }
                    End Function) _
            .ToArray

        Return New LibraryMatrix With {
            .ms2 = ms2,
            .Name = name
        }
    End Function
End Module
