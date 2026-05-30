Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace LCMS.Preprocessing

    ''' <summary>
    ''' LC-MS数据预处理管线类
    ''' 
    ''' 提供完整的预处理流程，支持灵活的方法组合。
    ''' 所有处理步骤均可独立调用，也可通过Process方法一次性执行。
    ''' </summary>
    ''' 
    ''' <remarks>
    ''' LC-MS表达矩阵数据预处理模块 - 主流程管线
    ''' 
    ''' 本模块是LC-MS数据预处理的入口点，将缺失值处理、归一化和批次矫正
    ''' 三个步骤串联为完整的预处理流程。
    ''' 
    ''' 典型处理流程：
    ''' 1. 数据输入：从xcms2离子数组构建表达矩阵
    ''' 2. 缺失值过滤：移除缺失率过高的特征
    ''' 3. 缺失值插补：用选定方法填充缺失值
    ''' 4. 归一化：消除样本间系统性差异
    ''' 5. 批次矫正：消除批次效应和信号漂移
    ''' 6. 数据输出：将处理后的矩阵转回xcms2离子数组
    ''' 
    ''' 使用示例：
    ''' Dim options As New PreprocessingOptions()
    ''' options.MissingValueMethod = MissingValueMethod.HalfMin
    ''' options.NormalizationMethod = NormalizationMethod.PQN
    ''' options.BatchCorrectionMethod = BatchCorrectionMethod.QC_RLSC
    ''' 
    ''' Dim preprocessor As New LCMSPreprocessor(options)
    ''' Dim result = preprocessor.Process(ions, samples)
    ''' </remarks>
    Public Class LCMSPreprocessor

        Private _options As PreprocessingOptions

        ''' <summary>
        ''' 预处理配置参数
        ''' </summary>
        Public Property Options As PreprocessingOptions
            Get
                Return _options
            End Get
            Set(value As PreprocessingOptions)
                _options = value
            End Set
        End Property

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="options">预处理配置参数，为Nothing时使用默认配置</param>
        Public Sub New(Optional options As PreprocessingOptions = Nothing)
            _options = If(options, New PreprocessingOptions())
        End Sub

        ' ================================================================
        '  主流程方法
        ' ================================================================

        ''' <summary>
        ''' 执行完整的预处理流程
        ''' 
        ''' 流程：缺失值标记 → 缺失值过滤 → 缺失值插补 → 归一化 → 批次矫正
        ''' </summary>
        ''' <param name="ions">xcms2离子数组</param>
        ''' <param name="samples">样本信息数组，在某些算法配置情况下可以缺失</param>
        ''' <returns>预处理结果对象</returns>
        Public Function Process(ions As xcms2(), Optional samples As SampleInfo() = Nothing) As PreprocessingResult
            If ions Is Nothing Then Throw New ArgumentNullException(NameOf(ions))
            If samples Is Nothing Then
                Call "sampleinfo metadata is missing, some of the algorithmwill not working as expected.".warning

                samples = ions.PropertyNames _
                    .GuessPossibleGroups(maxDepth:=False) _
                    .Select(Iterator Function(group) As IEnumerable(Of SampleInfo)
                                For Each id As String In group.value
                                    Yield New SampleInfo(id, group.name)
                                Next
                            End Function) _
                    .IteratesALL _
                    .ToArray
            End If

            If samples.All(Function(s) s.injectionOrder = 0) Then
                Call "missing injection order data!".warning

                For i As Integer = 0 To samples.Length - 1
                    samples(i).injectionOrder = i + 1
                Next
            End If
            If samples.All(Function(s) s.batch = 0) Then
                Call "missing batch group data, assuming that all samples in one sample batch.".warning

                For Each sample As SampleInfo In samples
                    sample.batch = 1
                Next
            End If

            Dim result As New PreprocessingResult()
            result.OriginalIons = ions
            result.Samples = samples

            ' Step 1: 构建表达矩阵
            Dim matrix = BuildMatrix(ions, samples)
            result.MatrixBeforePreprocessing = CType(matrix.Clone(), Double(,))

            ' Step 2: 标记缺失值
            matrix = MissingValueImputation.MarkMissingValues(matrix, _options)

            ' Step 3: 缺失值过滤
            Dim keptIndices As List(Of Integer) = Nothing
            If _options.EnableMissingValueFilter Then
                matrix = MissingValueImputation.FilterByMissingRate(matrix, _options.MaxMissingRate, keptIndices)
                result.FilteredIons = GetFilteredIons(ions, keptIndices)
            Else
                keptIndices = New List(Of Integer)
                For i As Integer = 0 To ions.Length - 1
                    keptIndices.Add(i)
                Next
            End If

            ' 记录缺失值统计
            Dim missingStats As New MissingValueStatistics()
            missingStats.TotalMissingBefore = MissingValueImputation.CountMissing(matrix)
            missingStats.MissingRateBefore = CDbl(missingStats.TotalMissingBefore) / (matrix.GetLength(0) * matrix.GetLength(1))
            missingStats.FilteredFeatures = ions.Length - keptIndices.Count

            ' Step 4: 缺失值插补
            If _options.MissingValueMethod <> MissingValueMethod.None Then
                matrix = MissingValueImputation.Impute(matrix, _options.MissingValueMethod, _options)
            End If

            missingStats.TotalMissingAfter = MissingValueImputation.CountMissing(matrix)
            missingStats.MethodUsed = _options.MissingValueMethod.ToString()
            result.MissingValueStats = missingStats

            ' Step 5: 归一化
            Dim normFactors As Double() = Nothing
            If _options.NormalizationMethod <> NormalizationMethod.None Then
                matrix = Normalization.Normalize(matrix, _options.NormalizationMethod, _options, normFactors)
            End If

            Dim normStats As New NormalizationStatistics()
            normStats.MethodUsed = _options.NormalizationMethod.ToString()
            If normFactors IsNot Nothing Then
                normStats.NormalizationFactors = New Dictionary(Of String, Double)
                For j As Integer = 0 To samples.Length - 1
                    normStats.NormalizationFactors(samples(j).ID) = normFactors(j)
                Next
            End If
            result.NormalizationStats = normStats

            ' Step 6: 批次矫正
            If _options.BatchCorrectionMethod <> BatchCorrectionMethod.None Then
                matrix = BatchCorrection.Correct(matrix, samples, _options.BatchCorrectionMethod, _options)
            End If

            Dim batchStats As New BatchCorrectionStatistics()
            batchStats.MethodUsed = _options.BatchCorrectionMethod.ToString()
            Dim batchIds = BatchCorrection.GetBatchIds(samples)
            batchStats.NumberOfBatches = batchIds.Length
            batchStats.SamplesPerBatch = New Dictionary(Of Integer, Integer)
            batchStats.QCsPerBatch = New Dictionary(Of Integer, Integer)
            For Each bid In batchIds
                batchStats.SamplesPerBatch(bid) = BatchCorrection.GetBatchSampleIndices(samples, bid).Length
                batchStats.QCsPerBatch(bid) = BatchCorrection.GetQCSampleIndices(samples, _options.QCLabel).
                                                    Count(Function(idx) samples(idx).batch = bid)
            Next
            result.BatchCorrectionStats = batchStats

            ' Step 7: 将矩阵转回xcms2数组
            result.ProcessedIons = BuildIons(matrix, ions, keptIndices, samples)
            result.MatrixAfterPreprocessing = matrix

            Return result
        End Function

        ' ================================================================
        '  独立步骤方法（可单独调用）
        ' ================================================================

        ''' <summary>
        ''' 仅执行缺失值处理
        ''' </summary>
        Public Function ProcessMissingValues(ions As xcms2(), samples As SampleInfo()) As xcms2()
            Dim matrix = BuildMatrix(ions, samples)
            matrix = MissingValueImputation.MarkMissingValues(matrix, _options)

            Dim keptIndices As List(Of Integer) = Nothing
            If _options.EnableMissingValueFilter Then
                matrix = MissingValueImputation.FilterByMissingRate(matrix, _options.MaxMissingRate, keptIndices)
            Else
                keptIndices = New List(Of Integer)
                For i As Integer = 0 To ions.Length - 1
                    keptIndices.Add(i)
                Next
            End If

            If _options.MissingValueMethod <> MissingValueMethod.None Then
                matrix = MissingValueImputation.Impute(matrix, _options.MissingValueMethod, _options)
            End If

            Return BuildIons(matrix, ions, keptIndices, samples)
        End Function

        ''' <summary>
        ''' 仅执行归一化
        ''' </summary>
        Public Function ProcessNormalization(ions As xcms2(), samples As SampleInfo()) As xcms2()
            Dim matrix = BuildMatrix(ions, samples)

            If _options.NormalizationMethod <> NormalizationMethod.None Then
                matrix = Normalization.Normalize(matrix, _options.NormalizationMethod, _options)
            End If

            Dim keptIndices As New List(Of Integer)
            For i As Integer = 0 To ions.Length - 1
                keptIndices.Add(i)
            Next

            Return BuildIons(matrix, ions, keptIndices, samples)
        End Function

        ''' <summary>
        ''' 仅执行批次矫正
        ''' </summary>
        Public Function ProcessBatchCorrection(ions As xcms2(), samples As SampleInfo()) As xcms2()
            Dim matrix = BuildMatrix(ions, samples)

            If _options.BatchCorrectionMethod <> BatchCorrectionMethod.None Then
                matrix = BatchCorrection.Correct(matrix, samples, _options.BatchCorrectionMethod, _options)
            End If

            Dim keptIndices As New List(Of Integer)
            For i As Integer = 0 To ions.Length - 1
                keptIndices.Add(i)
            Next

            Return BuildIons(matrix, ions, keptIndices, samples)
        End Function

        ' ================================================================
        '  矩阵构建与转换
        ' ================================================================

        ''' <summary>
        ''' 从xcms2离子数组构建表达矩阵
        ''' 矩阵维度：nFeatures × nSamples
        ''' 行对应离子特征，列对应样本
        ''' </summary>
        Public Shared Function BuildMatrix(ions As xcms2(), samples As SampleInfo()) As Double(,)
            Dim nFeatures As Integer = ions.Length
            Dim nSamples As Integer = samples.Length
            Dim matrix(nFeatures - 1, nSamples - 1) As Double

            For i As Integer = 0 To nFeatures - 1
                For j As Integer = 0 To nSamples - 1
                    Dim sampleId As String = samples(j).ID
                    If ions(i).HasProperty(sampleId) Then
                        matrix(i, j) = ions(i)(sampleId)
                    Else
                        matrix(i, j) = Double.NaN
                    End If
                Next
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' 将表达矩阵转回xcms2离子数组
        ''' </summary>
        ''' <param name="matrix">表达矩阵</param>
        ''' <param name="originalIons">原始离子数组（用于保留mz、rt等信息）</param>
        ''' <param name="keptIndices">保留的特征索引</param>
        ''' <param name="samples">样本信息数组</param>
        ''' <returns>更新后的xcms2离子数组</returns>
        Public Shared Function BuildIons(matrix As Double(,), originalIons As xcms2(),
                                          keptIndices As List(Of Integer),
                                          samples As SampleInfo()) As xcms2()
            Dim nFeatures As Integer = matrix.GetLength(0)
            Dim nSamples As Integer = matrix.GetLength(1)
            Dim result(nFeatures - 1) As xcms2

            For i As Integer = 0 To nFeatures - 1
                Dim ion As New xcms2()
                Dim origIdx As Integer = keptIndices(i)

                ' 保留原始离子的元数据
                ion.ID = originalIons(origIdx).ID
                ion.mz = originalIons(origIdx).mz
                ion.mzmin = originalIons(origIdx).mzmin
                ion.mzmax = originalIons(origIdx).mzmax
                ion.rt = originalIons(origIdx).rt
                ion.rtmin = originalIons(origIdx).rtmin
                ion.rtmax = originalIons(origIdx).rtmax

                ' 更新样本丰度数据
                ion.Properties = New Dictionary(Of String, Double)
                For j As Integer = 0 To nSamples - 1
                    ion(samples(j).ID) = matrix(i, j)
                Next

                result(i) = ion
            Next

            Return result
        End Function

        ''' <summary>
        ''' 获取被过滤掉的离子
        ''' </summary>
        Private Shared Function GetFilteredIons(ions As xcms2(), keptIndices As List(Of Integer)) As xcms2()
            If keptIndices Is Nothing Then Return New xcms2() {}
            Dim keptSet As New HashSet(Of Integer)(keptIndices)
            Dim filtered As New List(Of xcms2)
            For i As Integer = 0 To ions.Length - 1
                If Not keptSet.Contains(i) Then
                    filtered.Add(ions(i))
                End If
            Next
            Return filtered.ToArray()
        End Function

        ' ================================================================
        '  推荐配置方法
        ' ================================================================

        ''' <summary>
        ''' 根据数据特征自动推荐预处理参数
        ''' 
        ''' 推荐规则：
        ''' - 缺失值处理：
        '''   缺失率 &lt; 30% → HalfMin（假设左截断）
        '''   缺失率 30%-60% → KNN（随机缺失较多）
        '''   缺失率 > 60% → QRILC（左截断严重）
        ''' 
        ''' - 归一化：
        '''   默认推荐PQN（LC-MS最常用）
        ''' 
        ''' - 批次矫正：
        '''   QC样本 >= 5 → QC-RLSC
        '''   QC样本 3-4 → LOESS
        '''   无QC样本 → ComBat
        ''' </summary>
        Public Shared Function RecommendOptions(ions As xcms2(), samples As SampleInfo()) As PreprocessingOptions
            Dim options As New PreprocessingOptions()

            ' 估计缺失率
            Dim nFeatures As Integer = ions.Length
            Dim nSamples As Integer = samples.Length
            Dim totalCells As Integer = nFeatures * nSamples
            Dim missingCells As Integer = 0

            For Each ion In ions
                For Each s In samples
                    If Not ion.HasProperty(s.ID) OrElse ion(s.ID) = 0 Then
                        missingCells += 1
                    End If
                Next
            Next

            Dim missingRate As Double = CDbl(missingCells) / totalCells

            ' 推荐缺失值处理方法
            If missingRate < 0.3 Then
                options.MissingValueMethod = MissingValueMethod.HalfMin
            ElseIf missingRate < 0.6 Then
                options.MissingValueMethod = MissingValueMethod.KNN
            Else
                options.MissingValueMethod = MissingValueMethod.QRILC
            End If

            ' 推荐归一化方法
            options.NormalizationMethod = NormalizationMethod.PQN

            ' 推荐批次矫正方法
            Dim qcCount As Integer = samples.Count(Function(s) s.sample_info = "QC")
            If qcCount >= 5 Then
                options.BatchCorrectionMethod = BatchCorrectionMethod.QC_RLSC
            ElseIf qcCount >= 3 Then
                options.BatchCorrectionMethod = BatchCorrectionMethod.LOESS
            Else
                options.BatchCorrectionMethod = BatchCorrectionMethod.ComBat
            End If

            Return options
        End Function

    End Class
End Namespace
