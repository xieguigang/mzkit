Imports System.Collections.Specialized
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PoolData

    Public Class HttpTreeFs : Inherits PoolFs

        ''' <summary>
        ''' the web services base url
        ''' </summary>
        Friend ReadOnly base As String

        Sub New(url As String)
            base = url
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
            Return $"{base}/get/childs/?q={path.UrlEncode}".LoadJSON(Of String())
        End Function

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Return New HttpRESTMetadataPool(Me, path)
        End Function

        Public Overrides Function FindRootId(path As String) As String
            Return $"{base}/get/root".GET
        End Function

        Public Overrides Function ReadSpectrum(p As Metadata) As Spectra.PeakMs2
            Throw New NotImplementedException()
        End Function

        Public Overrides Function WriteSpectrum(spectral As Spectra.PeakMs2) As Metadata
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace