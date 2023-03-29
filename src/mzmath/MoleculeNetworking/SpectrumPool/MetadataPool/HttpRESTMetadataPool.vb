
Imports System.Collections.Specialized
Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    Public Class HttpRESTMetadataPool : Inherits MetadataProxy

        Dim local_cache As New Dictionary(Of String, Metadata)
        Dim url_get As String
        Dim url_put As String
        Dim url_setRoot As String
        Dim hash_index As String
        Dim cluster_data As JavaScriptObject
        Dim rootId As String

        Public ReadOnly Property guid As Long
            Get
                Return Val(cluster_data!id)
            End Get
        End Property

        Public ReadOnly Property RootSpectrumId As String
            Get
                If rootId.StringEmpty Then
                    Dim root = cluster_data!root

                    If root Is Nothing Then
                        Return Nothing
                    Else
                        rootId = CStr(root)
                    End If
                End If

                Return rootId
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id">
        ''' the spectrum md5 hashcode used as guid
        ''' </param>
        ''' <returns></returns>
        Default Public Overrides ReadOnly Property GetById(id As String) As Metadata
            Get
                If Not local_cache.ContainsKey(id) Then
                    Call local_cache.Add(id, GetMetadataByHashKey(id))
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
            Me.url_get = $"{http.base}/get/metadata/"
            Me.url_put = $"{http.base}/set/metadata/"
            Me.url_setRoot = $"{http.base}/set/root/"

            Dim url As String = $"{http.base}/get/cluster/?path_hash={hash_index}"
            Dim json As String = url.GET
            Dim obj As Restful = Restful.ParseJSON(json)

            If obj.code = 404 Then
                ' create new
                Dim payload As New NameValueCollection
                payload.Add("parent", parentId)
                payload.Add("key", path.BaseName)
                payload.Add("hashcode", hash_index)
                payload.Add("depth", path.Split("/"c).Length)
                url = $"{http.base}/new/cluster/"
                obj = Restful.ParseJSON(url.POST(payload))
                Me.cluster_data = obj.info
            Else
                Me.cluster_data = obj.info
            End If
        End Sub

        Public Function GetMetadataByHashKey(hash As String) As Metadata
            Dim url As String = $"{url_get}?id={hash}"
            Dim json As String = url.GET
            Dim obj As Restful = Restful.ParseJSON(json)

            If obj.code <> 0 Then
                Call VBDebugger.EchoLine(obj.debug)
                Return Nothing
            End If

            Dim fetch As JavaScriptObject = obj.info
            Dim data As New Metadata With {
                .adducts = fetch!adducts,
                .biodeep_id = fetch!biodeep_id,
                .block = New BufferRegion With {.position = Long.Parse(CStr(fetch!spectral_id))},
                .formula = fetch!formula,
                .guid = fetch!hashcode,
                .intensity = Val(fetch!intensity),
                .mz = Val(fetch!mz),
                .name = fetch!name,
                .organism = fetch!organism,
                .rt = Val(fetch!rt),
                .sample_source = fetch!biosample,
                .source_file = fetch!filename
            }

            Return data
        End Function

        ''' <summary>
        ''' write metadata to database at here
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="metadata"></param>
        Public Overrides Sub Add(id As String, metadata As Metadata)
            Dim payload As New NameValueCollection

            Call payload.Add("spectrum_id", metadata.block.position)
            Call payload.Add("metadata", metadata.GetJson(simpleDict:=True))
            Call payload.Add("cluster_id", Me.guid)

            Dim result = Restful.ParseJSON(url_put.POST(payload))

            If result.code = 0 Then
                Call local_cache.Add(id, metadata)
            End If
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return Me(id) IsNot Nothing
        End Function

        Public Overrides Sub SetRootId(hashcode As String)
            rootId = hashcode

            Dim args As New NameValueCollection

            args.Add("path_hash", hash_index)
            args.Add("id", hashcode)

            Dim result = Restful.ParseJSON(url_setRoot.POST(args))

            If result.code <> 0 Then
                Call VBDebugger.EchoLine(result.debug)
            End If
        End Sub
    End Class
End Namespace