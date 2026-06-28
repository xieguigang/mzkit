Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module smiles_test

    Sub Main()
        For Each str As String In {"c1ccccc1", "c1ccccc1C", "C1=CC=CC=C1", "c1ccc2ccccc2c1", "C%12", "[NH3+]"}
            Dim g = ChemicalFormula.ParseGraph(str)

            Call Console.WriteLine(g.ToString)
        Next


        Dim graph = ChemicalFormula.ParseGraph("COC1=CC(C[C@H]2C(=O)OC[C@@H]2CC2=CC=C3OCOC3=C2)=CC(O)=C1OC")
        Dim formula = graph.GetFormula
        Dim exact_mass = formula.ExactMass

        Pause()
    End Sub
End Module
