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
            Dim g As New G
            Dim id As i32 = 0
            Dim atom_keys As New List(Of String)

            For Each atom As Atom In mol.Atoms
                Call atom_keys.Add($"{atom.Atom}_{atom.Coordination.ToString}_{atom.GetHashCode}")
                Call g.AddVertex(New Node With {
                    .ID = ++id,
                    .label = atom.Atom
                })
            Next

            For Each key As Bound In mol.Bounds
                Call g.CreateEdge(mol.Atoms(key.i).Atom, mol.Atoms(key.j).Atom, key.Type)
            Next

            Return g
        End Function
    End Module
End Namespace