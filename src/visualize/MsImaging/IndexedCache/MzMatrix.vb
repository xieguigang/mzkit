Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace IndexedCache

    ''' <summary>
    ''' a data matrix object in format of row is pixel and 
    ''' column is mz intensity value across different 
    ''' pixels
    ''' </summary>
    Public Class MzMatrix

        ''' <summary>
        ''' m/z vector in numeric format of round to digit 4 
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As Double()

        ''' <summary>
        ''' the script string of the mz diff tolerance for <see cref="mz"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As String

        Public Property matrix As PixelData()

        Public Class PixelData

            Public Property X As Integer
            Public Property Y As Integer
            Public Property intensity As Double()

            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Public Overrides Function ToString() As String
                Return $"[{X},{Y}]"
            End Function

        End Class

        Public Function ExportCsvSheet(file As Stream) As Boolean
            Using text As New StreamWriter(file, Encodings.ASCII.CodePage) With {
                .NewLine = vbLf
            }
                Call text.WriteLine("Pixels," & mz.JoinBy(","))

                For Each pixelLine As String In matrix _
                    .AsParallel _
                    .Select(Function(pixel)
                                Return $"""{pixel.X},{pixel.Y}"",{pixel.intensity.JoinBy(",")}"
                            End Function)

                    Call text.WriteLine(pixelLine)
                Next

                Call text.Flush()
            End Using

            Return True
        End Function

        ''' <summary>
        ''' ms-imaging raw data matrix deconvolution
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq"></param>
        ''' <returns></returns>
        Public Shared Function CreateMatrix(raw As mzPack,
                                            Optional mzdiff As Double = 0.001,
                                            Optional freq As Double = 0.001) As MzMatrix

            Dim mzSet As Double() = GetMzIndex(raw:=raw, mzdiff:=mzdiff, freq:=freq)
            Dim mzIndex As New BlockSearchFunction(Of (mz As Double, Integer))(
                data:=mzSet.Select(Function(mzi, i) (mzi, i)),
                eval:=Function(i) i.mz,
                tolerance:=1,
                fuzzy:=True
            )
            Dim matrix = deconvoluteMatrix(raw, mzSet.Length, mzIndex).ToArray

            Return New MzMatrix With {
                .matrix = matrix,
                .mz = mzSet,
                .tolerance = mzdiff
            }
        End Function

        Private Shared Iterator Function deconvoluteMatrix(raw As mzPack,
                                                           len As Integer,
                                                           mzIndex As BlockSearchFunction(Of (mz As Double, Integer))) As IEnumerable(Of PixelData)
            For Each scan As ScanMS1 In raw.MS
                Dim xy As Point = scan.GetMSIPixel
                Dim v As Double() = DeconvoluteScan(scan.mz, scan.into, len, mzIndex)

                Yield New PixelData With {
                    .X = xy.X,
                    .Y = xy.Y,
                    .intensity = v
                }
            Next
        End Function

        Public Shared Function DeconvoluteScan(mz As Double(),
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
                    ' missing data
                Else
                    v(hit.idx) += into(i)
                End If
            Next

            Return v
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetMzIndex(raw As IEnumerable(Of ms2), mzdiff As Double, freq As Double) As Double()
            Return GetMzIndex(raw.Select(Function(r) r.mz), mzdiff, freq)
        End Function

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution
        ''' </summary>
        ''' <param name="scanMz"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq"></param>
        ''' <returns></returns>
        Public Shared Function GetMzIndex(scanMz As IEnumerable(Of Double), mzdiff As Double, freq As Double) As Double()
            Dim mzBins As NamedCollection(Of Double)() = scanMz _
                .GroupBy(offset:=mzdiff) _
                .Where(Function(v) v.Length > 0) _
                .OrderByDescending(Function(a) a.Length) _
                .ToArray
            Dim counts As Vector = mzBins.Select(Function(a) a.Length).AsVector
            Dim norm = (counts / counts.Max) * 100
            Dim n As Integer = (norm > freq).Sum
            Dim mzUnique As Double() = mzBins _
                .Take(n) _
                .Select(Function(v) v.Average) _
                .ToArray

            Return mzUnique
        End Function

        ''' <summary>
        ''' get a m/z vector for run matrix deconvolution
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzdiff"></param>
        ''' <param name="freq"></param>
        ''' <returns></returns>
        Public Shared Function GetMzIndex(raw As mzPack, mzdiff As Double, freq As Double) As Double()
            Dim scanMz As New List(Of Double)

            For Each x As Double() In raw.MS.Select(Function(ms) ms.mz)
                Call scanMz.AddRange(x)
            Next

            Return GetMzIndex(scanMz, mzdiff, freq)
        End Function
    End Class
End Namespace