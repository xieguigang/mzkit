#Region "Microsoft.VisualBasic::dc861883c13230fc9f65459ae91b86b7, E:/mzkit/src/mzmath/MoleculeNetworking//SpectrumPool/FileSystem/HttpTreeFs.vb"

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

    '   Total Lines: 321
    '    Code Lines: 216
    ' Comment Lines: 55
    '   Blank Lines: 50
    '     File Size: 12.44 KB


    '     Class HttpTreeFs
    ' 
    '         Properties: HttpServices, model_id, RootHashIndex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CheckExists, ClusterHashIndex, CreateModel, decode, DecodeConsensus
    '                   encode, FindRootId, GetCluster, getParentId, GetTreeChilds
    '                   (+2 Overloads) LoadMetadata, (+2 Overloads) ReadSpectrum, WriteSpectrum
    ' 
    '         Sub: Close, CommitMetadata, SetRootId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    Public Class HttpTreeFs : Inherits PoolFs

        ''' <summary>
        ''' the web services base url
        ''' </summary>
        Friend ReadOnly base As String
        Friend ReadOnly root_id As String
        Friend ReadOnly metadata_pool As New Dictionary(Of String, HttpRESTMetadataPool)
        Friend ReadOnly cluster_data As New Dictionary(Of String, JavaScriptObject)

        Public Shared ReadOnly Property RootHashIndex As String = "/".MD5.ToLower

        Public ReadOnly Property model_id As String
        Public ReadOnly Property HttpServices As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return base
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url"></param>
        Sub New(url As String, modelId As String)
            ' do system init, get model information and parameters
            Dim json = Restful.ParseJSON($"{url}/openModel/?id={modelId}".GET)

            If json.code <> 0 Then
                Throw New Exception(json.info)
            Else
                Dim args As Dictionary(Of String, String) = CStr(json.info!parameters) _
                    .LoadJSON(Of Dictionary(Of String, String))

                ' load parameters
                base = url
                model_id = json.info!model_id
                root_id = json.info!root_id
                m_level = Val(args!level)
                m_split = Val(args!split)
            End If
        End Sub

        Public Shared Function CreateModel(base As String, name As String, desc As String,
                                           Optional level As Double = 0.85,
                                           Optional split As Integer = 3) As (model_id$, root_id$)

            Dim params As New Dictionary(Of String, String) From {
                {"level", level},
                {"split", split}
            }
            Dim payload As New NameValueCollection
            Call payload.Add("params", params.GetJson)
            Call payload.Add("description", desc)
            Dim url As String = $"{base}/create_model/?name={name.UrlEncode}"
            Dim result = url.POST(payload)
            Dim json = Restful.ParseJSON(result.html)

            Call VBDebugger.EchoLine(result.html)

            If json.code <> 0 Then
                Throw New Exception(json.info)
            Else
                Return (json.info!model_id, json.info!root_id)
            End If
        End Function

        ''' <summary>
        ''' check the given spectrum object is already exists inside the database or not?
        ''' </summary>
        ''' <param name="spectral"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' <see cref="TreeFs.GetMetadata(PeakMs2)"/>
        ''' </remarks>
        Public Overrides Function CheckExists(spectral As PeakMs2) As Boolean
            ' $hashcode, $filename, $model_id, $project, $biodeep_id
            Dim hashcode As String = spectral.lib_guid
            Dim filename As String = spectral.file.UrlEncode
            Dim model_id As String = Me.model_id
            Dim project As String = spectral.meta.TryGetValue("project", [default]:="unknown project").UrlEncode
            Dim biodeep_id As String = spectral.meta.TryGetValue("biodeep_id", [default]:="unknown conserved").UrlEncode
            Dim url As String = $"{base}/check_exists/?hashcode={hashcode}&filename={filename}&model_id={model_id}&project={project}&biodeep_id={biodeep_id}"
            Dim check = Restful.ParseJSON(url.GET)

            If check.code = 0 Then
                Return CStr(check.info) = "1"
            Else
                ' unknown
                Return True
            End If
        End Function

        Public Overrides Sub CommitMetadata(path As String, data As MetadataProxy)
            ' do nothing
        End Sub

        ''' <summary>
        ''' do nothing at here
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="id"></param>
        Public Overrides Sub SetRootId(path As String, id As String)
            'Dim key As String = ClusterHashIndex(path)
            'Dim meta As MetadataProxy = metadata_pool(key)

            'Call meta.SetRootId(id)

            ' do nothing
        End Sub

        Protected Overrides Sub Close()
            ' do nothing
        End Sub

        ''' <summary>
        ''' Get cluster information data via path hash code
        ''' </summary>
        ''' <param name="key">
        ''' hashcode of the cluster tree path
        ''' </param>
        ''' <returns></returns>
        Public Function GetCluster(key As String) As JavaScriptObject
            If Not cluster_data.ContainsKey(key) Then
                Dim url As String = $"{base}/get/cluster/?path_hash={key}&model_id={model_id}"
                Dim json = Restful.ParseJSON(url.GET)

                If json.code <> 0 Then
                    Return Nothing
                Else
                    cluster_data.Add(key, json.info)
                End If
            End If

            Return cluster_data(key)
        End Function

        ''' <summary>
        ''' get tree path of the childs
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Overrides Iterator Function GetTreeChilds(path As String) As IEnumerable(Of String)
            Dim url As String = $"{base}/get/childs/?q={ClusterHashIndex(path)}&model_id={model_id}"
            Dim json As String = url.GET
            Dim data = Restful.ParseJSON(json)
            Dim childs_data As Array = data.info

            For i As Integer = 0 To childs_data.Length - 1
                Dim obj As JavaScriptObject = childs_data.GetValue(i)
                Dim key As String = Strings.Trim(CStr(obj!key))
                Dim dir As String = $"{path}/{key}"

                key = ClusterHashIndex(dir)

                If Not cluster_data.ContainsKey(key) Then
                    cluster_data.Add(key, obj)
                End If

                Yield dir
            Next
        End Function

        Public Shared Function ClusterHashIndex(ByRef path As String) As String
            path = path.StringReplace("/{2,}", "/")

            If path.Length > 1 Then
                path = path.TrimEnd("/"c)
            End If

            Return path.MD5.ToLower
        End Function

        Private Function getParentId(path As String) As Long
            If path = "/" Then
                Return -1
            Else
                Dim parent As String = path.ParentPath(full:=False)
                Dim parentHashKey As String = ClusterHashIndex(parent)
                Dim meta = metadata_pool(parentHashKey)

                Return meta.guid
            End If
        End Function

        Public Overrides Function LoadMetadata(id As Integer) As MetadataProxy
            Return New HttpRESTMetadataPool(Me, id)
        End Function

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Dim key As String = ClusterHashIndex(path)

            If Not metadata_pool.ContainsKey(key) Then
                Dim meta As New HttpRESTMetadataPool(Me, path, getParentId(path))
                metadata_pool.Add(key, meta)
                Return meta
            Else
                Return metadata_pool(key)
            End If
        End Function

        Public Overrides Function FindRootId(path As String) As String
            Dim key As String = ClusterHashIndex(path)

            If Not metadata_pool.ContainsKey(key) Then
                Return Nothing
            End If

            Return metadata_pool(key).RootId
        End Function

        ''' <summary>
        ''' the block location of the metadata is the database id of 
        ''' the target spectral data actually 
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ReadSpectrum(p As Metadata) As PeakMs2
            Return ReadSpectrum(p.block.position)
        End Function

        ''' <summary>
        ''' the block location of the metadata is the database id of 
        ''' the target spectral data actually 
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Overloads Function ReadSpectrum(p As String) As PeakMs2
            Dim url As String = $"{base}/get/spectrum/?id={p}&model_id={model_id}"
            Dim json As String = url.GET
            Dim data = Restful.ParseJSON(json)

            If data.code <> 0 Then
                Return Nothing
            End If

            Dim npeaks As Integer = Integer.Parse(CStr(data.info!npeaks))
            Dim hashcode As String = data.info!hashcode
            Dim mz As Double() = decode(CStr(data.info!mz))
            Dim into As Double() = decode(CStr(data.info!into))

            If npeaks <> mz.Length Then
                Return Nothing
            ElseIf npeaks <> into.Length Then
                Return Nothing
            End If

            Dim spectral As ms2() = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {.mz = mzi, .intensity = into(i)}
                        End Function) _
                .ToArray

            Return New PeakMs2 With {
                .lib_guid = hashcode,
                .mzInto = spectral
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function decode(base64 As String) As Double()
            Return base64.Base64RawBytes _
                .AddGzipMagic _
                .UnGzipStream _
                .ToArray _
                .Split(8) _
                .Select(Function(b) NetworkByteOrderBitConvertor.ToDouble(b, Scan0)) _
                .ToArray
        End Function

        Public Shared Function DecodeConsensus(base64 As String) As (mz As Double(), into As Double())
            Dim v = base64.Base64RawBytes _
                .UnZipStream _
                .ToArray _
                .Split(8) _
                .Select(Function(b) NetworkByteOrderBitConvertor.ToDouble(b, Scan0)) _
                .ToArray
            Dim p = v.Split(v.Length / 2)

            Return (p(0), p(1))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function encode(x As IEnumerable(Of Double)) As String
            Return x _
                .Select(AddressOf NetworkByteOrderBitConvertor.GetBytes) _
                .IteratesALL _
                .GZipAsBase64(noMagic:=True)
        End Function

        Public Overrides Function WriteSpectrum(spectral As PeakMs2) As Metadata
            Dim metadata As Metadata = TreeFs.GetMetadata(spectral)
            Dim mz As String = encode(spectral.mzInto.Select(Function(m) m.mz))
            Dim into As String = encode(spectral.mzInto.Select(Function(m) m.intensity))
            Dim payload As New NameValueCollection
            Dim url As String = $"{base}/put/spectral/?model_id={model_id}"
            payload.Add("mz", mz)
            payload.Add("into", into)
            payload.Add("npeaks", spectral.mzInto.Length)
            payload.Add("hashcode", spectral.lib_guid)
            payload.Add("model_id", model_id)
            payload.Add("entropy", SpectralEntropy.Entropy(spectral))
            Dim spectral_id As Restful = Restful.ParseJSON(url.POST(payload))
            metadata.block = New BufferRegion With {.position = Val(spectral_id.info)}
            Return metadata
        End Function
    End Class
End Namespace
