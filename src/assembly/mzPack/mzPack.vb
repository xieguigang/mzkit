Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' mzPack文件格式模型
''' </summary>
Public Class mzPack

    ''' <summary>
    ''' 一般为二维散点图
    ''' </summary>
    ''' <returns></returns>
    Public Property Thumbnail As Image
    Public Property MS As ScanMS1()

    ''' <summary>
    ''' 其他的扫描器数据，例如紫外扫描
    ''' </summary>
    ''' <returns></returns>
    Public Property Scanners As Dictionary(Of String, ChromatogramOverlap)

    Public Function GetAllScanMs1() As IEnumerable(Of ms1_scan)
        Return MS _
            .Select(Function(scan)
                        Return scan.mz _
                            .Select(Function(mzi, i)
                                        Return New ms1_scan With {
                                            .mz = mzi,
                                            .intensity = scan.into(i),
                                            .scan_time = scan.rt
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL
    End Function

    ''' <summary>
    ''' 一次性加载所有原始数据
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function ReadAll(file As Stream) As mzPack
        Using mzpack As New mzPackReader(file)
            Dim allMSscans As ScanMS1() = mzpack _
                .EnumerateIndex _
                .Select(AddressOf mzpack.ReadScan) _
                .ToArray
            Dim scanners As New Dictionary(Of String, ChromatogramOverlap)

            For Each id As String In mzpack.ChromatogramScanners
                Using buffer As Stream = mzpack.OpenScannerData(id)
                    scanners(id) = buffer.ReadPackData
                End Using
            Next

            Return New mzPack With {
                .Thumbnail = mzpack.GetThumbnail,
                .MS = allMSscans,
                .Scanners = scanners
            }
        End Using
    End Function

    Public Function Write(file As Stream) As Boolean
        Using mzpack As New mzPackWriter(file)
            For Each scan As ScanMS1 In MS
                Call mzpack.Write(scan)
            Next

            For Each scanner In Scanners.SafeQuery
                Call mzpack.AddOtherScanner(scanner.Key, scanner.Value)
            Next

            Call mzpack.SetThumbnail(Thumbnail)
        End Using

        Return True
    End Function
End Class
