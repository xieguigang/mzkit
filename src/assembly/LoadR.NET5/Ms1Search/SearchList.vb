Imports BioNovoGene.BioDeep.MSEngine

Public Class SearchList : Inherits SearchOp

    Public Sub New(repo As IMzQuery, unique As Boolean, uniqueByScore As Boolean, field_mz As String, field_score As String, env As Environment)
        MyBase.New(repo, unique, uniqueByScore, field_mz, field_score, env)
    End Sub

    Public Overrides Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
        Throw New NotImplementedException()
    End Function

    Public Overrides Function SearchUnique(mz As Object) As IEnumerable(Of MzSearch)
        Throw New NotImplementedException()
    End Function
End Class
