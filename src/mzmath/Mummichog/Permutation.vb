Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Module Permutation

    <Extension>
    Public Iterator Function CreateCombinations(input As IEnumerable(Of MzSet), permutations As Integer) As IEnumerable(Of MzQuery())
        Dim raw As MzSet() = input.ToArray
        Dim block As New List(Of MzQuery)
        Dim target As MzQuery
        Dim filter As Index(Of String) = {}

        For i As Integer = 0 To permutations
            Call filter.Clear()
            Call block.Clear()

            For Each mz As MzSet In raw
                target = mz.GetRandom(filter)

                If Not target.unique_id Is Nothing Then
                    Call block.Add(target)
                    Call filter.Add(target.unique_id)
                End If
            Next

            Yield block _
                .GroupBy(Function(a) a.unique_id) _
                .Select(Function(a) a.OrderBy(Function(v) v.ppm).First) _
                .ToArray
        Next
    End Function

    <Extension>
    Private Function GetRandom(mzset As MzSet, filter As Index(Of String)) As MzQuery
        Dim filters = mzset.query _
            .Where(Function(a) Not a.unique_id Like filter) _
            .ToArray

        If filters.Length = 0 Then
            Return Nothing
        Else
            Dim i As Integer = randf.NextInteger(mzset.size)
            Dim target As MzQuery = mzset(i)

            Return target
        End If
    End Function
End Module
