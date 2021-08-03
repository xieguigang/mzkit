Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports stdNum = System.Math

Namespace MsImaging

    ''' <summary>
    ''' combine helper for row scans MSI raw data files
    ''' </summary>
    Public Class MSIMeasurement

        ReadOnly maxScans As Integer
        ReadOnly maxRt As Double
        ReadOnly hasMs2 As Boolean

        Sub New(totalTime As Double, pixels As Integer, Optional hasMs2 As Boolean = False)
            Me.maxScans = pixels
            Me.hasMs2 = hasMs2
            Me.maxRt = totalTime
        End Sub

        Public Function GetCorrection() As Correction
            If hasMs2 Then
                Return New ScanMs2Correction(maxRt, maxScans)
            Else
                Return New ScanTimeCorrection(maxRt, maxScans)
            End If
        End Function

        Public Shared Function Measure(raw As IEnumerable(Of mzPack)) As MSIMeasurement
            Dim scans As New List(Of Integer)
            Dim maxrt As New List(Of Double)
            Dim maxScan As Integer
            Dim scanMs2 As Boolean = False

            For Each file As mzPack In raw
                maxScan = file.MS.Length
                scans.Add(maxScan)
                maxrt.Add(file.MS.Select(Function(scan) scan.rt).Max)

                If Not scanMs2 Then
                    scanMs2 = file.hasMs2
                End If
            Next

            Return New MSIMeasurement(maxrt.Average, scans.Average, hasMs2:=scanMs2)
        End Function

        Public Shared Function Measure(raw As IEnumerable(Of MSFileReader), Optional scanMs2 As Boolean = False) As MSIMeasurement
            Dim scans As New List(Of Integer)
            Dim maxrt As New List(Of Double)
            Dim maxScan As Integer

            For Each file As MSFileReader In raw
                maxScan = file.ThermoReader.GetNumScans
                scans.Add(maxScan)
                maxrt.Add(file.ScanTimeMax * 60)

                If Not scanMs2 Then
                    For i As Integer = 0 To stdNum.Min(64, maxScan)
                        If file.ThermoReader.GetMSLevel(scan:=i) > 1 Then
                            scanMs2 = True
                            Exit For
                        End If
                    Next
                End If
            Next

            Return New MSIMeasurement(maxrt.Average, scans.Average, hasMs2:=scanMs2)
        End Function

    End Class
End Namespace