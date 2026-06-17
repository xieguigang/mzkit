

' ============================================================================
' 使用示例 (取消注释后可直接运行测试)
' ----------------------------------------------------------------------------
'
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.MSEngine.Mummichog

Module MummichogExample

    Sub Main()
        ' 1. 准备KEGG数据库 (使用示例数据, 实际应用中从文件加载)
        Dim metabolites = CreateSampleMetabolites()
        Dim pathways = CreateSamplePathways()

        ' 2. 配置算法参数
        Dim params As New MummichogParams With {
            .PpmTolerance = 10.0,          ' ppm容忍度
            .PValueCutoff = 0.05,          ' 差异峰p值阈值
            .FdrCutoff = 0.2,              ' 通路FDR阈值
            .Mode = IonModes.Positive, ' 正离子模式
            .MaxCandidatesPerPeak = 3      ' 每峰最多3个候选
        }

        ' 3. 创建注释器
        Dim annotator As New MummichogAnnotator(metabolites, pathways, params)

        ' 4. 准备一级质谱峰表数据 (xcms2数组)
        Dim peaks As New List(Of xcms2)
        ' ... 从XCMS结果填充peaks ...

        ' 5. 方式A: 提供预计算的p值
        Dim pValues As New Dictionary(Of String, Double)
        ' pValues("peak_id_1") = 0.001
        ' pValues("peak_id_2") = 0.045
        ' ...
        Dim results = annotator.Annotate(peaks, pValues)

        ' 5. 方式B: 自动从样本分组计算p值
        ' Dim controlSamples = {"sample1", "sample2", "sample3"}
        ' Dim treatmentSamples = {"sample4", "sample5", "sample6"}
        ' Dim results = annotator.Annotate(peaks, controlSamples, treatmentSamples)

        ' 6. 输出注释结果
        Dim dt = MummichogAnnotator.ResultsToDataTable(results)
        Console.WriteLine($"共 {results.Count} 条注释结果")
        For Each r In results.Take(10)
            Console.WriteLine(r.ToString())
        Next

        ' 7. 输出通路富集结果
        Console.WriteLine(vbCrLf & "显著富集通路:")
        For Each pr In annotator.PathwayResults.Where(Function(p) p.IsSignificant)
            Console.WriteLine($"  {pr.Pathway.Name}: hits={pr.SignificantHits}, p={pr.PValue:G4}, fdr={pr.FDR:G4}")
        Next
    End Sub

    ''' <summary>
    ''' 创建示例KEGG代谢物数据 (用于测试)
    ''' </summary>
    Public Function CreateSampleMetabolites() As List(Of KEGGMetabolite)
        Dim list As New List(Of KEGGMetabolite)()

        ' 糖酵解通路代谢物
        list.Add(New KEGGMetabolite With {.Id = "C00022", .CommonName = "Pyruvate", .Formula = "C3H4O3", .Pathways = New HashSet(Of String) From {"map00010", "map00020", "map00620"}})
        list.Add(New KEGGMetabolite With {.Id = "C00031", .CommonName = "Glucose", .Formula = "C6H12O6", .Pathways = New HashSet(Of String) From {"map00010", "map00052"}})
        list.Add(New KEGGMetabolite With {.Id = "C00036", .CommonName = "Oxaloacetate", .Formula = "C4H2O5", .Pathways = New HashSet(Of String) From {"map00010", "map00020"}})
        list.Add(New KEGGMetabolite With {.Id = "C00074", .CommonName = "Phosphoenolpyruvate", .Formula = "C3H2O6P", .Pathways = New HashSet(Of String) From {"map00010", "map00260"}})
        list.Add(New KEGGMetabolite With {.Id = "C00118", .CommonName = "Glycerol-3-phosphate", .Formula = "C3H7O6P", .Pathways = New HashSet(Of String) From {"map00010", "map00561"}})

        ' TCA循环代谢物
        list.Add(New KEGGMetabolite With {.Id = "C00024", .CommonName = "Succinyl-CoA", .Formula = "C25H35N7O19P3S", .Pathways = New HashSet(Of String) From {"map00020", "map00640"}})
        list.Add(New KEGGMetabolite With {.Id = "C00149", .CommonName = "Malate", .Formula = "C4H4O5", .Pathways = New HashSet(Of String) From {"map00020", "map00630"}})
        list.Add(New KEGGMetabolite With {.Id = "C00158", .CommonName = "Citrate", .Formula = "C6H8O7", .Pathways = New HashSet(Of String) From {"map00020", "map00220"}})
        list.Add(New KEGGMetabolite With {.Id = "C00311", .CommonName = "Isocitrate", .Formula = "C6H8O7", .Pathways = New HashSet(Of String) From {"map00020"}})
        list.Add(New KEGGMetabolite With {.Id = "C00042", .CommonName = "Succinate", .Formula = "C4H6O4", .Pathways = New HashSet(Of String) From {"map00020", "map00640"}})
        list.Add(New KEGGMetabolite With {.Id = "C00026", .CommonName = "2-Oxoglutarate", .Formula = "C5H4O5", .Pathways = New HashSet(Of String) From {"map00020", "map00250"}})
        list.Add(New KEGGMetabolite With {.Id = "C00122", .CommonName = "Fumarate", .Formula = "C4H2O4", .Pathways = New HashSet(Of String) From {"map00020", "map00640"}})

        ' 氨基酸代谢
        list.Add(New KEGGMetabolite With {.Id = "C00037", .CommonName = "Glycine", .Formula = "C2H5NO2", .Pathways = New HashSet(Of String) From {"map00260", "map00330"}})
        list.Add(New KEGGMetabolite With {.Id = "C00041", .CommonName = "Alanine", .Formula = "C3H7NO2", .Pathways = New HashSet(Of String) From {"map00250", "map00470"}})
        list.Add(New KEGGMetabolite With {.Id = "C00049", .CommonName = "L-Aspartate", .Formula = "C4H7NO4", .Pathways = New HashSet(Of String) From {"map00250", "map00270"}})
        list.Add(New KEGGMetabolite With {.Id = "C00062", .CommonName = "L-Arginine", .Formula = "C6H14N4O2", .Pathways = New HashSet(Of String) From {"map00220", "map00330"}})
        list.Add(New KEGGMetabolite With {.Id = "C00073", .CommonName = "L-Methionine", .Formula = "C5H11NO2S", .Pathways = New HashSet(Of String) From {"map00270", "map00410"}})
        list.Add(New KEGGMetabolite With {.Id = "C00079", .CommonName = "L-Phenylalanine", .Formula = "C9H11NO2", .Pathways = New HashSet(Of String) From {"map00360", "map00400"}})
        list.Add(New KEGGMetabolite With {.Id = "C00135", .CommonName = "L-Proline", .Formula = "C5H9NO2", .Pathways = New HashSet(Of String) From {"map00330", "map00410"}})
        list.Add(New KEGGMetabolite With {.Id = "C00152", .CommonName = "L-Asparagine", .Formula = "C4H8N2O3", .Pathways = New HashSet(Of String) From {"map00250", "map00270"}})

        ' 脂质代谢
        list.Add(New KEGGMetabolite With {.Id = "C00162", .CommonName = "Palmitic acid", .Formula = "C16H32O2", .Pathways = New HashSet(Of String) From {"map00061", "map01040"}})
        list.Add(New KEGGMetabolite With {.Id = "C00226", .CommonName = "Oleic acid", .Formula = "C18H34O2", .Pathways = New HashSet(Of String) From {"map01040"}})
        list.Add(New KEGGMetabolite With {.Id = "C00422", .CommonName = "Arachidonic acid", .Formula = "C20H32O2", .Pathways = New HashSet(Of String) From {"map00590", "map01040"}})

        ' 核苷酸代谢
        list.Add(New KEGGMetabolite With {.Id = "C00002", .CommonName = "ATP", .Formula = "C10H12N5O13P3", .Pathways = New HashSet(Of String) From {"map00010", "map00190", "map00230"}})
        list.Add(New KEGGMetabolite With {.Id = "C00003", .CommonName = "NAD+", .Formula = "C21H26N7O14P2", .Pathways = New HashSet(Of String) From {"map00010", "map00190"}})
        list.Add(New KEGGMetabolite With {.Id = "C00004", .CommonName = "NADH", .Formula = "C21H27N7O14P2", .Pathways = New HashSet(Of String) From {"map00010", "map00190"}})
        list.Add(New KEGGMetabolite With {.Id = "C00186", .CommonName = "AMP", .Formula = "C10H12N5O7P", .Pathways = New HashSet(Of String) From {"map00230", "map00190"}})

        ' 计算精确分子量
        For Each m In list
            m.RecalculateMass()
        Next

        Return list
    End Function

    ''' <summary>
    ''' 创建示例KEGG通路数据 (用于测试)
    ''' </summary>
    Public Function CreateSamplePathways() As List(Of KEGGPathway)
        Dim list As New List(Of KEGGPathway)()

        list.Add(New KEGGPathway With {.ID = "map00010", .Name = "Glycolysis / Gluconeogenesis",
            .Metabolites = New HashSet(Of String) From {"C00022", "C00031", "C00036", "C00074", "C00118", "C00002", "C00003", "C00004"}})
        list.Add(New KEGGPathway With {.ID = "map00020", .Name = "TCA cycle",
            .Metabolites = New HashSet(Of String) From {"C00022", "C00024", "C00036", "C00149", "C00158", "C00311", "C00042", "C00026", "C00122", "C00003", "C00004"}})
        list.Add(New KEGGPathway With {.ID = "map00260", .Name = "Glycine, serine and threonine metabolism",
            .Metabolites = New HashSet(Of String) From {"C00037", "C00074", "C00152"}})
        list.Add(New KEGGPathway With {.ID = "map00250", .Name = "Alanine, aspartate and glutamate metabolism",
            .Metabolites = New HashSet(Of String) From {"C00041", "C00049", "C00026", "C00152"}})
        list.Add(New KEGGPathway With {.ID = "map00270", .Name = "Cysteine and methionine metabolism",
            .Metabolites = New HashSet(Of String) From {"C00073", "C00049", "C00152"}})
        list.Add(New KEGGPathway With {.ID = "map00330", .Name = "Arginine and proline metabolism",
            .Metabolites = New HashSet(Of String) From {"C00062", "C00037", "C00135"}})
        list.Add(New KEGGPathway With {.ID = "map01040", .Name = "Biosynthesis of unsaturated fatty acids",
            .Metabolites = New HashSet(Of String) From {"C00162", "C00226", "C00422"}})
        list.Add(New KEGGPathway With {.ID = "map00190", .Name = "Oxidative phosphorylation",
            .Metabolites = New HashSet(Of String) From {"C00002", "C00003", "C00004", "C00186"}})
        list.Add(New KEGGPathway With {.ID = "map00230", .Name = "Purine metabolism",
            .Metabolites = New HashSet(Of String) From {"C00002", "C00186"}})

        Return list
    End Function
End Module
