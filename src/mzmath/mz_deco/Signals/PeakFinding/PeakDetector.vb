Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports std = System.Math

Namespace Chromatogram.PeakFinding


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
                roi.peak_width = peakWidth
                roi.peak_fwhm = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.asymmetry_factor = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)

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
                    kernel(k) = MathUtils.MexicanHatWavelet(x, 0, 1.0) / std.Sqrt(scale)
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
                    Dim winHalf As Integer = std.Max(1, scale \ 2)
                    For j As Integer = std.Max(0, i - winHalf) To std.Min(ticks.Length - 1, i + winHalf)
                        If j <> i AndAlso cwtCoeffs(si, j) >= cwtCoeffs(si, i) Then
                            isMax = False
                            Exit For
                        End If
                    Next

                    If Not isMax Then Continue For
                    If cwtCoeffs(si, i) <= 0 Then Continue For

                    ' 检查是否已有脊线经过此位置
                    Dim belongsToExisting As Boolean = False
                    For Each ridge_line In ridgeLines
                        If ridge_line.Count > 0 AndAlso std.Abs(ridge_line(ridge_line.Count - 1) - i) <= params.CentWaveMaxGap Then
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
                        Dim searchRange As Integer = std.Max(1, scales(ssi) \ 2)

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
                    If ridge.Count >= std.Max(2, scales.Count * 0.3) Then
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
                Dim estimatedWidth As Integer = scales(std.Min(ridge.Count - 1, scales.Count - 1)) * 3

                Dim leftBound As Integer = std.Max(0, peakIdx - estimatedWidth)
                Dim rightBound As Integer = std.Min(ticks.Length - 1, peakIdx + estimatedWidth)

                ' 精确化边界：寻找信号降至基线+2σ的位置
                Dim threshold As Double = noiseLevel + 2.0 * noiseStd

                For i As Integer = peakIdx - 1 To std.Max(0, peakIdx - estimatedWidth * 2) Step -1
                    If ticks(i).Intensity <= threshold Then
                        leftBound = i
                        Exit For
                    End If
                    leftBound = i
                Next

                For i As Integer = peakIdx + 1 To std.Min(ticks.Length - 1, peakIdx + estimatedWidth * 2)
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
                roi.peak_width = peakWidth
                roi.peak_fwhm = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.asymmetry_factor = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additionals("cwt_ridge_length") = CDbl(ridge.Count)
                roi.additionals("cwt_max_scale") = CDbl(scales(std.Min(ridge.Count - 1, scales.Count - 1)))

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
            Dim kernelRadius As Integer = CInt(std.Ceiling(sigma * truncate))
            Dim kernelSize As Integer = 2 * kernelRadius + 1
            Dim kernel(kernelSize - 1) As Double
            Dim kernelSum As Double = 0.0

            For k As Integer = 0 To kernelSize - 1
                Dim x As Double = (k - kernelRadius)
                kernel(k) = std.Exp(-0.5 * (x / sigma) ^ 2)
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
            Dim sigmaPoints As Integer = CInt(std.Round(sigma / dt))
            If sigmaPoints < 1 Then sigmaPoints = 1

            ' 步骤3：对信号进行高斯滤波
            Dim intensities(ticks.Length - 1) As Double
            For i As Integer = 0 To ticks.Length - 1
                intensities(i) = ticks(i).Intensity
            Next

            Dim filtered As Double() = MathUtils.Convolve(intensities, kernel)

            ' 步骤4：在滤波后的信号上寻找局部极大值
            Dim halfW As Integer = std.Max(3, sigmaPoints)
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
                filteredNoiseStd = std.Sqrt(sumSqDiff / filtered.Length)
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

                For i As Integer = peakIdx - 1 To std.Max(0, peakIdx - sigmaPoints * 4) Step -1
                    If ticks(i).Intensity <= threshold Then
                        leftBound = i
                        Exit For
                    End If
                    leftBound = i
                Next

                For i As Integer = peakIdx + 1 To std.Min(ticks.Length - 1, peakIdx + sigmaPoints * 4)
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

                roi.peak_width = peakWidth
                roi.peak_fwhm = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.asymmetry_factor = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additionals("filtered_snr") = (filtered(peakIdx) - filtered.Average()) / filteredNoiseStd

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
            For pass As Integer = 0 To std.Max(0, params.DerivativeSmoothWindow \ 2)
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
                If std.Abs(d) > maxDeriv Then maxDeriv = std.Abs(d)
            Next
            Dim derivThreshold As Double = maxDeriv * params.DerivativeThresholdFactor

            ' 步骤3：寻找导数零交叉点
            ' 正零交叉（导数从负变正）：峰谷/峰起点
            ' 负零交叉（导数从正变负）：峰顶
            Dim negativeCrossings As New List(Of Integer)() ' 峰顶位置
            Dim positiveCrossings As New List(Of Integer)() ' 峰谷位置

            For i As Integer = 0 To derivative.Length - 2
                ' 忽略接近零的导数
                If std.Abs(derivative(i)) < derivThreshold AndAlso
                    std.Abs(derivative(i + 1)) < derivThreshold Then
                    Continue For
                End If

                ' 负零交叉：导数从正变负 → 峰顶
                If derivative(i) > derivThreshold AndAlso derivative(i + 1) < -derivThreshold Then
                    ' 精确化峰顶位置：在i和i+1之间找原始信号的最大值
                    Dim maxIdx As Integer = i
                    Dim maxVal As Double = smoothed(i)
                    For j As Integer = i To std.Min(i + 3, smoothed.Length - 1)
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
                If leftBound >= peakIdx Then leftBound = std.Max(0, peakIdx - CInt(params.MaxPeakWidth / dt / 2))
                If rightBound <= peakIdx Then rightBound = std.Min(ticks.Length - 1, peakIdx + CInt(params.MaxPeakWidth / dt / 2))

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

                roi.peak_width = peakWidth
                roi.peak_fwhm = EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.asymmetry_factor = EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additionals("derivative_max") = maxDeriv

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
        ''' As = 1 表示对称峰，As > 1 表示拖尾峰，As &lt; 1 表示前伸峰
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
                While j < sorted.Count AndAlso std.Abs(sorted(j).rt - current.rt) < mergeDistance
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

End Namespace