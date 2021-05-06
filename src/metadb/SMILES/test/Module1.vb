Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub Main()

        ' ethane	
        ' CH3CH3
        ' Call runTest("CC")

        ' formaldehyde	(CH2O)
        Call runTest("C=O")

        Pause()
    End Sub

    Sub runTest(SMILES As String)
        Dim formula = ParseChain.ParseGraph(SMILES).GetFormula

        Call Console.WriteLine(formula.ToString)
        Call Console.WriteLine(Formula.BuildFormula(formula.CountsByElement))
        Call Console.WriteLine($"exact mass: {formula.ExactMass.ToString("F4")}")

        Call Console.WriteLine(New String("-"c, 32))
        Call Console.WriteLine()
    End Sub

End Module
