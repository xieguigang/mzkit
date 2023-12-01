#Region "Microsoft.VisualBasic::96087a5f9b47326c6bfcfb43ef4a73db, mzkit\src\assembly\mzPack\Converter.vb"

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

'   Total Lines: 201
'    Code Lines: 170
' Comment Lines: 5
'   Blank Lines: 26
'     File Size: 8.16 KB


' Module Converter
' 
'     Function: GetUVScans, LoadimzML, LoadMsp, LoadMzML, LoadRawFileAuto
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports stdNum = System.Math

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
            .GroupBy(Function(t) Val(t.rt), Function(a, b) stdNum.Abs(a - b) <= 2) _
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
            .GroupBy(Function(t) Val(t.RetentionTime), Function(a, b) stdNum.Abs(a - b) <= 2) _
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
                .MS = New mzXMLScans(mzErr:=tolerance, intocutoff:=intocutoff) _
                    .Load(xml, progress) _
                    .ToArray,
                .source = SolveTagSource(xml)
            }
        ElseIf xml.ExtensionSuffix("mzML") Then
mzML:       Return LoadMzML(xml, tolerance, intocutoff, progress)
        ElseIf xml.ExtensionSuffix("imzML") Then
imzML:      Return LoadimzML(xml, Sub(p, msg) progress($"{msg}...{p}%"))
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

    Public Function LoadimzML(xml As String, Optional progress As RunSlavePipeline.SetProgressEventHandler = Nothing) As mzPack
        Dim scans As New List(Of ScanMS1)
        Dim metadata As imzMLMetadata = imzMLMetadata.ReadHeaders(imzml:=xml)
        Dim ibdStream As Stream = xml.ChangeSuffix("ibd").Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        Dim ibd As New ibdReader(ibdStream, metadata.format)
        Dim pixel As ScanMS1
        Dim ms As ms2()
        Dim allscans As ScanData() = imzML.XML.LoadScans(xml).ToArray
        Dim i As Integer = 0
        Dim d As Integer = allscans.Length / 100 * 8
        Dim j As i32 = 0
        Dim msiMetadata As New Dictionary(Of String, String)
        Dim ptag As String
        Dim filename As String = metadata.sourcefiles.First.FileName

        msiMetadata!width = metadata.dims.Width
        msiMetadata!height = metadata.dims.Height
        msiMetadata!resolution = (metadata.resolution.Width + metadata.resolution.Height) / 2

        For Each scan As ScanData In allscans
            ms = ibd.GetMSMS(scan)
            ptag = If(scan.polarity = IonModes.Positive, "+", If(scan.polarity = IonModes.Negative, "-", "?"))
            pixel = New ScanMS1 With {
                .meta = New Dictionary(Of String, String) From {
                    {"x", scan.x},
                    {"y", scan.y}
                },
                .TIC = scan.totalIon,
                .scan_id = $"[MS1][{scan.x},{scan.y}] [{filename}] {ptag} {scan.spotID} npeaks: {ms.Length} totalIon: {scan.totalIon.ToString("G2")} [{scan.mass.Min} - {scan.mass.Max}]",
                .mz = ms.Select(Function(m) m.mz).ToArray,
                .into = ms.Select(Function(m) m.intensity).ToArray
            }
            scans.Add(pixel)
            i += 1

            If Not progress Is Nothing AndAlso ++j = d Then
                j = 0
                progress(100 * (i / allscans.Length), pixel.scan_id & $" ({i}/{allscans.Length})")
            End If
        Next

        Call ibd.Dispose()

        Return New mzPack With {
            .MS = scans.ToArray,
            .source = If(metadata.sourcefiles.FirstOrDefault, SolveTagSource(xml)),
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
        Dim UV As New ChromatogramOverlap
        Dim PDA As New List(Of ChromatogramTick)

        For Each time_scan As GeneralSignal In scanLoader.GetUVScans(UVdetecor)
            Dim scan_time As Double = time_scan.meta!scan_time
            Dim TIC As Double = time_scan.meta!total_ion_current
            Dim scanId As String = $"[{time_scan.meta!scan}] {ExtractUVData.UVScanType} {TIC.ToString("G3")}@{scan_time.ToString("F3")}s"

            PDA += New ChromatogramTick With {
                .Time = scan_time,
                .Intensity = TIC
            }
            UV(scanId) = New DataReader.Chromatogram With {
                .TIC = time_scan.Strength,
                .scan_time = time_scan.Measures,
                .BPC = .TIC
            }

            If Not progress Is Nothing Then
                Call progress(scanId)
            End If
        Next

        Dim PDAPlot As New ChromatogramOverlap

        PDAPlot("PDA") = New DataReader.Chromatogram With {
            .scan_time = PDA.Select(Function(t) t.Time).ToArray,
            .TIC = PDA.Select(Function(t) t.Intensity).ToArray,
            .BPC = .TIC
        }

        Dim otherScanner As New Dictionary(Of String, ChromatogramOverlap)

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

        Dim time_scans As ChromatogramOverlap = mzpack.Scanners(ExtractUVData.UVScanType)
        Dim PDA As DataReader.Chromatogram = mzpack.Scanners!PDA!PDA
        Dim scan As DataReader.Chromatogram
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
