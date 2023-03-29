
Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    Public Class HttpRESTMetadataPool : Inherits MetadataProxy

        Dim local_cache As New Dictionary(Of String, Metadata)
        Dim url_get As String
        Dim url_put As String
        Dim hash_index As String
        Dim cluster_data As JavaScriptObject

        Public ReadOnly Property guid As Long
            Get
                Return Val(cluster_data!id)
            End Get
        End Property

        Public ReadOnly Property RootSpectrumId As String
            Get
                Dim root = cluster_data!root

                If root Is Nothing Then
                    Return Nothing
                End If

                Return root
            End Get
        End Property

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

        Sub New(http As HttpTreeFs, path As String, parentId As Long)
            Me.hash_index = HttpTreeFs.ClusterHashIndex(path)
            Me.url_get = $"{http.base}/get/metadata"
            Me.url_put = $"{http.base}/set/metadata"

            Dim url As String = $"{http.base}/get/cluster/?path_hash={hash_index}"
            Dim json As String = url.GET
            Dim obj As Restful = Restful.ParseJSON(json)

            If obj.code = 404 Then
                ' create new
                Dim payload As New NameValueCollection
                payload.Add("parent", parentId)
                payload.Add("key", path.BaseName)
                payload.Add("hashcode", hash_index)
                url = $"{http.base}/new/cluster/"
                obj = Restful.ParseJSON(url.POST(payload))
                Me.cluster_data = obj.info
            Else
                Me.cluster_data = obj.info
            End If
        End Sub

        ''' <summary>
        ''' write metadata to database at here
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="metadata"></param>
        Public Overrides Sub Add(id As String, metadata As Metadata)
            Dim payload As New NameValueCollection

            Call payload.Add("spectrum_id", metadata.block.position)
            Call payload.Add("metadata", metadata.GetJson(simpleDict:=True))

            Dim result = Restful.ParseJSON(url_put.POST(payload))

            If result.code = 0 Then
                Call local_cache.Add(id, metadata)
            End If
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return Me(id) IsNot Nothing
        End Function
    End Class
End Namespace