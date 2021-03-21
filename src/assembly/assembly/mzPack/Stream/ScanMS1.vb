Imports BioNovoGene.Analytical.MassSpectrometry.Math

Namespace mzData.mzWebCache

    ''' <summary>
    ''' MS scan
    ''' </summary>
    Public Class ScanMS1 : Inherits MSScan

        Public Property TIC As Double
        Public Property BPC As Double
        Public Property products As ScanMS2()

        Public Iterator Function GetMs1Scans() As IEnumerable(Of ms1_scan)
            For i As Integer = 0 To mz.Length - 1
                Yield New ms1_scan With {
                    .mz = mz(i),
                    .intensity = into(i),
                    .scan_time = rt
                }
            Next
        End Function
    End Class
End Namespace