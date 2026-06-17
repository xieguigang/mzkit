' ============================================================================
' Mummichog Pre-annotation Module for LC-MS Untargeted Metabolomics
' ----------------------------------------------------------------------------
' 基于KEGG通路先验知识的Mummichog算法VB.NET实现
' 核心思想: "先通路分析，后代谢物注释" (Guilt-by-association 连坐原则)
'
' 算法六步流程:
'   1. 输入数据准备 (xcms2峰表 + p值 + 电离模式)
'   2. 构建理论代谢物质量库 (KEGG代谢物 + 加合物)
'   3. 初始质荷比匹配 (ppm容忍度)
'   4. 构建独立背景模型 (加合物频率)
'   5. 通路富集与活性评估 (超几何检验 + BH-FDR)
'   6. 反向精确注释 (加合物一致性 + 同位素验证 + 优先级打分)
'
' 依赖: 仅使用VB.NET基础数学函数 (System.Math)，无第三方库
' ============================================================================

Imports System.Math
Imports System.Data
Imports System.Linq
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

''' <summary>
''' KEGG代谢物数据模型
''' </summary>
Public Class KEGGMetabolite

    ''' <summary>KEGG Compound ID, 例如 C00022</summary>
    Public Property ID As String

    ''' <summary>代谢物名称</summary>
    Public Property Name As String

    ''' <summary>分子式, 例如 C6H12O6</summary>
    Public Property Formula As String

    ''' <summary>精确分子量 (单同位素质量, Da)</summary>
    Public Property ExactMass As Double

    ''' <summary>该代谢物参与的所有KEGG通路ID集合</summary>
    Public Property Pathways As New HashSet(Of String)

    ''' <summary>
    ''' 从分子式重新计算精确分子量 (当ExactMass不可靠时使用)
    ''' </summary>
    Public Function RecalculateMass() As Double
        If String.IsNullOrEmpty(Formula) Then Return ExactMass
        Dim mass = FormulaUtils.CalculateMonoisotopicMass(Formula)
        If mass > 0 Then
            ExactMass = mass
        End If
        Return ExactMass
    End Function

    Public Overrides Function ToString() As String
        Return $"{ID} ({Name})"
    End Function
End Class

''' <summary>
''' KEGG代谢通路数据模型
''' </summary>
Public Class KEGGPathway

    ''' <summary>KEGG Pathway ID, 例如 map00010 (糖酵解/糖异生)</summary>
    Public Property ID As String

    ''' <summary>通路名称</summary>
    Public Property Name As String

    ''' <summary>该通路包含的所有代谢物ID集合</summary>
    Public Property Metabolites As New HashSet(Of String)

    Public Overrides Function ToString() As String
        Return $"{ID} ({Name})"
    End Function
End Class

''' <summary>
''' 加合物规则定义
''' <para>计算公式: mz = (M * Multiplier + MassAddition) / |Charge|</para>
''' </summary>
Public Class AdductRule

    ''' <summary>加合物名称, 例如 [M+H]+, [M-H]-</summary>
    Public Property Name As String

    ''' <summary>电荷数 (正为正离子, 负为负离子)</summary>
    Public Property Charge As Integer

    ''' <summary>质量增加值 (Da), 包含所有质子/钠/钾等的加减</summary>
    Public Property MassAddition As Double

    ''' <summary>分子倍数, 1=单体, 2=二聚体</summary>
    Public Property Multiplier As Integer = 1

    ''' <summary>电离模式</summary>
    Public Property Mode As IonModes

    ''' <summary>是否为常见加合物 (用于背景模型加权)</summary>
    Public Property IsCommon As Boolean = True

    ''' <summary>
    ''' 计算给定代谢物在此加合物下的理论m/z
    ''' </summary>
    ''' <param name="metaboliteMass">代谢物精确分子量</param>
    Public Function CalculateMz(metaboliteMass As Double) As Double
        Return (metaboliteMass * Multiplier + MassAddition) / Math.Abs(Charge)
    End Function

    Public Overrides Function ToString() As String
        Return Name
    End Function
End Class

''' <summary>
''' 理论m/z条目 (代谢物 + 加合物的组合)
''' </summary>
Public Class TheoreticalMz

    ''' <summary>对应的KEGG代谢物</summary>
    Public Property Metabolite As KEGGMetabolite

    ''' <summary>对应的加合物规则</summary>
    Public Property Adduct As AdductRule

    ''' <summary>理论m/z值</summary>
    Public ReadOnly Property Mz As Double
        Get
            Return Adduct.CalculateMz(Metabolite.ExactMass)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Metabolite.ID} {Adduct.Name} m/z={Mz:F6}"
    End Function
End Class

''' <summary>
''' 实验峰与理论m/z的匹配结果
''' </summary>
Public Class MzMatch

    ''' <summary>实验检测到的离子峰</summary>
    Public Property Peak As xcms2

    ''' <summary>匹配上的理论m/z条目</summary>
    Public Property Theoretical As TheoreticalMz

    ''' <summary>质量误差 (ppm)</summary>
    Public Property PpmError As Double

    ''' <summary>该峰的统计p值</summary>
    Public Property PValue As Double = 1.0

    ''' <summary>是否为显著差异峰</summary>
    Public ReadOnly Property IsSignificant As Boolean
        Get
            Return PValue < 0.05
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Peak.ID} -> {Theoretical.Metabolite.ID} {Theoretical.Adduct.Name} ({PpmError:F2} ppm)"
    End Function
End Class

''' <summary>
''' 通路富集分析结果
''' </summary>
Public Class PathwayEnrichmentResult

    ''' <summary>对应的KEGG通路</summary>
    Public Property Pathway As KEGGPathway

    ''' <summary>该通路中的总代谢物数</summary>
    Public Property PathwaySize As Integer

    ''' <summary>背景中匹配到该通路的特征峰数</summary>
    Public Property BackgroundHits As Integer

    ''' <summary>显著特征峰中匹配到该通路的数量</summary>
    Public Property SignificantHits As Integer

    ''' <summary>显著特征峰总数</summary>
    Public Property TotalSignificant As Integer

    ''' <summary>背景特征峰总数</summary>
    Public Property TotalBackground As Integer

    ''' <summary>超几何检验p值</summary>
    Public Property PValue As Double

    ''' <summary>BH校正后的FDR</summary>
    Public Property FDR As Double

    ''' <summary>富集得分 = -log10(p值)</summary>
    Public ReadOnly Property Score As Double
        Get
            If PValue <= 0 Then Return 300.0
            Return -Log10(PValue)
        End Get
    End Property

    ''' <summary>是否为显著富集通路</summary>
    Public Property IsSignificant As Boolean

    ''' <summary>匹配到的显著特征峰详情</summary>
    Public Property HitMatches As New List(Of MzMatch)

    Public Overrides Function ToString() As String
        Return $"{Pathway.ID} hits={SignificantHits}/{PathwaySize} p={PValue:G4} fdr={FDR:G4}"
    End Function
End Class

''' <summary>
''' 最终代谢物预注释结果
''' </summary>
Public Class AnnotationResult

    ''' <summary>排序名次 (按PriorityScore降序)</summary>
    Public Property Rank As Integer

    ''' <summary>实验离子峰</summary>
    Public Property Peak As xcms2

    ''' <summary>注释到的KEGG代谢物</summary>
    Public Property Metabolite As KEGGMetabolite

    ''' <summary>推断的加合物类型</summary>
    Public Property Adduct As AdductRule

    ''' <summary>质量误差 (ppm)</summary>
    Public Property PpmError As Double

    ''' <summary>该峰的统计p值</summary>
    Public Property PValue As Double

    ''' <summary>该代谢物所在的显著通路列表</summary>
    Public Property SignificantPathways As New List(Of KEGGPathway)

    ''' <summary>通路富集得分 (该代谢物所在所有显著通路的 -log10(p) 之和)</summary>
    Public Property PathwayScore As Double

    ''' <summary>加合物一致性得分 [0-1]</summary>
    Public Property AdductConsistencyScore As Double

    ''' <summary>同位素验证得分 [0-1]</summary>
    Public Property IsotopeScore As Double

    ''' <summary>质量精度得分 [0-1]</summary>
    Public Property MassAccuracyScore As Double

    ''' <summary>该代谢物检测到的加合物列表</summary>
    Public Property DetectedAdducts As New List(Of String)

    ''' <summary>同位素验证详情</summary>
    Public Property IsotopeDetails As String

    ''' <summary>最终优先级得分 (综合打分)</summary>
    Public Property PriorityScore As Double

    ''' <summary>置信度等级 (基于PriorityScore划分)</summary>
    Public ReadOnly Property ConfidenceLevel As String
        Get
            If PriorityScore >= 5.0 Then Return "High"
            If PriorityScore >= 3.0 Then Return "Medium"
            If PriorityScore >= 1.5 Then Return "Low"
            Return "Putative"
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"#{Rank} {Peak.mz:F4} -> {Metabolite.ID} {Metabolite.Name} [{Adduct.Name}] score={PriorityScore:F3} ({ConfidenceLevel})"
    End Function
End Class

''' <summary>
''' Mummichog算法参数配置
''' </summary>
Public Class MummichogParams

    ''' <summary>质量匹配容忍度 (ppm), 默认5.0</summary>
    Public Property PpmTolerance As Double = 5.0

    ''' <summary>差异峰p值阈值, 默认0.05</summary>
    Public Property PValueCutoff As Double = 0.05

    ''' <summary>通路富集FDR阈值, 默认0.2</summary>
    Public Property FdrCutoff As Double = 0.2

    ''' <summary>电离模式, 默认正离子</summary>
    Public Property Mode As IonModes = IonModes.Positive

    ''' <summary>同位素搜索m/z窗口 (ppm), 默认5.0</summary>
    Public Property IsotopePpmTolerance As Double = 10.0

    ''' <summary>同位素强度匹配容忍倍数, 默认3倍</summary>
    Public Property IsotopeIntensityTolerance As Double = 3.0

    ' --- 优先级打分权重 ---
    Public Property WeightPathway As Double = 3.0
    Public Property WeightAdduct As Double = 2.0
    Public Property WeightIsotope As Double = 1.5
    Public Property WeightMass As Double = 1.0

    ''' <summary>最大保留的注释结果数 (0=不限制)</summary>
    Public Property MaxResults As Integer = 0

    ''' <summary>每个峰最多保留的候选注释数</summary>
    Public Property MaxCandidatesPerPeak As Integer = 5
End Class

' ========================================================================
' 数学工具函数模块
' 实现Mummichog所需的统计检验, 全部基于VB.NET基础数学函数
' ========================================================================

''' <summary>
''' 数学与统计工具函数集合
''' </summary>
Public Module MathUtils

    ' --- 常量 ---
    Public ReadOnly LOG_SQRT_2PI As Double = 0.5 * Log(2 * PI)
    Public ReadOnly EPSILON As Double = 1.0E-300
    Public ReadOnly MAX_ITERATIONS As Integer = 500

    ''' <summary>
    ''' Lanczos近似计算 ln(Γ(x))
    ''' <para>用于支持大数阶乘的对数计算, 避免溢出</para>
    ''' </summary>
    ''' <param name="x">输入值, 必须为正数</param>
    Public Function LogGamma(x As Double) As Double
        ' Lanczos系数 (g=7, n=9)
        Dim c() As Double = {
            0.99999999999980993,
            676.5203681218851,
            -1259.1392167224028,
            771.32342877765313,
            -176.61502916214059,
            12.507343278686905,
            -0.13857109526572012,
            0.0000099843695780195716,
            0.00000015056327351493116
        }
        Dim g As Double = 7.0

        ' 反射公式: Γ(x)Γ(1-x) = π / sin(πx)
        If x < 0.5 Then
            Return Log(PI / Sin(PI * x)) - LogGamma(1.0 - x)
        End If

        ' 递推: Γ(x+1) = x·Γ(x), 将x平移到[0,1]区间
        x -= 1.0
        Dim a As Double = c(0)
        Dim t As Double = x + g + 0.5

        For i As Integer = 1 To 8
            a += c(i) / (x + i)
        Next

        Return LOG_SQRT_2PI + (x + 0.5) * Log(t) - t + Log(a)
    End Function

    ''' <summary>
    ''' 计算对数二项系数 ln(C(n, k))
    ''' <para>避免直接计算大数阶乘导致溢出</para>
    ''' </summary>
    Public Function LogBinomial(n As Integer, k As Integer) As Double
        If k < 0 OrElse k > n OrElse n < 0 Then Return Double.NegativeInfinity
        If k = 0 OrElse k = n Then Return 0.0
        ' ln(C(n,k)) = ln(n!) - ln(k!) - ln((n-k)!)
        Return LogGamma(n + 1.0) - LogGamma(k + 1.0) - LogGamma(n - k + 1.0)
    End Function

    ''' <summary>
    ''' 计算超几何分布的右尾p值 P(X >= k)
    ''' <para>
    ''' 超几何分布描述: 从N个总体(含K个成功)中抽取n个样本, 恰好得到i个成功的概率
    ''' P(X=i) = C(K,i) * C(N-K, n-i) / C(N, n)
    ''' 右尾p值 = Σ_{i=k}^{min(K,n)} P(X=i)
    ''' </para>
    ''' </summary>
    ''' <param name="N">总体大小 (背景特征峰总数)</param>
    ''' <param name="K">总体中成功数 (背景中匹配到该通路的峰数)</param>
    ''' <param name="n">样本大小 (显著特征峰总数)</param>
    ''' <param name="k">观察到的成功数 (显著峰中匹配到该通路的峰数)</param>
    Public Function HypergeometricPValue(N As Integer, K As Integer, n As Integer, k As Integer) As Double
        ' 边界条件处理
        If N <= 0 OrElse K < 0 OrElse N < 0 OrElse K < 0 Then Return 1.0
        If K > N OrElse N > N Then Return 1.0
        If K > Math.Min(K, N) Then Return 0.0
        If K <= 0 Then Return 1.0

        ' 计算对数空间下的概率, 避免数值下溢
        Dim logDenom As Double = LogBinomial(N, N)
        Dim upper As Integer = Math.Min(K, N)
        Dim pSum As Double = 0.0

        ' 使用对数空间累加
        For i As Integer = K To upper
            Dim logP As Double = LogBinomial(K, i) + LogBinomial(N - K, N - i) - logDenom
            pSum += Exp(logP)
        Next

        ' 防止浮点误差导致p值超过1
        Return Math.Min(1.0, pSum)
    End Function

    ''' <summary>
    ''' 正则化不完全Beta函数 I_x(a, b)
    ''' <para>用于计算Student's t分布和F分布的p值</para>
    ''' <para>采用Lentz连分数法, 参考Numerical Recipes</para>
    ''' </summary>
    ''' <param name="a">Beta分布参数a</param>
    ''' <param name="b">Beta分布参数b</param>
    ''' <param name="x">积分上限 [0,1]</param>
    Public Function IncompleteBeta(a As Double, b As Double, x As Double) As Double
        If x <= 0.0 Then Return 0.0
        If x >= 1.0 Then Return 1.0

        ' 前因子: x^a * (1-x)^b / (a * B(a,b))
        ' ln(B(a,b)) = ln(Γ(a)) + ln(Γ(b)) - ln(Γ(a+b))
        Dim lbt As Double = LogGamma(a + b) - LogGamma(a) - LogGamma(b) +
                            a * Log(x) + b * Log(1.0 - x)
        Dim bt As Double = Exp(lbt)

        ' 根据x的位置选择连分数方向, 保证收敛速度
        If x < (a + 1.0) / (a + b + 2.0) Then
            ' 直接计算: I_x = bt * cf(a,b,x) / a
            Return bt * BetaContinuedFraction(a, b, x) / a
        Else
            ' 利用对称性: I_x(a,b) = 1 - I_{1-x}(b,a)
            Return 1.0 - bt * BetaContinuedFraction(b, a, 1.0 - x) / b
        End If
    End Function

    ''' <summary>
    ''' 不完全Beta函数的连分数核心 (Lentz法)
    ''' </summary>
    Private Function BetaContinuedFraction(a As Double, b As Double, x As Double) As Double
        Const EPS As Double = 0.000000000000001
        Const FPMIN As Double = 1.0E-300
        Dim qab As Double = a + b
        Dim qap As Double = a + 1.0
        Dim qam As Double = a - 1.0

        Dim c As Double = 1.0
        Dim d As Double = 1.0 - qab * x / qap
        If Math.Abs(d) < FPMIN Then d = FPMIN
        d = 1.0 / d
        Dim h As Double = d

        For m As Integer = 1 To MAX_ITERATIONS
            Dim m2 As Integer = 2 * m
            ' 偶数步系数
            Dim aa As Double = m * (b - m) * x / ((qam + m2) * (a + m2))
            d = 1.0 + aa * d
            If Math.Abs(d) < FPMIN Then d = FPMIN
            c = 1.0 + aa / c
            If Math.Abs(c) < FPMIN Then c = FPMIN
            d = 1.0 / d
            h *= d * c

            ' 奇数步系数
            aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
            d = 1.0 + aa * d
            If Math.Abs(d) < FPMIN Then d = FPMIN
            c = 1.0 + aa / c
            If Math.Abs(c) < FPMIN Then c = FPMIN
            d = 1.0 / d
            Dim del As Double = d * c
            h *= del

            ' 收敛判断
            If Math.Abs(del - 1.0) < EPS Then Exit For
        Next

        Return h
    End Function

    ''' <summary>
    ''' Student's t分布的累积分布函数 (双尾)
    ''' <para>用于Welch's t检验的p值计算</para>
    ''' <para>p = 2 * (1 - CDF(|t|, df))</para>
    ''' </summary>
    ''' <param name="t">t统计量</param>
    ''' <param name="df">自由度</param>
    Public Function StudentTPValue(t As Double, df As Double) As Double
        If df <= 0 Then Return 1.0
        Dim x As Double = df / (df + t * t)
        ' CDF(|t|) = 1 - 0.5 * I_x(df/2, 0.5)
        ' 双尾p = 2 * (1 - CDF(|t|)) = I_x(df/2, 0.5)
        Dim p As Double = IncompleteBeta(df / 2.0, 0.5, x)
        Return Math.Min(1.0, Math.Max(0.0, p))
    End Function

    ''' <summary>
    ''' Welch's t检验 (不假设等方差的二样本t检验)
    ''' <para>计算两组样本均值差异的显著性p值</para>
    ''' </summary>
    ''' <param name="group1">第一组样本值</param>
    ''' <param name="group2">第二组样本值</param>
    Public Function WelchTTest(group1 As IEnumerable(Of Double), group2 As IEnumerable(Of Double)) As Double
        Dim s1 As Double() = group1.Where(Function(v) Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v)).ToArray()
        Dim s2 As Double() = group2.Where(Function(v) Not Double.IsNaN(v) AndAlso Not Double.IsInfinity(v)).ToArray()

        Dim n1 As Integer = s1.Length
        Dim n2 As Integer = s2.Length

        ' 样本量不足, 返回p=1 (不显著)
        If n1 < 2 OrElse n2 < 2 Then Return 1.0

        ' 计算均值
        Dim mean1 As Double = s1.Average()
        Dim mean2 As Double = s2.Average()

        ' 计算样本方差 (无偏估计)
        Dim var1 As Double = s1.Sum(Function(v) (v - mean1) ^ 2) / (n1 - 1)
        Dim var2 As Double = s2.Sum(Function(v) (v - mean2) ^ 2) / (n2 - 1)

        ' 两组方差均为0, 无法计算t
        If var1 = 0 AndAlso var2 = 0 Then
            If mean1 = mean2 Then Return 1.0
            Return 0.0
        End If

        ' Welch's t统计量
        Dim se As Double = Math.Sqrt(var1 / n1 + var2 / n2)
        If se = 0 Then Return 1.0
        Dim t As Double = (mean1 - mean2) / se

        ' Welch-Satterthwaite自由度
        Dim num As Double = (var1 / n1 + var2 / n2) ^ 2
        Dim den As Double = (var1 / n1) ^ 2 / (n1 - 1) + (var2 / n2) ^ 2 / (n2 - 1)
        Dim df As Double = num / den

        Return StudentTPValue(t, df)
    End Function

    ''' <summary>
    ''' Benjamini-Hochberg FDR校正
    ''' <para>控制错误发现率 (False Discovery Rate)</para>
    ''' </summary>
    ''' <param name="pValues">原始p值数组</param>
    ''' <returns>校正后的q值数组 (与输入顺序一致)</returns>
    Public Function BHCorrection(pValues As IEnumerable(Of Double)) As Double()
        Dim pList As Double() = pValues.ToArray()
        Dim n As Integer = pList.Length
        If n = 0 Then Return New Double(-1) {}

        ' 创建 (p值, 原始索引) 对, 按p值升序排列
        Dim indexed = pList.Select(Function(p, i) (p, i)).OrderBy(Function(x) x.Item1).ToArray()

        Dim q(n - 1) As Double
        Dim prevQ As Double = 1.0

        ' 从大到小遍历, 保证单调性: q(i) = min(q(i+1), p(i) * n / rank(i))
        For idx As Integer = n - 1 To 0 Step -1
            Dim rank As Integer = idx + 1 ' 1-indexed rank
            Dim raw As Double = indexed(idx).Item1 * CDbl(n) / CDbl(rank)
            ' 取与后一个q值的最小值, 保证单调不减
            prevQ = Math.Min(prevQ, raw)
            ' 上限为1
            prevQ = Math.Min(1.0, prevQ)
            q(idx) = prevQ
        Next

        ' 将q值放回原始顺序
        Dim result(n - 1) As Double
        For idx As Integer = 0 To n - 1
            result(indexed(idx).Item2) = q(idx)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 计算质量误差 (ppm)
    ''' <para>ppm = (观测值 - 理论值) / 理论值 * 1e6</para>
    ''' </summary>
    ''' <param name="observedMz">实验观测m/z</param>
    ''' <param name="theoreticalMz">理论m/z</param>
    Public Function CalculatePpm(observedMz As Double, theoreticalMz As Double) As Double
        If theoreticalMz = 0 Then Return Double.MaxValue
        Return (observedMz - theoreticalMz) / theoreticalMz * 1000000.0
    End Function

    ''' <summary>
    ''' 计算ppm容忍度对应的绝对质量窗口 (Da)
    ''' </summary>
    Public Function PpmToDa(mz As Double, ppm As Double) As Double
        Return mz * ppm / 1000000.0
    End Function

    ''' <summary>
    ''' 判断两个m/z是否在ppm容忍度内匹配
    ''' </summary>
    Public Function IsWithinPpm(mz1 As Double, mz2 As Double, ppmTolerance As Double) As Boolean
        Return Math.Abs(CalculatePpm(mz1, mz2)) <= ppmTolerance
    End Function

    ''' <summary>
    ''' 安全的 -log10(p) 计算, 处理p=0的边界情况
    ''' </summary>
    Public Function SafeNegLog10(p As Double) As Double
        If p <= 0 Then Return 300.0
        If p >= 1 Then Return 0.0
        Return -Log10(p)
    End Function

    ''' <summary>
    ''' 计算中位数
    ''' </summary>
    Public Function Median(values As IEnumerable(Of Double)) As Double
        Dim sorted As Double() = values.OrderBy(Function(v) v).ToArray()
        Dim n As Integer = sorted.Length
        If n = 0 Then Return 0.0
        If n Mod 2 = 1 Then Return sorted(n \ 2)
        Return (sorted(n \ 2 - 1) + sorted(n \ 2)) / 2.0
    End Function

    ''' <summary>
    ''' 计算标准差 (样本标准差, ddof=1)
    ''' </summary>
    Public Function StdDev(values As IEnumerable(Of Double)) As Double
        Dim arr As Double() = values.ToArray()
        Dim n As Integer = arr.Length
        If n < 2 Then Return 0.0
        Dim mean As Double = arr.Average()
        Dim sumSq As Double = arr.Sum(Function(v) (v - mean) ^ 2)
        Return Math.Sqrt(sumSq / (n - 1))
    End Function
End Module

' ========================================================================
' 分子式解析与同位素预测工具模块
' ========================================================================

''' <summary>
''' 分子式解析与单同位素质量/同位素模式计算工具
''' </summary>
Public Module FormulaUtils

    ' --- 单同位素原子质量表 (Da) ---
    ' 使用IUPAC推荐的单同位素质量
    Private ReadOnly AtomicMasses As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From
    {
        {"H", 1.00782503207},
        {"C", 12.0},
        {"N", 14.0030740048},
        {"O", 15.99491461957},
        {"F", 18.99840316273},
        {"Na", 22.9897692809},
        {"Mg", 23.9850417},
        {"P", 30.97376163},
        {"S", 31.972071},
        {"Cl", 34.96885268},
        {"K", 38.96370668},
        {"Ca", 39.96259098},
        {"Fe", 55.93493633},
        {"Cu", 62.9295975},
        {"Zn", 63.9291422},
        {"Br", 78.9183371},
        {"I", 126.904473},
        {"Se", 79.9165213},
        {"Mn", 54.9380451},
        {"Co", 58.933195},
        {"Mo", 97.905404},
        {"B", 11.0093054},
        {"Si", 27.9769265325},
        {"As", 74.9215964},
        {"Rb", 84.911789738},
        {"Sr", 87.9056121},
        {"Al", 25.98689186},
        {"Li", 7.016003},
        {"Ag", 106.9050916},
        {"Cd", 113.9033585},
        {"Ba", 137.9052472},
        {"Cr", 51.94050623},
        {"V", 50.9439595},
        {"Ni", 57.9353429},
        {"Sn", 119.901984},
        {"Sb", 120.9038157},
        {"Cs", 132.905429},
        {"Hg", 201.970625},
        {"Pb", 207.9766278}
    }

    ' --- 同位素质量差 (相对于最丰富同位素) ---
    Public ReadOnly C13_DELTA As Double = 1.0033548378
    Public ReadOnly N15_DELTA As Double = 0.9970349
    Public ReadOnly O18_DELTA As Double = 1.995794
    Public ReadOnly S34_DELTA As Double = 1.995796
    Public ReadOnly CL37_DELTA As Double = 1.9970499
    Public ReadOnly BR81_DELTA As Double = 1.9979528
    Public ReadOnly H2_DELTA As Double = 1.0062767

    ' --- 同位素自然丰度 (分数) ---
    Public ReadOnly C13_ABUNDANCE As Double = 0.0107
    Public ReadOnly N15_ABUNDANCE As Double = 0.00368
    Public ReadOnly O18_ABUNDANCE As Double = 0.00205
    Public ReadOnly S34_ABUNDANCE As Double = 0.0421
    Public ReadOnly CL37_ABUNDANCE As Double = 0.2422
    Public ReadOnly BR81_ABUNDANCE As Double = 0.4929
    Public ReadOnly H2_ABUNDANCE As Double = 0.000115

    ''' <summary>
    ''' 解析分子式, 返回元素->原子数的字典
    ''' <para>支持格式: C6H12O6, C10H15N1O2S1, 含括号的如C6H5(OH)2</para>
    ''' </summary>
    ''' <param name="formula">分子式字符串</param>
    Public Function ParseFormula(formula As String) As Dictionary(Of String, Integer)
        Dim result As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        If String.IsNullOrWhiteSpace(formula) Then Return result

        ' 预处理: 去除空格和常见前缀
        Dim cleanFormula As String = formula.Trim()
        ' 去除可能的开头 "C" 前缀 (如某些KEGG格式)
        ' 不去除, 因为C是碳元素

        ' 处理括号: 展开括号内容
        cleanFormula = ExpandParentheses(cleanFormula)

        ' 使用正则匹配元素+数字
        Dim pattern As String = "([A-Z][a-z]?)(\d*)"
        Dim matches As System.Text.RegularExpressions.MatchCollection =
            System.Text.RegularExpressions.Regex.Matches(cleanFormula, pattern)

        For Each m As System.Text.RegularExpressions.Match In matches
            Dim element As String = m.Groups(1).Value
            Dim countStr As String = m.Groups(2).Value
            Dim count As Integer = If(String.IsNullOrEmpty(countStr), 1, Integer.Parse(countStr))

            If result.ContainsKey(element) Then
                result(element) += count
            Else
                result(element) = count
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 展开分子式中的括号, 例如 C6H5(OH)2 -> C6H5O2H2
    ''' </summary>
    Private Function ExpandParentheses(formula As String) As String
        Dim result As New System.Text.StringBuilder(formula.Length)
        Dim stack As New Stack(Of Integer)()
        Dim multipliers As New Stack(Of Integer)()
        Dim i As Integer = 0

        While i < formula.Length
            Dim ch As Char = formula(i)
            If ch = "("c Then
                stack.Push(result.Length)
                multipliers.Push(1)
                i += 1
            ElseIf ch = ")"c Then
                i += 1
                ' 读取括号后的倍数
                Dim numStr As String = ""
                While i < formula.Length AndAlso Char.IsDigit(formula(i))
                    numStr &= formula(i)
                    i += 1
                End While
                Dim mult As Integer = If(String.IsNullOrEmpty(numStr), 1, Integer.Parse(numStr))
                ' 展开括号内容
                Dim startPos As Integer = stack.Pop()
                Dim content As String = result.ToString().Substring(startPos)
                result.Remove(startPos, result.Length - startPos)
                ' 将内容重复mult次
                Dim parsed = ParseSimpleFormula(content)
                For Each kvp In parsed
                    result.Append(kvp.Key)
                    If kvp.Value * mult > 1 Then result.Append(kvp.Value * mult)
                Next
            Else
                result.Append(ch)
                i += 1
            End If
        End While

        Return result.ToString()
    End Function

    ''' <summary>
    ''' 简单分子式解析 (不含括号)
    ''' </summary>
    Private Function ParseSimpleFormula(formula As String) As Dictionary(Of String, Integer)
        Dim result As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim pattern As String = "([A-Z][a-z]?)(\d*)"
        Dim matches As System.Text.RegularExpressions.MatchCollection =
            System.Text.RegularExpressions.Regex.Matches(formula, pattern)
        For Each m As System.Text.RegularExpressions.Match In matches
            Dim element As String = m.Groups(1).Value
            Dim countStr As String = m.Groups(2).Value
            Dim count As Integer = If(String.IsNullOrEmpty(countStr), 1, Integer.Parse(countStr))
            If result.ContainsKey(element) Then
                result(element) += count
            Else
                result(element) = count
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' 根据分子式计算单同位素精确分子量
    ''' </summary>
    Public Function CalculateMonoisotopicMass(formula As String) As Double
        Dim atoms = ParseFormula(formula)
        If atoms.Count = 0 Then Return 0.0

        Dim mass As Double = 0.0
        For Each kvp In atoms
            If AtomicMasses.ContainsKey(kvp.Key) Then
                mass += AtomicMasses(kvp.Key) * kvp.Value
            Else
                ' 未知元素, 返回0表示无法计算
                Return 0.0
            End If
        Next
        Return mass
    End Function

    ''' <summary>
    ''' 预测同位素模式 (M+1, M+2相对于M的强度比)
    ''' <para>
    ''' 简化模型:
    '''   M+1强度 ≈ 1.1%×nC + 0.37%×nN + 0.015%×nH + 0.04%×nO + 0.75%×nS
    '''   M+2强度 ≈ (1.1%×nC)²/2 + 4.2%×nS + 0.2%×nO + 32.5%×nCl + 97.3%×nBr
    ''' </para>
    ''' </summary>
    ''' <param name="formula">分子式</param>
    ''' <returns>包含(M+1/M, M+2/M)强度比的数组</returns>
    Public Function PredictIsotopeRatios(formula As String) As Double()
        Dim atoms = ParseFormula(formula)
        Dim nC As Integer = If(atoms.ContainsKey("C"), atoms("C"), 0)
        Dim nH As Integer = If(atoms.ContainsKey("H"), atoms("H"), 0)
        Dim nN As Integer = If(atoms.ContainsKey("N"), atoms("N"), 0)
        Dim nO As Integer = If(atoms.ContainsKey("O"), atoms("O"), 0)
        Dim nS As Integer = If(atoms.ContainsKey("S"), atoms("S"), 0)
        Dim nCl As Integer = If(atoms.ContainsKey("Cl"), atoms("Cl"), 0)
        Dim nBr As Integer = If(atoms.ContainsKey("Br"), atoms("Br"), 0)

        ' M+1 相对强度 (相对于M峰=1.0)
        Dim m1 As Double = nC * C13_ABUNDANCE +
                          nN * N15_ABUNDANCE +
                          nH * H2_ABUNDANCE +
                          nO * 0.00038 +
                          nS * 0.0076

        ' M+2 相对强度
        ' 包含: 2个C13的组合 + S34 + Cl37 + Br81 + O18
        Dim m2 As Double = (nC * C13_ABUNDANCE) ^ 2 / 2.0 +
                          nS * S34_ABUNDANCE +
                          nCl * CL37_ABUNDANCE +
                          nBr * BR81_ABUNDANCE +
                          nO * O18_ABUNDANCE

        Return New Double() {m1, m2}
    End Function

    ''' <summary>
    ''' 获取同位素峰的理论m/z偏移
    ''' <para>M+1主要由13C贡献, M+2由2个13C或34S等贡献</para>
    ''' </summary>
    Public Function GetIsotopeMassDelta(isotopeOrder As Integer) As Double
        Select Case isotopeOrder
            Case 1 : Return C13_DELTA   ' M+1
            Case 2 : Return 2 * C13_DELTA ' M+2 (近似)
            Case Else : Return isotopeOrder * C13_DELTA
        End Select
    End Function
End Module

' ========================================================================
' 加合物规则定义模块
' ========================================================================

''' <summary>
''' 预定义加合物规则 (参考Mummichog/MetaboAnalyst标准)
''' </summary>
Public Module AdductDefinitions

    ' --- 质子/离子精确质量 (Da) ---
    Public ReadOnly PROTON_MASS As Double = 1.00727646677
    Public ReadOnly SODIUM_MASS As Double = 22.989218
    Public ReadOnly POTASSIUM_MASS As Double = 38.963158
    Public ReadOnly AMMONIUM_MASS As Double = 18.033823
    Public ReadOnly CHLORIDE_MASS As Double = 34.969402
    Public ReadOnly WATER_MASS As Double = 18.0105646863
    Public ReadOnly CO2_MASS As Double = 43.989829
    Public ReadOnly NH3_MASS As Double = 17.0265491
    Public ReadOnly HCOOH_MASS As Double = 46.0054793
    Public ReadOnly CH3COOH_MASS As Double = 60.0211293
    Public ReadOnly ELECTRON_MASS As Double = 0.00054858

    ''' <summary>
    ''' 获取正离子模式下的标准加合物列表
    ''' <para>
    ''' 包含: [M+H]+, [M+Na]+, [M+NH4]+, [M+K]+,
    '''       [M+H-H2O]+, [M+H-NH3]+, [M+H-CO2]+,
    '''       [2M+H]+, [2M+Na]+, [M+2H]2+
    ''' </para>
    ''' </summary>
    Public Function GetPositiveAdducts() As List(Of AdductRule)
        Dim adducts As New List(Of AdductRule)

        ' 主要加合物 (常见, 高权重)
        adducts.Add(New AdductRule With {
            .Name = "[M+H]+", .Charge = 1, .MassAddition = PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = True})

        adducts.Add(New AdductRule With {
            .Name = "[M+Na]+", .Charge = 1, .MassAddition = SODIUM_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = True})

        adducts.Add(New AdductRule With {
            .Name = "[M+NH4]+", .Charge = 1, .MassAddition = AMMONIUM_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = True})

        adducts.Add(New AdductRule With {
            .Name = "[M+K]+", .Charge = 1, .MassAddition = POTASSIUM_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = True})

        ' 中性丢失加合物
        adducts.Add(New AdductRule With {
            .Name = "[M+H-H2O]+", .Charge = 1, .MassAddition = PROTON_MASS - WATER_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M+H-NH3]+", .Charge = 1, .MassAddition = PROTON_MASS - NH3_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M+H-CO2]+", .Charge = 1, .MassAddition = PROTON_MASS - CO2_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M+H-2H2O]+", .Charge = 1, .MassAddition = PROTON_MASS - 2 * WATER_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = False})

        ' 二聚体加合物
        adducts.Add(New AdductRule With {
            .Name = "[2M+H]+", .Charge = 1, .MassAddition = PROTON_MASS,
            .Multiplier = 2, .Mode = IonModes.Positive, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[2M+Na]+", .Charge = 1, .MassAddition = SODIUM_MASS,
            .Multiplier = 2, .Mode = IonModes.Positive, .IsCommon = False})

        ' 多电荷加合物
        adducts.Add(New AdductRule With {
            .Name = "[M+2H]2+", .Charge = 2, .MassAddition = 2 * PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M+H+Na]2+", .Charge = 2, .MassAddition = PROTON_MASS + SODIUM_MASS,
            .Multiplier = 1, .Mode = IonModes.Positive, .IsCommon = False})

        Return adducts
    End Function

    ''' <summary>
    ''' 获取负离子模式下的标准加合物列表
    ''' <para>
    ''' 包含: [M-H]-, [M+Cl]-, [M-H-H2O]-, [M-H-CO2]-,
    '''       [M+Na-2H]-, [M+K-2H]-, [2M-H]-, [M-HCOO]-, [M-CH3COO]-
    ''' </para>
    ''' </summary>
    Public Function GetNegativeAdducts() As List(Of AdductRule)
        Dim adducts As New List(Of AdductRule)

        ' 主要加合物
        adducts.Add(New AdductRule With {
            .Name = "[M-H]-", .Charge = -1, .MassAddition = -PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = True})

        adducts.Add(New AdductRule With {
            .Name = "[M+Cl]-", .Charge = -1, .MassAddition = CHLORIDE_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = True})

        ' 中性丢失
        adducts.Add(New AdductRule With {
            .Name = "[M-H-H2O]-", .Charge = -1, .MassAddition = -PROTON_MASS - WATER_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M-H-CO2]-", .Charge = -1, .MassAddition = -PROTON_MASS - CO2_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M-H-NH3]-", .Charge = -1, .MassAddition = -PROTON_MASS - NH3_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        ' 取代加合物
        adducts.Add(New AdductRule With {
            .Name = "[M+Na-2H]-", .Charge = -1, .MassAddition = SODIUM_MASS - 2 * PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M+K-2H]-", .Charge = -1, .MassAddition = POTASSIUM_MASS - 2 * PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        ' 二聚体
        adducts.Add(New AdductRule With {
            .Name = "[2M-H]-", .Charge = -1, .MassAddition = -PROTON_MASS,
            .Multiplier = 2, .Mode = IonModes.Negative, .IsCommon = False})

        ' 加合酸根
        adducts.Add(New AdductRule With {
            .Name = "[M-HCOO]-", .Charge = -1, .MassAddition = HCOOH_MASS - PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        adducts.Add(New AdductRule With {
            .Name = "[M-CH3COO]-", .Charge = -1, .MassAddition = CH3COOH_MASS - PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        ' 多电荷
        adducts.Add(New AdductRule With {
            .Name = "[M-2H]2-", .Charge = -2, .MassAddition = -2 * PROTON_MASS,
            .Multiplier = 1, .Mode = IonModes.Negative, .IsCommon = False})

        Return adducts
    End Function

    ''' <summary>
    ''' 根据电离模式获取对应的加合物列表
    ''' </summary>
    Public Function GetAdducts(mode As IonModes) As List(Of AdductRule)
        If mode = IonModes.Positive Then
            Return GetPositiveAdducts()
        Else
            Return GetNegativeAdducts()
        End If
    End Function
End Module

' ========================================================================
' Mummichog主注释器类
' 实现完整的六步算法流程
' ========================================================================

''' <summary>
''' Mummichog预注释器
''' <para>
''' 基于KEGG通路先验知识, 在无MS/MS数据的情况下,
''' 仅利用一级质谱m/z和统计p值进行代谢物预注释
''' </para>
''' <para>
''' 核心思想: "先通路分析, 后代谢物注释" (Guilt-by-association)
''' </para>
''' </summary>
Public Class MummichogAnnotator

    ' --- KEGG数据库 ---
    Private _metabolites As New Dictionary(Of String, KEGGMetabolite)
    Private _pathways As New Dictionary(Of String, KEGGPathway)
    Private _adducts As New List(Of AdductRule)

    ' --- 算法参数 ---
    Private _params As MummichogParams

    ' --- 中间结果 (供调试和报告使用) ---
    Private _theoreticalLibrary As List(Of TheoreticalMz)
    Private _allMatches As List(Of MzMatch)
    Private _adductFrequencies As Dictionary(Of String, Double)
    Private _pathwayResults As List(Of PathwayEnrichmentResult)

    ''' <summary>
    ''' 构造函数: 初始化Mummichog注释器
    ''' </summary>
    ''' <param name="metabolites">KEGG代谢物列表</param>
    ''' <param name="pathways">KEGG通路列表</param>
    ''' <param name="params">算法参数 (可选, 使用默认值)</param>
    Public Sub New(metabolites As IEnumerable(Of KEGGMetabolite),
                   pathways As IEnumerable(Of KEGGPathway),
                   Optional params As MummichogParams = Nothing)

        _params = If(params, New MummichogParams())

        ' 构建代谢物字典
        _metabolites = New Dictionary(Of String, KEGGMetabolite)
        For Each m In metabolites
            If m.ExactMass <= 0 AndAlso Not String.IsNullOrEmpty(m.Formula) Then
                m.RecalculateMass()
            End If
            If m.ExactMass > 0 Then
                _metabolites(m.ID) = m
            End If
        Next

        ' 构建通路字典
        _pathways = New Dictionary(Of String, KEGGPathway)
        For Each p As KEGGPathway In pathways
            _pathways(p.ID) = p
        Next

        ' 获取加合物规则
        _adducts = AdductDefinitions.GetAdducts(_params.Mode)
    End Sub

    ''' <summary>
    ''' 获取/设置算法参数
    ''' </summary>
    Public Property Parameters As MummichogParams
        Get
            Return _params
        End Get
        Set(value As MummichogParams)
            _params = value
            _adducts = AdductDefinitions.GetAdducts(_params.Mode)
        End Set
    End Property

    ''' <summary>
    ''' 获取理论m/z库 (步骤2完成后可用)
    ''' </summary>
    Public ReadOnly Property TheoreticalLibrary As List(Of TheoreticalMz)
        Get
            Return _theoreticalLibrary
        End Get
    End Property

    ''' <summary>
    ''' 获取所有匹配结果 (步骤3完成后可用)
    ''' </summary>
    Public ReadOnly Property AllMatches As List(Of MzMatch)
        Get
            Return _allMatches
        End Get
    End Property

    ''' <summary>
    ''' 获取加合物频率背景模型 (步骤4完成后可用)
    ''' </summary>
    Public ReadOnly Property AdductFrequencies As Dictionary(Of String, Double)
        Get
            Return _adductFrequencies
        End Get
    End Property

    ''' <summary>
    ''' 获取通路富集分析结果 (步骤5完成后可用)
    ''' </summary>
    Public ReadOnly Property PathwayResults As List(Of PathwayEnrichmentResult)
        Get
            Return _pathwayResults
        End Get
    End Property

    ' ================================================================
    ' 主注释方法
    ' ================================================================

    ''' <summary>
    ''' 执行Mummichog预注释 (主入口)
    ''' <para>
    ''' 完整执行六步算法流程:
    ''' 1. 输入数据准备
    ''' 2. 构建理论代谢物质量库
    ''' 3. 初始质荷比匹配
    ''' 4. 构建独立背景模型
    ''' 5. 通路富集与活性评估
    ''' 6. 反向精确注释 (加合物一致性 + 同位素验证 + 优先级打分)
    ''' </para>
    ''' </summary>
    ''' <param name="peaks">一级质谱离子峰数组 (xcms2)</param>
    ''' <param name="pValues">每个峰ID对应的差异表达p值</param>
    ''' <returns>按优先级得分降序排列的注释结果列表</returns>
    Public Function Annotate(peaks As IEnumerable(Of xcms2),
                              pValues As Dictionary(Of String, Double)) As List(Of AnnotationResult)

        Dim peakList As List(Of xcms2) = peaks.ToList()
        If peakList.Count = 0 Then Return New List(Of AnnotationResult)

        ' ========== 步骤1: 输入数据准备 ==========
        ' 为每个峰附加p值 (默认1.0表示不显著)
        Dim peakPValues As New Dictionary(Of String, Double)
        For Each p As xcms2 In peakList
            Dim pval As Double = 1.0
            If pValues IsNot Nothing AndAlso pValues.ContainsKey(p.ID) Then
                pval = pValues(p.ID)
            End If
            peakPValues(p.ID) = pval
        Next

        ' ========== 步骤2: 构建理论代谢物质量库 ==========
        _theoreticalLibrary = BuildTheoreticalLibrary()

        ' ========== 步骤3: 初始质荷比匹配 ==========
        _allMatches = MatchPeaksToLibrary(peakList, peakPValues)

        ' ========== 步骤4: 构建独立背景模型 ==========
        _adductFrequencies = BuildBackgroundModel(_allMatches)

        ' ========== 步骤5: 通路富集与活性评估 ==========
        _pathwayResults = PerformPathwayEnrichment(peakList, peakPValues)

        ' ========== 步骤6: 反向精确注释 ==========
        Dim results = ReverseAnnotation(peakList, peakPValues)

        ' 按优先级得分降序排序
        results = results.OrderByDescending(Function(r) r.PriorityScore).ToList()

        ' 分配排名
        For i As Integer = 0 To results.Count - 1
            results(i).Rank = i + 1
        Next

        ' 限制结果数量
        If _params.MaxResults > 0 AndAlso results.Count > _params.MaxResults Then
            results = results.Take(_params.MaxResults).ToList()
        End If

        Return results
    End Function

    ''' <summary>
    ''' 重载: 自动从样本分组计算p值后执行注释
    ''' </summary>
    ''' <param name="peaks">一级质谱离子峰数组</param>
    ''' <param name="controlSamples">对照组样本名列表</param>
    ''' <param name="treatmentSamples">处理组样本名列表</param>
    Public Function Annotate(peaks As IEnumerable(Of xcms2),
                              controlSamples As IEnumerable(Of String),
                              treatmentSamples As IEnumerable(Of String)) As List(Of AnnotationResult)

        Dim pValues = ComputePValues(peaks, controlSamples, treatmentSamples)
        Return Annotate(peaks, pValues)
    End Function

    ' ================================================================
    ' 步骤2: 构建理论代谢物质量库
    ' ================================================================

    ''' <summary>
    ''' 构建理论m/z库
    ''' <para>为每个KEGG代谢物计算所有加合物形式的理论m/z</para>
    ''' </summary>
    Private Function BuildTheoreticalLibrary() As List(Of TheoreticalMz)
        Dim library As New List(Of TheoreticalMz)()

        For Each kvp In _metabolites
            Dim metabolite = kvp.Value
            If metabolite.ExactMass <= 0 Then Continue For

            For Each adduct In _adducts
                Dim entry As New TheoreticalMz With {
                    .Metabolite = metabolite,
                    .Adduct = adduct
                }
                library.Add(entry)
            Next
        Next

        ' 按m/z排序, 便于后续二分查找匹配
        library.Sort(Function(a, b) a.Mz.CompareTo(b.Mz))

        Return library
    End Function

    ' ================================================================
    ' 步骤3: 初始质荷比匹配
    ' ================================================================

    ''' <summary>
    ''' 将实验峰与理论m/z库进行匹配
    ''' <para>使用ppm容忍度, 一个峰可匹配多个理论条目 (多对多模糊匹配)</para>
    ''' </summary>
    Private Function MatchPeaksToLibrary(peaks As List(Of xcms2),
                                          peakPValues As Dictionary(Of String, Double)) As List(Of MzMatch)

        Dim matches As New List(Of MzMatch)()

        ' 将理论库的m/z提取为数组, 用于二分查找
        Dim mzArray As Double() = _theoreticalLibrary.Select(Function(t) t.Mz).ToArray()

        For Each peak In peaks
            Dim observedMz As Double = peak.mz
            If observedMz <= 0 Then Continue For

            ' 计算ppm容忍度对应的质量窗口
            Dim toleranceDa As Double = MathUtils.PpmToDa(observedMz, _params.PpmTolerance)
            Dim mzMin As Double = observedMz - toleranceDa
            Dim mzMax As Double = observedMz + toleranceDa

            ' 二分查找范围内的理论m/z
            Dim lowerIdx As Integer = LowerBound(mzArray, mzMin)
            Dim upperIdx As Integer = UpperBound(mzArray, mzMax)

            ' 遍历范围内的所有理论条目
            For i As Integer = lowerIdx To upperIdx - 1
                If i < 0 OrElse i >= _theoreticalLibrary.Count Then Continue For

                Dim theoretical = _theoreticalLibrary(i)
                Dim ppmError As Double = MathUtils.CalculatePpm(observedMz, theoretical.Mz)

                ' 双重检查ppm容忍度
                If Math.Abs(ppmError) <= _params.PpmTolerance Then
                    Dim match As New MzMatch With {
                        .Peak = peak,
                        .Theoretical = theoretical,
                        .PpmError = ppmError,
                        .PValue = If(peakPValues.ContainsKey(peak.ID), peakPValues(peak.ID), 1.0)
                    }
                    matches.Add(match)
                End If
            Next
        Next

        Return matches
    End Function

    ''' <summary>
    ''' 二分查找: 返回第一个 >= target 的索引
    ''' </summary>
    Private Function LowerBound(arr As Double(), target As Double) As Integer
        Dim lo As Integer = 0
        Dim hi As Integer = arr.Length
        While lo < hi
            Dim mid As Integer = (lo + hi) \ 2
            If arr(mid) < target Then
                lo = mid + 1
            Else
                hi = mid
            End If
        End While
        Return lo
    End Function

    ''' <summary>
    ''' 二分查找: 返回第一个 > target 的索引
    ''' </summary>
    Private Function UpperBound(arr As Double(), target As Double) As Integer
        Dim lo As Integer = 0
        Dim hi As Integer = arr.Length
        While lo < hi
            Dim mid As Integer = (lo + hi) \ 2
            If arr(mid) <= target Then
                lo = mid + 1
            Else
                hi = mid
            End If
        End While
        Return lo
    End Function

    ' ================================================================
    ' 步骤4: 构建独立背景模型
    ' ================================================================

    ''' <summary>
    ''' 构建经验背景模型
    ''' <para>
    ''' 将所有匹配上的特征峰 (不论文显著与否) 作为经验背景,
    ''' 计算各个加合物的出现频率, 用于评估加合物可能性权重
    ''' </para>
    ''' </summary>
    Private Function BuildBackgroundModel(matches As List(Of MzMatch)) As Dictionary(Of String, Double)
        Dim adductCounts As New Dictionary(Of String, Integer)
        Dim totalMatches As Integer = 0

        For Each m In matches
            Dim adductName As String = m.Theoretical.Adduct.Name
            If adductCounts.ContainsKey(adductName) Then
                adductCounts(adductName) += 1
            Else
                adductCounts(adductName) = 1
            End If
            totalMatches += 1
        Next

        ' 计算频率
        Dim frequencies As New Dictionary(Of String, Double)
        For Each kvp In adductCounts
            ' 频率 = 该加合物匹配数 / 总匹配数
            ' 常见加合物给予基础权重, 防止罕见加合物频率过低
            Dim freq As Double = CDbl(kvp.Value) / CDbl(Math.Max(1, totalMatches))
            ' 平滑处理: 常见加合物至少0.1, 罕见加合物至少0.01
            Dim isCommon As Boolean = _adducts.First(Function(a) a.Name = kvp.Key).IsCommon
            Dim minFreq As Double = If(isCommon, 0.1, 0.01)
            frequencies(kvp.Key) = Math.Max(freq, minFreq)
        Next

        ' 确保所有加合物都有频率值
        For Each adduct In _adducts
            If Not frequencies.ContainsKey(adduct.Name) Then
                frequencies(adduct.Name) = If(adduct.IsCommon, 0.1, 0.01)
            End If
        Next

        Return frequencies
    End Function

    ' ================================================================
    ' 步骤5: 通路富集与活性评估
    ' ================================================================

    ''' <summary>
    ''' 执行通路富集分析
    ''' <para>
    ''' 1. 筛选显著差异峰 (p值 &lt; 阈值) 作为input list
    ''' 2. 对每条KEGG通路, 计算显著峰中匹配到该通路的数量
    ''' 3. 使用超几何检验计算p值
    ''' 4. BH-FDR校正
    ''' </para>
    ''' </summary>
    Private Function PerformPathwayEnrichment(peaks As List(Of xcms2),
                                                peakPValues As Dictionary(Of String, Double)) As List(Of PathwayEnrichmentResult)

        ' --- 筛选显著峰 ---
        Dim significantPeakIds As New HashSet(Of String)
        For Each p As xcms2 In peaks
            Dim pval As Double = If(peakPValues.ContainsKey(p.ID), peakPValues(p.ID), 1.0)
            If pval < _params.PValueCutoff Then
                significantPeakIds.Add(p.ID)
            End If
        Next

        ' --- 构建背景: 所有匹配上的峰 (去重) ---
        Dim backgroundPeakIds As New HashSet(Of String)
        For Each m In _allMatches
            backgroundPeakIds.Add(m.Peak.ID)
        Next

        ' --- 统计每条通路的匹配情况 ---
        ' 通路ID -> (背景匹配峰ID集合, 显著匹配峰ID集合)
        Dim pathwayBackgroundHits As New Dictionary(Of String, HashSet(Of String))
        Dim pathwaySignificantHits As New Dictionary(Of String, HashSet(Of String))

        For Each m In _allMatches
            ' 获取该代谢物所在的所有通路
            For Each pathwayId In m.Theoretical.Metabolite.Pathways
                If Not _pathways.ContainsKey(pathwayId) Then Continue For

                If Not pathwayBackgroundHits.ContainsKey(pathwayId) Then
                    pathwayBackgroundHits(pathwayId) = New HashSet(Of String)()
                    pathwaySignificantHits(pathwayId) = New HashSet(Of String)()
                End If

                ' 背景匹配 (使用峰ID去重, 一个峰匹配多个代谢物只算一次)
                pathwayBackgroundHits(pathwayId).Add(m.Peak.ID)

                ' 显著匹配
                If significantPeakIds.Contains(m.Peak.ID) Then
                    pathwaySignificantHits(pathwayId).Add(m.Peak.ID)
                End If
            Next
        Next

        ' --- 超几何检验 ---
        Dim N As Integer = backgroundPeakIds.Count  ' 背景总数
        Dim n As Integer = significantPeakIds.Count ' 显著峰总数

        Dim results As New List(Of PathwayEnrichmentResult)()

        For Each kvp In _pathways
            Dim pathwayId As String = kvp.Key
            Dim pathway As KEGGPathway = kvp.Value

            ' 该通路的背景匹配数
            Dim K As Integer = If(pathwayBackgroundHits.ContainsKey(pathwayId),
                                  pathwayBackgroundHits(pathwayId).Count, 0)

            ' 该通路的显著匹配数
            Dim k As Integer = If(pathwaySignificantHits.ContainsKey(pathwayId),
                                  pathwaySignificantHits(pathwayId).Count, 0)

            ' 跳过没有显著匹配的通路
            If K = 0 Then Continue For

            ' 超几何检验p值
            Dim pValue As Double = MathUtils.HypergeometricPValue(N, K, N, K)

            ' 收集匹配详情
            Dim hitMatches As New List(Of MzMatch)()
            If pathwaySignificantHits.ContainsKey(pathwayId) Then
                Dim sigPeakIds = pathwaySignificantHits(pathwayId)
                For Each m In _allMatches
                    If sigPeakIds.Contains(m.Peak.ID) AndAlso
                       m.Theoretical.Metabolite.Pathways.Contains(pathwayId) Then
                        hitMatches.Add(m)
                    End If
                Next
            End If

            Dim result As New PathwayEnrichmentResult With {
                .Pathway = pathway,
                .PathwaySize = pathway.Metabolites.Count,
                .BackgroundHits = K,
                .SignificantHits = K,
                .TotalSignificant = N,
                .TotalBackground = N,
                .PValue = pValue,
                .HitMatches = hitMatches
            }
            results.Add(result)
        Next

        ' --- BH-FDR校正 ---
        If results.Count > 0 Then
            Dim pVals As Double() = results.Select(Function(r) r.PValue).ToArray()
            Dim fdrs As Double() = MathUtils.BHCorrection(pVals)
            For i As Integer = 0 To results.Count - 1
                results(i).FDR = fdrs(i)
                results(i).IsSignificant = fdrs(i) < _params.FdrCutoff
            Next
        End If

        ' 按p值升序排序
        results.Sort(Function(a, b) a.PValue.CompareTo(b.PValue))

        Return results
    End Function

    ' ================================================================
    ' 步骤6: 反向精确注释
    ' ================================================================

    ''' <summary>
    ''' 反向精确注释: 仅保留显著通路中的候选代谢物, 并综合打分
    ''' <para>
    ''' 打分维度:
    ''' 1. 通路富集得分 (该代谢物所在显著通路的 -log10(p) 之和)
    ''' 2. 加合物一致性 (同一代谢物检测到多个加合物则加分)
    ''' 3. 同位素验证 (检测到M+1, M+2同位素峰则加分)
    ''' 4. 质量精度 (ppm误差越小得分越高)
    ''' </para>
    ''' </summary>
    Private Function ReverseAnnotation(peaks As List(Of xcms2),
                                        peakPValues As Dictionary(Of String, Double)) As List(Of AnnotationResult)

        ' --- 获取显著通路集合 ---
        Dim significantPathways As New HashSet(Of String)
        Dim pathwayScoreLookup As New Dictionary(Of String, Double) ' pathwayId -> -log10(p)

        For Each pr In _pathwayResults
            If pr.IsSignificant Then
                significantPathways.Add(pr.Pathway.ID)
                pathwayScoreLookup(pr.Pathway.ID) = pr.Score
            End If
        Next

        ' --- 按代谢物分组匹配结果, 用于加合物一致性分析 ---
        ' metaboliteId -> List(Of MzMatch)
        Dim matchesByMetabolite As New Dictionary(Of String, List(Of MzMatch))
        For Each m In _allMatches
            Dim metId As String = m.Theoretical.Metabolite.ID
            If Not matchesByMetabolite.ContainsKey(metId) Then
                matchesByMetabolite(metId) = New List(Of MzMatch)()
            End If
            matchesByMetabolite(metId).Add(m)
        Next

        ' --- 构建峰查找表 (用于同位素搜索) ---
        Dim peakList As List(Of xcms2) = peaks.ToList()
        Dim sortedPeaksByMz As List(Of xcms2) = peakList.OrderBy(Function(p) p.mz).ToList()
        Dim peakMzArray As Double() = sortedPeaksByMz.Select(Function(p) p.mz).ToArray()

        ' --- 峰强度查找 (用于同位素强度比较) ---
        Dim peakIntensityLookup As New Dictionary(Of String, Double)
        For Each p As xcms2 In peakList
            ' 使用所有样本的平均强度作为峰强度估计
            Dim avgIntensity As Double = 0.0
            If p.Properties IsNot Nothing AndAlso p.Properties.Count > 0 Then
                avgIntensity = p.Properties.Values.Average()
            End If
            peakIntensityLookup(p.ID) = avgIntensity
        Next

        ' --- 遍历所有匹配, 生成注释结果 ---
        Dim results As New List(Of AnnotationResult)()

        For Each m In _allMatches
            Dim metabolite = m.Theoretical.Metabolite
            Dim adduct = m.Theoretical.Adduct

            ' --- 反向过滤: 检查该代谢物是否在至少一条显著通路中 ---
            Dim metSignificantPathways As New List(Of KEGGPathway)()
            Dim pathwayScore As Double = 0.0

            For Each pathwayId In metabolite.Pathways
                If significantPathways.Contains(pathwayId) AndAlso _pathways.ContainsKey(pathwayId) Then
                    metSignificantPathways.Add(_pathways(pathwayId))
                    If pathwayScoreLookup.ContainsKey(pathwayId) Then
                        pathwayScore += pathwayScoreLookup(pathwayId)
                    End If
                End If
            Next

            ' 如果没有显著通路, 仍然保留但降低权重 (pathwayScore=0)
            ' 这允许在无显著通路时仍输出候选注释

            ' --- 加合物一致性评分 ---
            Dim detectedAdducts As New List(Of String)()
            Dim adductScore As Double = 0.0

            If matchesByMetabolite.ContainsKey(metabolite.ID) Then
                Dim metMatches = matchesByMetabolite(metabolite.ID)
                ' 收集该代谢物检测到的所有不同加合物
                Dim adductSet As New HashSet(Of String)()
                For Each mm In metMatches
                    adductSet.Add(mm.Theoretical.Adduct.Name)
                Next
                detectedAdducts = adductSet.ToList()

                ' 加合物一致性得分: log(1 + n_adducts) / log(1 + max_adducts)
                ' 检测到越多加合物, 可信度越高
                Dim nAdducts As Integer = detectedAdducts.Count
                Dim maxAdducts As Integer = _adducts.Count
                adductScore = Math.Log(1 + nAdducts) / Math.Log(1 + maxAdducts)

                ' 加合物频率加权 (常见加合物权重更高)
                Dim freqWeight As Double = 1.0
                If _adductFrequencies.ContainsKey(adduct.Name) Then
                    freqWeight = _adductFrequencies(adduct.Name)
                End If
                adductScore *= freqWeight
            End If

            ' --- 同位素验证评分 ---
            Dim isotopeScore As Double = 0.0
            Dim isotopeDetails As String = ""

            If Not String.IsNullOrEmpty(metabolite.Formula) Then
                Dim isoResult = ValidateIsotopePattern(m.Peak, metabolite, adduct,
                                                       sortedPeaksByMz, peakMzArray,
                                                       peakIntensityLookup)
                isotopeScore = isoResult.Item1
                isotopeDetails = isoResult.Item2
            End If

            ' --- 质量精度评分 ---
            Dim massScore As Double = 1.0 - Math.Abs(m.PpmError) / _params.PpmTolerance
            massScore = Math.Max(0.0, Math.Min(1.0, massScore))

            ' --- 计算最终优先级得分 ---
            Dim priorityScore As Double =
                _params.WeightPathway * pathwayScore +
                _params.WeightAdduct * adductScore +
                _params.WeightIsotope * isotopeScore +
                _params.WeightMass * massScore

            ' 如果不在显著通路中, 惩罚得分
            If metSignificantPathways.Count = 0 Then
                priorityScore *= 0.3
            End If

            ' --- 构建注释结果 ---
            Dim result As New AnnotationResult With {
                .Peak = m.Peak,
                .Metabolite = metabolite,
                .Adduct = adduct,
                .PpmError = m.PpmError,
                .PValue = m.PValue,
                .SignificantPathways = metSignificantPathways,
                .PathwayScore = pathwayScore,
                .AdductConsistencyScore = adductScore,
                .IsotopeScore = isotopeScore,
                .MassAccuracyScore = massScore,
                .DetectedAdducts = detectedAdducts,
                .IsotopeDetails = isotopeDetails,
                .PriorityScore = priorityScore
            }
            results.Add(result)
        Next

        ' --- 每个峰只保留得分最高的N个候选 ---
        If _params.MaxCandidatesPerPeak > 0 Then
            results = results _
                .GroupBy(Function(r) r.Peak.ID) _
                .SelectMany(Function(g) g _
                    .OrderByDescending(Function(r) r.PriorityScore) _
                    .Take(_params.MaxCandidatesPerPeak)) _
                .ToList()
        End If

        Return results
    End Function

    ''' <summary>
    ''' 同位素模式验证
    ''' <para>
    ''' 检查实验数据中是否存在对应于该代谢物的M+1, M+2同位素峰,
    ''' 并验证强度比是否符合理论预测
    ''' </para>
    ''' </summary>
    Private Function ValidateIsotopePattern(peak As xcms2,
                                             metabolite As KEGGMetabolite,
                                             adduct As AdductRule,
                                             sortedPeaks As List(Of xcms2),
                                             peakMzArray As Double(),
                                             peakIntensityLookup As Dictionary(Of String, Double)) As Tuple(Of Double, String)

        Dim score As Double = 0.0
        Dim details As New System.Text.StringBuilder()

        ' 预测理论同位素比
        Dim ratios As Double() = FormulaUtils.PredictIsotopeRatios(metabolite.Formula)
        Dim expectedM1Ratio As Double = ratios(0) ' M+1/M
        Dim expectedM2Ratio As Double = ratios(1) ' M+2/M

        ' 理论M+1, M+2的m/z (考虑加合物和电荷)
        ' 对于单电荷: M+1的m/z = 原m/z + 13C_delta / |charge|
        ' 对于多电荷: 每个电荷位置都可能有13C, 所以delta = 13C_delta / |charge|
        Dim charge As Integer = Math.Abs(adduct.Charge)
        Dim monoMz As Double = adduct.CalculateMz(metabolite.ExactMass)

        Dim m1Mz As Double = monoMz + FormulaUtils.C13_DELTA / charge
        Dim m2Mz As Double = monoMz + 2 * FormulaUtils.C13_DELTA / charge

        ' 获取当前峰的强度
        Dim monoIntensity As Double = If(peakIntensityLookup.ContainsKey(peak.ID),
                                          peakIntensityLookup(peak.ID), 0.0)

        ' --- 搜索M+1峰 ---
        Dim m1Peak As xcms2 = FindPeakByMz(sortedPeaks, peakMzArray, m1Mz, _params.IsotopePpmTolerance)
        If m1Peak IsNot Nothing Then
            Dim m1Intensity As Double = If(peakIntensityLookup.ContainsKey(m1Peak.ID),
                                            peakIntensityLookup(m1Peak.ID), 0.0)
            If monoIntensity > 0 AndAlso m1Intensity > 0 Then
                Dim observedRatio As Double = m1Intensity / monoIntensity
                ' 检查强度比是否在容忍范围内
                Dim lowerBound As Double = expectedM1Ratio / _params.IsotopeIntensityTolerance
                Dim upperBound As Double = expectedM1Ratio * _params.IsotopeIntensityTolerance
                If observedRatio >= lowerBound AndAlso observedRatio <= upperBound Then
                    score += 0.5
                    details.Append($"M+1: found (m/z={m1Peak.mz:F4}, ratio={observedRatio:F3}, expected={expectedM1Ratio:F3}); ")
                Else
                    score += 0.2
                    details.Append($"M+1: found but ratio mismatch (obs={observedRatio:F3}, exp={expectedM1Ratio:F3}); ")
                End If
            Else
                score += 0.1
                details.Append("M+1: found but no intensity; ")
            End If
        Else
            details.Append("M+1: not found; ")
        End If

        ' --- 搜索M+2峰 ---
        Dim m2Peak As xcms2 = FindPeakByMz(sortedPeaks, peakMzArray, m2Mz, _params.IsotopePpmTolerance)
        If m2Peak IsNot Nothing Then
            Dim m2Intensity As Double = If(peakIntensityLookup.ContainsKey(m2Peak.ID),
                                            peakIntensityLookup(m2Peak.ID), 0.0)
            If monoIntensity > 0 AndAlso m2Intensity > 0 Then
                Dim observedRatio As Double = m2Intensity / monoIntensity
                Dim lowerBound As Double = expectedM2Ratio / _params.IsotopeIntensityTolerance
                Dim upperBound As Double = expectedM2Ratio * _params.IsotopeIntensityTolerance
                If observedRatio >= lowerBound AndAlso observedRatio <= upperBound Then
                    score += 0.5
                    details.Append($"M+2: found (m/z={m2Peak.mz:F4}, ratio={observedRatio:F3}, expected={expectedM2Ratio:F3})")
                Else
                    score += 0.2
                    details.Append($"M+2: found but ratio mismatch (obs={observedRatio:F3}, exp={expectedM2Ratio:F3})")
                End If
            Else
                score += 0.1
                details.Append("M+2: found but no intensity")
            End If
        Else
            details.Append("M+2: not found")
        End If

        ' 限制得分在[0,1]范围
        score = Math.Max(0.0, Math.Min(1.0, score))

        Return Tuple.Create(score, details.ToString())
    End Function

    ''' <summary>
    ''' 在排序的峰列表中按m/z查找最接近的峰 (ppm容忍度)
    ''' </summary>
    Private Function FindPeakByMz(sortedPeaks As List(Of xcms2),
                                   mzArray As Double(),
                                   targetMz As Double,
                                   ppmTolerance As Double) As xcms2

        Dim toleranceDa As Double = MathUtils.PpmToDa(targetMz, ppmTolerance)
        Dim mzMin As Double = targetMz - toleranceDa
        Dim mzMax As Double = targetMz + toleranceDa

        Dim lowerIdx As Integer = LowerBound(mzArray, mzMin)
        Dim upperIdx As Integer = UpperBound(mzArray, mzMax)

        Dim bestPeak As xcms2 = Nothing
        Dim bestPpm As Double = Double.MaxValue

        For i As Integer = lowerIdx To upperIdx - 1
            If i < 0 OrElse i >= sortedPeaks.Count Then Continue For
            Dim ppm As Double = Math.Abs(MathUtils.CalculatePpm(sortedPeaks(i).mz, targetMz))
            If ppm < bestPpm Then
                bestPpm = ppm
                bestPeak = sortedPeaks(i)
            End If
        Next

        Return bestPeak
    End Function

    ' ================================================================
    ' 辅助方法: 从样本数据计算p值
    ' ================================================================

    ''' <summary>
    ''' 从xcms2峰表的样本数据计算差异表达p值
    ''' <para>使用Welch's t检验比较对照组和处理组</para>
    ''' </summary>
    ''' <param name="peaks">离子峰列表</param>
    ''' <param name="controlSamples">对照组样本名</param>
    ''' <param name="treatmentSamples">处理组样本名</param>
    Public Function ComputePValues(peaks As IEnumerable(Of xcms2),
                                    controlSamples As IEnumerable(Of String),
                                    treatmentSamples As IEnumerable(Of String)) As Dictionary(Of String, Double)

        Dim controlSet As New HashSet(Of String)(controlSamples)
        Dim treatmentSet As New HashSet(Of String)(treatmentSamples)
        Dim pValues As New Dictionary(Of String, Double)

        For Each peak In peaks
            Dim controlValues As New List(Of Double)
            Dim treatmentValues As New List(Of Double)

            If peak.Properties IsNot Nothing Then
                For Each kvp In peak.Properties
                    If controlSet.Contains(kvp.Key) AndAlso kvp.Value > 0 Then
                        controlValues.Add(kvp.Value)
                    End If
                    If treatmentSet.Contains(kvp.Key) AndAlso kvp.Value > 0 Then
                        treatmentValues.Add(kvp.Value)
                    End If
                Next
            End If

            ' 样本量不足, p值设为1.0 (不显著)
            If controlValues.Count < 2 OrElse treatmentValues.Count < 2 Then
                pValues(peak.ID) = 1.0
            Else
                pValues(peak.ID) = MathUtils.WelchTTest(controlValues, treatmentValues)
            End If
        Next

        Return pValues
    End Function

    ' ================================================================
    ' 结果导出方法
    ' ================================================================

    ''' <summary>
    ''' 将注释结果导出为DataTable (便于显示和保存)
    ''' </summary>
    Public Function ResultsToDataTable(results As List(Of AnnotationResult)) As DataTable
        Dim dt As New DataTable()

        dt.Columns.Add("Rank", GetType(Integer))
        dt.Columns.Add("Peak_ID", GetType(String))
        dt.Columns.Add("mz", GetType(Double))
        dt.Columns.Add("rt", GetType(Double))
        dt.Columns.Add("PValue", GetType(Double))
        dt.Columns.Add("KEGG_ID", GetType(String))
        dt.Columns.Add("Metabolite_Name", GetType(String))
        dt.Columns.Add("Formula", GetType(String))
        dt.Columns.Add("Adduct", GetType(String))
        dt.Columns.Add("PpmError", GetType(Double))
        dt.Columns.Add("SignificantPathways", GetType(String))
        dt.Columns.Add("PathwayScore", GetType(Double))
        dt.Columns.Add("AdductConsistencyScore", GetType(Double))
        dt.Columns.Add("IsotopeScore", GetType(Double))
        dt.Columns.Add("MassAccuracyScore", GetType(Double))
        dt.Columns.Add("DetectedAdducts", GetType(String))
        dt.Columns.Add("IsotopeDetails", GetType(String))
        dt.Columns.Add("PriorityScore", GetType(Double))
        dt.Columns.Add("ConfidenceLevel", GetType(String))

        For Each r In results
            Dim row As DataRow = dt.NewRow()
            row("Rank") = r.Rank
            row("Peak_ID") = r.Peak.ID
            row("mz") = r.Peak.mz
            row("rt") = r.Peak.rt
            row("PValue") = r.PValue
            row("KEGG_ID") = r.Metabolite.ID
            row("Metabolite_Name") = r.Metabolite.Name
            row("Formula") = If(r.Metabolite.Formula, "")
            row("Adduct") = r.Adduct.Name
            row("PpmError") = Math.Round(r.PpmError, 3)
            row("SignificantPathways") = String.Join("; ", r.SignificantPathways.Select(Function(p) p.ID))
            row("PathwayScore") = Math.Round(r.PathwayScore, 4)
            row("AdductConsistencyScore") = Math.Round(r.AdductConsistencyScore, 4)
            row("IsotopeScore") = Math.Round(r.IsotopeScore, 4)
            row("MassAccuracyScore") = Math.Round(r.MassAccuracyScore, 4)
            row("DetectedAdducts") = String.Join("; ", r.DetectedAdducts)
            row("IsotopeDetails") = If(r.IsotopeDetails, "")
            row("PriorityScore") = Math.Round(r.PriorityScore, 4)
            row("ConfidenceLevel") = r.ConfidenceLevel
            dt.Rows.Add(row)
        Next

        Return dt
    End Function

    ''' <summary>
    ''' 将通路富集结果导出为DataTable
    ''' </summary>
    Public Function PathwayResultsToDataTable() As DataTable
        Dim dt As New DataTable()

        dt.Columns.Add("Pathway_ID", GetType(String))
        dt.Columns.Add("Pathway_Name", GetType(String))
        dt.Columns.Add("PathwaySize", GetType(Integer))
        dt.Columns.Add("SignificantHits", GetType(Integer))
        dt.Columns.Add("BackgroundHits", GetType(Integer))
        dt.Columns.Add("TotalSignificant", GetType(Integer))
        dt.Columns.Add("TotalBackground", GetType(Integer))
        dt.Columns.Add("PValue", GetType(Double))
        dt.Columns.Add("FDR", GetType(Double))
        dt.Columns.Add("Score", GetType(Double))
        dt.Columns.Add("IsSignificant", GetType(Boolean))

        For Each r In _pathwayResults
            Dim row As DataRow = dt.NewRow()
            row("Pathway_ID") = r.Pathway.ID
            row("Pathway_Name") = r.Pathway.Name
            row("PathwaySize") = r.PathwaySize
            row("SignificantHits") = r.SignificantHits
            row("BackgroundHits") = r.BackgroundHits
            row("TotalSignificant") = r.TotalSignificant
            row("TotalBackground") = r.TotalBackground
            row("PValue") = r.PValue
            row("FDR") = r.FDR
            row("Score") = Math.Round(r.Score, 4)
            row("IsSignificant") = r.IsSignificant
            dt.Rows.Add(row)
        Next

        Return dt
    End Function

End Class

' ========================================================================
' KEGG数据加载辅助类
' ========================================================================

''' <summary>
''' KEGG数据库加载辅助工具
''' <para>提供从简单文本文件加载KEGG代谢物和通路数据的方法</para>
''' </summary>
Public Class KEGGDataLoader

    ''' <summary>
    ''' 从TSV文件加载KEGG代谢物
    ''' <para>
    ''' 文件格式 (Tab分隔):
    ''' KEGG_ID     Name    Formula ExactMass       Pathways
    ''' C00022      Pyruvate        C3H4O3  88.016045       map00010;map00020
    ''' </para>
    ''' </summary>
    Public Shared Function LoadMetabolites(filePath As String) As List(Of KEGGMetabolite)
        Dim metabolites As New List(Of KEGGMetabolite)()
        Dim lines As String() = System.IO.File.ReadAllLines(filePath)

        For i As Integer = 1 To lines.Length - 1 ' 跳过表头
            Dim line As String = lines(i).Trim()
            If String.IsNullOrEmpty(line) Then Continue For

            Dim fields As String() = line.Split(ControlChars.Tab)
            If fields.Length < 4 Then Continue For

            Dim met As New KEGGMetabolite With {
                .ID = fields(0).Trim(),
                .Name = fields(1).Trim(),
                .Formula = fields(2).Trim()
            }

            ' 解析精确分子量
            Dim mass As Double = 0.0
            If Double.TryParse(fields(3).Trim(), mass) Then
                met.ExactMass = mass
            Else
                ' 尝试从分子式计算
                met.RecalculateMass()
            End If

            ' 解析通路列表 (分号分隔)
            If fields.Length >= 5 AndAlso Not String.IsNullOrEmpty(fields(4)) Then
                Dim pathwayIds As String() = fields(4).Split(";"c)
                For Each PID As String In pathwayIds
                    Dim trimmed = PID.Trim()
                    If Not String.IsNullOrEmpty(trimmed) Then
                        met.Pathways.Add(trimmed)
                    End If
                Next
            End If

            ' 如果没有精确分子量, 尝试从分子式计算
            If met.ExactMass <= 0 AndAlso Not String.IsNullOrEmpty(met.Formula) Then
                met.RecalculateMass()
            End If

            If met.ExactMass > 0 Then
                metabolites.Add(met)
            End If
        Next

        Return metabolites
    End Function

    ''' <summary>
    ''' 从TSV文件加载KEGG通路
    ''' <para>
    ''' 文件格式 (Tab分隔):
    ''' Pathway_ID  Name    Metabolites
    ''' map00010    Glycolysis / Gluconeogenesis    C00022;C00033;C00036
    ''' </para>
    ''' </summary>
    Public Shared Function LoadPathways(filePath As String) As List(Of KEGGPathway)
        Dim pathways As New List(Of KEGGPathway)()
        Dim lines As String() = System.IO.File.ReadAllLines(filePath)

        For i As Integer = 1 To lines.Length - 1 ' 跳过表头
            Dim line As String = lines(i).Trim()
            If String.IsNullOrEmpty(line) Then Continue For

            Dim fields As String() = line.Split(ControlChars.Tab)
            If fields.Length < 2 Then Continue For

            Dim pathway As New KEGGPathway With {
                .ID = fields(0).Trim(),
                .Name = fields(1).Trim()
            }

            ' 解析代谢物列表
            If fields.Length >= 3 AndAlso Not String.IsNullOrEmpty(fields(2)) Then
                Dim metIds As String() = fields(2).Split(";"c)
                For Each Mid As String In metIds
                    Dim trimmed = Mid.Trim()
                    If Not String.IsNullOrEmpty(trimmed) Then
                        pathway.Metabolites.Add(trimmed)
                    End If
                Next
            End If

            pathways.Add(pathway)
        Next

        Return pathways
    End Function
End Class


' ============================================================================
' 算法原理说明
' ----------------------------------------------------------------------------
'
' Mummichog算法 (Li et al., 2013, Nature Methods) 是一种基于代谢通路先验
' 知识的代谢组学预注释方法。与传统"先注释后通路分析"的策略不同, Mummichog
' 采用"先通路分析, 后代谢物注释"的反向策略:
'
' 1. [输入准备] 接收一级质谱峰表(m/z, rt, 样本强度)和统计p值
'
' 2. [理论库构建] 将KEGG代谢物与所有可能的加合物组合, 生成理论m/z库
'    例如: 葡萄糖(M=180.0634) + [M+H]+ -> m/z = 181.0707
'                   + [M+Na]+ -> m/z = 203.0526
'                   + [M+K]+  -> m/z = 219.0265
'
' 3. [m/z匹配] 在ppm容忍度内, 将实验峰与理论m/z进行多对多模糊匹配
'    一个实验峰可能对应多个代谢物候选, 一个代谢物也可能被多个峰检测到
'
' 4. [背景模型] 统计所有匹配中各加合物的出现频率, 构建经验背景分布
'    用于后续评估加合物可能性 (常见加合物权重更高)
'
' 5. [通路富集] 对每条KEGG通路, 使用超几何检验评估显著差异峰是否富集
'    H0: 显著峰中匹配到该通路的比例 = 背景中匹配到该通路的比例
'    H1: 显著峰中匹配到该通路的比例 > 背景中匹配到该通路的比例
'    使用BH-FDR校正控制错误发现率
'
' 6. [反向注释] 仅保留显著通路中的候选代谢物, 综合以下维度打分:
'    - 通路富集得分: 该代谢物所在显著通路的 -log10(p) 之和
'    - 加合物一致性: 同一代谢物检测到多个加合物则可信度更高
'    - 同位素验证: 检测到M+1, M+2同位素峰且强度比符合理论预测
'    - 质量精度: ppm误差越小得分越高
'    最终按优先级得分降序排列, 输出预注释结果
'
' 优势: 无需MS/MS数据即可进行代谢物预注释, 适合大规模非靶向代谢组学
' 局限: 预注释结果为putative级别, 需后续MS/MS标准品库搜索验证
'
' ============================================================================
