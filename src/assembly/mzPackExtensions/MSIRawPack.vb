#Region "Microsoft.VisualBasic::d9ba01bebe1895ce89fdfdcc6b059472, mzkit\src\assembly\mzPackExtensions\MSIRawPack.vb"

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


' Code Statistics:

'   Total Lines: 69
'    Code Lines: 45
' Comment Lines: 11
'   Blank Lines: 13
'     File Size: 2.67 KB


' Module MSIRawPack
' 
'     Function: ExactPixelTable, LoadFromXMSIRaw, PixelScanId
' 
' /********************************************************************************/

#End Region

#If netcore5 = 0 Or NET48 Then
Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
#End If

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader.SCiLSLab
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports stdNum = System.Math

''' <summary>
''' read SCiLSLab table export or Xcalibur Raw data file for MS-imaging
''' </summary>
Public Module MSIRawPack

#If netcore5 = 0 Or NET48 Then

    ''' <summary>
    ''' single raw data file as MSI data
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
        Dim cal As New ScanTimeCorrection(totalTime:=maxrt, pixels:=pixels.Width)

        Return Function(scan, n)
                   If scan.MSLevel = 1 Then
                       Dim pt As Point = cal.GetPixelPoint(scan.RetentionTime)

                       Return $"[MS1][Scan_{scan.ScanNumber}][{pt.X},{pt.Y}] {scan.FilterText}"
                   Else
                       Return $"[MSn][Scan_{scan.ScanNumber}] {scan.FilterText}"
                   End If
               End Function
    End Function

#End If

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

    Public Function LoadMSIFromSCiLSLab(spots As Stream, msdata As Stream) As mzPack
        Dim spotsXy As SpotPack = SpotPack.ParseFile(spots)
        Dim spotsMs As MsPack = MsPack.ParseFile(msdata)
        Dim spotsList As New List(Of ScanMS1)
        Dim i As i32 = Scan0

        For Each spot As SpotMs In spotsMs.matrix
            Dim ref As String = (Integer.Parse(spot.spot_id.Match("\d+")) - 1).ToString
            Dim xy As SpotSite = spotsXy.index(ref)
            Dim ms1 As New ScanMS1 With {
                .BPC = spot.intensity.Max,
                .TIC = spot.intensity.Sum,
                .into = spot.intensity,
                .meta = New Dictionary(Of String, String) From {
                    {"x", xy.x},
                    {"y", xy.y},
                    {"spot_id", xy.index}
                },
                .mz = spotsMs.mz,
                .rt = ++i,
                .scan_id = $"[MS1][{CInt(xy.x)},{CInt(xy.y)}] {spot.spot_id} totalIon:{ .TIC}"
            }

            spotsList.Add(ms1)
        Next

        Return New mzPack With {
            .Application = FileApplicationClass.MSImaging,
            .MS = spotsList.ToArray
        }
    End Function
End Module
