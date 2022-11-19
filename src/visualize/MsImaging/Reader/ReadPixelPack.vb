Namespace Reader

    ''' <summary>
    ''' handling of the cdf matrix file
    ''' </summary>
    Public Class ReadPixelPack : Inherits PixelReader

        Public Overrides ReadOnly Property dimension As Size
        Public ReadOnly Property pixels As PixelData()

        Dim matrix As Grid(Of InMemoryVectorPixel)

        Sub New(pixels As IEnumerable(Of PixelData))
            Me.matrix = pixels _
                .GroupBy(Function(i) $"{i.x},{i.y}") _
                .Select(Function(i)
                            Dim mz As Double() = i.Select(Function(x) x.mz).ToArray
                            Dim into As Double() = i.Select(Function(x) x.intensity).ToArray

                            Return New InMemoryVectorPixel(i.First.x, i.First.y, mz, into, i.Key)
                        End Function) _
                .DoCall(AddressOf Grid(Of InMemoryVectorPixel).Create)
            Me.pixels = pixels
            Me.dimension = New Size(matrix.width, matrix.height)
        End Sub

        Protected Overrides Sub release()
            Erase _pixels
        End Sub

        Public Overrides Function GetPixel(x As Integer, y As Integer) As PixelScan
            Return matrix(x, y)
        End Function

        Public Overrides Function AllPixels() As IEnumerable(Of PixelScan)
            Return matrix.EnumerateData.Select(Function(i) DirectCast(i, PixelScan))
        End Function

        Public Overrides Function LoadMzArray(ppm As Double) As Double()
            Return pixels _
                .GroupBy(Function(d) d.mz, Tolerance.PPM(ppm)) _
                .Select(Function(d)
                            Return d _
                                .OrderByDescending(Function(i) i.intensity) _
                                .First _
                                .mz
                        End Function) _
                .ToArray
        End Function
    End Class

End Namespace