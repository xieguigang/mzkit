Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports stdNum = System.Math

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
            Dim scanners As New Dictionary(Of String, ChromatogramOverlap)
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
                    RunSlavePipeline.SendProgress(stdNum.Round(j / allIndex.Length, 2), id & $" ({(j / allIndex.Length * 100).ToString("F2")}%)")
                End If

                i = 0
            End If
        Next
    End Function

    Public Shared Function Write(data As mzPack, file As Stream, Optional progress As Action(Of String) = Nothing) As Boolean
        Using mzpack As New mzPackWriter(file)
            For Each scan As ScanMS1 In data.MS
                Call mzpack.Write(scan)

                If Not progress Is Nothing Then
                    Call progress("write: " & scan.scan_id)
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
