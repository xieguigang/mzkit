
Namespace LinearQuantitative

    ''' <summary>
    ''' + 积分方法太敏感了，可能会对ROI以及峰型要求非常高
    ''' + 净峰法简单相加会比较鲁棒一些
    ''' </summary>
    Public Enum PeakAreaMethods
#Region "A = S - B"
        ''' <summary>
        ''' 使用简单的信号相加净峰法来计算峰面积
        ''' </summary>
        NetPeakSum = 0

        ''' <summary>
        ''' 使用积分器来进行峰面积的计算
        ''' </summary>
        Integrator = 1
#End Region
        ''' <summary>
        ''' No peak finding, sum all chromatogram ticks signal intensity directly.
        ''' 基线非常低（接近于零）的时候可以使用
        ''' </summary>
        SumAll
        ''' <summary>
        ''' 如果色谱柱的压力非常大，出峰非常的集中，可以直接使用最大的峰高度来近似为峰面积
        ''' </summary>
        MaxPeakHeight
    End Enum
End Namespace