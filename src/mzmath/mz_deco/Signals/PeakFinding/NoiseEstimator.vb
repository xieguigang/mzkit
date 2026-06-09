Imports std = System.Math

Namespace Chromatogram.PeakFinding

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

            Dim segmentSize As Integer = CInt(std.Ceiling(ticks.Length / CDbl(segmentCount)))
            Dim minValues As New List(Of Double)()

            For seg As Integer = 0 To segmentCount - 1
                Dim startIdx As Integer = seg * segmentSize
                Dim endIdx As Integer = std.Min(startIdx + segmentSize - 1, ticks.Length - 1)
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
                absDeviations(i) = std.Abs(sortedDiffs(i) - median)
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
                    For j As Integer = i + 1 To std.Min(i + 10, ticks.Length - 2)
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

End Namespace