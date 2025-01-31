Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Network

Namespace SDF.Models

    ''' <summary>
    ''' graph model conversion of the molecule structure data and the network graph object
    ''' </summary>
    Public Module GraphData

        <Extension>
        Public Function AsMolecularGraph(Of Node As {New, Network.Node}, Edge As {New, Network.Edge(Of Node)}, G As NetworkGraph(Of Node, Edge))(mol As [Structure]) As G

        End Function
    End Module
End Namespace