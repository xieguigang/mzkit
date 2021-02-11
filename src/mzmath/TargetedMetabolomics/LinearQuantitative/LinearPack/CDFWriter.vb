#Region "Microsoft.VisualBasic::c63a55e865d9f80ce2f9ca3fbfd0160e, TargetedMetabolomics\LinearQuantitative\LinearPack\CDFWriter.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Module CDFWriter
    ' 
    '         Function: Write, writeSampleNames
    ' 
    '         Sub: peakLinearNames, Write, writeGlobals, writeLinear, writeLinears
    '              writePeak, writePeakNames, writePeakSamples, writeSampleLevel, writeSampleLevels
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
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
            Call pack.writeSampleLevels(file)
        End Sub

        <Extension>
        Private Sub writeSampleLevels(pack As LinearPack, file As netCDF.CDFWriter)
            Dim allSampleNames As String() = pack.writeSampleNames(file)
            Dim size As New Dimension With {.name = "samplePoints", .size = allSampleNames.Length}

            For Each level In pack.reference
                Call level.Value.writeSampleLevel(level.Key, allSampleNames, size, file)
            Next
        End Sub

        <Extension>
        Private Sub writeSampleLevel(levels As SampleContentLevels,
                                     ionName As String,
                                     allSampleNames As String(),
                                     size As Dimension,
                                     file As netCDF.CDFWriter)

            Dim data As CDFData = allSampleNames.Select(Function(lv) levels(lv)).ToArray
            Dim attrs As attribute() = {
                New attribute With {.name = "directMap", .type = CDFDataTypes.BOOLEAN, .value = levels.directMap}
            }

            file.AddVariable($"levels\{ionName}", data, size, attrs)
        End Sub

        <Extension>
        Private Function writeSampleNames(pack As LinearPack, file As netCDF.CDFWriter) As String()
            Dim allSampleNames As String() = pack.peakSamples _
                .Select(Function(p) p.SampleName) _
                .Distinct _
                .ToArray
            Dim data As New CDFData With {.chars = allSampleNames.GetJson}
            Dim size As New Dimension With {.name = "sizeofSamples", .size = data.Length}
            Dim attrs As attribute() = {
                New attribute With {.name = "size", .type = CDFDataTypes.INT, .value = allSampleNames.Length}
            }

            file.AddVariable("sampleNames", data, size, attrs)

            Return allSampleNames
        End Function

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
