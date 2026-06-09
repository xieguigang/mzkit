Namespace PeakAlignment

    ''' <summary>
    ''' m/z容差模式
    ''' </summary>
    Public Enum ToleranceMode
        ''' <summary>
        ''' 绝对容差，单位为Da
        ''' </summary>
        Absolute
        ''' <summary>
        ''' 相对容差，单位为ppm（百万分之一）
        ''' </summary>
        PPM
    End Enum

    ''' <summary>
    ''' 峰对齐算法类型
    ''' </summary>
    Public Enum AlignmentMethod
        ''' <summary>直接匹配对齐</summary>
        DirectMatch
        ''' <summary>LOESS保留时间校正对齐</summary>
        LOESS
        ''' <summary>Obiwarp动态时间规整对齐</summary>
        Obiwarp
        ''' <summary>密度分组对齐（XCMS风格）</summary>
        DensityGroup
    End Enum
End Namespace