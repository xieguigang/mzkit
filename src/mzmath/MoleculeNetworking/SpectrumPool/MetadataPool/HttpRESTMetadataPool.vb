#Region "Microsoft.VisualBasic::2ed7fd62990b089d935a58d20dbe523d, E:/mzkit/src/mzmath/MoleculeNetworking//SpectrumPool/MetadataPool/HttpRESTMetadataPool.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 300
    '    Code Lines: 203
    ' Comment Lines: 50
    '   Blank Lines: 47
    '     File Size: 11.31 KB


    '     Class HttpRESTMetadataPool
    ' 
    '         Properties: AllClusterMembers, Depth, guid, RootId
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: castMetaData, FetchClusterData, GetMetadataByHashKey, HasGuid, ParseMetadata
    ' 
    '         Sub: (+2 Overloads) Add, SetRootId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    Public Class HttpRESTMetadataPool : Inherits MetadataProxy

        ''' <summary>
        ''' metadata cache of current cluster node
        ''' </summary>
        Dim local_cache As Dictionary(Of String, Metadata)
        Dim url_get As String
        Dim url_put As String
        Dim url_setRoot As String
        Dim url_setScore As String
        Dim hash_index As String
        Dim cluster_data As JavaScriptObject
        Dim model_id As String

        Dim m_depth As Integer = 0
        Dim m_rootId As String = Nothing

        ''' <summary>
        ''' the cluster id in the database
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property guid As Long
            Get
                Return Val(cluster_data!id)
            End Get
        End Property

        ''' <summary>
        ''' the root spectrum id of current cluster
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property RootId As String
            Get
                If m_rootId.StringEmpty Then
                    Dim root = cluster_data!root

                    If root Is Nothing Then
                        Return Nothing
                    Else
                        m_rootId = CStr(root)
                    End If
                End If

                Return m_rootId
            End Get
        End Property

        Public Overrides ReadOnly Property Depth As Integer
            Get
                Return m_depth
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
                Return FetchClusterData(url_get, hash_index, model_id)
            End Get
        End Property

        ''' <summary>
        ''' open existed or create new cluster node
        ''' </summary>
        ''' <param name="http"></param>
        ''' <param name="path"></param>
        ''' <param name="parentId"></param>
        Sub New(http As HttpTreeFs, path As String, parentId As Long)
            Me.New(http)
            Me.hash_index = HttpTreeFs.ClusterHashIndex(path)

            Dim url As String = $"{http.base}/get/cluster/?path_hash={hash_index}&model_id={http.model_id}"
            Dim json As String = url.GET
            Dim obj As Restful = Restful.ParseJSON(json)

            If obj.code = 404 Then
                ' create new
                Dim payload As New NameValueCollection

                payload.Add("parent", parentId)
                payload.Add("key", path.BaseName)
                payload.Add("hashcode", hash_index)
                payload.Add("depth", path.Split("/"c).Length)

                url = $"{http.base}/new/cluster/?model_id={http.model_id}"
                obj = Restful.ParseJSON(url.POST(payload))
            End If

            Me.cluster_data = obj.info
            Me.m_depth = Val((cluster_data!depth).ToString)
        End Sub

        ''' <summary>
        ''' common pathway for initialize the cluster node data pool
        ''' </summary>
        ''' <param name="http"></param>
        Private Sub New(http As HttpTreeFs)
            Me.url_get = $"{http.base}/get/metadata/"
            Me.url_put = $"{http.base}/set/metadata/?model_id={http.model_id}"
            Me.url_setRoot = $"{http.base}/set/root/?model_id={http.model_id}"
            Me.url_setScore = $"{http.base}/set/score/?model_id={http.model_id}"
            Me.model_id = http.model_id
            Me.local_cache = New Dictionary(Of String, Metadata)
        End Sub

        ''' <summary>
        ''' open existsed cluster node
        ''' </summary>
        ''' <param name="http"></param>
        ''' <param name="cluster_id"></param>
        Sub New(http As HttpTreeFs, cluster_id As Integer)
            Me.New(http)

            Dim url As String = $"{http.base}/get/cluster/?id={cluster_id}&model_id={http.model_id}"
            Dim json As String = url.GET
            Dim obj As Restful = Restful.ParseJSON(json)

            If obj.code <> 0 Then
                Throw New MissingMemberException($"No cluster which its id is: '{cluster_id}'!")
            Else
                Me.cluster_data = obj.info
                Me.m_depth = Val((cluster_data!depth).ToString)
                Me.hash_index = CStr(cluster_data!hash_index)
            End If
        End Sub

        ''' <summary>
        ''' get metabolite ion data from a specific spectrum cluster
        ''' </summary>
        ''' <param name="url_get"></param>
        ''' <param name="hash_index"></param>
        ''' <param name="model_id"></param>
        ''' <returns></returns>
        Public Shared Iterator Function FetchClusterData(url_get As String,
                                                         hash_index As String,
                                                         model_id As String) As IEnumerable(Of Metadata)

            Dim url As String = $"{url_get}?id={hash_index}&is_cluster=true&model_id={model_id}"
            Dim json As String = url.GET
            Dim list As Restful = Restful.ParseJSON(json)

            If list.code <> 0 Then
                Return
            End If

            Dim info As JavaScriptObject = list.info
            Dim array As Array = info!metabolites

            For i As Integer = 0 To array.Length - 1
                Yield ParseMetadata(array(i))
            Next
        End Function

        Private Shared Function castMetaData(fetch As JavaScriptObject) As Metadata
            Return New Metadata With {
                .adducts = fetch!adducts,
                .biodeep_id = fetch!biodeep_id,
                .formula = fetch!formula,
                .guid = fetch!hashcode,
                .intensity = Val(fetch!intensity),
                .mz = Val(fetch!mz),
                .name = fetch!name,
                .organism = fetch!organism,
                .rt = Val(fetch!rt),
                .sample_source = fetch!biosample,
                .source_file = fetch!filename,
                .instrument = fetch!instrument,
                .project = fetch!project
            }
        End Function

        Public Shared Function ParseMetadata(fetch As JavaScriptObject) As Metadata
            Dim data As Metadata = castMetaData(fetch)

            If fetch!spectral_id IsNot Nothing Then
                data.block = New BufferRegion With {
                    .position = Long.Parse(CStr(fetch!spectral_id))
                }
            End If

            Return data
        End Function

        Public Function GetMetadataByHashKey(hash As String) As Metadata
            Dim url As String = $"{url_get}?id={hash}&model_id={model_id}&cluster_id={guid}"
            Dim json As String = url.GET
            Dim obj As Restful = Restful.ParseJSON(json)

            If obj.code <> 0 Then
                Call VBDebugger.EchoLine(obj.debug)
                Return Nothing
            Else
                Return ParseMetadata(fetch:=obj.info)
            End If
        End Function

        ''' <summary>
        ''' write metadata to database at here
        ''' </summary>
        ''' <param name="id">the spectrum hashcode</param>
        ''' <param name="metadata"></param>
        Public Overrides Sub Add(id As String, metadata As Metadata)
            Dim payload As New NameValueCollection

            Call payload.Add("spectrum_id", metadata.block.position)
            Call payload.Add("metadata", metadata.GetJson(simpleDict:=True))
            Call payload.Add("cluster_id", Me.guid)

            Dim result = Restful.ParseJSON(url_put.POST(payload))

            If result.code <= 0 Then
                local_cache(id) = metadata
            Else
                Call VBDebugger.EchoLine(result.debug)
                Call VBDebugger.EchoLine(result.info)
            End If
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return Me(id) IsNot Nothing
        End Function

        Public Overrides Sub SetRootId(hashcode As String)
            Dim args As New NameValueCollection

            m_rootId = hashcode
            ' build url parameters
            args.Add("path_hash", hash_index)
            args.Add("id", hashcode)

            Dim result = Restful.ParseJSON(url_setRoot.POST(args))

            If result.code <> 0 Then
                Call VBDebugger.EchoLine(result.debug)
            End If
        End Sub

        Public Overrides Sub Add(id As String, score As Double, align As AlignmentOutput, pval As Double)
            Dim payload As New NameValueCollection
            Dim metadata As Metadata = local_cache(id)

            If align Is Nothing Then
                ' config for root
                Call payload.Add("n_hits", 0)
                Call payload.Add("consensus", "*")
                Call payload.Add("forward", 1)
                Call payload.Add("reverse", 1)
                Call payload.Add("jaccard", 1)
                Call payload.Add("entropy", 1)
            Else
                ' member spectrum align with root spectrum 
                ' of current cluster
                Dim consensus As Double() = align.alignments _
                    .Where(Function(a) a.query > 0 AndAlso a.ref > 0) _
                    .Select(Function(a) a.mz) _
                    .ToArray

                Call payload.Add("n_hits", consensus.Length)
                Call payload.Add("consensus", consensus.Select(AddressOf NetworkByteOrderBitConvertor.GetBytes).IteratesALL.ToBase64String)
                Call payload.Add("forward", align.forward)
                Call payload.Add("reverse", align.reverse)
                Call payload.Add("jaccard", align.jaccard)
                Call payload.Add("entropy", align.entropy)
            End If

            Call payload.Add("p_value", pval)
            Call payload.Add("score", score)
            Call payload.Add("spectrum_id", metadata.block.position)
            Call payload.Add("cluster_id", Me.guid)

            Call url_setScore.POST(payload)
        End Sub
    End Class
End Namespace
