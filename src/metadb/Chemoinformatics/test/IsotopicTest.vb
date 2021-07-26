Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Module IsotopicTest

    Sub Main()
        Dim formula = FormulaScanner.ScanFormula("CHCl3")
        Dim dist = IsotopicPatterns.IsotopeDistribution.GenerateDistribution(formula)

        For Each item In dist.data
            Call Console.WriteLine(item.ToString)
        Next

        Pause()
    End Sub
End Module
