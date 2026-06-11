Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm

Namespace PeakAlignment

    ' ========================================================================
    '   算法3：Obiwarp动态时间规整对齐
    ' ========================================================================
    Public Module Obiwarp

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
        Public Function ObiwarpAlignment(peaks As Dictionary(Of String, PeakFeature()), params As AlignmentParameters) As List(Of AlignedPeakGroup)
            ' 第一步：选择参考样本
            Dim refName As String = SelectReferenceSample(peaks, params.referenceSample)

            Call $"sample file {refName} will be used as the reference sample.".debug

            ' 第二步：为每个样本构建TIC色谱轮廓
            Dim allSampleNames As List(Of String) = peaks.Keys.ToList()
            Dim rtRange As Tuple(Of Double, Double) = GetGlobalRTRange(peaks)
            Dim rtMin As Double = rtRange.Item1
            Dim rtMax As Double = rtRange.Item2

            ' 构建分段TIC轮廓
            Dim profiles As New Dictionary(Of String, Double())
            Dim rtBins As Double() = Nothing

            For Each kv In peaks
                Dim profile As Tuple(Of Double(), Double()) = BuildTICProfile(kv.Value, rtMin, rtMax, params.obiwarpBinSize)
                profiles(kv.Key) = profile.Item1
                rtBins = profile.Item2
            Next

            ' 第三步：对每个非参考样本进行DTW对齐
            Dim correctedPeaks As New Dictionary(Of String, PeakFeature())
            correctedPeaks(refName) = peaks(refName)

            Dim refProfile As Double() = profiles(refName)
            Dim bar As ProgressBar = Nothing

            For Each kv In TqdmWrapper.Wrap(peaks, bar:=bar)
                If kv.Key = refName Then
                    Continue For
                Else
                    Call bar.SetLabel(kv.Key)
                End If

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

    End Module
End Namespace