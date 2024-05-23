#Region "Microsoft.VisualBasic::42881ea20d0f404e90bd3417e28d0108, mzmath\MoleculeNetworking\Tree\MSScoreGenerator.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 128
    '    Code Lines: 64 (50.00%)
    ' Comment Lines: 46 (35.94%)
    '    - Xml Docs: 93.48%
    ' 
    '   Blank Lines: 18 (14.06%)
    '     File Size: 4.43 KB


    ' Class MSScoreGenerator
    ' 
    '     Properties: Ions
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: GetAlignment, GetObject, GetSimilarity, GetSpectral, ToString
    ' 
    '     Sub: Add, Clear
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.DataMining.BinaryTree

''' <summary>
''' A score evaluator that contains the cache
''' </summary>
''' <remarks>
''' this module has a <see cref="cache"/> for get spectrum data by a unique reference id
''' </remarks>
Public Class MSScoreGenerator : Inherits ComparisonProvider

    ReadOnly getSpectrum As Func(Of String, PeakMs2)
    ReadOnly cache As New Dictionary(Of String, PeakMs2)

    Protected ReadOnly align As AlignmentProvider

    Public ReadOnly Property Ions As IEnumerable(Of PeakMs2)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return cache.Values
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="align"></param>
    ''' <param name="getSpectrum">
    ''' get spectrum data by unique reference id[cache system was implemented in this module]
    ''' </param>
    ''' <param name="equals"></param>
    ''' <param name="gt"></param>
    Sub New(align As AlignmentProvider, getSpectrum As Func(Of String, PeakMs2),
            Optional equals As Double = 1,
            Optional gt As Double = 0)
        Call MyBase.New(equals, gt)

        Me.align = align
        Me.getSpectrum = getSpectrum
    End Sub

    Sub New(align As AlignmentProvider, Optional equals As Double = 1, Optional gt As Double = 0)
        Call MyBase.New(equals, gt)

        Me.align = align
        Me.getSpectrum = Function(guid) Nothing
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="align"></param>
    ''' <param name="ions">
    ''' these source spectrum data collection for run the similarity evaluation
    ''' </param>
    ''' <param name="equals"></param>
    ''' <param name="gt"></param>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(align As AlignmentProvider, ions As PeakMs2(), equals As Double, gt As Double)
        Call Me.New(align, AddressOf ions.ToDictionary(Function(i) i.lib_guid).GetValueOrNull, equals, gt)

        For Each spec As PeakMs2 In ions
            Call Add(spec)
        Next
    End Sub

    ''' <summary>
    ''' clear the cache of the spectrum data pool
    ''' </summary>
    Public Sub Clear()
        Call cache.Clear()
    End Sub

    Public Sub Add(spectral As PeakMs2)
        If Not cache.ContainsKey(spectral.lib_guid) Then
            Call cache.Add(spectral.lib_guid, spectral)
        End If
    End Sub

    ''' <summary>
    ''' get spectrum from dictionary via a key
    ''' </summary>
    ''' <param name="guid"></param>
    ''' <returns></returns>
    Public Function GetSpectral(guid As String) As PeakMs2
        If Not cache.ContainsKey(guid) Then
            cache.Add(guid, getSpectrum(guid))
        End If

        Return cache(guid)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAlignment(x As String, y As String) As AlignmentOutput
        Return align.CreateAlignment(GetSpectral(x).mzInto, GetSpectral(y).mzInto)
    End Function

    ''' <summary>
    ''' get the spectrum similarity score via theirs unique reference id
    ''' </summary>
    ''' <param name="x">
    ''' the <see cref="PeakMs2.lib_guid"/> unique reference id of spectrum object x
    ''' </param>
    ''' <param name="y">
    ''' the <see cref="PeakMs2.lib_guid"/> unique reference id of spectrum object y
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' spectrum could be <see cref="Add"/>
    ''' </remarks>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return align.GetScore(GetSpectral(x).mzInto, GetSpectral(y).mzInto)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"[{Me.GetHashCode.ToHexString}] {align.ToString}, has {cache.Count} spectrum data cached."
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetObject(id As String) As Object
        Return GetSpectral(guid:=id)
    End Function
End Class
