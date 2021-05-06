Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub Main()

        Call runTest("CC", "ethane CH3CH3")
        Call runTest("C=O", "formaldehyde (CH2O)")
        Call runTest("C=C", "ethene (CH2=CH2)")
        Call runTest("O=C=O", "carbon dioxide (CO2)")
        Call runTest("COC", "dimethyl ether (CH3OCH3)")
        Call runTest("C#N", "hydrogen cyanide (HCN)")
        Call runTest("CCO", "ethanol (CH3CH2OH)")
        Call runTest("[H][H]", "molecular hydrogen (H2)")

        Pause()
    End Sub

    Sub runTest(SMILES As String, prompt As String)
        Dim formula = ParseChain.ParseGraph(SMILES).GetFormula

        Call Console.WriteLine(prompt)
        Call Console.WriteLine(formula.ToString)
        Call Console.WriteLine(Formula.BuildFormula(formula.CountsByElement))
        Call Console.WriteLine($"exact mass: {formula.ExactMass.ToString("F4")}")

        Call Console.WriteLine(New String("-"c, 32))
        Call Console.WriteLine()
    End Sub

End Module
