#Region "Microsoft.VisualBasic::f94471f9c1cc5012920cea85718421e0, mzkit\Rscript\Library\mzkit\annotations\Mummichog.vb"

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

'   Total Lines: 304
'    Code Lines: 218
' Comment Lines: 47
'   Blank Lines: 39
'     File Size: 12.18 KB


' Module Mummichog
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: CreateKEGGBackground, createMzSet, fromGseaBackground, getResultTable, GroupPeaks
'               mzScore, PeakListAnnotation, queryCandidateSet
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports std = System.Math

''' <summary>
''' Mummichog searches for enrichment patterns on metabolic network, 
''' bypassing metabolite identification, to generate high-quality
''' hypotheses directly from a LC-MS data table.
''' </summary>
<Package("Mummichog")>
Module Mummichog

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(ActivityEnrichment()), AddressOf getResultTable)
    End Sub

    Private Function getResultTable(result As ActivityEnrichment(), args As list, env As Environment) As dataframe
        Dim output As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result _
                .Select(Function(i) i.Name) _
                .ToArray
        }

        Call output.add("description", result.Select(Function(i) i.Description))
        Call output.add("Q", result.Select(Function(i) i.Q))
        Call output.add("input_size", result.Select(Function(i) i.Input))
        Call output.add("background_size", result.Select(Function(i) i.Background))
        Call output.add("activity", result.Select(Function(i) i.Activity))
        Call output.add("p-value", result.Select(Function(i) i.Fisher.two_tail_pvalue))
        Call output.add("hits", result.Select(Function(i) i.Hits.JoinBy("; ")))

        Return output
    End Function

    ''' <summary>
    ''' export of the annotation score result table
    ''' </summary>
    ''' <param name="result">
    ''' the annotation result which is generated from the 
    ''' ``peakList_annotation`` function.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("mzScore")>
    Public Function mzScore(result As ActivityEnrichment(),
                            Optional minHits As Integer = -1,
                            Optional ignore_topology As Boolean = False) As dataframe

        Dim resultSet As ActivityEnrichment() = result _
            .Where(Function(a) a.Input >= minHits) _
            .ToArray
        Dim allUnion As MzQuery() = resultSet _
            .Select(Function(a) a.Hits.SafeQuery) _
            .IteratesALL _
            .GroupBy(Function(a) MzQuery.ReferenceKey(a)) _
            .Select(Function(a) a.First) _
            .ToArray
        Dim scores As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim unionScore As New Dictionary(Of String, Double)

        For Each a As MzQuery In allUnion
            Call unionScore.Add(MzQuery.ReferenceKey(a), 0)
        Next

        For Each pathway As ActivityEnrichment In resultSet
            Dim score As Double = If(ignore_topology, 1, pathway.Activity)

            If pathway.Fisher.two_tail_pvalue < 1.0E-100 Then
                score *= 100
            Else
                score *= -std.Log10(pathway.Fisher.two_tail_pvalue) + 1
            End If

            For Each hit In pathway.Hits.SafeQuery
                unionScore(MzQuery.ReferenceKey(hit)) += score
            Next
        Next

        Call scores.add("mz", allUnion.Select(Function(i) i.mz))
        Call scores.add("mz_theoretical", allUnion.Select(Function(i) i.mz_ref))
        Call scores.add("ppm", allUnion.Select(Function(i) i.ppm))
        Call scores.add("unique_id", allUnion.Select(Function(i) i.unique_id))
        Call scores.add("name", allUnion.Select(Function(i) i.name))
        Call scores.add("precursor_type", allUnion.Select(Function(i) i.precursorType))
        Call scores.add("score", allUnion.Select(Function(a) unionScore(MzQuery.ReferenceKey(a))))

        Return scores
    End Function

    ''' <summary>
    ''' ### do ms1 peaks annotation
    ''' 
    ''' Do ms1 peak list annotation based on the given biological context information
    ''' </summary>
    ''' <param name="background">
    ''' the enrichment and network topology graph mode list
    ''' </param>
    ''' <param name="candidates">
    ''' a set of m/z search result list based on the given background model
    ''' </param>
    ''' <param name="minhit"></param>
    ''' <param name="permutation"></param>
    ''' <param name="modelSize"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("peakList_annotation")>
    <RApiReturn(GetType(ActivityEnrichment))>
    Public Function PeakListAnnotation(background As list, candidates As MzSet(),
                                       Optional minhit As Integer = 3,
                                       Optional permutation As Integer = 100,
                                       Optional modelSize As Integer = -1,
                                       Optional pinned As String() = Nothing,
                                       Optional ignore_topology As Boolean = False,
                                       Optional ga As Boolean = False,
                                       Optional pop_size As Integer = 100,
                                       Optional mutation_rate As Double = 0.3,
                                       Optional env As Environment = Nothing) As Object

        Dim models As New List(Of NamedValue(Of NetworkGraph))
        Dim graph As NamedValue(Of NetworkGraph)
        Dim slot As list
        Dim println As Action(Of Object) = env.WriteLineHandler

        For Each name As String In background.getNames
            slot = background.getByName(name)
            graph = New NamedValue(Of NetworkGraph) With {
                .Name = name,
                .Description = slot.getValue({"desc", "description"}, env, "NA"),
                .Value = slot.getValue(Of NetworkGraph)({"model", "background", "graph"}, env)
            }

            If graph.Value.vertex.Count > 0 Then
                Call models.Add(graph)
            End If
        Next

        If Not pinned.IsNullOrEmpty Then
            Call println("a set of metabolites will pinned in the annotation loops:")
            Call println(pinned)
        End If

        Dim result As ActivityEnrichment()

        If ga Then
            result = GAPeakListAnnotation.PeakListAnnotation(
                candidates:=candidates, background:=models,
                minhit:=minhit, permutation:=permutation,
                modelSize:=modelSize, pinned:=pinned,
                popsize:=pop_size,
                ignoreTopology:=ignore_topology,
                mutation_rate:=mutation_rate
            )
        Else
            Call println($"Run mummichog algorithm with Monte-Carlo permutation in parallel with {VectorTask.n_threads} CPU threads!")
            Call println($"evaluate for {candidates.Length} ion features,")
            Call println($"based on {models.Count} biological context background model!")

            result = candidates.PeakListAnnotation(
                background:=models,
                minhit:=minhit,
                permutation:=permutation,
                modelSize:=modelSize,
                pinned:=pinned,
                ignoreTopology:=ignore_topology,
                mutation_rate:=mutation_rate
            )
        End If

        Return result
    End Function

    ''' <summary>
    ''' Matches all of the annotation hits candidates from a given of the mass peak list
    ''' </summary>
    ''' <param name="mz">A numeric vector, the given mass peak list for run candidate search.</param>
    ''' <param name="msData">
    ''' the <see cref="IMzQuery"/> annotation engine, should has the 
    ''' interface function for query annotation candidates by the
    ''' given m/z mass value.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A set of the metabolite ion m/z query candidates result
    ''' </returns>
    <ExportAPI("queryCandidateSet")>
    <RApiReturn(GetType(MzSet))>
    Public Function queryCandidateSet(mz As Double(), msData As Object, Optional env As Environment = Nothing) As Object
        If msData Is Nothing Then
            Return Internal.debug.stop("the given ms compound annotation repository can not be nothing!", env)
        ElseIf msData.GetType.ImplementInterface(Of IMzQuery) Then
            Return DirectCast(msData, IMzQuery).GetCandidateSet(peaks:=mz).ToArray
        Else
            Return Message.InCompatibleType(GetType(IMzQuery), msData.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' Extract all candidates unique id from the given query result
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("candidates_Id")>
    <RApiReturn(GetType(String))>
    Public Function extractCandidateUniqueId(<RRawVectorArgument> q As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of MzSet)(q, env, suppress:=True)
        Dim candidates As New List(Of MzQuery)

        If Not pull.isError Then
            For Each qi As MzSet In pull.populates(Of MzSet)(env)
                If qi.query IsNot Nothing Then
                    Call candidates.AddRange(qi.query)
                End If
            Next
        Else
            pull = pipeline.TryCreatePipeline(Of MzQuery)(q, env)

            If pull.isError Then
                Return pull.getError
            Else
                Call candidates.AddRange(pull.populates(Of MzQuery)(env))
            End If
        End If

        Return candidates _
            .Where(Function(i) Not i Is Nothing) _
            .Select(Function(i) i.unique_id) _
            .Distinct _
            .ToArray
    End Function

    <ExportAPI("createMzset")>
    <RApiReturn(GetType(MzSet))>
    Public Function createMzSet(query As MzQuery(),
                                Optional tolerance As Object = "ppm:20",
                                Optional env As Environment = Nothing) As Object

        Dim mzdiff = Math.getTolerance(tolerance, env)

        If mzdiff Like GetType(Message) Then
            Return mzdiff.TryCast(Of Message)
        End If

        Dim mzgroup As NamedCollection(Of MzQuery)() = query _
            .GroupBy(Function(i) i.mz, AddressOf mzdiff.TryCast(Of Tolerance).Equals) _
            .ToArray
        Dim mzset As MzSet() = mzgroup _
            .AsParallel _
            .Select(Function(mz)
                        Dim mzi As Double = mz.Select(Function(i) i.mz).Average
                        Dim candidates = mz _
                            .GroupBy(Function(i) i.unique_id) _
                            .Select(Function(i) i.First) _
                            .ToArray

                        Return New MzSet With {
                            .mz = mzi,
                            .query = candidates
                        }
                    End Function) _
            .ToArray

        Return mzset
    End Function

    ''' <summary>
    ''' cast background models for ``peakList_annotation`` analysis based on
    ''' a given gsea background model object, this conversion will loose
    ''' all of the network topology information
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns>
    ''' A tuple list object that contains elements inside each slot data:
    ''' 
    ''' 1. name: the pathway map id
    ''' 2. desc: the pathway map names
    ''' 3. model: the network graph object that will be used as the model for run enrichment
    ''' </returns>
    <ExportAPI("fromGseaBackground")>
    Public Function fromGseaBackground(background As Background, Optional min_size As Integer = 3) As list
        Dim gset As New Dictionary(Of String, Object)
        Dim filters = From ci As Cluster In background.clusters Where ci.size >= min_size

        For Each c As Cluster In filters
            gset(c.ID) = New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"name", c.ID},
                    {"desc", c.names},
                    {"model", c.SingularGraph}
                }
            }
        Next

        Return New list With {
            .slots = gset
        }
    End Function

    Friend Class MetabolicNetworkGraph : Inherits MapGraphPopulator

        ReadOnly reactions As ReactionTable()

        Sub New(reactions As IEnumerable(Of Reaction))
            Me.reactions = ReactionTable.Load(reactions).ToArray
        End Sub

        Public Overrides Function CreateGraphModel(map As Map) As NetworkGraph
            Dim allIdSet = map.shapes.mapdata _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim compounds As NamedValue(Of String)() = allIdSet _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .Select(Function(cid) New NamedValue(Of String)(cid, cid, cid)) _
                .ToArray
            Dim currentReactionIdSet As Index(Of String) = allIdSet _
                .Where(Function(id) id.IsPattern("R\d+")) _
                .Indexing
            Dim reactions = Me.reactions _
                .Where(Function(r) r.entry Like currentReactionIdSet) _
                .ToArray

            Return reactions.BuildModel(compounds, enzymaticRelated:=False, ignoresCommonList:=False, enzymeBridged:=True)
        End Function
    End Class

    ''' <summary>
    ''' create kegg pathway network graph background model
    ''' </summary>
    ''' <param name="maps">A collection of the kegg <see cref="Map"/> clr object</param>
    ''' <param name="reactions">A collection of the kegg <see cref="Reaction"/> clr object</param>
    ''' <returns></returns>
    <ExportAPI("kegg_background")>
    Public Function CreateKEGGBackground(maps As Map(), reactions As Reaction(), Optional alternative As Boolean = False) As list
        Dim subgraphs As NamedValue(Of NetworkGraph)()
        Dim networkIndex = reactions _
            .GroupBy(Function(r) r.ID) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)

        If alternative Then
            subgraphs = maps.CreateBackground(New MetabolicNetworkGraph(networkIndex.Values)).ToArray
        Else
            subgraphs = maps.CreateBackground(networkIndex).ToArray
        End If

        Dim graphSet As New list With {
            .slots = New Dictionary(Of String, Object)
        }
        Dim model As list

        For Each graph As NamedValue(Of NetworkGraph) In subgraphs
            model = New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"name", graph.Name},
                    {"desc", graph.Description},
                    {"model", graph.Value}
                }
            }

            Call graphSet.add(graph.Name, model)
        Next

        Return graphSet
    End Function

    <ExportAPI("group_peaks")>
    Public Function GroupPeaks(<RRawVectorArgument>
                               peaktable As Object,
                               <RRawVectorArgument(GetType(String))>
                               Optional adducts As Object = "[M]+|[M+H]+|[M+H2O]+|[M+H-H2O]+",
                               Optional isotopic_max As Integer = 5,
                               Optional mzdiff As Double = 0.01,
                               Optional delta_rt As Double = 3,
                               Optional env As Environment = Nothing) As Object

        Dim peakSet As [Variant](Of Message, Peaktable()) = Math.GetPeakList(peaktable, env)
        Dim precursors As MzCalculator() = Math.GetPrecursorTypes(adducts, env)

        If peakSet Like GetType(Message) Then
            Return peakSet.TryCast(Of Message)
        End If

        Dim alg As New PeakCorrelation(precursors, isotopic_max)
        Dim groups As PeakQuery(Of Peaktable)() = alg _
            .FindExactMass(peakSet.TryCast(Of Peaktable()), delta_rt, mzdiff) _
            .OrderByDescending(Function(m) m.size) _
            .ToArray

        Return groups
    End Function
End Module
