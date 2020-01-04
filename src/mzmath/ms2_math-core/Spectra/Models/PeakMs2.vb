#Region "Microsoft.VisualBasic::9733e15331c7aa1b5c3c49e67f38807b, src\mzmath\ms2_math-core\Spectra\Models\PeakMs2.vb"

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
'         Properties: Ms2Intensity
' 
'         Function: AlignMatrix, RtInSecond, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports stdNum = System.Math

Namespace Spectra

    ''' <summary>
    ''' 将mzXML文件之中的每一个ms2 scan转换而来
    ''' </summary>
    Public Structure PeakMs2

        ''' <summary>
        ''' The precursor ``m/z`` value.
        ''' (一级母离子的``m/z``)
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
        Dim activation As String
        Dim collisionEnergy As Double

        ''' <summary>
        ''' A unique variable name, meaning could be different with <see cref="LibraryMatrix.Name" />. 
        ''' </summary>
        Dim lib_guid As String

        ''' <summary>
        ''' adducts type of the <see cref="mz"/> value.
        ''' </summary>
        Dim precursor_type As String

        ''' <summary>
        ''' 二级碎片信息
        ''' </summary>
        Dim mzInto As LibraryMatrix

        Dim meta As Dictionary(Of String, String)

        ''' <summary>
        ''' 获取得到二级碎片的响应强度值的和,这个响应强度值是和其对应的一级母离子的响应强度值是呈正相关的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Ms2Intensity As Double
            Get
                Return Aggregate mz As ms2
                       In mzInto
                       Into Sum(mz.quantity)
            End Get
        End Property

        ''' <summary>
        ''' 将mzXML文件之中的RT文本解析为以秒为单位的rt保留时间数值
        ''' </summary>
        ''' <param name="rt">诸如``PT508.716S``这样格式的表达式字符串</param>
        ''' <returns></returns>
        Public Shared Function RtInSecond(rt As String) As Double
            rt = rt.Substring(2)
            rt = rt.Substring(0, rt.Length - 1)
            Return Double.Parse(rt)
        End Function

        Public Overrides Function ToString() As String
            Return $"M{stdNum.Round(mz)}T{stdNum.Round(rt)} intensity={Ms2Intensity.ToString("G3")} {file}#{scan}"
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
