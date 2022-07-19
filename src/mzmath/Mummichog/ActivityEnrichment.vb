Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact

Public Class ActivityEnrichment

    ''' <summary>
    ''' the network modularity
    ''' </summary>
    ''' <returns></returns>
    Public Property Q As Double

    ''' <summary>
    ''' the metabolite hits input size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Input As Integer
        Get
            Return Hits.Length
        End Get
    End Property

    ''' <summary>
    ''' the pathway map background size
    ''' </summary>
    ''' <returns></returns>
    Public Property Background As Integer

    ''' <summary>
    ''' Q * Input / Background
    ''' </summary>
    ''' <returns></returns>
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

    ''' <summary>
    ''' the ms1 peak list annotation result
    ''' </summary>
    ''' <returns></returns>
    Public Property Hits As MzQuery()
    ''' <summary>
    ''' usually the pathway name
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' the pathway description details
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String

    Public Overrides Function ToString() As String
        Return $"{Name}: activity={Activity.ToString("F4")}, fisher={Fisher.two_tail_pvalue.ToString("G4")}"
    End Function

    Public Shared Function Evaluate(input As Dictionary(Of String, MzQuery),
                                    background As NamedValue(Of NetworkGraph),
                                    modelSize As Integer) As ActivityEnrichment

        Dim mapping As NetworkGraph = getSubGraph(input.Keys, background)
        Dim graph As NetworkGraph = background.Value
        Dim modularity As Double = Communities.Modularity(g:=mapping)
        Dim F As FishersExactPvalues = FishersExactTest.FishersExact(
            n11:=mapping.vertex.Count,
            n12:=graph.vertex.Count,
            n21:=input.Count - mapping.vertex.Count,
            n22:=modelSize - graph.vertex.Count
        )
        Dim name As String = background.Name
        Dim description As String = background.Description

        Return New ActivityEnrichment With {
            .Q = (modularity - -0.5) / (1 - -0.5),  ' [-0.5, 1] -> [0, 1]
            .Background = graph.vertex.Count,
            .Hits = mapping.vertex _
                .Where(Function(v) input.ContainsKey(v.label)) _
                .Select(Function(v) input(v.label)) _
                .ToArray,
            .Fisher = F,
            .Name = name,
            .Description = description
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="input">the raw candidate input id list</param>
    ''' <param name="background"></param>
    ''' <returns></returns>
    Private Shared Function getSubGraph(input As IEnumerable(Of String), background As NetworkGraph) As NetworkGraph
        Dim subgraph As New NetworkGraph
        Dim idIndex As Index(Of String) = input.Indexing
        Dim edgeBundles = background.graphEdges _
            .Where(Function(i)
                       Return i.U.label Like idIndex OrElse i.V.label Like idIndex
                   End Function) _
            .ToArray

        ' get connected nodes
        For Each id As String In edgeBundles _
            .Select(Function(l) {l.U, l.V}) _
            .IteratesALL _
            .Select(Function(v) v.label) _
            .Distinct

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

        ' add island nodes
        For Each v As Node In background.vertex
            If v.label Like idIndex Then
                If subgraph.GetElementByID(v.label) Is Nothing Then
                    Call subgraph.CreateNode(v.label, v.data)
                End If
            End If
        Next

        Return subgraph
    End Function
End Class
