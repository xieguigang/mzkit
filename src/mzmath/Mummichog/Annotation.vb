Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq

Public Module Annotation

    <Extension>
    Public Function PeakListAnnotation(candidates As IEnumerable(Of MzSet),
                                       background As IEnumerable(Of NamedValue(Of NetworkGraph)),
                                       Optional minhit As Integer = 3,
                                       Optional permutation As Integer = 100,
                                       Optional modelSize As Integer = -1) As ActivityEnrichment()

        Dim result, tmp1 As ActivityEnrichment()
        Dim allsubgraph As NamedValue(Of NetworkGraph)() = background.ToArray
        Dim input As String()
        Dim scores As IEnumerable(Of ActivityEnrichment)
        Dim maxScore As Double = -9999999
        Dim score As Double

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
            input = candidateList.Select(Function(c) c.unique_id).ToArray
            scores = From graph As NamedValue(Of NetworkGraph)
                     In allsubgraph.AsParallel
                     Select ActivityEnrichment.Evaluate(input, background:=graph, modelSize:=modelSize)
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

