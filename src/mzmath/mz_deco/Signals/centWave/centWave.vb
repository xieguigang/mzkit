Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports std = System.Math

Namespace Chromatogram.PeakFinding

    Public Module centWave

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
        Public Function DetectByCentWave(ticks As ChromatogramTick(), params As PeakDetectionParameters) As List(Of ROI)
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
                roi.peak_fwhm = PeakDetector.EstimateFWHM(roi.ticks, peakIntensity, noiseLevel)
                roi.asymmetry_factor = PeakDetector.EstimateAsymmetryFactor(roi.ticks, peakIdx - leftBound, peakIntensity, noiseLevel)
                roi.additionals("cwt_ridge_length") = CDbl(ridge.Count)
                roi.additionals("cwt_max_scale") = CDbl(scales(std.Min(ridge.Count - 1, scales.Count - 1)))

                results.Add(roi)
            Next

            Return PeakDetector.MergeClosePeaks(results, params.PeakMergeDistance)
        End Function

    End Module
End Namespace