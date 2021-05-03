Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports Microsoft.VisualBasic.Language

Public Module RawStream

    <Extension>
    Public Function LoadFromXRaw(raw As MSFileReader) As mzPack
        Dim pack As New mzPack
        Dim scanInfo As SingleScanInfo = Nothing
        Dim MS1 As ScanMS1
        Dim MS2 As New List(Of ScanMS2)

        Call raw.LoadFile()

        For Each scan As RawLabelData In raw.GetLabelData
            scanInfo = Nothing
            raw.GetScanInfo(scan.ScanNumber)

            If scanInfo.MSLevel = 1 Then
                MS1.products = MS2.PopAll
            Else
                MS2 += New ScanMS2 With {
                    .activationMethod = scanInfo.ActivationType,
                    .centroided = scanInfo.IsCentroided,
                    .charge = scanInfo.ChargeState,
                    .collisionEnergy = 0,
                    .intensity = scanInfo.TotalIonCurrent,
                    .parentMz = scanInfo.ParentIonMZ,
                    .scan_id = scanInfo.FilterText,
                    .rt = scanInfo.RetentionTime,
                    .polarity = scanInfo.IonMode
                }
            End If
        Next

        Return pack
    End Function
End Module
