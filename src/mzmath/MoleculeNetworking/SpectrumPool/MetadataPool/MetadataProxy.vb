Imports System.Runtime.CompilerServices

Namespace PoolData

    Public MustInherit Class MetadataProxy

        Default Public MustOverride ReadOnly Property GetById(id As String) As Metadata
        Public MustOverride ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)

        Public MustOverride Sub Add(id As String, metadata As Metadata)
        Public MustOverride Function HasGuid(id As String) As Boolean

    End Class

    Public Class InMemoryKeyValueMetadataPool : Inherits MetadataProxy

        ReadOnly data As Dictionary(Of String, Metadata)

        Default Public Overrides ReadOnly Property GetById(id As String) As Metadata
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return data(id)
            End Get
        End Property

        Public Overrides ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)
            Get
                Return data.Values
            End Get
        End Property

        Sub New(data As Dictionary(Of String, Metadata))
            Me.data = data
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Sub Add(id As String, metadata As Metadata)
            Call data.Add(id, metadata)
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return data.ContainsKey(id)
        End Function
    End Class
End Namespace