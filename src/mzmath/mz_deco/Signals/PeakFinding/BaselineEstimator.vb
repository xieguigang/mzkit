Imports std = System.Math

Namespace Chromatogram.PeakFinding

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
            Dim nEndpoint As Integer = std.Max(1, CInt(std.Floor(ticks.Length * 0.1)))
            nEndpoint = std.Min(nEndpoint, ticks.Length \ 2)

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
            Dim lowerIndex As Integer = CInt(std.Floor(rank))
            Dim upperIndex As Integer = CInt(std.Ceiling(rank))
            lowerIndex = std.Max(0, std.Min(lowerIndex, intensities.Length - 1))
            upperIndex = std.Max(0, std.Min(upperIndex, intensities.Length - 1))

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

            boundaryPoints = std.Min(boundaryPoints, ticks.Length \ 4)

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

End Namespace