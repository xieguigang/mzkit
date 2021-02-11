#Region "Microsoft.VisualBasic::34ce4d82c7b47b7b70d8432bef8d2b2b, FormulaSearch.Extensions\AtomGroups\Alkyl.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Class Alkyl
    ' 
    '     Properties: amyl_group, butyl_group, ethyl_group, hexyl_group, isobutyl_group
    '                 isopropyl_group, methyl_group, propyl_group, secbutyl_group, tertbutyl_group
    ' 
    ' /********************************************************************************/

#End Region

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

