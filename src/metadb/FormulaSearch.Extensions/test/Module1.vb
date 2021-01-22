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


        Dim anno As New PeakAnnotation
        Dim result = anno.RunAnnotation(101 + Element.H, istoTest)

        Pause()
    End Sub

End Module
