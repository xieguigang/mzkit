
Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

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

    Public Class HttpRESTMetadataPool : Inherits MetadataProxy

        Dim local_cache As New Dictionary(Of String, Metadata)
        Dim url_get As String
        Dim url_put As String

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

        Public Overrides Sub Add(id As String, metadata As Metadata)
            Dim payload As New NameValueCollection

            Call payload.Add("metadata", metadata.GetJson)
            Call url_put.POST(payload)
            Call local_cache.Add(id, metadata)
        End Sub

        Public Overrides Function HasGuid(id As String) As Boolean
            Return Me(id) IsNot Nothing
        End Function
    End Class
End Namespace