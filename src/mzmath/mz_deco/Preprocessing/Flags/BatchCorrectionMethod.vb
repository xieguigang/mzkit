Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 批次矫正方法枚举
    ''' </summary>
    Public Enum BatchCorrectionMethod As Integer
        None = 0
        MeanCentering = 1
        MedianCentering = 2
        QC_RLSC = 3
        LOESS = 4
        ComBat = 5
        SVR = 6
        NormalizeQC = 7
    End Enum
End Namespace