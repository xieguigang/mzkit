Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Module IsotopicTest

    ' Formula: C1H1Cl3
    ' Formula weight :  119.369 g mol–1
    ' Isotope pattern
    '
    ' 118  100.00  __________________________________________________
    ' 119    1.09  _
    ' 120   95.78  ________________________________________________
    ' 121    1.04  _
    ' 122   30.58  _______________
    ' 123    0.33  
    ' 124    3.25  __
    ' 125    0.04  

    Sub Main()
        Dim formula = FormulaScanner.ScanFormula("CHCl3")
        Dim dist = IsotopicPatterns.IsotopeDistribution.GenerateDistribution(formula)

        For Each item In dist.data
            Call Console.WriteLine(item.ToString)
        Next

        Pause()
    End Sub
End Module
