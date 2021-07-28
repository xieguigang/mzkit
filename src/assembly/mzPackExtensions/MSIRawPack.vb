#Region "Microsoft.VisualBasic::d9fd14bd8f65ab247dd7240fa8595ba3, src\assembly\mzPackExtensions\MSIRawPack.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module MSIRawPack
    ' 
    '     Function: ExactPixelTable, LoadFromXMSIRaw, PixelScanId
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
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
        Dim loader As New XRawStream(raw, PixelScanId(pixels, maxrt:=raw.ScanTimeMax))
        Dim pack As mzPack = loader.StreamTo(skipEmptyScan:=False)

        If pixels.Width * pixels.Height <> pack.MS.Length Then
            Call $"Data inconsistent: image pixels number ({pixels.Width * pixels.Height}) is not equals to scan numbers ({pack.MS.Length})!".Warning
        End If

        Return pack
    End Function

    Private Function PixelScanId(pixels As Size, maxrt As Double) As Func(Of SingleScanInfo, Integer, String)
        Dim cal As New Correction(totalTime:=maxrt, pixels:=pixels.Width)

        Return Function(scan, n)
                   If scan.MSLevel = 1 Then
                       Dim pt As Point = cal.GetPixelPoint(scan.RetentionTime)

                       Return $"[MS1][Scan_{scan.ScanNumber}][{pt.X},{pt.Y}] {scan.FilterText}"
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
