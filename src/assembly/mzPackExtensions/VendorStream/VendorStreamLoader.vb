Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language

Public MustInherit Class VendorStreamLoader(Of T As IMsScanData)

    Protected MS1 As ScanMS1 = Nothing
    Protected MS2 As New List(Of ScanMS2)
    Protected MSscans As New List(Of ScanMS1)
    Protected scanIdFunc As Func(Of T, Integer, String)

    Public MustOverride ReadOnly Property rawFileName As String

    Protected Sub New(scanIdFunc As Func(Of T, Integer, String))
        Me.scanIdFunc = If(scanIdFunc, New Func(Of T, Integer, String)(AddressOf defaultScanId))
    End Sub

    Protected MustOverride Function defaultScanId(scaninfo As T, i As Integer) As String
    Protected MustOverride Function pullAllScans(skipEmptyScan As Boolean) As IEnumerable(Of T)
    Protected MustOverride Sub walkScan(scan As T)

    Public Function StreamTo(Optional skipEmptyScan As Boolean = True,
                             Optional println As Action(Of String) = Nothing) As mzPack

        Dim scan_times As New List(Of Double)
        Dim TIC As New List(Of Double)
        Dim BPC As New List(Of Double)

        For Each scaninfo As T In pullAllScans(skipEmptyScan)
            Call walkScan(scaninfo)

            If scaninfo.MSLevel = 1 Then
                scan_times += scaninfo.ScanTime * 60
                TIC += scaninfo.TotalIonCurrent
                BPC += scaninfo.BasePeakIntensity

                Call Application.DoEvents()

                If Not println Is Nothing Then
                    Call println($"Load " & scaninfo.ToString)
                End If
            End If
        Next

        If Not MS2.IsNullOrEmpty AndAlso Not MS1 Is Nothing Then
            MS1.products = MS2.PopAll
            MSscans += MS1
        End If

        If MSscans = 0 Then
            Call $"No MS scan data in raw data file: {rawFileName}".Warning
        End If

        Return New mzPack With {
            .MS = MSscans.PopAll,
            .Chromatogram = New Chromatogram With {
                .BPC = BPC.PopAll,
                .TIC = TIC.PopAll,
                .scan_time = scan_times.PopAll
            },
            .source = rawFileName.FileName
        }
    End Function
End Class
