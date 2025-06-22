Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime

Public Class SearchVector : Inherits ISearchOp

    Public Sub New(repo As IMzQuery, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)
        Throw New NotImplementedException()
    End Function
End Class
