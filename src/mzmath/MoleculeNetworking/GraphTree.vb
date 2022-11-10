Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Module GraphTree

    <Extension>
    Public Function CreateGraph(ionPack As IEnumerable(Of NamedCollection(Of PeakMs2))) As NetworkGraph
        Dim g As New NetworkGraph
        Dim seed As Node
        Dim ions As PeakMs2()

        For Each bin As NamedCollection(Of PeakMs2) In ionPack
            ions = bin.value
            seed = g.CreateNode(
                label:=bin.name,
                data:=New NodeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {"rt", ions.Where(Function(d) d.lib_guid = bin.name).First.rt}
                    },
                    .label = bin.name,
                    .origID = bin.name
                }
            )

            For Each ion In ions.Where(Function(i) i.lib_guid <> bin.name)
                If g.GetElementByID(ion.lib_guid) Is Nothing Then
                    g.CreateNode(
                        label:=ion.lib_guid,
                        data:=New NodeData With {
                            .label = ion.lib_guid,
                            .origID = ion.lib_guid,
                            .Properties = New Dictionary(Of String, String) From {
                                {"rt", ion.rt}
                            }
                        }
                    )
                End If

                Call g.CreateEdge(
                    u:=g.GetElementByID(bin.name),
                    v:=g.GetElementByID(ion.lib_guid),
                    weight:=1
                )
            Next
        Next

        Return g
    End Function

    <Extension>
    Public Function AddClusterLinks(g As NetworkGraph, tree As ClusterTree) As NetworkGraph
        For Each layer In ClusterTree.GetClusters(tree)
            If Not layer.Childs.IsNullOrEmpty Then
                For Each child As Tree(Of String) In layer.Childs.Values
                    Call g.CreateEdge(
                        u:=g.GetElementByID(layer.Data),
                        v:=g.GetElementByID(child.Data),
                        weight:=0.5
                    )
                Next
            End If
        Next

        Return g
    End Function
End Module