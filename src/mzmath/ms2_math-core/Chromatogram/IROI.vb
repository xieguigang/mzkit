Namespace Chromatogram

    ''' <summary>
    ''' 一个ROI区域就是色谱图上面的一个时间范围内的色谱峰数据
    ''' </summary>
    Public Interface IROI

        ''' <summary>
        ''' 色谱图区域范围的时间下限
        ''' </summary>
        ''' <returns></returns>
        Property rtmin As Double

        ''' <summary>
        ''' 色谱图区域范围的时间上限
        ''' </summary>
        ''' <returns></returns>
        Property rtmax As Double
    End Interface
End Namespace