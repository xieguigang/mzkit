' ============================================================================
' Exporters.vb
' ============================================================================
' 图数据导出器：JSON格式和PyG格式
' ============================================================================

Imports System.IO
Imports System.Text

Public Module Exporters

    ' ========================================================
    ' JSON导出
    ' ========================================================

    ''' <summary>
    ''' 将图数据导出为JSON文件
    ''' </summary>
    Public Sub ExportJSON(graph As GraphData, outputPath As String)
        Dim sb As New StringBuilder()
        sb.Append("{")
        sb.Append(vbLf)

        ' 谱图标题
        sb.Append("  ""title"": ").Append(JsonEscape(graph.SpectrumTitle)).Append(",").Append(vbLf)

        ' 元数据
        sb.Append("  ""metadata"": {")
        Dim firstMeta = True
        For Each kvp In graph.Metadata
            If Not firstMeta Then sb.Append(",")
            firstMeta = False
            sb.Append(vbLf).Append("    """).Append(JsonEscape(kvp.Key)).Append(""": ")
            sb.Append(JsonValue(kvp.Value))
        Next
        sb.Append(vbLf).Append("  },").Append(vbLf)

        ' 节点
        sb.Append("  ""nodes"": [")
        For i = 0 To graph.Nodes.Count - 1
            Dim node = graph.Nodes(i)
            If i > 0 Then sb.Append(",")
            sb.Append(vbLf)
            sb.Append("    {")
            sb.Append("""id"": ").Append(node.Id).Append(", ")
            sb.Append("""type"": """).Append(node.Type.ToString()).Append(""", ")
            sb.Append("""mz"": ").Append(node.Mz.ToString("F6")).Append(", ")
            sb.Append("""intensity"": ").Append(node.Intensity.ToString("F4")).Append(", ")
            sb.Append("""normalized_intensity"": ").Append(node.NormalizedIntensity.ToString("F6"))
            If node.FeatureIonName IsNot Nothing Then
                sb.Append(", ""feature_ion"": ").Append(JsonEscape(node.FeatureIonName))
            Else
                sb.Append(", ""feature_ion"": null")
            End If
            sb.Append(", ""feature_ion_index"": ").Append(node.FeatureIonIndex)

            ' 节点特征向量
            Dim features = node.GetFeatureVector(KnownConstants.FeatureIonCategories.Count)
            sb.Append(", ""features"": [")
            For j = 0 To features.Length - 1
                If j > 0 Then sb.Append(", ")
                sb.Append(features(j).ToString("F6"))
            Next
            sb.Append("]")
            sb.Append("}")
        Next
        sb.Append(vbLf).Append("  ],").Append(vbLf)

        ' 边
        sb.Append("  ""edges"": [")
        For i = 0 To graph.Edges.Count - 1
            Dim edge = graph.Edges(i)
            If i > 0 Then sb.Append(",")
            sb.Append(vbLf)
            sb.Append("    {")
            sb.Append("""source"": ").Append(edge.Source).Append(", ")
            sb.Append("""target"": ").Append(edge.Target).Append(", ")
            sb.Append("""edge_type"": ").Append(JsonEscape(edge.EdgeTypeName)).Append(", ")
            sb.Append("""edge_type_index"": ").Append(edge.EdgeTypeIndex).Append(", ")
            sb.Append("""mass_diff"": ").Append(edge.MassDiff.ToString("F6")).Append(", ")
            sb.Append("""signed_mass_diff"": ").Append(edge.SignedMassDiff.ToString("F6")).Append(", ")
            sb.Append("""mass_error"": ").Append(edge.MassError.ToString("F6")).Append(", ")
            sb.Append("""intensity_ratio"": ").Append(edge.IntensityRatio.ToString("F6"))

            ' 边特征向量
            Dim features = edge.GetFeatureVector(KnownConstants.EdgeTypeCategories.Count)
            sb.Append(", ""features"": [")
            For j = 0 To features.Length - 1
                If j > 0 Then sb.Append(", ")
                sb.Append(features(j).ToString("F6"))
            Next
            sb.Append("]")
            sb.Append("}")
        Next
        sb.Append(vbLf).Append("  ]").Append(vbLf)

        sb.Append("}")

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 将所有图数据导出为单个JSON文件
    ''' </summary>
    Public Sub ExportAllJSON(graphs As List(Of GraphData), outputPath As String)
        Dim sb As New StringBuilder()
        sb.Append("{").Append(vbLf)
        sb.Append("  ""num_graphs"": ").Append(graphs.Count).Append(",").Append(vbLf)

        ' 特征维度信息
        Dim numNodeFeats = If(graphs.Count > 0, graphs(0).NumNodeFeatures, 0)
        Dim numEdgeFeats = If(graphs.Count > 0, graphs(0).NumEdgeFeatures, 0)
        sb.Append("  ""feature_info"": {").Append(vbLf)
        sb.Append("    ""num_node_features"": ").Append(numNodeFeats).Append(",").Append(vbLf)
        sb.Append("    ""num_edge_features"": ").Append(numEdgeFeats).Append(",").Append(vbLf)

        ' 特征离子类别
        sb.Append("    ""feature_ion_categories"": [")
        Dim fiCats = KnownConstants.FeatureIonCategories
        For i = 0 To fiCats.Count - 1
            If i > 0 Then sb.Append(", ")
            sb.Append("{""name"": ").Append(JsonEscape(fiCats(i).Key)).Append(", ""mz"": ").Append(fiCats(i).Value.ToString("F6")).Append("}")
        Next
        sb.Append("],").Append(vbLf)

        ' 边类型类别
        sb.Append("    ""edge_type_categories"": [")
        Dim etCats = KnownConstants.EdgeTypeCategories
        For i = 0 To etCats.Count - 1
            If i > 0 Then sb.Append(", ")
            sb.Append(JsonEscape(etCats(i)))
        Next
        sb.Append("]").Append(vbLf)
        sb.Append("  },").Append(vbLf)

        ' 图列表
        sb.Append("  ""graphs"": [")
        For i = 0 To graphs.Count - 1
            If i > 0 Then sb.Append(",")
            sb.Append(vbLf)
            ' 嵌入单个图的JSON (简化版本)
            sb.Append("    {")
            sb.Append("""title"": ").Append(JsonEscape(graphs(i).SpectrumTitle)).Append(", ")
            sb.Append("""num_nodes"": ").Append(graphs(i).NumNodes).Append(", ")
            sb.Append("""num_edges"": ").Append(graphs(i).NumEdges)
            sb.Append("}")
        Next
        sb.Append(vbLf).Append("  ]").Append(vbLf)
        sb.Append("}")

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8)
    End Sub

    ' ========================================================
    ' PyG格式导出
    ' ========================================================
    ' PyG (PyTorch Geometric) 使用 torch.save 保存 Data 对象为 .pt 文件
    ' 由于VB.NET无法直接生成PyTorch pickle文件，
    ' 我们输出以下文件供Python脚本转换：
    '   - node_features.txt: 节点特征矩阵 (CSV)
    '   - edge_index.txt: 边索引矩阵 (CSV, 2列: source, target)
    '   - edge_attr.txt: 边特征矩阵 (CSV)
    '   - graph_info.json: 图元数据信息

    ''' <summary>
    ''' 将图数据导出为PyG兼容格式 (文本文件 + JSON元数据)
    ''' </summary>
    Public Sub ExportPyG(graph As GraphData, outputDir As String)
        Directory.CreateDirectory(outputDir)

        ' 1. 节点特征矩阵
        Dim nodeFeatures = graph.GetNodeFeatureMatrix()
        Using writer As New StreamWriter(Path.Combine(outputDir, "node_features.txt"))
            For i = 0 To graph.NumNodes - 1
                Dim parts As New List(Of String)()
                For j = 0 To graph.NumNodeFeatures - 1
                    parts.Add(nodeFeatures(i, j).ToString("F6"))
                Next
                writer.WriteLine(String.Join(",", parts))
            Next
        End Using

        ' 2. 边索引矩阵 (2列: source, target)
        Using writer As New StreamWriter(Path.Combine(outputDir, "edge_index.txt"))
            For Each edge In graph.Edges
                writer.WriteLine($"{edge.Source},{edge.Target}")
            Next
        End Using

        ' 3. 边特征矩阵
        Dim edgeFeatures = graph.GetEdgeFeatureMatrix()
        Using writer As New StreamWriter(Path.Combine(outputDir, "edge_attr.txt"))
            For i = 0 To graph.NumEdges - 1
                Dim parts As New List(Of String)()
                For j = 0 To graph.NumEdgeFeatures - 1
                    parts.Add(edgeFeatures(i, j).ToString("F6"))
                Next
                writer.WriteLine(String.Join(",", parts))
            Next
        End Using

        ' 4. 图元数据信息 (JSON)
        Dim sb As New StringBuilder()
        sb.Append("{").Append(vbLf)
        sb.Append("  ""title"": ").Append(JsonEscape(graph.SpectrumTitle)).Append(",").Append(vbLf)
        sb.Append("  ""num_nodes"": ").Append(graph.NumNodes).Append(",").Append(vbLf)
        sb.Append("  ""num_edges"": ").Append(graph.NumEdges).Append(",").Append(vbLf)
        sb.Append("  ""num_node_features"": ").Append(graph.NumNodeFeatures).Append(",").Append(vbLf)
        sb.Append("  ""num_edge_features"": ").Append(graph.NumEdgeFeatures).Append(",").Append(vbLf)

        ' 特征离子类别
        sb.Append("  ""feature_ion_categories"": [")
        Dim fiCats = KnownConstants.FeatureIonCategories
        For i = 0 To fiCats.Count - 1
            If i > 0 Then sb.Append(", ")
            sb.Append("{""name"": ").Append(JsonEscape(fiCats(i).Key)).Append(", ""mz"": ").Append(fiCats(i).Value.ToString("F6")).Append("}")
        Next
        sb.Append("],").Append(vbLf)

        ' 边类型类别
        sb.Append("  ""edge_type_categories"": [")
        Dim etCats = KnownConstants.EdgeTypeCategories
        For i = 0 To etCats.Count - 1
            If i > 0 Then sb.Append(", ")
            sb.Append(JsonEscape(etCats(i)))
        Next
        sb.Append("]").Append(vbLf)
        sb.Append("}")

        File.WriteAllText(Path.Combine(outputDir, "graph_info.json"), sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 导出所有图的PyG格式到指定目录
    ''' 每个图一个子目录: graph_0/, graph_1/, ...
    ''' </summary>
    Public Sub ExportAllPyG(graphs As List(Of GraphData), outputDir As String)
        Directory.CreateDirectory(outputDir)

        For i = 0 To graphs.Count - 1
            Dim graphDir = Path.Combine(outputDir, $"graph_{i}")
            ExportPyG(graphs(i), graphDir)
        Next

        ' 导出汇总信息
        Dim sb As New StringBuilder()
        sb.Append("{").Append(vbLf)
        sb.Append("  ""num_graphs"": ").Append(graphs.Count).Append(",").Append(vbLf)
        Dim numNodeFeats = If(graphs.Count > 0, graphs(0).NumNodeFeatures, 0)
        Dim numEdgeFeats = If(graphs.Count > 0, graphs(0).NumEdgeFeatures, 0)
        sb.Append("  ""num_node_features"": ").Append(numNodeFeats).Append(",").Append(vbLf)
        sb.Append("  ""num_edge_features"": ").Append(numEdgeFeats).Append(",").Append(vbLf)
        sb.Append("  ""graphs"": [")
        For i = 0 To graphs.Count - 1
            If i > 0 Then sb.Append(",")
            sb.Append(vbLf)
            sb.Append("    {""index"": ").Append(i).Append(", ")
            sb.Append("""title"": ").Append(JsonEscape(graphs(i).SpectrumTitle)).Append(", ")
            sb.Append("""num_nodes"": ").Append(graphs(i).NumNodes).Append(", ")
            sb.Append("""num_edges"": ").Append(graphs(i).NumEdges).Append("}")
        Next
        sb.Append(vbLf).Append("  ]").Append(vbLf)
        sb.Append("}")

        File.WriteAllText(Path.Combine(outputDir, "dataset_info.json"), sb.ToString(), Encoding.UTF8)
    End Sub

    ' ========================================================
    ' JSON辅助函数
    ' ========================================================

    Private Function JsonEscape(s As String) As String
        If s Is Nothing Then Return "null"
        Dim sb As New StringBuilder()
        For Each c In s
            Select Case c
                Case "\"c : sb.Append("\\")
                Case """"c : sb.Append("\""")
                Case vbLf : sb.Append("\n")
                Case vbCr : sb.Append("\r")
                Case vbTab : sb.Append("\t")
                Case Else
                    If c < " "c Then
                        sb.Append("\u").Append(AscW(c).ToString("x4"))
                    Else
                        sb.Append(c)
                    End If
            End Select
        Next
        Return """" & sb.ToString() & """"
    End Function

    Private Function JsonValue(o As Object) As String
        If o Is Nothing Then Return "null"
        If TypeOf o Is String Then Return JsonEscape(CStr(o))
        If TypeOf o Is Integer Then Return CStr(CInt(o))
        If TypeOf o Is Long Then Return CStr(CLng(o))
        If TypeOf o Is Double Then Return CDbl(o).ToString("F6")
        If TypeOf o Is Single Then Return CSng(o).ToString("F6")
        If TypeOf o Is Boolean Then Return If(CBool(o), "true", "false")
        Return JsonEscape(o.ToString())
    End Function

End Module
