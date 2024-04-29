#Region "Microsoft.VisualBasic::72021dcdce7e04b987a9850ffd5fa05e, E:/mzkit/src/mzmath/ms2_simulator//test/graph_test.vb"

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

    '   Total Lines: 33
    '    Code Lines: 26
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.60 KB


    ' Module graph_test
    ' 
    '     Function: vectorBuilder
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Serialization.JSON

Module graph_test

    Sub Main()
        Dim data As New List(Of ClusterEntity)

        data.Add(New ClusterEntity("leucine", vectorBuilder("CC(C)C[C@H](N)C(O)=O")))
        data.Add(New ClusterEntity("3-Isopropylmalic acid", vectorBuilder("CC(C)[C@@H]([C@@H](O)C(O)=O)C(O)=O")))
        data.Add(New ClusterEntity("Choline", vectorBuilder("C[N+](C)(C)CCO")))
        data.Add(New ClusterEntity("Serylvaline", vectorBuilder("CC(C)[C@H](NC(=O)[C@@H](N)CO)C(O)=O")))
        data.Add(New ClusterEntity("L-Valine", vectorBuilder("CC(C)[C@H](N)C(O)=O")))
        data.Add(New ClusterEntity("Trigonelline", vectorBuilder("C[N+]1=CC=CC(=C1)C([O-])=O")))

        Pause()
    End Sub

    Private Function vectorBuilder(test_smiles As String) As Double()
        Dim chemical_struct As ChemicalFormula = ParseChain.ParseGraph(test_smiles, strict:=False)
        Dim molecular_graph As NetworkGraph = chemical_struct.AsGraph

        Call VBDebugger.WaitOutput()
        Call Console.WriteLine(molecular_graph.vertex.Select(Function(v) v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)).GetJson)

        Return MolecularGraph.ToVector(molecular_graph)
    End Function
End Module
