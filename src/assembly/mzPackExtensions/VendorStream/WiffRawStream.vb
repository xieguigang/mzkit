Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language

''' <summary>
''' wiff raw to mzpack convertor
''' </summary>
Public Class WiffRawStream : Inherits VendorStreamLoader(Of ScanInfo)

    ReadOnly raw As WiffScanFileReader

    Public Overrides ReadOnly Property rawFileName As String
        Get
            Return raw.wiffPath
        End Get
    End Property

    Public Sub New(raw As WiffScanFileReader, Optional scanIdFunc As Func(Of ScanInfo, Integer, String) = Nothing)
        MyBase.New(scanIdFunc)
        Me.raw = raw
    End Sub

    Protected Overrides Sub walkScan(scan As ScanInfo)
        Dim msData As PeakList = raw.GetCentroidFromScanNum(scan.ScanNumber)
        Dim mz As Double() = msData.mz
        Dim into As Double() = msData.into
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
            Dim clean As ms2() = mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = into(i)
                            }
                        End Function) _
                .AbSciexBaselineHandling _
                .ToArray

            mz = clean.Select(Function(i) i.mz).ToArray
            into = clean.Select(Function(i) i.intensity).ToArray

            If mz.Length = 0 Then
                Return
            End If

            MS2 += New ScanMS2 With {
                .activationMethod = ActivationMethods.CID,
                .centroided = True,
                .charge = scan.PrecursorCharge,
                .collisionEnergy = scan.CollisionEnergy,
                .intensity = scan.TotalIonCurrent,
                .parentMz = scan.PrecursorMz,
                .scan_id = scanId,
                .rt = scan.RetentionTime * 60,
                .polarity = scan.Polarity,
                .mz = mz,
                .into = into
            }
        End If
    End Sub

    Protected Overrides Function defaultScanId(scaninfo As ScanInfo, i As Integer) As String
        Return scaninfo.ToString
    End Function

    Protected Overrides Iterator Function pullAllScans(skipEmptyScan As Boolean) As IEnumerable(Of ScanInfo)
        Dim i As i32 = Scan0

        For Each name As String In raw.sampleNames
            Call raw.SetCurrentSample(++i)

            Dim n As Integer = raw.GetLastSpectrumNumber

            For scanId As Integer = 0 To n
                Yield raw.GetScan(scanId)
            Next
        Next
    End Function
End Class
