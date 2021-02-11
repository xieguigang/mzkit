Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace LinearQuantitative.Data

    Module CDFWriter

        <Extension>
        Public Function Write(pack As LinearPack, file As Stream) As Boolean
            Using cdffile As New netCDF.CDFWriter(file)
                Call pack.Write(cdffile)
            End Using

            Return True
        End Function

        <Extension>
        Private Sub Write(pack As LinearPack, file As netCDF.CDFWriter)
            Call pack.writeGlobals(file)
            Call pack.peakLinearNames(file)
            Call pack.writeLinears(file)
        End Sub

        <Extension>
        Private Sub peakLinearNames(pack As LinearPack, file As netCDF.CDFWriter)
            Dim data As New CDFData With {.chars = pack.linears.Select(Function(l) l.name).GetJson}
            Dim size As New Dimension With {.name = "sizeofLinears", .size = data.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "linears", .type = CDFDataTypes.INT, .value = pack.linears.Length}
            }

            file.AddVariable("linears", data, size, attrs)
        End Sub

        <Extension>
        Private Sub writeLinears(pack As LinearPack, file As netCDF.CDFWriter)
            For Each line As StandardCurve In pack.linears
                Call line.writeLinear(file)
            Next
        End Sub

        <Extension>
        Private Sub writeLinear(linear As StandardCurve, file As netCDF.CDFWriter)
            Dim attrs As attribute() = {
                New attribute With {.name = "name", .type = CDFDataTypes.CHAR, .value = linear.name},
                New attribute With {.name = "points", .type = CDFDataTypes.INT, .value = linear.points.Length},
                New attribute With {.name = "R2", .type = CDFDataTypes.CHAR, .value = linear.linear.R2}
            }


        End Sub

        Private Function internalCDF() As MemoryStream
            Using ms As New MemoryStream, cdf As New netCDF.CDFWriter(ms)


                Return ms
            End Using
        End Function

        <Extension>
        Private Sub writeGlobals(pack As LinearPack, file As netCDF.CDFWriter)
            Dim title As New attribute With {.name = "title", .type = CDFDataTypes.CHAR, .value = pack.title}
            Dim time As New attribute With {.name = "time", .type = CDFDataTypes.CHAR, .value = pack.time.ToString}
            Dim github As New attribute With {.name = "github", .type = CDFDataTypes.CHAR, .value = "https://github.com/xieguigang/mzkit"}
            Dim linears As New attribute With {.name = "linears", .type = CDFDataTypes.INT, .value = pack.linears.Length}
            Dim peaks As New attribute With {.name = "peaks", .type = CDFDataTypes.INT, .value = pack.peakSamples.Length}

            Call file.GlobalAttributes(title, time, github, linears, peaks)
        End Sub
    End Module
End Namespace