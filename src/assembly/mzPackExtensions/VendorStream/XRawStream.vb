#Region "Microsoft.VisualBasic::1b7ba1ac6fb7a7cb54491ccf6aa181ee, mzkit\src\assembly\mzPackExtensions\XRawStream.vb"

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

'   Total Lines: 128
'    Code Lines: 106
' Comment Lines: 0
'   Blank Lines: 22
'     File Size: 4.84 KB


' Class XRawStream
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: defaultId, StreamTo
' 
'     Sub: WalkScan
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports stdNum = System.Math

''' <summary>
''' thermo raw to mzpack convertor
''' </summary>
Public Class XRawStream : Inherits VendorStreamLoader(Of (scaninfo As SingleScanInfo, ms As RawLabelData))

    ReadOnly raw As MSFileReader

    Public Overrides ReadOnly Property rawFileName As String
        Get
            Return raw.FileName
        End Get
    End Property

    Sub New(raw As MSFileReader, Optional scanIdFunc As Func(Of SingleScanInfo, Integer, String) = Nothing)
        Call MyBase.New(wrapId(scanIdFunc))

        Me.raw = raw
        Me.raw.Options.MaxScan = raw.ScanMax
    End Sub

    Private Shared Function wrapId(scanIdFunc As Func(Of SingleScanInfo, Integer, String)) As Func(Of (scaninfo As SingleScanInfo, ms As RawLabelData), Integer, String)
        If scanIdFunc Is Nothing Then
            Return Nothing
        Else
            Return Function(i, j) scanIdFunc(i.scaninfo, j)
        End If
    End Function

    Protected Overrides Function defaultScanId(scan As (scaninfo As SingleScanInfo, ms As RawLabelData), i As Integer) As String
        Dim xcms_id As String
        Dim scaninfo As SingleScanInfo = scan.scaninfo
        Dim nT As Integer = stdNum.Round(scaninfo.RetentionTime * 60)

        If scaninfo.MSLevel = 1 Then
            xcms_id = ""
        Else
            If nT = 0 Then
                xcms_id = $" M{stdNum.Round(scaninfo.ParentIonMZ)}"
            Else
                xcms_id = $" M{stdNum.Round(scaninfo.ParentIonMZ)}T{nT}"
            End If
        End If

        Return $"{If(scaninfo.MSLevel = 1, "[MS1]", "[MSn]")}[Scan_{scaninfo.ScanNumber}] {scaninfo.FilterText}{xcms_id}"
    End Function

    Protected Overrides Iterator Function pullAllScans(skipEmptyScan As Boolean) As IEnumerable(Of (SingleScanInfo, RawLabelData))
        For Each scan As RawLabelData In raw.GetLabelData(skipEmptyScan)
            Yield (raw.GetScanInfo(scan.ScanNumber), scan)
        Next
    End Function

    Protected Overrides Sub walkScan(scan As (scaninfo As SingleScanInfo, ms As RawLabelData))
        Dim scanInfo As SingleScanInfo = scan.scaninfo
        Dim mz As Double() = scan.ms.MSData.Select(Function(a) a.Mass).ToArray
        Dim into As Double() = scan.ms.MSData.Select(Function(a) a.Intensity).ToArray
        Dim scanId As String = scanIdFunc(scan, MSscans.Count)

        If scanInfo.MSLevel = 1 Then
            If Not MS1 Is Nothing Then
                MS1.products = MS2.PopAll
                MSscans += MS1
            End If

            MS1 = New ScanMS1 With {
                .BPC = scanInfo.BasePeakIntensity,
                .into = into,
                .mz = mz,
                .rt = scanInfo.RetentionTime * 60,
                .scan_id = scanId,
                .TIC = scanInfo.TotalIonCurrent
            }
        Else
            MS2 += New ScanMS2 With {
                .activationMethod = scanInfo.ActivationType,
                .centroided = scanInfo.IsCentroided,
                .charge = scanInfo.ChargeState,
                .collisionEnergy = 0,
                .intensity = scanInfo.TotalIonCurrent,
                .parentMz = scanInfo.ParentIonMZ,
                .scan_id = scanId,
                .rt = scanInfo.RetentionTime * 60,
                .polarity = scanInfo.IonMode,
                .mz = mz,
                .into = into
            }
        End If
    End Sub

End Class
