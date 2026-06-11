Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports std = System.Math

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
    ''' 输出：xcms2()，对齐后的峰面积表达矩阵
    ''' </summary>
    Public Module Algorithm

        ''' <summary>
        ''' 执行峰对齐，将各样本的离子峰在保留时间上对齐并生成峰面积表达矩阵
        ''' </summary>
        ''' <param name="peaks">各样本的离子峰字典，键名为原始数据文件名</param>
        ''' <param name="params">对齐参数配置</param>
        ''' <returns>对齐后的峰面积表达矩阵</returns>
        Public Function AlignPeaks(peaks As Dictionary(Of String, PeakFeature()), params As AlignmentParameters) As xcms2()
            If peaks Is Nothing OrElse peaks.Count = 0 Then
                Return New xcms2() {}
            End If

            ' 过滤空样本
            Dim validPeaks As New Dictionary(Of String, PeakFeature())
            For Each kv In peaks
                If kv.Value IsNot Nothing AndAlso kv.Value.Length > 0 Then
                    validPeaks(kv.Key) = kv.Value
                End If
            Next

            If validPeaks.Count = 0 Then
                Return New xcms2() {}
            End If

            ' 如果只有一个样本，直接转换
            If validPeaks.Count = 1 Then
                Return SingleSampleToTable(validPeaks.First)
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
                Call FillGaps(alignedGroups, validPeaks, params)
            End If

            ' 按最小样本比例过滤
            alignedGroups = FilterByMinFraction(alignedGroups, validPeaks.Keys.ToList, params.minFraction)

            ' 转换为IonExpression数组
            Return ConvertToTable(alignedGroups, validPeaks.Keys.ToList)
        End Function

        ''' <summary>
        ''' 按最小样本比例过滤对齐特征
        ''' 只有在至少minFraction比例的样本中出现的特征才会被保留
        ''' </summary>
        Private Function FilterByMinFraction(groups As List(Of AlignedPeakGroup),
                                              sampleNames As List(Of String),
                                              minFraction As Double) As List(Of AlignedPeakGroup)
            Dim minSamples As Integer = CInt(std.Ceiling(minFraction * sampleNames.Count))
            minSamples = std.Max(minSamples, 1)

            Return groups _
                .Where(Function(g)
                           Return g.sampleAreas.Where(Function(kv) kv.Value > 0).Count >= minSamples
                       End Function) _
                .ToList()
        End Function

        ''' <summary>
        ''' 便捷方法：使用默认参数执行密度分组对齐
        ''' </summary>
        Public Function AlignPeaks(peaks As Dictionary(Of String, PeakFeature())) As xcms2()
            Return AlignPeaks(peaks, New AlignmentParameters())
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
                    If g.sampleAreas.ContainsKey(sampleName) Then
                        Continue For
                    End If

                    ' 在该样本中搜索匹配的峰
                    Dim bestPeak As PeakFeature = Nothing
                    Dim bestScore As Double = Double.MaxValue

                    For Each p In peaks(sampleName)
                        Dim mzTol As Double = MassWindow.GetMzTolerance(g.avgMz, params.mzTolerance, params.mzToleranceMode)
                        Dim mzDiff As Double = std.Abs(p.mz - g.avgMz)
                        Dim rtDiff As Double = std.Abs(p.rt - g.avgRt)

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

        ''' <summary>
        ''' 将对齐峰组列表转换为IonExpression数组
        ''' </summary>
        Private Function ConvertToTable(groups As List(Of AlignedPeakGroup), sampleNames As List(Of String)) As xcms2()
            Dim result As xcms2() = New xcms2(groups.Count - 1) {}

            For i As Integer = 0 To groups.Count - 1
                Dim g As AlignedPeakGroup = groups(i)
                Dim expr As New xcms2()

                ' 生成特征ID：M{m/z}T{rt}
                expr.ID = String.Format("M{0:F4}T{1:F1}", g.avgMz, g.avgRt)
                expr.mz = g.avgMz
                expr.mzmin = g.minMz
                expr.mzmax = g.maxMz
                expr.rt = g.avgRt
                expr.rtmin = g.minRt
                expr.rtmax = g.maxRt

                ' 填充各样本的峰面积
                expr.Properties = New Dictionary(Of String, Double)()
                For Each sampleName In sampleNames
                    If g.sampleAreas.ContainsKey(sampleName) Then
                        expr.Properties(sampleName) = g.sampleAreas(sampleName)
                    Else
                        expr.Properties(sampleName) = 0.0
                    End If
                Next

                result(i) = expr
            Next

            ' 按m/z排序输出
            Array.Sort(result, Function(a, b) b.npeaks.CompareTo(a.npeaks))

            ' 重新编号ID
            For i As Integer = 0 To result.Length - 1
                result(i).ID = String.Format("F{0:D5}", i + 1)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 单样本直接转换为IonExpression数组
        ''' </summary>
        Private Function SingleSampleToTable(kv As KeyValuePair(Of String, PeakFeature())) As xcms2()
            Dim result As xcms2() = New xcms2(kv.Value.Length - 1) {}

            For i As Integer = 0 To kv.Value.Length - 1
                Dim p As PeakFeature = kv.Value(i)
                Dim expr As New xcms2()

                expr.ID = String.Format("F{0:D5}", i + 1)
                expr.mz = p.mz
                expr.mzmin = p.mz
                expr.mzmax = p.mz
                expr.rt = p.rt
                expr.rtmin = p.rtmin
                expr.rtmax = p.rtmax

                expr.Properties = New Dictionary(Of String, Double)()
                expr.Properties(kv.Key) = p.area

                result(i) = expr
            Next

            Array.Sort(result, Function(a, b) a.mz.CompareTo(b.mz))

            Return result
        End Function

        ' ========================================================================
        '   通用辅助函数
        ' ========================================================================

        ''' <summary>
        ''' 选择参考样本
        ''' 
        ''' 策略：选择峰数量最多的样本作为参考样本。
        ''' 如果指定了参考样本名称且存在，则使用指定的样本。
        ''' </summary>
        Public Function SelectReferenceSample(peaks As Dictionary(Of String, PeakFeature()),
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
        Public Function GetGlobalRTRange(peaks As Dictionary(Of String, PeakFeature())) As Tuple(Of Double, Double)
            Dim rtMin As Double = Double.MaxValue
            Dim rtMax As Double = Double.MinValue

            For Each kv In peaks
                For Each p In kv.Value
                    rtMin = std.Min(rtMin, p.rtmin)
                    rtMax = std.Max(rtMax, p.rtmax)
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
        Public Function ComputeMedian(values As List(Of Double)) As Double
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
        Public Function SilvermanBandwidth(values As Double()) As Double
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
            Dim stdDev As Double = std.Sqrt(variance)

            ' 计算四分位距（IQR）
            Dim q1Idx As Integer = CInt(std.Floor(n * 0.25))
            Dim q3Idx As Integer = CInt(std.Floor(n * 0.75))
            q1Idx = std.Max(0, std.Min(q1Idx, n - 1))
            q3Idx = std.Max(0, std.Min(q3Idx, n - 1))
            Dim iqr As Double = sorted(q3Idx) - sorted(q1Idx)

            ' Silverman法则
            Dim spread As Double = std.Min(stdDev, iqr / 1.34)
            If spread < Double.Epsilon Then spread = stdDev
            If spread < Double.Epsilon Then spread = 1.0

            Dim h As Double = 0.9 * spread * std.Pow(n, -0.2)
            Return std.Max(h, Double.Epsilon)
        End Function

    End Module

End Namespace
