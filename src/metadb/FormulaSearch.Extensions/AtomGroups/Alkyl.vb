Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' 烷基（alkyl），即饱和烃基，是烷烃分子中少掉一个氢原子而成的烃基，常用-R表示。烷基是一类仅含有碳、氢两种原子的链状有机基团。
''' </summary>
Public Class Alkyl

    ''' <summary>
    ''' 甲基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property methyl_group As Formula = FormulaScanner.ScanFormula("CH3")
    ''' <summary>
    ''' 乙基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property ethyl_group As Formula = FormulaScanner.ScanFormula("CH3CH2")
    ''' <summary>
    ''' 丙基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property propyl_group As Formula = FormulaScanner.ScanFormula("CH3CH2CH2")
    ''' <summary>
    ''' 异丙基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property isopropyl_group As Formula = FormulaScanner.ScanFormula("(CH3)2CH")
    ''' <summary>
    ''' 丁基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property butyl_group As Formula = FormulaScanner.ScanFormula("CH3CH2CH2CH2")
    ''' <summary>
    ''' 异丁基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property isobutyl_group As Formula = FormulaScanner.ScanFormula("(CH3)2CHCH2")
    ''' <summary>
    ''' 仲丁基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property secbutyl_group As Formula = FormulaScanner.ScanFormula("CH3CH2(CH3)CH")
    ''' <summary>
    ''' 叔丁基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property tertbutyl_group As Formula = FormulaScanner.ScanFormula("(CH3)3C")
    ''' <summary>
    ''' 戊基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property amyl_group As Formula = FormulaScanner.ScanFormula("C5H11")
    ''' <summary>
    ''' 己基
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property hexyl_group As Formula = FormulaScanner.ScanFormula("CH3(CH2)4CH2")

End Class
