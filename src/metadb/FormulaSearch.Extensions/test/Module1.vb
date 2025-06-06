﻿#Region "Microsoft.VisualBasic::9e0a477f44ca2c8db3916c773ad03ddc, metadb\FormulaSearch.Extensions\test\Module1.vb"

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


    ' Code Statistics:

    '   Total Lines: 54
    '    Code Lines: 38 (70.37%)
    ' Comment Lines: 2 (3.70%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (25.93%)
    '     File Size: 2.14 KB


    ' Module Module1
    ' 
    '     Sub: chargeTest, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Dim istoTest As ms2() = {
        New ms2 With {.mz = 101 + 3 * Element.H},
        New ms2 With {.mz = 101 + 2 * Element.H},
        New ms2 With {.mz = 101 + 1 * Element.H},
        New ms2 With {.mz = 101},
        New ms2 With {.mz = 59.1},
        New ms2 With {.mz = 25},
        New ms2 With {.mz = 29.0027396},
        New ms2 With {.mz = 13.222},
        New ms2 With {.mz = 25 + Element.H},
        New ms2 With {.mz = 43.0547722}
    }

    Sub Main()
        Call chargeTest()

        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2CH2").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("(CH3)2CH").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2CH2CH2").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("(CH3)2CHCH2").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2(CH3)CH").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("C5H11").CountsByElement.GetJson)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3(CH2)4CH2").CountsByElement.GetJson)

        Call Console.WriteLine(FormulaScanner.ScanFormula("COH").CountsByElement.GetJson)




        Pause()
    End Sub

    Sub chargeTest()
        ' https://pubchem.ncbi.nlm.nih.gov/compound/Malonate
        ' -2
        Dim formula As Formula = FormulaScanner.ScanFormula("C3H2O4")
        Dim charge As Integer = FormalCharge.EvaluateCharge(formula)


        Call Console.WriteLine(FormalCharge.EvaluateCharge(FormulaScanner.ScanFormula("C6H12O6")))

        Pause()
    End Sub

End Module
