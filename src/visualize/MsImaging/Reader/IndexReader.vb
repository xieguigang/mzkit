
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel

Namespace Reader

    Public Class IndexReader : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
            Get
                Return New Size(reader.meta.width, reader.meta.height)
            End Get
        End Property

        Dim reader As XICReader

        Protected Overrides Sub release()
            Call reader.Dispose()
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Return reader.GetPixel(x, y)
        End Function

        ''' <summary>
        ''' load all mz
        ''' </summary>
        ''' <param name="ppm"></param>
        ''' <returns></returns>
        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Return reader.GetMz
        End Function

        Protected Overrides Iterator Function AllPixels() As IEnumerable(Of PixelScan)
            For x As Integer = 1 To reader.meta.width
                For y As Integer = 1 To reader.meta.height
                    Yield reader.GetPixel(x, y)
                Next
            Next
        End Function
    End Class
End Namespace