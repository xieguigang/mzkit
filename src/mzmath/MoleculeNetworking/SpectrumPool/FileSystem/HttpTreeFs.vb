Imports System.Collections.Specialized
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Net.Http

Namespace PoolData

    Public Class HttpTreeFs : Inherits PoolFs

        ''' <summary>
        ''' the web services base url
        ''' </summary>
        Friend ReadOnly base As String
        Friend ReadOnly metadata_pool As New Dictionary(Of String, HttpRESTMetadataPool)

        Public Shared ReadOnly Property RootHashIndex As String = "/".MD5.ToLower

        Sub New(url As String)
            base = url
            ' do system init
            Call VBDebugger.EchoLine($"{base}/init/".GET)
        End Sub

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
        ''' get tree path of the childs
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Overrides Iterator Function GetTreeChilds(path As String) As IEnumerable(Of String)
            Dim url As String = $"{base}/get/childs/?q={ClusterHashIndex(path)}"
            Dim json As String = url.GET
            Dim data = Restful.ParseJSON(json)
            Dim childs_data As Array = data.info

            For i As Integer = 0 To childs_data.Length - 1
                Dim obj As JavaScriptObject = childs_data.GetValue(i)
                Dim key As String = obj!key

                Yield $"{path}/{key}"
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

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Dim meta As New HttpRESTMetadataPool(Me, path, getParentId(path))
            Dim key As String = ClusterHashIndex(path)
            metadata_pool.Add(key, meta)
            Return meta
        End Function

        Public Overrides Function FindRootId(path As String) As String
            Dim key As String = ClusterHashIndex(path)

            If Not metadata_pool.ContainsKey(key) Then
                Return Nothing
            End If

            Return metadata_pool(key).RootSpectrumId
        End Function

        ''' <summary>
        ''' the block location of the metadata is the database id of 
        ''' the target spectral data actually 
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Overrides Function ReadSpectrum(p As Metadata) As Spectra.PeakMs2
            Dim url As String = $"{base}/get/spectrum/?id={p.block.position}"
            Dim json As String = url.GET
            Dim data = Restful.ParseJSON(json)

            If data.code <> 0 Then
                Throw New InvalidDataException
            End If

            Dim npeaks As Integer = Integer.Parse(CStr(data.info!npeaks))
            Dim hashcode As String = data.info!hashcode
            Dim mz As Double() = CStr(data.info!mz).Base64RawBytes.Split(8).Select(Function(b) NetworkByteOrderBitConvertor.ToDouble(b)).ToArray
            Dim into As Double() = CStr(data.info!into).Base64RawBytes.Split(8).Select(Function(b) NetworkByteOrderBitConvertor.ToDouble(b)).ToArray

            If npeaks <> mz.Length Then
                Throw New InvalidDataException
            ElseIf npeaks <> into.Length Then
                Throw New InvalidDataException
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

        Public Overrides Function WriteSpectrum(spectral As Spectra.PeakMs2) As Metadata
            Dim metadata As Metadata = TreeFs.GetMetadata(spectral)
            Dim mz As String = spectral.mzInto.Select(Function(m) m.mz).Select(AddressOf NetworkByteOrderBitConvertor.GetBytes).IteratesALL.ToBase64String
            Dim into As String = spectral.mzInto.Select(Function(m) m.intensity).Select(AddressOf NetworkByteOrderBitConvertor.GetBytes).IteratesALL.ToBase64String
            Dim payload As New NameValueCollection
            Dim url As String = $"{base}/put/spectral/"
            payload.Add("mz", mz)
            payload.Add("into", into)
            payload.Add("npeaks", spectral.mzInto.Length)
            payload.Add("hashcode", spectral.lib_guid)
            Dim spectral_id As Restful = Restful.ParseJSON(url.POST(payload))
            metadata.block = New BufferRegion With {.position = Val(spectral_id.info)}
            Return metadata
        End Function
    End Class
End Namespace