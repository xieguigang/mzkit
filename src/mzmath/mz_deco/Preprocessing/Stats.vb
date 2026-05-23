Namespace LCMS.Preprocessing

    ' ====================================================================
    '  统计信息类
    ' ====================================================================

    ''' <summary>
    ''' 缺失值处理统计信息
    ''' </summary>
    Public Class MissingValueStatistics
        Public Property TotalMissingBefore As Integer
        Public Property MissingRateBefore As Double
        Public Property TotalMissingAfter As Integer
        Public Property FilteredFeatures As Integer
        Public Property MethodUsed As String
    End Class

    ''' <summary>
    ''' 归一化统计信息
    ''' </summary>
    Public Class NormalizationStatistics
        Public Property MethodUsed As String
        Public Property NormalizationFactors As Dictionary(Of String, Double)
        Public Property TICBefore As Dictionary(Of String, Double)
        Public Property TICAfter As Dictionary(Of String, Double)
    End Class

    ''' <summary>
    ''' 批次矫正统计信息
    ''' </summary>
    Public Class BatchCorrectionStatistics
        Public Property MethodUsed As String
        Public Property NumberOfBatches As Integer
        Public Property SamplesPerBatch As Dictionary(Of Integer, Integer)
        Public Property QCsPerBatch As Dictionary(Of Integer, Integer)
    End Class
End Namespace