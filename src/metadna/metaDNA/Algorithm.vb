#Region "Microsoft.VisualBasic::29667a5f31fdbb5aba7da9b3168fda85, metaDNA\Algorithm.vb"

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

' Class Algorithm
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: alignKeggCompound, (+2 Overloads) DIASearch, GetBestQuery, GetCandidateSeeds, RunInfer
'               RunIteration, SetKeggLibrary, SetNetwork, SetSamples, SetSearchRange
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports stdnum = System.Math

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

    Dim precursorTypes As MzCalculator()
    Dim typeOrders As Index(Of String)

    Dim unknowns As UnknownSet
    Dim kegg As KEGGHandler
    Dim network As KEGGNetwork
    Dim maxIterations As Integer = 1000

    Public ReadOnly Property ms1Err As Tolerance
        Get
            Return ms1ppm
        End Get
    End Property

#Region "algorithm initialization"

    Sub New(ms1ppm As Tolerance,
            dotcutoff As Double,
            mzwidth As Tolerance,
            Optional allowMs1 As Boolean = True,
            Optional maxIterations As Integer = 1000)

        Me.ms1ppm = ms1ppm
        Me.dotcutoff = dotcutoff
        Me.MSalignment = New CosAlignment(mzwidth, LowAbundanceTrimming.Default)
        Me.mzwidth = mzwidth
        Me.allowMs1 = allowMs1
        Me.maxIterations = maxIterations
    End Sub

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
    ''' <returns></returns>
    Public Function SetSamples(sample As IEnumerable(Of PeakMs2)) As Algorithm
        unknowns = UnknownSet.CreateTree(sample, ms1ppm)
        Return Me
    End Function

    Public Function SetSamples(sample As UnknownSet) As Algorithm
        unknowns = sample
        Return Me
    End Function

    Public Function SetKeggLibrary(library As IEnumerable(Of Compound)) As Algorithm
        kegg = KEGGHandler.CreateIndex(library, precursorTypes, ms1ppm)
        Return Me
    End Function

    Public Function SetNetwork(classLinks As IEnumerable(Of ReactionClass)) As Algorithm
        network = KEGGNetwork.CreateNetwork(classLinks)
        Return Me
    End Function
#End Region

    ''' <summary>
    ''' Create infer network
    ''' </summary>
    ''' <param name="seeds"></param>
    ''' <returns></returns>
    Public Function RunIteration(seeds As IEnumerable(Of AnnotatedSeed)) As IEnumerable(Of InferLink)
        Return seeds _
            .ToArray _
            .AsParallel _
            .Select(AddressOf RunInfer) _
            .IteratesALL
    End Function

    Private Iterator Function RunInfer(seed As AnnotatedSeed) As IEnumerable(Of InferLink)
        For Each kegg_id As String In network.FindPartners(seed.kegg_id)
            Dim compound As Compound = kegg.GetCompound(kegg_id)

            If compound Is Nothing Then
                Continue For
            End If

            For Each hit As InferLink In alignKeggCompound(seed, compound)
                Yield hit
            Next
        Next
    End Function

    Private Iterator Function alignKeggCompound(seed As AnnotatedSeed, compound As Compound) As IEnumerable(Of InferLink)
        For Each type As MzCalculator In precursorTypes
            Dim mz As Double = type.CalcMZ(compound.exactMass)
            Dim candidates As PeakMs2() = unknowns.QueryByParentMz(mz).ToArray

            If candidates.IsNullOrEmpty Then
                Continue For
            End If

            For Each hit As PeakMs2 In candidates
                Dim alignment As InferLink = GetBestQuery(hit, seed)
                Dim kegg As New KEGGQuery With {
                    .mz = mz,
                    .kegg_id = compound.entry,
                    .precursorType = type.ToString,
                    .ppm = PPMmethod.PPM(mz, hit.mz)
                }

                If alignment Is Nothing Then
                    Continue For
                End If

                alignment.kegg = kegg

                If stdnum.Min(alignment.forward, alignment.reverse) < dotcutoff Then
                    If alignment.jaccard >= 0.5 Then
                        alignment.level = InferLevel.Ms2
                        alignment.parentTrace *= (0.95 * dotcutoff)
                    ElseIf allowMs1 Then
                        alignment.alignments = Nothing
                        alignment.level = InferLevel.Ms1
                        alignment.parentTrace *= (0.5 * dotcutoff)
                    Else
                        Continue For
                    End If
                Else
                    alignment.level = InferLevel.Ms2
                    alignment.parentTrace *= stdnum.Min(alignment.forward, alignment.reverse)
                End If

                Yield alignment
            Next
        Next
    End Function

    Private Function GetBestQuery(hit As PeakMs2, seed As AnnotatedSeed) As InferLink
        Dim max As InferLink = Nothing
        Dim align As AlignmentOutput
        Dim score As (forward#, reverse#)

        For Each ref In seed.products.Where(Function(sd) sd.Key <> hit.lib_guid)
            align = MSalignment.CreateAlignment(hit.mzInto, ref.Value.ms2)
            score = MSalignment.GetScore(align.alignments)

            If max Is Nothing OrElse stdnum.Min(score.forward, score.reverse) > stdnum.Min(max.forward, max.reverse) Then
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
                    .rawFile = hit.file
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

            Call Console.WriteLine($"[iteration {++i}] infers {result.Length}, find {seeds.Count} seeds, {n} current candidates ...")

            If i > maxIterations Then
                Call Console.WriteLine($"Max iteration number {maxIterations} has been reached, exit metaDNA infer loop!")
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
        For Each unknown As PeakMs2 In unknowns.EnumerateAllUnknownFeatures
            Dim seedRef As New LibraryMatrix With {
                .ms2 = unknown.mzInto,
                .name = unknown.ToString
            }

            For Each DIAseed As KEGGQuery In kegg.QueryByMz(unknown.mz)
                Yield New AnnotatedSeed With {
                    .id = unknown.lib_guid,
                    .kegg_id = DIAseed.kegg_id,
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
        Next
    End Function

End Class
