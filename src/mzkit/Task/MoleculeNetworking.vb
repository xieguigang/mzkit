#Region "Microsoft.VisualBasic::a81e8bd6f9a0b04faa20da9dc5db5131, mzkit\src\mzkit\Task\MoleculeNetworking.vb"

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


    ' Code Statistics:

    '   Total Lines: 237
    '    Code Lines: 194
    ' Comment Lines: 0
    '   Blank Lines: 43
    '     File Size: 9.45 KB


    ' Module MoleculeNetworking
    ' 
    '     Function: (+2 Overloads) alignSearch, CreateMatrix, GetSpectrum, RunMetaDNA, (+2 Overloads) SearchFiles
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports BioNovoGene.BioDeep.MetaDNA
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
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

    ReadOnly ms1diff As Tolerance = Tolerance.DeltaMass(0.001)
    ReadOnly ms2diff As Tolerance = Tolerance.DeltaMass(0.3)

    <Extension>
    Public Function GetSpectrum(raw As Raw,
                                scanId As String,
                                cutoff As LowAbundanceTrimming,
                                reload As Action(Of String, String),
                                Optional ByRef properties As SpectrumProperty = Nothing) As LibraryMatrix
        Dim attrs As ScanMS2
        Dim msLevel As Integer

        If Not raw.isLoaded Then
            Call raw.LoadMzpack(reload)
        End If

        attrs = raw.FindMs2Scan(scanId)

        If attrs Is Nothing OrElse scanId.Contains("[MS1]") Then
            Dim ms1 = raw.FindMs1Scan(scanId)

            msLevel = 1
            attrs = New ScanMS2 With {
                .scan_id = ms1.scan_id,
                .activationMethod = ActivationMethods.Unknown,
                .centroided = False,
                .charge = 0,
                .collisionEnergy = 0,
                .intensity = ms1.TIC,
                .rt = ms1.rt,
                .polarity = 0,
                .parentMz = 0,
                .mz = ms1.mz,
                .into = ms1.into
            }
        Else
            msLevel = 2
        End If

        Dim scanData As New LibraryMatrix With {
            .name = scanId,
            .centroid = False,
            .ms2 = attrs _
                .GetMs _
                .ToArray _
                .Centroid(If(msLevel = 1, ms1diff, ms2diff), cutoff) _
                .ToArray
        }
        Dim pa As New PeakAnnotation(0.1)

        properties = New SpectrumProperty(scanId, raw.source.FileName, msLevel, attrs)

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
                                         progress As Action(Of String),
                                         reload As Action(Of String, String)) As IEnumerable(Of NamedCollection(Of AlignmentOutput))

        For Each result As NamedCollection(Of AlignmentOutput) In files _
            .AsParallel _
            .Select(Function(a) spectrum.alignSearch(a, tolerance, dotcutoff, reload))

            Call progress($"Spectrum search job done! [{result.name}]")

            Yield result
        Next
    End Function

    <Extension>
    Public Iterator Function SearchFiles(isotopic As IsotopeDistribution,
                                         files As IEnumerable(Of Raw),
                                         tolerance As Tolerance,
                                         dotcutoff As Double,
                                         progress As Action(Of String),
                                         reload As Action(Of String, String)) As IEnumerable(Of NamedCollection(Of AlignmentOutput))

        For Each result As NamedCollection(Of AlignmentOutput) In files _
            .AsParallel _
            .Select(Function(a) isotopic.alignSearch(a, tolerance, dotcutoff, reload))

            Call progress($"Spectrum search job done! [{result.name}]")

            Yield result
        Next
    End Function

    <Extension>
    Private Function alignSearch(isotopic As IsotopeDistribution,
                                 file As Raw,
                                 tolerance As Tolerance,
                                 dotcutoff As Double,
                                 reload As Action(Of String, String)) As NamedCollection(Of AlignmentOutput)

        Static pos As MzCalculator() = {"[M]+", "[M+H]+"} _
            .Select(Function(name)
                        Return Parser.ParseMzCalculator(name, "+")
                    End Function) _
            .ToArray
        Static neg As MzCalculator() = {"[M]-", "[M-H]-"} _
            .Select(Function(name)
                        Return Parser.ParseMzCalculator(name, "-")
                    End Function) _
            .ToArray

        Dim alignments As New List(Of AlignmentOutput)
        Dim cos As New CosAlignment(tolerance, New RelativeIntensityCutoff(0.01))
        Dim align As AlignmentOutput
        Dim types As MzCalculator()

        If Not file.isLoaded Then
            Call file.LoadMzpack(reload)
        End If

        For Each scan As ScanMS1 In file.GetMs1Scans
            For Each subject As ScanMS2 In scan.products.SafeQuery
                If subject.polarity > -1 Then
                    types = pos
                Else
                    types = neg
                End If

                If Not types.Any(Function(a) stdNum.Abs(a.CalcMZ(isotopic.exactMass) - subject.parentMz) < 0.3) Then
                    Continue For
                End If

                align = isotopic.AlignIsotopic(subject.GetMatrix, cos)

                If stdNum.Min(align.forward, align.reverse) >= dotcutoff Then
                    alignments += align
                End If
            Next
        Next

        Return New NamedCollection(Of AlignmentOutput)(file.source.FileName, alignments, file.source)
    End Function

    <Extension>
    Private Function alignSearch(spectrum As LibraryMatrix,
                                 file As Raw,
                                 tolerance As Tolerance,
                                 dotcutoff As Double,
                                 reload As Action(Of String, String)) As NamedCollection(Of AlignmentOutput)

        Dim alignments As New List(Of AlignmentOutput)
        Dim ref As ms2()

        If Not file.isLoaded Then
            Call file.LoadMzpack(reload)
        End If

        For Each scan As ScanMS1 In file.GetMs1Scans
            For Each subject As ScanMS2 In scan.products.SafeQuery
                Dim scanId = subject.scan_id

                ref = subject.GetMs.ToArray

                Dim scores = GlobalAlignment.TwoDirectionSSM(spectrum.ms2, ref, tolerance)

                If stdNum.Min(scores.forward, scores.reverse) >= dotcutoff Then
                    alignments += New AlignmentOutput With {
                            .alignments = GlobalAlignment _
                                .CreateAlignment(spectrum.ms2, ref, tolerance) _
                                .ToArray,
                            .forward = scores.forward,
                            .reverse = scores.reverse,
                            .query = New Meta With {.id = spectrum.name, .mz = Double.NaN, .scan_time = Double.NaN},
                            .reference = New Meta With {.id = scanId, .mz = subject.parentMz, .scan_time = subject.rt}
                        }
                End If
            Next
        Next

        Return New NamedCollection(Of AlignmentOutput)(file.source.FileName, alignments, file.source)
    End Function

    <Extension>
    Public Iterator Function RunMetaDNA(raw As IEnumerable(Of PeakMs2)) As IEnumerable(Of MetaDNAResult)

    End Function
End Module
