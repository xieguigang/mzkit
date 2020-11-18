#Region "Microsoft.VisualBasic::57015c606b43fc535d8df35b8bac7720, src\mzkit\Task\ImportsRawData.vb"

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

' Class ImportsRawData
' 
'     Properties: raw
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: importsMzML, importsMzXML, RunImports
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Text
Imports mzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Public Class ImportsRawData

    ReadOnly source As String
    ReadOnly scatterTemp As String
    ReadOnly temp1, temp2 As String
    ReadOnly showProgress As Action(Of String)
    ReadOnly success As Action
    ReadOnly tolerance As Tolerance = Tolerance.PPM(20)
    ReadOnly intoCutoff As LowAbundanceTrimming = New RelativeIntensityCutoff(0.01)

    Public ReadOnly Property raw As Raw

    Sub New(file As String, progress As Action(Of String), finished As Action)
        source = file
        temp1 = App.AppSystemTemp & "/" & file.GetFullPath.MD5 & "_1.cdf"
        temp2 = App.AppSystemTemp & "/" & file.GetFullPath.MD5 & "_2.cdf"
        scatterTemp = App.AppSystemTemp & "/" & file.GetFullPath.MD5 & ".scatter"
        showProgress = progress
        success = finished
        raw = New Raw With {
            .ms1_cache = temp1.GetFullPath,
            .ms2_cache = temp2.GetFullPath,
            .source = source.GetFullPath,
            .scatter = scatterTemp.GetFullPath
        }
    End Sub

    Public Sub RunImports()
        If source.ExtensionSuffix("mzxml") Then
            Call importsMzXML()
        ElseIf source.ExtensionSuffix("mzml") Then
            Call importsMzML()
        Else
            Call importsMgf()
        End If

        Call showProgress("Create snapshot...")
        Call raw.DrawScatter.SaveAs(raw.scatter, autoDispose:=True)

        Call showProgress("Job Done!")
        Call Thread.Sleep(1500)
        Call success()
    End Sub

    Private Sub importsMgf()
        Using cache1 As New CDFWriter(temp1, Encodings.UTF8WithoutBOM), cache2 As New CDFWriter(temp2, Encodings.UTF8WithoutBOM)
            Dim attrs As attribute()
            Dim data As Double()
            Dim name As String
            Dim nscans As New List(Of Ms1ScanEntry)
            Dim ms1Parent As Ms1ScanEntry = Nothing
            Dim ms2Temp As New List(Of ScanEntry)
            Dim rt As New List(Of Double)

            For Each scan As MGF.Ions In MGF.ReadIons(source)
                Dim msLevel As String = scan.Meta.TryGetValue("msLevel", [default]:=If(scan.PepMass.name = "0", "1", "2"))

                attrs = {
                    New attribute With {.name = NameOf(mzXML.scan.msLevel), .type = CDFDataTypes.INT, .value = msLevel},
                    New attribute With {.name = NameOf(mzXML.scan.collisionEnergy), .type = CDFDataTypes.CHAR, .value = scan.Meta.TryGetValue("collisionEnergy", [default]:="n/a")},
                    New attribute With {.name = NameOf(mzXML.scan.centroided), .type = CDFDataTypes.CHAR, .value = scan.Meta.TryGetValue("centroided", [default]:="n/a")},
                    New attribute With {.name = NameOf(mzXML.scan.precursorMz), .type = CDFDataTypes.DOUBLE, .value = scan.PepMass.name},
                    New attribute With {.name = NameOf(mzXML.scan.retentionTime), .type = CDFDataTypes.DOUBLE, .value = scan.RtInSeconds},
                    New attribute With {.name = NameOf(mzXML.scan.polarity), .type = CDFDataTypes.CHAR, .value = scan.Meta.TryGetValue("polarity", "1")},
                    New attribute With {.name = NameOf(mzXML.precursorMz.activationMethod), .type = CDFDataTypes.CHAR, .value = scan.Meta.TryGetValue("precursorMz", [default]:="n/a")},
                    New attribute With {.name = NameOf(mzXML.precursorMz.precursorCharge), .type = CDFDataTypes.DOUBLE, .value = scan.Charge}
                }
                data = compress(scan.Peaks)
                name = scan.Title & $" scan={nscans.Count + 1}"

                Dim scanData As New CDFData With {.numerics = data}
                Dim scanSize As New Dimension With {
                    .name = "m/z-int,scan_" & scan.Meta.TryGetValue("scan", nscans.Count),
                    .size = data.Length
                }

                If msLevel = "1" OrElse scan.PepMass.name = "0" Then
                    cache1.AddVariable(name, scanData, scanSize, attrs)

                    If Not ms1Parent Is Nothing Then
                        ms1Parent.products = ms2Temp.PopAll
                        nscans.Add(ms1Parent)
                    End If

                    ms1Parent = New Ms1ScanEntry With {
                       .id = name,
                       .rt = scan.RtInSeconds,
                       .BPC = scan.PepMass.text,
                       .TIC = scan.PepMass.text
                    }
                Else
                    cache2.AddVariable(name, scanData, scanSize, attrs)

                    Call New ScanEntry With {
                       .id = name,
                       .mz = scan.PepMass.name,
                       .rt = scan.RtInSeconds,
                       .BPC = scan.PepMass.text,
                       .TIC = scan.PepMass.text,
                       .XIC = scan.PepMass.text,
                       .polarity = scan.Meta.TryGetValue("polarity", "1"),
                       .charge = scan.Charge
                   }.DoCall(AddressOf ms2Temp.Add)
                End If

                rt.Add(scan.RtInSeconds)

                Call showProgress(name)
            Next

            If nscans.Last.id <> ms1Parent.id Then
                nscans.Add(ms1Parent)
            End If

            cache1.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Count})
            cache2.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Sum(Function(a) a.products.TryCount)})

            raw.scans = nscans.ToArray
            raw.rtmin = rt.Min
            raw.rtmax = rt.Max

            Call showProgress("Write cache data...")
        End Using
    End Sub

    Private Sub importsMzXML()
        Using cache1 As New CDFWriter(temp1, Encodings.UTF8WithoutBOM), cache2 As New CDFWriter(temp2, Encodings.UTF8WithoutBOM)
            Dim attrs As attribute()
            Dim data As Double()
            Dim name As String
            Dim nscans As New List(Of Ms1ScanEntry)
            Dim ms1Parent As Ms1ScanEntry = Nothing
            Dim ms2Temp As New List(Of ScanEntry)
            Dim rt As New List(Of Double)
            Dim i As i32 = 1

            For Each scan As mzXML.scan In mzXML.XML.LoadScans(source)
                If scan.peaks.compressedLen = 0 OrElse DirectCast(scan.peaks, IBase64Container).BinaryArray.StringEmpty Then
                    Continue For
                End If

                attrs = {
                    New attribute With {.name = NameOf(scan.msLevel), .type = CDFDataTypes.INT, .value = scan.msLevel},
                    New attribute With {.name = NameOf(scan.collisionEnergy), .type = CDFDataTypes.CHAR, .value = scan.collisionEnergy Or "n/a".AsDefault},
                    New attribute With {.name = NameOf(scan.centroided), .type = CDFDataTypes.CHAR, .value = scan.centroided Or "n/a".AsDefault},
                    New attribute With {.name = NameOf(scan.precursorMz), .type = CDFDataTypes.DOUBLE, .value = scan.precursorMz.value},
                    New attribute With {.name = NameOf(scan.retentionTime), .type = CDFDataTypes.DOUBLE, .value = PeakMs2.RtInSecond(scan.retentionTime)},
                    New attribute With {.name = NameOf(scan.polarity), .type = CDFDataTypes.CHAR, .value = scan.polarity},
                    New attribute With {.name = NameOf(scan.precursorMz.activationMethod), .type = CDFDataTypes.CHAR, .value = scan.precursorMz.activationMethod Or "n/a".AsDefault},
                    New attribute With {.name = NameOf(scan.precursorMz.precursorCharge), .type = CDFDataTypes.DOUBLE, .value = scan.precursorMz.precursorCharge}
                }
                data = compress(scan.peaks.Base64Decode(True)).ToArray

                If scan.msLevel = 1 Then
                    name = scan.getName & $" scan={nscans.Count + 1}"
                Else
                    name = scan.getName & $" index={++i}"
                End If

                Dim scanData As New CDFData With {.numerics = data}
                Dim scanSize As New Dimension With {
                    .name = "m/z-int,scan_" & scan.num,
                    .size = data.Length
                }

                If scan.msLevel = 1 Then
                    cache1.AddVariable(name, scanData, scanSize, attrs)

                    If Not ms1Parent Is Nothing Then
                        ms1Parent.products = ms2Temp.PopAll
                        nscans.Add(ms1Parent)
                    End If

                    ms1Parent = New Ms1ScanEntry With {
                       .id = name,
                       .rt = PeakMs2.RtInSecond(scan.retentionTime),
                       .BPC = scan.basePeakIntensity,
                       .TIC = scan.totIonCurrent
                    }
                Else
                    cache2.AddVariable(name, scanData, scanSize, attrs)

                    Call New ScanEntry With {
                       .id = name,
                       .mz = scan.precursorMz.value,
                       .rt = PeakMs2.RtInSecond(scan.retentionTime),
                       .BPC = scan.basePeakIntensity,
                       .TIC = scan.totIonCurrent,
                       .XIC = scan.precursorMz.precursorIntensity,
                       .polarity = Provider.ParseIonMode(scan.polarity),
                       .charge = scan.precursorMz.precursorCharge
                   }.DoCall(AddressOf ms2Temp.Add)
                End If

                rt.Add(PeakMs2.RtInSecond(scan.retentionTime))

                Call showProgress(name)
            Next

            If nscans.Last.id <> ms1Parent.id Then
                nscans.Add(ms1Parent)
            End If

            cache1.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Count})
            cache2.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Sum(Function(a) a.products.TryCount)})

            raw.scans = nscans.ToArray
            raw.rtmin = rt.Min
            raw.rtmax = rt.Max

            Call showProgress("Write cache data...")
        End Using
    End Sub

    Private Function compress(raw As IEnumerable(Of Double)) As IEnumerable(Of Double)
        Return compress(raw.AsMs2.ToArray)
    End Function

    Private Iterator Function compress(raw As ms2()) As IEnumerable(Of Double)
        For Each mz In raw.Centroid(tolerance, intoCutoff)
            Yield mz.intensity
            Yield mz.mz
        Next
    End Function

    Private Sub importsMzML()
        Using cache1 As New CDFWriter(temp1, Encodings.UTF8WithoutBOM), cache2 As New CDFWriter(temp2, Encodings.UTF8WithoutBOM)
            Dim attrs As attribute()
            Dim data As New List(Of Double)
            Dim name As String
            Dim nscans As New List(Of Ms1ScanEntry)
            Dim rt As New List(Of Double)
            Dim ms1Parent As Ms1ScanEntry = Nothing
            Dim ms2Temp As New List(Of ScanEntry)
            Dim UVscans As New List(Of GeneralSignal)
            Dim j As i32 = 1
            Dim UVdetecor As String = GetPhotodiodeArrayDetectorInstrumentConfigurationId(source)

            For Each scan As spectrum In mzML.Xml.LoadScans(source)
                Dim parent As (mz As Double, into As Double) = Nothing

                If Not scan.cvParams.KeyItem(UVScanType) Is Nothing Then
                    If Not ms1Parent Is Nothing Then
                        ms1Parent.products = ms2Temp.PopAll
                        nscans.Add(ms1Parent)
                        ms1Parent = Nothing
                    End If

                    Call showProgress($"load electromagnetic radiation spectrum at {scan.scan_time}...")
                    Call UVscans.Add(scan.CreateGeneralSignal(UVdetecor))

                    Continue For
                End If

                If scan.ms_level > 1 Then
                    parent = scan.selectedIon
                End If

                Dim polarity As String

                If Not scan.cvParams.KeyItem("positive scan") Is Nothing Then
                    polarity = "+"
                Else
                    polarity = "-"
                End If

                data.Clear()
                ' 在这里的attribute name需要与mzXML的名称保持一致
                attrs = {
                    New attribute With {.name = NameOf(mzXML.scan.msLevel), .type = CDFDataTypes.INT, .value = scan.ms_level},
                    New attribute With {.name = NameOf(mzXML.scan.collisionEnergy), .type = CDFDataTypes.CHAR, .value = If(scan.ms_level = 1, "n/a", scan.precursorList.precursor(Scan0).GetCollisionEnergy)},
                    New attribute With {.name = NameOf(mzXML.scan.centroided), .type = CDFDataTypes.CHAR, .value = Not scan.profile},
                    New attribute With {.name = NameOf(mzXML.scan.precursorMz), .type = CDFDataTypes.DOUBLE, .value = parent.mz},
                    New attribute With {.name = NameOf(mzXML.scan.retentionTime), .type = CDFDataTypes.DOUBLE, .value = scan.scan_time},
                    New attribute With {.name = NameOf(mzXML.scan.polarity), .type = CDFDataTypes.CHAR, .value = polarity}
                }

                Dim mz = scan.ByteArray("m/z array").Base64Decode
                Dim intensity = scan.ByteArray("intensity array").Base64Decode

                For i As Integer = 0 To mz.Length - 1
                    data.Add(intensity(i))
                    data.Add(mz(i))
                Next

                Dim scanType As String = scan.scanList.scans(0).cvParams.KeyItem("filter string")?.value

                If scan.ms_level = 1 Then
                    name = $"[MS1] {scanType}_{nscans.Count + 1}, ({polarity}) retentionTime={CInt(scan.scan_time)}"
                Else
                    name = $"[MS/MS] {scanType}_index={++j}, ({polarity}) M{CInt(parent.mz)}T{CInt(scan.scan_time)}"
                End If

                rt.Add(scan.scan_time)
                data = New List(Of Double)(compress(data))

                Dim scanData As New CDFData With {.numerics = data}
                Dim scanSize As New Dimension With {
                    .name = "m/z-int,scan_" & scan.index,
                    .size = data.Count
                }

                Dim BPC As Double = Val(scan.cvParams.KeyItem("base peak intensity")?.value)
                Dim TIC As Double = Val(scan.cvParams.KeyItem("total ion current")?.value)

                If scan.ms_level = 1 Then
                    cache1.AddVariable(name, scanData, scanSize, attrs)

                    If Not ms1Parent Is Nothing Then
                        ms1Parent.products = ms2Temp.PopAll
                        nscans.Add(ms1Parent)
                    End If

                    ms1Parent = New Ms1ScanEntry With {
                        .id = name,
                        .rt = scan.scan_time,
                        .BPC = BPC,
                        .TIC = TIC
                    }
                Else
                    cache2.AddVariable(name, scanData, scanSize, attrs)
                    ms2Temp.Add(New ScanEntry With {
                        .id = name,
                        .mz = parent.mz,
                        .rt = scan.scan_time,
                        .BPC = BPC,
                        .XIC = parent.into,
                        .TIC = TIC,
                        .polarity = Provider.ParseIonMode(polarity)
                    })
                End If

                Call showProgress(name)
            Next

            cache1.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Count})
            cache2.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Sum(Function(a) a.products.TryCount)})

            If Not ms1Parent Is Nothing AndAlso nscans.Last.id <> ms1Parent.id Then
                nscans.Add(ms1Parent)
            End If

            raw.scans = nscans.ToArray
            raw.rtmin = rt.Min
            raw.rtmax = rt.Max
            raw.UVscans = UVscans _
                .Select(Function(uv)
                            Return New UVScan With {
                                .intensity = uv.Strength,
                                .scan_time = Val(uv.meta("scan_time")),
                                .total_ion_current = Val(uv.meta("total_ion_current")),
                                .wavelength = uv.Measures
                            }
                        End Function) _
                .ToArray

            Call showProgress("Write cache data...")
        End Using
    End Sub
End Class
