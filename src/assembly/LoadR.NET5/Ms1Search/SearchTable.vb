Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class SearchTable : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Iterator Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Dim table As dataframe = DirectCast(mz, dataframe)

        For Each row As list In table.getRowList
            Dim mzi As Double = CLRVector.asNumeric(row(field_mz)).First
            Dim query As MzQuery() = repo.QueryByMz(mzi).ToArray
            Dim meta As KeyValuePair(Of String, Object)() = row.slots.ToArray
            Dim score As Double = 1.0

            If Not field_score.StringEmpty(, True) AndAlso row.hasName(field_score) Then
                score = CLRVector.asNumeric(row.getByName(field_score)).First
            End If

            For Each hit As MzQuery In query
                Dim result As New MzSearch(hit, 0)
                result.metadata = result.metadata.AddRange(meta, replaceDuplicated:=True)
                result.score = score * result.score
                result.metadata("index") = CStr(row.getAttribute("rowname"))
                Yield result
            Next
        Next
    End Function

    Protected Overrides Iterator Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)
        Dim groups = all.GroupBy(Function(a) CStr(a!index)).ToArray

        For Each group As IGrouping(Of Integer, MzSearch) In groups
            If uniqueByScore Then
                ' get top score
                Yield group.OrderByDescending(Function(a) a.score).First
            Else
                ' get min ppm
                Yield group.OrderBy(Function(a) a.ppm).First
            End If
        Next
    End Function
End Class
