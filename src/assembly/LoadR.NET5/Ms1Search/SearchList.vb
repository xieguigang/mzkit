Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class SearchList : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Iterator Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Dim list As list = DirectCast(mz, list)

        For Each item As KeyValuePair(Of String, Object) In list.slots
            For Each result As MzSearch In QueryItem(item)
                result.metadata("index") = item.Key
                Yield result
            Next
        Next
    End Function

    Private Function QueryItem(item As KeyValuePair(Of String, Object)) As IEnumerable(Of MzSearch)
        If TypeOf item.Value Is list Then
            Dim sublist As list = DirectCast(item.Value, list)
            Dim mz As Double() = CLRVector.asNumeric(sublist.getByName(field_mz))
            Dim scores As Double() = 1.0.Repeats(mz.Length)

            If Not field_score.StringEmpty(, True) AndAlso sublist.hasName(field_score) Then
                scores = CLRVector.asNumeric(sublist.getByName(field_score))
            End If


        Else
            Dim mz As Double() = CLRVector.asNumeric(item.Value)

            Return mz _
                .Select(Function(mzi) repo.QueryByMz(mzi)) _
                .IteratesALL
        End If
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
