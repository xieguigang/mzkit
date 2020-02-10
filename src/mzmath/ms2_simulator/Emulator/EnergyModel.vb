#Region "Microsoft.VisualBasic::8388eaf6426a9b3511ae692f6ed1cc9d, src\mzmath\ms2_simulator\Emulator\EnergyModel.vb"

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

    ' Class EnergyModel
    ' 
    '     Properties: MaxEnergy, MinEnergy
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: PercentageGreater, PercentageLess, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Calculus
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports stdNum = System.Math

''' <summary>
''' 分子的能量分布模型
''' </summary>
Public Class EnergyModel

    Dim model As ODE
    Dim energy As Sequence

    ''' <summary>
    ''' 分布函数积分总面积 
    ''' </summary>
    Dim totalArea#

    Public ReadOnly Property MinEnergy As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return energy.range.Min
        End Get
    End Property

    Public ReadOnly Property MaxEnergy As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return energy.range.Max
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="dist">
    ''' 能量分布函数:
    ''' 
    ''' ```
    ''' energy = f(x,y)
    ''' ```
    ''' </param>
    ''' <param name="lower">能量积分的下限</param>
    ''' <param name="upper">能量积分的上限</param>
    ''' <param name="n">积分的迭代计算次数</param>
    ''' <param name="y0">初始能量值</param>
    Sub New(dist As df, lower#, upper#, Optional n% = 50000, Optional y0# = 0)
        energy = New Sequence(lower, upper, n)
        model = New ODE With {
            .df = dist,
            .y0 = y0
        }

        ' 积分的最后一个值就是总面积，因为积分的过程就是一个求面积的过程
        totalArea = model _
            .RK4(n, lower, upper) _
            .Y _
            .vector _
            .Last
    End Sub

    Public Function Percentage(min#, max#, nIntervals%) As Double
        Dim lower# = model.RK4(energy.n / nIntervals, MinEnergy, min).Y.vector.Last
        Dim upper# = model.RK4(energy.n / nIntervals, MinEnergy, max).Y.vector.Last

        ' 因为对于递减区间而言
        ' max的计算结果值可能会小于min的计算结果值
        ' 所以在这里需要使用绝对值来消除这个错误
        Return stdNum.Abs(upper - lower) / totalArea
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return energy.ToString
    End Function
End Class
