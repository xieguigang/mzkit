Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class SearchVector : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Return QueryMz(CLRVector.asNumeric(mz))
    End Function

    Private Iterator Function QueryMz(mz As Double()) As IEnumerable(Of MzSearch())
        Dim index As Integer = 1

        For Each mzi As Double In mz
            Dim all As MzQuery() = repo.QueryByMz(mzi).ToArray
            Dim pops As MzSearch() = all.Select(Function(i) New MzSearch(i, index)).toarray

            index += 1

            Yield pops
        Next
    End Function

    Protected Overrides Iterator Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)
        Dim groups = all.GroupBy(Function(a) CInt(a!index)).ToArray

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
