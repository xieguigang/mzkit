#Region "Microsoft.VisualBasic::e8ccd6b9f77e1e73d43db4b7f7fe2cc8, src\assembly\mzPackExtensions\XRawStream.vb"

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

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default

Public Class XRawStream

    ReadOnly raw As MSFileReader

    Dim MS1 As ScanMS1 = Nothing
    Dim MS2 As New List(Of ScanMS2)
    Dim MSscans As New List(Of ScanMS1)

    Friend scanIdFunc As Func(Of SingleScanInfo, Integer, String)

    Sub New(raw As MSFileReader, Optional scanIdFunc As Func(Of SingleScanInfo, Integer, String) = Nothing)
        Me.raw = raw
        Me.raw.Options.MaxScan = raw.ScanMax
        Me.scanIdFunc = scanIdFunc Or defaultId()
    End Sub

    Private Shared Function defaultId() As [Default](Of Func(Of SingleScanInfo, Integer, String))
        Return New [Default](Of Func(Of SingleScanInfo, Integer, String))(
            Function(scaninfo As SingleScanInfo, i As Integer) As String
                Return $"{If(scaninfo.MSLevel = 1, "[MS1]", "[MSn]")}[Scan_{scaninfo.ScanNumber}] {scaninfo.FilterText}"
            End Function)
    End Function

    Public Function StreamTo(Optional skipEmptyScan As Boolean = True) As mzPack
        Dim scan_times As New List(Of Double)
        Dim TIC As New List(Of Double)
        Dim BPC As New List(Of Double)

        For Each scan As RawLabelData In raw.GetLabelData(skipEmptyScan)
            Call WalkScan(scan)

            If scan.MsLevel = 1 Then
                Dim scaninfo As SingleScanInfo = raw.GetScanInfo(scan.ScanNumber)

                scan_times += scan.ScanTime * 60
                TIC += scaninfo.TotalIonCurrent
                BPC += scaninfo.BasePeakIntensity

                Call Application.DoEvents()
            End If
        Next

        MS1.products = MS2.PopAll
        MSscans += MS1

        Return New mzPack With {
            .MS = MSscans.PopAll,
            .Chromatogram = New Chromatogram With {
                .BPC = BPC.PopAll,
                .TIC = TIC.PopAll,
                .scan_time = scan_times.PopAll
            }
        }
    End Function

    Private Sub WalkScan(scan As RawLabelData)
        Dim scanInfo As SingleScanInfo = raw.GetScanInfo(scan.ScanNumber)
        Dim mz As Double() = scan.MSData.Select(Function(a) a.Mass).ToArray
        Dim into As Double() = scan.MSData.Select(Function(a) a.Intensity).ToArray
        Dim scanId As String = scanIdFunc(scanInfo, MSscans.Count)

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

