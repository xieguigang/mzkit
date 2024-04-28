#Region "Microsoft.VisualBasic::916598bc18a6600c0f33eec5c7a55508, E:/mzkit/src/assembly/SpectrumTree//Query/JaccardSearch.vb"

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

    '   Total Lines: 131
    '    Code Lines: 76
    ' Comment Lines: 39
    '   Blank Lines: 16
    '     File Size: 5.71 KB


    '     Class JaccardSearch
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: score, Search
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Query

    Public Class JaccardSearch : Inherits Ms2Search

        ''' <summary>
        ''' the ms2 fragment data pack
        ''' </summary>
        ReadOnly mzSet As JaccardSet()
        ReadOnly cutoff As Double
        ReadOnly filter_complex_adducts As Boolean

        Sub New(ref As IEnumerable(Of JaccardSet), cutoff As Double, Optional filter_complex_adducts As Boolean = False)
            Call MyBase.New

            Me.mzSet = ref.ToArray
            Me.cutoff = cutoff
            Me.filter_complex_adducts = filter_complex_adducts
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="centroid">the ms2 spectrum</param>
        ''' <param name="mz1">the precursor m/z</param>
        ''' <returns></returns>
        Public Overrides Iterator Function Search(centroid() As ms2, mz1 As Double) As IEnumerable(Of ClusterHit)
            Dim query As Double() = centroid.Select(Function(i) i.mz).ToArray

            If query.Length = 0 Then
                Return
            End If
            If filter_complex_adducts Then
                ' 20230801
                ' current precursor has very complex adducts?
                ' skip of current result
                If query.Max - mz1 > 0.3 Then
                    Return
                End If
            End If

            Dim subset = mzSet.Where(Function(i) da(mz1, i.mz1)).ToArray
            Dim jaccard = subset _
                .Select(Function(i)
                            Dim itr = GlobalAlignment.MzIntersect(i.ms2, query, da)
                            Dim uni = GlobalAlignment.MzUnion(i.ms2, query, da)

                            Return (i, itr, uni)
                        End Function) _
                .Where(Function(j) j.itr.Length / j.uni.Length > cutoff) _
                .ToArray

            ' 20221001 the algorithm has been tested success
            ' do not modify the score at here

            ' get a cluster of the hit result
            If jaccard.Length > 0 Then
                ' get alignment score vector and the alignment details 
                Dim scores = jaccard.Select(Function(j) score(centroid, j.i, j.itr, j.uni)).ToArray
                ' get index of the max score
                Dim forward As New Vector(scores.Select(Function(a) a.forward))
                Dim reverse As New Vector(scores.Select(Function(a) a.reverse))
                Dim jaccard2 As New Vector(scores.Select(Function(a) a.jaccard))
                Dim i As Integer = which.Max(forward * reverse * jaccard2)
                'Dim alignments = scores _
                '    .Select(Function(a) a.alignment) _
                '    .IteratesALL _
                '    .GroupBy(Function(a) a.mz, da) _
                '    .Select(Function(a)
                '                Return New SSM2MatrixFragment With {
                '                    .mz = Val(a.name),
                '                    .da = da.DeltaTolerance,
                '                    .query = a.First.query,
                '                    .ref = a.Select(Function(ai) ai.ref).Average
                '                }
                '            End Function) _
                '    .ToArray

                ' 20221017 just returns the best one in jaccard alignment mode
                Dim best_align = scores(i)
                Dim best_hit = jaccard(i)

                Yield New ClusterHit With {
                    .jaccard = best_align.jaccard,
                    .ClusterForward = {best_align.forward},
                    .ClusterReverse = {best_align.reverse},
                    .ClusterJaccard = {best_align.jaccard},
                    .ClusterId = {best_hit.i.libname},
                    .ClusterRt = {best_hit.i.rt},
                    .Id = jaccard(i).i.libname,
                    .queryMz = mz1,
                    .forward = .ClusterForward.Average,
                    .reverse = .ClusterReverse.Average,
                    .representive = best_align.alignment
                }
            End If
        End Function

        ''' <summary>
        ''' create jaccard data alignment result
        ''' </summary>
        ''' <param name="centroid"></param>
        ''' <param name="ref"></param>
        ''' <param name="itr"></param>
        ''' <param name="uni"></param>
        ''' <returns></returns>
        Private Function score(centroid() As ms2,
                               ref As JaccardSet,
                               itr As Double(),
                               uni As Double()) As (jaccard As Double, forward As Double, reverse As Double, alignment As SSM2MatrixFragment())

            Dim jaccard As Double = itr.Length / uni.Length
            Dim hits = itr _
                .Select(Function(mzi)
                            Return centroid _
                                .Where(Function(m) da(m.mz, mzi)) _
                                .OrderByDescending(Function(m) m.intensity) _
                                .First
                        End Function) _
                .ToArray
            Dim cos = GlobalAlignment.TwoDirectionSSM(centroid, hits, da)
            Dim align = GlobalAlignment.CreateAlignment(centroid, hits, da).ToArray

            Return (jaccard, cos.forward, cos.reverse, align)
        End Function
    End Class
End Namespace
