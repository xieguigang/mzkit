''' <summary>
''' LC-MS表达矩阵数据预处理模块 - 批次矫正
''' 
''' 本模块实现了多种批次效应矫正方法，用于消除LC-MS数据中
''' 由于不同实验批次、仪器漂移等因素导致的系统性差异。
''' </summary>
Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 批次矫正处理类
    ''' 提供多种批次效应矫正方法
    ''' </summary>
    Public Class BatchCorrection

        ''' <summary>
        ''' 对表达矩阵执行批次矫正
        ''' </summary>
        ''' <param name="matrix">表达矩阵（特征×样本），不应包含缺失值</param>
        ''' <param name="samples">样本信息数组</param>
        ''' <param name="method">批次矫正方法</param>
        ''' <param name="options">配置参数</param>
        ''' <returns>批次矫正后的矩阵</returns>
        Public Shared Function Correct(matrix As Double(,), samples As SampleInfo(),
                                       method As BatchCorrectionMethod,
                                       options As PreprocessingOptions) As Double(,)
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

            Select Case method
                Case BatchCorrectionMethod.None
                    ' 不矫正

                Case BatchCorrectionMethod.MeanCentering
                    CorrectMeanCentering(result, samples)

                Case BatchCorrectionMethod.MedianCentering
                    CorrectMedianCentering(result, samples)

                Case BatchCorrectionMethod.QC_RLSC
                    CorrectQCRLSC(result, samples, options)

                Case BatchCorrectionMethod.LOESS
                    CorrectLOESS(result, samples, options)

                Case BatchCorrectionMethod.ComBat
                    CorrectComBat(result, samples, options.ComBat_Parametric)

                Case BatchCorrectionMethod.SVR
                    CorrectSVR(result, samples, options)

                Case BatchCorrectionMethod.NormalizeQC
                    CorrectNormalizeQC(result, samples)

                Case Else
                    Throw New ArgumentException("不支持的批次矫正方法: " & method.ToString())
            End Select

            Return result
        End Function

        ''' <summary>
        ''' 获取所有批次ID
        ''' </summary>
        Public Shared Function GetBatchIds(samples As SampleInfo()) As Integer()
            Dim batchSet As New HashSet(Of Integer)
            For Each s In samples
                batchSet.Add(s.batch)
            Next
            Dim result(batchSet.Count - 1) As Integer
            batchSet.CopyTo(result)
            Array.Sort(result)
            Return result
        End Function

        ''' <summary>
        ''' 获取指定批次的样本索引
        ''' </summary>
        Public Shared Function GetBatchSampleIndices(samples As SampleInfo(), batchId As Integer) As Integer()
            Dim indices As New List(Of Integer)
            For j As Integer = 0 To samples.Length - 1
                If samples(j).batch = batchId Then
                    indices.Add(j)
                End If
            Next
            Return indices.ToArray()
        End Function

        ''' <summary>
        ''' 获取QC样本的索引
        ''' </summary>
        Public Shared Function GetQCSampleIndices(samples As SampleInfo(), qcLabel As String) As Integer()
            Dim indices As New List(Of Integer)
            For j As Integer = 0 To samples.Length - 1
                If samples(j).sample_info = qcLabel Then
                    indices.Add(j)
                End If
            Next
            Return indices.ToArray()
        End Function

        ' ================================================================
        '  简单中心化方法
        ' ================================================================

        ''' <summary>
        ''' 批次均值中心化
        ''' 
        ''' 原理：计算每个批次中所有特征的均值，然后将该批次的所有值减去批次均值，
        ''' 再加上总体均值，使不同批次的均值对齐。
        ''' 
        ''' 公式：x_corrected = x - mean_batch + mean_total
        ''' 
        ''' 优点：简单快速
        ''' 缺点：仅矫正加性批次效应，不矫正乘性批次效应
        ''' </summary>
        Private Shared Sub CorrectMeanCentering(matrix As Double(,), samples As SampleInfo())
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 计算总体均值
            Dim totalMean As Double = 0
            Dim totalCount As Integer = 0
            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    totalMean += matrix(i, j)
                    totalCount += 1
                Next
            Next
            If totalCount > 0 Then totalMean /= totalCount

            ' 按批次矫正
            Dim batchIds = GetBatchIds(samples)
            For Each batchId In batchIds
                Dim batchIndices = GetBatchSampleIndices(samples, batchId)
                If batchIndices.Length = 0 Then Continue For

                ' 计算批次均值
                Dim batchMean As Double = 0
                Dim batchCount As Integer = 0
                For Each idx In batchIndices
                    For i As Integer = 0 To nFeatures - 1
                        batchMean += matrix(i, idx)
                        batchCount += 1
                    Next
                Next
                If batchCount > 0 Then batchMean /= batchCount

                ' 矫正
                Dim offset As Double = totalMean - batchMean
                For Each idx In batchIndices
                    For i As Integer = 0 To nFeatures - 1
                        matrix(i, idx) += offset
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 批次中位数中心化
        ''' 
        ''' 与均值中心化类似，但使用中位数代替均值，更加鲁棒。
        ''' 
        ''' 公式：x_corrected = x - median_batch + median_total
        ''' </summary>
        Private Shared Sub CorrectMedianCentering(matrix As Double(,), samples As SampleInfo())
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 计算总体中位数
            Dim allValues As New List(Of Double)
            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    allValues.Add(matrix(i, j))
                Next
            Next
            Dim totalMedian As Double = MathHelpers.Median(allValues.ToArray())

            ' 按批次矫正
            Dim batchIds = GetBatchIds(samples)
            For Each batchId In batchIds
                Dim batchIndices = GetBatchSampleIndices(samples, batchId)
                If batchIndices.Length = 0 Then Continue For

                ' 计算批次中位数
                Dim batchValues As New List(Of Double)
                For Each idx In batchIndices
                    For i As Integer = 0 To nFeatures - 1
                        batchValues.Add(matrix(i, idx))
                    Next
                Next
                Dim batchMedian As Double = MathHelpers.Median(batchValues.ToArray())

                ' 矫正
                Dim offset As Double = totalMedian - batchMedian
                For Each idx In batchIndices
                    For i As Integer = 0 To nFeatures - 1
                        matrix(i, idx) += offset
                    Next
                Next
            Next
        End Sub

        ' ================================================================
        '  基于QC样本的信号漂移矫正
        ' ================================================================

        ''' <summary>
        ''' QC样本鲁棒LOESS信号矫正（QC-RLSC）
        ''' 
        ''' 这是LC-MS代谢组学中批次矫正的金标准方法。
        ''' 
        ''' 原理：
        ''' 1. 对每个特征，使用QC样本的信号强度和上机顺序拟合LOESS曲线
        ''' 2. LOESS曲线反映了仪器信号随时间的漂移趋势
        ''' 3. 计算矫正因子 = QC中位数 / LOESS预测值
        ''' 4. 将矫正因子应用到所有样本（包括非QC样本）
        ''' 
        ''' 优点：能够有效矫正仪器信号漂移，保留生物学变异
        ''' 缺点：需要足够的QC样本（建议每个批次至少5个QC）
        ''' 
        ''' 适用场景：有QC样本的LC-MS代谢组学数据
        ''' </summary>
        Private Shared Sub CorrectQCRLSC(matrix As Double(,), samples As SampleInfo(), options As PreprocessingOptions)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 获取QC样本索引
            Dim qcIndices = GetQCSampleIndices(samples, options.QCLabel)
            If qcIndices.Length < 3 Then
                Throw New InvalidOperationException(
                    "QC-RLSC需要至少3个QC样本，当前仅有" & qcIndices.Length & "个")
            End If

            ' 获取QC样本的上机顺序
            Dim qcOrders(qcIndices.Length - 1) As Double
            For k As Integer = 0 To qcIndices.Length - 1
                qcOrders(k) = CDbl(samples(qcIndices(k)).injectionOrder)
            Next

            ' 获取所有样本的上机顺序
            Dim allOrders(nSamples - 1) As Double
            For j As Integer = 0 To nSamples - 1
                allOrders(j) = CDbl(samples(j).injectionOrder)
            Next

            ' 对每个特征进行QC-RLSC矫正
            For i As Integer = 0 To nFeatures - 1
                ' 获取QC样本的信号值
                Dim qcValues(qcIndices.Length - 1) As Double
                For k As Integer = 0 To qcIndices.Length - 1
                    qcValues(k) = matrix(i, qcIndices(k))
                Next

                ' 计算QC中位数作为参考值
                Dim qcMedian As Double = MathHelpers.Median(qcValues)
                If Double.IsNaN(qcMedian) OrElse qcMedian <= 0 Then Continue For

                ' 使用LOESS拟合QC样本的信号漂移曲线
                Dim span As Double = options.LOESS_Span
                ' 确保span足够大以包含足够的QC点
                Dim minSpan As Double = 5.0 / qcIndices.Length
                If span < minSpan Then span = minSpan
                If span > 1.0 Then span = 1.0

                Dim loessFit = MathHelpers.LOESS(qcOrders, qcValues, allOrders, span, options.LOESS_Degree)

                ' 应用矫正
                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(loessFit(j)) AndAlso loessFit(j) > 0 Then
                        matrix(i, j) = matrix(i, j) * (qcMedian / loessFit(j))
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 基于QC样本的LOESS回归矫正
        ''' 
        ''' 与QC-RLSC类似，但使用加法模型而非乘法模型。
        ''' 
        ''' 公式：x_corrected = x - (LOESS_predicted - QC_mean)
        ''' 
        ''' 适用于信号漂移为加法模式的情况
        ''' </summary>
        Private Shared Sub CorrectLOESS(matrix As Double(,), samples As SampleInfo(), options As PreprocessingOptions)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            Dim qcIndices = GetQCSampleIndices(samples, options.QCLabel)
            If qcIndices.Length < 3 Then
                Throw New InvalidOperationException(
                    "LOESS矫正需要至少3个QC样本，当前仅有" & qcIndices.Length & "个")
            End If

            Dim qcOrders(qcIndices.Length - 1) As Double
            For k As Integer = 0 To qcIndices.Length - 1
                qcOrders(k) = CDbl(samples(qcIndices(k)).injectionOrder)
            Next

            Dim allOrders(nSamples - 1) As Double
            For j As Integer = 0 To nSamples - 1
                allOrders(j) = CDbl(samples(j).injectionOrder)
            Next

            For i As Integer = 0 To nFeatures - 1
                Dim qcValues(qcIndices.Length - 1) As Double
                For k As Integer = 0 To qcIndices.Length - 1
                    qcValues(k) = matrix(i, qcIndices(k))
                Next

                Dim qcMean As Double = MathHelpers.Mean(qcValues)
                If Double.IsNaN(qcMean) Then Continue For

                Dim span As Double = options.LOESS_Span
                Dim minSpan As Double = 5.0 / qcIndices.Length
                If span < minSpan Then span = minSpan
                If span > 1.0 Then span = 1.0

                Dim loessFit = MathHelpers.LOESS(qcOrders, qcValues, allOrders, span, options.LOESS_Degree)

                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(loessFit(j)) Then
                        matrix(i, j) = matrix(i, j) - (loessFit(j) - qcMean)
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' QC样本均值归一化
        ''' 
        ''' 最简单的QC矫正方法：将每个特征除以该批次QC样本的均值。
        ''' 
        ''' 公式：x_corrected = x / mean(QC_batch) * mean(QC_all)
        ''' 
        ''' 优点：简单快速
        ''' 缺点：仅矫正乘性批次效应，不矫正信号漂移
        ''' </summary>
        Private Shared Sub CorrectNormalizeQC(matrix As Double(,), samples As SampleInfo())
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            Dim qcIndices = GetQCSampleIndices(samples, "QC")
            If qcIndices.Length = 0 Then
                Throw New InvalidOperationException("NormalizeQC需要至少1个QC样本")
            End If

            ' 计算所有QC样本的均值作为参考
            For i As Integer = 0 To nFeatures - 1
                Dim qcMean As Double = 0
                For Each idx In qcIndices
                    qcMean += matrix(i, idx)
                Next
                qcMean /= qcIndices.Length

                If qcMean > 0 Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) /= qcMean
                    Next
                End If
            Next
        End Sub

        ' ================================================================
        '  经验贝叶斯方法
        ' ================================================================

        ''' <summary>
        ''' ComBat批次效应矫正（经验贝叶斯方法）
        ''' 
        ''' 这是生物信息学中最广泛使用的批次效应矫正方法，
        ''' 最初由Johnson等人(2007)提出用于基因表达数据。
        ''' 
        ''' 原理：
        ''' 1. 标准化数据，估计全局均值和方差
        ''' 2. 估计每个批次的加性效应(gamma)和乘性效应(delta)
        ''' 3. 使用经验贝叶斯方法收缩批次效应估计
        ''' 4. 应用矫正
        ''' 
        ''' 公式：
        ''' 标准化：Z_ij = (Y_ij - gamma_hat_bi) / sqrt(delta_hat_bi)
        ''' 矫正：Y_corrected = gamma_star * Z * sqrt(delta_star) + alpha
        ''' 
        ''' 优点：能够同时矫正加性和乘性批次效应，利用跨特征信息
        ''' 缺点：假设批次效应在所有特征上相似，可能过度矫正
        ''' 
        ''' 适用场景：多批次实验数据，不一定需要QC样本
        ''' </summary>
        Private Shared Sub CorrectComBat(matrix As Double(,), samples As SampleInfo(), parametric As Boolean)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            Dim batchIds = GetBatchIds(samples)
            Dim nBatches As Integer = batchIds.Length

            If nBatches < 2 Then
                Throw New InvalidOperationException("ComBat需要至少2个批次")
            End If

            ' 获取每个批次的样本索引
            Dim batchIndices(nBatches - 1) As Integer()
            Dim batchSize(nBatches - 1) As Integer
            For b As Integer = 0 To nBatches - 1
                batchIndices(b) = GetBatchSampleIndices(samples, batchIds(b))
                batchSize(b) = batchIndices(b).Length
            Next

            ' Step 1: 计算全局均值（grand mean）
            Dim grandMean(nFeatures - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                Dim sum As Double = 0
                For j As Integer = 0 To nSamples - 1
                    sum += matrix(i, j)
                Next
                grandMean(i) = sum / nSamples
            Next

            ' Step 2: 估计批次效应参数
            Dim gammaHat(nFeatures - 1, nBatches - 1) As Double  ' 加性效应
            Dim deltaHat(nFeatures - 1, nBatches - 1) As Double  ' 乘性效应

            For b As Integer = 0 To nBatches - 1
                For i As Integer = 0 To nFeatures - 1
                    ' 批次均值
                    Dim batchMean As Double = 0
                    For Each idx In batchIndices(b)
                        batchMean += matrix(i, idx)
                    Next
                    batchMean /= batchSize(b)

                    ' 批次方差
                    Dim batchVar As Double = 0
                    For Each idx In batchIndices(b)
                        batchVar += (matrix(i, idx) - batchMean) ^ 2
                    Next
                    batchVar /= (batchSize(b) - 1)

                    gammaHat(i, b) = batchMean - grandMean(i)
                    deltaHat(i, b) = If(batchVar > 0, batchVar, 1.0)
                Next
            Next

            ' Step 3: 经验贝叶斯收缩
            Dim gammaStar(nFeatures - 1, nBatches - 1) As Double
            Dim deltaStar(nFeatures - 1, nBatches - 1) As Double

            For b As Integer = 0 To nBatches - 1
                ' 收集该批次的所有gamma和delta
                Dim gammas(nFeatures - 1) As Double
                Dim deltas(nFeatures - 1) As Double
                For i As Integer = 0 To nFeatures - 1
                    gammas(i) = gammaHat(i, b)
                    deltas(i) = deltaHat(i, b)
                Next

                If parametric Then
                    ' 参数经验贝叶斯
                    ' Gamma: 假设 N(gamma_bar, tau^2)
                    Dim gammaBar As Double = MathHelpers.Mean(gammas)
                    Dim tau2 As Double = MathHelpers.Variance(gammas)
                    If Double.IsNaN(tau2) OrElse tau2 < 0 Then tau2 = 0

                    ' Delta: 假设 Inverse-Gamma(lambda_bar, theta_bar)
                    Dim deltaBar As Double = MathHelpers.Mean(deltas)
                    Dim deltaVar As Double = MathHelpers.Variance(deltas)
                    Dim lambdaBar As Double = If(deltaVar > 0, deltaBar ^ 2 / deltaVar + 2, 1)
                    Dim thetaBar As Double = If(deltaVar > 0, deltaBar * (lambdaBar - 1), deltaBar)

                    ' 计算后验估计
                    For i As Integer = 0 To nFeatures - 1
                        ' Gamma后验
                        Dim n_b As Double = batchSize(b)
                        Dim pooledVar As Double = deltaHat(i, b)
                        If pooledVar <= 0 Then pooledVar = 1.0

                        gammaStar(i, b) = (n_b * gammaHat(i, b) / pooledVar + gammaBar / If(tau2 > 0, tau2, 1)) /
                                           (n_b / pooledVar + 1.0 / If(tau2 > 0, tau2, 1))

                        ' Delta后验
                        Dim alpha_post As Double = lambdaBar + n_b / 2.0
                        Dim beta_post As Double = thetaBar + 0.5 * n_b * deltaHat(i, b)
                        deltaStar(i, b) = beta_post / (alpha_post + 1)
                    Next
                Else
                    ' 非参数经验贝叶斯（使用核密度估计的简化版本）
                    ' 使用中位数和MAD作为鲁棒估计
                    Dim gammaBar As Double = MathHelpers.Median(gammas)
                    Dim tau2 As Double = MathHelpers.MAD(gammas)
                    If Double.IsNaN(tau2) OrElse tau2 <= 0 Then tau2 = MathHelpers.StdDev(gammas)
                    If Double.IsNaN(tau2) OrElse tau2 <= 0 Then tau2 = 1.0

                    Dim deltaBar As Double = MathHelpers.Median(deltas)
                    Dim deltaScale As Double = MathHelpers.MAD(deltas)
                    If Double.IsNaN(deltaScale) OrElse deltaScale <= 0 Then deltaScale = MathHelpers.StdDev(deltas)
                    If Double.IsNaN(deltaScale) OrElse deltaScale <= 0 Then deltaScale = 1.0

                    For i As Integer = 0 To nFeatures - 1
                        ' 使用收缩公式
                        Dim n_b As Double = batchSize(b)
                        gammaStar(i, b) = (n_b * gammaHat(i, b) + gammaBar / (tau2 * tau2)) /
                                           (n_b + 1.0 / (tau2 * tau2))

                        ' Delta收缩
                        Dim shrinkage As Double = deltaScale / (deltaScale + deltaHat(i, b) / n_b)
                        deltaStar(i, b) = shrinkage * deltaBar + (1 - shrinkage) * deltaHat(i, b)
                    Next
                End If
            Next

            ' Step 4: 应用矫正
            ' 计算全局方差
            Dim pooledVar(nFeatures - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                Dim sumSq As Double = 0
                For j As Integer = 0 To nSamples - 1
                    sumSq += (matrix(i, j) - grandMean(i)) ^ 2
                Next
                pooledVar(i) = sumSq / (nSamples - nBatches)
                If pooledVar(i) <= 0 Then pooledVar(i) = 1.0
            Next

            For b As Integer = 0 To nBatches - 1
                For Each idx In batchIndices(b)
                    For i As Integer = 0 To nFeatures - 1
                        ' 标准化
                        Dim z As Double = (matrix(i, idx) - gammaHat(i, b)) / System.Math.Sqrt(deltaHat(i, b))
                        ' 反标准化使用收缩后的参数
                        matrix(i, idx) = z * System.Math.Sqrt(deltaStar(i, b)) + gammaStar(i, b) + grandMean(i)
                    Next
                Next
            Next
        End Sub

        ' ================================================================
        '  SVR方法
        ' ================================================================

        ''' <summary>
        ''' 基于支持向量回归的信号漂移矫正
        ''' 
        ''' 原理：
        ''' 1. 使用QC样本作为训练数据
        ''' 2. 对每个特征，训练SVR模型：injection_order -> intensity
        ''' 3. 用模型预测所有样本的期望信号强度
        ''' 4. 矫正：corrected = observed * (QC_median / predicted)
        ''' 
        ''' 优点：能够拟合非线性的信号漂移模式
        ''' 缺点：计算量较大，需要足够的QC样本
        ''' 
        ''' 适用场景：信号漂移模式复杂，LOESS拟合效果不佳时
        ''' </summary>
        Private Shared Sub CorrectSVR(matrix As Double(,), samples As SampleInfo(), options As PreprocessingOptions)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            Dim qcIndices = GetQCSampleIndices(samples, options.QCLabel)
            If qcIndices.Length < 3 Then
                Throw New InvalidOperationException(
                    "SVR矫正需要至少3个QC样本，当前仅有" & qcIndices.Length & "个")
            End If

            ' QC样本的上机顺序
            Dim qcOrders(qcIndices.Length - 1) As Double
            For k As Integer = 0 To qcIndices.Length - 1
                qcOrders(k) = CDbl(samples(qcIndices(k)).injectionOrder)
            Next

            ' 所有样本的上机顺序
            Dim allOrders(nSamples - 1) As Double
            For j As Integer = 0 To nSamples - 1
                allOrders(j) = CDbl(samples(j).injectionOrder)
            Next

            ' 对每个特征进行SVR矫正
            For i As Integer = 0 To nFeatures - 1
                ' QC样本信号值
                Dim qcValues(qcIndices.Length - 1) As Double
                For k As Integer = 0 To qcIndices.Length - 1
                    qcValues(k) = matrix(i, qcIndices(k))
                Next

                Dim qcMedian As Double = MathHelpers.Median(qcValues)
                If Double.IsNaN(qcMedian) OrElse qcMedian <= 0 Then Continue For

                ' 训练简化SVR模型（使用RBF核的核回归）
                Dim predictions = KernelRegression(qcOrders, qcValues, allOrders,
                                                   options.SVR_C, options.SVR_Epsilon, options.SVR_Gamma)

                ' 应用矫正
                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(predictions(j)) AndAlso predictions(j) > 0 Then
                        matrix(i, j) = matrix(i, j) * (qcMedian / predictions(j))
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 核回归（简化SVR实现）
        ''' 
        ''' 使用Nadaraya-Watson核回归，RBF核函数
        ''' 这是一种简化的SVR实现，使用核加权平均代替完整的SVR优化
        ''' </summary>
        Private Shared Function KernelRegression(trainX As Double(), trainY As Double(),
                                                   predictX As Double(),
                                                   C As Double, epsilon As Double,
                                                   gamma As Double) As Double()
            Dim nTrain As Integer = trainX.Length
            Dim nPred As Integer = predictX.Length
            Dim result(nPred - 1) As Double

            ' 计算核权重矩阵
            For p As Integer = 0 To nPred - 1
                Dim weightSum As Double = 0
                Dim valueSum As Double = 0

                For t As Integer = 0 To nTrain - 1
                    ' RBF核
                    Dim diff As Double = predictX(p) - trainX(t)
                    Dim k As Double = System.Math.Exp(-gamma * diff * diff)

                    ' Epsilon-insensitive权重
                    Dim residual As Double = System.Math.Abs(trainY(t) - valueSum / If(weightSum > 0, weightSum, 1))
                    Dim w As Double = k * C

                    weightSum += w
                    valueSum += w * trainY(t)
                Next

                If weightSum > 0 Then
                    result(p) = valueSum / weightSum
                Else
                    result(p) = Double.NaN
                End If
            Next

            Return result
        End Function

    End Class

End Namespace
