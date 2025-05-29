Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports std = System.Math

Public Module AnthocyaninValidator

    ' 原子量常量
    ReadOnly C_Weight As Double = Formula.AllAtomElements!C.isotopic
    ReadOnly H_Weight As Double = Formula.AllAtomElements!H.isotopic
    ReadOnly O_Weight As Double = Formula.AllAtomElements!O.isotopic

    Public Function CalculateProbability(formula As String) As Double
        Dim score As Double = 0
        ' 解析化学式
        Dim elements As Dictionary(Of String, Integer) = FormulaScanner.ScanFormula(formula)?.CountsByElement

        If elements Is Nothing Then Return 0

        ' 规则1：必需元素检查
        If Not elements.ContainsKey("C") OrElse Not elements.ContainsKey("H") OrElse Not elements.ContainsKey("O") Then
            Return 0
        End If

        ' 规则2：碳数范围（15-30）
        Dim cCount = elements("C")
        score += std.Max(0, 1 - std.Abs(cCount - 20) / 10) ' 15-30得0.5-1分

        ' 规则3：氧数范围（5-20）
        Dim oCount = elements("O")
        score += std.Max(0, 1 - std.Abs(oCount - 12) / 8) ' 5-20得0.5-1分

        ' 规则4：分子量检查（280-1500）
        Dim molecularWeight = cCount * C_Weight + elements("H") * H_Weight + oCount * O_Weight
        score += If(molecularWeight >= 280 AndAlso molecularWeight <= 1500, 1, 0)

        ' 规则5：糖基检测（如C6H10O5）
        If Regex.IsMatch(formula, "C\d+H\d+O5") Then score += 1

        ' 规则6：酚羟基检测（H/O比例）
        Dim h_oRatio = elements("H") / oCount
        score += If(h_oRatio > 1.2 AndAlso h_oRatio < 2.5, 0.5, 0)

        Return score / 4.5 * 100 ' 总分4.5分转换为百分比
    End Function
End Module