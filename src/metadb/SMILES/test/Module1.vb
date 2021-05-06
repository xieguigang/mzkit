Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub Main()

        ' ethane	
        ' CH3CH3
        Call runTest("CC")

        Pause()
    End Sub

    Sub runTest(SMILES As String)
        Dim formula = ParseChain.ParseGraph("CC").GetFormula

        Call Console.WriteLine(formula.ToString)
        Call Console.WriteLine(Formula.BuildFormula(formula.CountsByElement))
        Call Console.WriteLine($"exact mass: {formula.ExactMass.ToString("F4")}")
    End Sub

End Module
