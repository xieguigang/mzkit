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

    Sub New(dist As df, lower#, upper#, Optional n% = 10000, Optional y0# = 0)
        model = New ODE With {.df = dist, .y0 = y0}
        energy = New Sequence(lower, upper, n)
        ' 积分的最后一个值就是总面积，因为积分的过程就是一个求面积的过程
        totalArea = model.RK4(n, lower, upper).Y.Vector.Last
    End Sub

    ''' <summary>
    ''' 求出大于或者等于指定的能量值的概率的百分比
    ''' </summary>
    ''' <param name="energy#"></param>
    ''' <returns></returns>
    Public Function Percentage(energy#) As Double
        Dim y0 = model.df(energy, 0)
        Dim area# = New ODE With {
            .df = model.df,
            .y0 = y0
        }.RK4(Me.energy.n, energy, Me.energy.Max).Y.Vector.Last

        Return area / totalArea
    End Function
End Class
