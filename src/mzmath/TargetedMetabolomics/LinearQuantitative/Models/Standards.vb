﻿#Region "Microsoft.VisualBasic::dbb34fc8bfe2744df4eb35f6b2a15c6d, mzmath\TargetedMetabolomics\LinearQuantitative\Models\Standards.vb"

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

    '   Total Lines: 71
    '    Code Lines: 34 (47.89%)
    ' Comment Lines: 31 (43.66%)
    '    - Xml Docs: 96.77%
    ' 
    '   Blank Lines: 6 (8.45%)
    '     File Size: 2.54 KB


    '     Class Standards
    ' 
    '         Properties: [IS], C, Factor, ID, ISTD
    '                     Name
    ' 
    '         Function: GetLevelKeys, PopulateLevels, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace LinearQuantitative

    ''' <summary>
    ''' The standard curve concentration gradient data.
    ''' </summary>
    ''' <remarks>
    ''' (标准品的标准曲线的浓度梯度信息)
    ''' </remarks>
    Public Class Standards : Implements INamedValue

        ''' <summary>
        ''' the reference id of the compound
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String Implements INamedValue.Key
        ''' <summary>
        ''' display name of the target compound
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 标准曲线的浓度梯度信息
        ''' </summary>
        ''' <returns></returns>
        Public Property C As Dictionary(Of String, Double)
        ''' <summary>
        ''' the is td name 
        ''' </summary>
        ''' <returns></returns>
        Public Property ISTD As String
        ''' <summary>
        ''' 内标的编号，需要使用这个编号来分别找到离子对和浓度信息
        ''' </summary>
        ''' <returns></returns>
        Public Property [IS] As String
        ''' <summary>
        ''' 系数因子，值位于[0,1]区间，默认是1
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <[DefaultValue](1)>
        <[Default](1)>
        Public Property Factor As Double

        Public Overrides Function ToString() As String
            Return $"[{[IS]}] {ID}: {Name}, {C.GetJson}"
        End Function

        Public Iterator Function PopulateLevels() As IEnumerable(Of Double)
            For Each lv In C.OrderBy(Function(ci) Integer.Parse(ci.Key.Match("\d+")))
                Yield lv.Value
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetLevelKeys(standards As IEnumerable(Of Standards)) As IEnumerable(Of String)
            Return standards _
                .Select(Function(c) c.C.Keys) _
                .IteratesALL _
                .Distinct _
                .OrderBy(Function(k) Integer.Parse(k.Match("\d+")))
        End Function
    End Class
End Namespace
