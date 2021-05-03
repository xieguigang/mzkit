Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports Microsoft.VisualBasic.Language

Public Module RawStream

    <Extension>
    Public Function LoadFromXRaw(raw As MSFileReader) As mzPack
        Dim pack As New mzPack
        Dim MSscans As New List(Of ScanMS1)
        Dim scanInfo As SingleScanInfo = Nothing
        Dim MS1 As ScanMS1 = Nothing
        Dim MS2 As New List(Of ScanMS2)
        Dim mz As Double()
        Dim into As Double()
        Dim scanId As String

        For Each scan As RawLabelData In raw.GetLabelData
            scanInfo = raw.GetScanInfo(scan.ScanNumber)
            mz = scan.MSData.Select(Function(a) a.Mass).ToArray
            into = scan.MSData.Select(Function(a) a.Intensity).ToArray
            scanId = $"{If(scanInfo.MSLevel = 1, "[MS1]", "[MSn]")}[Scan_{scan.ScanNumber}] {scanInfo.FilterText}"

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
        Next

        MS1.products = MS2.PopAll
        MSscans += MS1

        pack.MS = MSscans

        Return pack
    End Function
End Module
