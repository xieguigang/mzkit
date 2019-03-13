#Region "Microsoft.VisualBasic::829d51a7a7516ed07f6209237ee7da8f, ms2_math-core\Spectra\PeakMs2.vb"

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

    '     Structure PeakMs2
    ' 
    '         Function: AlignMatrix, RtInSecond
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Spectra

    ''' <summary>
    ''' 将mzXML文件之中的每一个ms2 scan转换而来
    ''' </summary>
    Public Structure PeakMs2

        ''' <summary>
        ''' 一级母离子的``m/z``
        ''' </summary>
        Dim mz As Double
        ''' <summary>
        ''' 一级母离子的出峰时间
        ''' </summary>
        Dim rt As Double
        ''' <summary>
        ''' 原始数据文件名
        ''' </summary>
        Dim file As String
        ''' <summary>
        ''' 数据扫描编号
        ''' </summary>
        Dim scan As Integer
        ''' <summary>
        ''' 二级碎片信息
        ''' </summary>
        Dim mzInto As LibraryMatrix

        Public Shared Function RtInSecond(rt As String) As Double
            rt = rt.Substring(2)
            rt = rt.Substring(0, rt.Length - 1)
            Return Double.Parse(rt)
        End Function

        ''' <summary>
        ''' 当前的这个<see cref="PeakMs2"/>如果在<paramref name="ref"/>找不到对应的``m/z``
        ''' 则对应的部分的into为零
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AlignMatrix(ref As ms2(), tolerance As Tolerance) As ms2()
            Return mzInto.ms2.AlignMatrix(ref, tolerance)
        End Function
    End Structure
End Namespace
