Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping

Public Module MolecularGraph

    Dim embedding As New Graph2Vec(AddressOf getVocabulary)

    Sub New()
        Call embedding.Setup({
            "-CH3",
            "-CH=",
            "-CH2-",
            "-OH",
            "-O-",
            "-NH2",
            "-NH3",
            "-NH4"
        })
    End Sub

    Private Function getVocabulary(atom_group As Vertex) As String
        Return DirectCast(atom_group, Node).data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
    End Function

    Public Function ToVector(molecular As NetworkGraph) As Double()
        Dim graph As New Graph

        For Each v As Node In molecular.vertex
            Call graph.AddVertex(v)
        Next
        For Each link As Edge In molecular.graphEdges
            Call graph.AddEdge(link.U, link.V, link.weight)
        Next

        Return embedding.GraphVector(graph)
    End Function

End Module
