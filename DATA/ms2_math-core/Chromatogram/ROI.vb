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
        Public Property Baseline As Double

        Public Overrides Function ToString() As String
            Return Time.ToString
        End Function
    End Class
End Namespace