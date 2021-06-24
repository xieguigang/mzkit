Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Pixel

    Public Class ibdPixel : Inherits PixelScan

        Public Overrides ReadOnly Property X As Integer
        Public Overrides ReadOnly Property Y As Integer

        ReadOnly i As ScanData
        ReadOnly raw As ibdReader

        Dim memoryCache As ms2()
        Dim enableCache As Boolean = False

        Sub New(ibd As ibdReader, pixel As ScanData, Optional enableCache As Boolean = False)
            Me.i = pixel
            Me.raw = ibd
            Me.enableCache = enableCache
            Me.X = i.x
            Me.Y = i.y
        End Sub

        Sub New(x As Integer, y As Integer, cache As IEnumerable(Of ms2))
            Me.memoryCache = cache
            Me.enableCache = True
            Me.X = x
            Me.Y = y
        End Sub

        Public Overrides Function GetMs() As ms2()
            If Not enableCache Then
                Return raw.GetMSMS(i)
            Else
                ' 有些像素点是空向量
                ' 所以就只判断nothing而不判断empty了
                If memoryCache Is Nothing Then
                    memoryCache = raw.GetMSMS(i)
                End If

                Return memoryCache
            End If
        End Function

        Public Function ReadMz() As Double()
            If Not enableCache Then
                Return raw.ReadArray(i.MzPtr)
                ' 有些像素点是空向量
                ' 所以就只判断nothing而不判断empty了
            ElseIf memoryCache.IsNullOrEmpty Then
                Return raw.ReadArray(i.MzPtr)
            Else
                Return (From m As ms2 In memoryCache Select m.mz).ToArray
            End If
        End Function

        Public Overrides Function HasAnyMzIon(mz() As Double, tolerance As Tolerance) As Boolean
            Dim mzlist As Double() = ReadMz()

            Return mz _
                .Any(Function(mzi)
                         Return mzlist.Any(Function(zzz) tolerance(zzz, mzi))
                     End Function)
        End Function

        Protected Friend Overrides Sub release()
            Erase memoryCache
        End Sub
    End Class
End Namespace