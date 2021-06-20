Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language

Public Module PixelsCDF

    <Extension>
    Public Sub CreateCDF(loadedPixels As PixelData(), file As Stream, dimension As Size, tolerance As Tolerance)
        Using matrix As New CDFWriter(file)
            Dim mz As New List(Of Double)
            Dim intensity As New List(Of Double)
            Dim x As New List(Of Integer)
            Dim y As New List(Of Integer)

            For Each p As PixelData In loadedPixels
                mz.Add(p.mz)
                intensity.Add(p.intensity)
                x.Add(p.x)
                y.Add(p.y)
            Next

            matrix.GlobalAttributes(New attribute With {.name = "width", .value = dimension.Width, .type = CDFDataTypes.INT})
            matrix.GlobalAttributes(New attribute With {.name = "height", .value = dimension.Height, .type = CDFDataTypes.INT})
            matrix.GlobalAttributes(New attribute With {.name = "program", .value = "mzkit_win32", .type = CDFDataTypes.CHAR})
            matrix.GlobalAttributes(New attribute With {.name = "github", .value = "https://github.com/xieguigang/mzkit", .type = CDFDataTypes.CHAR})
            matrix.GlobalAttributes(New attribute With {.name = "time", .value = Now.ToString, .type = CDFDataTypes.CHAR})
            matrix.Dimensions(New Dimension("pixels", loadedPixels.Length))

            Dim mzErr As New attribute With {
                .name = "tolerance",
                .type = CDFDataTypes.CHAR,
                .value = tolerance.GetScript
            }

            matrix.AddVariable("mz", New doubles(mz), "pixels", mzErr)
            matrix.AddVariable("intensity", New doubles(intensity), "pixels")
            matrix.AddVariable("x", New integers(x), "pixels")
            matrix.AddVariable("y", New integers(y), "pixels")
        End Using
    End Sub

    <Extension>
    Public Function GetMzTolerance(cdf As netCDFReader) As Tolerance
        Dim mz As variable = cdf.getDataVariableEntry("mz")
        Dim errStr As String = mz.FindAttribute("tolerance")?.value

        Return Tolerance.ParseScript(errStr)
    End Function

    <Extension>
    Public Function GetMsiDimension(cdf As netCDFReader) As Size
        Dim w As Integer = CType(cdf!width, Integer)
        Dim h As Integer = CType(cdf!height, Integer)

        Return New Size With {
            .Width = w,
            .Height = h
        }
    End Function

    <Extension>
    Public Iterator Function LoadPixelsData(cdf As netCDFReader) As IEnumerable(Of PixelData)
        Dim mz As doubles = cdf.getDataVariable("mz")
        Dim intensity As doubles = cdf.getDataVariable("intensity")
        Dim x As integers = cdf.getDataVariable("x")
        Dim y As integers = cdf.getDataVariable("y")

        For i As Integer = 0 To mz.Length - 1
            Yield New PixelData With {
                .mz = mz(i),
                .intensity = intensity(i),
                .x = x(i),
                .y = y(i)
            }
        Next
    End Function

    <Extension>
    Public Function CreatePixelReader(cdf As netCDFReader) As ReadRawPack
        Dim size As Size = cdf.GetMsiDimension
        Dim pixels As New List(Of mzPackPixel)
        Dim xy = cdf.LoadPixelsData.GroupBy(Function(p) p.x).ToArray
        Dim scan As ScanMS1

        For Each x In xy
            Dim ylist = x.GroupBy(Function(p) p.y).ToArray

            For Each y In ylist
                scan = New ScanMS1 With {
                    .mz = y.Select(Function(m) m.mz).ToArray,
                    .into = y.Select(Function(m) m.intensity).ToArray,
                    .meta = New Dictionary(Of String, String) From {
                        {"x", x.Key},
                        {"y", y.Key}
                    }
                }
                pixels += New mzPackPixel(scan)
            Next
        Next

        Return New ReadRawPack(pixels, size)
    End Function

End Module
