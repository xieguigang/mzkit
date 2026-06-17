' ════════════════════════════════════════════════════════════════════
' PKAnalysis.vb — 药代动力学非房室分析 (NCA) 模块
' ════════════════════════════════════════════════════════════════════
' 输入: DrugQuantify() 数组 (时间点 + 多个生物学重复浓度值)
' 输出: PKParameters 全参数集合 + 时间序列汇总 CSV
'
' 依赖: .NET Framework 4.8 (仅使用基础数学函数，无第三方库)
' ════════════════════════════════════════════════════════════════════

Imports System.Globalization
Imports System.IO
Imports std = System.Math

Namespace PKAnalysis

    ' ════════════════════════════════════════════════════════════════════
    ' 数据结构定义
    ' ════════════════════════════════════════════════════════════════════

    ''' <summary>
    ''' 单个时间点的药物定量检测数据
    ''' </summary>
    Public Class DrugQuantify
        ''' <summary>采样时间点（小时, h）</summary>
        Public Property Time As Double

        ''' <summary>
        ''' 该时间点上多个生物学重复的药物/代谢物浓度值数组。
        ''' 数组长度 = 生物学重复数。
        ''' </summary>
        Public Property Quantify As Double()
    End Class

    ''' <summary>
    ''' NCA 分析计算得到的全部药代动力学参数
    ''' </summary>
    Public Class PKParameters

        ' ── 实测峰谷参数 ──
        ''' <summary>峰浓度（实测最大均值浓度）</summary>
        Public Property Cmax As Double
        ''' <summary>达峰时间 (h)</summary>
        Public Property Tmax As Double
        ''' <summary>谷浓度（给药后最低均值浓度）</summary>
        Public Property Cmin As Double
        ''' <summary>达谷时间 (h)</summary>
        Public Property Tmin As Double
        ''' <summary>末点可测定浓度（最后一个高于定量下限的浓度）</summary>
        Public Property Clast As Double
        ''' <summary>末点时间 (h)</summary>
        Public Property Tlast As Double
        ''' <summary>外推至 t=0 的浓度（IV 给药对数外推）</summary>
        Public Property C0 As Double

        ' ── AUC 参数（药时曲线下面积） ──
        ''' <summary>AUC(0-t): 线性梯形法计算 0→Tlast 面积</summary>
        Public Property AUC0_t As Double
        ''' <summary>AUC(0-∞): 0→无穷大面积 = AUC(0-t) + Clast/λz</summary>
        Public Property AUC0_inf As Double
        ''' <summary>外推面积部分 = Clast/λz</summary>
        Public Property AUC0_inf_Extrap As Double
        ''' <summary>外推面积占 AUC(0-∞) 的百分比</summary>
        Public Property AUC_Extrap_Pct As Double

        ' ── AUMC 参数（一阶矩曲线下面积） ──
        ''' <summary>AUMC(0-t): ∫t·C(t)dt, 0→Tlast</summary>
        Public Property AUMC0_t As Double
        ''' <summary>AUMC(0-∞): 0→无穷大</summary>
        Public Property AUMC0_inf As Double
        ''' <summary>AUMC 外推部分</summary>
        Public Property AUMC0_inf_Extrap As Double

        ' ── 末端消除相参数 ──
        ''' <summary>末端消除速率常数 λz (h⁻¹)</summary>
        Public Property LambdaZ As Double
        ''' <summary>末端消除半衰期 t½ = ln2/λz (h)</summary>
        Public Property HalfLife As Double
        ''' <summary>末端线性回归 R²</summary>
        Public Property RSquared As Double
        ''' <summary>调整 R²</summary>
        Public Property AdjRSquared As Double
        ''' <summary>末端相参与拟合的数据点数</summary>
        Public Property NumTerminalPoints As Integer
        ''' <summary>末端相起始时间 (h)</summary>
        Public Property TerminalPhaseStart As Double

        ' ── 清除率与分布容积 ──
        ''' <summary>清除率 CL = Dose/AUC(0-∞)</summary>
        Public Property CL As Double
        ''' <summary>末端相表观分布容积 Vd = CL/λz</summary>
        Public Property Vd As Double
        ''' <summary>稳态表观分布容积 Vss = MRT × CL</summary>
        Public Property Vss As Double

        ' ── 平均驻留时间 ──
        ''' <summary>平均驻留时间 MRT = AUMC(0-∞)/AUC(0-∞) (h)</summary>
        Public Property MRT As Double

        ' ── 吸收速率（仅血管外给药，残差法） ──
        ''' <summary>吸收速率常数 ka (h⁻¹)，残差法估算</summary>
        Public Property Ka As Double
        ''' <summary>吸收半衰期 t½(ka) = ln2/ka (h)</summary>
        Public Property AbsorptionHalfLife As Double

        ' ── 给药信息 ──
        Public Property Dose As Double
        Public Property Route As String

        ' ── 数据描述 ──
        Public Property NumTimePoints As Integer
        Public Property NumReplicates As Integer
    End Class

    ''' <summary>
    ''' 单个时间点的统计汇总（用于 CSV 导出和绘图）
    ''' </summary>
    Public Class TimePointSummary
        Public Property Time As Double
        Public Property Mean As Double
        Public Property SD As Double
        Public Property SEM As Double
        Public Property Min As Double
        Public Property Max As Double
        Public Property Median As Double
        Public Property N As Integer
        Public Property Replicates As Double()
    End Class

    ' ════════════════════════════════════════════════════════════════════
    ' NCA 分析器模块
    ' ════════════════════════════════════════════════════════════════════

    Public Module NCAAnalyzer

        ''' <summary>
        ''' 执行完整的 NCA 分析
        ''' </summary>
        ''' <param name="data">DrugQuantify 数组（无需预排序）</param>
        ''' <param name="dose">给药剂量（单位须与浓度×体积匹配，如 µg/kg）</param>
        ''' <param name="route">给药途径: "IV"（静脉）/ "SC"（皮下）/ "PO"（口服）/ "IM"（肌肉）</param>
        ''' <returns>PKParameters 完整参数集</returns>
        Public Function Analyze(data As DrugQuantify(), dose As Double, route As String) As PKParameters
            ' ── 输入校验 ──
            If data Is Nothing OrElse data.Length < 3 Then
                Throw New ArgumentException("NCA 分析至少需要 3 个时间点数据")
            End If

            ' ── 排序并提取均值序列 ──
            Dim sorted = data.OrderBy(Function(d) d.Time).ToArray()
            Dim nTimes As Integer = sorted.Length
            Dim times(nTimes - 1) As Double
            Dim meanConcs(nTimes - 1) As Double
            Dim maxReps As Integer = 0

            For i = 0 To nTimes - 1
                times(i) = sorted(i).Time
                Dim q = sorted(i).Quantify
                If q IsNot Nothing AndAlso q.Length > 0 Then
                    meanConcs(i) = q.Average()
                    If q.Length > maxReps Then maxReps = q.Length
                Else
                    meanConcs(i) = 0.0
                End If
            Next

            Dim pk As New PKParameters()
            pk.Dose = dose
            pk.Route = route
            pk.NumTimePoints = nTimes
            pk.NumReplicates = maxReps

            ' ════════════════════════════════════════════════
            ' 1. Cmax / Tmax
            ' ════════════════════════════════════════════════
            Dim maxIdx As Integer = 0
            For i = 1 To nTimes - 1
                If meanConcs(i) > meanConcs(maxIdx) Then maxIdx = i
            Next
            pk.Cmax = meanConcs(maxIdx)
            pk.Tmax = times(maxIdx)

            ' ════════════════════════════════════════════════
            ' 2. Cmin / Tmin（给药后最低浓度）
            ' ════════════════════════════════════════════════
            Dim minIdx As Integer = 0
            For i = 1 To nTimes - 1
                If meanConcs(i) < meanConcs(minIdx) Then minIdx = i
            Next
            pk.Cmin = meanConcs(minIdx)
            pk.Tmin = times(minIdx)

            ' ════════════════════════════════════════════════
            ' 3. Clast / Tlast（最后一个可定量浓度点）
            ' ════════════════════════════════════════════════
            Dim lastIdx As Integer = nTimes - 1
            While lastIdx > 0 AndAlso meanConcs(lastIdx) <= 0
                lastIdx -= 1
            End While
            pk.Clast = meanConcs(lastIdx)
            pk.Tlast = times(lastIdx)

            ' ════════════════════════════════════════════════
            ' 4. AUC(0-t) — 线性梯形法
            ' ════════════════════════════════════════════════
            pk.AUC0_t = TrapezoidalAUC(times, meanConcs, lastIdx)

            ' ════════════════════════════════════════════════
            ' 5. AUMC(0-t) — ∫t·C(t)dt 线性梯形法
            ' ════════════════════════════════════════════════
            pk.AUMC0_t = TrapezoidalAUMC(times, meanConcs, lastIdx)

            ' ════════════════════════════════════════════════
            ' 6. 末端相 λz 拟合
            ' ════════════════════════════════════════════════
            Dim termFit = FitTerminalPhase(times, meanConcs, lastIdx)
            pk.LambdaZ = termFit.LambdaZ
            pk.RSquared = termFit.RSquared
            pk.AdjRSquared = termFit.AdjRSquared
            pk.NumTerminalPoints = termFit.NumPoints
            pk.TerminalPhaseStart = termFit.StartTime

            If pk.LambdaZ > 0 Then
                ' t½ = ln2 / λz
                pk.HalfLife = std.Log(2) / pk.LambdaZ

                ' AUC(0-∞) = AUC(0-t) + Clast/λz
                pk.AUC0_inf_Extrap = pk.Clast / pk.LambdaZ
                pk.AUC0_inf = pk.AUC0_t + pk.AUC0_inf_Extrap
                pk.AUC_Extrap_Pct = If(pk.AUC0_inf > 0,
                    (pk.AUC0_inf_Extrap / pk.AUC0_inf) * 100.0, 0.0)

                ' AUMC(0-∞) = AUMC(0-t) + Tlast·Clast/λz + Clast/λz²
                pk.AUMC0_inf_Extrap = pk.Tlast * pk.Clast / pk.LambdaZ +
                                      pk.Clast / (pk.LambdaZ * pk.LambdaZ)
                pk.AUMC0_inf = pk.AUMC0_t + pk.AUMC0_inf_Extrap

                ' MRT = AUMC(0-∞) / AUC(0-∞)
                If pk.AUC0_inf > 0 Then
                    pk.MRT = pk.AUMC0_inf / pk.AUC0_inf
                End If

                ' CL = Dose / AUC(0-∞)
                If pk.AUC0_inf > 0 Then
                    pk.CL = dose / pk.AUC0_inf
                End If

                ' Vd (末端相) = CL / λz
                pk.Vd = pk.CL / pk.LambdaZ

                ' Vss = MRT × CL
                pk.Vss = pk.MRT * pk.CL
            End If

            ' ════════════════════════════════════════════════
            ' 7. C0 外推（IV 给药对数外推至 t=0）
            ' ════════════════════════════════════════════════
            If route.ToUpper() = "IV" AndAlso nTimes >= 2 AndAlso
               meanConcs(0) > 0 AndAlso meanConcs(1) > 0 AndAlso times(0) >= 0 Then
                Dim logC0 As Double = std.Log(meanConcs(0))
                Dim logC1 As Double = std.Log(meanConcs(1))
                Dim dt01 As Double = times(1) - times(0)
                If std.Abs(dt01) > 0.000000000001 Then
                    Dim slope As Double = (logC1 - logC0) / dt01
                    pk.C0 = std.Exp(logC0 - slope * times(0))
                Else
                    pk.C0 = meanConcs(0)
                End If
            Else
                pk.C0 = meanConcs(0)
            End If

            ' ════════════════════════════════════════════════
            ' 8. 吸收速率常数 ka（残差法，仅血管外给药）
            ' ════════════════════════════════════════════════
            If route.ToUpper() <> "IV" AndAlso pk.LambdaZ > 0 AndAlso nTimes >= 4 Then
                Dim kaVal = EstimateKa(times, meanConcs, pk.LambdaZ, pk.Tmax)
                If kaVal > 0 Then
                    pk.Ka = kaVal
                    pk.AbsorptionHalfLife = std.Log(2) / kaVal
                End If
            End If

            Return pk
        End Function

        ''' <summary>
        ''' 线性梯形法 AUC: Σ ½(Cᵢ + Cᵢ₊₁) × Δt
        ''' </summary>
        Private Function TrapezoidalAUC(times As Double(), concs As Double(), lastIdx As Integer) As Double
            Dim auc As Double = 0.0
            For i = 0 To lastIdx - 1
                Dim dt As Double = times(i + 1) - times(i)
                auc += 0.5 * (concs(i) + concs(i + 1)) * dt
            Next
            Return auc
        End Function

        ''' <summary>
        ''' 线性梯形法 AUMC: Σ ½(tᵢ·Cᵢ + tᵢ₊₁·Cᵢ₊₁) × Δt
        ''' </summary>
        Private Function TrapezoidalAUMC(times As Double(), concs As Double(), lastIdx As Integer) As Double
            Dim aumc As Double = 0.0
            For i = 0 To lastIdx - 1
                Dim dt As Double = times(i + 1) - times(i)
                aumc += 0.5 * (times(i) * concs(i) + times(i + 1) * concs(i + 1)) * dt
            Next
            Return aumc
        End Function

        ''' <summary>
        ''' 末端相 λz 拟合：
        ''' 从最后一个数据点向前逐步增加点数（≥3），
        ''' 对 ln(C) vs t 做线性回归，选择调整 R² 最高的窗口。
        ''' 要求斜率为负（浓度递减）。
        ''' </summary>
        Private Function FitTerminalPhase(times As Double(), concs As Double(), lastIdx As Integer) As (LambdaZ As Double, RSquared As Double, AdjRSquared As Double, NumPoints As Integer, StartTime As Double)

            ' 收集末端浓度 > 0 的点（从后往前）
            Dim termTimes As New List(Of Double)()
            Dim termConcs As New List(Of Double)()
            For i = lastIdx To 0 Step -1
                If concs(i) > 0 Then
                    termTimes.Add(times(i))
                    termConcs.Add(concs(i))
                Else
                    Exit For
                End If
            Next

            If termTimes.Count < 3 Then
                Return (0.0, 0.0, 0.0, 0, 0.0)
            End If

            Dim bestLambda As Double = 0
            Dim bestR2 As Double = -1
            Dim bestAdjR2 As Double = -1
            Dim bestN As Integer = 0
            Dim bestStart As Double = 0

            ' 从 3 个点开始逐步增加末端窗口
            For nPts = 3 To termTimes.Count
                Dim xs(nPts - 1) As Double
                Dim ys(nPts - 1) As Double
                For j = 0 To nPts - 1
                    xs(j) = termTimes(j)
                    ys(j) = std.Log(termConcs(j))
                Next

                ' 线性回归: y = a + b·x,  b = -λz
                Dim n As Integer = nPts
                Dim sumX = 0.0, sumY = 0.0, sumXY = 0.0, sumX2 = 0.0
                For j = 0 To n - 1
                    sumX += xs(j)
                    sumY += ys(j)
                    sumXY += xs(j) * ys(j)
                    sumX2 += xs(j) * xs(j)
                Next
                Dim denom = n * sumX2 - sumX * sumX
                If std.Abs(denom) < 1.0E-30 Then Continue For

                Dim b = (n * sumXY - sumX * sumY) / denom
                Dim a = (sumY - b * sumX) / n

                ' R² 计算
                Dim meanY = sumY / n
                Dim ssRes = 0.0, ssTot = 0.0
                For j = 0 To n - 1
                    Dim pred = a + b * xs(j)
                    ssRes += (ys(j) - pred) * (ys(j) - pred)
                    ssTot += (ys(j) - meanY) * (ys(j) - meanY)
                Next
                Dim r2 = If(ssTot > 0, 1.0 - ssRes / ssTot, 0.0)
                Dim adjR2 = If(n > 2, 1.0 - (1.0 - r2) * (n - 1) / (n - 2), r2)

                ' 末端斜率必须为负
                If b >= 0 Then Continue For

                If adjR2 > bestAdjR2 Then
                    bestAdjR2 = adjR2
                    bestR2 = r2
                    bestLambda = -b
                    bestN = nPts
                    bestStart = xs(nPts - 1)  ' 最早的点（termTimes 是逆序，最后加入的是最早的时间）
                End If
            Next

            Return (bestLambda, bestR2, bestAdjR2, bestN, bestStart)
        End Function

        ''' <summary>
        ''' 残差法估算吸收速率常数 ka
        ''' 方法：用末端相拟合线外推回吸收相，残差 = 实测 - 外推，
        ''' 对残差曲线做对数线性回归，斜率 = -ka
        ''' </summary>
        Private Function EstimateKa(times As Double(), concs As Double(),
                                    lambdaZ As Double, tmax As Double) As Double
            ' 末端相拟合: ln(C) = a - λz·t → C_ext = exp(a - λz·t)
            ' 取末端 3 个点拟合截距 a
            Dim nEnd As Integer = std.Min(3, times.Length)
            Dim aIntercept As Double = 0
            Dim count As Integer = 0
            Dim sumLogC = 0.0, sumT = 0.0
            For i = times.Length - nEnd To times.Length - 1
                If concs(i) > 0 Then
                    sumLogC += std.Log(concs(i))
                    sumT += times(i)
                    count += 1
                End If
            Next
            If count = 0 Then Return 0
            Dim meanLogC = sumLogC / count
            Dim meanT = sumT / count
            aIntercept = meanLogC + lambdaZ * meanT  ' ln(C) = aIntercept - λz·t

            ' 计算吸收相残差（Tmax 之前且浓度>0的点）
            Dim resTimes As New List(Of Double)()
            Dim resVals As New List(Of Double)()
            For i = 0 To times.Length - 1
                If times(i) > 0 AndAlso times(i) <= tmax AndAlso concs(i) > 0 Then
                    Dim cExt = std.Exp(aIntercept - lambdaZ * times(i))
                    Dim residual = concs(i) - cExt
                    If residual > 0 Then
                        resTimes.Add(times(i))
                        resVals.Add(std.Log(residual))
                    End If
                End If
            Next

            If resTimes.Count < 2 Then Return 0

            ' 对数线性回归残差: ln(residual) = a_r - ka·t
            Dim n = resTimes.Count
            Dim sx = 0.0, sy = 0.0, sxy = 0.0, sx2 = 0.0
            For i = 0 To n - 1
                sx += resTimes(i)
                sy += resVals(i)
                sxy += resTimes(i) * resVals(i)
                sx2 += resTimes(i) * resTimes(i)
            Next
            Dim denom = n * sx2 - sx * sx
            If std.Abs(denom) < 1.0E-30 Then Return 0
            Dim slope = (n * sxy - sx * sy) / denom

            ' 斜率应为负
            If slope >= 0 Then Return 0
            Return -slope
        End Function

        ''' <summary>
        ''' 计算每个时间点的统计汇总
        ''' </summary>
        Public Function SummarizeTimePoints(data As DrugQuantify()) As List(Of TimePointSummary)
            Dim result As New List(Of TimePointSummary)()
            Dim sorted = data.OrderBy(Function(d) d.Time).ToArray()
            For Each d In sorted
                Dim s As New TimePointSummary()
                s.Time = d.Time
                s.Replicates = CType(d.Quantify.Clone(), Double())
                s.N = d.Quantify.Length
                s.Mean = d.Quantify.Average()
                s.Min = d.Quantify.Min()
                s.Max = d.Quantify.Max()
                s.Median = Median(d.Quantify)

                If s.N > 1 Then
                    Dim sumSq = d.Quantify.Sum(Function(v) (v - s.Mean) * (v - s.Mean))
                    s.SD = std.Sqrt(sumSq / (s.N - 1))
                    s.SEM = s.SD / std.Sqrt(s.N)
                Else
                    s.SD = 0.0
                    s.SEM = 0.0
                End If
                result.Add(s)
            Next
            Return result
        End Function

        ''' <summary>计算中位数</summary>
        Private Function Median(arr As Double()) As Double
            Dim sorted = arr.OrderBy(Function(v) v).ToArray()
            Dim n = sorted.Length
            If n = 0 Then Return 0
            If n Mod 2 = 0 Then
                Return (sorted(n \ 2 - 1) + sorted(n \ 2)) / 2.0
            Else
                Return sorted(n \ 2)
            End If
        End Function

        ' ════════════════════════════════════════════════════════════════════
        ' CSV 导出
        ' ════════════════════════════════════════════════════════════════════

        ''' <summary>
        ''' 导出时间序列汇总 CSV（供 R 绘图用）
        ''' 列: Time_h, Mean, SD, SEM, Min, Max, Median, N, Rep1..RepN
        ''' </summary>
        Public Sub ExportSummaryCSV(data As DrugQuantify(), filePath As String)
            Dim summaries = SummarizeTimePoints(data)
            Dim maxReps As Integer = data.Max(Function(d) If(d.Quantify?.Length, 0))

            Using writer As New StreamWriter(filePath, False, System.Text.Encoding.UTF8)
                ' ── 表头 ──
                Dim header = "Time_h,Mean,SD,SEM,Min,Max,Median,N"
                For j = 1 To maxReps
                    header &= $",Rep{j}"
                Next
                writer.WriteLine(header)

                ' ── 数据行 ──
                For Each s In summaries
                    Dim line = FormatNumber(s.Time) & "," &
                               FormatNumber(s.Mean) & "," &
                               FormatNumber(s.SD) & "," &
                               FormatNumber(s.SEM) & "," &
                               FormatNumber(s.Min) & "," &
                               FormatNumber(s.Max) & "," &
                               FormatNumber(s.Median) & "," &
                               s.N.ToString()
                    For j = 0 To maxReps - 1
                        If j < s.Replicates.Length Then
                            line &= "," & FormatNumber(s.Replicates(j))
                        Else
                            line &= ","
                        End If
                    Next
                    writer.WriteLine(line)
                Next
            End Using
        End Sub

        ''' <summary>
        ''' 导出 NCA 参数 CSV（供 R 表格图用）
        ''' </summary>
        Public Sub ExportParametersCSV(pk As PKParameters, filePath As String)
            Using writer As New StreamWriter(filePath, False, System.Text.Encoding.UTF8)
                writer.WriteLine("Parameter,Value,Unit,Description")
                WriteParam(writer, "Dose", pk.Dose, "given", "Administered dose")
                WriteParam(writer, "Route", 0, "", pk.Route, isText:=True)
                WriteParam(writer, "Cmax", pk.Cmax, "conc", "Maximum observed concentration (mean)")
                WriteParam(writer, "Tmax", pk.Tmax, "h", "Time of Cmax")
                WriteParam(writer, "Cmin", pk.Cmin, "conc", "Minimum observed concentration")
                WriteParam(writer, "Tmin", pk.Tmin, "h", "Time of Cmin")
                WriteParam(writer, "Clast", pk.Clast, "conc", "Last measurable concentration")
                WriteParam(writer, "Tlast", pk.Tlast, "h", "Time of last measurable concentration")
                WriteParam(writer, "C0", pk.C0, "conc", "Back-extrapolated concentration at t=0")
                WriteParam(writer, "LambdaZ", pk.LambdaZ, "1/h", "Terminal elimination rate constant")
                WriteParam(writer, "HalfLife", pk.HalfLife, "h", "Terminal elimination half-life")
                WriteParam(writer, "RSquared", pk.RSquared, "", "R-squared of terminal phase fit")
                WriteParam(writer, "AdjRSquared", pk.AdjRSquared, "", "Adjusted R-squared")
                WriteParam(writer, "NumTerminalPoints", pk.NumTerminalPoints, "", "Points used in terminal fit")
                WriteParam(writer, "TerminalPhaseStart", pk.TerminalPhaseStart, "h", "Start time of terminal phase")
                WriteParam(writer, "AUC0_t", pk.AUC0_t, "conc*h", "AUC from 0 to last time (trapezoidal)")
                WriteParam(writer, "AUC0_inf", pk.AUC0_inf, "conc*h", "AUC from 0 to infinity")
                WriteParam(writer, "AUC0_inf_Extrap", pk.AUC0_inf_Extrap, "conc*h", "Extrapolated AUC portion")
                WriteParam(writer, "AUC_Extrap_Pct", pk.AUC_Extrap_Pct, "%", "Extrapolated AUC percentage")
                WriteParam(writer, "AUMC0_t", pk.AUMC0_t, "conc*h^2", "AUMC from 0 to last time")
                WriteParam(writer, "AUMC0_inf", pk.AUMC0_inf, "conc*h^2", "AUMC from 0 to infinity")
                WriteParam(writer, "MRT", pk.MRT, "h", "Mean residence time")
                WriteParam(writer, "CL", pk.CL, "vol/h", "Clearance")
                WriteParam(writer, "Vd", pk.Vd, "vol", "Apparent volume of distribution (terminal)")
                WriteParam(writer, "Vss", pk.Vss, "vol", "Steady-state volume of distribution")
                WriteParam(writer, "Ka", pk.Ka, "1/h", "Absorption rate constant (residual method)")
                WriteParam(writer, "AbsorptionHalfLife", pk.AbsorptionHalfLife, "h", "Absorption half-life")
                WriteParam(writer, "NumTimePoints", pk.NumTimePoints, "", "Total time points")
                WriteParam(writer, "NumReplicates", pk.NumReplicates, "", "Biological replicates per time point")
            End Using
        End Sub

        Private Sub WriteParam(writer As StreamWriter, name As String, value As Double,
                               unit As String, desc As String, Optional isText As Boolean = False)
            If isText Then
                writer.WriteLine($"{name},{desc},{unit},{desc}")
            Else
                writer.WriteLine($"{name},{FormatNumber(value)},{unit},{desc}")
            End If
        End Sub

        ''' <summary>格式化数值为 CSV 友好字符串</summary>
        Private Function FormatNumber(v As Double) As String
            If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then Return ""
            Return v.ToString("G10", CultureInfo.InvariantCulture)
        End Function

    End Module

End Namespace
