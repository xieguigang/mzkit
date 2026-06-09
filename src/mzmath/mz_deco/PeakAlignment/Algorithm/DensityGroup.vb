Imports std = System.Math

Namespace PeakAlignment

    ' ========================================================================
    '   算法4：密度分组对齐（XCMS风格）
    ' ========================================================================
    Public Module DensityGroup

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
        Public Function DensityGroupAlignment(peaks As Dictionary(Of String, PeakFeature()),
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
                    bw = std.Max(bw, params.rtTolerance * 0.5)
                End If

                ' 在RT范围内构建密度估计网格
                Dim rtMinGroup As Double = rtValues.Min()
                Dim rtMaxGroup As Double = rtValues.Max()
                Dim rtRangeGroup As Double = rtMaxGroup - rtMinGroup
                ' 扩展范围以避免边界效应
                rtMinGroup -= bw * 3
                rtMaxGroup += bw * 3

                Dim nGrid As Integer = CInt(std.Max(512, (rtMaxGroup - rtMinGroup) / (bw * 0.1)))
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
                        density += std.Exp(-0.5 * u * u) / (bw * std.Sqrt(2.0 * std.PI))
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
                        Dim dist As Double = std.Abs(peakRt - gridRts(densityPeaks(di)))
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
                        g.minMz = std.Min(g.minMz, kv.Value.mz)
                        g.maxMz = std.Max(g.maxMz, kv.Value.mz)
                        g.minRt = std.Min(g.minRt, kv.Value.rtmin)
                        g.maxRt = std.Max(g.maxRt, kv.Value.rtmax)
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

    End Module
End Namespace