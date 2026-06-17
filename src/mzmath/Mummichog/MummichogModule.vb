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

Imports System.Data
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports F = Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact.FishersExactTest

' ========================================================================
' Mummichog主注释器类
' 实现完整的六步算法流程
' ========================================================================

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
    Private _adducts As MzCalculator()

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
    Public Sub New(metabolites As IEnumerable(Of KEGGMetabolite), pathways As IEnumerable(Of KEGGPathway), Optional params As MummichogParams = Nothing)
        _params = If(params, New MummichogParams())
        ' 构建代谢物字典
        _metabolites = New Dictionary(Of String, KEGGMetabolite)

        For Each m In metabolites
            If m.ExactMass <= 0 AndAlso Not String.IsNullOrEmpty(m.Formula) Then
                m.RecalculateMass()
            End If
            If m.ExactMass > 0 Then
                _metabolites(m.Id) = m
            End If
        Next

        Dim missingRefs = _metabolites.Values.All(Function(c) c.Pathways Is Nothing OrElse c.Pathways.Count = 0)

        ' 构建通路字典
        _pathways = New Dictionary(Of String, KEGGPathway)
        For Each p As KEGGPathway In pathways
            _pathways(p.ID) = p

            If missingRefs Then
                For Each id As String In p.Metabolites.AsEnumerable
                    If _metabolites.ContainsKey(id) Then
                        Call _metabolites(id).Pathways.Add(p.ID)
                    End If
                Next
            End If
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
    Public Function Annotate(peaks As IEnumerable(Of xcms2), pValues As Dictionary(Of String, Double)) As List(Of AnnotationResult)
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
    Public Function Annotate(peaks As IReadOnlyCollection(Of xcms2),
                              controlSamples As IEnumerable(Of String),
                              treatmentSamples As IEnumerable(Of String)) As List(Of AnnotationResult)

        Dim pValues = PValue.ComputePValues(peaks, controlSamples, treatmentSamples)
        Return Annotate(peaks, pValues)
    End Function

    ''' <summary>
    ''' 重载: 自动从样本分组计算p值后执行注释
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <param name="groups"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 采用多组ANOVA单因素F检验计算出pvalue后再排序
    ''' </remarks>
    Public Function Annotate(peaks As IReadOnlyCollection(Of xcms2), groups As DataGroup()) As List(Of AnnotationResult)
        Dim pValues = PValue.ComputePValues(peaks, groups)
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
            Dim toleranceDa As Double = PPMmethod.ConvertPpmToMassAccuracy(observedMz, _params.PpmTolerance)
            Dim mzMin As Double = observedMz - toleranceDa
            Dim mzMax As Double = observedMz + toleranceDa

            ' 二分查找范围内的理论m/z
            Dim lowerIdx As Integer = LowerBound(mzArray, mzMin)
            Dim upperIdx As Integer = UpperBound(mzArray, mzMax)

            ' 遍历范围内的所有理论条目
            For i As Integer = lowerIdx To upperIdx - 1
                If i < 0 OrElse i >= _theoreticalLibrary.Count Then Continue For

                Dim theoretical = _theoreticalLibrary(i)
                Dim ppmError As Double = PPMmethod.PPM(observedMz, theoretical.Mz)

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
            Dim adductName As String = m.Theoretical.Adduct.name
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
            Dim isCommon As Boolean = _adducts.First(Function(a) a.name = kvp.Key).IsCommon
            Dim minFreq As Double = If(isCommon, 0.1, 0.01)
            frequencies(kvp.Key) = Math.Max(freq, minFreq)
        Next

        ' 确保所有加合物都有频率值
        For Each adduct In _adducts
            If Not frequencies.ContainsKey(adduct.name) Then
                frequencies(adduct.name) = If(adduct.IsCommon, 0.1, 0.01)
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
        Dim ni As Integer = significantPeakIds.Count ' 显著峰总数

        Dim results As New List(Of PathwayEnrichmentResult)()

        For Each kvp In _pathways
            Dim pathwayId As String = kvp.Key
            Dim pathway As KEGGPathway = kvp.Value

            ' 该通路的背景匹配数
            Dim K As Integer = If(pathwayBackgroundHits.ContainsKey(pathwayId),
                                  pathwayBackgroundHits(pathwayId).Count, 0)

            ' 该通路的显著匹配数
            Dim ki As Integer = If(pathwaySignificantHits.ContainsKey(pathwayId),
                                  pathwaySignificantHits(pathwayId).Count, 0)

            ' 跳过没有显著匹配的通路
            If ki = 0 Then Continue For

            ' 我们的差异基因列表中，属于目标代谢途径的基因的数量
            Dim a% = ki
            ' 在目标基因组中，属于当前的代谢途径中的基因的数量
            Dim b% = K
            ' 在我们的差异基因列表中，不属于当前的代谢途径的基因的数量
            Dim c% = ni - a
            ' 在目标基因组中，不属于当前的代谢途径中的基因的数量
            Dim d% = N - b
            ' 超几何检验p值
            Dim pValue As FishersExactPvalues = F.FishersExact(a, b, c, d)

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
                .SignificantHits = ki,
                .TotalSignificant = ni,
                .TotalBackground = N,
                .PValue = pValue.two_tail_pvalue,
                .HitMatches = hitMatches
            }
            results.Add(result)
        Next

        ' --- BH-FDR校正 ---
        If results.Count > 0 Then
            Dim pVals As Double() = results.Select(Function(r) r.PValue).ToArray()
            Dim fdrs As Double() = BHCorrection(pVals)
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
            Dim metId As String = m.Theoretical.Metabolite.Id
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

            If matchesByMetabolite.ContainsKey(metabolite.Id) Then
                Dim metMatches = matchesByMetabolite(metabolite.Id)
                ' 收集该代谢物检测到的所有不同加合物
                Dim adductSet As New HashSet(Of String)()
                For Each mm In metMatches
                    adductSet.Add(mm.Theoretical.Adduct.name)
                Next
                detectedAdducts = adductSet.ToList()

                ' 加合物一致性得分: log(1 + n_adducts) / log(1 + max_adducts)
                ' 检测到越多加合物, 可信度越高
                Dim nAdducts As Integer = detectedAdducts.Count
                Dim maxAdducts As Integer = _adducts.Count
                adductScore = Math.Log(1 + nAdducts) / Math.Log(1 + maxAdducts)

                ' 加合物频率加权 (常见加合物权重更高)
                Dim freqWeight As Double = 1.0
                If _adductFrequencies.ContainsKey(adduct.name) Then
                    freqWeight = _adductFrequencies(adduct.name)
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
                                             adduct As MzCalculator,
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
        Dim charge As Integer = Math.Abs(adduct.charge)
        Dim monoMz As Double = adduct.CalcMZ(metabolite.ExactMass)

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

        Dim toleranceDa As Double = PPMmethod.ConvertPpmToMassAccuracy(targetMz, ppmTolerance)
        Dim mzMin As Double = targetMz - toleranceDa
        Dim mzMax As Double = targetMz + toleranceDa

        Dim lowerIdx As Integer = LowerBound(mzArray, mzMin)
        Dim upperIdx As Integer = UpperBound(mzArray, mzMax)

        Dim bestPeak As xcms2 = Nothing
        Dim bestPpm As Double = Double.MaxValue

        For i As Integer = lowerIdx To upperIdx - 1
            If i < 0 OrElse i >= sortedPeaks.Count Then Continue For
            Dim ppm As Double = Math.Abs(PPMmethod.PPM(sortedPeaks(i).mz, targetMz))
            If ppm < bestPpm Then
                bestPpm = ppm
                bestPeak = sortedPeaks(i)
            End If
        Next

        Return bestPeak
    End Function

    ' ================================================================
    ' 结果导出方法
    ' ================================================================

    ''' <summary>
    ''' 将注释结果导出为DataTable (便于显示和保存)
    ''' </summary>
    Public Shared Iterator Function ResultsToDataTable(results As IEnumerable(Of AnnotationResult)) As IEnumerable(Of MetaboliteResult)
        For Each r As AnnotationResult In results
            Yield New MetaboliteResult With {
                .Rank = r.Rank,
                .Peak_ID = r.Peak.ID,
                .mz = r.Peak.mz,
                .rt = r.Peak.rt,
                .PValue = r.PValue,
                .KEGG_ID = r.Metabolite.Id,
                .Metabolite_Name = r.Metabolite.CommonName,
                .Formula = If(r.Metabolite.Formula, ""),
                .Adduct = r.Adduct.name,
                .PpmError = Math.Round(r.PpmError, 3),
                .SignificantPathways = String.Join("; ", r.SignificantPathways.Select(Function(p) p.ID)),
                .PathwayScore = Math.Round(r.PathwayScore, 4),
                .AdductConsistencyScore = Math.Round(r.AdductConsistencyScore, 4),
                .IsotopeScore = Math.Round(r.IsotopeScore, 4),
                .MassAccuracyScore = Math.Round(r.MassAccuracyScore, 4),
                .DetectedAdducts = String.Join("; ", r.DetectedAdducts),
                .IsotopeDetails = If(r.IsotopeDetails, ""),
                .PriorityScore = Math.Round(r.PriorityScore, 4),
                .ConfidenceLevel = r.ConfidenceLevel
            }
        Next
    End Function

    ''' <summary>
    ''' 将通路富集结果导出为DataTable
    ''' </summary>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PathwayResultsToDataTable() As IEnumerable(Of PathwayEnrichment)
        Return PathwayResultsToDataTable(_pathwayResults)
    End Function

    ''' <summary>
    ''' 将通路富集结果导出为DataTable
    ''' </summary>
    Public Shared Iterator Function PathwayResultsToDataTable(pathwayResults As IEnumerable(Of PathwayEnrichmentResult)) As IEnumerable(Of PathwayEnrichment)
        For Each r As PathwayEnrichmentResult In pathwayResults
            Yield New PathwayEnrichment With {
                .Pathway_ID = r.Pathway.ID,
                .Pathway_Name = r.Pathway.Name,
                .PathwaySize = r.PathwaySize,
                .SignificantHits = r.SignificantHits,
                .BackgroundHits = r.BackgroundHits,
                .TotalSignificant = r.TotalSignificant,
                .TotalBackground = r.TotalBackground,
                .PValue = r.PValue,
                .FDR = r.FDR,
                .Score = Math.Round(r.Score, 4),
                .IsSignificant = r.IsSignificant
            }
        Next
    End Function

End Class
