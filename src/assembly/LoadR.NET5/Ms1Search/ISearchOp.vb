Imports BioNovoGene.BioDeep.MSEngine
Imports SMRUCC.Rsharp.Runtime

Public MustInherit Class ISearchOp

    Protected repo As IMzQuery

    Protected uniqueByScore As Boolean
    Protected field_mz As String
    Protected field_score As String
    Protected env As Environment

    Sub New(repo As IMzQuery,
            uniqueByScore As Boolean,
            field_mz As String,
            field_score As String,
            env As Environment)

        Me.repo = repo

        Me.uniqueByScore = uniqueByScore
        Me.field_mz = field_mz
        Me.field_score = field_score
        Me.env = env
    End Sub

    Public MustOverride Function SearchAll(mz As Object) As IEnumerable(Of MzSearch)

    Public Function SearchUnique(mz As Object) As IEnumerable(Of MzSearch)
        Return UniqueResult(SearchAll(mz))
    End Function

    Protected MustOverride Function UniqueResult(all As IEnumerable(Of MzSearch)) As IEnumerable(Of MzSearch)

End Class

Public Class MzSearch : Inherits MzQuery

    ''' <summary>
    ''' the additional metadata informartin of the search result.
    ''' this is a key-value pair dictionary that can be used to store
    ''' additional information about the search result, such as the index
    ''' of the result in the original data source, or any other metadata
    ''' that may be relevant to the search result.
    ''' </summary>
    ''' <returns></returns>
    Public Property metadata As Dictionary(Of String, String)

    Default Public Property Item(key As String) As String
        Get
            Return If(metadata.ContainsKey(key), metadata(key), Nothing)
        End Get
        Set
            metadata(key) = Value
        End Set
    End Property

    ''' <summary>
    ''' create a new search result object for the given m/z query.
    ''' </summary>
    Sub New()
    End Sub

    Sub New(copy As MzQuery, index As Integer)
        Call MyBase.New(copy)

        Me.metadata = New Dictionary(Of String, String) From {
            {"index", index}
        }
    End Sub

End Class