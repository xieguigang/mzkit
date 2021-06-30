Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader

Public Class PixelsSampler

    ''' <summary>
    ''' pixels[x][y]
    ''' </summary>
    ReadOnly col_scans As PixelScan()()
    ReadOnly dims As Size

    Sub New(reader As PixelReader)
        dims = reader.dimension
        col_scans = CreateColumns(reader, dims).ToArray
    End Sub

    Private Shared Iterator Function CreateColumns(reader As PixelReader, dims As Size) As IEnumerable(Of PixelScan())
        For x As Integer = 1 To dims.Width
            Dim column As PixelScan() = New PixelScan(dims.Height - 1) {}

            For y As Integer = 1 To dims.Height
                column(y - 1) = reader.GetPixel(x, y)
            Next

            Yield column
        Next
    End Function

    Public Iterator Function GetBlock(o As Point, width As Integer, height As Integer) As IEnumerable(Of PixelScan)
        For x As Integer = o.X To o.X + width
            For y As Integer = o.Y To o.Y + height
                Yield col_scans(x)(y)
            Next
        Next
    End Function

    ''' <summary>
    ''' scale to new size for reduce data size for downstream analysis by
    ''' pixels sampling
    ''' </summary>
    ''' <param name="samplingSize"></param>
    ''' <returns></returns>
    Public Iterator Function Sampling(samplingSize As Size, tolerance As Tolerance) As IEnumerable(Of mzPackPixel)
        For x As Integer = 1 To 
    End Function
End Class
