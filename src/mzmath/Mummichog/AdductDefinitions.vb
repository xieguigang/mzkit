Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Linq

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
    Public Iterator Function GetPositiveAdducts() As IEnumerable(Of MzCalculator)
        ' 主要加合物 (常见, 高权重)
        Yield New MzCalculator With {.name = "[M+H]+", .charge = 1, .adducts = PROTON_MASS, .M = 1, .mode = "+"c, .IsCommon = True}
        Yield New MzCalculator With {.name = "[M+Na]+", .charge = 1, .adducts = SODIUM_MASS, .M = 1, .mode = "+"c, .IsCommon = True}
        Yield New MzCalculator With {.name = "[M+NH4]+", .charge = 1, .adducts = AMMONIUM_MASS, .M = 1, .mode = "+"c, .IsCommon = True}
        Yield New MzCalculator With {.name = "[M+K]+", .charge = 1, .adducts = POTASSIUM_MASS, .M = 1, .mode = "+"c, .IsCommon = True}

        ' 中性丢失加合物
        Yield New MzCalculator With {.name = "[M+H-H2O]+", .charge = 1, .adducts = PROTON_MASS - WATER_MASS, .M = 1, .mode = "+"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M+H-NH3]+", .charge = 1, .adducts = PROTON_MASS - NH3_MASS, .M = 1, .mode = "+"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M+H-CO2]+", .charge = 1, .adducts = PROTON_MASS - CO2_MASS, .M = 1, .mode = "+"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M+H-2H2O]+", .charge = 1, .adducts = PROTON_MASS - 2 * WATER_MASS, .M = 1, .mode = "+"c, .IsCommon = False}

        ' 二聚体加合物
        Yield New MzCalculator With {.name = "[2M+H]+", .charge = 1, .adducts = PROTON_MASS, .M = 2, .mode = "+"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[2M+Na]+", .charge = 1, .adducts = SODIUM_MASS, .M = 2, .mode = "+"c, .IsCommon = False}

        ' 多电荷加合物
        Yield New MzCalculator With {.name = "[M+2H]2+", .charge = 2, .adducts = 2 * PROTON_MASS, .M = 1, .mode = "+"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M+H+Na]2+", .charge = 2, .adducts = PROTON_MASS + SODIUM_MASS, .M = 1, .mode = "+"c, .IsCommon = False}
    End Function

    ''' <summary>
    ''' 获取负离子模式下的标准加合物列表
    ''' <para>
    ''' 包含: [M-H]-, [M+Cl]-, [M-H-H2O]-, [M-H-CO2]-,
    '''       [M+Na-2H]-, [M+K-2H]-, [2M-H]-, [M-HCOO]-, [M-CH3COO]-
    ''' </para>
    ''' </summary>
    Public Iterator Function GetNegativeAdducts() As IEnumerable(Of MzCalculator)
        ' 主要加合物
        Yield New MzCalculator With {.name = "[M-H]-", .charge = -1, .adducts = -PROTON_MASS, .M = 1, .mode = "-"c, .IsCommon = True}
        Yield New MzCalculator With {.name = "[M+Cl]-", .charge = -1, .adducts = CHLORIDE_MASS, .M = 1, .mode = "-"c, .IsCommon = True}

        ' 中性丢失
        Yield New MzCalculator With {.name = "[M-H-H2O]-", .charge = -1, .adducts = -PROTON_MASS - WATER_MASS, .M = 1, .mode = "-"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M-H-CO2]-", .charge = -1, .adducts = -PROTON_MASS - CO2_MASS, .M = 1, .mode = "-"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M-H-NH3]-", .charge = -1, .adducts = -PROTON_MASS - NH3_MASS, .M = 1, .mode = "-"c, .IsCommon = False}

        ' 取代加合物
        Yield New MzCalculator With {.name = "[M+Na-2H]-", .charge = -1, .adducts = SODIUM_MASS - 2 * PROTON_MASS, .M = 1, .mode = "-"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M+K-2H]-", .charge = -1, .adducts = POTASSIUM_MASS - 2 * PROTON_MASS, .M = 1, .mode = "-"c, .IsCommon = False}

        ' 二聚体
        Yield New MzCalculator With {.name = "[2M-H]-", .charge = -1, .adducts = -PROTON_MASS, .M = 2, .mode = "-"c, .IsCommon = False}

        ' 加合酸根
        Yield New MzCalculator With {.name = "[M-HCOO]-", .charge = -1, .adducts = HCOOH_MASS - PROTON_MASS, .M = 1, .mode = "-"c, .IsCommon = False}
        Yield New MzCalculator With {.name = "[M-CH3COO]-", .charge = -1, .adducts = CH3COOH_MASS - PROTON_MASS, .M = 1, .mode = "-"c, .IsCommon = False}

        ' 多电荷
        Yield New MzCalculator With {.name = "[M-2H]2-", .charge = -2, .adducts = -2 * PROTON_MASS, .M = 1, .mode = "-"c, .IsCommon = False}
    End Function

    ''' <summary>
    ''' 根据电离模式获取对应的加合物列表
    ''' </summary>
    Public Function GetAdducts(mode As IonModes) As MzCalculator()
        If mode = IonModes.Positive Then
            Return GetPositiveAdducts().ToArray
        Else
            Return GetNegativeAdducts().ToArray
        End If
    End Function
End Module