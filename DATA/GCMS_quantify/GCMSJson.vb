#Region "Microsoft.VisualBasic::49ad9aa2a9e2c2c7d49d499a0e3e4df3, GCMS_quantify\GCMSJson.vb"

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

' Class GCMSJson
' 
'     Properties: ms, tic, times, title
' 
'     Function: GetTIC
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Class GCMSJson

    Public Property title As String

    ''' <summary>
    ''' 按照从开始到结束升序排序过了的
    ''' </summary>
    ''' <returns></returns>
    Public Property times As Double()
    ''' <summary>
    ''' 相应强度是和<see cref="times"/>一一对应的
    ''' </summary>
    ''' <returns></returns>
    Public Property tic As Double()
    Public Property ms As ms1_scan()()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetTIC() As NamedCollection(Of ChromatogramTick)
        Return New NamedCollection(Of ChromatogramTick) With {
            .Name = title,
            .Value = times _
                .Select(Function(time, i)
                            Return New ChromatogramTick(time, tic(i))
                        End Function) _
                .ToArray
        }
    End Function

    Public Iterator Function GetScanIndex(ROI As DoubleRange) As IEnumerable(Of Integer)

    End Function

    ''' <summary>
    ''' 将某一个时间范围内的<see cref="ms"/>扫描结果都拿出来
    ''' </summary>
    ''' <param name="ROI"></param>
    ''' <returns></returns>
    Public Function GetMsScan(ROI As DoubleRange) As ms1_scan()
        ' 先找到下标的集合

    End Function
End Class

