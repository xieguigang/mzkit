#Region "Microsoft.VisualBasic::7da9277c8ecb9cbdc1244ed7f54d1caa, E:/mzkit/Rscript/Library/mzkit_app/src/mzDIA//ms2_simulator.vb"

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

    '   Total Lines: 89
    '    Code Lines: 63
    ' Comment Lines: 12
    '   Blank Lines: 14
    '     File Size: 3.46 KB


    ' Module ms2_simulator
    ' 
    '     Function: embed_graph2vector, loadKCF, MolecularGraph_func
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mzkit.simulator")>
Module ms2_simulator

    <ExportAPI("read.kcf")>
    Public Function loadKCF(file As String) As KCF
        Return file.LoadKCF(False)
    End Function

    ''' <summary>
    ''' parse the smiles structure string as molecular network graph
    ''' </summary>
    ''' <param name="mol"></param>
    ''' <param name="verbose"></param>
    ''' <returns></returns>
    <ExportAPI("molecular.graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function MolecularGraph_func(<RRawVectorArgument>
                                        mol As Object,
                                        Optional verbose As Boolean = False,
                                        Optional env As Environment = Nothing) As Object

        Return env.EvaluateFramework(Of String, NetworkGraph)(
            x:=mol,
            eval:=Function(smiles)
                      If verbose Then
                          Call VBDebugger.EchoLine(smiles)
                      End If

                      Dim chemical_struct As ChemicalFormula = ParseChain.ParseGraph(smiles, strict:=False)
                      Dim molecular_graph As NetworkGraph = chemical_struct.AsGraph

                      If verbose Then
                          Call VBDebugger.WaitOutput()
                          Call Console.WriteLine(molecular_graph.vertex.Select(Function(v) v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)).GetJson)
                      End If

                      Return molecular_graph
                  End Function)
    End Function

    ''' <summary>
    ''' make the molecular graph embedding to vector
    ''' </summary>
    ''' <param name="mol"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("embedded.graph2vector")>
    Public Function embed_graph2vector(<RRawVectorArgument> mol As Object, Optional env As Environment = Nothing) As Object
        If TypeOf mol Is NetworkGraph Then
            Return MolecularGraph.ToVector(DirectCast(mol, NetworkGraph))
        End If

        Dim exports As list = list.empty
        Dim names As String() = Nothing
        Dim graphs As pipeline = pipeline.TryCreatePipeline(Of NetworkGraph)(mol, env)

        If TypeOf mol Is list Then
            names = DirectCast(mol, list).slotKeys
        End If
        If graphs.isError Then
            Return graphs.getError
        End If

        Dim i As Integer = 0
        Dim pool As NetworkGraph() = graphs.populates(Of NetworkGraph)(env).ToArray

        For Each graph As NetworkGraph In pool
            exports.add(names.ElementAtOrDefault(i, $"mol_{i + 1}"), MolecularGraph.ToVector(graph))
            i += 1
        Next

        Return exports
    End Function

End Module
