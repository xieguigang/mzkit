' ============================================================================
' KnownConstants.vb
' ============================================================================
' 已知特征离子、中性丢失、加合物质量差的哈希表定义
' 这些哈希表用于在网络图构建过程中匹配碎片峰之间的化学关系
' ============================================================================

Imports std = System.Math

Public Module KnownConstants

    ' ========================================================
    ' 质量匹配容差 (Dalton)
    ' ========================================================
    ' 默认 0.02 Da，适用于低分辨率质谱
    ' 高分辨率质谱可设为 0.005 Da
    Public Property MassTolerance As Double = 0.02

    ' 同位素质量差 (C12 -> C13)
    Public ReadOnly C13IsotopeMass As Double = 1.0033548378

    ' ========================================================
    ' 已知特征离子哈希表 (名称 -> m/z)
    ' ========================================================
    ' 这些是MS/MS谱图中常见的诊断离子/特征离子
    ' 用于碎片节点的One-hot特征编码
    Public ReadOnly FeatureIons As New Dictionary(Of String, Double) From {
        {"Phosphocholine", 184.0733},        ' 磷酸胆碱头部 (PC, LPC, SM)
        {"Choline", 104.107},                ' 胆碱
        {"Ethanolamine", 141.0191},          ' 乙醇胺磷酸 (PE)
        {"Phosphoserine", 185.0089},         ' 磷酸丝氨酸
        {"Hexose_H2O", 163.0601},            ' 己糖-H2O (Glc/Gal)
        {"Hexose", 180.0634},                ' 己糖
        {"Pentose", 150.0528},               ' 戊糖
        {"Adenine", 136.0618},               ' 腺嘌呤 (核苷酸)
        {"Guanine", 152.0567},               ' 鸟嘌呤
        {"Cytosine", 112.0505},              ' 胞嘧啶
        {"Thymine", 127.0502},               ' 胸腺嘧啶
        {"Uracil", 113.0345},                ' 尿嘧啶
        {"Adenosine", 268.1046},             ' 腺苷
        {"Guanosine", 284.0995},             ' 鸟苷
        {"Palmitate_FA", 255.2329},          ' 棕榈酸 (16:0) [M-H]-
        {"Stearate_FA", 283.2643},           ' 硬脂酸 (18:0) [M-H]-
        {"Oleate_FA", 281.2486},             ' 油酸 (18:1) [M-H]-
        {"Linoleate_FA", 279.233},           ' 亚油酸 (18:2) [M-H]-
        {"Arachidonate_FA", 303.2319},       ' 花生四烯酸 (20:4)
        {"Inositol", 180.0634},              ' 肌醇
        {"Glycerol", 92.0473},               ' 甘油
        {"Serine", 105.0426},                ' 丝氨酸
        {"Glutamate", 148.0604},             ' 谷氨酸
        {"Phenylalanine_Immonium", 120.0808},' 苯丙氨酸亚胺离子
        {"Tyrosine_Immonium", 136.0757},     ' 酪氨酸亚胺离子
        {"Tryptophan_Immonium", 159.0917},   ' 色氨酸亚胺离子
        {"Proline_Immonium", 70.0651},       ' 脯氨酸亚胺离子
        {"Leu_Ile_Immonium", 86.0964},       ' 亮/异亮氨酸亚胺离子
        {"Lys_Immonium", 101.107},           ' 赖氨酸亚胺离子
        {"Arg_Immonium", 129.1135},          ' 精氨酸亚胺离子
        {"His_Immonium", 110.0713},          ' 组氨酸亚胺离子
        {"Sulfate_SO3", 79.9568},            ' 硫酸基团
        {"Phosphate_PO3", 78.9585},          ' 磷酸基团
        {"Nitro_NO2", 45.9929},              ' 硝基
        {"Acetyl", 43.0184},                 ' 乙酰基
        {"Methyl", 15.0235}                  ' 甲基
    }

    ' ========================================================
    ' 已知中性丢失哈希表 (名称 -> 丢失质量)
    ' ========================================================
    ' 中性丢失是碎片化过程中从母离子或碎片离子上失去的中性分子
    Public ReadOnly NeutralLosses As New Dictionary(Of String, Double) From {
        {"H2O", 18.010565},                  ' 水分子丢失
        {"NH3", 17.026549},                  ' 氨丢失
        {"CO", 27.994915},                   ' 一氧化碳丢失
        {"CO2", 43.98983},                   ' 二氧化碳丢失
        {"CH2O", 30.010565},                 ' 甲醛丢失
        {"C2H4", 28.0313},                   ' 乙烯丢失
        {"C2H2", 26.01565},                  ' 乙炔丢失
        {"HCN", 27.010899},                  ' 氰化氢丢失
        {"HCOOH", 46.005479},                ' 甲酸丢失
        {"CH3OH", 32.026215},                ' 甲醇丢失
        {"C2H5OH", 46.041865},               ' 乙醇丢失
        {"H2S", 33.987721},                  ' 硫化氢丢失
        {"SO3", 79.956815},                  ' 三氧化硫丢失
        {"SO2", 63.961901},                  ' 二氧化硫丢失
        {"H3PO4", 97.976896},                ' 磷酸丢失
        {"HPO3", 79.966331},                 ' 偏磷酸丢失
        {"HCl", 35.976678},                  ' 氯化氢丢失
        {"HF", 19.992329},                   ' 氟化氢丢失
        {"HBr", 79.92616},                   ' 溴化氢丢失
        {"C3H6", 42.04695},                  ' 丙烯丢失
        {"C3H4", 40.0313},                   ' 丙二烯丢失
        {"C4H8", 56.0626},                   ' 丁烯丢失
        {"C5H8", 68.0626},                   ' 戊二烯丢失
        {"C6H6", 78.04695},                  ' 苯丢失
        {"AcOH", 60.02113},                  ' 醋酸丢失
        {"Acetone", 58.04195},               ' 丙酮丢失
        {"Ketene", 42.010565},               ' 乙烯酮丢失
        {"NO", 29.997989},                   ' 一氧化氮丢失
        {"NO2", 45.992904},                  ' 二氧化氮丢失
        {"N2", 28.006148},                   ' 氮气丢失
        {"O2", 31.989829},                   ' 氧气丢失
        {"Hexose", 162.052824},              ' 己糖丢失 (糖苷键断裂)
        {"Pentose", 132.042259},             ' 戊糖丢失
        {"Deoxyhexose", 146.057909},         ' 脱氧己糖丢失
        {"Amino_sugar", 161.068814},         ' 氨基糖丢失
        {"Nucleobase_Adenine", 135.054525},  ' 腺嘌呤碱基丢失
        {"Nucleobase_Guanine", 151.049535},  ' 鸟嘌呤碱基丢失
        {"FattyAcid_Palmitic", 256.24023},   ' 棕榈酸丢失
        {"FattyAcid_Oleic", 282.25588},      ' 油酸丢失
        {"FattyAcid_Stearic", 284.27153},    ' 硬脂酸丢失
        {"FattyAcid_Arachidonic", 304.24023},' 花生四烯酸丢失
        {"Phosphocholine", 184.0733},        ' 磷酸胆碱丢失
        {"Phosphoethanolamine", 141.0191},   ' 磷酸乙醇胺丢失
        {"Phosphoserine", 185.0089},         ' 磷酸丝氨酸丢失
        {"Glycerol", 92.0473},               ' 甘油丢失
        {"Guanidine", 59.0371},              ' 胍丢失
        {"CH2CO", 42.010565},                ' 乙酰基丢失(酮烯)
        {"C2H4O2", 60.02113},                ' 乙酸甲酯形式丢失
        {"C5H10", 70.07825},                 ' 戊烯丢失
        {"C3H6O", 58.04195},                 ' 丙酮/丙烯醛丢失
        {"C4H6O2", 86.03678},                ' 丁内酯相关丢失
        {"C3H4O2", 72.02113},                ' 丙烯酸丢失
        {"C2H4O", 44.026215},                ' 乙醛丢失
        {"C3H8O", 60.05751},                 ' 丙醇丢失
        {"C5H8O2", 100.05243},               ' 戊二烯酸相关丢失
        {"C6H10O5", 162.052824},             ' 己糖酐丢失
        {"C10H12N2O3", 208.0848},            ' 色氨酸侧链相关丢失
        {"C9H9NO2", 163.0633}                ' 酪氨酸侧链相关丢失
    }

    ' ========================================================
    ' 已知加合物质量差哈希表 (名称 -> 质量差)
    ' ========================================================
    ' 这些表示不同加合物形式之间的质量差
    ' 也包括同系物质量差 (如CH2) 和氧化修饰
    Public ReadOnly AdductDifferences As New Dictionary(Of String, Double) From {
        {"H", 1.007276},                     ' 质子加合
        {"Na", 22.989218},                   ' 钠加合
        {"K", 38.963158},                    ' 钾加合
        {"NH4", 18.033826},                  ' 铵加合
        {"Li", 7.015456},                    ' 锂加合
        {"Na_H_diff", 21.981942},            ' Na - H 质量差
        {"K_H_diff", 37.955882},             ' K - H 质量差
        {"NH4_H_diff", 17.02655},            ' NH4 - H 质量差
        {"K_Na_diff", 15.97394},             ' K - Na 质量差
        {"Cl", 34.969402},                   ' 氯加合
        {"Br", 78.918337},                   ' 溴加合
        {"F", 18.998403},                    ' 氟加合
        {"I", 126.904473},                   ' 碘加合
        {"Mg", 23.985042},                   ' 镁加合
        {"Ca", 39.962591},                   ' 钙加合
        {"Fe", 55.934942},                   ' 铁加合
        {"Cu", 62.929598},                   ' 铜加合
        {"Zn", 63.929142},                   ' 锌加合
        {"CH2", 14.01565},                   ' 亚甲基 (同系物系列)
        {"C2H4", 28.0313},                   ' 乙烯基 (同系物)
        {"C3H6", 42.04695},                  ' 丙烯基 (同系物)
        {"C4H8", 56.0626},                   ' 丁烯基 (同系物)
        {"C5H10", 70.07825},                 ' 戊烯基 (同系物)
        {"O", 15.994915},                    ' 氧化 (+O)
        {"O2", 31.989829},                   ' 双氧化 (+O2)
        {"S", 31.972071},                    ' 硫取代/加合
        {"N", 14.003074},                    ' 氮加合
        {"C2H2O", 42.010565},                ' 乙酰化修饰
        {"C2H3NO", 57.02146},                ' 甘氨酰加合
        {"C3H4O2", 72.02113},                ' 丙二酰相关
        {"C2H4O2", 60.02113},                ' 乙酸加合
        {"C6H10O5_Hexose", 162.052824},      ' 糖基化
        {"C6H8O6", 176.0321},                ' 抗坏血酸加合
        {"C5H8O4_Pentose", 132.042259},      ' 戊糖基化
        {"C7H4O4", 136.016},                 ' 没食子酸相关
        {"SO3_mod", 79.956815},              ' 硫酸化修饰
        {"PO3_mod", 78.9585},                ' 磷酸化修饰
        {"H2O_H_loss", -17.003289},          ' +H - H2O (脱水加合)
        {"2H_deuterium", 1.006277},          ' 氘代 (2H - 1H)
        {"C13_isotope", 1.003355},           ' C13同位素位移
        {"2C13_isotope", 2.00671},           ' 双C13同位素位移
        {"15N_isotope", 0.997035},           ' N15同位素位移
        {"34S_isotope", 1.995796},           ' S34同位素位移
        {"37Cl_isotope", 1.99705},           ' Cl37同位素位移
        {"81Br_isotope", 1.99795},           ' Br81同位素位移
        {"D2O_exchange", 2.012551},          ' 氘水交换
        {"H2", 2.01565},                     ' 氢气加合/还原
        {"CO_CH2", 41.99793},                ' CO-CH2 差
        {"C2H2O2", 58.00548},                ' 乙二醛相关
        {"C3H6O2", 74.03678},                ' 丙酸相关
        {"C4H6O3", 102.03169},               ' 乙酰丙酸相关
        {"C5H8O2", 100.05243},               ' 异戊烯相关
        {"C6H6O3", 126.03169},               ' 三羟基苯相关
        {"C7H4O2", 120.0211},                ' 苯甲酰相关
        {"C8H8O2", 136.05243},               ' 苯乙酰相关
        {"C9H8O2", 148.05243},               ' 肉桂酰相关
        {"C6H5NO2", 123.032},                ' 硝基苯相关
        {"C7H5NO3", 151.0269},               ' 硝基苯甲酸相关
        {"C8H9NO", 135.0684},                ' 乙酰苯胺相关
        {"C10H12N2O", 176.095},              ' 色胺相关
        {"C5H4N4", 120.0436},                ' 黄嘌呤相关
        {"C6H4N2O2", 136.0272},              ' 硝基咪唑相关
        {"C4H4N2", 80.0375},                 ' 咪唑相关
        {"C5H5N", 79.0422},                  ' 吡啶相关
        {"C6H6N", 92.0500},                  ' 苯胺相关
        {"C7H7O", 107.0491},                 ' 苯甲氧基相关
        {"C8H9O", 121.0648},                 ' 苯乙氧基相关
        {"C9H9O2", 149.0597},                ' 香豆素相关
        {"C10H10O2", 162.0681},              ' 肉桂酸酯相关
        {"C11H10O", 170.0732},               ' 萘酚相关
        {"C12H10O", 182.0732},               ' 联苯酚相关
        {"C13H10O", 194.0732},               ' 苯基苯酚相关
        {"C14H10O", 206.0732},               ' 蒽醌相关
        {"C15H10O", 218.0732},               ' 芘酮相关
        {"C16H10O", 230.0732}                ' 苯并芘酮相关
    }

    ' ========================================================
    ' 边类型类别列表 (用于One-hot编码)
    ' ========================================================
    ' 合并所有中性丢失和加合物差异作为边类型
    ' 最后加上同位素类型
    Private _edgeTypeCategories As List(Of String) = Nothing

    Public ReadOnly Property EdgeTypeCategories As List(Of String)
        Get
            If _edgeTypeCategories Is Nothing Then
                _edgeTypeCategories = New List(Of String)()
                ' 添加所有中性丢失类型
                For Each kvp In NeutralLosses
                    _edgeTypeCategories.Add("NL_" & kvp.Key)
                Next
                ' 添加所有加合物差异类型
                For Each kvp In AdductDifferences
                    _edgeTypeCategories.Add("AD_" & kvp.Key)
                Next
                ' 添加同位素类型
                _edgeTypeCategories.Add("ISO_C13")
                _edgeTypeCategories.Add("ISO_C13_2x")
                _edgeTypeCategories.Add("ISO_N15")
                _edgeTypeCategories.Add("ISO_S34")
            End If
            Return _edgeTypeCategories
        End Get
    End Property

    ' ========================================================
    ' 特征离子类别列表 (用于One-hot编码)
    ' ========================================================
    Private _featureIonCategories As List(Of KeyValuePair(Of String, Double)) = Nothing

    Public ReadOnly Property FeatureIonCategories As List(Of KeyValuePair(Of String, Double))
        Get
            If _featureIonCategories Is Nothing Then
                _featureIonCategories = New List(Of KeyValuePair(Of String, Double))()
                For Each kvp In FeatureIons
                    _featureIonCategories.Add(kvp)
                Next
            End If
            Return _featureIonCategories
        End Get
    End Property

    ' ========================================================
    ' 辅助函数：在指定哈希表中查找匹配的质量
    ' ========================================================
    ''' <summary>
    ''' 在给定的质量字典中查找与目标质量匹配的条目
    ''' </summary>
    ''' <param name="table">质量字典 (名称 -> 质量)</param>
    ''' <param name="targetMass">目标质量</param>
    ''' <param name="tolerance">质量容差</param>
    ''' <returns>匹配的条目列表 (名称, 质量, 偏差)</returns>
    Public Function FindMassMatches(table As Dictionary(Of String, Double),
                                     targetMass As Double,
                                     tolerance As Double) As List(Of (Name As String, Mass As Double, [Error] As Double))
        Dim matches As New List(Of (name As String, mass As Double, [error] As Double))()
        For Each kvp In table
            Dim err = std.Abs(kvp.Value - targetMass)
            If err <= tolerance Then
                matches.Add((kvp.Key, kvp.Value, err))
            End If
        Next
        ' 按偏差排序
        matches.Sort(Function(a, b) a.Error.CompareTo(b.Error))
        Return matches
    End Function

    ''' <summary>
    ''' 查找同位素匹配
    ''' </summary>
    Public Function FindIsotopeMatch(massDiff As Double, tolerance As Double) As String
        ' C13 单同位素
        If std.Abs(massDiff - C13IsotopeMass) <= tolerance Then
            Return "ISO_C13"
        End If
        ' C13 双同位素
        If std.Abs(massDiff - 2 * C13IsotopeMass) <= tolerance Then
            Return "ISO_C13_2x"
        End If
        ' N15 同位素
        If std.Abs(massDiff - 0.997035) <= tolerance Then
            Return "ISO_N15"
        End If
        ' S34 同位素
        If std.Abs(massDiff - 1.995796) <= tolerance Then
            Return "ISO_S34"
        End If
        Return Nothing
    End Function

End Module
