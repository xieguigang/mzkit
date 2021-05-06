Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub simpleTest()
        Call runTest("CC", "ethane CH3CH3")
        Call runTest("C=O", "formaldehyde (CH2O)")
        Call runTest("C=C", "ethene (CH2=CH2)")
        Call runTest("O=C=O", "carbon dioxide (CO2)")
        Call runTest("COC", "dimethyl ether (CH3OCH3)")
        Call runTest("C#N", "hydrogen cyanide (HCN)")
        Call runTest("CCO", "ethanol (CH3CH2OH)")
        Call runTest("[H][H]", "molecular hydrogen (H2)")
        Call runTest("C=C-C-C=C-C-O", "6-hydroxy-1,4-hexadiene CH2=CH-CH2-CH=CH-CH2-OH")
    End Sub

    Sub Main()

        Call simpleTest()
        Call branchTest()

        Pause()
    End Sub

    Sub branchTest()
        Call runTest("CCN(CC)CC", "Triethylamine  C6H15N")
        Call runTest("CC(C)C(=O)O", "Isobutyric acid  C4H8O2")
        Call runTest("C=CC(CCC)C(C(C)C)CCC", "3-propyl-4-isopropyl-1-heptene C10H20")
    End Sub

    Sub runTest(SMILES As String, prompt As String)
        Call Console.WriteLine(prompt)

        Dim formula = ParseChain.ParseGraph(SMILES).GetFormula

        Call Console.WriteLine(formula.ToString)
        Call Console.WriteLine(Formula.BuildFormula(formula.CountsByElement))
        Call Console.WriteLine($"exact mass: {formula.ExactMass.ToString("F4")}")

        Call Console.WriteLine(New String("-"c, 32))
        Call Console.WriteLine()
    End Sub

End Module
