Imports System.Data.Linq.Mapping
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Namespace GCMS

    ''' <summary>
    ''' 用于GCMS定量结果输出的色谱图ROI区域表格
    ''' </summary>
    Public Class ROITable : Implements IRetentionTime, IROI

        Public Property ID As String

        Public Property rtmin As Double Implements IROI.rtmin
        Public Property rtmax As Double Implements IROI.rtmax

        Public Property rt As Double Implements IRetentionTime.rt

        ''' <summary>
        ''' 以分钟为单位的保留时间
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="rt(minute)")>
        Public ReadOnly Property rtMinute As Double
            Get
                Return rt / 60
            End Get
        End Property

        ''' <summary>
        ''' 保留指数
        ''' </summary>
        ''' <returns></returns>
        Public Property ri As Double

        ''' <summary>
        ''' 这个区域的最大峰高度
        ''' </summary>
        ''' <returns></returns>
        Public Property maxInto As Double
        ''' <summary>
        ''' 所计算出来的基线的响应强度
        ''' </summary>
        ''' <returns></returns>
        Public Property baseline As Double
        ''' <summary>
        ''' 当前的这个ROI的峰面积积分值
        ''' </summary>
        ''' <returns></returns>
        Public Property integration As Double

        ''' <summary>
        ''' 信噪比
        ''' </summary>
        ''' <returns></returns>
        Public Property sn As Double
        Public Property mass_spectra As String

    End Class
End Namespace