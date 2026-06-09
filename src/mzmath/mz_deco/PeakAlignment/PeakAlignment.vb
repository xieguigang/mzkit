
Namespace PeakAlignment

    ''' <summary>
    ''' 代谢组学峰对齐算法模块
    ''' 
    ''' 本模块实现了四种主流的代谢组学峰对齐算法，用于解决不同样本原始数据间的保留时间漂移问题：
    '''   1. 直接匹配对齐（DirectMatch）—— 基于m/z和RT容差的简单匹配
    '''   2. LOESS保留时间校正对齐 —— 基于局部加权回归的RT校正后匹配
    '''   3. Obiwarp动态时间规整对齐 —— 基于色谱轮廓动态时间规整的RT校正
    '''   4. 密度分组对齐（DensityGroup）—— 类似XCMS的核密度估计分组方法
    ''' 
    ''' 输入：Dictionary(Of String, PeakFeature())，键名为原始数据文件名，键值为对应文件中提取的所有离子峰
    ''' 输出：IonExpression()，对齐后的峰面积表达矩阵
    ''' </summary>
    Public Module Algorithm
        ''' <summary>
        ''' 执行峰对齐，将各样本的离子峰在保留时间上对齐并生成峰面积表达矩阵
        ''' </summary>
        ''' <param name="peaks">各样本的离子峰字典，键名为原始数据文件名</param>
        ''' <param name="params">对齐参数配置</param>
        ''' <returns>对齐后的峰面积表达矩阵</returns>
        Public Function AlignPeaks(peaks As Dictionary(Of String, PeakFeature()),
                                params As AlignmentParameters) As IonExpression()
            If peaks Is Nothing OrElse peaks.Count = 0 Then
                Return New IonExpression() {}
            End If

            ' 过滤空样本
            Dim validPeaks As New Dictionary(Of String, PeakFeature())
            For Each kv In peaks
                If kv.Value IsNot Nothing AndAlso kv.Value.Length > 0 Then
                    validPeaks(kv.Key) = kv.Value
                End If
            Next

            If validPeaks.Count = 0 Then
                Return New IonExpression() {}
            End If

            ' 如果只有一个样本，直接转换
            If validPeaks.Count = 1 Then
                Return SingleSampleToExpression(validPeaks.First)
            End If

            ' 根据算法类型选择对齐方法
            Dim alignedGroups As List(Of AlignedPeakGroup)

            Select Case params.method
                Case AlignmentMethod.DirectMatch
                    alignedGroups = DirectMatchAlignment(validPeaks, params)
                Case AlignmentMethod.LOESS
                    alignedGroups = LOESSAlignment(validPeaks, params)
                Case AlignmentMethod.Obiwarp
                    alignedGroups = ObiwarpAlignment(validPeaks, params)
                Case AlignmentMethod.DensityGroup
                    alignedGroups = DensityGroupAlignment(validPeaks, params)
                Case Else
                    alignedGroups = DensityGroupAlignment(validPeaks, params)
            End Select

            ' 缺失值填充
            If params.fillGaps Then
                FillGaps(alignedGroups, validPeaks, params)
            End If

            ' 按最小样本比例过滤
            alignedGroups = FilterByMinFraction(alignedGroups, validPeaks.Keys.ToList, params.minFraction)

            ' 转换为IonExpression数组
            Return ConvertToIonExpression(alignedGroups, validPeaks.Keys.ToList)
        End Function

        ''' <summary>
        ''' 便捷方法：使用默认参数执行密度分组对齐
        ''' </summary>
        Public Function AlignPeaks(peaks As Dictionary(Of String, PeakFeature())) As IonExpression()
            Return AlignPeaks(peaks, New AlignmentParameters())
        End Function

        ' ========================================================================
        '   算法1：直接匹配对齐（DirectMatch）
        ' ========================================================================

        ''' <summary>
        ''' 直接匹配对齐算法
        ''' 
        ''' 原理：将所有样本的离子峰按照m/z和RT容差进行全局匹配。
        ''' 首先按m/z对所有峰排序并分组，然后在每个m/z组内按RT进行层次聚类，
        ''' 将RT差异小于容差的峰归为同一个对齐组。
        ''' 
        ''' 优点：实现简单，计算速度快
        ''' 缺点：无法处理非线性的保留时间漂移，对大范围RT偏移效果较差
        ''' </summary>
        Private Function DirectMatchAlignment(peaks As Dictionary(Of String, PeakFeature()),
                                               params As AlignmentParameters) As List(Of AlignedPeakGroup)
            ' 第一步：收集所有样本的所有峰，并标记来源
            Dim allPeaks As New List(Of Tuple(Of String, PeakFeature))
            For Each kv In peaks
                For Each p In kv.Value
                    allPeaks.Add(Tuple.Create(kv.Key, p))
                Next
            Next

            ' 第二步：按m/z排序
            allPeaks = allPeaks.OrderBy(Function(t) t.Item2.mz).ToList()

            ' 第三步：按m/z容差分组
            Dim mzGroups As New List(Of List(Of Tuple(Of String, PeakFeature)))
            If allPeaks.Count = 0 Then
                Return New List(Of AlignedPeakGroup)
            End If

            Dim currentGroup As New List(Of Tuple(Of String, PeakFeature))
            currentGroup.Add(allPeaks(0))

            For i As Integer = 1 To allPeaks.Count - 1
                Dim mzDiff As Double = allPeaks(i).Item2.mz - currentGroup(0).Item2.mz
                Dim tol As Double = GetMzTolerance(currentGroup(0).Item2.mz, params.mzTolerance, params.mzToleranceMode)

                If mzDiff <= tol Then
                    currentGroup.Add(allPeaks(i))
                Else
                    mzGroups.Add(currentGroup)
                    currentGroup = New List(Of Tuple(Of String, PeakFeature))
                    currentGroup.Add(allPeaks(i))
                End If
            Next
            mzGroups.Add(currentGroup)

            ' 第四步：在每个m/z组内按RT进行层次聚类
            Dim result As New List(Of AlignedPeakGroup)

            For Each mzGroup In mzGroups
                ' 检查是否包含多个样本的峰（避免同一样本多个峰归为一组）
                If mzGroup.Count = 1 Then
                    Dim g As New AlignedPeakGroup()
                    g.avgMz = mzGroup(0).Item2.mz
                    g.minMz = mzGroup(0).Item2.mz
                    g.maxMz = mzGroup(0).Item2.mz
                    g.avgRt = mzGroup(0).Item2.rt
                    g.minRt = mzGroup(0).Item2.rtmin
                    g.maxRt = mzGroup(0).Item2.rtmax
                    g.sampleAreas(mzGroup(0).Item1) = mzGroup(0).Item2.area
                    g.samplePeaks(mzGroup(0).Item1) = mzGroup(0).Item2
                    result.Add(g)
                    Continue For
                End If

                ' 按RT排序
                Dim sortedByRt = mzGroup.OrderBy(Function(t) t.Item2.rt).ToList()

                ' 层次聚类：将RT差异小于容差的相邻峰归为一组
                Dim rtClusters As New List(Of List(Of Tuple(Of String, PeakFeature)))
                Dim currentCluster As New List(Of Tuple(Of String, PeakFeature))
                currentCluster.Add(sortedByRt(0))

                For j As Integer = 1 To sortedByRt.Count - 1
                    Dim rtDiff As Double = sortedByRt(j).Item2.rt - currentCluster(0).Item2.rt
                    If rtDiff <= params.rtTolerance Then
                        currentCluster.Add(sortedByRt(j))
                    Else
                        rtClusters.Add(currentCluster)
                        currentCluster = New List(Of Tuple(Of String, PeakFeature))
                        currentCluster.Add(sortedByRt(j))
                    End If
                Next
                rtClusters.Add(currentCluster)

                ' 将每个RT聚类转换为AlignedPeakGroup
                For Each cluster In rtClusters
                    ' 检查是否有同一样本的多个峰，取面积最大的
                    Dim sampleBest As New Dictionary(Of String, PeakFeature)
                    For Each item In cluster
                        If Not sampleBest.ContainsKey(item.Item1) Then
                            sampleBest(item.Item1) = item.Item2
                        Else
                            If item.Item2.area > sampleBest(item.Item1).area Then
                                sampleBest(item.Item1) = item.Item2
                            End If
                        End If
                    Next

                    Dim g As New AlignedPeakGroup()
                    Dim mzSum As Double = 0
                    Dim rtSum As Double = 0
                    g.minMz = Double.MaxValue
                    g.maxMz = Double.MinValue
                    g.minRt = Double.MaxValue
                    g.maxRt = Double.MinValue

                    For Each kv In sampleBest
                        mzSum += kv.Value.mz
                        rtSum += kv.Value.rt
                        g.minMz = Math.Min(g.minMz, kv.Value.mz)
                        g.maxMz = Math.Max(g.maxMz, kv.Value.mz)
                        g.minRt = Math.Min(g.minRt, kv.Value.rtmin)
                        g.maxRt = Math.Max(g.maxRt, kv.Value.rtmax)
                        g.sampleAreas(kv.Key) = kv.Value.area
                        g.samplePeaks(kv.Key) = kv.Value
                    Next

                    g.avgMz = mzSum / sampleBest.Count
                    g.avgRt = rtSum / sampleBest.Count
                    result.Add(g)
                Next
            Next

            Return result
        End Function

        ' ========================================================================
        '   算法2：LOESS保留时间校正对齐
        ' ========================================================================

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
        Private Function LOESSAlignment(peaks As Dictionary(Of String, PeakFeature()),
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
        '   算法3：Obiwarp动态时间规整对齐
        ' ========================================================================

        ''' <summary>
        ''' Obiwarp动态时间规整对齐算法
        ''' 
        ''' 原理：
        ''' 1. 为每个样本构建TIC色谱轮廓（将峰按RT分段累加响应强度）
        ''' 2. 选择参考样本
        ''' 3. 使用动态时间规整（DTW）算法找到参考色谱与样本色谱之间的最优对齐路径
        ''' 4. 根据对齐路径计算RT校正函数
        ''' 5. 校正每个样本中所有峰的RT值
        ''' 6. 在校正后的RT空间中进行直接匹配对齐
        ''' 
        ''' 优点：不依赖峰匹配，直接利用色谱轮廓信息，对峰缺失较鲁棒
        ''' 缺点：计算量较大，TIC轮廓质量影响对齐效果
        ''' </summary>
        Private Function ObiwarpAlignment(peaks As Dictionary(Of String, PeakFeature()),
                                           params As AlignmentParameters) As List(Of AlignedPeakGroup)
            ' 第一步：选择参考样本
            Dim refName As String = SelectReferenceSample(peaks, params.referenceSample)

            ' 第二步：为每个样本构建TIC色谱轮廓
            Dim allSampleNames As List(Of String) = peaks.Keys.ToList()
            Dim rtRange As Tuple(Of Double, Double) = GetGlobalRTRange(peaks)
            Dim rtMin As Double = rtRange.Item1
            Dim rtMax As Double = rtRange.Item2

            ' 构建分段TIC轮廓
            Dim profiles As New Dictionary(Of String, Double())
            Dim rtBins As Double()

            For Each kv In peaks
                Dim profile As Tuple(Of Double(), Double()) = BuildTICProfile(kv.Value, rtMin, rtMax, params.obiwarpBinSize)
                profiles(kv.Key) = profile.Item1
                rtBins = profile.Item2
            Next

            ' 第三步：对每个非参考样本进行DTW对齐
            Dim correctedPeaks As New Dictionary(Of String, PeakFeature())
            correctedPeaks(refName) = peaks(refName)

            Dim refProfile As Double() = profiles(refName)

            For Each kv In peaks
                If kv.Key = refName Then Continue For

                Dim sampleProfile As Double() = profiles(kv.Key)

                ' 计算DTW最优路径
                Dim warpPath As List(Of Tuple(Of Integer, Integer)) = ComputeDTW(refProfile, sampleProfile, params.obiwarpGapPenalty)

                ' 从warp路径构建RT校正函数
                ' warpPath中的每一项为 (refBinIndex, sampleBinIndex)
                ' 表示参考色谱的第refBinIndex个分段对应样本色谱的第sampleBinIndex个分段
                Dim rtCorrection As Func(Of Double, Double) = BuildRTCorrectionFromWarpPath(
                    warpPath, rtBins, rtMin, params.obiwarpBinSize)

                ' 校正每个峰的RT
                Dim corrected As PeakFeature() = CType(kv.Value.Clone(), PeakFeature())
                For Each p In corrected
                    Dim deltaRt As Double = rtCorrection(p.rt) - p.rt
                    p.rt += deltaRt
                    p.rtmin += deltaRt
                    p.rtmax += deltaRt
                Next

                correctedPeaks(kv.Key) = corrected
            Next

            ' 第四步：在校正后的数据上进行直接匹配对齐
            Return DirectMatchAlignment(correctedPeaks, params)
        End Function

        ' ========================================================================
        '   算法4：密度分组对齐（XCMS风格）
        ' ========================================================================

        ''' <summary>
        ''' 密度分组对齐算法（XCMS风格）
        ''' 
        ''' 原理：
        ''' 1. 将所有样本的离子峰按m/z排序并分组（m/z容差内）
        ''' 2. 在每个m/z组内，使用核密度估计（KDE）计算所有峰RT值的密度分布
        ''' 3. 在密度分布中寻找局部极大值，每个极大值代表一个对齐后的RT特征
        ''' 4. 将每个峰分配到最近的密度峰，形成对齐组
        ''' 5. 按最小样本比例过滤低质量特征
        ''' 
        ''' 优点：统计方法稳健，能够自动识别对齐特征，是XCMS等主流软件的默认方法
        ''' 缺点：带宽参数选择影响结果，计算量中等
        ''' </summary>
        Private Function DensityGroupAlignment(peaks As Dictionary(Of String, PeakFeature()),
                                                params As AlignmentParameters) As List(Of AlignedPeakGroup)
            ' 第一步：收集所有峰并按m/z排序
            Dim allPeaks As New List(Of Tuple(Of String, PeakFeature))
            For Each kv In peaks
                For Each p In kv.Value
                    allPeaks.Add(Tuple.Create(kv.Key, p))
                Next
            Next

            allPeaks = allPeaks.OrderBy(Function(t) t.Item2.mz).ToList()

            If allPeaks.Count = 0 Then
                Return New List(Of AlignedPeakGroup)
            End If

            ' 第二步：按m/z容差分组（使用滑动窗口方法）
            Dim mzGroups As New List(Of List(Of Tuple(Of String, PeakFeature)))
            Dim currentMzGroup As New List(Of Tuple(Of String, PeakFeature))
            currentMzGroup.Add(allPeaks(0))

            For i As Integer = 1 To allPeaks.Count - 1
                ' 使用组内中位数m/z作为参考
                Dim medianMz As Double = ComputeMedian(currentMzGroup.Select(Function(t) t.Item2.mz).ToList())
                Dim tol As Double = GetMzTolerance(medianMz, params.mzTolerance, params.mzToleranceMode)

                If allPeaks(i).Item2.mz - medianMz <= tol Then
                    currentMzGroup.Add(allPeaks(i))
                Else
                    mzGroups.Add(currentMzGroup)
                    currentMzGroup = New List(Of Tuple(Of String, PeakFeature))
                    currentMzGroup.Add(allPeaks(i))
                End If
            Next
            mzGroups.Add(currentMzGroup)

            ' 第三步：在每个m/z组内进行核密度估计分组
            Dim result As New List(Of AlignedPeakGroup)

            For Each mzGroup In mzGroups
                ' 收集该m/z组内所有峰的RT值
                Dim rtValues As Double() = mzGroup.Select(Function(t) t.Item2.rt).ToArray()

                If rtValues.Length = 1 Then
                    ' 只有一个峰，直接作为一个对齐组
                    Dim g As New AlignedPeakGroup()
                    g.avgMz = mzGroup(0).Item2.mz
                    g.minMz = mzGroup(0).Item2.mz
                    g.maxMz = mzGroup(0).Item2.mz
                    g.avgRt = mzGroup(0).Item2.rt
                    g.minRt = mzGroup(0).Item2.rtmin
                    g.maxRt = mzGroup(0).Item2.rtmax
                    g.sampleAreas(mzGroup(0).Item1) = mzGroup(0).Item2.area
                    g.samplePeaks(mzGroup(0).Item1) = mzGroup(0).Item2
                    result.Add(g)
                    Continue For
                End If

                ' 确定核密度带宽
                Dim bw As Double = params.densityBandwidth
                If bw <= 0 Then
                    ' 使用Silverman法则自动估计带宽
                    bw = SilvermanBandwidth(rtValues)
                    ' 确保带宽不小于RT容差的一半
                    bw = Math.Max(bw, params.rtTolerance * 0.5)
                End If

                ' 在RT范围内构建密度估计网格
                Dim rtMinGroup As Double = rtValues.Min()
                Dim rtMaxGroup As Double = rtValues.Max()
                Dim rtRangeGroup As Double = rtMaxGroup - rtMinGroup
                ' 扩展范围以避免边界效应
                rtMinGroup -= bw * 3
                rtMaxGroup += bw * 3

                Dim nGrid As Integer = CInt(Math.Max(512, (rtMaxGroup - rtMinGroup) / (bw * 0.1)))
                Dim gridStep As Double = (rtMaxGroup - rtMinGroup) / nGrid
                Dim densityGrid As Double() = New Double(nGrid) {}
                Dim gridRts As Double() = New Double(nGrid) {}

                ' 计算核密度估计（高斯核）
                For gi As Integer = 0 To nGrid
                    Dim gridRt As Double = rtMinGroup + gi * gridStep
                    gridRts(gi) = gridRt
                    Dim density As Double = 0.0

                    For Each rt In rtValues
                        Dim u As Double = (gridRt - rt) / bw
                        ' 高斯核函数
                        density += Math.Exp(-0.5 * u * u) / (bw * Math.Sqrt(2.0 * Math.PI))
                    Next

                    densityGrid(gi) = density / rtValues.Length
                Next

                ' 在密度估计中寻找局部极大值
                Dim densityPeaks As New List(Of Integer)
                For gi As Integer = 1 To nGrid - 1
                    If densityGrid(gi) > densityGrid(gi - 1) AndAlso
                       densityGrid(gi) > densityGrid(gi + 1) Then
                        ' 检查密度值是否足够大（避免噪声峰）
                        Dim threshold As Double = densityGrid.Max() * 0.05
                        If densityGrid(gi) >= threshold Then
                            densityPeaks.Add(gi)
                        End If
                    End If
                Next

                ' 如果没有找到密度峰，使用密度最大值位置
                If densityPeaks.Count = 0 Then
                    Dim maxIdx As Integer = 0
                    For gi As Integer = 1 To nGrid
                        If densityGrid(gi) > densityGrid(maxIdx) Then
                            maxIdx = gi
                        End If
                    Next
                    densityPeaks.Add(maxIdx)
                End If

                ' 合并距离过近的密度峰（小于RT容差的一半）
                Dim mergedPeaks As New List(Of Integer)
                mergedPeaks.Add(densityPeaks(0))
                For pi As Integer = 1 To densityPeaks.Count - 1
                    If gridRts(densityPeaks(pi)) - gridRts(mergedPeaks.Last) > params.rtTolerance * 0.5 Then
                        mergedPeaks.Add(densityPeaks(pi))
                    End If
                Next
                densityPeaks = mergedPeaks

                ' 将每个峰分配到最近的密度峰
                Dim peakAssignments As Integer() = New Integer(mzGroup.Count - 1) {}
                For pi As Integer = 0 To mzGroup.Count - 1
                    Dim peakRt As Double = mzGroup(pi).Item2.rt
                    Dim bestPeakIdx As Integer = 0
                    Dim bestDist As Double = Double.MaxValue

                    For di As Integer = 0 To densityPeaks.Count - 1
                        Dim dist As Double = Math.Abs(peakRt - gridRts(densityPeaks(di)))
                        If dist < bestDist Then
                            bestDist = dist
                            bestPeakIdx = di
                        End If
                    Next

                    peakAssignments(pi) = bestPeakIdx
                Next

                ' 按密度峰分组创建AlignedPeakGroup
                For di As Integer = 0 To densityPeaks.Count - 1
                    Dim groupPeaks As New List(Of Tuple(Of String, PeakFeature))
                    For pi As Integer = 0 To mzGroup.Count - 1
                        If peakAssignments(pi) = di Then
                            groupPeaks.Add(mzGroup(pi))
                        End If
                    Next

                    If groupPeaks.Count = 0 Then Continue For

                    ' 处理同一样本可能有多个峰的情况，取面积最大的
                    Dim sampleBest As New Dictionary(Of String, PeakFeature)
                    For Each item In groupPeaks
                        If Not sampleBest.ContainsKey(item.Item1) Then
                            sampleBest(item.Item1) = item.Item2
                        Else
                            If item.Item2.area > sampleBest(item.Item1).area Then
                                sampleBest(item.Item1) = item.Item2
                            End If
                        End If
                    Next

                    Dim g As New AlignedPeakGroup()
                    Dim mzSum As Double = 0
                    Dim rtSum As Double = 0
                    g.minMz = Double.MaxValue
                    g.maxMz = Double.MinValue
                    g.minRt = Double.MaxValue
                    g.maxRt = Double.MinValue

                    For Each kv In sampleBest
                        mzSum += kv.Value.mz
                        rtSum += kv.Value.rt
                        g.minMz = Math.Min(g.minMz, kv.Value.mz)
                        g.maxMz = Math.Max(g.maxMz, kv.Value.mz)
                        g.minRt = Math.Min(g.minRt, kv.Value.rtmin)
                        g.maxRt = Math.Max(g.maxRt, kv.Value.rtmax)
                        g.sampleAreas(kv.Key) = kv.Value.area
                        g.samplePeaks(kv.Key) = kv.Value
                    Next

                    g.avgMz = mzSum / sampleBest.Count
                    g.avgRt = rtSum / sampleBest.Count
                    result.Add(g)
                Next
            Next

            Return result
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

        ' ========================================================================
        '   DTW（动态时间规整）实现
        ' ========================================================================

        ''' <summary>
        ''' 计算两个序列之间的DTW最优对齐路径
        ''' 
        ''' 使用带约束的DTW算法，通过Sakoe-Chiba带限制搜索范围，
        ''' 减少计算量并避免不合理的对齐。
        ''' </summary>
        ''' <param name="refProfile">参考色谱轮廓</param>
        ''' <param name="sampleProfile">样本色谱轮廓</param>
        ''' <param name="gapPenalty">间隙惩罚值</param>
        ''' <returns>最优对齐路径，每项为(参考索引, 样本索引)</returns>
        Private Function ComputeDTW(refProfile As Double(), sampleProfile As Double(),
                                     gapPenalty As Double) As List(Of Tuple(Of Integer, Integer))
            Dim nRef As Integer = refProfile.Length
            Dim nSample As Integer = sampleProfile.Length

            If nRef = 0 OrElse nSample = 0 Then
                Return New List(Of Tuple(Of Integer, Integer))
            End If

            ' 归一化轮廓
            Dim normRef As Double() = NormalizeProfile(refProfile)
            Dim normSample As Double() = NormalizeProfile(sampleProfile)

            ' 代价矩阵
            Dim cost As Double(,) = New Double(nRef - 1, nSample - 1) {}
            ' 累积代价矩阵
            Dim dtw As Double(,) = New Double(nRef - 1, nSample - 1) {}
            ' 路径追踪矩阵
            Dim traceback As Integer(,) = New Integer(nRef - 1, nSample - 1) {}
            ' 0: 对角线, 1: 上方, 2: 左方

            ' 计算局部代价（使用欧氏距离）
            For i As Integer = 0 To nRef - 1
                For j As Integer = 0 To nSample - 1
                    cost(i, j) = Math.Abs(normRef(i) - normSample(j))
                Next
            Next

            ' 初始化
            dtw(0, 0) = cost(0, 0)
            traceback(0, 0) = 0

            ' 填充第一行
            For j As Integer = 1 To nSample - 1
                dtw(0, j) = dtw(0, j - 1) + cost(0, j) + gapPenalty
                traceback(0, j) = 2 ' 左方
            Next

            ' 填充第一列
            For i As Integer = 1 To nRef - 1
                dtw(i, 0) = dtw(i - 1, 0) + cost(i, 0) + gapPenalty
                traceback(i, 0) = 1 ' 上方
            Next

            ' 填充其余部分
            For i As Integer = 1 To nRef - 1
                ' Sakoe-Chiba带约束，限制搜索宽度为对角线附近
                Dim bandWidth As Integer = CInt(Math.Max(10, Math.Ceiling(nSample * 0.1)))
                Dim jStart As Integer = Math.Max(1, i - bandWidth)
                Dim jEnd As Integer = Math.Min(nSample - 1, i + bandWidth)

                For j As Integer = jStart To jEnd
                    Dim diag As Double = dtw(i - 1, j - 1) + cost(i, j)
                    Dim up As Double = dtw(i - 1, j) + cost(i, j) + gapPenalty
                    Dim left As Double = dtw(i, j - 1) + cost(i, j) + gapPenalty

                    If diag <= up AndAlso diag <= left Then
                        dtw(i, j) = diag
                        traceback(i, j) = 0
                    ElseIf up <= left Then
                        dtw(i, j) = up
                        traceback(i, j) = 1
                    Else
                        dtw(i, j) = left
                        traceback(i, j) = 2
                    End If
                Next
            Next

            ' 回溯最优路径
            Dim path As New List(Of Tuple(Of Integer, Integer))
            Dim ci As Integer = nRef - 1
            Dim cj As Integer = nSample - 1

            While ci > 0 OrElse cj > 0
                path.Add(Tuple.Create(ci, cj))

                If ci = 0 Then
                    cj -= 1
                ElseIf cj = 0 Then
                    ci -= 1
                Else
                    Select Case traceback(ci, cj)
                        Case 0 : ci -= 1 : cj -= 1
                        Case 1 : ci -= 1
                        Case 2 : cj -= 1
                    End Select
                End If
            End While
            path.Add(Tuple.Create(0, 0))

            ' 反转路径（从起点到终点）
            path.Reverse()

            Return path
        End Function

        ''' <summary>
        ''' 从DTW对齐路径构建RT校正函数
        ''' 
        ''' 将离散的对齐路径转换为连续的RT校正映射。
        ''' 对于任意给定的样本RT值，通过线性插值计算对应的参考RT值。
        ''' </summary>
        Private Function BuildRTCorrectionFromWarpPath(path As List(Of Tuple(Of Integer, Integer)),
                                                        rtBins As Double(),
                                                        rtMin As Double,
                                                        binSize As Double) As Func(Of Double, Double)
            ' 构建样本RT -> 参考RT的映射点
            Dim mappingPoints As New List(Of Tuple(Of Double, Double))

            For Each pt In path
                Dim sampleRt As Double = rtMin + pt.Item2 * binSize
                Dim refRt As Double = rtMin + pt.Item1 * binSize
                mappingPoints.Add(Tuple.Create(sampleRt, refRt))
            Next

            ' 去除重复的样本RT点（保留最后一个映射）
            Dim uniqueMapping As New List(Of Tuple(Of Double, Double))
            Dim lastSampleRt As Double = Double.NaN

            For i As Integer = mappingPoints.Count - 1 To 0 Step -1
                If Double.IsNaN(lastSampleRt) OrElse Math.Abs(mappingPoints(i).Item1 - lastSampleRt) > Double.Epsilon Then
                    uniqueMapping.Add(mappingPoints(i))
                    lastSampleRt = mappingPoints(i).Item1
                End If
            Next
            uniqueMapping.Reverse()

            ' 返回线性插值函数
            Return Function(sampleRt As Double) As Double
                       If uniqueMapping.Count = 0 Then Return sampleRt
                       If uniqueMapping.Count = 1 Then Return uniqueMapping(0).Item2

                       ' 边界处理
                       If sampleRt <= uniqueMapping(0).Item1 Then
                           Return uniqueMapping(0).Item2
                       End If
                       If sampleRt >= uniqueMapping.Last.Item1 Then
                           Return uniqueMapping.Last.Item2
                       End If

                       ' 二分查找
                       Dim lo As Integer = 0
                       Dim hi As Integer = uniqueMapping.Count - 1
                       While lo < hi - 1
                           Dim mid As Integer = (lo + hi) \ 2
                           If uniqueMapping(mid).Item1 <= sampleRt Then
                               lo = mid
                           Else
                               hi = mid
                           End If
                       End While

                       ' 线性插值
                       Dim x0 As Double = uniqueMapping(lo).Item1
                       Dim x1 As Double = uniqueMapping(hi).Item1
                       Dim y0 As Double = uniqueMapping(lo).Item2
                       Dim y1 As Double = uniqueMapping(hi).Item2

                       If Math.Abs(x1 - x0) < Double.Epsilon Then Return y0

                       Dim t As Double = (sampleRt - x0) / (x1 - x0)
                       Return y0 + t * (y1 - y0)
                   End Function
        End Function

        ' ========================================================================
        '   TIC色谱轮廓构建
        ' ========================================================================

        ''' <summary>
        ''' 从峰列表构建TIC色谱轮廓
        ''' 将保留时间范围划分为等宽的分段，在每个分段内累加所有峰的响应强度
        ''' </summary>
        ''' <returns>轮廓强度数组和分段中心RT数组</returns>
        Private Function BuildTICProfile(peaks As PeakFeature(), rtMin As Double, rtMax As Double,
                                          binSize As Double) As Tuple(Of Double(), Double())
            Dim nBins As Integer = CInt(Math.Ceiling((rtMax - rtMin) / binSize))
            nBins = Math.Max(nBins, 1)

            Dim profile As Double() = New Double(nBins - 1) {}
            Dim rtBins As Double() = New Double(nBins - 1) {}

            ' 初始化分段中心RT
            For i As Integer = 0 To nBins - 1
                rtBins(i) = rtMin + (i + 0.5) * binSize
            Next

            ' 累加每个峰的响应强度到对应的分段
            For Each p In peaks
                Dim binIdx As Integer = CInt(Math.Floor((p.rt - rtMin) / binSize))
                If binIdx >= 0 AndAlso binIdx < nBins Then
                    ' 使用高斯加权将峰强度分配到相邻分段
                    Dim centerBin As Double = (p.rt - rtMin) / binSize - 0.5
                    Dim sigma As Double = Math.Max((p.rtmax - p.rtmin) / binSize / 2.0, 1.0)

                    For b As Integer = Math.Max(0, CInt(Math.Floor(centerBin - 3 * sigma))) To Math.Min(nBins - 1, CInt(Math.Ceiling(centerBin + 3 * sigma)))
                        Dim dist As Double = b - centerBin
                        Dim weight As Double = Math.Exp(-0.5 * dist * dist / (sigma * sigma))
                        profile(b) += p.maxInto * weight
                    Next
                End If
            Next

            Return Tuple.Create(profile, rtBins)
        End Function

        ''' <summary>
        ''' 归一化轮廓到[0,1]范围
        ''' </summary>
        Private Function NormalizeProfile(profile As Double()) As Double()
            Dim minVal As Double = profile.Min()
            Dim maxVal As Double = profile.Max()
            Dim range As Double = maxVal - minVal

            If range < Double.Epsilon Then
                Dim result As Double() = New Double(profile.Length - 1) {}
                Return result
            End If

            Dim normalized As Double() = New Double(profile.Length - 1) {}
            For i As Integer = 0 To profile.Length - 1
                normalized(i) = (profile(i) - minVal) / range
            Next
            Return normalized
        End Function

        ' ========================================================================
        '   缺失值填充
        ' ========================================================================

        ''' <summary>
        ''' 缺失值填充
        ''' 
        ''' 对于对齐后的每个特征，检查哪些样本缺失该特征。
        ''' 对于缺失的样本，在原始峰列表中搜索m/z和RT在对齐特征范围内的峰，
        ''' 如果找到则使用其面积值，否则填充为零。
        ''' </summary>
        Private Sub FillGaps(groups As List(Of AlignedPeakGroup),
                              peaks As Dictionary(Of String, PeakFeature()),
                              params As AlignmentParameters)
            For Each g In groups
                For Each sampleName In peaks.Keys
                    If g.sampleAreas.ContainsKey(sampleName) Then Continue For

                    ' 在该样本中搜索匹配的峰
                    Dim bestPeak As PeakFeature = Nothing
                    Dim bestScore As Double = Double.MaxValue

                    For Each p In peaks(sampleName)
                        Dim mzTol As Double = GetMzTolerance(g.avgMz, params.mzTolerance, params.mzToleranceMode)
                        Dim mzDiff As Double = Math.Abs(p.mz - g.avgMz)
                        Dim rtDiff As Double = Math.Abs(p.rt - g.avgRt)

                        If mzDiff <= mzTol AndAlso rtDiff <= params.rtTolerance Then
                            ' 使用m/z和RT的加权距离作为评分
                            Dim score As Double = mzDiff / mzTol + rtDiff / params.rtTolerance
                            If score < bestScore Then
                                bestScore = score
                                bestPeak = p
                            End If
                        End If
                    Next

                    If bestPeak IsNot Nothing Then
                        g.sampleAreas(sampleName) = bestPeak.area
                        g.samplePeaks(sampleName) = bestPeak
                    Else
                        g.sampleAreas(sampleName) = 0.0
                    End If
                Next
            Next
        End Sub

        ' ========================================================================
        '   过滤与转换
        ' ========================================================================

        ''' <summary>
        ''' 按最小样本比例过滤对齐特征
        ''' 只有在至少minFraction比例的样本中出现的特征才会被保留
        ''' </summary>
        Private Function FilterByMinFraction(groups As List(Of AlignedPeakGroup),
                                              sampleNames As List(Of String),
                                              minFraction As Double) As List(Of AlignedPeakGroup)
            Dim minSamples As Integer = CInt(Math.Ceiling(minFraction * sampleNames.Count))
            minSamples = Math.Max(minSamples, 1)

            Return groups.Where(Function(g) g.sampleAreas.Count(Function(kv) kv.Value > 0) >= minSamples).ToList()
        End Function

        ''' <summary>
        ''' 将对齐峰组列表转换为IonExpression数组
        ''' </summary>
        Private Function ConvertToIonExpression(groups As List(Of AlignedPeakGroup),
                                                 sampleNames As List(Of String)) As IonExpression()
            Dim result As IonExpression() = New IonExpression(groups.Count - 1) {}

            For i As Integer = 0 To groups.Count - 1
                Dim g As AlignedPeakGroup = groups(i)
                Dim expr As New IonExpression()

                ' 生成特征ID：M{m/z}T{rt}
                expr.ID = String.Format("M{0:F4}T{1:F1}", g.avgMz, g.avgRt)
                expr.mz = g.avgMz
                expr.mzmin = g.minMz
                expr.mzmax = g.maxMz
                expr.rt = g.avgRt
                expr.rtmin = g.minRt
                expr.rtmax = g.maxRt

                ' 填充各样本的峰面积
                expr.SampleAreas = New Dictionary(Of String, Double)()
                For Each sampleName In sampleNames
                    If g.sampleAreas.ContainsKey(sampleName) Then
                        expr.SampleAreas(sampleName) = g.sampleAreas(sampleName)
                    Else
                        expr.SampleAreas(sampleName) = 0.0
                    End If
                Next

                result(i) = expr
            Next

            ' 按m/z排序输出
            Array.Sort(result, Function(a, b) a.mz.CompareTo(b.mz))

            ' 重新编号ID
            For i As Integer = 0 To result.Length - 1
                result(i).ID = String.Format("F{0:D5}", i + 1)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 单样本直接转换为IonExpression数组
        ''' </summary>
        Private Function SingleSampleToExpression(kv As KeyValuePair(Of String, PeakFeature())) As IonExpression()
            Dim result As IonExpression() = New IonExpression(kv.Value.Length - 1) {}

            For i As Integer = 0 To kv.Value.Length - 1
                Dim p As PeakFeature = kv.Value(i)
                Dim expr As New IonExpression()

                expr.ID = String.Format("F{0:D5}", i + 1)
                expr.mz = p.mz
                expr.mzmin = p.mz
                expr.mzmax = p.mz
                expr.rt = p.rt
                expr.rtmin = p.rtmin
                expr.rtmax = p.rtmax

                expr.SampleAreas = New Dictionary(Of String, Double)()
                expr.SampleAreas(kv.Key) = p.area

                result(i) = expr
            Next

            Array.Sort(result, Function(a, b) a.mz.CompareTo(b.mz))

            Return result
        End Function

        ' ========================================================================
        '   通用辅助函数
        ' ========================================================================

        ''' <summary>
        ''' 计算m/z容差值
        ''' </summary>
        ''' <param name="mz">当前m/z值</param>
        ''' <param name="tolerance">容差参数</param>
        ''' <param name="mode">容差模式</param>
        ''' <returns>绝对容差值（Da）</returns>
        Private Function GetMzTolerance(mz As Double, tolerance As Double, mode As ToleranceMode) As Double
            Select Case mode
                Case ToleranceMode.Absolute
                    Return tolerance
                Case ToleranceMode.PPM
                    Return mz * tolerance / 1000000.0
                Case Else
                    Return tolerance
            End Select
        End Function

        ''' <summary>
        ''' 选择参考样本
        ''' 
        ''' 策略：选择峰数量最多的样本作为参考样本。
        ''' 如果指定了参考样本名称且存在，则使用指定的样本。
        ''' </summary>
        Private Function SelectReferenceSample(peaks As Dictionary(Of String, PeakFeature()),
                                                preferredRef As String) As String
            ' 如果指定了参考样本且存在，直接使用
            If Not String.IsNullOrEmpty(preferredRef) AndAlso peaks.ContainsKey(preferredRef) Then
                Return preferredRef
            End If

            ' 选择峰数量最多的样本
            Dim bestSample As String = peaks.First.Key
            Dim bestCount As Integer = peaks.First.Value.Length

            For Each kv In peaks
                If kv.Value.Length > bestCount Then
                    bestCount = kv.Value.Length
                    bestSample = kv.Key
                End If
            Next

            Return bestSample
        End Function

        ''' <summary>
        ''' 获取所有样本的全局RT范围
        ''' </summary>
        Private Function GetGlobalRTRange(peaks As Dictionary(Of String, PeakFeature())) As Tuple(Of Double, Double)
            Dim rtMin As Double = Double.MaxValue
            Dim rtMax As Double = Double.MinValue

            For Each kv In peaks
                For Each p In kv.Value
                    rtMin = Math.Min(rtMin, p.rtmin)
                    rtMax = Math.Max(rtMax, p.rtmax)
                Next
            Next

            If rtMin = Double.MaxValue Then
                rtMin = 0.0
                rtMax = 1.0
            End If

            Return Tuple.Create(rtMin, rtMax)
        End Function

        ''' <summary>
        ''' 计算中位数
        ''' </summary>
        Private Function ComputeMedian(values As List(Of Double)) As Double
            If values.Count = 0 Then Return 0.0

            Dim sorted As List(Of Double) = values.OrderBy(Function(v) v).ToList()
            Dim mid As Integer = sorted.Count \ 2

            If sorted.Count Mod 2 = 0 Then
                Return (sorted(mid - 1) + sorted(mid)) / 2.0
            Else
                Return sorted(mid)
            End If
        End Function

        ''' <summary>
        ''' Silverman法则估计核密度带宽
        ''' h = 0.9 * min(σ, IQR/1.34) * n^(-1/5)
        ''' </summary>
        Private Function SilvermanBandwidth(values As Double()) As Double
            If values.Length < 2 Then Return 1.0

            Dim n As Integer = values.Length
            Dim sorted As Double() = values.OrderBy(Function(v) v).ToArray()

            ' 计算标准差
            Dim mean As Double = values.Average()
            Dim variance As Double = 0.0
            For Each v In values
                variance += (v - mean) * (v - mean)
            Next
            variance /= n
            Dim stdDev As Double = Math.Sqrt(variance)

            ' 计算四分位距（IQR）
            Dim q1Idx As Integer = CInt(Math.Floor(n * 0.25))
            Dim q3Idx As Integer = CInt(Math.Floor(n * 0.75))
            q1Idx = Math.Max(0, Math.Min(q1Idx, n - 1))
            q3Idx = Math.Max(0, Math.Min(q3Idx, n - 1))
            Dim iqr As Double = sorted(q3Idx) - sorted(q1Idx)

            ' Silverman法则
            Dim spread As Double = Math.Min(stdDev, iqr / 1.34)
            If spread < Double.Epsilon Then spread = stdDev
            If spread < Double.Epsilon Then spread = 1.0

            Dim h As Double = 0.9 * spread * Math.Pow(n, -0.2)
            Return Math.Max(h, Double.Epsilon)
        End Function

    End Module

End Namespace
