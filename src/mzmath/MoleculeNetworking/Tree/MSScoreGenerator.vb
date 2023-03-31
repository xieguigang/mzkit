Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.DataMining.BinaryTree

''' <summary>
''' A score evaluator that contains the cache
''' </summary>
Public Class MSScoreGenerator : Inherits ComparisonProvider

    ReadOnly getSpectrum As Func(Of String, PeakMs2)
    ReadOnly cache As New Dictionary(Of String, PeakMs2)

    Protected ReadOnly align As AlignmentProvider

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="align"></param>
    ''' <param name="getSpectrum">
    ''' get spectrum data by unique reference id[cache system was implemented in this module]
    ''' </param>
    ''' <param name="equals"></param>
    ''' <param name="gt"></param>
    Sub New(align As AlignmentProvider,
            getSpectrum As Func(Of String, PeakMs2),
            Optional equals As Double = 1,
            Optional gt As Double = 0)

        Call MyBase.New(equals, gt)

        Me.align = align
        Me.getSpectrum = getSpectrum
    End Sub

    Public Sub Add(spectral As PeakMs2)
        If Not cache.ContainsKey(spectral.lib_guid) Then
            Call cache.Add(spectral.lib_guid, spectral)
        End If
    End Sub

    Private Function GetSpectral(guid As String) As PeakMs2
        If Not cache.ContainsKey(guid) Then
            cache.Add(guid, getSpectrum(guid))
        End If

        Return cache(guid)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAlignment(x As String, y As String) As AlignmentOutput
        Return align.CreateAlignment(GetSpectral(x).mzInto, GetSpectral(y).mzInto)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return align.GetScore(GetSpectral(x).mzInto, GetSpectral(y).mzInto)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"[{Me.GetHashCode.ToHexString}] {align.ToString}, has {cache.Count} spectrum data cached."
    End Function
End Class
