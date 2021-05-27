Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
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
        Dim pack As mzPack = loader.StreamTo

        If pixels.Width * pixels.Height <> pack.MS.Length Then
            Call $"Data inconsistent: image pixels number ({pixels.Width * pixels.Height}) is not equals to scan numbers ({pack.MS.Length})!".Warning
        End If

        Return pack
    End Function

    Private Function PixelScanId(pixels As Size) As Func(Of SingleScanInfo, Integer, String)
        Return Function(scan, n)
                   If scan.MSLevel = 1 Then
                       Dim y As Integer = stdNum.Floor(n / pixels.Width)
                       Dim x As Integer = n - y * pixels.Width

                       Return $"[MS1][Scan_{scan.ScanNumber}][x:{x}, y:{y}] {scan.FilterText}"
                   Else
                       Return $"[MSn][Scan_{scan.ScanNumber}] {scan.FilterText}"
                   End If
               End Function
    End Function
End Module
