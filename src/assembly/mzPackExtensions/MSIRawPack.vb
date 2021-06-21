Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports Microsoft.VisualBasic.Data.csv.IO
Imports stdNum = System.Math

Public Module MSIRawPack

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="pixels">
    ''' 扫描在[X,Y]上的像素点数量
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadFromXMSIRaw(raw As MSFileReader, pixels As Size) As mzPack
        Dim loader As New XRawStream(raw, PixelScanId(pixels))
        Dim pack As mzPack = loader.StreamTo(skipEmptyScan:=False)

        If pixels.Width * pixels.Height <> pack.MS.Length Then
            Call $"Data inconsistent: image pixels number ({pixels.Width * pixels.Height}) is not equals to scan numbers ({pack.MS.Length})!".Warning
        End If

        Return pack
    End Function

    Private Function PixelScanId(pixels As Size) As Func(Of SingleScanInfo, Integer, String)
        Return Function(scan, n)
                   If scan.MSLevel = 1 Then
                       Dim y As Integer = stdNum.Floor(n / pixels.Width) + 1
                       Dim x As Integer = n - (y - 1) * pixels.Width + 1

                       Return $"[MS1][Scan_{scan.ScanNumber}][{x},{y}] {scan.FilterText}"
                   Else
                       Return $"[MSn][Scan_{scan.ScanNumber}] {scan.FilterText}"
                   End If
               End Function
    End Function

    <Extension>
    Public Function ExactPixelTable(mzpack As mzPack) As DataSet()
        Dim mz As New Dictionary(Of String, DataSet)

        For Each scan As ScanMS1 In mzpack.MS
            Dim pixel As String = scan.scan_id.Match("\[\d+,\d+\]")

            For i As Integer = 0 To scan.mz.Length - 1
                Dim mzi As String = scan.mz(i).ToString("F4")

                If Not mz.ContainsKey(mzi) Then
                    mz.Add(mzi, New DataSet With {.ID = mzi})
                End If

                mz(mzi)(pixel) = stdNum.Max(mz(mzi)(pixel), scan.into(i))
            Next
        Next

        Return mz.Values.ToArray
    End Function
End Module
