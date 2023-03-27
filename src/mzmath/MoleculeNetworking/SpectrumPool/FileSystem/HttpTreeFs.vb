Namespace PoolData

    Public Class HttpTreeFs : Inherits PoolFs

        Public Overrides Sub CommitMetadata(path As String, data As MetadataProxy)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Sub SetRootId(path As String, id As String)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Sub Add(spectrum As Spectra.PeakMs2)
            Throw New NotImplementedException()
        End Sub

        Protected Overrides Sub Close()
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function GetTreeChilds(path As String) As IEnumerable(Of String)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function LoadMetadata(path As String) As MetadataProxy
            Throw New NotImplementedException()
        End Function

        Public Overrides Function FindRootId(path As String) As String
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetScore(x As String, y As String) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ReadSpectrum(p As Metadata) As Spectra.PeakMs2
            Throw New NotImplementedException()
        End Function

        Public Overrides Function WriteSpectrum(spectral As Spectra.PeakMs2) As Metadata
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace