﻿#Region "Microsoft.VisualBasic::953687bb3110fcdc2c685acaf36fc8dc, assembly\mzPack\Converter.vb"

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

    '   Total Lines: 378
    '    Code Lines: 296 (78.31%)
    ' Comment Lines: 37 (9.79%)
    '    - Xml Docs: 97.30%
    ' 
    '   Blank Lines: 45 (11.90%)
    '     File Size: 16.14 KB


    ' Module Converter
    ' 
    '     Function: GetUVScans, LoadAsciiFileAuto, LoadimzML, loadimzMLMetadata, LoadMgf
    '               LoadMsp, LoadMzML, LoadRawFileAuto, LoadScanStream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports chromatogramObj = BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram.Chromatogram
Imports std = System.Math
Imports toleranceErr = BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Tolerance

Public Module Converter

    ''' <summary>
    ''' load ions from mgf/msp file and then convert to mzpack
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function LoadAsciiFileAuto(file As String) As mzPack
        If file.ExtensionSuffix("mgf") Then
            Return LoadMgf(file)
        ElseIf file.ExtensionSuffix("msp") Then
            Return LoadMsp(file)
        Else
            Throw New NotImplementedException(file.FileName)
        End If
    End Function

    Public Function LoadMgf(file As String) As mzPack
        Dim data = MgfReader.ReadIons(file).IonPeaks.ToArray
        Dim rt_scans = data _
            .GroupBy(Function(t) Val(t.rt), Function(a, b) std.Abs(a - b) <= 2) _
            .OrderBy(Function(d) Val(d.name)) _
            .ToArray
        Dim ms1 = rt_scans _
            .Select(Function(group, i)
                        Dim ms2 = group.Select(Function(a) a.Scan2).ToArray
                        Dim bpc = ms2.OrderByDescending(Function(a) a.intensity).First

                        Return New ScanMS1 With {
                            .products = ms2,
                            .BPC = bpc.intensity,
                            .into = ms2.Select(Function(a) a.intensity).ToArray,
                            .mz = ms2.Select(Function(a) a.parentMz).ToArray,
                            .rt = Val(group.name),
                            .TIC = .into.Sum,
                            .scan_id = $"[MS1] scan_{i + 1}, {ms2.Length} ions, basePeak_m/z={bpc.parentMz}, total_ions={ .TIC}"
                        }
                    End Function) _
            .ToArray

        Return New mzPack With {
            .Application = FileApplicationClass.LCMS,
            .MS = ms1,
            .source = SolveTagSource(file)
        }
    End Function

    Public Function LoadMsp(file As String) As mzPack
        Dim msp As MspData() = MspData.Load(file).ToArray
        Dim rt_scans = msp _
            .GroupBy(Function(t) Val(t.RetentionTime), Function(a, b) std.Abs(a - b) <= 2) _
            .OrderBy(Function(d) Val(d.name)) _
            .ToArray
        Dim ms1 = rt_scans _
            .Select(Function(group, i)
                        Dim ms2 = group.Select(Function(a) MspData.ToScan2(a)).ToArray
                        Dim bpc = ms2.OrderByDescending(Function(a) a.intensity).First

                        Return New ScanMS1 With {
                            .products = ms2,
                            .BPC = bpc.intensity,
                            .into = ms2.Select(Function(a) a.intensity).ToArray,
                            .mz = ms2.Select(Function(a) a.parentMz).ToArray,
                            .rt = Val(group.name),
                            .TIC = .into.Sum,
                            .scan_id = $"[MS1] scan_{i + 1}, {ms2.Length} ions, basePeak_m/z={bpc.parentMz}, total_ions={ .TIC}"
                        }
                    End Function) _
            .ToArray

        Return New mzPack With {
            .Application = FileApplicationClass.LCMS,
            .MS = ms1,
            .source = SolveTagSource(file)
        }
    End Function

    ''' <summary>
    ''' A unify method for load mzpack data from mzXML/mzML raw data file
    ''' </summary>
    ''' <param name="xml">the file path of the raw mzXML/mzML data file.</param>
    ''' <returns></returns>
    Public Function LoadRawFileAuto(xml As String,
                                    Optional tolerance$ = "ppm:20",
                                    Optional intocutoff As Double = 0.0001,
                                    Optional progress As Action(Of String) = Nothing,
                                    Optional prefer As String = Nothing) As mzPack

        If xml.ExtensionSuffix("mzXML") Then
mzXML:      Return New mzPack With {
                .MS = New mzXMLScans(mzErr:=tolerance, intocutoff:=intocutoff) With {
                    .verbose = Not progress Is Nothing
                } _
                    .Load(xml, progress) _
                    .ToArray,
                .source = SolveTagSource(xml)
            }
        ElseIf xml.ExtensionSuffix("mzML") Then
mzML:       Return LoadMzML(xml, tolerance, intocutoff, progress)
        ElseIf xml.ExtensionSuffix("imzML") Then
imzML:      Return LoadimzML(xml, intocutoff,
                             defaultIon:=IonModes.Positive,
                             make_centroid:=toleranceErr.ParseScript(tolerance),
                             progress:=Sub(p, msg) progress($"{msg}...{p}%"))
        Else
            If Not prefer.StringEmpty Then
                Select Case prefer.ToLower
                    Case "mzxml" : GoTo mzXML
                    Case "mzml" : GoTo mzML
                    Case "imzml" : GoTo imzML
                End Select
            End If

            Throw New NotImplementedException(xml.ExtensionSuffix)
        End If
    End Function

    ''' <summary>
    ''' Load the ms scan rawdata from ibd file on the fly 
    ''' </summary>
    ''' <param name="allscans">
    ''' the scan metadata that read from the imzML file
    ''' </param>
    ''' <param name="ibd">
    ''' the data reader for the ibd rawdata file
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function LoadScanStream(allscans As ScanData(), ibd As ibdReader,
                                            Optional sourceName As String = Nothing,
                                            Optional defaultIon As IonModes = IonModes.Positive,
                                            Optional noiseCutoff As Double = 0,
                                            Optional make_centroid As toleranceErr = Nothing,
                                            Optional progress As RunSlavePipeline.SetProgressEventHandler = Nothing,
                                            Optional verbose As Integer = 100) As IEnumerable(Of ScanMS1)
        Dim mz As Double() = Nothing
        Dim intensity As Double() = Nothing
        Dim pixel As ScanMS1
        Dim ms As ms2()
        Dim ptag As String
        Dim maxinto As Double
        Dim i As Integer = 0
        Dim d As Integer = allscans.Length / If(verbose > 0, verbose, 100) * 8
        Dim j As i32 = 0
        Dim zero_cutoff As New RelativeIntensityCutoff(0.0)

        If sourceName.StringEmpty(, True) Then
            sourceName = ibd.fileName
            If sourceName.StringEmpty(, True) Then
                sourceName = "unknown"
            End If
        End If

        For Each scan As ScanData In allscans
            Call ibd.GetMSVector(scan, mz, intensity)

            If scan.polarity = IonModes.Unknown Then
                scan.polarity = defaultIon
            End If

            ' filter low intensity noise at here
            If noiseCutoff > 0 AndAlso intensity.Length > 0 Then
                maxinto = intensity.Max
                ms = mz _
                    .Select(Function(mzi, offset) New ms2(mzi, intensity(offset))) _
                    .AsParallel _
                    .Where(Function(a) a.intensity / maxinto >= noiseCutoff) _
                    .ToArray
            Else
                ms = mz _
                    .Select(Function(mzi, offset) New ms2(mzi, intensity(offset))) _
                    .ToArray
            End If

            Dim scan_mass As DoubleRange = scan.mass

            If scan_mass Is Nothing Then
                If mz.IsNullOrEmpty Then
                    scan_mass = New DoubleRange(0, 1000)
                Else
                    scan_mass = New DoubleRange(mz.Min, mz.Max)
                End If
            End If
            If scan.totalIon <= 0.0 Then
                scan.totalIon = intensity.Sum
            End If
            If make_centroid IsNot Nothing Then
                ' the noise filter as already be done at the above code block
                ' so the intensity cutoff at here for centroid is zero
                ms = ms.Centroid(make_centroid, zero_cutoff).ToArray
            End If

            ptag = If(scan.polarity = IonModes.Positive, "+", If(scan.polarity = IonModes.Negative, "-", "?"))
            pixel = New ScanMS1 With {
                .meta = New Dictionary(Of String, String) From {
                    {"x", scan.x},
                    {"y", scan.y}
                },
                .TIC = scan.totalIon,
                .scan_id = $"[MS1][{scan.x},{scan.y}] [{sourceName}] {ptag} {scan.spotID} npeaks: {ms.Length} totalIon: {scan.totalIon.ToString("G2")} [{scan_mass.Min} - {scan_mass.Max}]",
                .mz = ms.Select(Function(m) m.mz).ToArray,
                .into = ms.Select(Function(m) m.intensity).ToArray
            }
            i += 1

            Yield pixel

            If Not progress Is Nothing AndAlso ++j = d Then
                j = 0
                progress(100 * (i / allscans.Length), pixel.scan_id & $" ({i}/{allscans.Length})")
            End If
        Next
    End Function

    ''' <summary>
    ''' load imzML rawdata and construct a new mzpack object
    ''' </summary>
    ''' <param name="xml"></param>
    ''' <param name="noiseCutoff">
    ''' the intensity cutoff value for the scan peaks data, value 
    ''' in range [0,1), is a percentage value cutoff.
    ''' </param>
    ''' <param name="progress"></param>
    ''' <returns></returns>
    Public Function LoadimzML(xml As String,
                              Optional noiseCutoff As Double = 0,
                              Optional defaultIon As IonModes = IonModes.Positive,
                              Optional make_centroid As toleranceErr = Nothing,
                              Optional progress As RunSlavePipeline.SetProgressEventHandler = Nothing) As mzPack

        Dim allscans As ScanData() = Nothing
        Dim metadata As imzMLMetadata = Nothing
        Dim scans As New List(Of ScanMS1)
        Dim mzpack As mzPack = loadimzMLMetadata(xml, allscans, metadata)
        Dim ibdStream As Stream = xml.ChangeSuffix("ibd").Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        Dim ibd As New ibdReader(ibdStream, metadata.format)
        Dim filename As String = metadata.sourcefiles.First.FileName

        scans.AddRange(allscans.LoadScanStream(ibd, filename, defaultIon, noiseCutoff, make_centroid, progress))
        ibd.Dispose()
        mzpack.MS = scans.ToArray

        Return mzpack
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <returns>
    ''' this function just returns a mzpack object **without** scan data.
    ''' </returns>
    Public Function loadimzMLMetadata(imzML As String,
                                      Optional ByRef allscans As ScanData() = Nothing,
                                      Optional ByRef metadata As imzMLMetadata = Nothing) As mzPack

        Dim msiMetadata As Dictionary(Of String, String)

        metadata = imzMLMetadata.ReadHeaders(imzml:=imzML)
        allscans = MarkupData.imzML.XML.LoadScans(imzML).ToArray

        If allscans.Any AndAlso (metadata.dims.Width <= 1 OrElse metadata.dims.Height <= 1) Then
            metadata.dims = New Size(
                allscans.Select(Function(a) a.x).Max,
                allscans.Select(Function(a) a.y).Max
            )
        End If

        msiMetadata = metadata.AsList
        msiMetadata!width = metadata.dims.Width
        msiMetadata!height = metadata.dims.Height
        msiMetadata!resolution = (metadata.resolution.Width + metadata.resolution.Height) / 2

        Return New mzPack With {
            .source = If(metadata.sourcefiles.FirstOrDefault, SolveTagSource(imzML)),
            .Application = FileApplicationClass.MSImaging,
            .metadata = msiMetadata
        }
    End Function

    Public Function LoadMzML(xml As String,
                             Optional tolerance$ = "ppm:20",
                             Optional intocutoff As Double = 0.0001,
                             Optional progress As Action(Of String) = Nothing) As mzPack

        Dim UVdetecor As String = ExtractUVData.GetPhotodiodeArrayDetectorInstrumentConfigurationId(xml)
        Dim scanLoader As New mzMLScans(mzErr:=tolerance, intocutoff:=intocutoff)
        Dim MS As ScanMS1() = scanLoader.Load(xml, progress).ToArray
        Dim UV As New ChromatogramOverlapList
        Dim PDA As New List(Of ChromatogramTick)

        For Each time_scan As GeneralSignal In scanLoader.GetUVScans(UVdetecor)
            Dim scan_time As Double = time_scan.meta!scan_time
            Dim TIC As Double = time_scan.meta!total_ion_current
            Dim scanId As String = $"[{time_scan.meta!scan}] {ExtractUVData.UVScanType} {TIC.ToString("G3")}@{scan_time.ToString("F3")}s"

            PDA += New ChromatogramTick With {
                .Time = scan_time,
                .Intensity = TIC
            }
            UV(scanId) = New chromatogramObj With {
                .TIC = time_scan.Strength,
                .scan_time = time_scan.Measures,
                .BPC = .TIC
            }

            If Not progress Is Nothing Then
                Call progress(scanId)
            End If
        Next

        Dim PDAPlot As New ChromatogramOverlapList

        PDAPlot("PDA") = New chromatogramObj With {
            .scan_time = PDA.Select(Function(t) t.Time).ToArray,
            .TIC = PDA.Select(Function(t) t.Intensity).ToArray,
            .BPC = .TIC
        }

        Dim otherScanner As New Dictionary(Of String, ChromatogramOverlapList)

        If UV.length > 0 Then
            otherScanner(ExtractUVData.UVScanType) = UV
            otherScanner("PDA") = PDAPlot
        End If

        Return New mzPack With {
            .MS = MS,
            .Scanners = otherScanner,
            .source = SolveTagSource(xml)
        }
    End Function

    <Extension>
    Public Iterator Function GetUVScans(mzpack As mzPack) As IEnumerable(Of UVScan)
        If mzpack.Scanners Is Nothing Then
            Return
        ElseIf Not mzpack.Scanners.ContainsKey(ExtractUVData.UVScanType) Then
            Return
        End If

        Dim time_scans As ChromatogramOverlapList = mzpack.Scanners(ExtractUVData.UVScanType)
        Dim PDA As chromatogramObj = mzpack.Scanners!PDA!PDA
        Dim scan As chromatogramObj
        Dim UV As UVScan

        For Each timeId As SeqValue(Of String) In time_scans.overlaps.Keys.SeqIterator
            scan = time_scans(timeId.value)
            UV = New UVScan With {
                .scan_time = PDA.scan_time(timeId),
                .total_ion_current = PDA.TIC(timeId),
                .wavelength = scan.scan_time,
                .intensity = scan.TIC
            }

            Yield UV
        Next
    End Function
End Module
