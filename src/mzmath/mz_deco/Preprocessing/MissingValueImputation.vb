Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 缺失值处理类
    ''' 提供多种缺失值插补方法，所有方法均基于VB.NET基础数学函数实现
    ''' </summary>
    ''' <remarks>
    ''' LC-MS表达矩阵数据预处理模块 - 缺失值处理
    ''' 
    ''' 本模块实现了多种缺失值插补方法，适用于LC-MS代谢组学数据中
    ''' 不同类型的缺失值模式。
    ''' </remarks>
    Public Class MissingValueImputation

        ''' <summary>
        ''' 对表达矩阵执行缺失值处理
        ''' </summary>
        ''' <param name="matrix">表达矩阵（特征×样本），缺失值用NaN表示</param>
        ''' <param name="method">插补方法</param>
        ''' <param name="options">配置参数</param>
        ''' <returns>插补后的矩阵</returns>
        Public Shared Function Impute(matrix As Double(,), method As MissingValueMethod,
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
                Case MissingValueMethod.None
                    ' 不处理
                Case MissingValueMethod.HalfMin
                    ImputeHalfMin(result)
                Case MissingValueMethod.MinValue
                    ImputeMinValue(result)
                Case MissingValueMethod.Zero
                    ImputeZero(result)
                Case MissingValueMethod.Mean
                    ImputeMean(result)
                Case MissingValueMethod.Median
                    ImputeMedian(result)
                Case MissingValueMethod.KNN
                    ImputeKNN(result, options.KNN_K)
                Case MissingValueMethod.PCA
                    ImputePCA(result, options.PCA_Components, options.PCA_MaxIterations, options.PCA_Tolerance)
                Case MissingValueMethod.QRILC
                    ImputeQRILC(result)
                Case MissingValueMethod.RandomMin
                    ImputeRandomMin(result)
                Case Else
                    Throw New ArgumentException("不支持的缺失值处理方法: " & method.ToString())
            End Select

            Return result
        End Function

        ''' <summary>
        ''' 将原始矩阵中的0值和NaN值标记为缺失值，转换为NaN表示
        ''' </summary>
        Public Shared Function MarkMissingValues(matrix As Double(,), options As PreprocessingOptions) As Double(,)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            Dim result(nFeatures - 1, nSamples - 1) As Double

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    Dim val As Double = matrix(i, j)
                    Dim isMissing As Boolean = False

                    If options.TreatNaNAsMissing AndAlso Double.IsNaN(val) Then
                        isMissing = True
                    End If

                    If Not Double.IsNaN(val) AndAlso Not Double.IsInfinity(val) Then
                        If System.Math.Abs(val - options.MissingValueMarker) < MathHelpers.EPSILON Then
                            isMissing = True
                        End If
                    End If

                    result(i, j) = If(isMissing, Double.NaN, val)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 过滤缺失率过高的特征
        ''' </summary>
        ''' <param name="matrix">表达矩阵（特征×样本），缺失值用NaN表示</param>
        ''' <param name="maxMissingRate">最大允许缺失率，超过此值的特征被过滤</param>
        ''' <param name="keptIndices">保留的特征索引列表</param>
        ''' <returns>过滤后的矩阵</returns>
        Public Shared Function FilterByMissingRate(matrix As Double(,), maxMissingRate As Double, ByRef keptIndices As List(Of Integer)) As Double(,)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            keptIndices = New List(Of Integer)()

            For i As Integer = 0 To nFeatures - 1
                Dim missingCount As Integer = 0
                For j As Integer = 0 To nSamples - 1
                    If Double.IsNaN(matrix(i, j)) Then missingCount += 1
                Next
                Dim missingRate As Double = CDbl(missingCount) / nSamples
                If missingRate <= maxMissingRate Then
                    keptIndices.Add(i)
                End If
            Next

            Dim result(keptIndices.Count - 1, nSamples - 1) As Double
            For idx As Integer = 0 To keptIndices.Count - 1
                For j As Integer = 0 To nSamples - 1
                    result(idx, j) = matrix(keptIndices(idx), j)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 统计矩阵中的缺失值信息
        ''' </summary>
        Public Shared Function CountMissing(matrix As Double(,)) As Integer
            Dim count As Integer = 0
            For i As Integer = 0 To matrix.GetLength(0) - 1
                For j As Integer = 0 To matrix.GetLength(1) - 1
                    If Double.IsNaN(matrix(i, j)) Then count += 1
                Next
            Next
            Return count
        End Function

        ' ================================================================
        '  具体插补方法实现
        ' ================================================================

        ''' <summary>
        ''' HalfMin插补：用特征最小检测值的一半填充缺失值
        ''' 
        ''' 这是LC-MS代谢组学中最常用的缺失值处理方法之一。
        ''' 假设缺失值是由于信号低于检测限导致的（左截断数据），
        ''' 用最小检测值的一半来近似这些低于检测限的信号。
        ''' 
        ''' 优点：简单、保守，不会高估低丰度代谢物的信号
        ''' 缺点：可能低估方差，对所有缺失值使用相同的插补值
        ''' </summary>
        Private Shared Sub ImputeHalfMin(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                ' 找到该特征的最小非缺失值
                Dim minVal As Double = Double.NaN
                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(matrix(i, j)) Then
                        If Double.IsNaN(minVal) OrElse matrix(i, j) < minVal Then
                            minVal = matrix(i, j)
                        End If
                    End If
                Next

                If Not Double.IsNaN(minVal) Then
                    Dim fillValue As Double = minVal / 2.0
                    For j As Integer = 0 To nSamples - 1
                        If Double.IsNaN(matrix(i, j)) Then
                            matrix(i, j) = fillValue
                        End If
                    Next
                Else
                    ' 全部缺失，填充0
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' MinValue插补：用特征最小检测值填充缺失值
        ''' 
        ''' 比HalfMin更保守，假设缺失值恰好等于检测限。
        ''' </summary>
        Private Shared Sub ImputeMinValue(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim minVal As Double = Double.NaN
                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(matrix(i, j)) Then
                        If Double.IsNaN(minVal) OrElse matrix(i, j) < minVal Then
                            minVal = matrix(i, j)
                        End If
                    End If
                Next

                If Not Double.IsNaN(minVal) Then
                    For j As Integer = 0 To nSamples - 1
                        If Double.IsNaN(matrix(i, j)) Then
                            matrix(i, j) = minVal
                        End If
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' Zero插补：用0填充缺失值
        ''' 
        ''' 最简单的插补方法，适用于确实不存在的情况。
        ''' 注意：0值在后续对数变换时需要特殊处理。
        ''' </summary>
        Private Shared Sub ImputeZero(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    If Double.IsNaN(matrix(i, j)) Then
                        matrix(i, j) = 0.0
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' Mean插补：用特征检测值的均值填充缺失值
        ''' 
        ''' 适用于随机缺失（MAR），但可能低估方差。
        ''' 对异常值敏感。
        ''' </summary>
        Private Shared Sub ImputeMean(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next
                Dim avg As Double = MathHelpers.Mean(row)

                If Not Double.IsNaN(avg) Then
                    For j As Integer = 0 To nSamples - 1
                        If Double.IsNaN(matrix(i, j)) Then
                            matrix(i, j) = avg
                        End If
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' Median插补：用特征检测值的中位数填充缺失值
        ''' 
        ''' 比Mean更鲁棒，对异常值不敏感。
        ''' </summary>
        Private Shared Sub ImputeMedian(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            For i As Integer = 0 To nFeatures - 1
                Dim row(nSamples - 1) As Double
                For j As Integer = 0 To nSamples - 1
                    row(j) = matrix(i, j)
                Next
                Dim med As Double = MathHelpers.Median(row)

                If Not Double.IsNaN(med) Then
                    For j As Integer = 0 To nSamples - 1
                        If Double.IsNaN(matrix(i, j)) Then
                            matrix(i, j) = med
                        End If
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                End If
            Next
        End Sub

        ''' <summary>
        ''' KNN插补：K近邻插补法
        ''' 
        ''' 算法步骤：
        ''' 1. 计算每对特征之间的欧氏距离（基于非缺失值）
        ''' 2. 对于每个含缺失值的特征，找到K个最相似的特征
        ''' 3. 用K个近邻的加权平均值填充缺失值
        ''' 
        ''' 优点：能够保留数据的局部结构信息
        ''' 缺点：计算量较大，对K值敏感
        ''' </summary>
        Private Shared Sub ImputeKNN(matrix As Double(,), k As Integer)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 先用列均值临时填充缺失值（用于距离计算）
            Dim tempMatrix(nFeatures - 1, nSamples - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                Dim avg As Double = 0
                Dim count As Integer = 0
                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(matrix(i, j)) Then
                        avg += matrix(i, j)
                        count += 1
                    End If
                Next
                If count > 0 Then avg /= count Else avg = 0

                For j As Integer = 0 To nSamples - 1
                    tempMatrix(i, j) = If(Double.IsNaN(matrix(i, j)), avg, matrix(i, j))
                Next
            Next

            ' 计算特征间距离矩阵
            Dim distMatrix(nFeatures - 1, nFeatures - 1) As Double
            For i As Integer = 0 To nFeatures - 1
                distMatrix(i, i) = 0
                For j As Integer = i + 1 To nFeatures - 1
                    Dim dist As Double = 0
                    For s As Integer = 0 To nSamples - 1
                        Dim diff As Double = tempMatrix(i, s) - tempMatrix(j, s)
                        dist += diff * diff
                    Next
                    dist = System.Math.Sqrt(dist)
                    distMatrix(i, j) = dist
                    distMatrix(j, i) = dist
                Next
            Next

            ' 对每个特征的缺失值进行KNN插补
            k = System.Math.Min(k, nFeatures - 1)
            If k < 1 Then k = 1

            For i As Integer = 0 To nFeatures - 1
                ' 找到K个最近邻（排除自身）
                Dim neighborIdx = FindKNearest(distMatrix, i, k)

                For j As Integer = 0 To nSamples - 1
                    If Double.IsNaN(matrix(i, j)) Then
                        ' 用近邻的加权平均值插补
                        Dim weightSum As Double = 0
                        Dim valueSum As Double = 0

                        For Each nIdx As Integer In neighborIdx
                            If Not Double.IsNaN(matrix(nIdx, j)) Then
                                Dim w As Double = 1.0 / (distMatrix(i, nIdx) + MathHelpers.EPSILON)
                                weightSum += w
                                valueSum += w * matrix(nIdx, j)
                            End If
                        Next

                        If weightSum > 0 Then
                            matrix(i, j) = valueSum / weightSum
                        Else
                            ' 如果所有近邻也是缺失，用0填充
                            matrix(i, j) = 0.0
                        End If
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 找到距离第rowIndex行最近的K个特征的索引
        ''' </summary>
        Private Shared Function FindKNearest(distMatrix As Double(,), rowIndex As Integer, k As Integer) As Integer()
            Dim n As Integer = distMatrix.GetLength(0)
            Dim indices As New List(Of Integer)
            Dim distances As New List(Of Double)

            For j As Integer = 0 To n - 1
                If j <> rowIndex Then
                    indices.Add(j)
                    distances.Add(distMatrix(rowIndex, j))
                End If
            Next

            ' 按距离排序
            Dim sortedIdx(distances.Count - 1) As Integer
            For i As Integer = 0 To distances.Count - 1
                sortedIdx(i) = i
            Next
            Array.Sort(sortedIdx, Function(a, b) distances(a).CompareTo(distances(b)))

            Dim result(System.Math.Min(k, sortedIdx.Length) - 1) As Integer
            For i As Integer = 0 To result.Length - 1
                result(i) = indices(sortedIdx(i))
            Next
            Return result
        End Function

        ''' <summary>
        ''' PCA插补：基于PCA的插补法（NIPALS算法）
        ''' 
        ''' 算法步骤：
        ''' 1. 用列均值初始化缺失值
        ''' 2. 使用NIPALS算法进行PCA分解
        ''' 3. 用PCA模型重建数据，替换缺失值
        ''' 4. 重复步骤2-3直到收敛
        ''' 
        ''' 优点：能够利用特征间的相关性进行插补
        ''' 缺点：计算量较大，对主成分数敏感
        ''' </summary>
        Private Shared Sub ImputePCA(matrix As Double(,), nComponents As Integer,
                                      maxIter As Integer, tolerance As Double)
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)

            ' 记录原始缺失位置
            Dim isMissing(nFeatures - 1, nSamples - 1) As Boolean
            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    isMissing(i, j) = Double.IsNaN(matrix(i, j))
                Next
            Next

            ' 迭代PCA插补
            nComponents = System.Math.Min(nComponents, System.Math.Min(nFeatures, nSamples))

            For outerIter As Integer = 1 To maxIter
                ' 用列均值初始化/更新缺失值
                For i As Integer = 0 To nFeatures - 1
                    Dim avg As Double = 0
                    Dim count As Integer = 0
                    For j As Integer = 0 To nSamples - 1
                        If Not isMissing(i, j) Then
                            avg += matrix(i, j)
                            count += 1
                        End If
                    Next
                    If count > 0 Then avg /= count Else avg = 0

                    For j As Integer = 0 To nSamples - 1
                        If isMissing(i, j) Then
                            matrix(i, j) = avg
                        End If
                    Next
                Next

                ' 中心化
                Dim colMeans(nFeatures - 1) As Double
                For i As Integer = 0 To nFeatures - 1
                    Dim sum As Double = 0
                    For j As Integer = 0 To nSamples - 1
                        sum += matrix(i, j)
                    Next
                    colMeans(i) = sum / nSamples
                Next

                Dim centered(nFeatures - 1, nSamples - 1) As Double
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = 0 To nSamples - 1
                        centered(i, j) = matrix(i, j) - colMeans(i)
                    Next
                Next

                ' NIPALS PCA
                Dim scores As Double(,) = Nothing
                Dim loadings As Double(,) = Nothing
                MathHelpers.NIPALSWithMissing(centered, nComponents, maxIter, tolerance, scores, loadings)

                ' 重建矩阵：X_hat = loadings * scores' + colMeans
                Dim oldValues(nFeatures - 1, nSamples - 1) As Double
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = 0 To nSamples - 1
                        oldValues(i, j) = matrix(i, j)
                    Next
                Next

                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = 0 To nSamples - 1
                        Dim reconstructed As Double = colMeans(i)
                        For comp As Integer = 0 To nComponents - 1
                            reconstructed += loadings(i, comp) * scores(j, comp)
                        Next
                        If isMissing(i, j) Then
                            matrix(i, j) = reconstructed
                        End If
                    Next
                Next

                ' 检查收敛
                Dim maxChange As Double = 0
                For i As Integer = 0 To nFeatures - 1
                    For j As Integer = 0 To nSamples - 1
                        If isMissing(i, j) Then
                            Dim change As Double = System.Math.Abs(matrix(i, j) - oldValues(i, j))
                            If change > maxChange Then maxChange = change
                        End If
                    Next
                Next

                If maxChange < tolerance Then Exit For
            Next outerIter
        End Sub

        ''' <summary>
        ''' QRILC插补：分位数回归左截断插补法
        ''' 
        ''' 专门针对左截断数据（信号低于检测限）设计的插补方法。
        ''' 基于分位数回归模型，假设缺失值来自分布的低尾部分。
        ''' 
        ''' 算法步骤：
        ''' 1. 对每个特征，估计非缺失值的分布
        ''' 2. 基于缺失比例，计算左截断点对应的分位数
        ''' 3. 从截断分布的低尾部分随机采样填充缺失值
        ''' 
        ''' 优点：专门为左截断数据设计，在代谢组学中表现优异
        ''' 缺点：假设缺失机制为左截断，不适用于随机缺失
        ''' </summary>
        Private Shared Sub ImputeQRILC(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            Dim seed As Long = 42

            For i As Integer = 0 To nFeatures - 1
                ' 收集非缺失值
                Dim validValues As New List(Of Double)
                Dim missingCount As Integer = 0
                For j As Integer = 0 To nSamples - 1
                    If Double.IsNaN(matrix(i, j)) Then
                        missingCount += 1
                    Else
                        validValues.Add(matrix(i, j))
                    End If
                Next

                If validValues.Count = 0 Then
                    For j As Integer = 0 To nSamples - 1
                        matrix(i, j) = 0.0
                    Next
                    Continue For
                End If

                If missingCount = 0 Then Continue For

                ' 估计分布参数
                validValues.Sort()
                Dim med As Double = MathHelpers.Median(validValues.ToArray())
                Dim iqrVal As Double = MathHelpers.IQR(validValues.ToArray())
                Dim sd As Double = If(Double.IsNaN(iqrVal) OrElse iqrVal <= 0,
                                       MathHelpers.StdDev(validValues.ToArray()),
                                       iqrVal / 1.349)

                If Double.IsNaN(sd) OrElse sd <= 0 Then sd = 1.0

                ' 计算截断分位数
                Dim missingRate As Double = CDbl(missingCount) / nSamples
                Dim lowerQuantile As Double = MathHelpers.NormalInvCDF(missingRate)

                ' 从截断正态分布的低尾采样
                For j As Integer = 0 To nSamples - 1
                    If Double.IsNaN(matrix(i, j)) Then
                        ' 生成[0, missingRate]区间内的均匀随机数
                        Dim u As Double = MathHelpers.PseudoRandom(seed) * missingRate
                        If u < 0.001 Then u = 0.001
                        Dim z As Double = MathHelpers.NormalInvCDF(u)
                        matrix(i, j) = med + z * sd
                        ' 确保插补值不超过最小检测值
                        Dim minDetected As Double = validValues(0)
                        If matrix(i, j) > minDetected Then
                            matrix(i, j) = minDetected * MathHelpers.PseudoRandom(seed)
                        End If
                        If matrix(i, j) < 0 Then matrix(i, j) = 0
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' RandomMin插补：用0到特征最小值之间的随机值填充缺失值
        ''' 
        ''' 模拟低于检测限的随机信号，减少插补偏差。
        ''' 比固定值插补（如HalfMin）更能保留数据的随机性。
        ''' </summary>
        Private Shared Sub ImputeRandomMin(matrix As Double(,))
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            Dim seed As Long = 42

            For i As Integer = 0 To nFeatures - 1
                Dim minVal As Double = Double.NaN
                For j As Integer = 0 To nSamples - 1
                    If Not Double.IsNaN(matrix(i, j)) Then
                        If Double.IsNaN(minVal) OrElse matrix(i, j) < minVal Then
                            minVal = matrix(i, j)
                        End If
                    End If
                Next

                If Not Double.IsNaN(minVal) AndAlso minVal > 0 Then
                    For j As Integer = 0 To nSamples - 1
                        If Double.IsNaN(matrix(i, j)) Then
                            matrix(i, j) = MathHelpers.PseudoRandom(seed) * minVal
                        End If
                    Next
                Else
                    For j As Integer = 0 To nSamples - 1
                        If Double.IsNaN(matrix(i, j)) Then
                            matrix(i, j) = 0.0
                        End If
                    Next
                End If
            Next
        End Sub

    End Class

End Namespace
