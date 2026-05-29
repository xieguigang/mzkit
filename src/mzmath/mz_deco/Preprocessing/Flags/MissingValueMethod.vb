Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 缺失值处理方法枚举
    ''' 
    ''' 在LC-MS代谢组学数据中，缺失值通常来源于以下几种情况：
    ''' 1. 信号低于检测限（左截断数据）- 推荐使用HalfMin、QRILC
    ''' 2. 随机缺失（如仪器波动）- 推荐使用KNN、PCA、Mean、Median
    ''' 3. 稀疏缺失（某些代谢物在部分样本中确实不存在）- 推荐使用Zero
    ''' </summary>
    Public Enum MissingValueMethod As Integer
        None = 0
        HalfMin = 1
        MinValue = 2
        Zero = 3
        Mean = 4
        Median = 5
        KNN = 6
        PCA = 7
        QRILC = 8
        RandomMin = 9
    End Enum
End Namespace