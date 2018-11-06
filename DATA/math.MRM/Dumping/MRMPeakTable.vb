Imports System.Runtime.CompilerServices
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

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class

    ''' <summary>
    ''' 表示标准曲线上面的一个实验数据点
    ''' </summary>
    Public Class MRMStandards

        Public Property ID As String
        Public Property Name As String

        ''' <summary>
        ''' 内标峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property AIS As Double
        ''' <summary>
        ''' 当前试验点的标准品峰面积
        ''' </summary>
        ''' <returns></returns>
        Public Property Ati As Double
        ''' <summary>
        ''' 内标浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property cIS As Double
        ''' <summary>
        ''' 当前试验点的标准品浓度
        ''' </summary>
        ''' <returns></returns>
        Public Property Cti As Double

        ''' <summary>
        ''' 浓度梯度水平的名称，例如：``L1, L2, L3, ...``
        ''' </summary>
        ''' <returns></returns>
        Public Property level As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"Dim {Name} As {ID} = {Cti}"
        End Function
    End Class
End Namespace