Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
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

    ''' <summary>
    ''' 一次性加载所有原始数据
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function ReadAll(file As Stream) As mzPack

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
