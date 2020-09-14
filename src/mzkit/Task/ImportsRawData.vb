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
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports mzML = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML

Public Class ImportsRawData

    ReadOnly source As String
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
        showProgress = progress
        success = finished
        raw = New Raw With {
            .ms1_cache = temp1.GetFullPath,
            .ms2_cache = temp2.GetFullPath,
            .source = source.GetFullPath
        }
    End Sub

    Public Sub RunImports()
        If source.ExtensionSuffix("mzxml") Then
            Call importsMzXML()
        Else
            Call importsMzML()
        End If

        Call showProgress("Job Done!")
        Call Thread.Sleep(1500)
        Call success()
    End Sub

    Private Sub importsMzXML()
        Using cache1 As New CDFWriter(temp1, Encodings.UTF8WithoutBOM), cache2 As New CDFWriter(temp2, Encodings.UTF8WithoutBOM)
            Dim attrs As attribute()
            Dim data As Double()
            Dim name As String
            Dim nscans As New List(Of ScanEntry)
            Dim rt As New List(Of Double)

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
                name = scan.getName & $" scan={nscans.Count + 1}"

                Dim scanData As New CDFData With {.numerics = data}
                Dim scanSize As New Dimension With {
                    .name = "m/z-int,scan_" & scan.num,
                    .size = data.Length
                }

                If scan.msLevel = 1 Then
                    cache1.AddVariable(name, scanData, scanSize, attrs)
                Else
                    cache2.AddVariable(name, scanData, scanSize, attrs)
                End If

                rt.Add(PeakMs2.RtInSecond(scan.retentionTime))

                Call New ScanEntry With {
                    .id = name,
                    .mz = scan.precursorMz.value,
                    .rt = PeakMs2.RtInSecond(scan.retentionTime),
                    .BPC = scan.basePeakIntensity,
                    .TIC = scan.totIonCurrent,
                    .XIC = scan.precursorMz.precursorIntensity,
                    .polarity = Provider.ParseIonMode(scan.polarity),
                    .charge = scan.precursorMz.precursorCharge
                }.DoCall(AddressOf nscans.Add)

                Call showProgress(name)
            Next

            cache1.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Where(Function(a) a.mz = 0.0).Count})
            cache2.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Where(Function(a) a.mz > 0.0).Count})

            raw.scans = nscans.ToArray
            raw.rtmin = rt.Min
            raw.rtmax = rt.Max

            Call showProgress("Write cache data...")
        End Using
    End Sub

    Private Iterator Function compress(raw As IEnumerable(Of Double)) As IEnumerable(Of Double)
        For Each mz In raw.AsMs2.ToArray.Centroid(tolerance, intoCutoff)
            Yield mz.intensity
            Yield mz.mz
        Next
    End Function

    Private Sub importsMzML()
        Using cache1 As New CDFWriter(temp1, Encodings.UTF8WithoutBOM), cache2 As New CDFWriter(temp2, Encodings.UTF8WithoutBOM)
            Dim attrs As attribute()
            Dim data As New List(Of Double)
            Dim name As String
            Dim nscans As New List(Of ScanEntry)
            Dim rt As New List(Of Double)

            For Each scan As spectrum In mzML.Xml.LoadScans(source)
                Dim parent As (mz As Double, into As Double) = Nothing

                If Not scan.cvParams.KeyItem("electromagnetic radiation spectrum") Is Nothing Then
                    Call showProgress($"skip electromagnetic radiation spectrum at {scan.scan_time}...")
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
                    name = $"[MS/MS] {scanType}_{nscans.Count + 1}, ({polarity}) M{CInt(parent.mz)}T{CInt(scan.scan_time)}"
                End If

                rt.Add(scan.scan_time)
                data = New List(Of Double)(compress(data))

                Dim scanData As New CDFData With {.numerics = data}
                Dim scanSize As New Dimension With {
                    .name = "m/z-int,scan_" & scan.index,
                    .size = data.Count
                }

                If scan.ms_level = 1 Then
                    cache1.AddVariable(name, scanData, scanSize, attrs)
                Else
                    cache2.AddVariable(name, scanData, scanSize, attrs)
                End If

                Call New ScanEntry With {
                    .id = name,
                    .mz = parent.mz,
                    .rt = scan.scan_time,
                    .BPC = parent.into,
                    .XIC = parent.into,
                    .TIC = parent.into,
                    .polarity = Provider.ParseIonMode(polarity)
                }.DoCall(AddressOf nscans.Add)

                Call showProgress(name)
            Next

            cache1.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Where(Function(a) a.mz = 0.0).Count})
            cache2.GlobalAttributes(New attribute With {.name = NameOf(nscans), .type = CDFDataTypes.INT, .value = nscans.Where(Function(a) a.mz > 0.0).Count})

            raw.scans = nscans.ToArray
            raw.rtmin = rt.Min
            raw.rtmax = rt.Max

            Call showProgress("Write cache data...")
        End Using
    End Sub
End Class
