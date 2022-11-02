Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
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

                Call text.Flush()
            End Using

            Return True
        End Function

        Public Shared Function CreateMatrix(raw As mzPack, Optional mzdiff As String = "da:0.01") As MzMatrix
            Dim mzSet As (mz As Double(), Index As BlockSearchFunction(Of (mz As Double, Integer))) = getMzIndex(raw:=raw)
            Dim matrix = getMatrix(raw, mzSet.mz.Length, mzSet.Index).ToArray

            ' removes zero vector


            Return New MzMatrix With {
                .matrix = matrix,
                .mz = mzSet.mz,
                .tolerance = mzdiff
            }
        End Function

        Private Shared Iterator Function getMatrix(raw As mzPack, len As Integer, mzIndex As BlockSearchFunction(Of (mz As Double, Integer))) As IEnumerable(Of PixelData)
            For Each scan As ScanMS1 In raw.MS
                Dim xy As Point = scan.GetMSIPixel
                Dim v As Double() = New Double(len - 1) {}
                Dim mz As Double() = scan.mz
                Dim mzi As Double
                Dim hit As (mz As Double, idx As Integer)

                For i As Integer = 0 To scan.size - 1
                    mzi = mz(i)
                    hit = mzIndex _
                        .Search((mzi, -1)) _
                        .OrderBy(Function(a) stdNum.Abs(a.mz - mzi)) _
                        .First

                    v(hit.idx) += scan.into(i)
                Next

                Yield New PixelData With {
                    .X = xy.X,
                    .Y = xy.Y,
                    .intensity = v
                }
            Next
        End Function

        Private Shared Function getMzIndex(raw As mzPack) As (Double(), BlockSearchFunction(Of (mz As Double, Integer)))
            Dim scanMz As New List(Of Double)

            For Each x As Double() In raw.MS.Select(Function(ms) ms.mz)
                Call scanMz.AddRange(x)
            Next

            Dim mzUnique As Double() = scanMz _
                .GroupBy(offset:=0.001) _
                .Select(Function(v) Val(v.name)) _
                .ToArray
            Dim mzIndex As New BlockSearchFunction(Of (mz As Double, Integer))(
                data:=mzUnique.Select(Function(mzi, i) (mzi, i)),
                eval:=Function(i) i.mz,
                tolerance:=1,
                fuzzy:=True
            )

            Return (mzUnique, mzIndex)
        End Function
    End Class
End Namespace