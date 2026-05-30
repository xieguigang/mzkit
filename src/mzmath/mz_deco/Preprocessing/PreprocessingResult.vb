Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace LCMS.Preprocessing

    ''' <summary>
    ''' 预处理结果类
    ''' 包含预处理前后的数据和统计信息
    ''' </summary>
    Public Class PreprocessingResult

        ''' <summary>原始离子数据</summary>
        Public Property OriginalIons As xcms2()
        ''' <summary>预处理后的离子数据</summary>
        Public Property ProcessedIons As xcms2()
        ''' <summary>被过滤掉的离子数据</summary>
        Public Property FilteredIons As xcms2()
        ''' <summary>样本信息</summary>
        Public Property Samples As SampleInfo()

        ''' <summary>预处理前的表达矩阵</summary>
        Public Property MatrixBeforePreprocessing As Double(,)
        ''' <summary>预处理后的表达矩阵</summary>
        Public Property MatrixAfterPreprocessing As Double(,)

        ''' <summary>缺失值处理统计信息</summary>
        Public Property MissingValueStats As MissingValueStatistics
        ''' <summary>归一化统计信息</summary>
        Public Property NormalizationStats As NormalizationStatistics
        ''' <summary>批次矫正统计信息</summary>
        Public Property BatchCorrectionStats As BatchCorrectionStatistics

    End Class
End Namespace