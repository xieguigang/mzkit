#Region "Microsoft.VisualBasic::53fb0aebcdb0bcbdb69b23954c788d73, mzkit\src\mzmath\MoleculeNetworking\GraphTree.vb"

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

'   Total Lines: 70
'    Code Lines: 62
' Comment Lines: 0
'   Blank Lines: 8
'     File Size: 2.58 KB


' Module GraphTree
' 
'     Function: AddClusterLinks, CreateGraph
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.BinaryTree

''' <summary>
''' create the network graph data from a given spectrum clustering tree
''' </summary>
Public Module GraphTree

    ''' <summary>
    ''' convert the cluster tree nodes as the graph object
    ''' </summary>
    ''' <param name="ionPack">
    ''' the graph tree nodes object which is extracted from the cluster tree object
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateGraph(ionPack As IEnumerable(Of NamedCollection(Of PeakMs2))) As NetworkGraph
        Dim g As New NetworkGraph

        For Each bin As NamedCollection(Of PeakMs2) In ionPack
            Call bin.ProcessingClusterTreeNode(g)
        Next

        Return g
    End Function

    <Extension>
    Private Sub ProcessingClusterTreeNode(bin As NamedCollection(Of PeakMs2), g As NetworkGraph)
        Dim ions As PeakMs2() = bin.value
        Dim seed As Node = g.CreateNode(
            label:=bin.name,
            data:=New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {"rt", ions.Where(Function(d) d.lib_guid = bin.name).First.rt}
                },
                .label = bin.name,
                .origID = bin.name
            }
        )

        For Each ion As PeakMs2 In ions.Where(Function(i) i.lib_guid <> bin.name)
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
    End Sub

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
