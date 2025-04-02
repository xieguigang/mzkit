#Region "Microsoft.VisualBasic::4de58d730f30eb89339772f2ef077cfa, assembly\SpectrumTree\Pack\PackAlignment.vb"

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

'   Total Lines: 225
'    Code Lines: 132 (58.67%)
' Comment Lines: 70 (31.11%)
'    - Xml Docs: 78.57%
' 
'   Blank Lines: 23 (10.22%)
'     File Size: 9.49 KB


'     Class PackAlignment
' 
'         Properties: dotcutoff, libnames, parallel, size, spectrum
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetReferenceSpectrum, reportClusterHit, Search, SearchParallel, SearchSequential
'         Structure ___tmp
' 
' 
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Query
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.Tree
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace PackLib

    ''' <summary>
    ''' alignment the query spectrum to the reference spectrum
    ''' </summary>
    ''' <remarks>
    ''' the data <see cref="spectrum"/> is the file stream connection
    ''' to the reference database file, spectrum could be get via id
    ''' based on the id elements inside <see cref="libnames"/> list.
    ''' </remarks>
    Public Class PackAlignment : Inherits Ms2Search

        ''' <summary>
        ''' a stream data reader for the reference spectrum
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property spectrum As SpectrumReader
        ''' <summary>
        ''' cutoff of the cos similarity
        ''' </summary>
        Public ReadOnly Property dotcutoff As Double = 0.6
        Public ReadOnly Property libnames As String()
            Get
                Return spectrum.GetAllLibNames.ToArray
            End Get
        End Property

        ''' <summary>
        ''' the library size, get the number of the spectrum in current reference library
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' map from <see cref="SpectrumReader.nspectrum"/>
        ''' </remarks>
        Public ReadOnly Property size As Integer
            Get
                Return spectrum.nspectrum
            End Get
        End Property

        ''' <summary>
        ''' make the spectrum alignment of <see cref="Search"/> parallel?
        ''' </summary>
        ''' <returns></returns>
        Public Property parallel As Boolean = True

        Sub New(pack As SpectrumReader, Optional dotcutoff As Double = 0.6)
            Call MyBase.New

            ' set the reference spectrum library
            Me.spectrum = pack
            Me.dotcutoff = dotcutoff
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="mz1">the reference spectrum will be filtered via the precursor ion.</param>
        ''' <returns>
        ''' populate output all of the possible candidates hits via the
        ''' spectrum alignment operation. All of the hit result that 
        ''' populate out from this function matched the <see cref="dotcutoff"/>
        ''' threshold condition.
        ''' </returns>
        Public Overrides Iterator Function Search(centroid() As ms2, mz1 As Double) As IEnumerable(Of ClusterHit)
            ' get spectrum reference via matches the precursor ions
            Dim candidates As BlockNode() = spectrum.QueryByMz(mz1).ToArray
            Dim hits As ___tmp()

            If False Then
                If centroid Is Nothing OrElse centroid.Any(Function(mzi) mzi Is Nothing) Then
                    Throw New InvalidProgramException($"Unexpected null reference of the search query input collection, precursor ion: {mz1}!")
                End If
                If candidates Is Nothing OrElse candidates.Any(Function(ci) ci Is Nothing) Then
                    Throw New InvalidProgramException($"Unexpected null reference of the search reference candidates collection, precursor ion: {mz1}!")
                End If
            End If

            If parallel Then
                hits = SearchParallel(centroid, candidates).ToArray
            Else
                hits = SearchSequential(centroid, candidates).ToArray
            End If

            ' hits may contains multiple metabolite reference data
            ' multiple cluster object should be populates from
            ' this function?
            If hits.Length > 0 Then
                For Each metabolite In hits.GroupBy(Function(i) i.id)
                    Yield reportClusterHit(centroid, hit_group:=metabolite)
                Next
            End If
        End Function

        ''' <summary>
        ''' force run parallel alignment with 4 threads
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="candidates"></param>
        ''' <returns></returns>
        Private Function SearchParallel(centroid() As ms2, candidates As BlockNode(), Optional n_threads As Integer = 8) As IEnumerable(Of ___tmp)
            Return candidates _
                .Where(Function(c) spectrum.HasMapName(c.Id)) _
                .AsParallel _
                .WithDegreeOfParallelism(n_threads) _
                .Select(Function(hit)
                            Dim align = GlobalAlignment.CreateAlignment(centroid, hit.centroid, da).ToArray
                            Dim score = CosAlignment.GetCosScore(align)
                            Dim min = std.Min(score.forward, score.reverse)

                            ' if the spectrum cos dotcutoff
                            ' matches the cutoff threshold then we have a candidate hit
                            If min > dotcutoff Then
                                Return New ___tmp With {
                                    .id = spectrum(libname:=hit.Id),
                                    .hit = hit,
                                    .align = align,
                                    .forward = score.forward,
                                    .reverse = score.reverse
                                }
                            Else
                                Return Nothing
                            End If
                        End Function) _
                .Where(Function(c)
                           Return Not c.hit Is Nothing
                       End Function)
        End Function

        Private Iterator Function SearchSequential(centroid() As ms2, candidates As BlockNode()) As IEnumerable(Of ___tmp)
            ' do spectrum alignment for all matched
            ' spectrum candidates
            For Each hit As BlockNode In candidates
                If Not spectrum.HasMapName(hit.Id) Then
                    ' skip data for reduce the alignment math time
                    Continue For
                End If

                Dim align = GlobalAlignment.CreateAlignment(centroid, hit.centroid, da).ToArray
                Dim score = CosAlignment.GetCosScore(align)
                Dim min = std.Min(score.forward, score.reverse)

                ' if the spectrum cos dotcutoff
                ' matches the cutoff threshold then we have a candidate hit
                If min > dotcutoff Then
                    Yield New ___tmp With {
                        .id = spectrum(libname:=hit.Id),
                        .hit = hit,
                        .align = align,
                        .forward = score.forward,
                        .reverse = score.reverse
                    }
                End If
            Next
        End Function

        ''' <summary>
        ''' pull all reference spectrum inside current library object
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetReferenceSpectrum(Optional tqdm_verbose As Boolean = True) As IEnumerable(Of PeakMs2)
            Dim offsets As IEnumerable(Of MassIndex)

            If tqdm_verbose Then
                offsets = Tqdm.Wrap(spectrum.LoadMass().ToArray)
            Else
                offsets = spectrum.LoadMass
            End If

            ' load mass returns mass set which already been filter 
            ' by the given target reference id set
            For Each meta As MassIndex In offsets
                For Each spec As PeakMs2 In spectrum.GetSpectrum(meta)
                    Yield spec
                Next
            Next
        End Function

        ''' <summary>
        ''' the temp result data tuple object
        ''' </summary>
        Private Structure ___tmp

            Dim id As String
            Dim hit As BlockNode
            Dim align As SSM2MatrixFragment()
            Dim forward As Double
            Dim reverse As Double

        End Structure

        ''' <summary>
        ''' report the cluster alignment result
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="hit_group"></param>
        ''' <returns></returns>
        Private Function reportClusterHit(centroid() As ms2, hit_group As IGrouping(Of String, ___tmp)) As ClusterHit
            Dim desc = hit_group.OrderByDescending(Function(n) std.Min(n.forward, n.reverse)).ToArray
            Dim max = desc.First
            Dim forward = desc.Select(Function(n) n.forward).ToArray
            Dim reverse = desc.Select(Function(n) n.reverse).ToArray
            Dim jaccard = desc.Select(Function(n) JaccardAlignment.GetJaccardScore(n.align)).ToArray
            Dim entropy = desc _
                .Select(Function(c)
                            Return SpectralEntropy.calculate_entropy_similarity(centroid, c.hit.centroid, da)
                        End Function) _
                .ToArray

            Return New ClusterHit With {
                .Id = hit_group.Key,
                .forward = forward.Average,
                .reverse = reverse.Average,
                .ClusterEntropy = entropy,
                .entropy = entropy.Average,
                .ClusterForward = forward,
                .ClusterReverse = reverse,
                .ClusterJaccard = jaccard,
                .jaccard = jaccard.Average,
                .ClusterId = desc.Select(Function(i) i.hit.Id).ToArray,
                .ClusterRt = desc.Select(Function(i) i.hit.rt).ToArray,
                .representive = max.align
            }
        End Function
    End Class
End Namespace
