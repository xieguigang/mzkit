
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports std = System.Math

''' <summary>
''' 理论m/z条目 (代谢物 + 加合物的组合)
''' </summary>
Public Class TheoreticalMz

    ''' <summary>对应的KEGG代谢物</summary>
    Public Property Metabolite As KEGGMetabolite

    ''' <summary>对应的加合物规则</summary>
    Public Property Adduct As MzCalculator

    ''' <summary>理论m/z值</summary>
    Public ReadOnly Property Mz As Double
        Get
            Return Adduct.CalcMZ(Metabolite.ExactMass)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Metabolite.Id} {Adduct.name} m/z={Mz:F6}"
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
        Return $"{Peak.ID} -> {Theoretical.Metabolite.Id} {Theoretical.Adduct.name} ({PpmError:F2} ppm)"
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
            Return -std.Log10(PValue)
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
    Public Property Adduct As MzCalculator

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
        Return $"#{Rank} {Peak.mz:F4} -> {Metabolite.Id} {Metabolite.CommonName} [{Adduct.name}] score={PriorityScore:F3} ({ConfidenceLevel})"
    End Function
End Class