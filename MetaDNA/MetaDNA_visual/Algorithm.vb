#Region "Microsoft.VisualBasic::a9094c93e0b6cb5eb7716f611a985229, MetaDNA\MZ.MetaDNA\Algorithm.vb"

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

' Module Algorithm
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module Algorithm

    <Extension>
    Public Function CreateGraph(metaDNA As XML) As NetworkGraph
        Dim g As New NetworkGraph
        Dim kegg_compound As Graph.Node
        Dim candidate_compound As Graph.Node
        Dim edge As Edge

        For Each compound In metaDNA.compounds
            kegg_compound = New Graph.Node With {
                .Label = compound.kegg,
                .data = New NodeData()
            }

            Call g.AddNode(kegg_compound)

            For Each candidate In compound.candidates
                candidate_compound = New Graph.Node With {
                    .Label = candidate.name,
                    .data = New NodeData With {.label = candidate.Msn}
                }
                edge = New Edge With {
                    .U = kegg_compound,
                    .V = candidate_compound,
                    .weight = candidate.edges.Length,
                    .data = New EdgeData
                }

                Call g.AddNode(candidate_compound)
                Call g.AddEdge(edge)

                For i As Integer = 0 To candidate.length - 2
                    edge = New Edge With {
                        .data = New EdgeData,
                        .U = g.GetNode(candidate.edges(i).ms1),
                        .V = g.GetNode(candidate.edges(i + 1).ms1)
                    }

                    Call g.AddEdge(edge)
                Next

                ' add edge that infer to current candidate
                edge = New Edge With {
                    .data = New EdgeData,
                    .U = g.GetNode(candidate.edges.Last.ms1),
                    .V = g.GetNode(candidate.name)
                }
                Call g.AddEdge(edge)
            Next
        Next

        Return g
    End Function
End Module
