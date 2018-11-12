Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace Chromatogram

    ''' <summary>
    ''' Region of interest
    ''' </summary>
    Public Class ROI

        ''' <summary>
        ''' 这个区域的起始和结束的时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property Time As DoubleRange
        ''' <summary>
        ''' 这个区域的最大峰高度
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxInto As Double
        Public Property Ticks As ChromatogramTick()
        ''' <summary>
        ''' 所计算出来的基线的响应强度
        ''' </summary>
        ''' <returns></returns>
        Public Property Baseline As Double
        ''' <summary>
        ''' 当前的这个ROI的峰面积积分值
        ''' </summary>
        ''' <returns></returns>
        Public Property Integration As Double

        Public ReadOnly Property PeakWidth As Single
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Time.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Time.ToString
        End Function
    End Class
End Namespace