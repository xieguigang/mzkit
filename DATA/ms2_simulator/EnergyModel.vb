#Region "Microsoft.VisualBasic::f20d73ab7dc0153d5db89c3a8cab1105, ms2_simulator\EnergyModel.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Math.Calculus

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
            Return energy.Min
        End Get
    End Property

    Public ReadOnly Property MaxEnergy As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return energy.Max
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
            .Vector _
            .Last
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return energy.ToString
    End Function

    ''' <summary>
    ''' 求出大于或者等于指定的能量值的概率的百分比
    ''' </summary>
    ''' <param name="energy#"></param>
    ''' <returns></returns>
    Public Function PercentageGreater(energy#) As Double
        Dim integrate As ODEOutput = model _
            .RK4(Me.energy.n, energy, Me.energy.Max)
        Dim area = integrate _
            .Y _
            .Vector _
            .Last

        ' Call integrate.DataFrame.Save($"./{energy}.csv", Encodings.ASCII)

        Return area / totalArea
    End Function

    ''' <summary>
    ''' 求出小于或者等于指定的能量值的概率的百分比
    ''' </summary>
    ''' <param name="energy#"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PercentageLess(energy#) As Double
        Return 1 - PercentageGreater(energy)
    End Function
End Class
