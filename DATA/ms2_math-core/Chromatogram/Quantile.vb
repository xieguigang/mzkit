Namespace Chromatogram

    ''' <summary>
    ''' 可以通过这个quantile分布对象来了解基线数据是否计算正确
    ''' </summary>
    Public Class Quantile

        ''' <summary>
        ''' Quantile value in this <see cref="Percentage"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Quantile As Double
        ''' <summary>
        ''' [0, 1] quantile percentage
        ''' </summary>
        ''' <returns></returns>
        Public Property Percentage As Double

        Public Overrides Function ToString() As String
            Return $"{Quantile} @ {Percentage * 100}%"
        End Function
    End Class
End Namespace