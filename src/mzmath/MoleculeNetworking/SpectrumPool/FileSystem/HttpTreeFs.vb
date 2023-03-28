Imports System.Collections.Specialized
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

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

        Public Overrides Sub SetRootId(path As String, id As String)
            Dim args As New NameValueCollection
            args.Add("path", path)
            args.Add("id", id)
            Call $"{base}/set/root".POST(args)
        End Sub

        Protected Overrides Sub Close()
            ' do nothing
        End Sub

        Public Overrides Function GetTreeChilds(path As String) As IEnumerable(Of String)
            Return $"{base}/get/childs/?q={ClusterHashIndex(path)}".LoadJSON(Of String())
        End Function

        Public Shared Function ClusterHashIndex(path As String) As String
            path = path.StringReplace("/{2,}", "/")

            If path.Length > 1 Then
                path = path.TrimEnd("/"c)
            End If

            Return path.MD5.ToLower
        End Function

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Dim meta As New HttpRESTMetadataPool(Me, path)
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
            Return $"{base}/get/spectrum/?hashcode={p.guid}&q={p.block.position}".GET.LoadJSON(Of PeakMs2)
        End Function

        Public Overrides Function WriteSpectrum(spectral As Spectra.PeakMs2) As Metadata
            Dim metadata As Metadata = TreeFs.GetMetadata(spectral)
            Dim mz As String = spectral.mzInto.Select(Function(m) m.mz).Select(AddressOf NetworkByteOrderBitConvertor.GetBytes).IteratesALL.ToBase64String
            Dim into As String = spectral.mzInto.Select(Function(m) m.intensity).Select(AddressOf NetworkByteOrderBitConvertor.GetBytes).IteratesALL.ToBase64String
            Dim payload As New NameValueCollection
            payload.Add("mz", mz)
            payload.Add("into", into)
            payload.Add("npeaks", spectral.mzInto.Length)
            payload.Add("hashcode", spectral.lib_guid)
            Dim spectral_id As String = $"{base}/put/spectral/".POST(payload).html
            metadata.block = New BufferRegion With {.position = spectral_id}
            Return metadata
        End Function
    End Class
End Namespace