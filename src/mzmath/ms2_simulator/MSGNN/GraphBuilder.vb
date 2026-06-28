' ============================================================================
' GraphBuilder.vb
' ============================================================================
' 网络图构建逻辑
' 将MS/MS谱图转换为GNN训练用的网络图数据
' ============================================================================

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports std = System.Math

Public Module GraphBuilder

    ''' <summary>
    ''' 图构建选项
    ''' </summary>
    Public Class GraphBuildOptions
        ''' <summary>质量匹配容差 (Da)</summary>
        Public Property MassTolerance As Double = 0.02

        ''' <summary>是否构建母离子-碎片边</summary>
        Public Property BuildParentFragmentEdges As Boolean = True

        ''' <summary>是否构建碎片-碎片边</summary>
        Public Property BuildFragmentFragmentEdges As Boolean = True

        ''' <summary>是否检测同位素边</summary>
        Public Property DetectIsotopeEdges As Boolean = True

        ''' <summary>是否检测加合物边</summary>
        Public Property DetectAdductEdges As Boolean = True

        ''' <summary>是否检测中性丢失边</summary>
        Public Property DetectNeutralLossEdges As Boolean = True

        ''' <summary>每个节点最大边数 (0表示不限制)</summary>
        Public Property MaxEdgesPerNode As Integer = 0

        ''' <summary>是否构建无向图 (添加反向边)</summary>
        Public Property Undirected As Boolean = True

        ''' <summary>强度归一化方式: "base_peak" 或 "total_ion"</summary>
        Public Property IntensityNormalization As String = "base_peak"

        ''' <summary>是否对强度取对数</summary>
        Public Property LogIntensity As Boolean = False

        ''' <summary>最小强度阈值 (低于此值的峰被过滤, 0表示不过滤)</summary>
        Public Property MinIntensityThreshold As Double = 0.0

        ''' <summary>最大峰数量 (超过则按强度排序取Top N, 0表示不限制)</summary>
        Public Property MaxPeaks As Integer = 0

        ''' <summary>是否过滤母离子附近的峰 (避免母离子自身作为碎片)</summary>
        Public Property FilterPrecursorPeak As Boolean = True

        ''' <summary>母离子峰过滤窗口 (Da)</summary>
        Public Property PrecursorFilterWindow As Double = 1.5
    End Class

    ''' <summary>
    ''' 从谱图构建网络图
    ''' </summary>
    ''' <param name="spectrum">输入谱图</param>
    ''' <param name="options">构建选项</param>
    ''' <returns>图数据</returns>
    Public Function BuildGraph(spectrum As PeakMs2, Optional options As GraphBuildOptions = Nothing) As GraphData
        If options Is Nothing Then
            options = New GraphBuildOptions()
        End If

        Dim graph As New GraphData()
        graph.SpectrumTitle = spectrum.lib_guid

        ' ========================================================
        ' 步骤1: 预处理峰列表
        ' ========================================================
        Dim processedPeaks = PreprocessPeaks(spectrum, options)

        ' ========================================================
        ' 步骤2: 计算归一化强度
        ' ========================================================
        Dim normalizedIntensities = NormalizeIntensities(processedPeaks, spectrum, options)

        ' ========================================================
        ' 步骤3: 构建节点
        ' ========================================================
        Dim featureIonList = KnownConstants.FeatureIonCategories

        ' 添加母离子根节点 (节点ID = 0)
        Dim parentNode As New GraphNode()
        parentNode.Id = 0
        parentNode.Type = NodeType.ParentIon
        parentNode.Mz = spectrum.mz
        parentNode.Intensity = If(spectrum.intensity > 0, spectrum.intensity, spectrum.BasePeakIntensity)
        parentNode.NormalizedIntensity = If(options.LogIntensity,
                                             std.Log(parentNode.Intensity + 1),
                                            1.0)  ' 母离子归一化强度设为1.0
        parentNode.FeatureIonName = Nothing
        parentNode.FeatureIonIndex = -1
        graph.Nodes.Add(parentNode)

        ' 添加碎片节点
        For i = 0 To processedPeaks.Count - 1
            Dim peak = processedPeaks(i)
            Dim node As New GraphNode()
            node.Id = i + 1
            node.Type = NodeType.Fragment
            node.Mz = peak.mz
            node.Intensity = peak.intensity
            node.NormalizedIntensity = normalizedIntensities(i)

            ' 匹配特征离子
            Dim bestMatch = FindBestFeatureIonMatch(peak.mz, options.MassTolerance)
            If bestMatch IsNot Nothing Then
                node.FeatureIonName = bestMatch.Value.Key
                node.FeatureIonIndex = featureIonList.FindIndex(Function(x) x.Key = bestMatch.Value.Key)
            Else
                node.FeatureIonName = Nothing
                node.FeatureIonIndex = -1
            End If

            graph.Nodes.Add(node)
        Next

        ' ========================================================
        ' 步骤4: 构建边
        ' ========================================================
        Dim edgeTypeList = KnownConstants.EdgeTypeCategories

        ' 4a: 母离子-碎片边 (基于中性丢失)
        If options.BuildParentFragmentEdges Then
            BuildParentFragmentEdges(graph, spectrum, options, edgeTypeList)
        End If

        ' 4b: 碎片-碎片边 (基于中性丢失、加合物、同位素)
        If options.BuildFragmentFragmentEdges Then
            BuildFragmentFragmentEdges(graph, options, edgeTypeList)
        End If

        ' ========================================================
        ' 步骤5: 边数量限制 (K近邻)
        ' ========================================================
        If options.MaxEdgesPerNode > 0 Then
            LimitEdgesPerNode(graph, options.MaxEdgesPerNode)
        End If

        ' ========================================================
        ' 步骤6: 构建无向图 (添加反向边)
        ' ========================================================
        If options.Undirected Then
            MakeUndirected(graph, edgeTypeList)
        End If

        ' ========================================================
        ' 步骤7: 记录元数据
        ' ========================================================
        graph.Metadata("num_nodes") = graph.NumNodes
        graph.Metadata("num_edges") = graph.NumEdges
        graph.Metadata("num_node_features") = graph.NumNodeFeatures
        graph.Metadata("num_edge_features") = graph.NumEdgeFeatures
        graph.Metadata("pepmass") = spectrum.mz
        graph.Metadata("charge") = spectrum.charge
        graph.Metadata("num_fragment_peaks") = processedPeaks.Count
        graph.Metadata("mass_tolerance") = options.MassTolerance
        graph.Metadata("title") = spectrum.lib_guid

        Return graph
    End Function

    ' ========================================================
    ' 峰预处理
    ' ========================================================
    Private Function PreprocessPeaks(spectrum As PeakMs2, options As GraphBuildOptions) As List(Of ms2)
        Dim peaks As New List(Of ms2)(spectrum.mzInto)

        ' 过滤母离子附近的峰
        If options.FilterPrecursorPeak Then
            peaks = peaks.Where(Function(p) std.Abs(p.mz - spectrum.mz) > options.PrecursorFilterWindow).ToList()
        End If

        ' 过滤低强度峰
        If options.MinIntensityThreshold > 0 Then
            peaks = peaks.Where(Function(p) p.intensity >= options.MinIntensityThreshold).ToList()
        End If

        ' 按强度排序，取Top N
        If options.MaxPeaks > 0 AndAlso peaks.Count > options.MaxPeaks Then
            peaks = peaks.OrderByDescending(Function(p) p.intensity).Take(options.MaxPeaks).ToList()
        End If

        ' 按m/z排序
        peaks = peaks.OrderBy(Function(p) p.mz).ToList()

        Return peaks
    End Function

    ' ========================================================
    ' 强度归一化
    ' ========================================================
    Private Function NormalizeIntensities(peaks As List(Of ms2), spectrum As PeakMs2, options As GraphBuildOptions) As Double()
        If peaks.Count = 0 Then
            Return New Double(-1) {}
        End If

        Dim normalized(peaks.Count - 1) As Double

        Select Case options.IntensityNormalization.ToLower()
            Case "base_peak"
                Dim basePeak = If(spectrum.BasePeakIntensity > 0, spectrum.BasePeakIntensity, 1.0)
                For i = 0 To peaks.Count - 1
                    normalized(i) = peaks(i).intensity / basePeak
                Next

            Case "total_ion"
                Dim totalIon = peaks.Sum(Function(p) p.intensity)
                If totalIon = 0 Then totalIon = 1.0
                For i = 0 To peaks.Count - 1
                    normalized(i) = peaks(i).intensity / totalIon
                Next

            Case Else
                For i = 0 To peaks.Count - 1
                    normalized(i) = peaks(i).intensity
                Next
        End Select

        ' 对数变换
        If options.LogIntensity Then
            For i = 0 To peaks.Count - 1
                normalized(i) = std.Log(normalized(i) + 1.0)
            Next
        End If

        Return normalized
    End Function

    ' ========================================================
    ' 查找最佳特征离子匹配
    ' ========================================================
    Private Function FindBestFeatureIonMatch(mz As Double, tolerance As Double) As KeyValuePair(Of String, Double)?
        Dim bestName As String = Nothing
        Dim bestMass As Double = 0
        Dim bestError As Double = Double.MaxValue

        For Each kvp In KnownConstants.FeatureIons
            Dim err = std.Abs(kvp.Value - mz)
            If err <= tolerance AndAlso err < bestError Then
                bestError = err
                bestName = kvp.Key
                bestMass = kvp.Value
            End If
        Next

        If bestName IsNot Nothing Then
            Return New KeyValuePair(Of String, Double)(bestName, bestMass)
        End If
        Return Nothing
    End Function

    ' ========================================================
    ' 构建母离子-碎片边 (基于中性丢失)
    ' ========================================================
    Private Sub BuildParentFragmentEdges(graph As GraphData, spectrum As PeakMs2,
                                         options As GraphBuildOptions, edgeTypeList As List(Of String))
        Dim parentNode = graph.Nodes(0)
        Dim edgeTypeListRef = edgeTypeList

        For i = 1 To graph.Nodes.Count - 1
            Dim fragNode = graph.Nodes(i)
            ' 中性丢失 = 母离子m/z - 碎片m/z
            Dim neutralLoss = spectrum.mz - fragNode.Mz

            ' 只考虑正的中性丢失 (碎片应该比母离子轻)
            If neutralLoss <= 0 Then Continue For

            ' 在中性丢失哈希表中查找匹配
            If options.DetectNeutralLossEdges Then
                Dim matches = KnownConstants.FindMassMatches(KnownConstants.NeutralLosses, neutralLoss, options.MassTolerance)
                If matches.Count > 0 Then
                    Dim bestMatch = matches(0)  ' 取偏差最小的
                    Dim typeName = "NL_" & bestMatch.Name
                    Dim typeIdx = edgeTypeListRef.IndexOf(typeName)

                    Dim edge As New GraphEdge()
                    edge.Source = parentNode.Id
                    edge.Target = fragNode.Id
                    edge.EdgeTypeName = typeName
                    edge.EdgeTypeIndex = typeIdx
                    edge.MassDiff = std.Abs(neutralLoss)
                    edge.SignedMassDiff = neutralLoss
                    edge.MassError = bestMatch.Error
                    edge.IntensityRatio = ComputeLogIntensityRatio(parentNode.NormalizedIntensity, fragNode.NormalizedIntensity)
                    graph.Edges.Add(edge)
                End If
            End If
        Next
    End Sub

    ' ========================================================
    ' 构建碎片-碎片边 (基于中性丢失、加合物、同位素)
    ' ========================================================
    Private Sub BuildFragmentFragmentEdges(graph As GraphData, options As GraphBuildOptions,
                                           edgeTypeList As List(Of String))
        Dim edgeTypeListRef = edgeTypeList
        Dim numFragments = graph.Nodes.Count - 1  ' 排除母离子

        ' 遍历所有碎片对
        For i = 1 To graph.Nodes.Count - 1
            For j = i + 1 To graph.Nodes.Count - 1
                Dim nodeA = graph.Nodes(i)
                Dim nodeB = graph.Nodes(j)
                Dim massDiff = nodeA.Mz - nodeB.Mz
                Dim absMassDiff = std.Abs(massDiff)

                ' 跳过质量差为0的情况
                If absMassDiff < 0.001 Then Continue For

                Dim matchedEdges As New List(Of GraphEdge)()

                ' 检查中性丢失匹配
                If options.DetectNeutralLossEdges Then
                    Dim nlMatches = KnownConstants.FindMassMatches(KnownConstants.NeutralLosses, absMassDiff, options.MassTolerance)
                    For Each m In nlMatches
                        Dim typeName = "NL_" & m.Name
                        Dim typeIdx = edgeTypeListRef.IndexOf(typeName)
                        Dim edge As New GraphEdge()
                        edge.Source = nodeA.Id
                        edge.Target = nodeB.Id
                        edge.EdgeTypeName = typeName
                        edge.EdgeTypeIndex = typeIdx
                        edge.MassDiff = absMassDiff
                        edge.SignedMassDiff = massDiff
                        edge.MassError = m.Error
                        edge.IntensityRatio = ComputeLogIntensityRatio(nodeA.NormalizedIntensity, nodeB.NormalizedIntensity)
                        matchedEdges.Add(edge)
                    Next
                End If

                ' 检查加合物匹配
                If options.DetectAdductEdges Then
                    Dim adMatches = KnownConstants.FindMassMatches(KnownConstants.AdductDifferences, absMassDiff, options.MassTolerance)
                    For Each m In adMatches
                        Dim typeName = "AD_" & m.Name
                        Dim typeIdx = edgeTypeListRef.IndexOf(typeName)
                        Dim edge As New GraphEdge()
                        edge.Source = nodeA.Id
                        edge.Target = nodeB.Id
                        edge.EdgeTypeName = typeName
                        edge.EdgeTypeIndex = typeIdx
                        edge.MassDiff = absMassDiff
                        edge.SignedMassDiff = massDiff
                        edge.MassError = m.Error
                        edge.IntensityRatio = ComputeLogIntensityRatio(nodeA.NormalizedIntensity, nodeB.NormalizedIntensity)
                        matchedEdges.Add(edge)
                    Next
                End If

                ' 检查同位素匹配
                If options.DetectIsotopeEdges Then
                    Dim isoType = KnownConstants.FindIsotopeMatch(absMassDiff, options.MassTolerance)
                    If isoType IsNot Nothing Then
                        Dim typeIdx = edgeTypeListRef.IndexOf(isoType)
                        Dim edge As New GraphEdge()
                        edge.Source = nodeA.Id
                        edge.Target = nodeB.Id
                        edge.EdgeTypeName = isoType
                        edge.EdgeTypeIndex = typeIdx
                        edge.MassDiff = absMassDiff
                        edge.SignedMassDiff = massDiff
                        edge.MassError = std.Abs(absMassDiff - GetIsotopeMass(isoType))
                        edge.IntensityRatio = ComputeLogIntensityRatio(nodeA.NormalizedIntensity, nodeB.NormalizedIntensity)
                        matchedEdges.Add(edge)
                    End If
                End If

                ' 只保留偏差最小的那条边 (避免同一对节点产生多条边)
                If matchedEdges.Count > 0 Then
                    matchedEdges.Sort(Function(a, b) a.MassError.CompareTo(b.MassError))
                    graph.Edges.Add(matchedEdges(0))
                End If
            Next
        Next
    End Sub

    ' ========================================================
    ' 获取同位素质量
    ' ========================================================
    Private Function GetIsotopeMass(isoType As String) As Double
        Select Case isoType
            Case "ISO_C13" : Return KnownConstants.C13IsotopeMass
            Case "ISO_C13_2x" : Return 2 * KnownConstants.C13IsotopeMass
            Case "ISO_N15" : Return 0.997035
            Case "ISO_S34" : Return 1.995796
            Case Else : Return 0
        End Select
    End Function

    ' ========================================================
    ' 计算log强度比
    ' ========================================================
    Private Function ComputeLogIntensityRatio(intensityA As Double, intensityB As Double) As Double
        ' 避免log(0)
        Dim a = If(intensityA > 0, intensityA, 0.0000000001)
        Dim b = If(intensityB > 0, intensityB, 0.0000000001)
        Return std.Log(a / b)
    End Function

    ' ========================================================
    ' 限制每个节点的边数 (K近邻)
    ' ========================================================
    Private Sub LimitEdgesPerNode(graph As GraphData, maxEdges As Integer)
        ' 按源节点分组
        Dim edgesBySource = New Dictionary(Of Integer, List(Of GraphEdge))()
        For Each edge In graph.Edges
            If Not edgesBySource.ContainsKey(edge.Source) Then
                edgesBySource(edge.Source) = New List(Of GraphEdge)()
            End If
            edgesBySource(edge.Source).Add(edge)
        Next

        ' 对每个节点的边按质量误差排序，保留前K条
        Dim keptEdges As New List(Of GraphEdge)()
        For Each kvp In edgesBySource
            Dim edges = kvp.Value
            edges.Sort(Function(a, b) a.MassError.CompareTo(b.MassError))
            Dim take = std.Min(maxEdges, edges.Count)
            For i = 0 To take - 1
                keptEdges.Add(edges(i))
            Next
        Next

        graph.Edges = keptEdges
    End Sub

    ' ========================================================
    ' 构建无向图 (添加反向边)
    ' ========================================================
    Private Sub MakeUndirected(graph As GraphData, edgeTypeList As List(Of String))
        Dim reverseEdges As New List(Of GraphEdge)()
        For Each edge In graph.Edges
            Dim reverse As New GraphEdge()
            reverse.Source = edge.Target
            reverse.Target = edge.Source
            reverse.EdgeTypeName = edge.EdgeTypeName
            reverse.EdgeTypeIndex = edge.EdgeTypeIndex
            reverse.MassDiff = edge.MassDiff
            reverse.SignedMassDiff = -edge.SignedMassDiff
            reverse.MassError = edge.MassError
            reverse.IntensityRatio = -edge.IntensityRatio  ' 反向的强度比取负
            reverseEdges.Add(reverse)
        Next
        graph.Edges.AddRange(reverseEdges)
    End Sub

End Module
