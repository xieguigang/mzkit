#Region "Microsoft.VisualBasic::ced351bc8d6c81a393469d1a00a4b06a, metadb\Chemoinformatics\SDF\Struct\GraphData.vb"

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

    '   Total Lines: 34
    '    Code Lines: 24 (70.59%)
    ' Comment Lines: 3 (8.82%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (20.59%)
    '     File Size: 1.16 KB


    '     Module GraphData
    ' 
    '         Function: AsMolecularGraph
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
            Dim graph As New G

            For Each atom As Atom In mol.Atoms
                Call graph.AddVertex(New Node With {
                    .ID = atom.GetHashCode,
                    .label = atom.Atom
                })
            Next

            For Each key As Bound In mol.Bounds
                Dim key1 = mol.Atoms(key.i).GetHashCode
                Dim key2 = mol.Atoms(key.j).GetHashCode

                Call graph.CreateEdge(key1, key2, key.Type)
            Next

            Return graph
        End Function
    End Module
End Namespace
