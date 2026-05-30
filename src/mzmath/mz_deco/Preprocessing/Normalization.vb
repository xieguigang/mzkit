Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 归一化处理类
    ''' 提供多种归一化和数据变换方法
    ''' </summary>
    ''' <remarks>
    ''' LC-MS表达矩阵数据预处理模块 - 归一化
    ''' 
    ''' 本模块实现了多种归一化和数据变换方法，用于消除LC-MS数据中的
    ''' 系统性差异和改善数据分布特性。
    ''' </remarks>
    Public Class Normalization

        ''' <summary>
        ''' 对表达矩阵执行归一化
        ''' </summary>
        ''' <param name="matrix">表达矩阵（特征×样本），不应包含缺失值</param>
        ''' <param name="method">归一化方法</param>
        ''' <param name="options">配置参数</param>
        ''' <param name="normFactors">输出的归一化因子（仅信号强度归一化方法有效）</param>
        ''' <returns>归一化后的矩阵</returns>
        Public Shared Function Normalize(matrix As Double(,), method As NormalizationMethod,
                                         options As PreprocessingOptions,
                                         Optional ByRef normFactors As Double() = Nothing) As Double(,)
            If matrix Is Nothing Then Return Nothing

            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 复制矩阵
            Dim result(nFeatures - 1, nSamples - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    result(i, j) = matrix(i, j)
                Next
            Next

            normFactors = Nothing

            Select Case method
                Case NormalizationMethod.None
                    ' 不归一化

                Case NormalizationMethod.TotalIonCount
                    NormalizeTIC(result, normFactors)

                Case NormalizationMethod.MedianNorm
                    NormalizeMedian(result, normFactors)

                Case NormalizationMethod.Quantile
                    NormalizeQuantile(result)

                Case NormalizationMethod.PQN
                    NormalizePQN(result, options.PQN_ReferenceType, normFactors)

                Case NormalizationMethod.Log2
                    TransformLog(result, 2.0, options.LogOffset)

                Case NormalizationMethod.Log10
                    TransformLog(result, 10.0, options.LogOffset)

                Case NormalizationMethod.Ln
                    TransformLog(result, System.Math.E, options.LogOffset)

                Case NormalizationMethod.CubeRoot
                    TransformCubeRoot(result)

                Case NormalizationMethod.SqrtTransform
                    TransformSqrt(result)

                Case NormalizationMethod.AutoScaling
                    ScaleAuto(result)

                Case NormalizationMethod.ParetoScaling
                    ScalePareto(result)

                Case NormalizationMethod.RangeScaling
                    ScaleRange(result)

                Case NormalizationMethod.VastScaling
                    ScaleVast(result)

                Case NormalizationMethod.LevelScaling
                    ScaleLevel(result)

                Case Else
                    Throw New ArgumentException("不支持的归一化方法: " & method.ToString())
            End Select

            Return result
        End Function

        ' ================================================================
        '  信号强度归一化方法
        ' ================================================================

        ''' <summary>
        ''' 总离子计数归一化（TIC Normalization）
        ''' 
        ''' 公式：x_norm = x / TIC_sample * mean(TIC_all)
        ''' 
        ''' 原理：假设每个样本的总信号量应该相同，通过除以总离子计数
        ''' 来消除样本间总信号量的差异，再乘以所有样本TIC的均值
        ''' 使归一化后的数值尺度与原始数据保持一致。
        ''' 
        ''' 优点：简单直观，计算快速
        ''' 缺点：受高丰度代谢物影响大，可能掩盖低丰度代谢物的变化
        ''' 
        ''' 适用场景：样本组成相似，总代谢物含量差异主要由技术因素导致
        ''' </summary>
        Private Shared Sub NormalizeTIC(matrix As Double(,), ByRef normFactors As Double())
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 计算每个样本的TIC
            Dim tic(nSamples - 1) As Double
            For j As Integer = 0 To nSamples - 1
                Dim sum As Double = 0
                For i As Integer = 0 To nFeatures - 1
                    sum += matrix(i, j)
                Next
                tic(j) = sum
            Next

            ' 计算TIC均值
            Dim meanTIC As Double = MathHelpers.Mean(tic)
            If Double.IsNaN(meanTIC) OrElse meanTIC = 0 Then meanTIC = 1.0

            ' 归一化
            normFactors = New Double(nSamples - 1) {}
            For j As Integer = 0 To nSamples - 1
                If tic(j) > 0 Then
                    normFactors(j) = meanTIC / tic(j)
                Else
                    normFactors(j) = 1.0
                End If
                For i As Integer = 0 To nFeatures - 1
                    matrix(i, j) *= normFactors(j)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 中位数归一化（Median Normalization）
        ''' 
        ''' 公式：x_norm = x / median_sample * median(median_all)
        ''' 
        ''' 原理：与TIC归一化类似，但使用中位数代替总和。
        ''' 中位数对异常值和高丰度代谢物不敏感，更加鲁棒。
        ''' 
        ''' 优点：比TIC更鲁棒，不受少数高丰度代谢物影响
        ''' 缺点：当样本间代谢物组成差异大时可能不适用
        ''' 
        ''' 适用场景：存在少数极高丰度代谢物的情况
        ''' </summary>
        Private Shared Sub NormalizeMedian(matrix As Double(,), ByRef normFactors As Double())
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 计算每个样本的中位数
            Dim medians(nSamples - 1) As Double
            For j As Integer = 0 To nSamples - 1
                Dim col(nFeatures - 1) As Double
                For i As Integer = 0 To nFeatures - 1
                    col(i) = matrix(i, j)
                Next
                medians(j) = MathHelpers.Median(col)
            Next

            ' 计算中位数的中位数
            Dim medianOfMedians As Double = MathHelpers.Median(medians)
            If Double.IsNaN(medianOfMedians) OrElse medianOfMedians = 0 Then medianOfMedians = 1.0

            ' 归一化
            normFactors = New Double(nSamples - 1) {}
            For j As Integer = 0 To nSamples - 1
                If medians(j) > 0 Then
                    normFactors(j) = medianOfMedians / medians(j)
                Else
                    normFactors(j) = 1.0
                End If
                For i As Integer = 0 To nFeatures - 1
                    matrix(i, j) *= normFactors(j)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 分位数归一化（Quantile Normalization）
        ''' 
        ''' 原理：强制所有样本具有相同的分布。
        ''' 1. 对每个样本的特征值排序
        ''' 2. 计算每个排名位置上所有样本的均值
        ''' 3. 用该均值替换对应排名的值
        ''' 4. 恢复原始顺序
        ''' 
        ''' 优点：完全消除样本间的分布差异
        ''' 缺点：可能过度矫正，改变真实的生物学差异
        ''' 
        ''' 适用场景：假设所有样本的代谢物分布应该相同
        ''' </summary>
        Private Shared Sub NormalizeQuantile(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 对每列排序，记录排序索引
            Dim sortedIndices(nSamples - 1)() As Integer
            For j As Integer = 0 To nSamples - 1
                Dim col(nFeatures - 1) As Double
                For i As Integer = 0 To nFeatures - 1
                    col(i) = matrix(i, j)
                Next
                sortedIndices(j) = MathHelpers.OrderBy(col)
            Next

            ' 计算每个排名位置的均值
            Dim rankMeans(nFeatures - 1) As Double
            For rank As Integer = 0 To nFeatures - 1
                Dim sum As Double = 0
                For j As Integer = 0 To nSamples - 1
                    sum += matrix(sortedIndices(j)(rank), j)
                Next
                rankMeans(rank) = sum / nSamples
            Next

            ' 用排名均值替换
            For j As Integer = 0 To nSamples - 1
                For rank As Integer = 0 To nFeatures - 1
                    matrix(sortedIndices(j)(rank), j) = rankMeans(rank)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 概率商归一化（Probabilistic Quotient Normalization, PQN）
        ''' 
        ''' 原理：
        ''' 1. 先进行TIC归一化
        ''' 2. 计算参考谱（所有样本的中位数谱）
        ''' 3. 对每个样本，计算各特征与参考谱的商
        ''' 4. 用商的中位数作为该样本的归一化因子
        ''' 
        ''' 优点：专门为光谱数据设计，不受高丰度代谢物主导
        ''' 缺点：需要合理的参考谱
        ''' 
        ''' 适用场景：LC-MS代谢组学中最推荐的归一化方法之一
        ''' </summary>
        Private Shared Sub NormalizePQN(matrix As Double(,), referenceType As String, ByRef normFactors As Double())
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 先TIC归一化
            Dim dummyFactors As Double() = Nothing
            NormalizeTIC(matrix, dummyFactors)

            ' 计算参考谱
            Dim reference(nFeatures - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next
                If referenceType.ToLower() = "mean" Then
                    reference(i) = MathHelpers.Mean(row)
                Else
                    reference(i) = MathHelpers.Median(row)
                End If
            Next

            ' 计算每个样本的PQN因子
            normFactors = New Double(nSamples - 1) {}
            For j As Integer = 0 To nSamples - 1
                Dim quotients As New List(Of Double)
                For i As Integer = 0 To nFeatures - 1
                    If reference(i) > 0 AndAlso matrix(i, j) > 0 Then
                        quotients.Add(matrix(i, j) / reference(i))
                    End If
                Next

                If quotients.Count > 0 Then
                    quotients.Sort()
                    normFactors(j) = MathHelpers.Median(quotients.ToArray())
                Else
                    normFactors(j) = 1.0
                End If

                If Double.IsNaN(normFactors(j)) OrElse normFactors(j) = 0 Then
                    normFactors(j) = 1.0
                End If
            Next

            ' 应用PQN因子
            For j As Integer = 0 To nSamples - 1
                For i As Integer = 0 To nFeatures - 1
                    matrix(i, j) /= normFactors(j)
                Next
            Next
        End Sub

        ' ================================================================
        '  数据变换方法
        ' ================================================================

        ''' <summary>
        ''' 对数变换
        ''' 
        ''' 公式：x_log = log_base(x + offset)
        ''' 
        ''' 原理：LC-MS数据通常呈右偏分布，对数变换可以：
        ''' 1. 使数据更接近正态分布
        ''' 2. 稳定方差（使方差与均值无关）
        ''' 3. 减小高丰度代谢物的影响
        ''' 
        ''' offset参数用于避免对0取对数，通常设为1或更小的值
        ''' </summary>
        Private Shared Sub TransformLog(matrix As Double(,), base As Double, offset As Double)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    matrix(i, j) = MathHelpers.SafeLog(matrix(i, j), base, offset)
                    If Double.IsNaN(matrix(i, j)) Then
                        matrix(i, j) = 0.0
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 平方根变换
        ''' 
        ''' 公式：x_sqrt = sqrt(x)
        ''' 
        ''' 比对数变换更温和的方差稳定变换。
        ''' 适用于计数数据或方差与均值成正比的数据。
        ''' </summary>
        Private Shared Sub TransformSqrt(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    If matrix(i, j) >= 0 Then
                        matrix(i, j) = System.Math.Sqrt(matrix(i, j))
                    Else
                        matrix(i, j) = -System.Math.Sqrt(-matrix(i, j))
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 立方根变换
        ''' 
        ''' 公式：x_cbrt = sign(x) * |x|^(1/3)
        ''' 
        ''' 适用于包含负值的数据（如对数比值）。
        ''' </summary>
        Private Shared Sub TransformCubeRoot(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    If matrix(i, j) >= 0 Then
                        matrix(i, j) = System.Math.Pow(matrix(i, j), 1.0 / 3.0)
                    Else
                        matrix(i, j) = -System.Math.Pow(-matrix(i, j), 1.0 / 3.0)
                    End If
                Next
            Next
        End Sub

        ' ================================================================
        '  缩放方法
        ' ================================================================

        ''' <summary>
        ''' 自动缩放 / Z-score标准化（Auto Scaling）
        ''' 
        ''' 公式：x_scaled = (x - mean) / std
        ''' 
        ''' 效果：每个特征的均值为0，标准差为1。
        ''' 所有特征在统计分析中具有相同的权重。
        ''' 
        ''' 优点：消除量纲差异，所有特征权重相同
        ''' 缺点：放大噪声（低丰度代谢物的噪声也被放大）
        ''' 
        ''' 适用场景：当所有特征同等重要时，如PCA、PLS-DA等
        ''' </summary>
        Private Shared Sub ScaleAuto(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next

                Dim avg As Double = MathHelpers.Mean(row)
                Dim sd As Double = MathHelpers.StdDev(row)

                If Double.IsNaN(avg) Then avg = 0
                If Double.IsNaN(sd) OrElse sd < MathHelpers.EPSILON Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = (matrix(i, j) - avg) / sd
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' Pareto缩放
        ''' 
        ''' 公式：x_scaled = (x - mean) / sqrt(std)
        ''' 
        ''' 效果：介于不缩放和自动缩放之间的折中方案。
        ''' 高丰度代谢物仍有一定优势，但低丰度代谢物不会被过度放大。
        ''' 
        ''' 优点：平衡了高丰度和低丰度代谢物的影响
        ''' 缺点：缩放程度介于Auto和None之间，需要根据数据选择
        ''' 
        ''' 适用场景：LC-MS代谢组学中最常用的缩放方法
        ''' </summary>
        Private Shared Sub ScalePareto(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next

                Dim avg As Double = MathHelpers.Mean(row)
                Dim sd As Double = MathHelpers.StdDev(row)

                If Double.IsNaN(avg) Then avg = 0
                If Double.IsNaN(sd) OrElse sd < MathHelpers.EPSILON Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                Else
                    Dim paretoSd As Double = System.Math.Sqrt(sd)
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = (matrix(i, j) - avg) / paretoSd
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' 范围缩放（Range Scaling）
        ''' 
        ''' 公式：x_scaled = (x - mean) / (max - min)
        ''' 
        ''' 效果：将数据缩放到以均值为中心，范围为1的尺度。
        ''' 
        ''' 优点：结果直观，最大值和最小值之间的范围归一化
        ''' 缺点：对异常值敏感（max和min受极端值影响）
        ''' 
        ''' 适用场景：关注代谢物的相对变化范围
        ''' </summary>
        Private Shared Sub ScaleRange(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next

                Dim avg As Double = MathHelpers.Mean(row)
                Dim maxVal As Double = MathHelpers.Max(row)
                Dim minVal As Double = MathHelpers.Min(row)
                Dim range As Double = maxVal - minVal

                If Double.IsNaN(avg) Then avg = 0
                If Double.IsNaN(range) OrElse range < MathHelpers.EPSILON Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = (matrix(i, j) - avg) / range
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' VAST缩放（Variable Stability Scaling）
        ''' 
        ''' 公式：x_scaled = (x - mean) / std * CV^2
        ''' 
        ''' 效果：根据变异系数的平方对特征进行加权。
        ''' 变异系数大的特征（稳定性差）被赋予更大的权重。
        ''' 
        ''' 优点：强调稳定性差的变量，适合分类问题
        ''' 缺点：可能过度放大噪声变量
        ''' 
        ''' 适用场景：关注变量的稳定性，如生物标志物发现
        ''' </summary>
        Private Shared Sub ScaleVast(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next

                Dim avg As Double = MathHelpers.Mean(row)
                Dim sd As Double = MathHelpers.StdDev(row)
                Dim cv As Double = MathHelpers.CV(row)

                If Double.IsNaN(avg) Then avg = 0
                If Double.IsNaN(sd) OrElse sd < MathHelpers.EPSILON Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                Else
                    Dim vastFactor As Double = cv * cv
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = (matrix(i, j) - avg) / sd * vastFactor
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' 水平缩放（Level Scaling）
        ''' 
        ''' 公式：x_scaled = (x - mean) / mean
        ''' 
        ''' 效果：将数据转换为相对于均值的倍数变化（fold change）。
        ''' 
        ''' 优点：直观地反映相对于平均水平的变化
        ''' 缺点：对均值接近0的特征不稳定
        ''' 
        ''' 适用场景：关注相对于平均水平的倍数变化
        ''' </summary>
        Private Shared Sub ScaleLevel(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next

                Dim avg As Double = MathHelpers.Mean(row)

                If Double.IsNaN(avg) OrElse System.Math.Abs(avg) < MathHelpers.EPSILON Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = (matrix(i, j) - avg) / avg
                    Next
                End If
            Next
        End Sub

    End Class

End Namespace
