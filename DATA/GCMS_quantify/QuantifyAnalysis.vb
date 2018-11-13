Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MIME.application.netCDF
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Math

''' <summary>
''' https://github.com/cheminfo-js/netcdf-gcms
''' </summary>
Public Module QuantifyAnalysis

    ''' <summary>
    ''' 读取CDF文件然后读取原始数据
    ''' </summary>
    ''' <param name="cdfPath"></param>
    ''' <returns></returns>
    Public Function ReadData(cdfPath$, Optional vendor$ = "agilentGCMS", Optional showSummary As Boolean = True) As GCMSJson
        Dim cdf As New netCDFReader(cdfPath)

        If showSummary Then
            Call Console.WriteLine(cdf.ToString)
        End If

        Select Case vendor
            Case "agilentGCMS" : Return agilentGCMS.Read(cdf)
            Case Else
                Throw New NotImplementedException(vendor)
        End Select
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ExportROI(gcms As GCMSJson) As ROI()
        Return gcms.GetTIC _
            .Shadows _
            .PopulateROI _
            .ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <param name="sn">
    ''' 信噪比筛选阈值
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 保留指数的计算：在标准化流程之中，GCMS的出峰顺序保持不变，但是保留时间可能会在不同批次实验间有变化
    ''' 这个时候如果定量用的标准品混合物和样本之中的所检测物质的出峰顺序一致，则可以将标准品混合物之中的
    ''' 第一个出峰的物质和最后一个出峰的物质作为保留指数的参考，在这里假设第一个出峰的物质的保留指数为零，
    ''' 最后一个出峰的物质的保留指数为1000，则可以根据这个区间和rt之间的线性关系计算出保留指数
    ''' </remarks>
    <Extension> Public Function ExportROITable(regions As ROI(), Optional sn# = 1.25) As ROITable()

    End Function
End Module
