Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 归一化方法枚举
    ''' </summary>
    Public Enum NormalizationMethod As Integer
        None = 0
        TotalIonCount = 1
        MedianNorm = 2
        Quantile = 3
        PQN = 4
        Log2 = 10
        Log10 = 11
        Ln = 12
        CubeRoot = 13
        SqrtTransform = 14
        AutoScaling = 20
        ParetoScaling = 21
        RangeScaling = 22
        VastScaling = 23
        LevelScaling = 24
    End Enum
End Namespace