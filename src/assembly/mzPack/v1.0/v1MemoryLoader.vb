#Region "Microsoft.VisualBasic::a49c5ebb173bbbc917fa9adb85109f94, G:/mzkit/src/assembly/mzPack//v1.0/v1MemoryLoader.vb"

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

    '   Total Lines: 88
    '    Code Lines: 67
    ' Comment Lines: 5
    '   Blank Lines: 16
    '     File Size: 3.31 KB


    ' Class v1MemoryLoader
    ' 
    '     Function: PopulateAllScans, ReadAll, Write
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' handling file format for mzPack version 1
''' </summary>
Public Class v1MemoryLoader

    ''' <summary>
    ''' 一次性加载所有原始数据
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function ReadAll(file As Stream,
                                   Optional ignoreThumbnail As Boolean = False,
                                   Optional skipMsn As Boolean = False,
                                   Optional verbose As Boolean = True) As mzPack

        Using mzpack As New mzPackReader(file)
            Dim allMSscans As ScanMS1() = PopulateAllScans(mzpack, skipMsn, verbose).ToArray
            Dim scanners As New Dictionary(Of String, ChromatogramOverlapList)
            Dim source As String = Nothing

            If TypeOf file Is FileStream Then
                source = DirectCast(file, FileStream).Name.FileName
            End If

            For Each id As String In mzpack.ChromatogramScanners
                Using buffer As Stream = mzpack.OpenScannerData(id)
                    scanners(id) = buffer.ReadPackData
                End Using
            Next

            Return New mzPack With {
                .Thumbnail = If(ignoreThumbnail, Nothing, mzpack.GetThumbnail),
                .MS = allMSscans,
                .Scanners = scanners,
                .Chromatogram = mzpack.chromatogram,
                .source = source
            }
        End Using
    End Function

    Private Shared Iterator Function PopulateAllScans(mzpack As mzPackReader, skipMsn As Boolean, verbose As Boolean) As IEnumerable(Of ScanMS1)
        Dim allIndex As String() = mzpack.EnumerateIndex.ToArray
        Dim i As i32 = 0
        Dim d As Integer = allIndex.Length / 10
        Dim j As Integer = 0

        For Each id As String In allIndex
            j += 1

            Yield mzpack.ReadScan(id, skipMsn)

            If ++i = d Then
                If verbose Then
                    RunSlavePipeline.SendProgress(std.Round(j / allIndex.Length, 2), id & $" ({(j / allIndex.Length * 100).ToString("F2")}%)")
                End If

                i = 0
            End If
        Next
    End Function

    ''' <summary>
    ''' write mzpack data file in version 1 format
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="file"></param>
    ''' <param name="progress"></param>
    ''' <returns></returns>
    Public Shared Function Write(data As mzPack, file As Stream, Optional progress As Action(Of String) = Nothing) As Boolean
        Using mzpack As New mzPackWriter(file)
            Dim d As Integer = data.MS.TryCount / 7
            Dim i As i32 = 0

            For Each scan As ScanMS1 In data.MS
                Call mzpack.Write(scan)

                If Not progress Is Nothing Then
                    If ++i Mod d = 0 Then
                        Call progress($"write: {(i / d * 100).ToString("F0")}%" & scan.scan_id)
                    End If
                End If
            Next

            For Each scanner In data.Scanners.SafeQuery
                Call mzpack.AddOtherScanner(scanner.Key, scanner.Value)
            Next

            Call mzpack.SetChromatogram(data.Chromatogram)
            Call mzpack.SetThumbnail(data.Thumbnail)
            Call file.Flush()
        End Using

        Return True
    End Function
End Class
