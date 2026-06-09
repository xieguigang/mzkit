' ============================================================================
' PeakProcessing.vb - 代谢组学XIC色谱图峰提取与峰面积计算算法模块
' ============================================================================
' 本模块实现了代谢组学领域中多种主流的峰提取算法和峰面积计算算法，
' 支持不同算法组合以适应不同数据特征，获得高质量的峰提取结果。
' ============================================================================

Namespace MetabolomicsPeakProcessing

    ''' <summary>
    ''' 峰检测算法类型枚举
    ''' </summary>
    Public Enum PeakDetectionMethod
        ''' <summary>
        ''' 局部极大值检测法：基于滑动窗口寻找局部强度最大值，
        ''' 配合信噪比阈值过滤，是最基础且广泛使用的峰检测方法。
        ''' 适用于信噪比较高、峰形较为规则的色谱数据。
        ''' </summary>
        LocalMaximum

        ''' <summary>
        ''' CentWave算法（受XCMS启发）：基于连续小波变换(CWT)的峰检测方法，
        ''' 通过墨西哥帽小波在不同尺度上对信号进行卷积，识别具有高斯形状的峰。
        ''' 对重叠峰和宽峰有较好的检测能力，是代谢组学中最常用的算法之一。
        ''' </summary>
        CentWave

        ''' <summary>
        ''' MatchedFilter算法（受XCMS启发）：使用高斯核函数对色谱信号进行卷积滤波，
        ''' 然后在滤波后的信号上检测峰。该方法通过匹配滤波器最大化信噪比，
        ''' 对噪声有较强的抑制能力，适用于低信噪比的数据。
        ''' </summary>
        MatchedFilter

        ''' <summary>
        ''' 一阶导数法：通过计算色谱信号的一阶导数，在导数零交叉点处检测峰，
        ''' 同时利用导数符号变化确定峰的起止边界。该方法对峰边界的确定较为精确，
        ''' 适合于峰形不对称或需要精确边界定义的场景。
        ''' </summary>
        FirstDerivative
    End Enum

    ''' <summary>
    ''' 峰面积计算算法类型枚举
    ''' </summary>
    Public Enum PeakAreaMethod
        ''' <summary>
        ''' 梯形法则积分：将峰区域下的面积用一系列梯形近似，
        ''' 计算简单高效，是色谱峰面积计算的最基本方法。
        ''' 对于等间距采样数据，等价于矩形求和。
        ''' </summary>
        Trapezoidal

        ''' <summary>
        ''' 辛普森法则积分：使用二次多项式对相邻三个数据点进行插值，
        ''' 然后对插值多项式积分，精度高于梯形法则。
        ''' 要求峰区域内至少有3个数据点。
        ''' </summary>
        Simpson

        ''' <summary>
        ''' 高斯曲线拟合：使用高斯函数对峰形进行最小二乘拟合，
        ''' 然后对拟合的高斯曲线进行解析积分计算峰面积。
        ''' 适用于峰形近似高斯分布的理想色谱峰，
        ''' 拟合参数（中心、宽度、高度）可提供额外的峰特征信息。
        ''' </summary>
        GaussianFitting

        ''' <summary>
        ''' 基线校正积分法：先估计峰区域的基线水平，
        ''' 然后从原始信号中扣除基线后使用梯形法则计算峰面积。
        ''' 该方法能有效消除基线漂移对峰面积计算的影响，
        ''' 是实际代谢组学数据处理中最常用的峰面积计算策略。
        ''' </summary>
        BaselineCorrected
    End Enum

    ''' <summary>
    ''' 基线估计算法类型枚举
    ''' </summary>
    Public Enum BaselineMethod
        ''' <summary>
        ''' 线性基线：连接峰起止点的直线作为基线
        ''' </summary>
        Linear

        ''' <summary>
        ''' 最小值基线：使用峰区域内的最小强度值作为基线
        ''' </summary>
        Minimum

        ''' <summary>
        ''' 百分位基线：使用峰区域内指定百分位数的强度值作为基线
        ''' </summary>
        Percentile

        ''' <summary>
        ''' 局部最小值基线：使用峰起止点附近局部最小值的平均作为基线
        ''' </summary>
        LocalMinimum
    End Enum

    ' ========================================================================
    ' 算法参数配置类
    ' ========================================================================

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

    ' ========================================================================
    ' 基线估计器
    ' ========================================================================

    ''' <summary>
    ''' 基线估计器，提供多种基线估计方法
    ''' </summary>
    Public Class BaselineEstimator

        ''' <summary>
        ''' 估计指定峰区域的基线水平
        ''' </summary>
        ''' <param name="ticks">峰区域内的色谱数据</param>
        ''' <param name="method">基线估计方法</param>
        ''' <param name="params">参数配置</param>
        ''' <returns>基线强度值</returns>
        Public Shared Function EstimateBaseline(ticks As ChromatogramTick(),
                                               method As BaselineMethod,
                                               params As PeakAreaParameters) As Double
            If ticks Is Nothing OrElse ticks.Length = 0 Then Return 0.0

            Select Case method
                Case BaselineMethod.Linear
                    Return EstimateLinearBaseline(ticks)
                Case BaselineMethod.Minimum
                    Return EstimateMinimumBaseline(ticks)
                Case BaselineMethod.Percentile
                    Return EstimatePercentileBaseline(ticks, params.BaselinePercentile)
                Case BaselineMethod.LocalMinimum
                    Return EstimateLocalMinimumBaseline(ticks, params.LocalMinimumBoundaryPoints)
                Case Else
                    Return EstimateLinearBaseline(ticks)
            End Select
        End Function

        ''' <summary>
        ''' 线性基线估计：连接峰起止点的直线在峰中心处的值
        ''' 对于基线漂移的情况，线性基线能较好地反映基线趋势
        ''' </summary>
        Private Shared Function EstimateLinearBaseline(ticks As ChromatogramTick()) As Double
            If ticks.Length = 0 Then Return 0.0
            If ticks.Length = 1 Then Return ticks(0).Intensity

            ' 取起始和结束处若干点的平均值作为端点
            Dim nEndpoint As Integer = Math.Max(1, CInt(Math.Floor(ticks.Length * 0.1)))
            nEndpoint = Math.Min(nEndpoint, ticks.Length \ 2)

            Dim startAvg As Double = 0.0
            For i As Integer = 0 To nEndpoint - 1
                startAvg += ticks(i).Intensity
            Next
            startAvg /= nEndpoint

            Dim endAvg As Double = 0.0
            For i As Integer = ticks.Length - nEndpoint To ticks.Length - 1
                endAvg += ticks(i).Intensity
            Next
            endAvg /= nEndpoint

            ' 返回两端点强度的平均值作为基线
            ' 这等效于线性插值在峰中心处的值
            Return (startAvg + endAvg) / 2.0
        End Function

        ''' <summary>
        ''' 最小值基线估计：取峰区域内最低强度值
        ''' 适用于基线平坦且噪声较小的数据
        ''' </summary>
        Private Shared Function EstimateMinimumBaseline(ticks As ChromatogramTick()) As Double
            Dim minIntensity As Double = Double.MaxValue
            For Each tick In ticks
                If tick.Intensity < minIntensity Then
                    minIntensity = tick.Intensity
                End If
            Next
            Return minIntensity
        End Function

        ''' <summary>
        ''' 百分位基线估计：取峰区域内指定百分位数的强度值
        ''' 百分位数的选择取决于数据特征，通常使用5-20百分位
        ''' </summary>
        Private Shared Function EstimatePercentileBaseline(ticks As ChromatogramTick(),
                                                           percentile As Double) As Double
            Dim intensities(ticks.Length - 1) As Double
            For i As Integer = 0 To ticks.Length - 1
                intensities(i) = ticks(i).Intensity
            Next

            Array.Sort(intensities)

            ' 计算百分位数对应的索引
            Dim rank As Double = (percentile / 100.0) * (intensities.Length - 1)
            Dim lowerIndex As Integer = CInt(Math.Floor(rank))
            Dim upperIndex As Integer = CInt(Math.Ceiling(rank))
            lowerIndex = Math.Max(0, Math.Min(lowerIndex, intensities.Length - 1))
            upperIndex = Math.Max(0, Math.Min(upperIndex, intensities.Length - 1))

            If lowerIndex = upperIndex Then
                Return intensities(lowerIndex)
            End If

            ' 线性插值
            Dim fraction As Double = rank - lowerIndex
            Return intensities(lowerIndex) * (1.0 - fraction) + intensities(upperIndex) * fraction
        End Function

        ''' <summary>
        ''' 局部最小值基线估计：取峰起止边界附近局部最小值的平均
        ''' 该方法假设峰的起止点附近信号接近基线水平
        ''' </summary>
        Private Shared Function EstimateLocalMinimumBaseline(ticks As ChromatogramTick(),
                                                             boundaryPoints As Integer) As Double
            If ticks.Length <= boundaryPoints * 2 Then
                Return EstimateMinimumBaseline(ticks)
            End If

            boundaryPoints = Math.Min(boundaryPoints, ticks.Length \ 4)

            ' 起始端局部最小值
            Dim startMin As Double = Double.MaxValue
            For i As Integer = 0 To boundaryPoints - 1
                If ticks(i).Intensity < startMin Then
                    startMin = ticks(i).Intensity
                End If
            Next

            ' 末端局部最小值
            Dim endMin As Double = Double.MaxValue
            For i As Integer = ticks.Length - boundaryPoints To ticks.Length - 1
                If ticks(i).Intensity < endMin Then
                    endMin = ticks(i).Intensity
                End If
            Next

            Return (startMin + endMin) / 2.0
        End Function

    End Class

    ' ========================================================================
    ' 噪声估计器
    ' ========================================================================

    ''' <summary>
    ''' 噪声水平估计器，提供多种噪声估计方法
    ''' </summary>
    Public Class NoiseEstimator

        ''' <summary>
        ''' 使用分段最小值法估计噪声水平
        ''' 将色谱图等分为若干段，取各段最小值的平均作为噪声估计
        ''' 这是一种稳健的噪声估计方法，不受峰信号的影响
        ''' </summary>
        ''' <param name="ticks">完整的色谱图数据</param>
        ''' <param name="segmentCount">分段数量</param>
        ''' <returns>噪声水平估计值</returns>
        Public Shared Function EstimateBySegmentMinima(ticks As ChromatogramTick(),
                                                       segmentCount As Integer) As Double
            If ticks Is Nothing OrElse ticks.Length = 0 Then Return 0.0
            If ticks.Length < segmentCount Then segmentCount = ticks.Length

            Dim segmentSize As Integer = CInt(Math.Ceiling(ticks.Length / CDbl(segmentCount)))
            Dim minValues As New List(Of Double)()

            For seg As Integer = 0 To segmentCount - 1
                Dim startIdx As Integer = seg * segmentSize
                Dim endIdx As Integer = Math.Min(startIdx + segmentSize - 1, ticks.Length - 1)
                If startIdx >= ticks.Length Then Exit For

                Dim segMin As Double = Double.MaxValue
                For i As Integer = startIdx To endIdx
                    If ticks(i).Intensity < segMin Then
                        segMin = ticks(i).Intensity
                    End If
                Next
                If segMin < Double.MaxValue Then
                    minValues.Add(segMin)
                End If
            Next

            If minValues.Count = 0 Then Return 0.0
            Return minValues.Average()
        End Function

        ''' <summary>
        ''' 使用MAD（Median Absolute Deviation）方法估计噪声标准差
        ''' MAD是一种对异常值（峰信号）稳健的离散度估计方法
        ''' </summary>
        ''' <param name="ticks">色谱图数据</param>
        ''' <returns>噪声标准差估计值</returns>
        Public Shared Function EstimateByMAD(ticks As ChromatogramTick()) As Double
            If ticks Is Nothing OrElse ticks.Length < 2 Then Return 0.0

            ' 计算一阶差分（近似导数），去除低频基线漂移
            Dim diffs(ticks.Length - 2) As Double
            For i As Integer = 0 To ticks.Length - 2
                diffs(i) = ticks(i + 1).Intensity - ticks(i).Intensity
            Next

            ' 计算差分的中位数
            Dim sortedDiffs = diffs.ToArray()
            Array.Sort(sortedDiffs)
            Dim median As Double
            Dim mid As Integer = sortedDiffs.Length \ 2
            If sortedDiffs.Length Mod 2 = 0 Then
                median = (sortedDiffs(mid - 1) + sortedDiffs(mid)) / 2.0
            Else
                median = sortedDiffs(mid)
            End If

            ' 计算绝对偏差的中位数
            Dim absDeviations(sortedDiffs.Length - 1) As Double
            For i As Integer = 0 To sortedDiffs.Length - 1
                absDeviations(i) = Math.Abs(sortedDiffs(i) - median)
            Next
            Array.Sort(absDeviations)

            Dim mad As Double
            mid = absDeviations.Length \ 2
            If absDeviations.Length Mod 2 = 0 Then
                mad = (absDeviations(mid - 1) + absDeviations(mid)) / 2.0
            Else
                mad = absDeviations(mid)
            End If

            ' MAD到标准差的转换因子（对于正态分布）
            ' 1.4826 = 1 / Φ^(-1)(3/4)，其中Φ是标准正态分布的CDF
            Return 1.4826 * mad
        End Function

        ''' <summary>
        ''' 使用峰谷差值法估计噪声水平
        ''' 在信号中寻找相邻的局部极大值和极小值，取其差值的平均作为噪声估计
        ''' </summary>
        ''' <param name="ticks">色谱图数据</param>
        ''' <returns>噪声水平估计值</returns>
        Public Shared Function EstimateByPeakValley(ticks As ChromatogramTick()) As Double
            If ticks Is Nothing OrElse ticks.Length < 3 Then Return 0.0

            Dim peakValleyDiffs As New List(Of Double)()

            For i As Integer = 1 To ticks.Length - 2
                ' 检测局部极大值
                If ticks(i).Intensity > ticks(i - 1).Intensity AndAlso
                   ticks(i).Intensity > ticks(i + 1).Intensity Then
                    ' 寻找下一个局部极小值
                    For j As Integer = i + 1 To Math.Min(i + 10, ticks.Length - 2)
                        If ticks(j).Intensity < ticks(j - 1).Intensity AndAlso
                           ticks(j).Intensity < ticks(j + 1).Intensity Then
                            peakValleyDiffs.Add(ticks(i).Intensity - ticks(j).Intensity)
                            Exit For
                        End If
                    Next
                End If
            Next

            If peakValleyDiffs.Count = 0 Then Return 0.0

            ' 取差值的中位数作为噪声估计（比均值更稳健）
            Dim sorted = peakValleyDiffs.ToArray()
            Array.Sort(sorted)
            Dim mid As Integer = sorted.Length \ 2
            If sorted.Length Mod 2 = 0 Then
                Return (sorted(mid - 1) + sorted(mid)) / 2.0
            Else
                Return sorted(mid)
            End If
        End Function

    End Class

    ' ========================================================================
    ' 数学工具类
    ' ========================================================================

    ''' <summary>
    ''' 数学辅助工具类，提供信号处理和数值计算所需的基础函数
    ''' </summary>
    Public Class MathUtils

        ''' <summary>
        ''' 高斯函数
        ''' </summary>
        ''' <param name="x">自变量</param>
        ''' <param name="center">中心位置</param>
        ''' <param name="sigma">标准差</param>
        ''' <param name="height">峰高</param>
        ''' <returns>高斯函数值</returns>
        Public Shared Function Gaussian(x As Double, center As Double,
                                        sigma As Double, height As Double) As Double
            Dim z As Double = (x - center) / sigma
            Return height * Math.Exp(-0.5 * z * z)
        End Function

        ''' <summary>
        ''' 墨西哥帽小波（Ricker Wavelet）函数
        ''' 这是高斯函数的二阶导数，用于CentWave算法
        ''' </summary>
        ''' <param name="x">自变量</param>
        ''' <param name="center">中心位置</param>
        ''' <param name="sigma">尺度参数</param>
        ''' <returns>小波函数值</returns>
        Public Shared Function MexicanHatWavelet(x As Double, center As Double,
                                                 sigma As Double) As Double
            Dim z As Double = (x - center) / sigma
            Dim z2 As Double = z * z
            ' ψ(x) = (1 - x²) * exp(-x²/2) * 2/(√3 * σ * π^(1/4))
            ' 归一化系数可以省略，因为我们只关心相对值
            Return (1.0 - z2) * Math.Exp(-0.5 * z2)
        End Function

        ''' <summary>
        ''' 移动平均平滑
        ''' </summary>
        ''' <param name="data">输入数据</param>
        ''' <param name="windowSize">窗口大小（必须为奇数）</param>
        ''' <returns>平滑后的数据</returns>
        Public Shared Function MovingAverageSmooth(data As Double(), windowSize As Integer) As Double()
            If data Is Nothing OrElse data.Length = 0 Then Return New Double() {}
            If windowSize < 2 Then Return CType(data.Clone(), Double())
            If windowSize Mod 2 = 0 Then windowSize += 1 ' 确保窗口为奇数

            Dim result(data.Length - 1) As Double
            Dim halfW As Integer = windowSize \ 2

            For i As Integer = 0 To data.Length - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0
                For j As Integer = Math.Max(0, i - halfW) To Math.Min(data.Length - 1, i + halfW)
                    sum += data(j)
                    count += 1
                Next
                result(i) = sum / count
            Next

            Return result
        End Function

        ''' <summary>
        ''' Savitzky-Golay平滑（二次多项式，5点窗口的简化实现）
        ''' 相比移动平均，能更好地保留峰形特征
        ''' </summary>
        ''' <param name="data">输入数据</param>
        ''' <returns>平滑后的数据</returns>
        Public Shared Function SavitzkyGolaySmooth(data As Double()) As Double()
            If data Is Nothing OrElse data.Length < 5 Then
                Return If(data Is Nothing, New Double() {}, CType(data.Clone(), Double()))
            End If

            ' 5点二次Savitzky-Golay卷积核
            ' 由最小二乘法拟合二次多项式推导得到
            Dim kernel As Double() = {-0.085714, 0.342857, 0.514286, 0.342857, -0.085714}
            Dim result(data.Length - 1) As Double

            For i As Integer = 0 To data.Length - 1
                Dim sum As Double = 0.0
                For k As Integer = -2 To 2
                    Dim idx As Integer = Math.Max(0, Math.Min(data.Length - 1, i + k))
                    sum += kernel(k + 2) * data(idx)
                Next
                result(i) = sum
            Next

            Return result
        End Function

        ''' <summary>
        ''' 一维卷积运算
        ''' </summary>
        ''' <param name="signal">输入信号</param>
        ''' <param name="kernel">卷积核</param>
        ''' <returns>卷积结果</returns>
        Public Shared Function Convolve(signal As Double(), kernel As Double()) As Double()
            If signal Is Nothing OrElse kernel Is Nothing Then Return New Double() {}
            If signal.Length = 0 OrElse kernel.Length = 0 Then Return New Double() {}

            Dim result(signal.Length - 1) As Double
            Dim halfK As Integer = kernel.Length \ 2

            For i As Integer = 0 To signal.Length - 1
                Dim sum As Double = 0.0
                For k As Integer = 0 To kernel.Length - 1
                    Dim idx As Integer = i - halfK + k
                    If idx >= 0 AndAlso idx < signal.Length Then
                        sum += signal(idx) * kernel(k)
                    End If
                Next
                result(i) = sum
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算一阶导数（中心差分法）
        ''' </summary>
        ''' <param name="data">输入数据</param>
        ''' <param name="dt">采样间隔</param>
        ''' <returns>一阶导数</returns>
        Public Shared Function FirstDerivative(data As Double(), dt As Double) As Double()
            If data Is Nothing OrElse data.Length < 2 Then Return New Double() {}

            Dim deriv(data.Length - 1) As Double

            ' 中心差分（内部点）
            For i As Integer = 1 To data.Length - 2
                deriv(i) = (data(i + 1) - data(i - 1)) / (2.0 * dt)
            Next

            ' 前向差分（第一个点）
            deriv(0) = (data(1) - data(0)) / dt

            ' 后向差分（最后一个点）
            deriv(data.Length - 1) = (data(data.Length - 1) - data(data.Length - 2)) / dt

            Return deriv
        End Function

        ''' <summary>
        ''' 计算二阶导数（中心差分法）
        ''' </summary>
        Public Shared Function SecondDerivative(data As Double(), dt As Double) As Double()
            If data Is Nothing OrElse data.Length < 3 Then Return New Double() {}

            Dim deriv(data.Length - 1) As Double
            Dim dt2 As Double = dt * dt

            ' 中心差分（内部点）
            For i As Integer = 1 To data.Length - 2
                deriv(i) = (data(i + 1) - 2.0 * data(i) + data(i - 1)) / dt2
            Next

            ' 边界处理
            deriv(0) = deriv(1)
            deriv(data.Length - 1) = deriv(data.Length - 2)

            Return deriv
        End Function

        ''' <summary>
        ''' 线性插值
        ''' </summary>
        Public Shared Function LinearInterpolation(x0 As Double, y0 As Double,
                                                    x1 As Double, y1 As Double,
                                                    x As Double) As Double
            If Math.Abs(x1 - x0) < Double.Epsilon Then Return (y0 + y1) / 2.0
            Return y0 + (y1 - y0) * (x - x0) / (x1 - x0)
        End Function

    End Class

    ' ========================================================================
    ' 峰面积计算器
    ' ========================================================================

    ''' <summary>
    ''' 峰面积计算器，提供多种峰面积计算方法
    ''' </summary>
    Public Class PeakAreaCalculator

        ''' <summary>
        ''' 计算峰面积
        ''' </summary>
        ''' <param name="roi">峰ROI对象</param>
        ''' <param name="method">峰面积计算方法</param>
        ''' <param name="params">参数配置</param>
        ''' <returns>计算得到的峰面积</returns>
        Public Shared Function CalculatePeakArea(roi As ROI,
                                                 method As PeakAreaMethod,
                                                 params As PeakAreaParameters) As Double
            If roi Is Nothing OrElse roi.ticks Is Nothing OrElse roi.ticks.Length < 2 Then
                Return 0.0
            End If

            Select Case method
                Case PeakAreaMethod.Trapezoidal
                    Return TrapezoidalIntegration(roi.ticks)

                Case PeakAreaMethod.Simpson
                    Return SimpsonIntegration(roi.ticks)

                Case PeakAreaMethod.GaussianFitting
                    Return GaussianFittingIntegration(roi, params)

                Case PeakAreaMethod.BaselineCorrected
                    Return BaselineCorrectedIntegration(roi, params)

                Case Else
                    Return TrapezoidalIntegration(roi.ticks)
            End Select
        End Function

        ''' <summary>
        ''' 梯形法则积分
        ''' 将相邻数据点之间的面积用梯形近似，然后求和
        ''' 公式：A = Σ (y_i + y_{i+1}) * (x_{i+1} - x_i) / 2
        ''' </summary>
        Private Shared Function TrapezoidalIntegration(ticks As ChromatogramTick()) As Double
            Dim area As Double = 0.0
            For i As Integer = 0 To ticks.Length - 2
                Dim dx As Double = ticks(i + 1).Time - ticks(i).Time
                Dim avgY As Double = (ticks(i).Intensity + ticks(i + 1).Intensity) / 2.0
                area += dx * avgY
            Next
            Return area
        End Function

        ''' <summary>
        ''' 辛普森法则积分
        ''' 使用二次多项式对相邻三个点进行插值，然后积分
        ''' 公式：A = Σ (Δx/3) * (y_{2k} + 4*y_{2k+1} + y_{2k+2})
        ''' 精度为O(Δx^4)，高于梯形法则的O(Δx^2)
        ''' </summary>
        Private Shared Function SimpsonIntegration(ticks As ChromatogramTick()) As Double
            If ticks.Length < 3 Then
                ' 数据点不足3个，退化为梯形法则
                Return TrapezoidalIntegration(ticks)
            End If

            Dim area As Double = 0.0
            Dim n As Integer = ticks.Length - 1 ' 区间数

            ' 如果区间数为奇数，先处理最后一个区间用梯形法则
            Dim simpsonEnd As Integer
            If n Mod 2 = 0 Then
                simpsonEnd = n
            Else
                simpsonEnd = n - 1
                ' 最后一个区间用梯形法则
                Dim dx As Double = ticks(n).Time - ticks(n - 1).Time
                area += dx * (ticks(n - 1).Intensity + ticks(n).Intensity) / 2.0
            End If

            ' 辛普森1/3法则
            For i As Integer = 0 To simpsonEnd - 2 Step 2
                Dim dx As Double = ticks(i + 2).Time - ticks(i).Time
                area += (dx / 6.0) * (ticks(i).Intensity +
                                       4.0 * ticks(i + 1).Intensity +
                                       ticks(i + 2).Intensity)
            Next

            Return area
        End Function

        ''' <summary>
        ''' 高斯曲线拟合积分
        ''' 使用Levenberg-Marquardt算法拟合高斯函数参数，
        ''' 然后对高斯函数进行解析积分：A = height * sigma * √(2π)
        ''' </summary>
        Private Shared Function GaussianFittingIntegration(roi As ROI,
                                                           params As PeakAreaParameters) As Double
            Dim ticks = roi.ticks
            If ticks.Length < 4 Then
                Return TrapezoidalIntegration(ticks)
            End If

            ' 初始参数估计
            Dim center0 As Double = roi.rt
            Dim height0 As Double = roi.maxInto - roi.baseline
            If height0 <= 0 Then height0 = roi.maxInto * 0.5

            ' 估计初始sigma：使用半峰宽法
            ' FWHM ≈ 2.355 * sigma
            Dim halfMax As Double = roi.baseline + height0 / 2.0
            Dim leftTime As Double = roi.time.Min
            Dim rightTime As Double = roi.time.Max

            For i As Integer = 0 To ticks.Length - 2
                If ticks(i).Intensity <= halfMax AndAlso ticks(i + 1).Intensity >= halfMax Then
                    leftTime = MathUtils.LinearInterpolation(
                        ticks(i).Time, ticks(i).Intensity,
                        ticks(i + 1).Time, ticks(i + 1).Intensity, halfMax)
                    Exit For
                End If
            Next

            For i As Integer = ticks.Length - 2 To 0 Step -1
                If ticks(i).Intensity >= halfMax AndAlso ticks(i + 1).Intensity <= halfMax Then
                    rightTime = MathUtils.LinearInterpolation(
                        ticks(i).Time, ticks(i).Intensity,
                        ticks(i + 1).Time, ticks(i + 1).Intensity, halfMax)
                    Exit For
                End If
            Next

            Dim fwhm As Double = rightTime - leftTime
            If fwhm <= 0 Then fwhm = roi.time.Length
            Dim sigma0 As Double = fwhm / 2.355

            ' Levenberg-Marquardt 拟合
            Dim center As Double = center0
            Dim sigma As Double = sigma0
            Dim height As Double = height0
            Dim baseline As Double = roi.baseline

            Dim lambda As Double = 0.001 ' 阻尼因子

            For iter As Integer = 0 To params.GaussianMaxIterations - 1
                ' 计算残差和雅可比矩阵
                Dim residuals(ticks.Length - 1) As Double
                Dim jCenter(ticks.Length - 1) As Double
                Dim jSigma(ticks.Length - 1) As Double
                Dim jHeight(ticks.Length - 1) As Double

                Dim totalError As Double = 0.0

                For i As Integer = 0 To ticks.Length - 1
                    Dim z As Double = (ticks(i).Time - center) / sigma
                    Dim expVal As Double = Math.Exp(-0.5 * z * z)
                    Dim modelVal As Double = baseline + height * expVal

                    residuals(i) = ticks(i).Intensity - modelVal
                    totalError += residuals(i) * residuals(i)

                    ' 雅可比矩阵元素
                    jCenter(i) = height * expVal * z / sigma
                    jSigma(i) = height * expVal * z * z / sigma
                    jHeight(i) = expVal
                Next

                ' 构建正规方程 (J^T * J + λ*I) * δ = J^T * r
                Dim jtj_cc As Double = 0, jtj_cs As Double = 0, jtj_ch As Double = 0
                Dim jtj_ss As Double = 0, jtj_sh As Double = 0, jtj_hh As Double = 0
                Dim jtr_c As Double = 0, jtr_s As Double = 0, jtr_h As Double = 0

                For i As Integer = 0 To ticks.Length - 1
                    jtj_cc += jCenter(i) * jCenter(i)
                    jtj_cs += jCenter(i) * jSigma(i)
                    jtj_ch += jCenter(i) * jHeight(i)
                    jtj_ss += jSigma(i) * jSigma(i)
                    jtj_sh += jSigma(i) * jHeight(i)
                    jtj_hh += jHeight(i) * jHeight(i)
                    jtr_c += jCenter(i) * residuals(i)
                    jtr_s += jSigma(i) * residuals(i)
                    jtr_h += jHeight(i) * residuals(i)
                Next

                ' 加入阻尼
                jtj_cc += lambda * jtj_cc
                jtj_ss += lambda * jtj_ss
                jtj_hh += lambda * jtj_hh

                ' 使用Cramer法则求解3x3线性方程组
                Dim A(,) As Double = {
                    {jtj_cc, jtj_cs, jtj_ch},
                    {jtj_cs, jtj_ss, jtj_sh},
                    {jtj_ch, jtj_sh, jtj_hh}
                }
                Dim b() As Double = {jtr_c, jtr_s, jtr_h}

                Dim detA As Double = A(0, 0) * (A(1, 1) * A(2, 2) - A(1, 2) * A(2, 1)) -
                                     A(0, 1) * (A(1, 0) * A(2, 2) - A(1, 2) * A(2, 0)) +
                                     A(0, 2) * (A(1, 0) * A(2, 1) - A(1, 1) * A(2, 0))

                If Math.Abs(detA) < 1.0E-20 Then Exit For ' 矩阵奇异，退出

                Dim delta(2) As Double
                For col As Integer = 0 To 2
                    Dim M(,) As Double = CType(A.Clone(), Double(,))
                    For row As Integer = 0 To 2
                        M(row, col) = b(row)
                    Next
                    Dim detM As Double = M(0, 0) * (M(1, 1) * M(2, 2) - M(1, 2) * M(2, 1)) -
                                         M(0, 1) * (M(1, 0) * M(2, 2) - M(1, 2) * M(2, 0)) +
                                         M(0, 2) * (M(1, 0) * M(2, 1) - M(1, 1) * M(2, 0))
                    delta(col) = detM / detA
                Next

                ' 更新参数
                Dim newCenter As Double = center + delta(0)
                Dim newSigma As Double = Math.Abs(sigma + delta(1)) ' sigma必须为正
                Dim newHeight As Double = height + delta(2)

                ' 检查收敛
                Dim paramChange As Double = Math.Abs(delta(0)) + Math.Abs(delta(1)) + Math.Abs(delta(2))
                If paramChange < params.GaussianConvergence Then
                    center = newCenter
                    sigma = newSigma
                    height = newHeight
                    Exit For
                End If

                ' 计算新误差以决定是否接受更新
                Dim newError As Double = 0.0
                For i As Integer = 0 To ticks.Length - 1
                    Dim z As Double = (ticks(i).Time - newCenter) / newSigma
                    Dim modelVal As Double = baseline + newHeight * Math.Exp(-0.5 * z * z)
                    Dim r As Double = ticks(i).Intensity - modelVal
                    newError += r * r
                Next

                If newError < totalError Then
                    ' 接受更新，减小阻尼
                    center = newCenter
                    sigma = newSigma
                    height = newHeight
                    lambda *= 0.1
                Else
                    ' 拒绝更新，增大阻尼
                    lambda *= 10.0
                End If

                ' 防止sigma过小
                If sigma < 0.001 Then sigma = 0.001
            Next

            ' 解析积分：A = height * sigma * sqrt(2*pi)
            Dim peakArea As Double = height * sigma * Math.Sqrt(2.0 * Math.PI)

            ' 将拟合参数存入additional字典
            roi.additional("gaussian_center") = center
            roi.additional("gaussian_sigma") = sigma
            roi.additional("gaussian_height") = height
            roi.additional("gaussian_fwhm") = 2.355 * sigma
            roi.additional("gaussian_r2") = CalculateR2(ticks, center, sigma, height, baseline)

            Return peakArea
        End Function

        ''' <summary>
        ''' 计算高斯拟合的R²决定系数
        ''' </summary>
        Private Shared Function CalculateR2(ticks As ChromatogramTick(),
                                            center As Double, sigma As Double,
                                            height As Double, baseline As Double) As Double
            Dim ssTot As Double = 0.0
            Dim ssRes As Double = 0.0
            Dim meanY As Double = 0.0

            For Each tick In ticks
                meanY += tick.Intensity
            Next
            meanY /= ticks.Length

            For Each tick In ticks
                Dim z As Double = (tick.Time - center) / sigma
                Dim predicted As Double = baseline + height * Math.Exp(-0.5 * z * z)
                ssRes += (tick.Intensity - predicted) ^ 2
                ssTot += (tick.Intensity - meanY) ^ 2
            Next

            If ssTot < Double.Epsilon Then Return 1.0
            Return 1.0 - ssRes / ssTot
        End Function

        ''' <summary>
        ''' 基线校正积分法
        ''' 先估计基线，然后从信号中扣除基线后使用梯形法则积分
        ''' 这是最实用的峰面积计算方法，能有效消除基线漂移的影响
        ''' </summary>
        Private Shared Function BaselineCorrectedIntegration(roi As ROI,
                                                              params As PeakAreaParameters) As Double
            Dim ticks = roi.ticks
            If ticks.Length < 2 Then Return 0.0

            ' 估计基线
            Dim baseline As Double = BaselineEstimator.EstimateBaseline(ticks, params.BaselineMethod, params)
            roi.baseline = baseline

            ' 扣除基线后进行梯形积分
            Dim area As Double = 0.0
            For i As Integer = 0 To ticks.Length - 2
                Dim dx As Double = ticks(i + 1).Time - ticks(i).Time
                Dim y0 As Double = Math.Max(0, ticks(i).Intensity - baseline)
                Dim y1 As Double = Math.Max(0, ticks(i + 1).Intensity - baseline)
                area += dx * (y0 + y1) / 2.0
            Next

            Return area
        End Function

    End Class

    ' ========================================================================
    ' 峰检测算法实现
    ' ========================================================================

    ''' <summary>
    ''' 峰检测器，实现多种峰检测算法
    ''' </summary>
    Public Class PeakDetector

        ''' <summary>
        ''' 从XIC色谱图数据中检测峰
        ''' </summary>
        ''' <param name="ticks">XIC色谱图数据</param>
        ''' <param name="method">峰检测方法</param>
        ''' <param name="params">检测参数</param>
        ''' <returns>检测到的ROI列表</returns>
        Public Shared Function DetectPeaks(ticks As ChromatogramTick(),
                                           method As PeakDetectionMethod,
                                           params As PeakDetectionParameters) As List(Of ROI)
            If ticks Is Nothing OrElse ticks.Length < 3 Then Return New List(Of ROI)()

            Select Case method
                Case PeakDetectionMethod.LocalMaximum
                    Return DetectByLocalMaximum(ticks, params)
                Case PeakDetectionMethod.CentWave
                    Return DetectByCentWave(ticks, params)
                Case PeakDetectionMethod.MatchedFilter
                    Return DetectByMatchedFilter(ticks, params)
                Case PeakDetectionMethod.FirstDerivative
                    Return DetectByFirstDerivative(ticks, params)
                Case Else
                    Return DetectByLocalMaximum(ticks, params)
            End Select
        End Function

        ' --------------------------------------------------------------------
        ' 算法1：局部极大值检测法
        ' --------------------------------------------------------------------

        ''' <summary>
        ''' 局部极大值检测法
        ''' 
        ''' 算法原理：
        ''' 1. 使用滑动窗口在色谱图上扫描，寻找局部强度最大值点
        ''' 2. 对每个局部极大值，向两侧扩展寻找峰的起止边界
        '''    边界定义为强度降至峰高一定比例处或开始上升处
        ''' 3. 估计噪声水平，计算信噪比，过滤低信噪比的峰
        ''' 4. 合并距离过近的峰
        ''' 
        ''' 优点：实现简单，计算速度快，适用于信噪比较高的数据
        ''' 缺点：对重叠峰的分辨能力有限，对噪声敏感
        ''' </summary>
        Private Shared Function DetectByLocalMaximum(ticks As ChromatogramTick(),
                                                      params As PeakDetectionParameters) As List(Of ROI)
            Dim results As New List(Of ROI)()

            ' 估计噪声水平
            Dim noiseLevel As Double = NoiseEstimator.EstimateBySegmentMinima(ticks, params.NoiseSegmentCount)
            Dim noiseStd As Double = NoiseEstimator.EstimateByMAD(ticks)
            If noiseStd <= 0 Then noiseStd = noiseLevel * 0.1 + 1.0

            ' 预处理：平滑信号以减少噪声对局部极大值检测的干扰
            Dim intensities(ticks.Length - 1) As Double
            For i As Integer = 0 To ticks.Length - 1
                intensities(i) = ticks(i).Intensity
            Next
            Dim smoothed As Double() = MathUtils.SavitzkyGolaySmooth(intensities)

            Dim halfW As Integer = params.WindowHalfWidth

            ' 步骤1：寻找局部极大值
            Dim peakCandidates As New List(Of Integer)()
            For i As Integer = halfW To ticks.Length - halfW - 1
                Dim isLocalMax As Boolean = True
                For j As Integer = i - halfW To i + halfW
                    If j = i Then Continue For
                    If smoothed(j) >= smoothed(i) Then
                        isLocalMax = False
                        Exit For
                    End If
                Next

                If isLocalMax Then
                    peakCandidates.Add(i)
                End If
            Next

            ' 步骤2：对每个局部极大值确定峰边界
            For Each peakIdx In peakCandidates
                Dim peakIntensity As Double = ticks(peakIdx).Intensity
                Dim peakTime As Double = ticks(peakIdx).Time

                ' 过滤低于最小峰高的候选峰
                If peakIntensity < params.MinPeakHeight Then Continue For

                ' 计算信噪比
                Dim snr As Double = (peakIntensity - noiseLevel) / noiseStd
                If snr < params.SNRThreshold Then Continue For

                ' 向左寻找峰起点
                Dim leftBound As Integer = peakIdx
                Dim threshold As Double = noiseLevel + (peakIntensity - noiseLevel) * 0.05
                For i As Integer = peakIdx - 1 To 0 Step -1
                    If ticks(i).Intensity <= threshold Then
                        leftBound = i
                        Exit For
                    End If
                    ' 如果强度开始上升，说明进入了前一个峰
                    If i > 0 AndAlso ticks(i).Intensity > ticks(i - 1).Intensity AndAlso
                       ticks(i).Intensity < peakIntensity * 0.5 Then
                        leftBound = i
                        Exit For
                    End If
                    leftBound = i
                Next

                ' 向右寻找峰终点
                Dim rightBound As Integer = peakIdx
                For i As Integer = peakIdx + 1 To ticks.Length - 1
                    If ticks(i).Intensity <= threshold Then
                        rightBound = i
                        Exit For
                    End If
                    If i < ticks.Length - 1 AndAlso ticks(i).Intensity > ticks(i + 1).Intensity AndAlso
                       ticks(i).Intensity < peakIntensity * 0.5 Then
                        rightBound = i
                        Exit For
                    End If
                    rightBound = i
                Next

                ' 检查峰宽是否在允许范围内
                Dim peakWidth As Double = ticks(rightBound).Time - ticks(leftBound).Time
                If peakWidth < params.MinPeakWidth OrElse peakWidth > params.MaxPeakWidth Then
                    Continue For
                End If

                ' 创建ROI
                Dim roi As New ROI()
                roi.time = New DoubleRange(ticks(leftBound).Time, ticks(rightBound).Time)
                roi.rt = peakTime
                roi.maxInto = peakIntensity
                roi.baseline = noiseLevel
                roi.snRatio = snr

                ' 提取ROI内的色谱数据
                Dim roiTicks As New List(Of ChromatogramTick)()
                For i As Integer = leftBound To rightBound
                    roiTicks.Add(ticks(i))
                Next
                roi.ticks = roiTicks.ToArray()

                ' 计算噪声面积百分比
                Dim baselineArea As Double = noiseLevel * peakWidth
                Dim totalArea As Double = 0.0
                For i As Integer = 0 To roi.ticks.Length - 2
                    Dim dx As Double = roi.ticks(i + 1).Time - roi.ticks(i).Time
                    totalArea += dx * (roi.ticks(i).Intensity + roi.ticks(i + 1).Intensity) / 2.0
                Next
                roi.noise = If(totalArea > 0, baselineArea / totalArea * 100.0, 0.0)

                ' 附加信息
                roi.additional("peak_width") = peakWidth
                roi.additional("peak_fwhm") = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.additional("asymmetry_factor") = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additional("detection_method") = CDbl(PeakDetectionMethod.LocalMaximum)

                results.Add(roi)
            Next

            ' 步骤3：合并距离过近的峰
            Return MergeClosePeaks(results, params.PeakMergeDistance)
        End Function

        ' --------------------------------------------------------------------
        ' 算法2：CentWave算法（受XCMS启发）
        ' --------------------------------------------------------------------

        ''' <summary>
        ''' CentWave算法（受XCMS启发）
        ''' 
        ''' 算法原理：
        ''' 1. 使用墨西哥帽小波（Ricker Wavelet）在多个尺度上对色谱信号进行连续小波变换(CWT)
        ''' 2. 在CWT系数矩阵中，峰对应的位置会在多个连续尺度上产生显著的系数
        ''' 3. 通过追踪CWT系数的脊线(Ridge Line)，识别具有高斯形状的峰
        ''' 4. 脊线的最大尺度反映了峰的宽度信息
        ''' 5. 根据脊线位置在原始信号上确定峰的精确边界
        ''' 
        ''' 优点：对重叠峰有较好的分辨能力，能检测不同宽度的峰，抗噪声能力强
        ''' 缺点：计算量较大，参数（尺度范围）需要根据数据特征调整
        ''' </summary>
        Private Shared Function DetectByCentWave(ticks As ChromatogramTick(),
                                                  params As PeakDetectionParameters) As List(Of ROI)
            Dim results As New List(Of ROI)()

            ' 估计噪声
            Dim noiseStd As Double = NoiseEstimator.EstimateByMAD(ticks)
            If noiseStd <= 0 Then noiseStd = 1.0
            Dim noiseLevel As Double = NoiseEstimator.EstimateBySegmentMinima(ticks, params.NoiseSegmentCount)

            ' 预处理：平滑信号
            Dim intensities(ticks.Length - 1) As Double
            For i As Integer = 0 To ticks.Length - 1
                intensities(i) = ticks(i).Intensity
            Next
            Dim smoothed As Double() = MathUtils.SavitzkyGolaySmooth(intensities)

            ' 步骤1：计算多尺度CWT系数矩阵
            Dim scales As New List(Of Integer)()
            For s As Integer = params.CentWaveMinScale To params.CentWaveMaxScale Step params.CentWaveScaleStep
                scales.Add(s)
            Next

            If scales.Count = 0 Then scales.Add(params.CentWaveMinScale)

            ' cwtCoeffs(scaleIndex, timeIndex) = CWT系数
            Dim cwtCoeffs(scales.Count - 1, ticks.Length - 1) As Double

            For si As Integer = 0 To scales.Count - 1
                Dim scale As Double = CDbl(scales(si))
                Dim kernelSize As Integer = CInt(scale * 6) + 1 ' 核大小约为6σ
                If kernelSize Mod 2 = 0 Then kernelSize += 1
                Dim halfK As Integer = kernelSize \ 2

                ' 构建墨西哥帽小波核
                Dim kernel(kernelSize - 1) As Double
                For k As Integer = 0 To kernelSize - 1
                    Dim x As Double = (k - halfK) / scale
                    kernel(k) = MathUtils.MexicanHatWavelet(x, 0, 1.0) / Math.Sqrt(scale)
                Next

                ' 卷积计算CWT系数
                For i As Integer = 0 To ticks.Length - 1
                    Dim sum As Double = 0.0
                    For k As Integer = 0 To kernelSize - 1
                        Dim idx As Integer = i - halfK + k
                        If idx >= 0 AndAlso idx < ticks.Length Then
                            sum += smoothed(idx) * kernel(k)
                        End If
                    Next
                    cwtCoeffs(si, i) = sum
                Next
            Next

            ' 步骤2：在每个尺度上寻找CWT系数的局部极大值（候选峰位置）
            Dim ridgeLines As New List(Of List(Of Integer))() ' 每条脊线是一组时间索引

            ' 从最大尺度开始追踪脊线
            For si As Integer = scales.Count - 1 To 0 Step -1
                Dim scale As Integer = scales(si)
                ' 寻找当前尺度上的局部极大值
                For i As Integer = scale To ticks.Length - scale - 1
                    ' 检查是否为局部极大值
                    Dim isMax As Boolean = True
                    Dim winHalf As Integer = Math.Max(1, scale \ 2)
                    For j As Integer = Math.Max(0, i - winHalf) To Math.Min(ticks.Length - 1, i + winHalf)
                        If j <> i AndAlso cwtCoeffs(si, j) >= cwtCoeffs(si, i) Then
                            isMax = False
                            Exit For
                        End If
                    Next

                    If Not isMax Then Continue For
                    If cwtCoeffs(si, i) <= 0 Then Continue For

                    ' 检查是否已有脊线经过此位置
                    Dim belongsToExisting As Boolean = False
                    For Each ridge In ridgeLines
                        If ridge.Count > 0 AndAlso Math.Abs(ridge(ridge.Count - 1) - i) <= params.CentWaveMaxGap Then
                            belongsToExisting = True
                            Exit For
                        End If
                    Next

                    If belongsToExisting Then Continue For

                    ' 开始新的脊线追踪
                    Dim ridge As New List(Of Integer)()
                    ridge.Add(i)

                    ' 向更小尺度追踪
                    Dim currentPos As Integer = i
                    For ssi As Integer = si - 1 To 0 Step -1
                        Dim bestPos As Integer = currentPos
                        Dim bestVal As Double = cwtCoeffs(ssi, currentPos)
                        Dim searchRange As Integer = Math.Max(1, scales(ssi) \ 2)

                        For offset As Integer = -searchRange To searchRange
                            Dim testPos As Integer = currentPos + offset
                            If testPos >= 0 AndAlso testPos < ticks.Length Then
                                If cwtCoeffs(ssi, testPos) > bestVal Then
                                    bestVal = cwtCoeffs(ssi, testPos)
                                    bestPos = testPos
                                End If
                            End If
                        Next

                        If bestVal > 0 Then
                            ridge.Add(bestPos)
                            currentPos = bestPos
                        Else
                            Exit For
                        End If
                    Next

                    ' 脊线必须跨越足够的尺度才被认为是有效的
                    If ridge.Count >= Math.Max(2, scales.Count * 0.3) Then
                        ridgeLines.Add(ridge)
                    End If
                Next
            Next

            ' 步骤3：根据脊线确定峰的位置和边界
            For Each ridge In ridgeLines
                ' 峰中心取脊线中最小尺度对应的位置（精度最高）
                Dim peakIdx As Integer = ridge(ridge.Count - 1)
                If peakIdx < 0 OrElse peakIdx >= ticks.Length Then Continue For

                Dim peakIntensity As Double = ticks(peakIdx).Intensity
                Dim peakTime As Double = ticks(peakIdx).Time

                ' 过滤
                If peakIntensity < params.MinPeakHeight Then Continue For

                Dim snr As Double = (peakIntensity - noiseLevel) / noiseStd
                If snr < params.SNRThreshold Then Continue For

                ' 确定峰边界：从峰中心向两侧扩展，直到信号降至基线附近
                ' 使用脊线的最大尺度作为峰宽的初始估计
                Dim estimatedWidth As Integer = scales(Math.Min(ridge.Count - 1, scales.Count - 1)) * 3

                Dim leftBound As Integer = Math.Max(0, peakIdx - estimatedWidth)
                Dim rightBound As Integer = Math.Min(ticks.Length - 1, peakIdx + estimatedWidth)

                ' 精确化边界：寻找信号降至基线+2σ的位置
                Dim threshold As Double = noiseLevel + 2.0 * noiseStd

                For i As Integer = peakIdx - 1 To Math.Max(0, peakIdx - estimatedWidth * 2) Step -1
                    If ticks(i).Intensity <= threshold Then
                        leftBound = i
                        Exit For
                    End If
                    leftBound = i
                Next

                For i As Integer = peakIdx + 1 To Math.Min(ticks.Length - 1, peakIdx + estimatedWidth * 2)
                    If ticks(i).Intensity <= threshold Then
                        rightBound = i
                        Exit For
                    End If
                    rightBound = i
                Next

                Dim peakWidth As Double = ticks(rightBound).Time - ticks(leftBound).Time
                If peakWidth < params.MinPeakWidth OrElse peakWidth > params.MaxPeakWidth Then
                    Continue For
                End If

                ' 创建ROI
                Dim roi As New ROI()
                roi.time = New DoubleRange(ticks(leftBound).Time, ticks(rightBound).Time)
                roi.rt = peakTime
                roi.maxInto = peakIntensity
                roi.baseline = noiseLevel
                roi.snRatio = snr

                Dim roiTicks As New List(Of ChromatogramTick)()
                For i As Integer = leftBound To rightBound
                    roiTicks.Add(ticks(i))
                Next
                roi.ticks = roiTicks.ToArray()

                ' 噪声面积百分比
                Dim baselineArea As Double = noiseLevel * peakWidth
                Dim totalArea As Double = 0.0
                For i As Integer = 0 To roi.ticks.Length - 2
                    Dim dx As Double = roi.ticks(i + 1).Time - roi.ticks(i).Time
                    totalArea += dx * (roi.ticks(i).Intensity + roi.ticks(i + 1).Intensity) / 2.0
                Next
                roi.noise = If(totalArea > 0, baselineArea / totalArea * 100.0, 0.0)

                ' 附加信息
                roi.additional("peak_width") = peakWidth
                roi.additional("peak_fwhm") = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.additional("asymmetry_factor") = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additional("cwt_ridge_length") = CDbl(ridge.Count)
                roi.additional("cwt_max_scale") = CDbl(scales(Math.Min(ridge.Count - 1, scales.Count - 1)))
                roi.additional("detection_method") = CDbl(PeakDetectionMethod.CentWave)

                results.Add(roi)
            Next

            Return MergeClosePeaks(results, params.PeakMergeDistance)
        End Function

        ' --------------------------------------------------------------------
        ' 算法3：MatchedFilter算法（受XCMS启发）
        ' --------------------------------------------------------------------

        ''' <summary>
        ''' MatchedFilter算法（受XCMS启发）
        ''' 
        ''' 算法原理：
        ''' 1. 使用高斯核函数对色谱信号进行卷积滤波（匹配滤波）
        '''    匹配滤波器在加性高斯白噪声条件下能最大化输出信噪比
        ''' 2. 在滤波后的信号上寻找局部极大值作为峰候选
        ''' 3. 根据滤波后信号的信噪比过滤假阳性峰
        ''' 4. 将滤波后检测到的峰位置映射回原始信号，确定峰边界
        ''' 
        ''' 优点：对噪声有较强的抑制能力，适合低信噪比数据
        ''' 缺点：对峰宽与高斯核不匹配的峰检测效果较差，
        '''       需要合理设置sigma参数以匹配目标峰宽
        ''' </summary>
        Private Shared Function DetectByMatchedFilter(ticks As ChromatogramTick(),
                                                       params As PeakDetectionParameters) As List(Of ROI)
            Dim results As New List(Of ROI)()

            ' 估计噪声
            Dim noiseStd As Double = NoiseEstimator.EstimateByMAD(ticks)
            If noiseStd <= 0 Then noiseStd = 1.0
            Dim noiseLevel As Double = NoiseEstimator.EstimateBySegmentMinima(ticks, params.NoiseSegmentCount)

            ' 步骤1：构建高斯卷积核
            Dim sigma As Double = params.MatchedFilterSigma
            Dim truncate As Double = params.MatchedFilterTruncateWidth
            Dim kernelRadius As Integer = CInt(Math.Ceiling(sigma * truncate))
            Dim kernelSize As Integer = 2 * kernelRadius + 1
            Dim kernel(kernelSize - 1) As Double
            Dim kernelSum As Double = 0.0

            For k As Integer = 0 To kernelSize - 1
                Dim x As Double = (k - kernelRadius)
                kernel(k) = Math.Exp(-0.5 * (x / sigma) ^ 2)
                kernelSum += kernel(k)
            Next

            ' 归一化核
            For k As Integer = 0 To kernelSize - 1
                kernel(k) /= kernelSum
            Next

            ' 步骤2：计算采样间隔（假设近似等间距）
            Dim dt As Double = 1.0
            If ticks.Length > 1 Then
                dt = (ticks(ticks.Length - 1).Time - ticks(0).Time) / (ticks.Length - 1)
            End If

            ' 将sigma转换为数据点数
            Dim sigmaPoints As Integer = CInt(Math.Round(sigma / dt))
            If sigmaPoints < 1 Then sigmaPoints = 1

            ' 步骤3：对信号进行高斯滤波
            Dim intensities(ticks.Length - 1) As Double
            For i As Integer = 0 To ticks.Length - 1
                intensities(i) = ticks(i).Intensity
            Next

            Dim filtered As Double() = MathUtils.Convolve(intensities, kernel)

            ' 步骤4：在滤波后的信号上寻找局部极大值
            Dim halfW As Integer = Math.Max(3, sigmaPoints)
            Dim peakIndices As New List(Of Integer)()

            For i As Integer = halfW To filtered.Length - halfW - 1
                Dim isLocalMax As Boolean = True
                For j As Integer = i - halfW To i + halfW
                    If j = i Then Continue For
                    If j >= 0 AndAlso j < filtered.Length AndAlso filtered(j) >= filtered(i) Then
                        isLocalMax = False
                        Exit For
                    End If
                Next

                If isLocalMax Then
                    peakIndices.Add(i)
                End If
            Next

            ' 步骤5：计算滤波后信号的信噪比并过滤
            Dim filteredNoiseStd As Double = 0.0
            If filtered.Length > 0 Then
                Dim filteredMean As Double = filtered.Average()
                Dim sumSqDiff As Double = 0.0
                For i As Integer = 0 To filtered.Length - 1
                    sumSqDiff += (filtered(i) - filteredMean) ^ 2
                Next
                filteredNoiseStd = Math.Sqrt(sumSqDiff / filtered.Length)
            End If
            If filteredNoiseStd <= 0 Then filteredNoiseStd = 1.0

            For Each peakIdx In peakIndices
                Dim peakIntensity As Double = ticks(peakIdx).Intensity
                Dim peakTime As Double = ticks(peakIdx).Time

                ' 使用原始信号计算SNR
                Dim snr As Double = (peakIntensity - noiseLevel) / noiseStd
                If snr < params.SNRThreshold Then Continue For
                If peakIntensity < params.MinPeakHeight Then Continue For

                ' 确定峰边界
                Dim leftBound As Integer = peakIdx
                Dim rightBound As Integer = peakIdx
                Dim threshold As Double = noiseLevel + 2.0 * noiseStd

                For i As Integer = peakIdx - 1 To Math.Max(0, peakIdx - sigmaPoints * 4) Step -1
                    If ticks(i).Intensity <= threshold Then
                        leftBound = i
                        Exit For
                    End If
                    leftBound = i
                Next

                For i As Integer = peakIdx + 1 To Math.Min(ticks.Length - 1, peakIdx + sigmaPoints * 4)
                    If ticks(i).Intensity <= threshold Then
                        rightBound = i
                        Exit For
                    End If
                    rightBound = i
                Next

                Dim peakWidth As Double = ticks(rightBound).Time - ticks(leftBound).Time
                If peakWidth < params.MinPeakWidth OrElse peakWidth > params.MaxPeakWidth Then
                    Continue For
                End If

                ' 创建ROI
                Dim roi As New ROI()
                roi.time = New DoubleRange(ticks(leftBound).Time, ticks(rightBound).Time)
                roi.rt = peakTime
                roi.maxInto = peakIntensity
                roi.baseline = noiseLevel
                roi.snRatio = snr

                Dim roiTicks As New List(Of ChromatogramTick)()
                For i As Integer = leftBound To rightBound
                    roiTicks.Add(ticks(i))
                Next
                roi.ticks = roiTicks.ToArray()

                Dim baselineArea As Double = noiseLevel * peakWidth
                Dim totalArea As Double = 0.0
                For i As Integer = 0 To roi.ticks.Length - 2
                    Dim dx As Double = roi.ticks(i + 1).Time - roi.ticks(i).Time
                    totalArea += dx * (roi.ticks(i).Intensity + roi.ticks(i + 1).Intensity) / 2.0
                Next
                roi.noise = If(totalArea > 0, baselineArea / totalArea * 100.0, 0.0)

                roi.additional("peak_width") = peakWidth
                roi.additional("peak_fwhm") = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.additional("asymmetry_factor") = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additional("filtered_snr") = (filtered(peakIdx) - filtered.Average()) / filteredNoiseStd
                roi.additional("detection_method") = CDbl(PeakDetectionMethod.MatchedFilter)

                results.Add(roi)
            Next

            Return MergeClosePeaks(results, params.PeakMergeDistance)
        End Function

        ' --------------------------------------------------------------------
        ' 算法4：一阶导数法
        ' --------------------------------------------------------------------

        ''' <summary>
        ''' 一阶导数法
        ''' 
        ''' 算法原理：
        ''' 1. 对色谱信号进行平滑预处理，减少噪声对导数计算的影响
        ''' 2. 计算平滑后信号的一阶导数
        ''' 3. 峰的起点对应导数从负变正的零交叉点（信号从下降转为上升）
        ''' 4. 峰的终点对应导数从正变负的零交叉点（信号从上升转为下降）
        ''' 5. 峰顶对应导数从正变负的零交叉点（信号达到极大值）
        ''' 6. 通过导数阈值和信噪比过滤假阳性峰
        ''' 
        ''' 优点：对峰边界的确定较为精确，适合不对称峰形
        ''' 缺点：对噪声敏感，需要充分的平滑预处理
        ''' </summary>
        Private Shared Function DetectByFirstDerivative(ticks As ChromatogramTick(),
                                                         params As PeakDetectionParameters) As List(Of ROI)
            Dim results As New List(Of ROI)()

            ' 估计噪声
            Dim noiseStd As Double = NoiseEstimator.EstimateByMAD(ticks)
            If noiseStd <= 0 Then noiseStd = 1.0
            Dim noiseLevel As Double = NoiseEstimator.EstimateBySegmentMinima(ticks, params.NoiseSegmentCount)

            ' 步骤1：平滑信号
            Dim intensities(ticks.Length - 1) As Double
            For i As Integer = 0 To ticks.Length - 1
                intensities(i) = ticks(i).Intensity
            Next

            ' 多次平滑以减少噪声
            Dim smoothed As Double() = CType(intensities.Clone(), Double())
            For pass As Integer = 0 To Math.Max(0, params.DerivativeSmoothWindow \ 2)
                smoothed = MathUtils.SavitzkyGolaySmooth(smoothed)
            Next

            ' 步骤2：计算一阶导数
            Dim dt As Double = 1.0
            If ticks.Length > 1 Then
                dt = (ticks(ticks.Length - 1).Time - ticks(0).Time) / (ticks.Length - 1)
            End If
            Dim derivative As Double() = MathUtils.FirstDerivative(smoothed, dt)

            ' 导数阈值：低于此阈值的导数被视为零（平坦区域）
            Dim maxDeriv As Double = 0.0
            For Each d In derivative
                If Math.Abs(d) > maxDeriv Then maxDeriv = Math.Abs(d)
            Next
            Dim derivThreshold As Double = maxDeriv * params.DerivativeThresholdFactor

            ' 步骤3：寻找导数零交叉点
            ' 正零交叉（导数从负变正）：峰谷/峰起点
            ' 负零交叉（导数从正变负）：峰顶
            Dim negativeCrossings As New List(Of Integer)() ' 峰顶位置
            Dim positiveCrossings As New List(Of Integer)() ' 峰谷位置

            For i As Integer = 0 To derivative.Length - 2
                ' 忽略接近零的导数
                If Math.Abs(derivative(i)) < derivThreshold AndAlso
                   Math.Abs(derivative(i + 1)) < derivThreshold Then
                    Continue For
                End If

                ' 负零交叉：导数从正变负 → 峰顶
                If derivative(i) > derivThreshold AndAlso derivative(i + 1) < -derivThreshold Then
                    ' 精确化峰顶位置：在i和i+1之间找原始信号的最大值
                    Dim maxIdx As Integer = i
                    Dim maxVal As Double = smoothed(i)
                    For j As Integer = i To Math.Min(i + 3, smoothed.Length - 1)
                        If smoothed(j) > maxVal Then
                            maxVal = smoothed(j)
                            maxIdx = j
                        End If
                    Next
                    negativeCrossings.Add(maxIdx)
                End If

                ' 正零交叉：导数从负变正 → 峰谷
                If derivative(i) < -derivThreshold AndAlso derivative(i + 1) > derivThreshold Then
                    positiveCrossings.Add(i)
                End If
            Next

            ' 步骤4：根据零交叉点确定峰区域
            ' 每个峰顶（负零交叉）对应一个峰，其边界为相邻的峰谷（正零交叉）
            For Each peakIdx In negativeCrossings
                If peakIdx < 0 OrElse peakIdx >= ticks.Length Then Continue For

                Dim peakIntensity As Double = ticks(peakIdx).Intensity
                Dim peakTime As Double = ticks(peakIdx).Time

                If peakIntensity < params.MinPeakHeight Then Continue For

                Dim snr As Double = (peakIntensity - noiseLevel) / noiseStd
                If snr < params.SNRThreshold Then Continue For

                ' 寻找左侧最近的峰谷作为峰起点
                Dim leftBound As Integer = 0
                For Each valleyIdx In positiveCrossings
                    If valleyIdx < peakIdx Then
                        leftBound = valleyIdx
                    Else
                        Exit For
                    End If
                Next

                ' 寻找右侧最近的峰谷作为峰终点
                Dim rightBound As Integer = ticks.Length - 1
                For Each valleyIdx In positiveCrossings
                    If valleyIdx > peakIdx Then
                        rightBound = valleyIdx
                        Exit For
                    End If
                Next

                ' 确保边界合理
                If leftBound >= peakIdx Then leftBound = Math.Max(0, peakIdx - CInt(params.MaxPeakWidth / dt / 2))
                If rightBound <= peakIdx Then rightBound = Math.Min(ticks.Length - 1, peakIdx + CInt(params.MaxPeakWidth / dt / 2))

                Dim peakWidth As Double = ticks(rightBound).Time - ticks(leftBound).Time
                If peakWidth < params.MinPeakWidth OrElse peakWidth > params.MaxPeakWidth Then
                    Continue For
                End If

                ' 创建ROI
                Dim roi As New ROI()
                roi.time = New DoubleRange(ticks(leftBound).Time, ticks(rightBound).Time)
                roi.rt = peakTime
                roi.maxInto = peakIntensity
                roi.baseline = noiseLevel
                roi.snRatio = snr

                Dim roiTicks As New List(Of ChromatogramTick)()
                For i As Integer = leftBound To rightBound
                    roiTicks.Add(ticks(i))
                Next
                roi.ticks = roiTicks.ToArray()

                Dim baselineArea As Double = noiseLevel * peakWidth
                Dim totalArea As Double = 0.0
                For i As Integer = 0 To roi.ticks.Length - 2
                    Dim dx As Double = roi.ticks(i + 1).Time - roi.ticks(i).Time
                    totalArea += dx * (roi.ticks(i).Intensity + roi.ticks(i + 1).Intensity) / 2.0
                Next
                roi.noise = If(totalArea > 0, baselineArea / totalArea * 100.0, 0.0)

                roi.additional("peak_width") = peakWidth
                roi.additional("peak_fwhm") = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.additional("asymmetry_factor") = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additional("derivative_max") = maxDeriv
                roi.additional("detection_method") = CDbl(PeakDetectionMethod.FirstDerivative)

                results.Add(roi)
            Next

            Return MergeClosePeaks(results, params.PeakMergeDistance)
        End Function

        ' --------------------------------------------------------------------
        ' 辅助方法
        ' --------------------------------------------------------------------

        ''' <summary>
        ''' 估计半峰宽(FWHM)
        ''' 通过线性插值找到强度降至峰高一半处的时间点
        ''' </summary>
        Private Shared Function EstimateFWHM(ticks As ChromatogramTick(),
                                              peakIntensity As Double,
                                              baseline As Double) As Double
            If ticks Is Nothing OrElse ticks.Length < 3 Then Return 0.0

            Dim halfMax As Double = baseline + (peakIntensity - baseline) / 2.0
            Dim leftTime As Double = ticks(0).Time
            Dim rightTime As Double = ticks(ticks.Length - 1).Time

            ' 寻找左侧半高点
            For i As Integer = 0 To ticks.Length - 2
                If ticks(i).Intensity <= halfMax AndAlso ticks(i + 1).Intensity >= halfMax Then
                    leftTime = MathUtils.LinearInterpolation(
                        ticks(i).Time, ticks(i).Intensity,
                        ticks(i + 1).Time, ticks(i + 1).Intensity, halfMax)
                    Exit For
                End If
            Next

            ' 寻找右侧半高点
            For i As Integer = ticks.Length - 2 To 0 Step -1
                If ticks(i).Intensity >= halfMax AndAlso ticks(i + 1).Intensity <= halfMax Then
                    rightTime = MathUtils.LinearInterpolation(
                        ticks(i).Time, ticks(i).Intensity,
                        ticks(i + 1).Time, ticks(i + 1).Intensity, halfMax)
                    Exit For
                End If
            Next

            Return rightTime - leftTime
        End Function

        ''' <summary>
        ''' 估计峰的不对称因子(Asymmetry Factor, As)
        ''' As = b/a，其中a为峰顶到10%峰高处的左侧时间距离，
        ''' b为峰顶到10%峰高处的右侧时间距离
        ''' As = 1 表示对称峰，As > 1 表示拖尾峰，As < 1 表示前伸峰
        ''' </summary>
        Private Shared Function EstimateAsymmetryFactor(ticks As ChromatogramTick(),
                                                         peakOffset As Integer,
                                                         peakIntensity As Double,
                                                         baseline As Double) As Double
            If ticks Is Nothing OrElse ticks.Length < 5 Then Return 1.0
            If peakOffset < 0 OrElse peakOffset >= ticks.Length Then Return 1.0

            Dim tenPercentMax As Double = baseline + (peakIntensity - baseline) * 0.1
            Dim peakTime As Double = ticks(peakOffset).Time

            ' 左侧10%峰高点
            Dim leftTime As Double = ticks(0).Time
            For i As Integer = peakOffset - 1 To 0 Step -1
                If ticks(i).Intensity <= tenPercentMax Then
                    If i < peakOffset Then
                        leftTime = MathUtils.LinearInterpolation(
                            ticks(i).Time, ticks(i).Intensity,
                            ticks(i + 1).Time, ticks(i + 1).Intensity, tenPercentMax)
                    End If
                    Exit For
                End If
                leftTime = ticks(i).Time
            Next

            ' 右侧10%峰高点
            Dim rightTime As Double = ticks(ticks.Length - 1).Time
            For i As Integer = peakOffset + 1 To ticks.Length - 1
                If ticks(i).Intensity <= tenPercentMax Then
                    If i > 0 Then
                        rightTime = MathUtils.LinearInterpolation(
                            ticks(i - 1).Time, ticks(i - 1).Intensity,
                            ticks(i).Time, ticks(i).Intensity, tenPercentMax)
                    End If
                    Exit For
                End If
                rightTime = ticks(i).Time
            Next

            Dim a As Double = peakTime - leftTime
            Dim b As Double = rightTime - peakTime

            If a <= 0 Then Return 1.0
            Return b / a
        End Function

        ''' <summary>
        ''' 合并距离过近的峰
        ''' 当两个峰的保留时间差小于指定距离时，保留强度较高的峰
        ''' </summary>
        Private Shared Function MergeClosePeaks(peaks As List(Of ROI),
                                                 mergeDistance As Double) As List(Of ROI)
            If peaks.Count <= 1 Then Return peaks

            ' 按保留时间排序
            Dim sorted = peaks.OrderBy(Function(p) p.rt).ToList()

            Dim merged As New List(Of ROI)()
            Dim i As Integer = 0

            While i < sorted.Count
                Dim current As ROI = sorted(i)
                Dim j As Integer = i + 1

                ' 查找需要合并的峰
                While j < sorted.Count AndAlso
                      Math.Abs(sorted(j).rt - current.rt) < mergeDistance
                    ' 保留强度较高的峰
                    If sorted(j).maxInto > current.maxInto Then
                        current = sorted(j)
                    End If
                    j += 1
                End While

                merged.Add(current)
                i = j
            End While

            Return merged
        End Function

    End Class

    ' ========================================================================
    ' 主处理器类
    ' ========================================================================

    ''' <summary>
    ''' 峰处理主处理器，整合峰检测和峰面积计算
    ''' 
    ''' 使用示例：
    '''   Dim processor As New PeakProcessor()
    '''   processor.DetectionMethod = PeakDetectionMethod.CentWave
    '''   processor.AreaMethod = PeakAreaMethod.BaselineCorrected
    '''   
    '''   Dim peaks = processor.ProcessPeaks(xicTicks)
    '''   
    '''   For Each peak In peaks
    '''       Console.WriteLine($"RT={peak.rt:F2}s, Area={peak.peakarea:F2}, SNR={peak.snRatio:F1}")
    '''   Next
    ''' </summary>
    Public Class PeakProcessor

        ''' <summary>
        ''' 峰检测方法
        ''' </summary>
        Public Property DetectionMethod As PeakDetectionMethod = PeakDetectionMethod.CentWave

        ''' <summary>
        ''' 峰面积计算方法
        ''' </summary>
        Public Property AreaMethod As PeakAreaMethod = PeakAreaMethod.BaselineCorrected

        ''' <summary>
        ''' 基线估计方法
        ''' </summary>
        Public Property BaselineMethod As BaselineMethod = BaselineMethod.Linear

        ''' <summary>
        ''' 峰检测参数
        ''' </summary>
        Public Property DetectionParams As New PeakDetectionParameters()

        ''' <summary>
        ''' 峰面积计算参数
        ''' </summary>
        Public Property AreaParams As New PeakAreaParameters()

        ''' <summary>
        ''' 是否在峰面积计算后重新计算信噪比
        ''' </summary>
        Public Property RecalculateSNR As Boolean = True

        ''' <summary>
        ''' 处理XIC色谱图数据，执行峰检测和峰面积计算
        ''' </summary>
        ''' <param name="ticks">XIC色谱图数据（ChromatogramTick数组）</param>
        ''' <returns>检测到的峰列表（ROI数组）</returns>
        Public Function ProcessPeaks(ticks As ChromatogramTick()) As ROI()
            Return ProcessPeaks(ticks, DetectionMethod, AreaMethod, DetectionParams, AreaParams)
        End Function

        ''' <summary>
        ''' 使用指定算法和参数处理XIC色谱图数据
        ''' </summary>
        ''' <param name="ticks">XIC色谱图数据</param>
        ''' <param name="detectionMethod">峰检测方法</param>
        ''' <param name="areaMethod">峰面积计算方法</param>
        ''' <param name="detectionParams">峰检测参数</param>
        ''' <param name="areaParams">峰面积计算参数</param>
        ''' <returns>检测到的峰列表</returns>
        Public Function ProcessPeaks(ticks As ChromatogramTick(),
                                     detectionMethod As PeakDetectionMethod,
                                     areaMethod As PeakAreaMethod,
                                     detectionParams As PeakDetectionParameters,
                                     areaParams As PeakAreaParameters) As ROI()
            If ticks Is Nothing OrElse ticks.Length < 3 Then
                Return New ROI() {}
            End If

            ' 步骤1：峰检测
            Dim peaks As List(Of ROI) = PeakDetector.DetectPeaks(ticks, detectionMethod, detectionParams)

            If peaks.Count = 0 Then Return New ROI() {}

            ' 步骤2：计算总TIC面积（用于计算积分百分比）
            Dim totalTICArea As Double = 0.0
            For i As Integer = 0 To ticks.Length - 2
                Dim dx As Double = ticks(i + 1).Time - ticks(i).Time
                totalTICArea += dx * (ticks(i).Intensity + ticks(i + 1).Intensity) / 2.0
            Next

            ' 步骤3：对每个峰计算基线和峰面积
            For Each peak In peaks
                ' 估计基线
                peak.baseline = BaselineEstimator.EstimateBaseline(peak.ticks, BaselineMethod, areaParams)

                ' 计算峰面积
                peak.peakarea = PeakAreaCalculator.CalculatePeakArea(peak, areaMethod, areaParams)

                ' 计算积分百分比
                If totalTICArea > 0 Then
                    peak.integration = peak.peakarea / totalTICArea * 100.0
                End If

                ' 重新计算信噪比（基于基线校正后的峰高）
                If RecalculateSNR Then
                    Dim correctedHeight As Double = peak.maxInto - peak.baseline
                    Dim noiseStd As Double = NoiseEstimator.EstimateByMAD(peak.ticks)
                    If noiseStd > 0 Then
                        peak.snRatio = correctedHeight / noiseStd
                    End If
                End If

                ' 计算噪声面积百分比
                Dim peakBaselineArea As Double = peak.baseline * peak.time.Length
                If peak.peakarea > 0 Then
                    peak.noise = peakBaselineArea / (peak.peakarea + peakBaselineArea) * 100.0
                End If

                ' 添加通用附加信息
                peak.additional("baseline_method") = CDbl(BaselineMethod)
                peak.additional("area_method") = CDbl(areaMethod)
                peak.additional("detection_method_name") = CDbl(detectionMethod)
                peak.additional("tick_count") = CDbl(peak.ticks.Length)
                peak.additional("time_range") = peak.time.Length
            Next

            ' 步骤4：过滤信噪比不达标的峰（面积计算后SNR可能变化）
            Dim filteredPeaks = peaks.Where(Function(p) p.snRatio >= detectionParams.SNRThreshold).ToList()

            Return filteredPeaks.ToArray()
        End Function

        ''' <summary>
        ''' 使用多种峰检测算法进行组合检测，取结果的并集
        ''' 适用于不确定哪种算法最适合当前数据的情况
        ''' 当不同算法检测到同一位置的峰时，保留信噪比最高的结果
        ''' </summary>
        ''' <param name="ticks">XIC色谱图数据</param>
        ''' <param name="methods">要组合使用的峰检测方法列表</param>
        ''' <param name="mergeDistance">峰合并距离（秒）</param>
        ''' <returns>合并后的峰列表</returns>
        Public Function ProcessPeaksCombined(ticks As ChromatogramTick(),
                                             methods As PeakDetectionMethod(),
                                             mergeDistance As Double) As ROI()
            If ticks Is Nothing OrElse ticks.Length < 3 Then Return New ROI() {}
            If methods Is Nothing OrElse methods.Length = 0 Then
                Return ProcessPeaks(ticks)
            End If

            Dim allPeaks As New List(Of ROI)()

            ' 使用每种方法分别检测
            For Each method In methods
                Dim peaks = PeakDetector.DetectPeaks(ticks, method, DetectionParams)
                allPeaks.AddRange(peaks)
            Next

            If allPeaks.Count = 0 Then Return New ROI() {}

            ' 计算总TIC面积
            Dim totalTICArea As Double = 0.0
            For i As Integer = 0 To ticks.Length - 2
                Dim dx As Double = ticks(i + 1).Time - ticks(i).Time
                totalTICArea += dx * (ticks(i).Intensity + ticks(i + 1).Intensity) / 2.0
            Next

            ' 对每个峰计算基线和峰面积
            For Each peak In allPeaks
                peak.baseline = BaselineEstimator.EstimateBaseline(peak.ticks, BaselineMethod, AreaParams)
                peak.peakarea = PeakAreaCalculator.CalculatePeakArea(peak, AreaMethod, AreaParams)
                If totalTICArea > 0 Then
                    peak.integration = peak.peakarea / totalTICArea * 100.0
                End If
                If RecalculateSNR Then
                    Dim correctedHeight As Double = peak.maxInto - peak.baseline
                    Dim noiseStd As Double = NoiseEstimator.EstimateByMAD(peak.ticks)
                    If noiseStd > 0 Then
                        peak.snRatio = correctedHeight / noiseStd
                    End If
                End If
                Dim peakBaselineArea As Double = peak.baseline * peak.time.Length
                If peak.peakarea > 0 Then
                    peak.noise = peakBaselineArea / (peak.peakarea + peakBaselineArea) * 100.0
                End If
                peak.additional("tick_count") = CDbl(peak.ticks.Length)
            Next

            ' 按保留时间排序
            allPeaks = allPeaks.OrderBy(Function(p) p.rt).ToList()

            ' 合并重叠或距离过近的峰，保留SNR最高的
            Dim merged As New List(Of ROI)()
            Dim i As Integer = 0

            While i < allPeaks.Count
                Dim current As ROI = allPeaks(i)
                Dim j As Integer = i + 1

                While j < allPeaks.Count AndAlso
                      Math.Abs(allPeaks(j).rt - current.rt) < mergeDistance
                    ' 保留信噪比更高的峰
                    If allPeaks(j).snRatio > current.snRatio Then
                        current = allPeaks(j)
                    End If
                    j += 1
                End While

                ' 过滤低SNR峰
                If current.snRatio >= DetectionParams.SNRThreshold Then
                    merged.Add(current)
                End If

                i = j
            End While

            Return merged.ToArray()
        End Function

        ''' <summary>
        ''' 对已检测到的峰ROI列表进行峰面积重新计算
        ''' 可用于更换峰面积计算方法而不重新检测峰
        ''' </summary>
        ''' <param name="peaks">已检测到的峰列表</param>
        ''' <param name="ticks">完整的XIC色谱图数据（用于计算TIC总面积）</param>
        ''' <param name="areaMethod">新的峰面积计算方法</param>
        ''' <returns>更新了峰面积的峰列表</returns>
        Public Function RecalculatePeakAreas(peaks As ROI(),
                                              ticks As ChromatogramTick(),
                                              areaMethod As PeakAreaMethod) As ROI()
            If peaks Is Nothing OrElse peaks.Length = 0 Then Return peaks

            ' 计算总TIC面积
            Dim totalTICArea As Double = 0.0
            If ticks IsNot Nothing AndAlso ticks.Length >= 2 Then
                For i As Integer = 0 To ticks.Length - 2
                    Dim dx As Double = ticks(i + 1).Time - ticks(i).Time
                    totalTICArea += dx * (ticks(i).Intensity + ticks(i + 1).Intensity) / 2.0
                Next
            End If

            For Each peak In peaks
                ' 重新估计基线
                peak.baseline = BaselineEstimator.EstimateBaseline(peak.ticks, BaselineMethod, AreaParams)

                ' 使用新方法计算峰面积
                peak.peakarea = PeakAreaCalculator.CalculatePeakArea(peak, areaMethod, AreaParams)

                ' 更新积分百分比
                If totalTICArea > 0 Then
                    peak.integration = peak.peakarea / totalTICArea * 100.0
                End If

                ' 更新噪声面积百分比
                Dim peakBaselineArea As Double = peak.baseline * peak.time.Length
                If peak.peakarea > 0 Then
                    peak.noise = peakBaselineArea / (peak.peakarea + peakBaselineArea) * 100.0
                End If

                peak.additional("area_method") = CDbl(areaMethod)
            Next

            Return peaks
        End Function

    End Class

    ' ========================================================================
    ' 峰质量评估器
    ' ========================================================================

    ''' <summary>
    ''' 峰质量评估器，提供多种峰质量评估指标
    ''' 用于筛选和评估峰提取结果的质量
    ''' </summary>
    Public Class PeakQualityEvaluator

        ''' <summary>
        ''' 评估单个峰的质量得分
        ''' 综合考虑峰形对称性、信噪比、高斯拟合优度等指标
        ''' </summary>
        ''' <param name="roi">待评估的峰</param>
        ''' <returns>质量得分（0-100），越高表示峰质量越好</returns>
        Public Shared Function EvaluatePeakQuality(roi As ROI) As Double
            If roi Is Nothing OrElse roi.ticks Is Nothing OrElse roi.ticks.Length < 3 Then
                Return 0.0
            End If

            Dim score As Double = 0.0

            ' 1. 信噪比得分（权重30%）
            ' SNR > 10 得满分，SNR < 3 得0分
            Dim snrScore As Double
            If roi.snRatio >= 10.0 Then
                snrScore = 30.0
            ElseIf roi.snRatio < 3.0 Then
                snrScore = 0.0
            Else
                snrScore = (roi.snRatio - 3.0) / 7.0 * 30.0
            End If
            score += snrScore

            ' 2. 峰形对称性得分（权重25%）
            ' 对称因子在0.8-1.5之间得满分
            Dim asymFactor As Double = 1.0
            If roi.additional.ContainsKey("asymmetry_factor") Then
                asymFactor = roi.additional("asymmetry_factor")
            End If

            Dim asymScore As Double
            If asymFactor >= 0.8 AndAlso asymFactor <= 1.5 Then
                asymScore = 25.0
            ElseIf asymFactor < 0.5 OrElse asymFactor > 3.0 Then
                asymScore = 0.0
            Else
                ' 线性衰减
                If asymFactor < 0.8 Then
                    asymScore = (asymFactor - 0.5) / 0.3 * 25.0
                Else
                    asymScore = (3.0 - asymFactor) / 1.5 * 25.0
                End If
            End If
            score += asymScore

            ' 3. 高斯拟合优度得分（权重25%）
            Dim r2Score As Double = 0.0
            If roi.additional.ContainsKey("gaussian_r2") Then
                Dim r2 As Double = roi.additional("gaussian_r2")
                If r2 >= 0.95 Then
                    r2Score = 25.0
                ElseIf r2 < 0.5 Then
                    r2Score = 0.0
                Else
                    r2Score = (r2 - 0.5) / 0.45 * 25.0
                End If
            Else
                ' 未进行高斯拟合，使用峰形尖锐度评估
                Dim peakSharpness As Double = CalculatePeakSharpness(roi)
                If peakSharpness >= 0.7 Then
                    r2Score = 20.0
                ElseIf peakSharpness < 0.3 Then
                    r2Score = 5.0
                Else
                    r2Score = 5.0 + (peakSharpness - 0.3) / 0.4 * 15.0
                End If
            End If
            score += r2Score

            ' 4. 噪声面积占比得分（权重20%）
            ' 噪声占比越低越好
            Dim noiseScore As Double
            If roi.noise <= 10.0 Then
                noiseScore = 20.0
            ElseIf roi.noise >= 50.0 Then
                noiseScore = 0.0
            Else
                noiseScore = (50.0 - roi.noise) / 40.0 * 20.0
            End If
            score += noiseScore

            ' 保存质量得分
            roi.additional("quality_score") = score

            Return score
        End Function

        ''' <summary>
        ''' 计算峰的尖锐度（峰高与峰宽的归一化比值）
        ''' 尖锐度越高，峰形越好
        ''' </summary>
        Private Shared Function CalculatePeakSharpness(roi As ROI) As Double
            If roi.ticks Is Nothing OrElse roi.ticks.Length < 3 Then Return 0.0

            Dim correctedHeight As Double = roi.maxInto - roi.baseline
            If correctedHeight <= 0 Then Return 0.0

            ' 计算峰面积与矩形面积的比值
            ' 完美高斯峰的比值为 sqrt(2*pi*e) / (2*sqrt(2*pi)) ≈ 0.58
            Dim rectangleArea As Double = correctedHeight * roi.time.Length
            If rectangleArea <= 0 Then Return 0.0

            Dim sharpness As Double = roi.peakarea / rectangleArea

            ' 归一化到0-1范围
            Return Math.Min(1.0, sharpness / 0.6)
        End Function

        ''' <summary>
        ''' 批量评估峰质量，并按质量得分排序
        ''' </summary>
        ''' <param name="peaks">峰列表</param>
        ''' <param name="minQualityScore">最低质量得分阈值</param>
        ''' <returns>按质量排序的峰列表</returns>
        Public Shared Function EvaluateAndFilter(peaks As ROI(),
                                                  minQualityScore As Double) As ROI()
            If peaks Is Nothing OrElse peaks.Length = 0 Then Return New ROI() {}

            Dim evaluated As New List(Of ROI)()
            For Each peak In peaks
                Dim score As Double = EvaluatePeakQuality(peak)
                If score >= minQualityScore Then
                    evaluated.Add(peak)
                End If
            Next

            ' 按质量得分降序排序
            Return evaluated.OrderByDescending(Function(p) p.additional("quality_score")).ToArray()
        End Function

    End Class

End Namespace
