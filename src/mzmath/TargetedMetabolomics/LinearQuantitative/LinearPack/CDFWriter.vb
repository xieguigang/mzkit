Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
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
            Call pack.writePeakNames(file)
            Call pack.writePeakSamples(file)
        End Sub

        <Extension>
        Private Sub writePeakSamples(pack As LinearPack, file As netCDF.CDFWriter)
            For Each peak As TargetPeakPoint In pack.peakSamples
                Call peak.writePeak(file)
            Next
        End Sub

        <Extension>
        Private Sub writePeak(peak As TargetPeakPoint, file As netCDF.CDFWriter)
            Dim time As Double() = peak.Peak.ticks.Select(Function(t) t.Time).ToArray
            Dim into As Double() = peak.Peak.ticks.Select(Function(t) t.Intensity).ToArray
            Dim data As New CDFData With {.numerics = time.JoinIterates(into).ToArray}
            Dim size As New Dimension With {.name = $"sizeof_{peak.SampleName}\{peak.Name}", .size = data.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "name", .type = CDFDataTypes.CHAR, .value = peak.Name},
                New attribute With {.name = "sample_name", .type = CDFDataTypes.CHAR, .value = peak.SampleName},
                New attribute With {.name = "summary", .type = CDFDataTypes.CHAR, .value = peak.ChromatogramSummary.GetJson},
                New attribute With {.name = "rtmin", .type = CDFDataTypes.CHAR, .value = peak.Peak.window.Max},
                New attribute With {.name = "rtmax", .type = CDFDataTypes.CHAR, .value = peak.Peak.window.Min},
                New attribute With {.name = "maxinto", .type = CDFDataTypes.CHAR, .value = peak.Peak.peakHeight},
                New attribute With {.name = "base", .type = CDFDataTypes.CHAR, .value = peak.Peak.base}
            }

            Call file.AddVariable($"{peak.SampleName}\{peak.Name}", data, size, attrs)
        End Sub

        <Extension>
        Private Sub writePeakNames(pack As LinearPack, file As netCDF.CDFWriter)
            Dim data As New CDFData With {.chars = pack.peakSamples.Select(Function(p) $"{p.SampleName}\{p.Name}").GetJson}
            Dim size As New Dimension With {.name = "sizeofPeaks", .size = pack.peakSamples.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "peaks", .type = CDFDataTypes.INT, .value = pack.linears.Length}
            }

            file.AddVariable("peaks", data, size, attrs)
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

            Using buffer As MemoryStream = StandardCurveCDF.WriteCDF(linear)
                Dim bytes As CDFData = buffer.ToArray
                Dim chunkSize As New Dimension With {.name = $"chunkof_{linear.name}", .size = bytes.byteStream.Length}

                file.AddVariable(linear.name, bytes, chunkSize, attrs)
            End Using
        End Sub

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