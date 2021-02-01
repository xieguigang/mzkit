Imports stdNum = System.Math

Namespace GCMS

    Public Class TimeScanMatrix

        ReadOnly mz_scanList As List(Of ms1_scan)()
        ReadOnly mzList As Double()

        Private Sub New(mzScans As ms1_scan()())
            mzList = mzScans.Select(Function(mzi) mzi(Scan0).mz).ToArray
            mz_scanList = mzScans.Select(Function(mz) mz.ToList).ToArray
        End Sub

        Public Iterator Function TimeScan(time As Double) As IEnumerable(Of ms1_scan)
            For i As Integer = 0 To mz_scanList.Length - 1
                Dim list = mz_scanList(i)
                Dim tick As ms1_scan = list _
                    .Where(Function(t) stdNum.Abs(t.scan_time - time) <= 0.5) _
                    .FirstOrDefault

                If tick Is Nothing Then
                    Yield New ms1_scan With {
                        .intensity = 0,
                        .mz = mzList(i),
                        .scan_time = time
                    }
                Else
                    list.Remove(tick)
                    Yield tick
                End If
            Next
        End Function

        Friend Shared Function CreateMatrixHelper(mzScans As ms1_scan()()) As TimeScanMatrix
            Return New TimeScanMatrix(mzScans)
        End Function
    End Class
End Namespace