Namespace PeakAlignment

    ' ========================================================================
    '   算法2：LOESS保留时间校正对齐
    ' ========================================================================
    Public Module LOESS

        ''' <summary>
        ''' LOESS保留时间校正对齐算法
        ''' 
        ''' 原理：
        ''' 1. 选择一个参考样本（自动选择或手动指定）
        ''' 2. 在参考样本与每个其他样本之间找到匹配的峰对（基于m/z和初始RT容差）
        ''' 3. 使用匹配峰对的RT值拟合LOESS曲线，建立样本RT到参考RT的映射关系
        ''' 4. 使用LOESS模型校正每个样本中所有峰的RT值
        ''' 5. 在校正后的RT空间中进行直接匹配对齐
        ''' 
        ''' 优点：能够处理非线性的保留时间漂移，校正效果较好
        ''' 缺点：依赖参考样本的选择，匹配峰对数量不足时LOESS拟合不稳定
        ''' </summary>
        Public Function LOESSAlignment(peaks As Dictionary(Of String, PeakFeature()),
                                         params As AlignmentParameters) As List(Of AlignedPeakGroup)
            ' 第一步：选择参考样本
            Dim refName As String = SelectReferenceSample(peaks, params.referenceSample)
            Dim refPeaks As PeakFeature() = peaks(refName)

            ' 第二步：对每个非参考样本进行LOESS校正
            Dim correctedPeaks As New Dictionary(Of String, PeakFeature())
            correctedPeaks(refName) = refPeaks

            ' 使用较大的初始RT容差来寻找匹配峰对
            Dim initialRtTol As Double = params.rtTolerance * 2.0

            For Each kv In peaks
                If kv.Key = refName Then Continue For

                ' 找到参考样本和当前样本之间的匹配峰对
                Dim matchedPairs As New List(Of Tuple(Of Double, Double)) ' (rt_sample, rt_ref)

                For Each refP In refPeaks
                    For Each sampleP In kv.Value
                        Dim mzTol As Double = GetMzTolerance(refP.mz, params.mzTolerance, params.mzToleranceMode)
                        If Math.Abs(refP.mz - sampleP.mz) <= mzTol AndAlso
                           Math.Abs(refP.rt - sampleP.rt) <= initialRtTol Then
                            matchedPairs.Add(Tuple.Create(sampleP.rt, refP.rt))
                        End If
                    Next
                Next

                ' 如果匹配峰对太少，逐步放宽RT容差
                Dim rtTolMultiplier As Double = 2.0
                While matchedPairs.Count < 10 AndAlso rtTolMultiplier <= 8.0
                    matchedPairs.Clear()
                    Dim expandedRtTol As Double = params.rtTolerance * rtTolMultiplier

                    For Each refP In refPeaks
                        For Each sampleP In kv.Value
                            Dim mzTol As Double = GetMzTolerance(refP.mz, params.mzTolerance, params.mzToleranceMode)
                            If Math.Abs(refP.mz - sampleP.mz) <= mzTol AndAlso
                               Math.Abs(refP.rt - sampleP.rt) <= expandedRtTol Then
                                matchedPairs.Add(Tuple.Create(sampleP.rt, refP.rt))
                            End If
                        Next
                    Next

                    rtTolMultiplier *= 2.0
                End While

                ' 对当前样本的所有峰进行RT校正
                Dim corrected As PeakFeature() = CType(kv.Value.Clone(), PeakFeature())

                If matchedPairs.Count >= 4 Then
                    ' 有足够的匹配峰对，进行LOESS校正
                    Dim sampleRts As Double() = matchedPairs.Select(Function(p) p.Item1).ToArray()
                    Dim refRts As Double() = matchedPairs.Select(Function(p) p.Item2).ToArray()

                    ' 去除重复的sample RT（取平均ref RT）
                    Dim uniqueSampleRts As New List(Of Double)
                    Dim uniqueRefRts As New List(Of Double)
                    Dim sortedIndices As Integer() = Enumerable.Range(0, sampleRts.Length).OrderBy(Function(i) sampleRts(i)).ToArray()

                    Dim i As Integer = 0
                    While i < sortedIndices.Length
                        Dim currentSampleRt As Double = sampleRts(sortedIndices(i))
                        Dim refSum As Double = refRts(sortedIndices(i))
                        Dim count As Integer = 1
                        i += 1
                        While i < sortedIndices.Length AndAlso Math.Abs(sampleRts(sortedIndices(i)) - currentSampleRt) < 0.001
                            refSum += refRts(sortedIndices(i))
                            count += 1
                            i += 1
                        End While
                        uniqueSampleRts.Add(currentSampleRt)
                        uniqueRefRts.Add(refSum / count)
                    End While

                    ' 计算RT偏移量：delta = rt_ref - rt_sample
                    Dim sampleRtArr As Double() = uniqueSampleRts.ToArray()
                    Dim deltaRtArr As Double() = New Double(sampleRtArr.Length - 1) {}
                    For j As Integer = 0 To sampleRtArr.Length - 1
                        deltaRtArr(j) = uniqueRefRts(j) - sampleRtArr(j)
                    Next

                    ' 使用LOESS拟合 deltaRt ~ sampleRt
                    Dim loessModel As LOESSModel = FitLOESS(sampleRtArr, deltaRtArr, params.loessSpan, params.loessDegree)

                    ' 校正每个峰的RT
                    For Each p In corrected
                        Dim deltaRt As Double = PredictLOESS(loessModel, p.rt)
                        p.rt += deltaRt
                        p.rtmin += deltaRt
                        p.rtmax += deltaRt
                    Next
                End If
                ' 如果匹配峰对不足，不做校正，保留原始RT

                correctedPeaks(kv.Key) = corrected
            Next

            ' 第三步：在校正后的数据上进行直接匹配对齐
            Return DirectMatchAlignment(correctedPeaks, params)
        End Function


        ' ========================================================================
        '   LOESS回归实现
        ' ========================================================================

        ''' <summary>
        ''' LOESS模型，存储训练数据和参数
        ''' </summary>
        Private Class LOESSModel
            Public Property X As Double()
            Public Property Y As Double()
            Public Property Span As Double
            Public Property Degree As Integer
        End Class

        ''' <summary>
        ''' 拟合LOESS模型
        ''' 
        ''' LOESS（LOcally Estimated Scatterplot Smoothing）是一种非参数的局部回归方法。
        ''' 对于每个预测点，在其邻域内使用加权最小二乘法拟合一个低阶多项式，
        ''' 权重随距离增加而减小（使用三立方核函数）。
        ''' </summary>
        ''' <param name="x">自变量数组</param>
        ''' <param name="y">因变量数组</param>
        ''' <param name="span">带宽参数（0~1），控制邻域大小</param>
        ''' <param name="degree">多项式阶数（1或2）</param>
        ''' <returns>LOESS模型</returns>
        Private Function FitLOESS(x As Double(), y As Double(), span As Double, degree As Integer) As LOESSModel
            If x.Length <> y.Length Then
                Throw New ArgumentException("x和y数组长度必须相同")
            End If

            ' 按x排序
            Dim indices As Integer() = Enumerable.Range(0, x.Length).OrderBy(Function(i) x(i)).ToArray()
            Dim sortedX As Double() = New Double(x.Length - 1) {}
            Dim sortedY As Double() = New Double(y.Length - 1) {}

            For i As Integer = 0 To indices.Length - 1
                sortedX(i) = x(indices(i))
                sortedY(i) = y(indices(i))
            Next

            Dim model As New LOESSModel()
            model.X = sortedX
            model.Y = sortedY
            model.Span = Math.Max(0.1, Math.Min(1.0, span))
            model.Degree = degree

            Return model
        End Function

        ''' <summary>
        ''' 使用LOESS模型进行预测
        ''' </summary>
        ''' <param name="model">LOESS模型</param>
        ''' <param name="x0">预测点</param>
        ''' <returns>预测值</returns>
        Private Function PredictLOESS(model As LOESSModel, x0 As Double) As Double
            Dim n As Integer = model.X.Length
            Dim k As Integer = CInt(Math.Ceiling(model.Span * n))
            k = Math.Max(k, model.Degree + 1) ' 确保有足够的点拟合多项式
            k = Math.Min(k, n)

            ' 找到距离x0最近的k个点
            Dim distances As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                distances(i) = Math.Abs(model.X(i) - x0)
            Next

            Dim sortedIndices As Integer() = Enumerable.Range(0, n).OrderBy(Function(i) distances(i)).Take(k).ToArray()
            Dim maxDist As Double = distances(sortedIndices(k - 1))

            ' 避免除零
            If maxDist < Double.Epsilon Then
                ' x0与某个训练点完全重合
                For i As Integer = 0 To k - 1
                    If distances(sortedIndices(i)) < Double.Epsilon Then
                        Return model.Y(sortedIndices(i))
                    End If
                Next
                Return model.Y(sortedIndices(0))
            End If

            ' 计算三立方核权重
            Dim weights As Double() = New Double(k - 1) {}
            For i As Integer = 0 To k - 1
                Dim u As Double = distances(sortedIndices(i)) / maxDist
                ' 三立方核函数: w(u) = (1 - u^3)^3
                weights(i) = Math.Pow(1.0 - Math.Pow(u, 3), 3)
            Next

            ' 加权最小二乘法拟合多项式
            Dim xPts As Double() = New Double(k - 1) {}
            Dim yPts As Double() = New Double(k - 1) {}
            For i As Integer = 0 To k - 1
                xPts(i) = model.X(sortedIndices(i))
                yPts(i) = model.Y(sortedIndices(i))
            Next

            ' 使用加权最小二乘法
            Dim coeffs As Double() = WeightedPolynomialFit(xPts, yPts, weights, model.Degree)

            ' 在x0处预测
            Dim y0 As Double = 0.0
            For j As Integer = 0 To coeffs.Length - 1
                y0 += coeffs(j) * Math.Pow(x0, j)
            Next

            Return y0
        End Function

        ''' <summary>
        ''' 加权多项式拟合
        ''' 使用正规方程求解加权最小二乘问题
        ''' </summary>
        Private Function WeightedPolynomialFit(x As Double(), y As Double(), w As Double(), degree As Integer) As Double()
            Dim n As Integer = x.Length
            Dim m As Integer = degree + 1 ' 系数个数

            ' 构建正规方程 (X^T W X) * beta = X^T W y
            Dim xtwx As Double(,) = New Double(m - 1, m - 1) {}
            Dim xtwy As Double() = New Double(m - 1) {}

            For i As Integer = 0 To n - 1
                Dim xi As Double() = New Double(m - 1) {}
                For j As Integer = 0 To m - 1
                    xi(j) = Math.Pow(x(i), j)
                Next

                For j As Integer = 0 To m - 1
                    xtwy(j) += w(i) * xi(j) * y(i)
                    For k As Integer = 0 To m - 1
                        xtwx(j, k) += w(i) * xi(j) * xi(k)
                    Next
                Next
            Next

            ' 使用高斯消元法求解线性方程组
            Return SolveLinearSystem(xtwx, xtwy)
        End Function

        ''' <summary>
        ''' 高斯消元法求解线性方程组 Ax = b
        ''' </summary>
        Private Function SolveLinearSystem(A As Double(,), b As Double()) As Double()
            Dim n As Integer = b.Length
            ' 创建增广矩阵
            Dim aug As Double(,) = New Double(n - 1, n) {}

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    aug(i, j) = A(i, j)
                Next
                aug(i, n) = b(i)
            Next

            ' 前向消元（部分主元选取）
            For col As Integer = 0 To n - 1
                ' 找到主元
                Dim maxRow As Integer = col
                Dim maxVal As Double = Math.Abs(aug(col, col))
                For row As Integer = col + 1 To n - 1
                    If Math.Abs(aug(row, col)) > maxVal Then
                        maxVal = Math.Abs(aug(row, col))
                        maxRow = row
                    End If
                Next

                ' 交换行
                If maxRow <> col Then
                    For j As Integer = col To n
                        Dim temp As Double = aug(col, j)
                        aug(col, j) = aug(maxRow, j)
                        aug(maxRow, j) = temp
                    Next
                End If

                ' 消元
                Dim pivot As Double = aug(col, col)
                If Math.Abs(pivot) < Double.Epsilon Then
                    ' 奇异矩阵，返回零向量
                    Dim result As Double() = New Double(n - 1) {}
                    Return result
                End If

                For row As Integer = col + 1 To n - 1
                    Dim factor As Double = aug(row, col) / pivot
                    For j As Integer = col To n
                        aug(row, j) -= factor * aug(col, j)
                    Next
                Next
            Next

            ' 回代
            Dim x As Double() = New Double(n - 1) {}
            For i As Integer = n - 1 To 0 Step -1
                x(i) = aug(i, n)
                For j As Integer = i + 1 To n - 1
                    x(i) -= aug(i, j) * x(j)
                Next
                x(i) /= aug(i, i)
            Next

            Return x
        End Function

    End Module
End Namespace