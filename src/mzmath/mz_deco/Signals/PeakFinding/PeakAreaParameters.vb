Namespace Chromatogram.PeakFinding

    ''' <summary>
    ''' 峰面积计算参数配置
    ''' </summary>
    Public Class PeakAreaParameters

        ''' <summary>
        ''' 基线估计方法
        ''' </summary>
        Public Property BaselineMethod As BaselineMethod = BaselineMethod.Linear

        ''' <summary>
        ''' 百分位基线法使用的百分位数（0-100）。默认值为10。
        ''' </summary>
        Public Property BaselinePercentile As Double = 10.0

        ''' <summary>
        ''' 局部最小值基线法中，边界区域的数据点数。默认值为5。
        ''' </summary>
        Public Property LocalMinimumBoundaryPoints As Integer = 5

        ''' <summary>
        ''' 高斯拟合的最大迭代次数。默认值为100。
        ''' </summary>
        Public Property GaussianMaxIterations As Integer = 100

        ''' <summary>
        ''' 高斯拟合的收敛阈值。默认值为1e-6。
        ''' </summary>
        Public Property GaussianConvergence As Double = 0.000001

    End Class
End Namespace