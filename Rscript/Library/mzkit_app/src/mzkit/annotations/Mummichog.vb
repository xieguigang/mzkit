#Region "Microsoft.VisualBasic::0874c8f39e33a68431f137f21a535eec, Rscript\Library\mzkit_app\src\mzkit\annotations\Mummichog.vb"

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

'   Total Lines: 415
'    Code Lines: 298 (71.81%)
' Comment Lines: 65 (15.66%)
'    - Xml Docs: 96.92%
' 
'   Blank Lines: 52 (12.53%)
'     File Size: 17.46 KB


' Module Mummichog
' 
'     Function: CreateKEGGBackground, createMzSet, extractCandidateUniqueId, fromGseaBackground, getResultTable
'               GroupPeaks, mzScore, PeakListAnnotation, queryCandidateSet
' 
'     Sub: Main
'     Class MetabolicNetworkGraph
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateGraphModel
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
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
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Model.Network.KEGG
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Mummichog searches for enrichment patterns on metabolic network, 
''' bypassing metabolite identification, to generate high-quality
''' hypotheses directly from a LC-MS data table.
''' </summary>
<Package("Mummichog")>
<RTypeExport("mummichog_pars", GetType(MummichogParams))>
Module Mummichog

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PathwayEnrichmentResult()), AddressOf getResultTable)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(PathwayEnrichment()), AddressOf castResultDataframe)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(MetaboliteResult()), AddressOf mzScore)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function castResultDataframe(result As PathwayEnrichment(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result _
                .Select(Function(p) p.Pathway_ID) _
                .ToArray
        }

        Call df.add("Pathway_ID", From p As PathwayEnrichment In result Select p.Pathway_ID)
        Call df.add("Pathway_Name", From p As PathwayEnrichment In result Select p.Pathway_Name)
        Call df.add("PathwaySize", From p As PathwayEnrichment In result Select p.PathwaySize)
        Call df.add("SignificantHits", From p As PathwayEnrichment In result Select p.SignificantHits)
        Call df.add("BackgroundHits", From p As PathwayEnrichment In result Select p.BackgroundHits)
        Call df.add("TotalSignificant", From p As PathwayEnrichment In result Select p.TotalSignificant)
        Call df.add("TotalBackground", From p As PathwayEnrichment In result Select p.TotalBackground)
        Call df.add("PValue", From p As PathwayEnrichment In result Select p.PValue)
        Call df.add("FDR", From p As PathwayEnrichment In result Select p.FDR)
        Call df.add("Score", From p As PathwayEnrichment In result Select p.Score)
        Call df.add("IsSignificant", From p As PathwayEnrichment In result Select p.IsSignificant)

        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function getResultTable(result As PathwayEnrichmentResult(), args As list, env As Environment) As dataframe
        Return castResultDataframe(MummichogAnnotator.PathwayResultsToDataTable(result).ToArray, args, env)
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
    <RGenericOverloads("as.data.frame")>
    Public Function mzScore(result As MetaboliteResult(),
                            <RListObjectArgument>
                            Optional args As list = Nothing,
                            Optional env As Environment = Nothing) As dataframe

        Dim scores As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result _
                .Select(Function(a) a.Rank & "." & a.Peak_ID) _
                .ToArray
        }

        Call scores.add("Rank", From r As MetaboliteResult In result Select r.Rank)
        Call scores.add("Peak_ID", From r As MetaboliteResult In result Select r.Peak_ID)
        Call scores.add("m/z", From r As MetaboliteResult In result Select r.mz)
        Call scores.add("rt", From r As MetaboliteResult In result Select r.rt)
        Call scores.add("PValue", From r As MetaboliteResult In result Select r.PValue)
        Call scores.add("KEGG_ID", From r As MetaboliteResult In result Select r.KEGG_ID)
        Call scores.add("Metabolite_Name", From r As MetaboliteResult In result Select r.Metabolite_Name)
        Call scores.add("Formula", From r As MetaboliteResult In result Select r.Formula)
        Call scores.add("Adduct", From r As MetaboliteResult In result Select r.Adduct)
        Call scores.add("PpmError", From r As MetaboliteResult In result Select r.PpmError)
        Call scores.add("SignificantPathways", From r As MetaboliteResult In result Select r.SignificantPathways)
        Call scores.add("PathwayScore", From r As MetaboliteResult In result Select r.PathwayScore)
        Call scores.add("AdductConsistencyScore", From r As MetaboliteResult In result Select r.AdductConsistencyScore)
        Call scores.add("IsotopeScore", From r As MetaboliteResult In result Select r.IsotopeScore)
        Call scores.add("MassAccuracyScore", From r As MetaboliteResult In result Select r.MassAccuracyScore)
        Call scores.add("DetectedAdducts", From r As MetaboliteResult In result Select r.DetectedAdducts)
        Call scores.add("IsotopeDetails", From r As MetaboliteResult In result Select r.IsotopeDetails)
        Call scores.add("PriorityScore", From r As MetaboliteResult In result Select r.PriorityScore)
        Call scores.add("ConfidenceLevel", From r As MetaboliteResult In result Select r.ConfidenceLevel)

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
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("peakList_annotation")>
    <RApiReturn("enrichment", "metabolites")>
    Public Function PeakListAnnotation(background As MummichogAnnotator,
                                       <RRawVectorArgument> peaks As Object,
                                       <RRawVectorArgument> sampleinfo As Object,
                                       Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of xcms2)(peaks, env, suppress:=True)
        Dim sampleIds As pipeline = pipeline.TryCreatePipeline(Of SampleInfo)(sampleinfo, env, suppress:=True)

        If TypeOf peaks Is PeakSet Then
            peaks = DirectCast(peaks, PeakSet).peaks
        ElseIf pull.isError Then
            Return pull.getError
        End If

        Dim groups = DataGroup.CreateDataGroups(sampleIds.populates(Of SampleInfo)(env)).ToArray
        Dim peaktable = pull.populates(Of xcms2)(env).ToArray
        Dim metabo = background.Annotate(peaktable, groups).ToArray
        Dim enrich = background.PathwayResults.ToArray

        Return New list(
            slot("enrichment") = enrich,
            slot("metabolites") = metabo
        )
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
            Return RInternal.debug.stop("the given ms compound annotation repository can not be nothing!", env)
        ElseIf msData.GetType.ImplementInterface(Of IMzQuery) Then
            Return MzSet.GetCandidateSet(DirectCast(msData, IMzQuery), peaks:=mz).ToArray
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

    ''' <summary>
    ''' Create a graph model for run Mummichog annotation without 
    ''' any network topology information
    ''' </summary>
    ''' <param name="cluster">
    ''' One of the cluster inside a gsea <see cref="Background"/> model
    ''' </param>
    ''' <returns>
    ''' just create a graph with node set, no edges
    ''' </returns>
    <Extension>
    Public Function SingularGraph(cluster As Cluster) As NetworkGraph
        Dim g As New NetworkGraph
        Dim uniqs As IEnumerable(Of BackgroundGene) = cluster.members _
        .GroupBy(Function(a) a.accessionID) _
        .Select(Function(d) d.First)
        Dim metadata As NodeData

        For Each member As BackgroundGene In uniqs
            metadata = New NodeData With {
            .label = member.name,
            .origID = member.accessionID
        }
            g.CreateNode(member.accessionID, metadata)
        Next

        ' just create a graph with node set, no edges
        Return g
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

    <ExportAPI("kegg_background")>
    <RApiReturn(GetType(MummichogAnnotator))>
    Public Function CreateKEGGBackground(<RRawVectorArgument> metabolites As Object, <RRawVectorArgument> pathways As Object, Optional params As MummichogParams = Nothing, Optional env As Environment = Nothing) As Object
        Dim pullMetab As pipeline = pipeline.TryCreatePipeline(Of Compound)(metabolites, env)
        Dim pullMaps As pipeline = pipeline.TryCreatePipeline(Of Map)(pathways, env)

        If pullMetab.isError Then
            Return pullMetab.getError
        ElseIf pullMaps.isError Then
            Return pullMaps.getError
        End If

        Dim metabSet As IEnumerable(Of KEGGMetabolite) = pullMetab _
            .populates(Of Compound)(env) _
            .Select(Function(c)
                        Return New KEGGMetabolite With {
                            .CommonName = c.commonNames.DefaultFirst(c.formula),
                            .ExactMass = c.exactMass,
                            .Formula = c.formula,
                            .Id = c.entry
                        }
                    End Function)
        Dim mapSet As IEnumerable(Of KEGGPathway) = pullMaps _
            .populates(Of Map)(env) _
            .Select(Function(map)
                        Return New KEGGPathway With {
                            .ID = map.EntryId,
                            .Name = map.name,
                            .Description = map.description,
                            .Metabolites = New HashSet(Of String)(map.GetMembers.Where(Function(id) id.IsPattern("C\d{5}")))
                        }
                    End Function)

        Return New MummichogAnnotator(metabSet, mapSet, params)
    End Function

    ''' <summary>
    ''' create kegg pathway network graph background model
    ''' </summary>
    ''' <param name="maps">A collection of the kegg <see cref="Map"/> clr object</param>
    ''' <param name="reactions">A collection of the kegg <see cref="Reaction"/> clr object</param>
    ''' <returns></returns>
    <ExportAPI("kegg_graph")>
    Public Function CreateKEGGBackground(maps As Map(), reactions As Reaction(), Optional alternative As Boolean = False) As list
        Dim subgraphs As NamedValue(Of NetworkGraph)()
        Dim networkIndex = reactions _
            .GroupBy(Function(r) r.ID) _
            .ToDictionary(Function(r) r.Key,
                          Function(r)
                              Return r.First
                          End Function)

        If alternative Then
            subgraphs = GraphBackground.CreateBackground(maps, New MetabolicNetworkGraph(networkIndex.Values)).ToArray
        Else
            subgraphs = GraphBackground.CreateBackground(maps, networkIndex).ToArray
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
