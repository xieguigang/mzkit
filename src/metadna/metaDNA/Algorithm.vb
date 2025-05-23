﻿#Region "Microsoft.VisualBasic::b78a4dedd206ddcf458e2711ecb4f416, metadna\metaDNA\Algorithm.vb"

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
    '    Code Lines: 366 (72.05%)
    ' Comment Lines: 63 (12.40%)
    '    - Xml Docs: 80.95%
    ' 
    '   Blank Lines: 79 (15.55%)
    '     File Size: 18.42 KB


    ' Class Algorithm
    ' 
    '     Properties: ms1Err
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) alignKeggCompound, (+2 Overloads) DIASearch, ExportTable, GetBestQuery, GetCandidateSeeds
    '               GetPerfermanceCounter, GetUnknownSet, inferAlignment, querySingle, RunInfer
    '               RunIteration, SetKeggLibrary, SetLibrary, (+3 Overloads) SetNetwork, SetReportHandler
    '               (+2 Overloads) SetSamples, SetSearchRange, SimpleSetROI
    '     Class NetworkInferTask
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetNetwork
    ' 
    '         Sub: Solve
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports std = System.Math

''' <summary>
''' implements of the metadna algorithm in VisualBasic language
''' </summary>
Public Class Algorithm

    ''' <summary>
    ''' tolerance error between two ms1 m/z in ppm
    ''' </summary>
    ReadOnly ms1ppm As Tolerance
    ReadOnly dotcutoff As Double
    ReadOnly MSalignment As AlignmentProvider
    ReadOnly mzwidth As Tolerance
    ReadOnly allowMs1 As Boolean = True
    ReadOnly debug As Boolean = False

    Dim precursorTypes As MzCalculator()
    Dim typeOrders As Index(Of String)

    Dim unknowns As UnknownSet
    Dim kegg As MSSearch(Of GenericCompound)
    Dim network As Networking
    Dim maxIterations As Integer = 1000
    Dim report As Action(Of String)

    Public ReadOnly Property ms1Err As Tolerance
        Get
            Return ms1ppm
        End Get
    End Property

#Region "algorithm initialization"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ms1ppm"></param>
    ''' <param name="dotcutoff"></param>
    ''' <param name="mzwidth">
    ''' the product m/z tolerance of the ms2 data
    ''' </param>
    ''' <param name="allowMs1"></param>
    ''' <param name="maxIterations"></param>
    Sub New(ms1ppm As Tolerance,
            dotcutoff As Double,
            mzwidth As Tolerance,
            Optional allowMs1 As Boolean = True,
            Optional maxIterations As Integer = 1000,
            Optional debug As Boolean = False)

        Me.ms1ppm = ms1ppm
        Me.dotcutoff = dotcutoff
        Me.MSalignment = New CosAlignment(mzwidth, LowAbundanceTrimming.Default)
        Me.mzwidth = mzwidth
        Me.allowMs1 = allowMs1
        Me.maxIterations = maxIterations
        Me.report = AddressOf Console.WriteLine
        Me.debug = debug

        If debug Then
            Call VBDebugger.EchoLine("run metadna algorithm in debug mode.")
        End If
    End Sub

    Public Function SetReportHandler(report As Action(Of String)) As Algorithm
        Me.report = report
        Return Me
    End Function

    Public Function SetSearchRange(ParamArray precursorTypes As String()) As Algorithm
        Me.precursorTypes = precursorTypes _
            .Select(Function(name)
                        Return Parser.ParseMzCalculator(name, name.Last)
                    End Function) _
            .ToArray
        Me.typeOrders = precursorTypes

        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetUnknownSet() As UnknownSet
        Return unknowns
    End Function

    ''' <summary>
    ''' create sample data set: <see cref="unknowns"/>
    ''' </summary>
    ''' <param name="sample"></param>
    ''' <param name="autoROIid">
    ''' assign the ms1 ROI id in <see cref="PeakMs2.meta"/> automatically?
    ''' </param>
    ''' <returns></returns>
    Public Function SetSamples(sample As IEnumerable(Of PeakMs2), Optional autoROIid As Boolean = True) As Algorithm
        If autoROIid Then
            ' 20210318
            ' toarray is required at here
            ' or stack overflow error will be happends
            sample = (Iterator Function() As IEnumerable(Of PeakMs2)
                          For Each peak As PeakMs2 In sample
                              Yield SimpleSetROI(peak, id:=Nothing)
                          Next
                      End Function)().ToArray
        End If

        unknowns = UnknownSet.CreateTree(sample, ms1ppm)

        Return Me
    End Function

    ''' <summary>
    ''' set ROI to the spectrum metadata slot
    ''' </summary>
    ''' <param name="peak"></param>
    ''' <param name="id">the ROI id to set in this function</param>
    ''' <returns></returns>
    Public Shared Function SimpleSetROI(<Out> ByRef peak As PeakMs2, id As String) As PeakMs2
        If peak.meta Is Nothing Then
            peak.meta = New Dictionary(Of String, String)
        End If

        If id.StringEmpty Then
            ' one ms1 peak feature may mapping to multiple peakms2
            ' so the ROI id maybe duplicated
            ' but the ms2 lib guid is unique identified for each
            ' peakms2 feature data.
            If Not peak.meta.ContainsKey("ROI") Then
                If CInt(peak.rt) = 0 Then
                    peak.meta!ROI = $"M{CInt(peak.mz)}"
                Else
                    peak.meta!ROI = $"M{CInt(peak.mz)}T{CInt(peak.rt)}"
                End If
            End If
        Else
            peak.meta!ROI = id
        End If

        Return peak
    End Function

    Public Function SetSamples(sample As UnknownSet) As Algorithm
        unknowns = sample
        Return Me
    End Function

    ''' <summary>
    ''' 必须要先执行<see cref="SetSearchRange"/>
    ''' </summary>
    ''' <param name="library"></param>
    ''' <returns></returns>
    Public Function SetKeggLibrary(library As IEnumerable(Of Compound)) As Algorithm
        kegg = CompoundSolver.CreateIndex(library, precursorTypes, ms1ppm)
        Return Me
    End Function

    Public Function SetLibrary(solver As MSSearch(Of GenericCompound)) As Algorithm
        kegg = solver
        Return Me
    End Function

    Public Function SetNetwork(classLinks As IEnumerable(Of ReactionClass)) As Algorithm
        network = KEGGNetwork.CreateNetwork(classLinks)
        Return Me
    End Function

    Public Function SetNetwork(metabolism As IEnumerable(Of Reaction)) As Algorithm
        network = KEGGNetwork.CreateNetwork(metabolism)
        Return Me
    End Function

    Public Function SetNetwork(networking As Networking) As Algorithm
        network = networking
        Return Me
    End Function
#End Region

    ''' <summary>
    ''' Create infer network
    ''' </summary>
    ''' <param name="seeds"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function RunIteration(seeds As IEnumerable(Of AnnotatedSeed)) As IEnumerable(Of InferLink)
        Dim task As New NetworkInferTask(seeds.ToArray, Me)

        If debug Then
            Call task.Solve()
        Else
            Call task.Run()
        End If

        Return task.GetNetwork
    End Function

    Private Class NetworkInferTask : Inherits VectorTask

        ReadOnly seeds As AnnotatedSeed()
        ReadOnly result_buffer As InferLink()()
        ReadOnly metadna As Algorithm

        Sub New(seeds As AnnotatedSeed(), metadna As Algorithm)
            Call MyBase.New(seeds.Length)

            Me.metadna = metadna
            Me.seeds = seeds
            Me.result_buffer = Allocate(Of InferLink())(all:=False)
        End Sub

        Public Iterator Function GetNetwork() As IEnumerable(Of InferLink)
            If result_buffer Is Nothing Then
                Call $"the parallel result for metaDNA network infer could not be null???".Warning
                Return
            End If

            For Each block As InferLink() In result_buffer
                If block Is Nothing Then
                    Continue For
                End If

                For Each link As InferLink In block
                    Yield link
                Next
            Next
        End Function

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            Dim result As New List(Of InferLink)

            For i As Integer = start To ends
                Call result.AddRange(metadna.RunInfer(seeds(i)))
            Next

            SyncLock result_buffer
                result_buffer(cpu_id) = result.ToArray
                result.Clear()
            End SyncLock
        End Sub
    End Class

    ReadOnly partner_cache As New Dictionary(Of String, String())

    Private Iterator Function RunInfer(seed As AnnotatedSeed) As IEnumerable(Of InferLink)
        Dim partners As String()

        SyncLock partner_cache
            ' enable the cache for reduce the mysql query impact
            partners = partner_cache.ComputeIfAbsent(
                key:=seed.kegg_id,
                lazyValue:=Function(kegg_id)
                               Return network.FindPartners(seed.kegg_id).ToArray
                           End Function)
        End SyncLock

        For Each kegg_id As String In partners
            Dim compound As GenericCompound = kegg.GetCompound(kegg_id)

            If compound Is Nothing OrElse compound.ExactMass <= 0 Then
                Continue For
            End If

            For Each hit As InferLink In alignKeggCompound(seed, compound)
                Yield hit
            Next
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function alignKeggCompound(seed As AnnotatedSeed, compound As GenericCompound) As IEnumerable(Of InferLink)
        Return precursorTypes _
            .Select(Function(type)
                        Return alignKeggCompound(type, seed, compound)
                    End Function) _
            .IteratesALL
    End Function

    Private Iterator Function alignKeggCompound(type As MzCalculator, seed As AnnotatedSeed, compound As GenericCompound) As IEnumerable(Of InferLink)
        Dim mz As Double = type.CalcMZ(compound.ExactMass)
        Dim candidates As PeakMs2() = unknowns.QueryByParentMz(mz).ToArray

        If candidates.IsNullOrEmpty Then
            Return
        End If

        For Each infer As InferLink In candidates _
            .Select(Function(hit)
                        Return inferAlignment(hit, mz, type, seed, compound)
                    End Function)

            If infer IsNot Nothing Then
                Yield infer
            End If
        Next
    End Function

    Private Function inferAlignment(hit As PeakMs2, mz As Double, type As MzCalculator, seed As AnnotatedSeed, compound As GenericCompound) As InferLink
        Dim alignment As InferLink = GetBestQuery(hit, seed)
        Dim kegg As New MzQuery With {
            .mz = mz,
            .unique_id = compound.Identity,
            .precursor_type = type.ToString,
            .ppm = PPMmethod.PPM(mz, hit.mz)
        }

        If alignment Is Nothing Then
            Return Nothing
        End If

        alignment.kegg = kegg

        If std.Min(alignment.forward, alignment.reverse) < dotcutoff Then
            If alignment.jaccard >= 0.5 Then
                alignment.level = InferLevel.Ms2
                alignment.parentTrace *= (0.95 * dotcutoff)
            ElseIf allowMs1 Then
                alignment.alignments = Nothing
                alignment.level = InferLevel.Ms1
                alignment.parentTrace *= (0.5 * dotcutoff)
            Else
                Return Nothing
            End If
        Else
            alignment.level = InferLevel.Ms2
            alignment.parentTrace *= std.Min(alignment.forward, alignment.reverse)
        End If

        Return alignment
    End Function

    Private Function GetBestQuery(hit As PeakMs2, seed As AnnotatedSeed) As InferLink
        Dim max As InferLink = Nothing
        Dim align As AlignmentOutput
        Dim score As (forward#, reverse#)

        For Each ref In seed.products.Where(Function(sd) sd.Key <> hit.lib_guid)
            align = MSalignment.CreateAlignment(hit.mzInto, ref.Value.ms2)
            score = MSalignment.GetScore(align.alignments)

            If max Is Nothing OrElse std.Min(score.forward, score.reverse) > std.Min(max.forward, max.reverse) Then
                max = New InferLink With {
                    .reverse = score.reverse,
                    .forward = score.forward,
                    .jaccard = GlobalAlignment.JaccardIndex(hit.mzInto, ref.Value.ms2, mzwidth),
                    .alignments = align.alignments,
                    .query = New Meta With {
                        .id = hit.lib_guid,
                        .mz = hit.mz,
                        .scan_time = hit.rt,
                        .intensity = hit.intensity
                    },
                    .reference = New Meta With {
                        .id = seed.ToString,
                        .mz = seed.parent.mz,
                        .scan_time = seed.parent.scan_time,
                        .intensity = seed.parent.intensity
                    },
                    .parentTrace = seed.parentTrace,
                    .inferSize = seed.inferSize + 1,
                    .rawFile = hit.file,
                    .entropy = align.entropy
                }
            End If
        Next

        Return max
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ExportTable(result As IEnumerable(Of CandidateInfer), Optional unique As Boolean = False) As IEnumerable(Of MetaDNAResult)
        Dim data = result.ExportTable(kegg, keggNetwork:=network)

        If unique Then
            data = data.GetUniques(typeOrders)
        End If

        Return data.OrderBy(Function(r) r.rt)
    End Function

    ReadOnly perfermanceCounter As New List(Of (Integer, TimeSpan, Integer, Integer, Integer))

    Public Function GetPerfermanceCounter() As (iteration As Integer, ticks As TimeSpan, inferLinks As Integer, seeding As Integer, candidates As Integer)()
        Return perfermanceCounter.ToArray
    End Function

    ''' <summary>
    ''' 基于种子进行推断注释
    ''' </summary>
    ''' <param name="seeds"></param>
    ''' <returns></returns>
    Public Iterator Function DIASearch(seeds As IEnumerable(Of AnnotatedSeed)) As IEnumerable(Of CandidateInfer)
        Dim result As InferLink()
        Dim seeding As New SeedsProvider(
            unknowns:=unknowns,
            ranges:=precursorTypes,
            kegg:=kegg
        )
        Dim candidates As CandidateInfer()
        Dim i As i32 = 1
        Dim n As Integer = 0
        Dim start As Long = App.NanoTime
        Dim tickTime As TimeSpan

        Call perfermanceCounter.Clear()

        Do
            result = RunIteration(seeds).ToArray
            candidates = seeding.CandidateInfers(infer:=result).ToArray
            seeds = seeding.Seeding(infers:=candidates).ToArray

            For Each seed As AnnotatedSeed In seeds
                Call unknowns.AddTrace(seed.products.Keys)
            Next
            For Each infer As CandidateInfer In candidates
                Yield infer
            Next

            n += candidates.Length
            tickTime = TimeSpan.FromTicks(App.NanoTime - start)
            start = App.NanoTime

            Call perfermanceCounter.Add((CInt(i), tickTime, result.Length, seeds.Count, n))
            Call report($"[iteration {++i}, {tickTime.FormatTime}] infers {result.Length}, find {seeds.Count} seeds, {n} current candidates ...")

            If i > maxIterations Then
                Call report($"Max iteration number {maxIterations} has been reached, exit metaDNA infer loop!")
                Exit Do
            End If
        Loop While result.Length > 0
    End Function

    ''' <summary>
    ''' 单纯的进行推断，没有种子做基础
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 算法模块测试用
    ''' </remarks>
    Public Function DIASearch() As IEnumerable(Of CandidateInfer)
        Return DIASearch(GetCandidateSeeds)
    End Function

    Private Iterator Function GetCandidateSeeds() As IEnumerable(Of AnnotatedSeed)
        Dim ms2Peaks = unknowns.EnumerateAllUnknownFeatures.ToArray
        Dim n As Integer = 0
        Dim popOut As Func(Of IEnumerable(Of IEnumerable(Of AnnotatedSeed)))

        If debug Then
            popOut = Function() ms2Peaks.Select(AddressOf querySingle)
        Else
            popOut = Function() DirectCast(ms2Peaks.AsParallel.Select(AddressOf querySingle), IEnumerable(Of IEnumerable(Of AnnotatedSeed)))
        End If

        Call report($"Create candidate seeds by query metabolite annotation library...")
        Call report($"impute from {ms2Peaks.Length} ms2 peaks!")

        For Each seeds As IEnumerable(Of AnnotatedSeed) In popOut()
            For Each seed As AnnotatedSeed In seeds
                n += 1
                Yield seed
            Next
        Next

        Call report($"populate out {n} candidate seeds for run metaDNA inferacne!")
    End Function

    Private Iterator Function querySingle(unknown As PeakMs2) As IEnumerable(Of AnnotatedSeed)
        Dim seedRef As New LibraryMatrix With {
            .ms2 = unknown.mzInto,
            .name = SplashID.MsSplashId(unknown)
        }

        ' get metabolite hits just based on the parent ion
        ' mz matches
        For Each DIAseed As MzQuery In kegg.QueryByMz(unknown.mz)
            Yield New AnnotatedSeed With {
                .id = unknown.lib_guid,
                .kegg_id = DIAseed.unique_id,
                .parent = New ms1_scan With {
                    .mz = unknown.mz,
                    .scan_time = unknown.rt,
                    .intensity = unknown.Ms2Intensity
                },
                .products = New Dictionary(Of String, LibraryMatrix) From {
                    {unknown.lib_guid, seedRef}
                },
                .parentTrace = 100,
                .inferSize = 1
            }
        Next
    End Function

End Class
