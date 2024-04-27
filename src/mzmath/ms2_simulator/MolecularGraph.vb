#Region "Microsoft.VisualBasic::258438b434539241c00c628a419a694c, G:/mzkit/src/mzmath/ms2_simulator//MolecularGraph.vb"

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

    '   Total Lines: 42
    '    Code Lines: 34
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.20 KB


    ' Module MolecularGraph
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getVocabulary, ToVector
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.MachineLearning.Bootstrapping

Public Module MolecularGraph

    Dim embedding As New Graph2Vec(AddressOf getVocabulary)

    Sub New()
        Call embedding.Setup({
            "-CH3",
            "-CH=",
            "-CH2-",
            "-OH",
            "-O-",
            "-NH2",
            "-NH3",
            "-NH4",
            "C",
            "N"
        })
    End Sub

    Private Function getVocabulary(atom_group As Vertex) As String
        Return DirectCast(atom_group, Node).data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
    End Function

    Public Function ToVector(molecular As NetworkGraph) As Double()
        Dim graph As New Graph

        For Each v As Node In molecular.vertex
            Call graph.AddVertex(v)
        Next
        For Each link As Edge In molecular.graphEdges
            Call graph.AddEdge(link.U, link.V, link.weight)
        Next

        Return embedding.GraphVector(graph)
    End Function

End Module

