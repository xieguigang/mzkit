Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports stdNum = System.Math

Namespace Deconvolute

    Public Module Math

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
        ''' <param name="freq"></param>
        ''' <returns></returns>
        Public Function GetMzIndex(scanMz As IEnumerable(Of Double), mzdiff As Double, freq As Double) As Double()
            Dim mzBins As NamedCollection(Of Double)() = scanMz _
                .GroupBy(offset:=mzdiff) _
                .Where(Function(v) v.Length > 0) _
                .OrderByDescending(Function(a) a.Length) _
                .ToArray
            Dim counts As Vector = mzBins.Select(Function(a) a.Length).AsVector
            Dim norm As Vector = (counts / counts.Max) * 100
            Dim n As Integer = (norm > freq).Sum
            Dim mzUnique As Double() = mzBins _
                .Take(n) _
                .Select(Function(v) v.Average) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return mzUnique
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
                    .OrderBy(Function(a) stdNum.Abs(a.mz - mzi)) _
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