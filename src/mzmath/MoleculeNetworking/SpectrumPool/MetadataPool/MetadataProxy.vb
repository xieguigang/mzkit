Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml

Namespace PoolData

    ''' <summary>
    ''' a spectrum cluster node collection proxy
    ''' </summary>
    Public MustInherit Class MetadataProxy

        Default Public MustOverride ReadOnly Property GetById(id As String) As Metadata
        Public MustOverride ReadOnly Property AllClusterMembers As IEnumerable(Of Metadata)

        Public MustOverride Sub Add(id As String, metadata As Metadata)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="score"></param>
        ''' <param name="align">
        ''' the root spectrum to current cluster node is nothing, due 
        ''' to the reason of no spectrum compares to it
        ''' </param>
        ''' <param name="pval"></param>
        Public MustOverride Sub Add(id As String, score As Double, align As AlignmentOutput, pval As Double)

        Public MustOverride Function HasGuid(id As String) As Boolean
        Public MustOverride Sub SetRootId(hashcode As String)

    End Class

    Public Class InMemoryKeyValueMetadataPool : Inherits MetadataProxy

        Dim data As Dictionary(Of String, Metadata)
        Dim rootId As String

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

        Public Overrides Sub SetRootId(hashcode As String)
            rootId = hashcode
        End Sub

        Public Overrides Sub Add(id As String, score As Double, align As AlignmentOutput, pval As Double)

        End Sub
    End Class
End Namespace