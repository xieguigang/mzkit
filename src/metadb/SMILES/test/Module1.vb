Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub Main()

        ' ethane	
        ' CH3CH3
        Dim formula = ParseChain.ParseGraph("CC").GetFormula

        Call Console.WriteLine(formula.ToString)

        Pause()
    End Sub

End Module
