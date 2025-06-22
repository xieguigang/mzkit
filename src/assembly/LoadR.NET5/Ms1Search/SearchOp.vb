Imports BioNovoGene.BioDeep.MSEngine

Public MustInherit Class SearchOp

    Protected repo As IMzQuery
    Protected unique As Boolean
    Protected uniqueByScore As Boolean
    Protected field_mz As String
    Protected field_score As String
    Protected env As Environment

    Sub New(repo As IMzQuery,
            unique As Boolean,
            uniqueByScore As Boolean,
            field_mz As String,
            field_score As String,
            env As Environment)

        Me.repo = repo
        Me.unique = unique
        Me.uniqueByScore = uniqueByScore
        Me.field_mz = field_mz
        Me.field_score = field_score
        Me.env = env
    End Sub

    Public MustOverride Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)
    Public MustOverride Function SearchUnique(mz As Object) As IEnumerable(Of MzSearch)

End Class

Public Class MzSearch : Inherits MzQuery

    Public Property metadata As Dictionary(Of String, Object)

End Class