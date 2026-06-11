Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports std = System.Math

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
        Public Function LOESSAlignment(peaks As Dictionary(Of String, PeakFeature()), params As AlignmentParameters) As List(Of AlignedPeakGroup)
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
                        Dim mzTol As Double = MassWindow.GetMzTolerance(refP.mz, params.mzTolerance, params.mzToleranceMode)
                        If std.Abs(refP.mz - sampleP.mz) <= mzTol AndAlso
                            std.Abs(refP.rt - sampleP.rt) <= initialRtTol Then
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
                            Dim mzTol As Double = MassWindow.GetMzTolerance(refP.mz, params.mzTolerance, params.mzToleranceMode)
                            If std.Abs(refP.mz - sampleP.mz) <= mzTol AndAlso
                               std.Abs(refP.rt - sampleP.rt) <= expandedRtTol Then
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
                    Dim sortedIndices As Integer() = Enumerable.Range(0, sampleRts.Length).OrderBy(Function(idx) sampleRts(idx)).ToArray()

                    Dim i As Integer = 0
                    While i < sortedIndices.Length
                        Dim currentSampleRt As Double = sampleRts(sortedIndices(i))
                        Dim refSum As Double = refRts(sortedIndices(i))
                        Dim count As Integer = 1
                        i += 1
                        While i < sortedIndices.Length AndAlso std.Abs(sampleRts(sortedIndices(i)) - currentSampleRt) < 0.001
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

    End Module
End Namespace