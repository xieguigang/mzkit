
Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    Public Class HttpRESTMetadataPool : Inherits MetadataProxy

        Dim local_cache As New Dictionary(Of String, Metadata)
        Dim url_get As String
        Dim url_put As String
        Dim path As String

        Default Public Overrides ReadOnly Property GetById(id As String) As Metadata
            Get
                If Not local_cache.ContainsKey(id) Then
                    Dim json As String = $"{url_get}&guid={id}".GET
                    Dim data As Metadata = json.LoadJSON(Of Metadata)

                    Call local_cache.Add(data.guid, data)
                End If

                Return local_cache.TryGetValue(id)
            End Get
        End Property

        Public Overrides ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)
            Get
                Dim json As String = url_get.GET
                Dim data As Dictionary(Of String, Metadata) = json.LoadJSON(Of Dictionary(Of String, Metadata))
                local_cache = data
                Return data
            End Get
        End Property

        Sub New(http As HttpTreeFs, path As String)
            Me.path = path
            Me.url_get = $"{http.base}/get/metadata"
            Me.url_put = $"{http.base}/set/metadata"
        End Sub

        Public Overrides Sub Add(id As String, metadata As Metadata)
            Dim payload As New NameValueCollection

            Call payload.Add("metadata", metadata.GetJson)
            Call url_put.POST(payload)
            Call local_cache.Add(id, metadata)
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return Me(id) IsNot Nothing
        End Function
    End Class
End Namespace