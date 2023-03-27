
Namespace PoolData

    Public MustInherit Class PoolFs

        Public MustOverride Function LoadMetadata(path As String) As MetadataProxy
        Public MustOverride Function FindRootId(path As String) As String

    End Class
End Namespace