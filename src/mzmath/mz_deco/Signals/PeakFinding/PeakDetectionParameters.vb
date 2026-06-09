Namespace Chromatogram.PeakFinding

    ''' <summary>
    ''' 峰检测参数配置
    ''' </summary>
    Public Class PeakDetectionParameters

        ''' <summary>
        ''' 信噪比阈值，低于此阈值的峰将被过滤。默认值为3.0。
        ''' 在代谢组学中，通常要求SNR >= 3才认为峰是可靠的。
        ''' </summary>
        Public Property SNRThreshold As Double = 3.0

        ''' <summary>
        ''' 峰检测的滑动窗口半宽（以数据点数计）。默认值为5。
        ''' 窗口越大，检测到的峰越宽；窗口越小，对窄峰更敏感。
        ''' </summary>
        Public Property WindowHalfWidth As Integer = 5

        ''' <summary>
        ''' 最小峰宽（秒）。默认值为3.0秒。
        ''' 保留时间宽度小于此值的峰将被过滤。
        ''' </summary>
        Public Property MinPeakWidth As Double = 3.0

        ''' <summary>
        ''' 最大峰宽（秒）。默认值为30.0秒。
        ''' 保留时间宽度大于此值的峰将被过滤。
        ''' </summary>
        Public Property MaxPeakWidth As Double = 30.0

        ''' <summary>
        ''' 最小峰高度阈值。默认值为0。
        ''' 强度低于此值的峰将被过滤。
        ''' </summary>
        Public Property MinPeakHeight As Double = 0.0

        ''' <summary>
        ''' CentWave算法：小波变换的最小尺度。默认值为1。
        ''' 对应于最窄的可检测峰宽度。
        ''' </summary>
        Public Property CentWaveMinScale As Integer = 1

        ''' <summary>
        ''' CentWave算法：小波变换的最大尺度。默认值为20。
        ''' 对应于最宽的可检测峰宽度。
        ''' </summary>
        Public Property CentWaveMaxScale As Integer = 20

        ''' <summary>
        ''' CentWave算法：小波变换尺度步进。默认值为1。
        ''' </summary>
        Public Property CentWaveScaleStep As Integer = 1

        ''' <summary>
        ''' CentWave算法：峰连接的最大间隙（数据点数）。默认值为2。
        ''' 在CWT脊线追踪中，允许跨过的最大间隙。
        ''' </summary>
        Public Property CentWaveMaxGap As Integer = 2

        ''' <summary>
        ''' MatchedFilter算法：高斯核的标准差（秒）。默认值为3.0。
        ''' 应与目标峰的宽度相匹配。
        ''' </summary>
        Public Property MatchedFilterSigma As Double = 3.0

        ''' <summary>
        ''' MatchedFilter算法：高斯核的截断宽度（标准差倍数）。默认值为4.0。
        ''' 核函数在 ±sigma*TruncateWidth 范围外截断为零。
        ''' </summary>
        Public Property MatchedFilterTruncateWidth As Double = 4.0

        ''' <summary>
        ''' 一阶导数法：导数平滑窗口大小。默认值为3。
        ''' 在计算导数前对信号进行移动平均平滑的窗口大小。
        ''' </summary>
        Public Property DerivativeSmoothWindow As Integer = 3

        ''' <summary>
        ''' 一阶导数法：导数阈值因子。默认值为0.01。
        ''' 导数绝对值小于 (max_derivative * factor) 的点被视为平坦区域。
        ''' </summary>
        Public Property DerivativeThresholdFactor As Double = 0.01

        ''' <summary>
        ''' 噪声估计所使用的分段数。默认值为20。
        ''' 将色谱图等分为N段，取各段最小值的平均作为噪声水平估计。
        ''' </summary>
        Public Property NoiseSegmentCount As Integer = 20

        ''' <summary>
        ''' 峰合并距离（秒）。默认值为1.0秒。
        ''' 两个峰的保留时间差小于此值时，将合并为一个峰。
        ''' </summary>
        Public Property PeakMergeDistance As Double = 1.0

    End Class

End Namespace