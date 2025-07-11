﻿#Region "Microsoft.VisualBasic::b908336dff318d0aa20b15e7c8fa703c, Rscript\Library\mzkit_app\src\mzDIA\metaDNA.vb"

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

    '   Total Lines: 789
    '    Code Lines: 517 (65.53%)
    ' Comment Lines: 173 (21.93%)
    '    - Xml Docs: 98.27%
    ' 
    '   Blank Lines: 99 (12.55%)
    '     File Size: 34.58 KB


    ' Module metaDNAInfer
    ' 
    '     Function: CreateKEGGSearch, DIAInfer, ExportNetwork, getResultTable, handleSample
    '               InferTable, loadCompoundLibrary, loadKeggNetwork, loadMetaDNAInferNetwork, loadOntologyTree
    '               MetaDNAAlgorithm, MgfSeeds, readInferDetails, readReactionClassTable, ResultAlignments
    '               ResultTable, SaveAlgorithmPerfermance, SetInferNetwork, SetKeggLibrary, setLibrary
    '               setNetworking, SetSearchRange
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep
Imports BioNovoGene.BioDeep.Chemistry.ChEBI
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MetaDNA.Visual
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.Object.Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports KeggCompound = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound
Imports kegReaction = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction
Imports kegReactionClass = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.ReactionClass
Imports metadata = BioNovoGene.BioDeep.Chemistry.MetaLib.Models.MetaInfo
Imports MetaDNAAlgorithm = BioNovoGene.BioDeep.MetaDNA.Algorithm
Imports ReactionClass = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.ReactionClass
Imports ReactionClassTbl = BioNovoGene.BioDeep.MetaDNA.Visual.ReactionClass
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math

''' <summary>
''' Metabolic Reaction Network-based Recursive Metabolite Annotation for Untargeted Metabolomics
''' </summary>
<Package("metadna",
         Category:=APICategories.ResearchTools,
         Cites:="X. Shen, R. Wang, X. Xiong, Y. Yin, Y. Cai, Z. Ma, N. Liu, and Z.-J. Zhu* (Corresponding Author), 
         Metabolic Reaction Network-based Recursive Metabolite Annotation for Untargeted Metabolomics, 
         Nature Communications, 
         2019, 10: 1516.")>
<RTypeExport("metadna", GetType(MetaDNAAlgorithm))>
<RTypeExport("obo_ontology", GetType(OBOFile))>
<RTypeExport("metadna_infer", GetType(CandidateInfer))>
Module metaDNAInfer

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(MetaDNAResult()), AddressOf getResultTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function getResultTable(list As MetaDNAResult(), args As list, env As Environment) As dataframe
        Dim data As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        data.columns(NameOf(MetaDNAResult.ROI_id)) = list.Select(Function(i) i.ROI_id).ToArray
        data.columns(NameOf(MetaDNAResult.query_id)) = list.Select(Function(i) i.query_id).ToArray
        data.columns(NameOf(MetaDNAResult.mz)) = list.Select(Function(i) i.mz).ToArray
        data.columns(NameOf(MetaDNAResult.rt)) = list.Select(Function(i) i.rt).ToArray
        data.columns(NameOf(MetaDNAResult.intensity)) = list.Select(Function(i) i.intensity).ToArray
        data.columns(NameOf(MetaDNAResult.ppm)) = list.Select(Function(i) i.ppm).ToArray
        data.columns(NameOf(MetaDNAResult.KEGGId)) = list.Select(Function(i) i.KEGGId).ToArray
        data.columns(NameOf(MetaDNAResult.precursorType)) = list.Select(Function(i) i.precursorType).ToArray
        data.columns(NameOf(MetaDNAResult.name)) = list.Select(Function(i) i.name).ToArray
        data.columns(NameOf(MetaDNAResult.formula)) = list.Select(Function(i) i.formula).ToArray
        data.columns(NameOf(MetaDNAResult.exactMass)) = list.Select(Function(i) i.exactMass).ToArray
        data.columns(NameOf(MetaDNAResult.mzCalc)) = list.Select(Function(i) i.mzCalc).ToArray

        data.columns(NameOf(MetaDNAResult.inferLevel)) = list.Select(Function(i) i.inferLevel).ToArray
        data.columns(NameOf(MetaDNAResult.inferSize)) = list.Select(Function(i) i.inferSize).ToArray
        data.columns(NameOf(MetaDNAResult.forward)) = list.Select(Function(i) i.forward).ToArray
        data.columns(NameOf(MetaDNAResult.reverse)) = list.Select(Function(i) i.reverse).ToArray
        data.columns(NameOf(MetaDNAResult.jaccard)) = list.Select(Function(i) i.jaccard).ToArray
        data.columns(NameOf(MetaDNAResult.mirror)) = list.Select(Function(i) i.mirror).ToArray
        data.columns(NameOf(MetaDNAResult.score1)) = list.Select(Function(i) i.score1).ToArray
        data.columns(NameOf(MetaDNAResult.score2)) = list.Select(Function(i) i.score2).ToArray
        data.columns(NameOf(MetaDNAResult.pvalue)) = list.Select(Function(i) i.pvalue).ToArray

        data.columns(NameOf(MetaDNAResult.seed)) = list.Select(Function(i) i.seed).ToArray
        data.columns(NameOf(MetaDNAResult.parentTrace)) = list.Select(Function(i) i.parentTrace).ToArray
        data.columns(NameOf(MetaDNAResult.partnerKEGGId)) = list.Select(Function(i) i.partnerKEGGId).ToArray
        data.columns(NameOf(MetaDNAResult.reaction)) = list.Select(Function(i) i.reaction).ToArray
        data.columns(NameOf(MetaDNAResult.KEGG_reaction)) = list.Select(Function(i) i.KEGG_reaction).ToArray

        Return data
    End Function

    ''' <summary>
    ''' Load network graph model from the kegg metaDNA infer network data.
    ''' </summary>
    ''' <param name="debugOutput"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>read data;metadna</keywords>
    <ExportAPI("read.metadna.infer")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function loadMetaDNAInferNetwork(debugOutput As Object, Optional env As Environment = Nothing) As Object
        If debugOutput Is Nothing Then
            Return Nothing
        ElseIf debugOutput.GetType Is GetType(String) Then
            debugOutput = DirectCast(debugOutput, String).LoadXml(Of MetaDNA.Visual.XML)
        End If

        If Not TypeOf debugOutput Is MetaDNA.Visual.XML Then
            Return REnv.Internal.debug.stop(New InvalidCastException, env)
        End If

        Return DirectCast(debugOutput, MetaDNA.Visual.XML).CreateGraph
    End Function

    ''' <summary>
    ''' load kegg reaction class data in table format from given file
    ''' </summary>
    ''' <param name="file">csv table file or a directory with raw xml model data file in it.</param>
    ''' <param name="env"></param>
    ''' <returns>A collection of the reaction class table for provides 
    ''' the data links between the compounds.</returns>
    ''' <keywords>read data;kegg</keywords>
    <ExportAPI("reaction_class.table")>
    <RApiReturn(GetType(ReactionClassTbl))>
    Public Function readReactionClassTable(file As String, Optional env As Environment = Nothing) As Object
        If file.ExtensionSuffix("csv") Then
            Return file.LoadCsv(Of ReactionClassTbl).ToArray
        ElseIf file.DirectoryExists Then
            Return kegReactionClass _
                .ScanRepository(file, loadsAll:=True) _
                .Select(Function(cls)
                            Return cls.reactantPairs _
                                .Select(Function(r)
                                            Return New ReactionClassTbl With {
                                                .define = cls.definition,
                                                .from = r.from,
                                                .[to] = r.to,
                                                .rId = cls.entryId,
                                                .category = Integer.Parse(cls.category.Match("\d"))
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL _
                .ToArray
        Else
            Return RInternal.debug.stop($"unable to determin the data source type of the given file '{file}'", env)
        End If
    End Function

#Region "metadna algorithm"

    ''' <summary>
    ''' Create an algorithm module for run metaDNA inferance
    ''' </summary>
    ''' <param name="ms1ppm">the mass tolerance error for matches the ms1 ion</param>
    ''' <param name="mzwidth"></param>
    ''' <param name="dotcutoff">
    ''' network propagation score cutoff, could be lower to 0.4 ~ 0.5.
    ''' </param>
    ''' <param name="allowMs1"></param>
    ''' <param name="maxIterations"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>metadna</keywords>
    <ExportAPI("metadna")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function MetaDNAAlgorithm(Optional ms1ppm As Object = "ppm:20",
                                     Optional mzwidth As Object = "da:0.3",
                                     Optional dotcutoff As Double = 0.5,
                                     Optional allowMs1 As Boolean = True,
                                     Optional maxIterations As Integer = 1000,
                                     Optional debug As Boolean = False,
                                     Optional env As Environment = Nothing) As Object

        Dim ms1Err As [Variant](Of Tolerance, Message) = Math.getTolerance(ms1ppm, env)
        Dim mz2Err As [Variant](Of Tolerance, Message) = Math.getTolerance(mzwidth, env)

        If ms1Err Like GetType(Message) Then
            Return ms1Err.TryCast(Of Message)
        ElseIf mz2Err Like GetType(Message) Then
            Return mz2Err.TryCast(Of Message)
        End If

        Return New MetaDNAAlgorithm(ms1Err, dotcutoff, mz2Err, allowMs1,
            maxIterations:=maxIterations,
            debug:=debug)
    End Function

    ''' <summary>
    ''' Configs the precursor adducts range for the metaDNA algorithm
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="precursorTypes">a collection of the ms1 precursor adducts type data,
    ''' could be a character vector of the adducts type string.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>adducts</keywords>
    <ExportAPI("range")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function SetSearchRange(metadna As Algorithm,
                                   <RRawVectorArgument>
                                   precursorTypes As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim types As String() = CLRVector.asCharacter(precursorTypes)

        If env.globalEnvironment.options.verbose Then
            Call base.print("Set precursor types:", , env)
            Call base.print(types, , env)
        End If

        Return metadna.SetSearchRange(types)
    End Function

    ''' <summary>
    ''' Set kegg compound library
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="kegg">
    ''' should be a collection of the <see cref="KeggCompound"/> data,
    ''' or a general <see cref="CompoundSolver"/>.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>kegg;metabolite</keywords>
    <ExportAPI("load.kegg")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function SetKeggLibrary(metadna As Algorithm,
                                   <RRawVectorArgument>
                                   kegg As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim library As pipeline = pipeline.TryCreatePipeline(Of KeggCompound)(kegg, env, suppress:=True)

        If Not library.isError Then
            Return metadna.SetKeggLibrary(library.populates(Of KeggCompound)(env))
        ElseIf TypeOf kegg Is CompoundSolver Then
            Return metadna.SetLibrary(DirectCast(kegg, CompoundSolver))
        Else

        End If

        Return library.getError
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="[library]"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>kegg;metabolite;pubchem;biodeep</keywords>
    <ExportAPI("setLibrary")>
    Public Function setLibrary(metadna As Algorithm, [library] As Object, Optional env As Environment = Nothing) As Object
        If library Is Nothing Then
            Return RInternal.debug.stop("the required compound library should not be nothing!", env)
        End If
        If TypeOf library Is CompoundSolver Then
            Return metadna.SetLibrary(DirectCast(library, CompoundSolver))
        ElseIf library.GetType.IsInheritsFrom(GetType(MSSearch(Of GenericCompound))) Then
            Return metadna.SetLibrary(DirectCast(library, MSSearch(Of GenericCompound)))
        Else
            Return Message.InCompatibleType(GetType(CompoundSolver), library.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="networking"></param>
    ''' <returns></returns>
    ''' <keywords>kegg;reaction;metacyc;reactome;biodeep</keywords>
    <ExportAPI("setNetworking")>
    Public Function setNetworking(metadna As Algorithm, networking As Networking) As Object
        Return metadna.SetNetwork(networking)
    End Function

    ''' <summary>
    ''' set the kegg reaction class data links for the compounds
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="links">
    ''' should be a collection of the <see cref="ReactionClass"/> data
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>kegg;reaction</keywords>
    <ExportAPI("load.kegg_network")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function SetInferNetwork(metadna As Algorithm,
                                    <RRawVectorArgument> links As Object,
                                    Optional env As Environment = Nothing) As Object

        Dim network As pipeline = pipeline.TryCreatePipeline(Of ReactionClass)(links, env)

        If network.isError Then
            network = pipeline.TryCreatePipeline(Of kegReaction)(links, env)

            If Not network.isError Then
                Return metadna.SetNetwork(network.populates(Of kegReaction)(env))
            End If

            Return network.getError
        End If

        Return metadna.SetNetwork(network.populates(Of ReactionClass)(env))
    End Function

    ''' <summary>
    ''' load the ontology tree as the network graph for search
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="obo">raw data for build <see cref="OntologyTree"/>.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>networking</keywords>
    <ExportAPI("load.ontology")>
    Public Function loadOntologyTree(metadna As Algorithm, obo As OBOFile, Optional env As Environment = Nothing) As Object
        Return metadna.SetNetwork(New OntologyTree(obo))
    End Function

    ''' <summary>
    ''' set ms2 spectrum data for run the annotation
    ''' </summary>
    ''' <param name="metadna"></param>
    ''' <param name="sample">
    ''' a collection of the mzkit peak ms2 data objects
    ''' </param>
    ''' <param name="peaktable">
    ''' used for generates the ROI id for matches with the ms1 peaks data 
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>spectrum;lc-ms</keywords>
    <ExportAPI("load.raw")>
    <RApiReturn(GetType(MetaDNAAlgorithm))>
    Public Function handleSample(metadna As Algorithm, <RRawVectorArgument> sample As Object,
                                 <RRawVectorArgument>
                                 Optional peaktable As Object = Nothing,
                                 Optional ms1diff As Double = 0.1,
                                 Optional rt_win As Double = 30,
                                 Optional env As Environment = Nothing) As Object
        Dim raw As pipeline
        Dim peakSet As pipeline = Nothing

        If Not peaktable Is Nothing Then
            peakSet = pipeline.TryCreatePipeline(Of xcms2)(peaktable, env)

            If peakSet.isError AndAlso TypeOf peaktable Is dataframe Then
                peakSet = pipeline.CreateFromPopulator(
                    Iterator Function() As IEnumerable(Of xcms2)
                        Dim df As dataframe = DirectCast(peaktable, dataframe)
                        Dim mz As Double() = df.getVector(Of Double)("mz", "MZ", "m/z", "mass to charge")
                        Dim rt As Double() = df.getVector(Of Double)("rt", "RT", "retention time", "retention_time")
                        Dim xcms_id As String() = df.getVector(Of String)("xcms_id", "id", "ID", "roi", "ROI")

                        For i As Integer = 0 To xcms_id.Length - 1
                            Yield New xcms2(xcms_id(i), mz(i), rt(i))
                        Next
                    End Function())
            End If
            If peakSet.isError Then
                Return peakSet.getError
            End If
        End If

        If TypeOf sample Is list Then
            sample = RConversion.unlist(sample, env:=env)

            If Program.isException(sample) Then
                Return sample
            End If
        End If

        raw = pipeline.TryCreatePipeline(Of PeakMs2)(sample, env, suppress:=True)

        If raw.isError Then
            raw = pipeline.TryCreatePipeline(Of mzPack)(sample, env)

            If raw.isError Then
                Return raw.getError
            End If

            raw = pipeline.CreateFromPopulator(
                Iterator Function() As IEnumerable(Of PeakMs2)
                    For Each rawdata As mzPack In raw.populates(Of mzPack)(env)
                        For Each peak As PeakMs2 In rawdata.GetMs2Peaks
                            Yield peak
                        Next
                    Next
                End Function().ToArray)
        End If

        Dim pool As PeakMs2() = raw.populates(Of PeakMs2)(env).ToArray

        If pool.Length = 0 Then
            Return RInternal.debug.stop("the required sample data collection should not be empty!", env)
        End If

        If Not peakSet Is Nothing Then
            Dim peaksdata As New PeakSet(peakSet.populates(Of xcms2)(env))
            Dim println = env.WriteLineHandler

            Call println("set ms2 peak data associated ROI id from the ms1 peaktable data!")

            ' assign ms1 peak id to the ms2 spectrum
            For i As Integer = 0 To pool.Length - 1
                Dim peak2 As PeakMs2 = pool(i)
                Dim peak1 = peaksdata.FindIonSet(peak2.mz, peak2.rt, ms1diff, rt_win).ToArray
                Dim xcms_id As String = Nothing

                If Not peak1.IsNullOrEmpty Then
                    If peak1.Length = 1 Then
                        xcms_id = peak1(0).ID
                    Else
                        With peak1 _
                            .OrderBy(Function(p1)
                                         Return std.Abs(p1.mz - peak2.mz + 0.0001) * std.Abs(p1.rt - peak2.rt + 0.1)
                                     End Function) _
                            .First

                            xcms_id = .ID
                        End With
                    End If
                End If

                pool(i) = Algorithm.SimpleSetROI(peak2, xcms_id)
            Next
        End If

        Return metadna.SetSamples(pool, autoROIid:=peakSet Is Nothing)
    End Function

    ''' <summary>
    ''' apply of the metadna annotation workflow
    ''' </summary>
    ''' <param name="metaDNA"></param>
    ''' <param name="sample"></param>
    ''' <param name="seeds"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>metadna;lc-ms</keywords>
    <ExportAPI("DIA.infer")>
    <RApiReturn(GetType(CandidateInfer))>
    Public Function DIAInfer(metaDNA As Algorithm,
                             <RRawVectorArgument> Optional sample As Object = Nothing,
                             <RRawVectorArgument> Optional seeds As Object = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim raw As pipeline = If(sample Is Nothing, Nothing, pipeline.TryCreatePipeline(Of PeakMs2)(sample, env))
        Dim infer As CandidateInfer()

        If seeds Is Nothing Then
            If Not raw Is Nothing Then
                If raw.isError Then
                    Return raw.getError
                End If

                Call metaDNA.SetSamples(raw.populates(Of PeakMs2)(env))
            End If

            infer = metaDNA.DIASearch.ToArray
        ElseIf TypeOf seeds Is dataframe Then
            infer = DirectCast(seeds, dataframe).InferTable(raw, metaDNA, env)
        Else
            Throw New NotImplementedException
        End If

        Return infer
    End Function

    <Extension>
    Private Function InferTable(seeds As dataframe, raw As pipeline, metaDNA As Algorithm, env As Environment) As CandidateInfer()
        Dim id As String() = DirectCast(seeds, dataframe).getColumnVector(1)
        Dim kegg_id As String() = DirectCast(seeds, dataframe).getColumnVector(2)
        Dim rawFile As UnknownSet = UnknownSet.CreateTree(raw.populates(Of PeakMs2)(env), metaDNA.ms1Err)

        If rawFile.is_empty Then
            Call VBDebugger.EchoLine("no spectrum feature for run annotation, returns empty result collection!")
            Return {}
        End If

        Dim annoSet As NamedValue(Of String)() = id _
            .Select(Function(uid, i) (uid, kegg_id(i))) _
            .GroupBy(Function(map) map.uid) _
            .Select(Function(map)
                        Return map _
                            .GroupBy(Function(anno) anno.Item2) _
                            .Select(Function(anno)
                                        Return New NamedValue(Of String) With {
                                            .Name = map.Key,
                                            .Value = anno.Key
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .Where(Function(map)
                       Return map.Value.IsPattern("C\d+")
                   End Function) _
            .ToArray
        Dim seedsRaw As AnnotatedSeed()

        If env.globalEnvironment.options.verbose Then
            Call base.print("Create seeds by dataframe...", , env)
        End If

        seedsRaw = rawFile.CreateAnnotatedSeeds(annoSet).ToArray

        If env.globalEnvironment.options.verbose Then
            Call base.print($"We create {seedsRaw.Length} seeds for running metaDNA algorithm!", , env)
        End If

        Return metaDNA _
            .SetSamples(rawFile) _
            .DIASearch(seedsRaw) _
            .ToArray
    End Function

    ''' <summary>
    ''' create seeds from mgf file data
    ''' </summary>
    ''' <param name="seeds">A set of the mzkit <see cref="PeakMs2"/> clr object that could 
    ''' be used for the seeds for run the metadna annotation.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <keywords>spectrum</keywords>
    <ExportAPI("as.seeds")>
    <RApiReturn(GetType(AnnotatedSeed))>
    Public Function MgfSeeds(<RRawVectorArgument> seeds As Object, Optional env As Environment = Nothing) As Object
        Dim seedList As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(seeds, env)

        If seedList.isError Then
            Return seedList.getError
        End If

        Return seedList _
            .populates(Of PeakMs2)(env) _
            .MgfSeeds _
            .ToArray
    End Function

#End Region

#Region "result output"

    ''' <summary>
    ''' get result alignments raw data for data plots.
    ''' </summary>
    ''' <param name="DIAinfer">the result candidates of clr data type in mzkit: <see cref="CandidateInfer"/></param>
    ''' <param name="table">the <see cref="MetaDNAResult"/> data table</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("result.alignment")>
    <RApiReturn(GetType(MetaDNARawSet))>
    Public Function ResultAlignments(<RRawVectorArgument> DIAinfer As Object,
                                     <RRawVectorArgument> table As Object,
                                     Optional env As Environment = Nothing) As Object

        Dim raw As pipeline = pipeline.TryCreatePipeline(Of CandidateInfer)(DIAinfer, env)
        Dim filter As pipeline = pipeline.TryCreatePipeline(Of MetaDNAResult)(table, env)

        If raw.isError Then
            Return raw.getError
        ElseIf filter.isError Then
            Return filter.getError
        End If

        Dim rawFilter As MetaDNAResult() = filter.populates(Of MetaDNAResult)(env).ToArray

        Return raw _
            .populates(Of CandidateInfer)(env) _
            .ExportInferRaw(rawFilter)
    End Function

    ''' <summary>
    ''' Extract the annotation result from metaDNA algorithm module as data table
    ''' </summary>
    ''' <param name="metaDNA"></param>
    ''' <param name="result">a collection of the <see cref="CandidateInfer"/>.</param>
    ''' <param name="unique"></param>
    ''' <param name="cutoff">
    ''' the score cutoff for filter the result list
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>A collection of the <see cref="MetaDNAResult"/> data objects that could be
    ''' used for represented as the result table.</returns>
    <ExportAPI("as.table")>
    <RApiReturn(GetType(MetaDNAResult))>
    Public Function ResultTable(metaDNA As Algorithm,
                                <RRawVectorArgument>
                                result As Object,
                                Optional unique As Boolean = False,
                                Optional cutoff As Double = 0.75,
                                Optional env As Environment = Nothing) As Object

        Dim data As pipeline = pipeline.TryCreatePipeline(Of CandidateInfer)(result, env)

        If data.isError Then
            Return data.getError
        End If

        Return metaDNA _
            .ExportTable(data.populates(Of CandidateInfer)(env), unique) _
            .DoCall(Function(a)
                        Return MetaDNAResult.FilterInferenceHits(a, cutoff)
                    End Function) _
            .ToArray
    End Function

    <ExportAPI("as.graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function ExportNetwork(<RRawVectorArgument> result As Object, Optional env As Environment = Nothing) As Object
        Dim data As pipeline = pipeline.TryCreatePipeline(Of MetaDNAResult)(result, env)

        If data.isError Then
            Return data.getError
        End If

        Return data.populates(Of MetaDNAResult)(env).ExportNetwork
    End Function

    <ExportAPI("as.ticks")>
    Public Function SaveAlgorithmPerfermance(metaDNA As Algorithm) As dataframe
        Dim counter = metaDNA.GetPerfermanceCounter
        Dim iteration As Integer() = counter.Select(Function(c) c.iteration).ToArray
        Dim ticks As String() = counter.Select(Function(c) c.ticks.FormatTime).ToArray
        Dim inferLinks As Integer() = counter.Select(Function(c) c.inferLinks).ToArray
        Dim seeding As Integer() = counter.Select(Function(c) c.seeding).ToArray
        Dim candidates As Integer() = counter.Select(Function(c) c.candidates).ToArray

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {NameOf(iteration), iteration},
                {NameOf(ticks), ticks},
                {NameOf(inferLinks), inferLinks},
                {NameOf(seeding), seeding},
                {NameOf(candidates), candidates}
            }
        }
    End Function
#End Region

    <ExportAPI("read.infer_details")>
    Public Function readInferDetails(file As String) As CandidateInfer()
        Return file.LoadJsonFile(Of CandidateInfer())
    End Function

#Region "kegg"

    ''' <summary>
    ''' Create the kegg compound ms1 annotation query engine.
    ''' </summary>
    ''' <param name="kegg">
    ''' a set of kegg/pubchem/chebi/hmdb compound data.
    ''' </param>
    ''' <param name="precursors">
    ''' a character vector of the ms1 precursor ion names or 
    ''' a list of the given mzcalculator object models.
    ''' </param>
    ''' <param name="mzdiff">
    ''' the mass tolerance value to match between the 
    ''' experiment m/z value and the reference m/z value
    ''' which is calculated from the compound exact mass
    ''' with a given specific ion precursor type.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a data query engine model to run ms1 data search 
    ''' for the kegg metaolite compounds.
    ''' </returns>
    ''' <keywords>kegg;metabolites</keywords>
    <ExportAPI("annotationSet")>
    <RApiReturn(GetType(CompoundSolver))>
    Public Function CreateKEGGSearch(<RRawVectorArgument> kegg As Object,
                                     <RRawVectorArgument()>
                                     Optional precursors As Object = "[M]+|[M+H]+|[M+H-H2O]+",
                                     Optional mzdiff As Object = "ppm:20",
                                     <RRawVectorArgument(TypeCodes.string)>
                                     Optional excludes As Object = Nothing,
                                     <RRawVectorArgument(TypeCodes.double)>
                                     Optional mass_range As Object = Nothing,
                                     Optional env As Environment = Nothing) As Object

        Dim keggSet = pipeline.TryCreatePipeline(Of KeggCompound)(kegg, env, suppress:=True)
        Dim mzErr = Math.getTolerance(mzdiff, env)
        Dim calculators As MzCalculator() = Math.GetPrecursorTypes(precursors, env)
        Dim excludesEntry As Index(Of String) = CLRVector.asCharacter(excludes).Indexing
        Dim mz_range As Double() = CLRVector.asNumeric(mass_range)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        If Not keggSet.isError Then
            Return CompoundSolver.CreateIndex(
                compounds:=keggSet _
                    .populates(Of KeggCompound)(env) _
                    .Where(Function(c)
                               Return Not c.entry Like excludesEntry
                           End Function),
                types:=calculators,
                tolerance:=mzErr.TryCast(Of Tolerance),
                mass_range:=If(mz_range.IsNullOrEmpty, Nothing, New DoubleRange(mz_range))
            )
        End If

        If TypeOf kegg Is OBOFile Then
            keggSet = pipeline.CreateFromPopulator(ChEBIObo.ImportsMetabolites(DirectCast(kegg, OBOFile)))
        Else
            keggSet = pipeline.TryCreatePipeline(Of metadata)(kegg, env)
        End If

        If Not keggSet.isError Then
            Dim pull = keggSet.populates(Of metadata)(env).ToArray
            Dim filter = pull.Where(Function(c) Not c.ID Like excludesEntry).ToArray

            Return CompoundSolver.CreateIndex(
                compounds:=filter,
                types:=calculators,
                tolerance:=mzErr.TryCast(Of Tolerance),
                mass_range:=If(mz_range.IsNullOrEmpty, Nothing, New DoubleRange(mz_range))
            )
        End If

        Return keggSet.getError
    End Function

    ''' <summary>
    ''' load kegg compounds
    ''' </summary>
    ''' <param name="repo">
    ''' the file path to the messagepack data repository
    ''' </param>
    ''' <returns>
    ''' a collection of the kegg compound data model
    ''' </returns>
    ''' <keywords>kegg;metabolites</keywords>
    <ExportAPI("kegg.library")>
    <RApiReturn(GetType(KeggCompound))>
    Public Function loadCompoundLibrary(repo As String) As Object
        If repo.FileExists Then
            Using file As Stream = repo.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return KEGGCompoundPack.ReadKeggDb(file)
            End Using
        Else
            Return CompoundRepository _
                .ScanRepository(repo, ignoreGlycan:=False) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If
    End Function

    ''' <summary>
    ''' load the kegg reaction class data.
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    ''' <keywords>kegg;networking</keywords>
    <ExportAPI("kegg.network")>
    <RApiReturn(GetType(ReactionClass))>
    Public Function loadKeggNetwork(repo As String) As Object
        If repo.FileExists Then
            Using file As Stream = repo.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return ReactionClassPack.ReadKeggDb(file)
            End Using
        Else
            Return ReactionClass _
                .ScanRepository(repo) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If
    End Function

#End Region

End Module
