Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Friend Class QueryWeb : Inherits WebQuery(Of (name_query As Boolean, q As String))

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        Call MyBase.New(
            url:=AddressOf getRestUrl,
            contextGuid:=Function(id) $"{id.name_query}+{id.q}".MD5,
            parser:=AddressOf parseJSON,
            prefix:=Function(id) id.Substring(0, 2),
            cache:=cache,
            interval:=interval,
            offline:=offline
        )
    End Sub

    Private Shared Function parseJSON(json As String, schema As Type) As Object
        If schema Is GetType(WebJSON) Then
            Return json.LoadJSON(Of WebJSON)
        ElseIf schema Is GetType(WebJSON()) Then
            Return json.LoadJSON(Of WebJSON())
        Else
            Throw New NotImplementedException
        End If
    End Function

    Private Shared Function getRestUrl(context As (name_query As Boolean, q As String)) As String
        If context.name_query Then
            Return WebJSON.query.Replace("{q}", context.q.UrlEncode)
        Else
            Return sprintf(WebJSON.urlBase, context.q)
        End If
    End Function
End Class