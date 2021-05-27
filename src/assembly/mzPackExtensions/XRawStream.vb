Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
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
        Me.scanIdFunc = scanIdFunc Or defaultId()
    End Sub

    Private Shared Function defaultId() As [Default](Of Func(Of SingleScanInfo, Integer, String))
        Return New [Default](Of Func(Of SingleScanInfo, Integer, String))(
            Function(scaninfo As SingleScanInfo, i As Integer) As String
                Return $"{If(scaninfo.MSLevel = 1, "[MS1]", "[MSn]")}[Scan_{scaninfo.ScanNumber}] {scaninfo.FilterText}"
            End Function)
    End Function

    Public Function StreamTo(Optional skipEmptyScan As Boolean = True) As mzPack
        For Each scan As RawLabelData In raw.GetLabelData(skipEmptyScan)
            Call WalkScan(scan)
        Next

        MS1.products = MS2.PopAll
        MSscans += MS1

        Return New mzPack With {
            .MS = MSscans.PopAll
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
                .rt = scanInfo.RetentionTime,
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
                .rt = scanInfo.RetentionTime,
                .polarity = scanInfo.IonMode,
                .mz = mz,
                .into = into
            }
        End If
    End Sub

End Class
