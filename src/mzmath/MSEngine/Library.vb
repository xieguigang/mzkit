Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Class Library(Of T As {INamedValue, GenericCompound})

    ReadOnly metadata As New Dictionary(Of String, T)
    ReadOnly pool As PeakMs2()
    ReadOnly cos As CosAlignment
    ReadOnly dotcutoff As Double

    Sub New(data As IEnumerable(Of (meta As T, spec As PeakMs2)), Optional dotcutoff As Double = 0.6)
        Dim pool As New List(Of PeakMs2)

        For Each ref As (meta As T, spec As PeakMs2) In data
            Call pool.Add(ref.spec)
            Call metadata.Add(ref.meta.Identity, ref.meta)
        Next

        Me.cos = New CosAlignment(DAmethod.DeltaMass(0.3), New RelativeIntensityCutoff(0.05))
        Me.pool = pool.ToArray
    End Sub

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
        Return From ref As PeakMs2
               In pool.AsParallel
               Let alignment As AlignmentOutput = cos.CreateAlignment(sample, ref)
               Select alignment
               Order By alignment.cosine Descending
    End Function

End Class
