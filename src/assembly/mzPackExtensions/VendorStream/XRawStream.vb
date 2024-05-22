#Region "Microsoft.VisualBasic::15a72e37c6fadeb5767d6e91c15aa53c, assembly\mzPackExtensions\VendorStream\XRawStream.vb"

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

    '   Total Lines: 98
    '    Code Lines: 80 (81.63%)
    ' Comment Lines: 3 (3.06%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (15.31%)
    '     File Size: 3.50 KB


    ' Class XRawStream
    ' 
    '     Properties: getExperimentType, rawFileName
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: defaultScanId, pullAllScans
    ' 
    '     Sub: walkScan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If NET48 Then

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports stdNum = System.Math

''' <summary>
''' thermo raw to mzpack convertor
''' </summary>
Public Class XRawStream : Inherits VendorStreamLoader(Of SingleScanInfo)

    ReadOnly raw As MSFileReader

    Public Overrides ReadOnly Property rawFileName As String
        Get
            Return raw.FileName
        End Get
    End Property

    Public Overrides ReadOnly Property getExperimentType As FileApplicationClass
        Get
            Return FileApplicationClass.LCMS
        End Get
    End Property

    Sub New(raw As MSFileReader, Optional scanIdFunc As Func(Of SingleScanInfo, Integer, String) = Nothing)
        Call MyBase.New(scanIdFunc)

        Me.raw = raw
        Me.raw.Options.MaxScan = raw.ScanMax
    End Sub

    Protected Overrides Function defaultScanId(scaninfo As SingleScanInfo, i As Integer) As String
        Dim xcms_id As String
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

    Protected Overrides Iterator Function pullAllScans(skipEmptyScan As Boolean) As IEnumerable(Of SingleScanInfo)
        For Each scan As RawLabelData In raw.GetLabelData(skipEmptyScan)
            Dim info As SingleScanInfo = raw.GetScanInfo(scan.ScanNumber)
            info.MSData = scan
            Yield info
        Next
    End Function

    Protected Overrides Sub walkScan(scan As SingleScanInfo)
        Dim mz As Double() = scan.MSData.Select(Function(a, i) a.Mass).ToArray
        Dim into As Double() = scan.MSData.Select(Function(a, i) a.Intensity).ToArray
        Dim scanId As String = scanIdFunc(scan, MSscans.Count)

        If scan.MSLevel = 1 Then
            If Not MS1 Is Nothing Then
                MS1.products = MS2.PopAll
                MSscans += MS1
            End If

            MS1 = New ScanMS1 With {
                .BPC = scan.BasePeakIntensity,
                .into = into,
                .mz = mz,
                .rt = scan.RetentionTime * 60,
                .scan_id = scanId,
                .TIC = scan.TotalIonCurrent
            }
        Else
            MS2 += New ScanMS2 With {
                .activationMethod = scan.ActivationType,
                .centroided = scan.IsCentroided,
                .charge = scan.ChargeState,
                .collisionEnergy = 0,
                .intensity = scan.TotalIonCurrent,
                .parentMz = scan.ParentIonMZ,
                .scan_id = scanId,
                .rt = scan.RetentionTime * 60,
                .polarity = scan.IonMode,
                .mz = mz,
                .into = into
            }
        End If
    End Sub

End Class
#End If
