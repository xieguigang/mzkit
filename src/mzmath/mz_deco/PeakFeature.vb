
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports stdNum = System.Math

Public Class PeakFeature
    Implements IRetentionTime
    Implements IROI

    Public Property xcms_id As String

    Public Property mz As Double

    ''' <summary>
    ''' 出峰达到峰高最大值<see cref="MaxInto"/>的时间点
    ''' </summary>
    ''' <returns></returns>
    Public Property rt As Double Implements IRetentionTime.rt
    ''' <summary>
    ''' 这个区域的最大峰高度
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxInto As Double

    ''' <summary>
    ''' 所计算出来的基线的响应强度
    ''' </summary>
    ''' <returns></returns>
    Public Property Baseline As Double
    ''' <summary>
    ''' 当前的这个ROI的峰面积积分值
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 为当前的ROI峰面积占整个TIC峰面积的百分比，一个实验所导出来的所有的ROI的
    ''' 积分值加起来应该是约等于100的
    ''' </remarks>
    Public Property Integration As Double
    ''' <summary>
    ''' 噪声的面积积分百分比
    ''' </summary>
    ''' <returns></returns>
    Public Property Noise As Double

    Public Property rtmin As Double Implements IROI.rtmin
    Public Property rtmax As Double Implements IROI.rtmax

    ''' <summary>
    ''' 信噪比
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property snRatio As Double
        Get
            Return stdNum.Log(Integration / Noise)
        End Get
    End Property

End Class

Public Class MzGroup

    Public Property mz As Double
    Public Property XIC As ChromatogramTick()

End Class