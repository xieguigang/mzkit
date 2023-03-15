Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Class MSScoreGenerator : Inherits ComparisonProvider

    ReadOnly getSpectrum As Func(Of String, PeakMs2)
    ReadOnly cache As New Dictionary(Of String, PeakMs2)

    Protected ReadOnly align As AlignmentProvider

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="align"></param>
    ''' <param name="getSpectrum">
    ''' get spectrum data by unique reference id
    ''' </param>
    ''' <param name="equals"></param>
    ''' <param name="gt"></param>
    Sub New(align As AlignmentProvider, getSpectrum As Func(Of String, PeakMs2), equals As Double, gt As Double)
        Call MyBase.New(equals, gt)

        Me.align = align
        Me.getSpectrum = getSpectrum
    End Sub

    Private Function GetSpectral(guid As String) As PeakMs2
        If Not cache.ContainsKey(guid) Then
            cache.Add(guid, getSpectrum(guid))
        End If

        Return cache(guid)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return align.GetScore(GetSpectral(x).mzInto, GetSpectral(y).mzInto)
    End Function
End Class
