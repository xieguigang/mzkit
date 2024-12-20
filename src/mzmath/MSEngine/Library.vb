Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' A reference spectrum and annotation data provider
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class Library(Of T As {INamedValue, GenericCompound})

    Dim search As AVLClusterTree(Of PeakMs2)
    Dim metadata As IMetaDb

    ReadOnly dotcutoff As Double
    ReadOnly right As Double
    ReadOnly cos As CosAlignment

    Private Class HashIndex : Implements IMetaDb

        ReadOnly metadata As New Dictionary(Of String, T)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(metadata As T)
            Call Me.metadata.Add(metadata.Identity, metadata)
        End Sub

        Public Function GetAnnotation(uniqueId As String) As (name As String, formula As String) Implements IMetaDb.GetAnnotation
            Throw New NotImplementedException()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMetadata(uniqueId As String) As Object Implements IMetaDb.GetMetadata
            Return metadata(uniqueId)
        End Function

        Public Function GetDbXref(uniqueId As String) As Dictionary(Of String, String) Implements IMetaDb.GetDbXref
            Throw New NotImplementedException()
        End Function
    End Class

    Sub New(data As IEnumerable(Of (meta As T, spec As PeakMs2)),
            Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Call Me.New(dotcutoff, right)

        Dim metadata = New HashIndex

        For Each ref As (meta As T, spec As PeakMs2) In data
            Call search.Add(ref.spec)
            Call metadata.Add(ref.meta)
        Next

        Me.metadata = metadata
    End Sub

    Sub New(Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Me.dotcutoff = dotcutoff
        Me.right = right
        Me.cos = New CosAlignment(DAmethod.DeltaMass(0.3), New RelativeIntensityCutoff(0.05))
        Me.search = New AVLClusterTree(Of PeakMs2)(AddressOf Compares, Function(a) a.lib_guid)
    End Sub

    Sub New(metadb As IMetaDb, refSpec As IEnumerable(Of PeakMs2),
            Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Me.New(dotcutoff, right)
        Me.metadata = metadb

        For Each ref As PeakMs2 In refSpec
            Call search.Add(ref)
        Next
    End Sub

    Private Function Compares(a As PeakMs2, b As PeakMs2) As Integer
        Dim cosine As Double = cos.GetScore(a.mzInto, b.mzInto)

        If cosine > dotcutoff Then
            Return 0
        ElseIf cosine > right Then
            Return 1
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' get metabolite annotation data by id reference
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMetadataByID(id As String) As T
        Return metadata.GetMetadata(id)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function SearchCandidates(sample As PeakMs2) As IEnumerable(Of AlignmentOutput)
        Dim cluster = search.Search(sample).ToArray

        Return From ref As PeakMs2
               In cluster.AsParallel
               Let alignment As AlignmentOutput = cos.CreateAlignment(sample, ref)
               Select alignment
               Order By alignment.cosine Descending
    End Function

End Class
