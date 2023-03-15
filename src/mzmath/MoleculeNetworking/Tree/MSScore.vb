#Region "Microsoft.VisualBasic::7626a5f6112a364c6f87082775ca58a4, mzkit\src\mzmath\MoleculeNetworking\MSScore.vb"

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

'   Total Lines: 25
'    Code Lines: 19
' Comment Lines: 0
'   Blank Lines: 6
'     File Size: 826 B


' Class MSScore
' 
'     Properties: Ions
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetSimilarity
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

''' <summary>
''' Do spectrum tuple similarity evaluation via the cosine score
''' </summary>
''' <remarks>
''' just works for a given certain collection of the spectrum data
''' </remarks>
Public Class MSScore : Inherits MSScoreGenerator

    ReadOnly ms2 As Dictionary(Of String, PeakMs2)

    Public ReadOnly Property Ions As IEnumerable(Of PeakMs2)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ms2.Values
        End Get
    End Property

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
    Public Sub New(align As AlignmentProvider, ions As IEnumerable(Of PeakMs2), equals As Double, gt As Double)
        MyBase.New(align, AddressOf ions.ToDictionary(Function(i) i.lib_guid).GetValueOrNull, equals, gt)
    End Sub

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
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return align.GetScore(ms2(x).mzInto, ms2(y).mzInto)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return align.ToString
    End Function
End Class
