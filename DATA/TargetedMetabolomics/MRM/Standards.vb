#Region "Microsoft.VisualBasic::b5b94bb0fa736149e242a0145a6800b3, math.MRM\Standards.vb"

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

    '     Class Standards
    ' 
    '         Properties: [IS], C, Factor, HMDB, ISTD
    '                     Name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MRM.Models

    ''' <summary>
    ''' The standard curve
    ''' </summary>
    Public Class Standards

        Public Property HMDB As String
        Public Property Name As String
        ''' <summary>
        ''' 标准曲线的浓度梯度信息
        ''' </summary>
        ''' <returns></returns>
        Public Property C As Dictionary(Of String, Double)
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
            Return $"[{[IS]}] {HMDB}: {Name}, {C.GetJson}"
        End Function
    End Class
End Namespace
