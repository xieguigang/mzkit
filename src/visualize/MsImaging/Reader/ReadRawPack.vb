Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Reader

    Public Class ReadRawPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size

        Dim pixels As mzPackPixel()

        Sub New(mzpack As Assembly.mzPack)
            Me.pixels = mzpack.MS _
                .Select(Function(pixel)
                            Return New mzPackPixel(pixel)
                        End Function) _
                .ToArray

            Call ReadDimensions()
        End Sub

        Sub New(mzpack As String)
            Using file As Stream = mzpack.Open
                Me.pixels = Assembly.mzPack _
                    .ReadAll(file).MS _
                    .Select(Function(pixel)
                                Return New mzPackPixel(pixel)
                            End Function) _
                    .ToArray
            End Using

            Call ReadDimensions()
        End Sub

        Private Sub ReadDimensions()
            Dim width As Integer = pixels.Select(Function(p) p.X).Max
            Dim height As Integer = pixels.Select(Function(p) p.Y).Max

            _dimension = New Size(width, height)
        End Sub

        Sub New(pixels As IEnumerable(Of mzPackPixel), MsiDim As Size)
            Me.dimension = MsiDim
            Me.pixels = pixels.ToArray
        End Sub

        Protected Overrides Sub release()
            For Each p In pixels
                Call p.release()
            Next

            Erase pixels
        End Sub

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Dim mzlist As Double() = pixels _
                .Select(Function(p) p.mz) _
                .IteratesALL _
                .ToArray
            Dim groups = mzlist _
                .GroupBy(Function(mz) mz, Tolerance.PPM(ppm)) _
                .Select(Function(mz) Val(mz.name)) _
                .OrderBy(Function(mzi) mzi) _
                .ToArray

            Return groups
        End Function

        Protected Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return pixels.Select(Function(p) DirectCast(p, PixelScan))
        End Function
    End Class
End Namespace