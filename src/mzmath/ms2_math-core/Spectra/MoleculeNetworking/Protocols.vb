#Region "Microsoft.VisualBasic::ac51a00f059bff3a36dae35f2e93a66c, mzmath\ms2_math-core\Spectra\MoleculeNetworking\Protocols.vb"

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

    '   Total Lines: 240
    '    Code Lines: 143
    ' Comment Lines: 65
    '   Blank Lines: 32
    '     File Size: 9.96 KB


    '     Class Protocols
    ' 
    '         Properties: Cluster
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: BinaryTree, centroid, centroidlized, MakeCopy, Networking
    '                   ProduceNodes, RunProtocol
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Spectra.MoleculeNetworking

    ''' <summary>
    ''' this algorithm module works on create molecular networking
    ''' for a small bundle of the ms2 spectrum data
    ''' </summary>
    ''' <remarks>
    ''' algorithm descriptions:
    ''' 
    ''' 1. 按照母离子m/z二叉树聚类
    ''' 2. 每一个聚类结果作为一个node进行碎片合并
    ''' 3. 基于node进行molecular networking
    ''' 4. 导出网络数据
    ''' </remarks>
    Public Class Protocols

        ReadOnly ms1_tolerance As Tolerance
        ReadOnly ms2_tolerance As Tolerance

        ReadOnly treeIdentical As Double
        ReadOnly treeSimilar As Double
        ReadOnly intoCutoff As LowAbundanceTrimming

        ReadOnly raw As New Dictionary(Of String, PeakMs2)
        ReadOnly clusters As New Dictionary(Of String, NetworkingNode)

        Default Public ReadOnly Property GetSpectrum(ref As String) As PeakMs2
            Get
                Return raw.TryGetValue(ref)
            End Get
        End Property

        Public ReadOnly Property Cluster(ref As String) As NetworkingNode
            Get
                Return clusters.TryGetValue(ref)
            End Get
        End Property

        ''' <summary>
        ''' construct a workflow for create molecular networking pipeline which is
        ''' used for running on a small bundle of the ms2 spectrum data.
        ''' </summary>
        ''' <param name="ms1_tolerance">
        ''' the mzdiff tolerance value for group the ms2 spectrum via the precursor m/z,
        ''' for precursor m/z comes from the ms1 deconvolution peaktable, tolerance error
        ''' should be smaller in ppm unit; 
        ''' for precursor m/z comes from the ms2 parent ion m/z, tolerance error should 
        ''' be larger in da unit. 
        ''' </param>
        ''' <param name="ms2_tolerance">
        ''' the mzdiff tolerance value for do ms2 peak centroid or peak matches for do the
        ''' cos similarity score evaluation, should be larger tolerance value in unit da,
        ''' value of this tolerance parameter could be da:0.3
        ''' </param>
        ''' <param name="treeIdentical">score cutoff for assert that spectrum in the binary tree
        ''' is in the same cluster node</param>
        ''' <param name="treeSimilar">
        ''' score cutoff for assert that spectrum in the binary tree should be put into the right
        ''' node.
        ''' </param>
        ''' <param name="intoCutoff">intensity cutoff value for make spectrum centroid</param>
        ''' <remarks>
        ''' this workflow usually used for processing the ms2 spectrum inside a 
        ''' single raw data file
        ''' </remarks>
        Sub New(ms1_tolerance As Tolerance,
                ms2_tolerance As Tolerance,
                treeIdentical As Double,
                treeSimilar As Double,
                intoCutoff As LowAbundanceTrimming)

            Me.treeIdentical = treeIdentical
            Me.treeSimilar = treeSimilar
            Me.intoCutoff = intoCutoff
            Me.ms1_tolerance = ms1_tolerance
            Me.ms2_tolerance = ms2_tolerance
        End Sub

        Public Function RunProtocol(raw As IEnumerable(Of PeakMs2), progress As Action(Of String)) As ProtocolPipeline
            Dim centroid As PeakMs2()

            Call progress("run data centroidlized...")

            centroid = centroidlized(raw)

            For Each ion As PeakMs2 In centroid
                Me.raw.Add(ion.lib_guid, ion)
            Next

            Return New ProtocolPipeline(Me, centroid, progress)
        End Function

        ''' <summary>
        ''' 步骤1
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        Friend Iterator Function BinaryTree(raw As IEnumerable(Of PeakMs2)) As IEnumerable(Of SpectrumCluster)
            Dim tree As New SpectrumTreeCluster(
                compares:=SpectrumTreeCluster.SSMCompares(ms2_tolerance, Nothing, treeIdentical, treeSimilar),
                showReport:=False
            )

            Call tree.doCluster(raw.ToArray)

            For Each cluster As SpectrumCluster In tree.PopulateClusters
                Yield cluster
            Next
        End Function

        ''' <summary>
        ''' 为了减少内存占用，在这里进行离散化
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <returns></returns>
        Private Function centroid(raw As PeakMs2) As PeakMs2
            raw = MakeCopy(raw)
            raw.mzInto = raw.mzInto _
                .Centroid(ms2_tolerance, intoCutoff) _
                .ToArray

            Return raw
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function MakeCopy(raw As PeakMs2) As PeakMs2
            Return New PeakMs2 With {
                .activation = raw.activation,
                .file = raw.file,
                .collisionEnergy = raw.collisionEnergy,
                .lib_guid = raw.lib_guid,
                .meta = If(raw.meta Is Nothing,
                    New Dictionary(Of String, String),
                    New Dictionary(Of String, String)(raw.meta)
                 ),
                .mz = raw.mz,
                .mzInto = raw.mzInto,
                .precursor_type = raw.precursor_type,
                .rt = raw.rt,
                .scan = raw.scan
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function centroidlized(raw As IEnumerable(Of PeakMs2)) As PeakMs2()
            Return raw _
                .AsParallel _
                .Select(AddressOf centroid) _
                .ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw">
        ''' processing all ms2 spectrum data at here for a cluster session
        ''' </param>
        ''' <returns></returns>
        Friend Iterator Function ProduceNodes(raw As IEnumerable(Of PeakMs2)) As IEnumerable(Of NetworkingNode)
            Dim groupByMz As NamedCollection(Of PeakMs2)() = raw _
                .GroupBy(Function(peak) peak.mz, ms1_tolerance) _
                .ToArray
            Dim uniqueCounter As New Dictionary(Of String, Integer)
            Dim v As NetworkingNode
            Dim vkey As String

            For Each mz As NamedCollection(Of PeakMs2) In groupByMz
                For Each cluster As SpectrumCluster In BinaryTree(mz)
                    ' 20230625 due to the reason of the reference id is generated
                    ' via take the top 3 ions from the ms2 spectrum, so the duplicated
                    ' reference id can not be avoid in current cluster session.
                    ' and the duplicated session id will crashed the network node
                    ' indexing in the downstream process
                    '
                    ' make the reference unique by add a counter suffix at here
                    ' for avoid such duplicated key problem
                    v = NetworkingNode.Create(Val(mz.name), cluster, ms2_tolerance, intoCutoff)
                    vkey = v.referenceId

                    If Not uniqueCounter.ContainsKey(vkey) Then
                        Call uniqueCounter.Add(vkey, 1)
                    Else
                        ' has a duplicated id hit
                        v.representation.name = $"{vkey}_{uniqueCounter(vkey)}"
                        uniqueCounter(vkey) += 1
                    End If

                    Yield v
                Next
            Next
        End Function

        Friend Iterator Function Networking(nodes As IEnumerable(Of NetworkingNode), progress As Action(Of String)) As IEnumerable(Of LinkSet)
            Dim i As i32 = 1
            Dim rawData As NetworkingNode() = nodes.ToArray

            For Each scan As NetworkingNode In rawData
                Dim scores = rawData _
                    .Where(Function(a) Not a Is scan) _
                    .AsParallel _
                    .Select(Function(a)
                                Dim id As String = a.referenceId
                                Dim score = GlobalAlignment.TwoDirectionSSM(scan.representation.ms2, a.representation.ms2, ms2_tolerance)

                                Return (id, score.forward, score.reverse)
                            End Function) _
                    .ToArray

                If ++i Mod 3 = 0 Then
                    Call progress($"[{i}/{rawData.Length}] {scan.ToString} has {scores.Where(Function(a) a.Item2 >= 0.8).Count} homologous spectrum")
                End If

                Call clusters.Add(scan.referenceId, scan)

                Dim links = scores _
                    .ToDictionary(Function(a) a.id,
                                  Function(a)
                                      Return New NetworkClusterLinkEndPoint With {
                                         .id = a.id,
                                         .forward = a.forward,
                                         .reverse = a.reverse
                                      }
                                  End Function)

                Yield New LinkSet With {
                    .reference = scan.referenceId,
                    .links = links
                }
            Next
        End Function
    End Class
End Namespace
