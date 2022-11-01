Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
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

            Public Overrides Function ToString() As String
                Return $"[{X},{Y}]"
            End Function

        End Class

        Public Function ExportCsvSheet(file As Stream) As Boolean
            Using text As New StreamWriter(file, Encodings.ASCII.CodePage) With {
                .NewLine = vbLf
            }
                Call text.WriteLine("Pixels," & mz.JoinBy(","))

                For Each pixel As PixelData In matrix
                    Call text.WriteLine($"""{pixel.X},{pixel.Y}"",{pixel.intensity.JoinBy(",")}")
                Next
            End Using

            Return True
        End Function

        Public Shared Function CreateMatrix(raw As mzPack, Optional mzdiff As String = "da:0.001") As MzMatrix
            Dim mzSet As (mz As Double(), Index As Index(Of String)) = getMzIndex(raw, Ms1.Tolerance.ParseScript(mzdiff))
            Dim matrix = getMatrix(raw, mzSet.mz.Length, mzSet.Index).ToArray

            Return New MzMatrix With {
                .matrix = matrix,
                .mz = mzSet.mz,
                .tolerance = mzdiff
            }
        End Function

        Private Shared Iterator Function getMatrix(raw As mzPack, len As Integer, mzIndex As Index(Of String)) As IEnumerable(Of PixelData)
            For Each scan As ScanMS1 In raw.MS
                Dim xy As Point = scan.GetMSIPixel
                Dim v As Double() = New Double(len - 1) {}
                Dim j As Integer

                For i As Integer = 0 To scan.size - 1
                    j = mzIndex(x:=scan.mz(i).ToString("F3"))
                    v(j) += scan.into(i)
                Next

                Yield New PixelData With {
                    .X = xy.X,
                    .Y = xy.Y,
                    .intensity = v
                }
            Next
        End Function

        Private Shared Function getMzIndex(raw As mzPack, mzErr As Tolerance) As (Double(), Index(Of String))
            Dim zero As New RelativeIntensityCutoff(0)
            Dim mzSet = raw.MS _
                .AsParallel _
                .Select(Function(ms)
                            Return ms.GetMs.ToArray.Centroid(mzErr, cutoff:=zero)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(m) m.mz.ToString("F3")) _
                .Select(Function(a) Aggregate mzi As ms2 In a Into Average(mzi.mz)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray
            Dim mzUnique As Double() = mzSet.Select(Function(mzi) stdNum.Round(mzi, 3)).Distinct.ToArray
            Dim mzIndex As Index(Of String) = mzUnique _
                .Select(Function(mzi) mzi.ToString) _
                .Indexing

            Return (mzUnique, mzIndex)
        End Function
    End Class
End Namespace