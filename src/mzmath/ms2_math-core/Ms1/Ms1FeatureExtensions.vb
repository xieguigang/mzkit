#Region "Microsoft.VisualBasic::7ed6bda26ef5f6d5f05052915d007c32, src\mzmath\ms2_math-core\Ms1\Ms1FeatureExtensions.vb"

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

' Module Ms1FeatureExtensions
' 
'     Function: peakGroup
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports stdNum = System.Math

<HideModuleName>
Public Module Ms1FeatureExtensions

    ''' <summary>
    ''' 从xcms程序包所生成的格式为``MaTb``的uid中产生一个选择的区间
    ''' </summary>
    ''' <param name="mzrt"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 利用得到的编号列表从另外的一个物质表中选取对应的feature
    ''' </remarks>
    <Extension>
    Public Function peakGroup(mzrt As String, Optional dt% = 5) As NamedValue(Of Index(Of String))
        Dim mz = mzrt.Match("M\d+")
        Dim rt = mzrt.Match("T\d+")

        If rt.StringEmpty Then
            ' 当前的mz只有一个物质的peak
            Return New NamedValue(Of Index(Of String)) With {
                .Name = mzrt,
                .Value = {mzrt}
            }
        Else
            rt = rt.Match("\d+")
        End If

        Dim rtInt As Integer = rt.ParseInteger
        Dim uidGroup = Iterator Function() As IEnumerable(Of String)
                           For t As Integer = rtInt - dt To rtInt + dt
                               Yield $"{mz}T{t}"
                           Next
                       End Function

        Return New NamedValue(Of Index(Of String)) With {
            .Name = mzrt,
            .Value = uidGroup().ToArray
        }
    End Function

    ''' <summary>
    ''' reorder scan points into a sequence for downstream data analysis
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="seq"></param>
    ''' <param name="mzwidth"></param>
    ''' <param name="rtwin"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function SequenceOrder(Of T As IMs1)(seq As IEnumerable(Of T), mzwidth As Tolerance, rtwin As Double) As IEnumerable(Of T)
        Dim mzgroup As NamedCollection(Of T)() = seq _
            .GroupBy(Function(mzi) mzi.mz, mzwidth) _
            .OrderBy(Function(a) Val(a.name)) _
            .ToArray

        For Each mzBlock As NamedCollection(Of T) In mzgroup
            For Each block As NamedCollection(Of T) In mzBlock _
                .AsEnumerable _
                .GroupBy(Function(i) i.rt, Function(x, y) stdNum.Abs(x - y) <= rtwin) _
                .OrderBy(Function(a) Val(a.name))

                For Each ROI In block.OrderBy(Function(a) a.rt)
                    Yield ROI
                Next
            Next
        Next
    End Function
End Module
