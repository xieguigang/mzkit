#Region "Microsoft.VisualBasic::5b68614dd78aa5bb23387afc3d2c6e08, mzkit\src\mzmath\SingleCells\Deconvolute\Math.vb"

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

'   Total Lines: 104
'    Code Lines: 60
' Comment Lines: 31
'   Blank Lines: 13
'     File Size: 4.00 KB


'     Module Math
' 
'         Function: DeconvoluteScan, (+3 Overloads) GetMzIndex
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

Namespace Deconvolute

    Public Module Math

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution by pick top N ions in each pixel
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="topN"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetMzIndex(raw As mzPack, mzdiff As Double, topN As Integer) As Double()
            Dim scanMz As New List(Of Double)
            Dim top As IEnumerable(Of ms2)

            For Each x As ScanMS1 In raw.MS
                top = x.GetMs _
                    .OrderByDescending(Function(i) i.intensity) _
                    .Take(topN)
                scanMz.AddRange(top.Select(Function(mzi) mzi.mz))
            Next

            ' just pick the top intensity ion, 
            ' no frequency filter
            Return GetMzIndex(scanMz, mzdiff, freq:=0.0)
        End Function

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq"></param>
        ''' <returns>
        ''' m/z data vector has been re-order ascding
        ''' </returns>
        Public Function GetMzIndex(raw As mzPack, mzdiff As Double, freq As Double) As Double()
            Dim scanMz As New List(Of Double)

            For Each x As Double() In raw.MS.Select(Function(ms) ms.mz)
                Call scanMz.AddRange(x)
            Next

            Return GetMzIndex(scanMz, mzdiff, freq)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMzIndex(raw As IEnumerable(Of ms2), mzdiff As Double, freq As Double) As Double()
            Return GetMzIndex(raw.Select(Function(r) r.mz), mzdiff, freq)
        End Function

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution
        ''' </summary>
        ''' <param name="scanMz"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq">[0,1] percentage</param>
        ''' <returns></returns>
        Public Function GetMzIndex(scanMz As IEnumerable(Of Double), mzdiff As Double, freq As Double) As Double()
            Dim mzBins As NamedCollection(Of Double)() = scanMz _
                .GroupBy(offset:=mzdiff) _
                .Where(Function(v) v.Length > 0) _
                .OrderByDescending(Function(a) a.Length) _
                .ToArray
            Dim counts As Vector = mzBins.Select(Function(a) a.Length).AsVector
            ' normalize to [0,1]
            Dim norm As Vector = counts / counts.Max
            Dim n As Integer = (norm > freq).Sum
            Dim mzUnique As Double() = mzBins _
                .Take(n) _
                .Select(Function(v) v.Average) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return mzUnique
        End Function

        <Extension>
        Public Function DeconvoluteMS(sp As LibraryMatrix, len As Integer, mzIndex As BlockSearchFunction(Of (mz As Double, Integer))) As Double()
            Return DeconvoluteScan(sp.Select(Function(a) a.mz).ToArray, sp.Select(Function(a) a.intensity).ToArray, len, mzIndex)
        End Function

        ''' <summary>
        ''' make alignment of the scan data to a given set of the mz index data
        ''' </summary>
        ''' <param name="mz"></param>
        ''' <param name="into"></param>
        ''' <param name="len"></param>
        ''' <param name="mzIndex"></param>
        ''' <returns>
        ''' a vector of the intensity data which is aligned with the mz vector
        ''' </returns>
        Public Function DeconvoluteScan(mz As Double(),
                                        into As Double(),
                                        len As Integer,
                                        mzIndex As BlockSearchFunction(Of (mz As Double, Integer))) As Double()

            Dim v As Double() = New Double(len - 1) {}
            Dim mzi As Double
            Dim hit As (mz As Double, idx As Integer)
            Dim scan_size As Integer = mz.Length

            For i As Integer = 0 To scan_size - 1
                mzi = mz(i)
                hit = mzIndex _
                    .Search((mzi, -1)) _
                    .OrderBy(Function(a) std.Abs(a.mz - mzi)) _
                    .FirstOrDefault

                If hit.mz < 1 AndAlso hit.idx = 0 Then
                    ' 20221102
                    '
                    ' missing data
                    ' could be caused by the selective ion data export
                    ' just ignores of this problem
                Else
                    v(hit.idx) += into(i)
                End If
            Next

            Return v
        End Function
    End Module
End Namespace
