Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports std = System.Math

Namespace Chromatogram.PeakFinding

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
                    Dim expVal As Double = std.Exp(-0.5 * z * z)
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

                If std.Abs(detA) < 1.0E-20 Then Exit For ' 矩阵奇异，退出

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
                Dim newSigma As Double = std.Abs(sigma + delta(1)) ' sigma必须为正
                Dim newHeight As Double = height + delta(2)

                ' 检查收敛
                Dim paramChange As Double = std.Abs(delta(0)) + std.Abs(delta(1)) + std.Abs(delta(2))
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
                    Dim modelVal As Double = baseline + newHeight * std.Exp(-0.5 * z * z)
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
            Dim peakArea As Double = height * sigma * std.Sqrt(2.0 * std.PI)

            ' 将拟合参数存入additional字典
            roi.additionals("gaussian_center") = center
            roi.additionals("gaussian_sigma") = sigma
            roi.additionals("gaussian_height") = height
            roi.additionals("gaussian_fwhm") = 2.355 * sigma
            roi.additionals("gaussian_r2") = CalculateR2(ticks, center, sigma, height, baseline)

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
                Dim predicted As Double = baseline + height * std.Exp(-0.5 * z * z)
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
                Dim y0 As Double = std.Max(0, ticks(i).Intensity - baseline)
                Dim y1 As Double = std.Max(0, ticks(i + 1).Intensity - baseline)
                area += dx * (y0 + y1) / 2.0
            Next

            Return area
        End Function

    End Class

End Namespace