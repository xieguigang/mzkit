#Region "Microsoft.VisualBasic::7da9277c8ecb9cbdc1244ed7f54d1caa, Rscript\Library\mzkit_app\src\mzDIA\ms2_simulator.vb"

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
'    Code Lines: 63 (70.79%)
' Comment Lines: 12 (13.48%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 14 (15.73%)
'     File Size: 3.46 KB


' Module ms2_simulator
' 
'     Function: embed_graph2vector, loadKCF, MolecularGraph_func
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("spectral_simulator")>
Module ms2_simulator

    <ExportAPI("read.kcf")>
    Public Function loadKCF(file As String) As KCF
        Return file.LoadKCF(False)
    End Function

    ''' <summary>
    ''' build molecule documents for do embedding
    ''' </summary>
    ''' <param name="mols"></param>
    ''' <returns></returns>
    <ExportAPI("buildDocuments")>
    Public Function buildDocuments(<RRawVectorArgument> mols As Object,
                                   <RRawVectorArgument>
                                   Optional id As Object = Nothing,
                                   Optional env As Environment = Nothing) As Object

        Dim formulas As pipeline = pipeline.TryCreatePipeline(Of ChemicalFormula)(mols, env)

        If formulas.isError Then
            Return formulas.getError
        End If

        Dim idset As String() = CLRVector.asCharacter(id)
        Dim i As i32 = 0
        Dim stream As IEnumerable(Of NamedValue(Of ChemicalFormula)) = (
            Iterator Function() As IEnumerable(Of NamedValue(Of ChemicalFormula))
                For Each f As ChemicalFormula In formulas.populates(Of ChemicalFormula)(env)
                    Yield New NamedValue(Of ChemicalFormula)(idset.ElementAtOrDefault(++i, f.id), f)
                Next
            End Function)()

        Return New list With {
            .slots = SMILESEmbedding _
                .StructureDocuments(mols:=stream) _
                .ToDictionary(Function(a) a.name,
                              Function(a)
                                  Return CObj(a.value)
                              End Function)
        }
    End Function

    ''' <summary>
    ''' parse the smiles structure string as molecular network graph
    ''' </summary>
    ''' <param name="mol"></param>
    ''' <param name="id">
    ''' tag the id data with the corresponding smiles graph data, this character id vector
    ''' length should be equals to the given molecules vector size.
    ''' </param>
    ''' <param name="verbose"></param>
    ''' <returns></returns>
    <ExportAPI("molecular_graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function MolecularGraph_func(<RRawVectorArgument>
                                        mol As Object,
                                        <RRawVectorArgument> Optional id As Object = Nothing,
                                        <RRawVectorArgument> Optional name As Object = Nothing,
                                        Optional digest_formula As Boolean = True,
                                        Optional verbose As Boolean = False,
                                        Optional tqdm As Boolean = True,
                                        Optional env As Environment = Nothing) As Object

        Dim idset As String() = CLRVector.asCharacter(id)
        Dim nameSet As String() = CLRVector.asCharacter(name)
        Dim i As i32 = 0

        Return env.EvaluateFramework(Of String, Object)(
            x:=mol,
            eval:=Function(smiles)
                      Return smiles.parseSingleSmiles(
                            idset.ElementAtOrDefault(i),
                            nameSet.ElementAtOrDefault(++i),
                            verbose, digest_formula)
                  End Function,
            parallel:=False,
            tqdm:=tqdm)
    End Function

    <Extension>
    Private Function parseSingleSmiles(smiles As String,
                                       id As String, name As String,
                                       verbose As Boolean,
                                       digest_formula As Boolean) As Object
        If verbose Then
            Call VBDebugger.EchoLine(smiles)
        End If

        Dim chemical_struct As ChemicalFormula = ParseChain.ParseGraph(smiles, strict:=False)

        chemical_struct.id = id
        chemical_struct.name = name

        If digest_formula Then
            Dim molecular_graph As NetworkGraph = chemical_struct.AsGraph

            If verbose Then
                Call VBDebugger.WaitOutput()
                Call Console.WriteLine(molecular_graph.vertex _
                        .Select(Function(v) v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)) _
                        .GetJson)
            End If

            Return molecular_graph
        Else
            Return chemical_struct
        End If
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
