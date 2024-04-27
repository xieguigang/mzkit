#Region "Microsoft.VisualBasic::7d2c1a91888f148616684b42f5f8c422, G:/mzkit/src/mzmath/Mummichog//ActivityEnrichment.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 160
    '    Code Lines: 91
    ' Comment Lines: 52
    '   Blank Lines: 17
    '     File Size: 5.57 KB


    ' Class ActivityEnrichment
    ' 
    '     Properties: Activity, Background, Description, Fisher, Hits
    '                 Input, Name, Q
    ' 
    '     Function: Evaluate, getSubGraph, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact

''' <summary>
''' a network module enrichment result
''' </summary>
''' <remarks>
''' the <see cref="Fisher"/> data is the gsea enrichment result of 
''' the vertex in current network module.
''' 
''' get a set of the metabolite annotation candidates from the <see cref="Hits"/> data.
''' </remarks>
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
                                    modelSize As Integer,
                                    pinList As Index(Of String),
                                    ignoreTopology As Boolean) As ActivityEnrichment

        Dim mapping As NetworkGraph = getSubGraph(input.Keys, background)
        Dim graph As NetworkGraph = background.Value
        Dim modularity As Double = If(ignoreTopology, 1, Communities.Modularity(g:=mapping))
        Dim F As FishersExactPvalues = FishersExactTest.FishersExact(
            n11:=mapping.vertex.Count,
            n12:=graph.vertex.Count,
            n21:=input.Count - mapping.vertex.Count,
            n22:=modelSize - graph.vertex.Count
        )
        Dim name As String = background.Name
        Dim description As String = background.Description
        Dim hits As MzQuery() = mapping.vertex _
            .Where(Function(v) input.ContainsKey(v.label)) _
            .Select(Function(v) input(v.label)) _
            .ToArray

        ' if any hits in pinned id list
        ' then the target background network topology
        ' result will be ignored
        If hits.Any(Function(m) m.unique_id Like pinList) Then
            modularity = 1
        End If

        Return New ActivityEnrichment With {
            .Q = (modularity - -0.5) / (1 - -0.5),  ' [-0.5, 1] -> [0, 1]
            .Background = graph.vertex.Count,
            .Hits = hits,
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
