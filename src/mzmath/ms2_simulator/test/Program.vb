' ============================================================================
' Program.vb
' ============================================================================
' 主程序入口
' 用法: SpectrumToGraph <input.mgf> <output_dir> [options]
' ============================================================================

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon

Module Program

    Sub Main(args As String())
        Console.WriteLine("="c, 70)
        Console.WriteLine("MS/MS Spectrum to GNN Graph Data Converter")
        Console.WriteLine("MS/MS 谱图 -> GNN 网络图数据 转换工具")
        Console.WriteLine("="c, 70)
        Console.WriteLine()

        ' 解析命令行参数
        If args.Length < 2 Then
            PrintUsage()
            Return
        End If

        Dim inputPath = args(0)
        Dim outputDir = args(1)

        ' 默认选项
        Dim options As New GraphBuilder.GraphBuildOptions()

        ' 解析可选参数
        For i = 2 To args.Length - 1
            Dim arg = args(i)
            If arg.StartsWith("--") Then
                Dim parts = arg.Substring(2).Split({"="c}, 2)
                Dim key = parts(0).ToLower()
                Dim value = If(parts.Length > 1, parts(1), "")

                Select Case key
                    Case "tolerance"
                        Double.TryParse(value, options.MassTolerance)
                    Case "max-edges"
                        Integer.TryParse(value, options.MaxEdgesPerNode)
                    Case "max-peaks"
                        Integer.TryParse(value, options.MaxPeaks)
                    Case "min-intensity"
                        Double.TryParse(value, options.MinIntensityThreshold)
                    Case "normalization"
                        options.IntensityNormalization = value
                    Case "log-intensity"
                        Boolean.TryParse(value, options.LogIntensity)
                    Case "directed"
                        options.Undirected = Not (String.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                    Case "no-parent-edges"
                        options.BuildParentFragmentEdges = False
                    Case "no-fragment-edges"
                        options.BuildFragmentFragmentEdges = False
                    Case "no-isotope"
                        options.DetectIsotopeEdges = False
                    Case "no-adduct"
                        options.DetectAdductEdges = False
                    Case "no-neutral-loss"
                        options.DetectNeutralLossEdges = False
                    Case "format"
                        ' 输出格式: json, pyg, both (默认both)
                    Case Else
                        Console.WriteLine($"  警告: 未知选项 --{key}")
                End Select
            End If
        Next

        ' 创建输出目录
        Directory.CreateDirectory(outputDir)

        ' 解析MGF文件
        Console.WriteLine($"[1/4] 解析MGF文件: {inputPath}")
        Dim spectra = MgfReader.ReadIons(inputPath).IonPeaks.ToArray
        Console.WriteLine($"      共解析 {spectra.Count} 个谱图")

        If spectra.Count = 0 Then
            Console.WriteLine("错误: MGF文件中未找到任何谱图")
            Return
        End If

        ' 打印构建选项
        Console.WriteLine()
        Console.WriteLine("[2/4] 构建选项:")
        Console.WriteLine($"      质量容差: {options.MassTolerance} Da")
        Console.WriteLine($"      强度归一化: {options.IntensityNormalization}")
        Console.WriteLine($"      对数强度: {options.LogIntensity}")
        Console.WriteLine($"      母离子-碎片边: {options.BuildParentFragmentEdges}")
        Console.WriteLine($"      碎片-碎片边: {options.BuildFragmentFragmentEdges}")
        Console.WriteLine($"      同位素边: {options.DetectIsotopeEdges}")
        Console.WriteLine($"      加合物边: {options.DetectAdductEdges}")
        Console.WriteLine($"      中性丢失边: {options.DetectNeutralLossEdges}")
        Console.WriteLine($"      无向图: {options.Undirected}")
        If options.MaxEdgesPerNode > 0 Then
            Console.WriteLine($"      每节点最大边数: {options.MaxEdgesPerNode}")
        End If
        If options.MaxPeaks > 0 Then
            Console.WriteLine($"      每谱图最大峰数: {options.MaxPeaks}")
        End If
        Console.WriteLine()

        ' 构建图
        Console.WriteLine("[3/4] 构建网络图...")
        Dim graphs As New List(Of GraphData)()
        For i = 0 To spectra.Count - 1
            Dim graph = GraphBuilder.BuildGraph(spectra(i), options)
            graphs.Add(graph)

            If (i + 1) Mod 10 = 0 OrElse i = spectra.Count - 1 Then
                Console.Write(vbCr & $"      进度: {i + 1}/{spectra.Count} 谱图已处理")
            End If
        Next
        Console.WriteLine()
        Console.WriteLine($"      共构建 {graphs.Count} 个网络图")

        ' 统计信息
        Dim totalNodes = 0
        Dim totalEdges = 0
        For Each g In graphs
            totalNodes += g.NumNodes
            totalEdges += g.NumEdges
        Next
        Console.WriteLine($"      总节点数: {totalNodes}")
        Console.WriteLine($"      总边数: {totalEdges}")
        Console.WriteLine($"      平均每图节点数: {totalNodes / graphs.Count:F1}")
        Console.WriteLine($"      平均每图边数: {totalEdges / graphs.Count:F1}")
        Console.WriteLine()

        ' 导出
        Console.WriteLine("[4/4] 导出数据...")
        Dim jsonDir = Path.Combine(outputDir, "json")
        Dim pygDir = Path.Combine(outputDir, "pyg")
        Directory.CreateDirectory(jsonDir)
        Directory.CreateDirectory(pygDir)

        ' 导出每个图的JSON
        For i = 0 To graphs.Count - 1
            Dim jsonPath = Path.Combine(jsonDir, $"graph_{i:D4}.json")
            Exporters.ExportJSON(graphs(i), jsonPath)
        Next
        Console.WriteLine($"      JSON文件已导出: {jsonDir}/ ({graphs.Count} 个文件)")

        ' 导出PyG格式
        Exporters.ExportAllPyG(graphs, pygDir)
        Console.WriteLine($"      PyG格式已导出: {pygDir}/")

        ' 导出汇总信息
        Dim summaryPath = Path.Combine(outputDir, "summary.json")
        Exporters.ExportAllJSON(graphs, summaryPath)
        Console.WriteLine($"      汇总信息: {summaryPath}")

        ' 导出特征离子和边类型参考表
        ExportReferenceTables(outputDir)

        Console.WriteLine()
        Console.WriteLine("="c, 70)
        Console.WriteLine("转换完成!")
        Console.WriteLine($"输出目录: {outputDir}")
        Console.WriteLine()
        Console.WriteLine("下一步: 运行 convert_to_pyg.py 将文本文件转换为PyTorch .pt文件")
        Console.WriteLine("  python convert_to_pyg.py " & pygDir)
        Console.WriteLine("="c, 70)
    End Sub

    ''' <summary>
    ''' 导出特征离子和边类型参考表
    ''' </summary>
    Private Sub ExportReferenceTables(outputDir As String)
        Dim refPath = Path.Combine(outputDir, "reference_tables.json")
        Dim sb As New Text.StringBuilder()
        sb.Append("{").Append(vbLf)

        ' 特征离子表
        sb.Append("  ""feature_ions"": {").Append(vbLf)
        Dim fiCats = KnownConstants.FeatureIonCategories
        For i = 0 To fiCats.Count - 1
            sb.Append("    ").Append(JsonEscapeStr(fiCats(i).Key)).Append(": {")
            sb.Append("""index"": ").Append(i).Append(", ")
            sb.Append("""mz"": ").Append(fiCats(i).Value.ToString("F6"))
            sb.Append("}")
            If i < fiCats.Count - 1 Then sb.Append(",")
            sb.Append(vbLf)
        Next
        sb.Append("  },").Append(vbLf)

        ' 边类型表
        sb.Append("  ""edge_types"": {").Append(vbLf)
        Dim etCats = KnownConstants.EdgeTypeCategories
        For i = 0 To etCats.Count - 1
            sb.Append("    ").Append(JsonEscapeStr(etCats(i))).Append(": ").Append(i)
            If i < etCats.Count - 1 Then sb.Append(",")
            sb.Append(vbLf)
        Next
        sb.Append("  },").Append(vbLf)

        ' 中性丢失表
        sb.Append("  ""neutral_losses"": {").Append(vbLf)
        Dim nlKeys = KnownConstants.NeutralLosses.Keys.ToList()
        For i = 0 To nlKeys.Count - 1
            sb.Append("    ").Append(JsonEscapeStr(nlKeys(i))).Append(": ")
            sb.Append(KnownConstants.NeutralLosses(nlKeys(i)).ToString("F6"))
            If i < nlKeys.Count - 1 Then sb.Append(",")
            sb.Append(vbLf)
        Next
        sb.Append("  },").Append(vbLf)

        ' 加合物表
        sb.Append("  ""adduct_differences"": {").Append(vbLf)
        Dim adKeys = KnownConstants.AdductDifferences.Keys.ToList()
        For i = 0 To adKeys.Count - 1
            sb.Append("    ").Append(JsonEscapeStr(adKeys(i))).Append(": ")
            sb.Append(KnownConstants.AdductDifferences(adKeys(i)).ToString("F6"))
            If i < adKeys.Count - 1 Then sb.Append(",")
            sb.Append(vbLf)
        Next
        sb.Append("  }").Append(vbLf)

        sb.Append("}")

        File.WriteAllText(refPath, sb.ToString(), Text.Encoding.UTF8)
    End Sub

    Private Function JsonEscapeStr(s As String) As String
        Return """" & s.Replace("\", "\\").Replace("""", "\""") & """"
    End Function

    Private Sub PrintUsage()
        Console.WriteLine("用法: SpectrumToGraph <input.mgf> <output_dir> [options]")
        Console.WriteLine()
        Console.WriteLine("选项:")
        Console.WriteLine("  --tolerance=<value>       质量匹配容差 (Da, 默认: 0.02)")
        Console.WriteLine("  --max-edges=<value>       每节点最大边数 (默认: 0=不限制)")
        Console.WriteLine("  --max-peaks=<value>       每谱图最大峰数 (默认: 0=不限制)")
        Console.WriteLine("  --min-intensity=<value>   最小强度阈值 (默认: 0=不过滤)")
        Console.WriteLine("  --normalization=<type>    强度归一化: base_peak|total_ion (默认: base_peak)")
        Console.WriteLine("  --log-intensity=<bool>    是否对强度取对数 (默认: false)")
        Console.WriteLine("  --directed=<bool>         是否构建有向图 (默认: false=无向)")
        Console.WriteLine("  --no-parent-edges         不构建母离子-碎片边")
        Console.WriteLine("  --no-fragment-edges       不构建碎片-碎片边")
        Console.WriteLine("  --no-isotope              不检测同位素边")
        Console.WriteLine("  --no-adduct               不检测加合物边")
        Console.WriteLine("  --no-neutral-loss         不检测中性丢失边")
        Console.WriteLine()
        Console.WriteLine("示例:")
        Console.WriteLine("  SpectrumToGraph data.mgf output/")
        Console.WriteLine("  SpectrumToGraph data.mgf output/ --tolerance=0.01 --max-edges=10")
        Console.WriteLine("  SpectrumToGraph data.mgf output/ --max-peaks=100 --log-intensity=true")
    End Sub

End Module
