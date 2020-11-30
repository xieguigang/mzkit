
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace mzData.mzWebCache

    Public MustInherit Class ScanPopulator(Of Scan)

        Protected ms1 As ScanMS1
        Protected products As New List(Of ScanMS2)
        Protected trim As LowAbundanceTrimming = New RelativeIntensityCutoff(0.03)

        Protected MustOverride Function loadScans(rawfile As String) As IEnumerable(Of Scan)

        Protected MustOverride Function isMs1(scan As Scan) As Boolean
        Protected MustOverride Function isValid(scan As Scan) As Boolean
        Protected MustOverride Function getScanTime(scan As Scan) As Double
        Protected MustOverride Function getScanId(scan As Scan) As String

        Protected MustOverride Sub readScan(scan As Scan)

        Public Iterator Function Load(scans As IEnumerable(Of Scan)) As IEnumerable(Of ScanMS1)
            For Each scan As Scan In scans.Where(AddressOf isValid)
                Dim scan_time As Double = getScanTime(scan)
                Dim scan_id As String = getScanId(scan)

                If isMs1(scan) Then
                    If Not ms1 Is Nothing Then
                        ms1.products = products.ToArray
                        products.Clear()

                        Yield ms1
                    End If

                    ms1 = New ScanMS1 With {
                        .BPC = scan.basePeakIntensity,
                        .TIC = scan.totIonCurrent,
                        .rt = scan_time,
                        .scan_id = scan_id,
                        .mz = msms.Select(Function(a) a.mz).ToArray,
                        .into = msms.Select(Function(a) a.intensity).ToArray
                    }
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(rawfile As String) As IEnumerable(Of ScanMS1)
            Return Load(loadScans(rawfile))
        End Function

    End Class
End Namespace