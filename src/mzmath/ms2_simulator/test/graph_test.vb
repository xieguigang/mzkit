Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Serialization.JSON

Module graph_test

    Sub Main()
        Dim data As New List(Of ClusterEntity)

        data.Add(New ClusterEntity("leucine", vectorBuilder("CC(C)C[C@H](N)C(O)=O")))
        data.Add(New ClusterEntity("3-Isopropylmalic acid", vectorBuilder("CC(C)[C@@H]([C@@H](O)C(O)=O)C(O)=O")))
        data.Add(New ClusterEntity("Choline", vectorBuilder("C[N+](C)(C)CCO")))

        Pause()
    End Sub

    Private Function vectorBuilder(test_smiles As String) As Double()
        Dim chemical_struct As ChemicalFormula = ParseChain.ParseGraph(test_smiles, strict:=False)
        Dim molecular_graph As NetworkGraph = chemical_struct.AsGraph

        Call VBDebugger.WaitOutput()
        Call Console.WriteLine(molecular_graph.vertex.Select(Function(v) v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)).GetJson)

        Return MolecularGraph.ToVector(molecular_graph)
    End Function
End Module
