#Region "Microsoft.VisualBasic::4664998e1b599f8590c005186575182c, metaDNA\Algorithm.vb"

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
'     Function: alignKeggCompound, GetBestQuery, RunInfer, RunIteration
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.MetaDNA.Infer
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

    Dim precursorTypes As MzCalculator()

    Dim unknowns As UnknownSet
    Dim kegg As KEGGHandler
    Dim network As KEGGNetwork

#Region "algorithm initialization"

    Sub New(ms1ppm As Tolerance, dotcutoff As Double, mzwidth As Tolerance)
        Me.ms1ppm = ms1ppm
        Me.dotcutoff = dotcutoff
        Me.MSalignment = New CosAlignment(mzwidth, LowAbundanceTrimming.Default)
    End Sub

    Public Function SetSearchRange(ParamArray precursorTypes As String()) As Algorithm
        Me.precursorTypes = precursorTypes _
            .Select(Function(name)
                        Return Parser.ParseMzCalculator(name, name.Last)
                    End Function) _
            .ToArray

        Return Me
    End Function

    Public Function SetSamples(sample As IEnumerable(Of PeakMs2)) As Algorithm
        unknowns = UnknownSet.CreateTree(sample, ms1ppm)
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
            Dim candidates As PeakMs2() = unknowns.QueryByParentMz(mz)

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

                alignment.kegg = kegg

                If stdnum.Min(alignment.forward, alignment.reverse) < dotcutoff Then
                    alignment.alignments = Nothing
                    alignment.level = InferLevel.Ms1
                    alignment.forward = 0
                    alignment.reverse = 0
                Else
                    alignment.level = InferLevel.Ms2
                End If

                Yield alignment
            Next
        Next
    End Function

    Private Function GetBestQuery(hit As PeakMs2, seed As AnnotatedSeed) As InferLink
        Dim max As InferLink = Nothing
        Dim align As AlignmentOutput
        Dim score As (forward#, reverse#)

        For Each ref In seed.products
            align = MSalignment.CreateAlignment(hit.mzInto, ref.Value.ms2)
            score = MSalignment.GetScore(align.alignments)

            If max Is Nothing OrElse stdnum.Min(score.forward, score.reverse) > stdnum.Min(max.forward, max.reverse) Then
                max = New InferLink With {
                    .reverse = score.reverse,
                    .forward = score.forward,
                    .alignments = align.alignments,
                    .query = New Meta With {.id = hit.lib_guid, .mz = hit.mz, .rt = hit.rt},
                    .reference = New Meta With {.id = seed.ToString, .mz = seed.parent.mz, .rt = seed.parent.scan_time}
                }
            End If
        Next

        Return max
    End Function

    Public Iterator Function DIASearch(seeds As IEnumerable(Of AnnotatedSeed)) As IEnumerable(Of CandidateInfer)
        Dim result As InferLink()
        Dim seeding As New SeedsProvider(
            unknowns:=unknowns,
            ranges:=precursorTypes,
            kegg:=kegg
        )
        Dim candidates As CandidateInfer()

        Do
            result = RunIteration(seeds).ToArray
            candidates = seeding.CandidateInfers(infer:=result).ToArray
            seeds = seeding.Seeding(infers:=candidates).ToArray

            For Each infer As CandidateInfer In candidates
                Yield infer
            Next
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
                    }
                }
            Next
        Next
    End Function

End Class
