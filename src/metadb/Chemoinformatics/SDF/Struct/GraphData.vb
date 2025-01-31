Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Language

Namespace SDF.Models

    ''' <summary>
    ''' graph model conversion of the molecule structure data and the network graph object
    ''' </summary>
    Public Module GraphData

        <Extension>
        Public Function AsMolecularGraph(Of Node As {New, Network.Node}, Edge As {New, Network.Edge(Of Node)}, G As {New, NetworkGraph(Of Node, Edge)})(mol As [Structure]) As G
            Dim graph As New G

            For Each atom As Atom In mol.Atoms
                Call graph.AddVertex(New Node With {
                    .ID = atom.GetHashCode,
                    .label = atom.Atom
                })
            Next

            For Each key As Bound In mol.Bounds
                Dim key1 = mol.Atoms(key.i).GetHashCode
                Dim key2 = mol.Atoms(key.j).GetHashCode

                Call graph.CreateEdge(key1, key2, key.Type)
            Next

            Return graph
        End Function
    End Module
End Namespace