#Region "Microsoft.VisualBasic::c33400512e42a200459f0ab147b6ea51, src\metadb\FormulaSearch.Extensions\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

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
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2CH2").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("(CH3)2CH").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2CH2CH2").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("(CH3)2CHCH2").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3CH2(CH3)CH").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("C5H11").DebugView)
        Call Console.WriteLine(FormulaScanner.ScanFormula("CH3(CH2)4CH2").DebugView)

        Call Console.WriteLine(FormulaScanner.ScanFormula("COH").DebugView)


        Dim anno As New PeakAnnotation
        Dim result = anno.RunAnnotation(101 + Element.H, istoTest)

        Pause()
    End Sub

End Module
