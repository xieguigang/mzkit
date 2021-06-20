Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

Public Module PixelsCDF

    <Extension>
    Public Sub CreateCDF(loadedPixels As PixelData(), file As Stream, dimension As Size)
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

            matrix.AddVariable("mz", New doubles(mz), "pixels")
            matrix.AddVariable("intensity", New doubles(intensity), "pixels")
            matrix.AddVariable("x", New integers(x), "pixels")
            matrix.AddVariable("y", New integers(y), "pixels")
        End Using
    End Sub
End Module
