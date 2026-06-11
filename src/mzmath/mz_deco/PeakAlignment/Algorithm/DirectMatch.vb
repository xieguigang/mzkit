Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports std = System.Math

Namespace PeakAlignment

    ' ========================================================================
    '   算法1：直接匹配对齐（DirectMatch）
    ' ========================================================================
    Public Module DirectMatch

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
        Public Function DirectMatchAlignment(peaks As Dictionary(Of String, PeakFeature()), params As AlignmentParameters) As List(Of AlignedPeakGroup)
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
                Dim tol As Double = MassWindow.GetMzTolerance(currentGroup(0).Item2.mz, params.mzTolerance, params.mzToleranceMode)

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
            Dim bar As ProgressBar = Nothing

            For Each mzGroup In TqdmWrapper.Wrap(mzGroups, bar:=bar)
                If mzGroup.Any Then
                    Call bar.SetLabel($"Process m/z: {mzGroup.Average(Function(a) a.Item2.mz)}")
                End If

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