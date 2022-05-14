Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact

Public Class ActivityEnrichment

    ''' <summary>
    ''' the network modularity
    ''' </summary>
    ''' <returns></returns>
    Public Property Q As Double

    Public ReadOnly Property Input As Integer
        Get
            Return Hits.Length
        End Get
    End Property

    Public Property Background As Integer

    Public ReadOnly Property Activity As Double
        Get
            Return Q * Input / Background
        End Get
    End Property

    ''' <summary>
    ''' the fisher test p-value
    ''' </summary>
    ''' <returns></returns>
    Public Property Fisher As FishersExactPvalues
    Public Property Hits As String()

    Public Shared Function Evaluate(input As IEnumerable(Of String), background As NetworkGraph, modelSize As Integer) As ActivityEnrichment
        Dim allInputId As String() = input.ToArray
        Dim mapping As NetworkGraph = getSubGraph(allInputId, background)
        Dim modularity As Double = Communities.Modularity(g:=mapping)
        Dim F As FishersExactPvalues = FishersExactTest.FishersExact(
            n11:=mapping.vertex.Count,
            n12:=background.vertex.Count,
            n21:=allInputId.Length - mapping.vertex.Count,
            n22:=modelSize - background.vertex.Count
        )

        Return New ActivityEnrichment With {
            .Q = modularity,
            .Background = background.vertex.Count,
            .Hits = mapping.vertex _
                .Select(Function(v) v.label) _
                .ToArray,
            .Fisher = F
        }
    End Function

    Private Shared Function getSubGraph(input As IEnumerable(Of String), background As NetworkGraph) As NetworkGraph
        Dim subgraph As New NetworkGraph
        Dim idIndex As Index(Of String) = input.Indexing
        Dim edgeBundles = background.graphEdges _
            .Where(Function(i)
                       Return i.U.label Like idIndex OrElse i.V.label Like idIndex
                   End Function) _
            .ToArray

        For Each id As String In idIndex.Objects
            Call subgraph.CreateNode(id, background.GetElementByID(id).data)
        Next
        For Each edge As Edge In edgeBundles
            Call subgraph.CreateEdge(
                u:=subgraph.GetElementByID(edge.U.label),
                v:=subgraph.GetElementByID(edge.V.label),
                weight:=edge.weight,
                data:=edge.data
            )
        Next

        Return subgraph
    End Function
End Class
