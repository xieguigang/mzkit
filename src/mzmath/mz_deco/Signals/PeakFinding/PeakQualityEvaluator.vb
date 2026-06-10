Imports std = System.Math

' ============================================================================
' PeakProcessing.vb - 代谢组学XIC色谱图峰提取与峰面积计算算法模块
' ============================================================================
' 本模块实现了代谢组学领域中多种主流的峰提取算法和峰面积计算算法，
' 支持不同算法组合以适应不同数据特征，获得高质量的峰提取结果。
' ============================================================================

Namespace Chromatogram.PeakFinding

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
            Dim asymFactor As Double = roi.asymmetry_factor
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
            If roi.additionals.ContainsKey("gaussian_r2") Then
                Dim r2 As Double = roi.additionals("gaussian_r2")
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
            roi.additionals("quality_score") = score

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
            Return std.Min(1.0, sharpness / 0.6)
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
            Return evaluated.OrderByDescending(Function(p) p.additionals("quality_score")).ToArray()
        End Function

    End Class

End Namespace
