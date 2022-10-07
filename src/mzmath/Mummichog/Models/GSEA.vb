Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Module GSEA

    ''' <summary>
    ''' Create a graph model for run Mummichog annotation without 
    ''' any network topology information
    ''' </summary>
    ''' <param name="cluster"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SingularGraph(cluster As Cluster) As NetworkGraph
        Dim g As New NetworkGraph

        For Each member As BackgroundGene In cluster.members
            Call g.CreateNode(member.accessionID, New NodeData With {.label = member.name})
        Next

        Return g
    End Function
End Module
