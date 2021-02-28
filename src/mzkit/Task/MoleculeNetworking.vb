#Region "Microsoft.VisualBasic::a087eb67e3748d6c8b2a6fd688da619c, src\mzkit\Task\MoleculeNetworking.vb"

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

' Module MoleculeNetworking
' 
'     Function: CreateMatrix, GetSpectrum
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

Public Module MoleculeNetworking

    <Extension>
    Public Iterator Function CreateMatrix(ms2 As PeakMs2(),
                                          cutoff As Double,
                                          tolerance As Tolerance,
                                          progressCallback As Action(Of String)) As IEnumerable(Of DataSet)
        Dim i As i32 = 1

        For Each scan In ms2
            Dim scores = ms2 _
                .Where(Function(a) Not a Is scan) _
                .AsParallel _
                .Select(Function(a)
                            Dim id As String = a.lib_guid
                            Dim score = GlobalAlignment.TwoDirectionSSM(scan.mzInto, a.mzInto, tolerance)

                            Return (id, System.Math.Min(score.forward, score.reverse))
                        End Function) _
                .ToArray

            Call progressCallback($"[{++i}/{ms2.Length}] {scan.ToString} has {scores.Where(Function(a) a.Item2 >= cutoff).Count} homologous spectrum")

            Yield New DataSet With {
                .ID = scan.lib_guid,
                .Properties = scores.ToDictionary(Function(a) a.id, Function(a) a.Item2)
            }
        Next
    End Function

    <Extension>
    Public Function GetSpectrum(raw As Raw, scanId As String, cutoff As LowAbundanceTrimming, Optional ByRef properties As SpectrumProperty = Nothing) As LibraryMatrix
        Dim data As CDFData
        Dim attrs As attribute()
        Dim cacheFile As String

        If scanId.StartsWith("[MS1]") Then
            cacheFile = raw.ms1_cache
        Else
            cacheFile = raw.ms2_cache
        End If

        Using cache As New netCDFReader(cacheFile)
            data = cache.getDataVariable(cache.getDataVariableEntry(scanId))
            attrs = cache.getDataVariableEntry(scanId).attributes
        End Using

        Dim rawData As ms2() = data.numerics.AsMs2.ToArray
        Dim scanData As New LibraryMatrix With {
            .name = scanId,
            .centroid = False,
            .ms2 = rawData.Centroid(Tolerance.DeltaMass(0.1), cutoff).ToArray
        }
        Dim pa As New PeakAnnotation

        properties = New SpectrumProperty(scanId, raw.source.FileName, attrs)

        If properties.precursorMz > 0 Then
            scanData.ms2 = pa.RunAnnotation(properties.precursorMz, scanData.ms2).products
        End If

        Return scanData
    End Function

    <Extension>
    Public Iterator Function SearchFiles(spectrum As LibraryMatrix,
                                         files As IEnumerable(Of Raw),
                                         tolerance As Tolerance,
                                         dotcutoff As Double,
                                         progress As Action(Of String)) As IEnumerable(Of NamedCollection(Of AlignmentOutput))

        For Each result As NamedCollection(Of AlignmentOutput) In files _
            .AsParallel _
            .Select(Function(a) spectrum.alignSearch(a, tolerance, dotcutoff))

            Call progress($"Spectrum search job done! [{result.name}]")

            Yield result
        Next
    End Function

    <Extension>
    Private Function alignSearch(spectrum As LibraryMatrix,
                                 file As Raw,
                                 tolerance As Tolerance,
                                 dotcutoff As Double) As NamedCollection(Of AlignmentOutput)

        Dim alignments As New List(Of AlignmentOutput)
        Dim data As CDFData
        Dim ref As ms2()

        Using cache As New netCDFReader(file.ms2_cache)
            For Each scan As Ms1ScanEntry In file.scans
                For Each subject As ScanEntry In scan.products.SafeQuery
                    Dim scanId = subject.id

                    data = cache.getDataVariable(cache.getDataVariableEntry(scanId))
                    ref = data.numerics.AsMs2.ToArray

                    Dim scores = GlobalAlignment.TwoDirectionSSM(spectrum.ms2, ref, tolerance)

                    If stdNum.Min(scores.forward, scores.reverse) >= dotcutoff Then
                        alignments += New AlignmentOutput With {
                            .alignments = GlobalAlignment _
                                .CreateAlignment(spectrum.ms2, ref, tolerance) _
                                .ToArray,
                            .forward = scores.forward,
                            .reverse = scores.reverse,
                            .query = New Meta With {.id = spectrum.name, .mz = Double.NaN, .scan_time = Double.NaN},
                            .reference = New Meta With {.id = scanId, .mz = subject.mz, .scan_time = subject.rt}
                        }
                    End If
                Next
            Next
        End Using

        Return New NamedCollection(Of AlignmentOutput)(file.source.FileName, alignments, file.source)
    End Function
End Module
