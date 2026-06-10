Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Namespace PeakAlignment

    ''' <summary>
    ''' 峰对齐参数配置
    ''' </summary>
    Public Class AlignmentParameters
        ''' <summary>m/z容差值</summary>
        Public Property mzTolerance As Double = 0.025
        ''' <summary>m/z容差模式</summary>
        Public Property mzToleranceMode As MassToleranceType = MassToleranceType.Da
        ''' <summary>保留时间容差（秒）</summary>
        Public Property rtTolerance As Double = 30.0
        ''' <summary>对齐算法类型</summary>
        Public Property method As AlignmentMethod = AlignmentMethod.DensityGroup
        ''' <summary>LOESS带宽参数（0~1之间，越大越平滑）</summary>
        Public Property loessSpan As Double = 0.75
        ''' <summary>LOESS多项式阶数（1或2）</summary>
        Public Property loessDegree As Integer = 2
        ''' <summary>参考样本名称（留空则自动选择）</summary>
        Public Property referenceSample As String = ""
        ''' <summary>密度分组核密度带宽（秒），0表示自动估计</summary>
        Public Property densityBandwidth As Double = 0.0
        ''' <summary>特征在样本中出现的最小比例（0~1），低于此比例的特征将被过滤</summary>
        Public Property minFraction As Double = 0.5
        ''' <summary>Obiwarp的RT分段宽度（秒）</summary>
        Public Property obiwarpBinSize As Double = 1.0
        ''' <summary>Obiwarp的gap惩罚值</summary>
        Public Property obiwarpGapPenalty As Double = 0.6
        ''' <summary>Obiwarp的响应强度归一化分段数</summary>
        Public Property obiwarpResponse As Integer = 100
        ''' <summary>是否进行缺失值填充（基于m/z和RT窗口重新搜索）</summary>
        Public Property fillGaps As Boolean = True
    End Class
End Namespace