Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Dumping

    Public Class MRMPeakTable

        ''' <summary>
        ''' 标志物物质编号
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String
        ''' <summary>
        ''' 通用名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 保留时间的下限
        ''' </summary>
        ''' <returns></returns>
        Public Property rtmin As Double
        ''' <summary>
        ''' 保留时间的上限
        ''' </summary>
        ''' <returns></returns>
        Public Property rtmax As Double
        ''' <summary>
        ''' 浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property content As Double
        ''' <summary>
        ''' 最大的信号响应值
        ''' </summary>
        ''' <returns></returns>
        Public Property maxinto As Double
        ''' <summary>
        ''' 内标的最大信号响应值
        ''' </summary>
        ''' <returns></returns>
        Public Property maxinto_IS As Double

        ''' <summary>
        ''' 峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA As Double
        ''' <summary>
        ''' 内标的峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property TPA_IS As Double
        ''' <summary>
        ''' 内标编号
        ''' </summary>
        ''' <returns></returns>
        <Column("IS")>
        Public Property [IS] As String
        ''' <summary>
        ''' 信号基线水平
        ''' </summary>
        ''' <returns></returns>
        Public Property base As Double
        ''' <summary>
        ''' 实验数据的原始文件名
        ''' </summary>
        ''' <returns></returns>
        Public Property raw As String

    End Class

    Public Class MRMStandards

        Public Property ID As String
        Public Property Name As String

        Public Property AIS As Double
        Public Property Ati As Double
        Public Property cIS As Double
        Public Property Cti As Double

        Public Property level As String
    End Class
End Namespace