#Region "Microsoft.VisualBasic::f0eec22665fb78f2cd9251da864f4ead, mzkit\Rscript\Library\mzkit.insilicons\MoleculeNetworking.vb"

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

    '   Total Lines: 120
    '    Code Lines: 85
    ' Comment Lines: 16
    '   Blank Lines: 19
    '     File Size: 4.41 KB


    ' Module MoleculeNetworking
    ' 
    '     Function: createGraph, MsBin, RepresentativeSpectrum, Tree, unqiueNames
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

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

    ''' <summary>
    ''' makes the spectrum data its unique id reference uniqued!
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <returns></returns>
    <ExportAPI("uniqueNames")>
    Public Function unqiueNames(ions As PeakMs2()) As PeakMs2()
        Dim id As String() = ions.SafeQuery.Select(Function(i) i.lib_guid).uniqueNames

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
    Public Function Tree(ions As PeakMs2(),
                         Optional mzdiff As Double = 0.3,
                         Optional intocutoff As Double = 0.05,
                         Optional equals As Double = 0.85) As ClusterTree

        Return ions.Tree(mzdiff, intocutoff, equals)
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
    ''' <param name="tree"></param>
    ''' <param name="ions"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <see cref="PeakMs2.collisionEnergy"/> is tagged as the cluster size
    ''' </remarks>
    <ExportAPI("representative")>
    <RApiReturn(GetType(PeakMs2))>
    Public Function RepresentativeSpectrum(tree As ClusterTree,
                                           ions As PeakMs2(),
                                           Optional mzdiff As Object = "da:0.3",
                                           Optional env As Environment = Nothing) As Object

        Dim pack As list = tree.MsBin(ions)
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
End Module
