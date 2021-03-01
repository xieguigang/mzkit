#Region "Microsoft.VisualBasic::18fe4ff418ef6a38bb838f77d17fa990, FormulaSearch.Extensions\AtomGroups\Ketones.vb"

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

    ' Class Ketones
    ' 
    '     Properties: acid_amides, acid_anhydride, acyl_halideBr, acyl_halideCl, acyl_halideF
    '                 acyl_halideI, acyl_peroxide, aldehyde, carboxylic_acid, carboxylic_ester
    '                 isocyanate, ketenes, ketone
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' 酮基是一个碳原子和氧原子形成双键，同时这个碳原子还和另外两个碳原子形成共价键结构式，可以用R1-(C=O)-R2。
''' 酮基是羰基的一种。酮基能够强烈吸收300nm左右光波的基团，含酮基的高分子容易吸收紫外线而导致光降解。
''' </summary>
Public Class Ketones

    Public Shared ReadOnly Property aldehyde As Formula = FormulaScanner.ScanFormula("COH")
    Public Shared ReadOnly Property ketone As Formula = FormulaScanner.ScanFormula("CO")
    Public Shared ReadOnly Property carboxylic_acid As Formula = FormulaScanner.ScanFormula("COOH")
    Public Shared ReadOnly Property carboxylic_ester As Formula = FormulaScanner.ScanFormula("COO")
    Public Shared ReadOnly Property acid_anhydride As Formula = FormulaScanner.ScanFormula("COOCO")
    Public Shared ReadOnly Property acyl_peroxide As Formula = FormulaScanner.ScanFormula("COOOCO")
    Public Shared ReadOnly Property acid_amides As Formula = FormulaScanner.ScanFormula("CONH2")
    Public Shared ReadOnly Property ketenes As Formula = FormulaScanner.ScanFormula("CHCO")
    Public Shared ReadOnly Property isocyanate As Formula = FormulaScanner.ScanFormula("NCO")

    Public Shared ReadOnly Property acyl_halideF As Formula = FormulaScanner.ScanFormula("COF")
    Public Shared ReadOnly Property acyl_halideCl As Formula = FormulaScanner.ScanFormula("COCl")
    Public Shared ReadOnly Property acyl_halideBr As Formula = FormulaScanner.ScanFormula("COBr")
    Public Shared ReadOnly Property acyl_halideI As Formula = FormulaScanner.ScanFormula("COI")

End Class
