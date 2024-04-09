Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Module graph_test

    Sub Main()
        Dim test_smiles As String = "CC(C)C[C@H](N)C(O)=O"
        Dim chemical_struct As ChemicalFormula = ParseChain.ParseGraph(test_smiles, strict:=False)
        Dim molecular_graph As NetworkGraph = chemical_struct.AsGraph
        Dim vec = MolecularGraph.ToVector(molecular_graph)

        Pause()
    End Sub
End Module
