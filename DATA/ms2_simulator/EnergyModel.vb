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
