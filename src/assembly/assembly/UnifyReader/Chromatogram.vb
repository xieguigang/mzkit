Namespace DataReader

    Public Class Chromatogram

        Public Property scan_time As Double()
        Public Property TIC As Double()
        Public Property BPC As Double()

        Public Overrides Function ToString() As String
            Return $"Chromatogram between scan_time [{CInt(scan_time.Min)},{CInt(scan_time.Max)}]"
        End Function

        Public Shared Function GetChromatogram(Of Scan)(scans As IEnumerable(Of Scan)) As Chromatogram
            Dim scan_time As New List(Of Double)
            Dim tic As New List(Of Double)
            Dim bpc As New List(Of Double)
            Dim reader As MsDataReader(Of Scan) = MsDataReader(Of Scan).ScanProvider

            For Each scanVal As Scan In scans.Where(Function(s) reader.GetMsLevel(s) = 1)
                Call scan_time.Add(reader.GetScanTime(scanVal))
                Call tic.Add(reader.GetTIC(scanVal))
                Call bpc.Add(reader.GetBPC(scanVal))
            Next

            Return New Chromatogram With {
                .BPC = bpc.ToArray,
                .scan_time = scan_time.ToArray,
                .TIC = tic.ToArray
            }
        End Function
    End Class
End Namespace