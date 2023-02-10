#Region "Microsoft.VisualBasic::bccdea2dc76081e0ecfbec18b19c38e4, mzkit\src\mzmath\Mummichog\Annotation.vb"

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

    '   Total Lines: 104
    '    Code Lines: 90
    ' Comment Lines: 1
    '   Blank Lines: 13
    '     File Size: 4.02 KB


    ' Module Annotation
    ' 
    '     Function: GetCandidateSet, PeakListAnnotation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Public Module Annotation

    <Extension>
    Public Function PeakListAnnotation(candidates As IEnumerable(Of MzSet),
                                       background As IEnumerable(Of NamedValue(Of NetworkGraph)),
                                       Optional minhit As Integer = 3,
                                       Optional permutation As Integer = 100,
                                       Optional modelSize As Integer = -1,
                                       Optional pinned As String() = Nothing,
                                       Optional ignoreTopology As Boolean = False) As ActivityEnrichment()

        Dim result, tmp1 As ActivityEnrichment()
        Dim allsubgraph As NamedValue(Of NetworkGraph)() = background.ToArray
        Dim scores As IEnumerable(Of ActivityEnrichment)
        Dim maxScore As Double = -9999999
        Dim score As Double
        Dim input As Dictionary(Of String, MzQuery)
        Dim pinList As Index(Of String) = pinned.Indexing
        Dim nsize As Integer

        If modelSize <= 0 Then
            modelSize = allsubgraph _
                .Select(Function(g) g.Value.vertex) _
                .IteratesALL _
                .Select(Function(v) v.label) _
                .Distinct _
                .Count
        End If

        result = Nothing

        For Each candidateList As MzQuery() In candidates.CreateCombinations(permutation)
            input = candidateList.ToDictionary(Function(a) a.unique_id)
            scores = From graph As NamedValue(Of NetworkGraph)
                     In allsubgraph.AsParallel
                     Let query = ActivityEnrichment.Evaluate(
                         input:=input,
                         background:=graph,
                         modelSize:=modelSize,
                         pinList:=pinList,
                         ignoreTopology:=ignoreTopology
                     )
                     Where query.Background > 0 AndAlso
                         query.Input > 0 AndAlso
                         query.Activity > 0 AndAlso
                         Not query.Q.IsNaNImaginary
                     Select query
                     Order By query.Activity Descending
            tmp1 = scores.ToArray
            nsize = tmp1 _
                .Select(Function(a) a.Hits) _
                .IteratesALL _
                .GroupBy(Function(a) a.unique_id) _
                .Count

            If ignoreTopology Then
                score = Aggregate v As ActivityEnrichment
                        In tmp1
                        Let pscore As Double = If(
                            v.Fisher.two_tail_pvalue < 1.0E-100,
                            100,
                            -stdNum.Log10(v.Fisher.two_tail_pvalue)
                        )
                        Into Sum(pscore)
            Else
                score = Aggregate v As ActivityEnrichment
                        In tmp1
                        Into Sum(v.Activity)
            End If

            score *= nsize

            ' evaluate the best candidate collection
            If maxScore < score Then
                result = tmp1
                maxScore = score

                Call Console.WriteLine(maxScore)
            End If
        Next

        Return result
    End Function

    <Extension>
    Public Function GetCandidateSet(MsDb As IMzQuery, peaks As IEnumerable(Of Double)) As IEnumerable(Of MzSet)
        Return From mzi As Double
               In peaks
               Let candidates = MsDb.QueryByMz(mzi).ToArray
               Where candidates.Count > 0
               Select New MzSet With {
                   .mz = mzi,
                   .query = candidates
               }
    End Function

End Module
