#Region "Microsoft.VisualBasic::f1f076a6f6c69a663dfe5b72b43baaa7, Rscript\Library\mzkit_app\src\mzDIA\MoleculeNetworking.vb"

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

    '   Total Lines: 508
    '    Code Lines: 300 (59.06%)
    ' Comment Lines: 152 (29.92%)
    '    - Xml Docs: 96.71%
    ' 
    '   Blank Lines: 56 (11.02%)
    '     File Size: 22.28 KB


    ' Module MoleculeNetworking
    ' 
    '     Function: clustering, create_spectrum_grid, createGraph, grid_assigned, makePeakAssignTable
    '               MsBin, RepresentativeSpectrum, splitClusterRT, Tree, unpack_assign
    '               unqiueNames
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.BinaryDumping
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Internal.[Object].Converts
Imports SMRUCC.Rsharp.Runtime.Interop
Imports rDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math

''' <summary>
''' Molecular Networking (MN) is a computational strategy that may help visualization and interpretation of the complex data arising from MS analysis. 
''' </summary>
''' <remarks>
''' MN is able to identify potential similarities among all MS/MS spectra within 
''' the dataset and to propagate annotation to unknown but related molecules 
''' (Wang et al., 2016). This approach exploits the assumption that structurally
''' related molecules produce similar fragmentation patterns, and therefore they 
''' should be related within a network (Quinn et al., 2017). In MN, MS/MS data 
''' are represented in a graphical form, where each node represents an ion with 
''' an associated fragmentation spectrum; the links among the nodes indicate 
''' similarities of the spectra. By propagation of the structural information within
''' the network, unknown but structurally related molecules can be highlighted
''' and successful dereplication can be obtained (Yang et al., 2013); this may
''' be particularly useful for metabolite and NPS identification.
''' 
''' MN has been implemented In different fields, particularly metabolomics And 
''' drug discovery (Quinn et al., 2017); MN In forensic toxicology was previously
''' used by Allard et al. (2019) For the retrospective analysis Of routine 
''' cases involving biological sample analysis. Yu et al. (2019) also used MN 
''' analysis For the detection Of designer drugs such As NBOMe derivatives And 
''' they showed that unknown compounds could be recognized As NBOMe-related 
''' substances by MN.
''' 
''' In the present work the Global Natural Products Social platform (GNPS) was 
''' exploited to analyze HRMS/MS data obtained from the analysis of seizures 
''' collected by the Italian Department of Scientific Investigation of Carabinieri 
''' (RIS). The potential of MN to highlight And support the identification of
''' unknown NPS belonging to chemical classes such as fentanyls And synthetic
''' cannabinoids has been demonstrated.
''' </remarks>
<Package("MoleculeNetworking")>
Module MoleculeNetworking

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(RawPeakAssign()), AddressOf makePeakAssignTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Public Function makePeakAssignTable(assign As RawPeakAssign(), args As list, env As Environment) As rDataframe
        Dim unzip As (spectrum As PeakMs2, assign As RawPeakAssign)() = assign _
            .Select(Function(a) a.ms2.Select(Function(b) (b, a))) _
            .IteratesALL _
            .ToArray
        Dim df As New rDataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        Static network As New NetworkByteOrderBuffer

        Call df.add("xcms_id", unzip.Select(Function(a) a.assign.Id))
        Call df.add("mz", unzip.Select(Function(a) a.assign.peak.mz))
        Call df.add("rt1", unzip.Select(Function(a) a.assign.peak.rt))
        Call df.add("rt2", unzip.Select(Function(a) a.spectrum.rt))
        Call df.add("ms2", unzip.Select(Function(a) a.spectrum.lib_guid))
        Call df.add("basePeak", unzip.Select(Function(a) a.spectrum.mzInto.OrderByDescending(Function(b) b.intensity).First.mz))
        Call df.add("intensity", unzip.Select(Function(a) a.spectrum.intensity))
        Call df.add("cor", unzip.Select(Function(a) a.assign.cor))
        Call df.add("score", unzip.Select(Function(a) a.assign.score))
        Call df.add("p-value", unzip.Select(Function(a) a.assign.pval))
        Call df.add("samplefile", unzip.Select(Function(a) a.spectrum.file))
        ' add intensity vector data for run debug test
        Call df.add("v1", unzip.Select(Function(a) network.Base64String(a.assign.v1)))
        Call df.add("v2", unzip.Select(Function(a) network.Base64String(a.assign.v2)))

        Return df
    End Function

    ''' <summary>
    ''' makes the spectrum data its unique id reference uniqued!
    ''' </summary>
    ''' <param name="ions">A collection of the mzkit spectrum object</param>
    ''' <returns></returns>
    <ExportAPI("uniqueNames")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function unqiueNames(ions As PeakMs2()) As Object
        Dim id As String() = ions.SafeQuery.Select(Function(i) i.lib_guid).UniqueNames

        For i As Integer = 0 To id.Length - 1
            ions(i).lib_guid = id(i)
        Next

        Return ions
    End Function

    ''' <summary>
    ''' convert the cluster tree into the graph model
    ''' </summary>
    ''' <param name="tree">A cluster tree which is created via the ``tree`` function.</param>
    ''' <param name="ions">
    ''' the source data for create the cluster tree
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("as.graph")>
    Public Function createGraph(tree As ClusterTree, ions As PeakMs2()) As NetworkGraph
        Dim bins As list = tree.MsBin(ions)
        Dim g As NetworkGraph = bins.slots _
            .Select(Function(i)
                        Dim ionSet As PeakMs2() = REnv.asVector(Of PeakMs2)(i.Value)
                        Dim bin As New NamedCollection(Of PeakMs2)(i.Key, ionSet)

                        Return bin
                    End Function) _
            .CreateGraph _
            .AddClusterLinks(tree)

        Return g
    End Function

    ''' <summary>
    ''' do spectrum data clustering
    ''' </summary>
    ''' <param name="ions">A set of the spectrum data</param>
    ''' <param name="mzdiff">
    ''' the ms2 fragment mass tolerance when used for compares 
    ''' ms2 spectrum data
    ''' </param>
    ''' <param name="intocutoff">
    ''' intensity cutoff value that used for make the spectrum 
    ''' centroid and noise cleanup
    ''' </param>
    ''' <param name="equals"></param>
    ''' <returns></returns>
    <ExportAPI("tree")>
    <RApiReturn(GetType(TreeCluster))>
    Public Function Tree(ions As PeakMs2(),
                         Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85) As TreeCluster

        Return ions.Tree(mzdiff, intocutoff, equals)
    End Function

    ''' <summary>
    ''' Split each cluster data into multiple parts by a givne rt window
    ''' </summary>
    ''' <param name="clusters"></param>
    ''' <param name="rtwin"></param>
    ''' <param name="wrap_peaks">
    ''' wraping the networking node data as the spectrum peak object?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the value type of this function is affects by the <paramref name="wrap_peaks"/> parameter:
    ''' 
    ''' 1. for wrap_peaks is set to false by default, a vector of the raw <see cref="NetworkingNode"/> 
    '''    which is extract from the cluster data will be returns
    ''' 2. otherwise the spectrum peaks data will be returns if the parameter 
    '''    value is set to value true.
    ''' </returns>
    ''' <remarks>
    ''' This function works for the small molecular networking analysis
    ''' </remarks>
    <ExportAPI("splitClusterRT")>
    <RApiReturn(GetType(PeakMs2), GetType(NetworkingNode))>
    Public Function splitClusterRT(<RRawVectorArgument>
                                   clusters As Object,
                                   Optional rtwin As Double = 30,
                                   Optional wrap_peaks As Boolean = False,
                                   Optional env As Environment = Nothing) As Object

        Dim src As pipeline = pipeline.TryCreatePipeline(Of NetworkingNode)(clusters, env)

        If src.isError Then
            Return src.getError
        End If

        Dim subNodes = src.populates(Of NetworkingNode)(env) _
            .Select(Function(c) c.SplitClusterRT(rt_win:=rtwin)) _
            .IteratesALL _
            .ToArray

        If wrap_peaks Then
            Return subNodes _
                .Select(Function(n)
                            Return New PeakMs2 With {
                                .mz = n.mz,
                                .activation = "NA",
                                .collisionEnergy = 30,
                                .file = n.referenceId,
                                .intensity = n.size,
                                .lib_guid = n.referenceId,
                                .mzInto = n.representation.ms2,
                                .precursor_type = "NA",
                                .rt = n.members.Average(Function(p) p.rt),
                                .scan = "NA"
                            }
                        End Function) _
                .ToArray
        Else
            Return subNodes
        End If
    End Function

    ''' <summary>
    ''' Do spectrum clustering on a small bundle of the ms2 spectrum from a single raw data file
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="mzdiff1">the mzdiff tolerance value for group the ms2 spectrum via the precursor m/z,
    ''' for precursor m/z comes from the ms1 deconvolution peaktable, tolerance error
    ''' should be smaller in ppm unit; 
    ''' for precursor m/z comes from the ms2 parent ion m/z, tolerance error should 
    ''' be larger in da unit.</param>
    ''' <param name="mzdiff2">the mzdiff tolerance value for do ms2 peak centroid or peak matches for do the
    ''' cos similarity score evaluation, should be larger tolerance value in unit da,
    ''' value of this tolerance parameter could be da:0.3</param>
    ''' <param name="intocutoff">intensity cutoff value for make spectrum centroid</param>
    ''' <param name="tree_identical">
    ''' score cutoff for assert that spectrum in the binary tree
    ''' is in the same cluster node
    ''' </param>
    ''' <param name="tree_right">
    ''' score cutoff for assert that spectrum in the binary tree should be put into the right
    ''' node.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this workflow usually used for processing the ms2 spectrum inside a 
    ''' single raw data file
    ''' </remarks>
    <ExportAPI("clustering")>
    <RApiReturn("graph", "clusters", "matrix", "cluster.raw")>
    Public Function clustering(<RRawVectorArgument>
                               ions As Object,
                               Optional mzdiff1 As Object = "da:0.1",
                               Optional mzdiff2 As Object = "da:0.3",
                               Optional intocutoff As Double = 0.05,
                               Optional tree_identical As Double = 0.8,
                               Optional tree_right As Double = 0.01,
                               Optional env As Environment = Nothing) As Object

        Dim peakms2 As pipeline = pipeline.TryCreatePipeline(Of PeakMs2)(ions, env)
        Dim mz1 = Math.getTolerance(mzdiff1, env, "da:0.1")
        Dim mz2 = Math.getTolerance(mzdiff2, env, "da:0.3")

        If peakms2.isError Then
            Return peakms2.getError
        ElseIf mz1 Like GetType(Message) Then
            Return mz1.TryCast(Of Message)
        ElseIf mz2 Like GetType(Message) Then
            Return mz2.TryCast(Of Message)
        End If

        Dim println As Action(Of Object) = env.WriteLineHandler
        Dim workflow As New Protocols(mz1, mz2, tree_identical, tree_right, New RelativeIntensityCutoff(intocutoff))
        Dim graph = workflow.RunProtocol(peakms2.populates(Of PeakMs2)(env), Sub(msg) println(msg)) _
            .ProduceNodes _
            .Networking
        Dim matrix As rDataframe = ProtocolPipeline _
            .Networking(Of IO.DataSet)(graph, Function(a, b) std.Min(a, b)) _
            .RMatrix
        Dim clusters As NetworkingNode() = graph _
            .Select(Function(u) workflow.Cluster(u.reference)) _
            .ToArray
        Dim graph_score As New list With {.slots = New Dictionary(Of String, Object)}
        Dim spectrum_cluster As New list With {.slots = New Dictionary(Of String, Object)}

        For Each cluster As NetworkingNode In clusters
            spectrum_cluster.slots(cluster.referenceId) = New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"reference_id", cluster.referenceId},
                    {"size", cluster.size},
                    {"representation", cluster.representation},
                    {"members", cluster.members}
                }
            }
        Next

        For Each link As LinkSet In graph
            graph_score.slots(link.reference) = New dataframe With {
                .rownames = link.links.Keys.ToArray,
                .columns = New Dictionary(Of String, Array) From {
                    {"forward", .rownames.Select(Function(i) link.links(i).forward).ToArray},
                    {"reverse", .rownames.Select(Function(i) link.links(i).reverse).ToArray}
                }
            }
        Next

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"graph", graph_score},
                {"clusters", spectrum_cluster},
                {"matrix", matrix},
                {"cluster.raw", clusters}
            }
        }
    End Function

    ''' <summary>
    ''' populate a list of peak ms2 cluster data
    ''' </summary>
    ''' <param name="tree"></param>
    ''' <param name="ions"></param>
    ''' <returns>
    ''' a set of ms2 data groups, in format of ``[guid => peakms2]`` vector tuples
    ''' </returns>
    <ExportAPI("msBin")>
    <Extension>
    Public Function MsBin(tree As ClusterTree, ions As PeakMs2()) As list
        Dim clusters As IEnumerable(Of ClusterTree) = ClusterTree.GetClusters(tree)
        Dim list = ions.ToDictionary(Function(i) i.lib_guid)
        Dim payload As New list With {
            .slots = New Dictionary(Of String, Object)
        }
        Dim members As New List(Of PeakMs2)

        If ions.IsNullOrEmpty Then
            Return payload
        End If

        For Each cluster As ClusterTree In clusters
            Call members.Clear()
            Call members.Add(list(cluster.Data))
            Call members.AddRange(cluster.Members.SafeQuery.Select(Function(r) list(r)))
            Call payload.add(cluster.Data, members.ToArray)
        Next

        Return payload
    End Function

    ''' <summary>
    ''' create representative spectrum data
    ''' </summary>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <see cref="PeakMs2.collisionEnergy"/> is tagged as the cluster size
    ''' </remarks>
    <ExportAPI("representative")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function RepresentativeSpectrum(tree As TreeCluster,
                                           Optional mzdiff As Object = "da:0.3",
                                           Optional env As Environment = Nothing) As Object

        Dim pack As list = tree.tree.MsBin(tree.spectrum)
        Dim output As New List(Of PeakMs2)
        Dim zero As RelativeIntensityCutoff = 0.0
        Dim mzerr = Math.getTolerance(mzdiff, env, [default]:="da:0.3")

        If mzerr Like GetType(Message) Then
            Return mzerr.TryCast(Of Message)
        End If

        Dim tolerance As Tolerance = mzerr.TryCast(Of Tolerance)

        For Each key As String In pack.getNames
            Dim cluster As PeakMs2() = pack.getValue(Of PeakMs2())(key, env, [default]:={})
            Dim ref = cluster.RepresentativeSpectrum(tolerance, zero, key:=key)

            ' <see cref="PeakMs2.collisionEnergy"/> is tagged as the cluster size
            Call output.Add(ref)
        Next

        Return output.ToArray
    End Function

    ''' <summary>
    ''' Create grid clustering of the ms2 spectrum data
    ''' </summary>
    ''' <param name="rawdata"></param>
    ''' <param name="centroid"></param>
    ''' <param name="intocutoff"></param>
    ''' <param name="dia_n">
    ''' set this decompose parameter to any positive integer value greater 
    ''' than 1 may produce too many data for analysis, make the workflow 
    ''' too slow.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("spectrum_grid")>
    Public Function create_spectrum_grid(<RRawVectorArgument> rawdata As Object,
                                         Optional centroid As Object = "da:0.3",
                                         Optional intocutoff As Double = 0.05,
                                         Optional rt_win As Double = 15,
                                         Optional dia_n As Integer = -1,
                                         Optional env As Environment = Nothing) As Object

        Dim rawPool As pipeline = pipeline.TryCreatePipeline(Of mzPack)(rawdata, env)
        Dim specData As New List(Of NamedCollection(Of PeakMs2))
        Dim specSet As PeakMs2()
        Dim massError = Math.getTolerance(centroid, env, [default]:="da:0.3")

        If massError Like GetType(Message) Then
            Return massError.TryCast(Of Message)
        End If

        Dim massWin As Tolerance = massError.TryCast(Of Tolerance)
        Dim cutoff As New RelativeIntensityCutoff(intocutoff)
        Dim filename As String
        Dim id As String()

        If rawPool.isError Then
            Return rawPool.getError
        End If

        For Each raw As mzPack In TqdmWrapper.Wrap(rawPool.populates(Of mzPack)(env).ToArray)
            filename = raw.source.BaseName
            specSet = raw.GetMs2Peaks _
                .AsParallel _
                .Select(Function(si)
                            si.mzInto = si.mzInto.Centroid(massWin, cutoff).ToArray
                            si.file = filename
                            Return si
                        End Function) _
                .ToArray
            id = specSet _
                .Select(Function(si) $"M{CInt(si.mz)}T{CInt(si.rt)}") _
                .UniqueNames _
                .ToArray

            For i As Integer = 0 To specSet.Length - 1
                specSet(i).lib_guid = $"{filename}#{id(i)}"
            Next

            Call specData.Add(New NamedCollection(Of PeakMs2)(filename, specSet))
        Next

        Dim grid As New SpectrumGrid(rt_win, dia_n)
        grid = grid.SetRawDataFiles(specData)

        Return grid
    End Function

    ''' <summary>
    ''' Make precursor assigned to the cluster node
    ''' </summary>
    ''' <param name="grid"></param>
    ''' <param name="peakset"></param>
    ''' <returns></returns>
    <ExportAPI("grid_assigned")>
    <RApiReturn(GetType(RawPeakAssign))>
    Public Function grid_assigned(grid As SpectrumGrid, peakset As PeakSet, Optional assign_top As Integer = 3) As Object
        Return grid.AssignPeaks(peakset.peaks, assign_top:=assign_top).ToArray
    End Function

    <ExportAPI("unpack_unmapped")>
    Public Function unpack_unmapped(grid As SpectrumGrid) As Object
        Dim unmapped As SpectrumLine() = grid.GetUnmapped.ToArray
        Dim spec = unmapped.Select(Function(c) c.cluster).IteratesALL.ToArray
        Dim groups = spec _
            .GroupBy(Function(a) a.file) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return CObj(a.ToArray)
                          End Function)
        Dim list As New list With {.slots = groups}

        Call list.setAttribute("total_clusters", grid.GetTotal.Count)
        Call list.setAttribute("total_spectrum", grid.GetTotal.Select(Function(a) a.cluster).IteratesALL.Count)
        Call list.setAttribute("unmapped_clusters", unmapped.Length)
        Call list.setAttribute("unmapped_spectrum", spec.Length)

        Return list
    End Function

    ''' <summary>
    ''' Unpack of the spectrum data into multiple file groups
    ''' </summary>
    ''' <param name="assign"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A tuple list of the spectrum data in multiple file groups, 
    ''' each slot tuple is a rawdata file content.
    ''' </returns>
    <ExportAPI("unpack_assign")>
    Public Function unpack_assign(<RRawVectorArgument> assign As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of RawPeakAssign)(assign, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim data As New List(Of PeakMs2)

        For Each peak As RawPeakAssign In pull.populates(Of RawPeakAssign)(env)
            Dim id As String = peak.Id

            For Each spec As PeakMs2 In peak.ms2
                spec = New PeakMs2(spec)
                spec.lib_guid = id & "@" & spec.file.Replace(".mzPack", "")
                spec.mz = peak.peak.mz

                Call data.Add(spec)
            Next
        Next

        Call VBDebugger.EchoLine($"make group handling of {data.Count} spectrum data!")

        Return New list With {
            .slots = data _
                .GroupBy(Function(a) a.file) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CObj(a.ToArray)
                              End Function)
        }
    End Function
End Module
