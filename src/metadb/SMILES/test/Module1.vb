Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub Main()

        ' ethane	
        ' CH3CH3
        Dim parser As New ParseChain("CC")
        Dim graph = parser.ParseGraph
        Dim formula = graph.GetFormula

        Pause()
    End Sub

End Module
