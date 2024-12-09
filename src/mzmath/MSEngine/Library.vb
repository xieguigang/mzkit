Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class Library(Of T As {INamedValue, GenericCompound})

    ReadOnly metadata As New Dictionary(Of String, T)
    ReadOnly search As AVLClusterTree(Of PeakMs2)
    ReadOnly cos As CosAlignment
    ReadOnly dotcutoff As Double
    ReadOnly right As Double

    Sub New(data As IEnumerable(Of (meta As T, spec As PeakMs2)),
            Optional dotcutoff As Double = 0.6,
            Optional right As Double = 0.5)

        Me.dotcutoff = dotcutoff
        Me.right = right
        Me.search = New AVLClusterTree(Of PeakMs2)(AddressOf Compares, Function(a) a.lib_guid)
        Me.cos = New CosAlignment(DAmethod.DeltaMass(0.3), New RelativeIntensityCutoff(0.05))

        For Each ref As (meta As T, spec As PeakMs2) In data
            Call search.Add(ref.spec)
            Call metadata.Add(ref.meta.Identity, ref.meta)
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
        Return metadata(id)
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
