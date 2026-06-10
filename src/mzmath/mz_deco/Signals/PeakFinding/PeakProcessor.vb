Imports System.Runtime.CompilerServices
Imports std = System.Math

Namespace Chromatogram.PeakFinding

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
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ProcessPeaks(ticks As ChromatogramTick()) As ROI()
            Return ProcessPeaks(ticks,
                                DetectionMethod,
                                AreaMethod,
                                DetectionParams,
                                AreaParams,
                                BaselineMethod,
                                RecalculateSNR)
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
        Public Shared Function ProcessPeaks(ticks As ChromatogramTick(),
                                            detectionMethod As PeakDetectionMethod,
                                            areaMethod As PeakAreaMethod,
                                            detectionParams As PeakDetectionParameters,
                                            areaParams As PeakAreaParameters,
                                            baselineMethod As BaselineMethod,
                                            recalculateSNR As Boolean) As ROI()

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
                peak.baseline = BaselineEstimator.EstimateBaseline(peak.ticks, baselineMethod, areaParams)

                ' 计算峰面积
                peak.peakarea = PeakAreaCalculator.CalculatePeakArea(peak, areaMethod, areaParams)

                ' 计算积分百分比
                If totalTICArea > 0 Then
                    peak.integration = peak.peakarea / totalTICArea * 100.0
                End If

                ' 重新计算信噪比（基于基线校正后的峰高）
                If recalculateSNR Then
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
                peak.additionals("tick_count") = CDbl(peak.ticks.Length)
                peak.additionals("time_range") = peak.time.Length
            Next

            ' 步骤4：过滤信噪比不达标的峰（面积计算后SNR可能变化）
            Dim filteredPeaks = peaks.Where(Function(p) p.snRatio >= detectionParams.SNRThreshold).ToList()

            For Each roi As ROI In filteredPeaks
                roi.additionals("quality_score") = PeakQualityEvaluator.EvaluatePeakQuality(roi)
            Next

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
                peak.additionals("tick_count") = CDbl(peak.ticks.Length)
            Next

            ' 按保留时间排序
            allPeaks = allPeaks.OrderBy(Function(p) p.rt).ToList()

            ' 合并重叠或距离过近的峰，保留SNR最高的
            Dim merged As New List(Of ROI)()
            Dim idx As Integer = 0

            While idx < allPeaks.Count
                Dim current As ROI = allPeaks(idx)
                Dim j As Integer = idx + 1

                While j < allPeaks.Count AndAlso std.Abs(allPeaks(j).rt - current.rt) < mergeDistance
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

                idx = j
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
        Public Function RecalculatePeakAreas(peaks As ROI(), ticks As ChromatogramTick(), areaMethod As PeakAreaMethod) As ROI()
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
            Next

            Return peaks
        End Function

    End Class

End Namespace