#Region "Microsoft.VisualBasic::ca8164570d3716f7be3f12139c08e1c4, mzkit\src\assembly\mzPack\Converter.vb"

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

    '   Total Lines: 153
    '    Code Lines: 126
    ' Comment Lines: 5
    '   Blank Lines: 22
    '     File Size: 6.29 KB


    ' Module Converter
    ' 
    '     Function: GetUVScans, LoadimzML, LoadMzML, LoadRawFileAuto
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module Converter

    ''' <summary>
    ''' A unify method for load mzpack data from mzXML/mzML raw data file
    ''' </summary>
    ''' <param name="xml">the file path of the raw mzXML/mzML data file.</param>
    ''' <returns></returns>
    Public Function LoadRawFileAuto(xml As String,
                                    Optional tolerance$ = "ppm:20",
                                    Optional intocutoff As Double = 0.0001,
                                    Optional progress As Action(Of String) = Nothing) As mzPack

        If xml.ExtensionSuffix("mzXML") Then
            Return New mzPack With {
                .MS = New mzXMLScans(mzErr:=tolerance, intocutoff:=intocutoff).Load(xml, progress).ToArray
            }
        ElseIf xml.ExtensionSuffix("mzML") Then
            Return LoadMzML(xml, tolerance, intocutoff, progress)
        ElseIf xml.ExtensionSuffix("imzML") Then
            Return LoadimzML(xml, Sub(p, msg) progress($"{msg}...{p}%"))
        Else
            Throw New NotImplementedException(xml.ExtensionSuffix)
        End If
    End Function

    Public Function LoadimzML(xml As String, Optional progress As RunSlavePipeline.SetProgressEventHandler = Nothing) As mzPack
        Dim scans As New List(Of ScanMS1)
        Dim ibd As New ibdReader(xml.ChangeSuffix("ibd").Open(FileMode.Open, doClear:=False, [readOnly]:=True), Format.Continuous)
        Dim pixel As ScanMS1
        Dim ms As ms2()
        Dim allscans As ScanData() = imzML.XML.LoadScans(xml).ToArray
        Dim i As Integer = 0
        Dim d As Integer = allscans.Length / 100
        Dim j As i32 = 0

        For Each scan As ScanData In allscans
            ms = ibd.GetMSMS(scan)
            pixel = New ScanMS1 With {
                .meta = New Dictionary(Of String, String) From {
                    {"x", scan.x},
                    {"y", scan.y}
                },
                .TIC = scan.totalIon,
                .scan_id = $"[MS1][{scan.x},{scan.y}] totalIon: {scan.totalIon.ToString("G2")}",
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
            .source = xml.FileName
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
            .Scanners = otherScanner
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
