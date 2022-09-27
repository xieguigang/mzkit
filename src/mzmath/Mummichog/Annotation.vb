Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq

Public Module Annotation

    <Extension>
    Public Function PeakListAnnotation(candidates As IEnumerable(Of MzSet),
                                       background As IEnumerable(Of NamedValue(Of NetworkGraph)),
                                       Optional minhit As Integer = 3,
                                       Optional permutation As Integer = 100,
                                       Optional modelSize As Integer = -1,
                                       Optional pinned As String() = Nothing) As ActivityEnrichment()

        Dim result, tmp1 As ActivityEnrichment()
        Dim allsubgraph As NamedValue(Of NetworkGraph)() = background.ToArray
        Dim scores As IEnumerable(Of ActivityEnrichment)
        Dim maxScore As Double = -9999999
        Dim score As Double
        Dim input As Dictionary(Of String, MzQuery)
        Dim pinList As Index(Of String) = pinned.Indexing

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
                         pinList:=pinList
                     )
                     Where query.Background > 0 AndAlso Not query.Q.IsNaNImaginary
                     Select query
                     Order By query.Activity Descending
            tmp1 = scores.ToArray
            score = Aggregate v As ActivityEnrichment
                    In tmp1
                    Into Sum(v.Activity)

            ' evaluate the best candidate collection
            If maxScore < score Then
                result = tmp1
                maxScore = score
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

