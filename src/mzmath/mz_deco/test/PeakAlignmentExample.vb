Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.PeakAlignment


''' <summary>
''' PeakAlignment模块使用示例
''' 
''' 本文件演示了如何使用PeakAlignment模块中的四种峰对齐算法
''' 将各样本的离子峰在保留时间上对齐并生成峰面积表达矩阵
''' </summary>
Public Class PeakAlignmentExample

    ''' <summary>
    ''' 示例1：使用默认参数（密度分组对齐）进行峰对齐
    ''' </summary>
    Public Sub Example1_DefaultAlignment()
        ' 假设已经从原始数据文件中提取了各样本的离子峰
        Dim peaks As New Dictionary(Of String, PeakFeature())

        ' 从各样本文件中加载峰数据（此处为示意，实际应从文件读取）
        peaks("sample_01.raw") = LoadPeaksFromFile("sample_01.raw")
        peaks("sample_02.raw") = LoadPeaksFromFile("sample_02.raw")
        peaks("sample_03.raw") = LoadPeaksFromFile("sample_03.raw")
        peaks("QC_01.raw") = LoadPeaksFromFile("QC_01.raw")

        ' 使用默认参数执行峰对齐（密度分组算法，m/z容差0.025Da，RT容差30秒）
        Dim expressionMatrix As xcms2() = PeakAlignment.AlignPeaks(peaks)

        ' 输出结果
        Console.WriteLine("峰对齐完成，共得到 {0} 个离子特征", expressionMatrix.Length)
        For Each feature In expressionMatrix
            Console.WriteLine("特征 {0}: m/z={1:F4}, RT={2:F1}s", feature.ID, feature.mz, feature.rt)
            For Each kv In feature.Properties
                Console.WriteLine("  样本 {0}: 峰面积={1:F2}", kv.Key, kv.Value)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 示例2：使用直接匹配对齐算法
    ''' 适用于保留时间漂移较小的情况
    ''' </summary>
    Public Sub Example2_DirectMatchAlignment()
        Dim peaks As New Dictionary(Of String, PeakFeature())
        peaks("sample_01.raw") = LoadPeaksFromFile("sample_01.raw")
        peaks("sample_02.raw") = LoadPeaksFromFile("sample_02.raw")

        Dim params As New AlignmentParameters()
        params.method = AlignmentMethod.DirectMatch
        params.mzTolerance = 0.025       ' m/z绝对容差 0.025 Da
        params.mzToleranceMode = Ms1.MassToleranceType.Da
        params.rtTolerance = 30.0        ' RT容差 30秒
        params.minFraction = 0.5         ' 特征至少在50%的样本中出现
        params.fillGaps = True           ' 启用缺失值填充

        Dim result As xcms2() = PeakAlignment.AlignPeaks(peaks, params)
        Console.WriteLine("直接匹配对齐完成，共 {0} 个特征", result.Length)
    End Sub

    ''' <summary>
    ''' 示例3：使用LOESS保留时间校正对齐
    ''' 适用于存在非线性保留时间漂移的情况
    ''' </summary>
    Public Sub Example3_LOESSAlignment()
        Dim peaks As New Dictionary(Of String, PeakFeature())
        peaks("sample_01.raw") = LoadPeaksFromFile("sample_01.raw")
        peaks("sample_02.raw") = LoadPeaksFromFile("sample_02.raw")
        peaks("sample_03.raw") = LoadPeaksFromFile("sample_03.raw")

        Dim params As New AlignmentParameters()
        params.method = AlignmentMethod.LOESS
        params.mzTolerance = 0.025
        params.mzToleranceMode = Ms1.MassToleranceType.Da
        params.rtTolerance = 15.0        ' 校正后使用较小的RT容差
        params.loessSpan = 0.75          ' LOESS带宽参数
        params.loessDegree = 2           ' 二次多项式
        params.referenceSample = "sample_01.raw"  ' 指定参考样本（留空则自动选择）
        params.minFraction = 0.5
        params.fillGaps = True

        Dim result As xcms2() = PeakAlignment.AlignPeaks(peaks, params)
        Console.WriteLine("LOESS对齐完成，共 {0} 个特征", result.Length)
    End Sub

    ''' <summary>
    ''' 示例4：使用Obiwarp动态时间规整对齐
    ''' 适用于保留时间漂移较大且色谱轮廓质量较好的情况
    ''' </summary>
    Public Sub Example4_ObiwarpAlignment()
        Dim peaks As New Dictionary(Of String, PeakFeature())
        peaks("sample_01.raw") = LoadPeaksFromFile("sample_01.raw")
        peaks("sample_02.raw") = LoadPeaksFromFile("sample_02.raw")

        Dim params As New AlignmentParameters()
        params.method = AlignmentMethod.Obiwarp
        params.mzTolerance = 0.025
        params.mzToleranceMode = Ms1.MassToleranceType.Da
        params.rtTolerance = 15.0
        params.obiwarpBinSize = 1.0      ' TIC分段宽度1秒
        params.obiwarpGapPenalty = 0.6   ' DTW间隙惩罚
        params.minFraction = 0.5
        params.fillGaps = True

        Dim result As xcms2() = PeakAlignment.AlignPeaks(peaks, params)
        Console.WriteLine("Obiwarp对齐完成，共 {0} 个特征", result.Length)
    End Sub

    ''' <summary>
    ''' 示例5：使用密度分组对齐（XCMS风格），使用ppm容差
    ''' 适用于高分辨质谱数据
    ''' </summary>
    Public Sub Example5_DensityGroupPPM()
        Dim peaks As New Dictionary(Of String, PeakFeature())
        peaks("sample_01.raw") = LoadPeaksFromFile("sample_01.raw")
        peaks("sample_02.raw") = LoadPeaksFromFile("sample_02.raw")
        peaks("sample_03.raw") = LoadPeaksFromFile("sample_03.raw")

        Dim params As New AlignmentParameters()
        params.method = AlignmentMethod.DensityGroup
        params.mzTolerance = 10.0        ' 10 ppm容差
        params.mzToleranceMode = MassToleranceType.Ppm  ' 使用ppm模式
        params.rtTolerance = 30.0
        params.densityBandwidth = 0.0    ' 自动估计核密度带宽
        params.minFraction = 0.5
        params.fillGaps = True

        Dim result As xcms2() = PeakAlignment.AlignPeaks(peaks, params)
        Console.WriteLine("密度分组对齐完成，共 {0} 个特征", result.Length)
    End Sub

    ''' <summary>
    ''' 示例6：将峰面积表达矩阵导出为CSV格式
    ''' </summary>
    Public Sub Example6_ExportToCSV(expressionMatrix As xcms2(), outputPath As String)
        ' 收集所有样本名称
        Dim sampleNames As New List(Of String)
        If expressionMatrix.Length > 0 Then
            sampleNames = expressionMatrix(0).Properties.Keys.ToList()
        End If

        Using writer As New System.IO.StreamWriter(outputPath, False, System.Text.Encoding.UTF8)
            ' 写入表头
            Dim header As String = "ID,m/z,m/z_min,m/z_max,RT,RT_min,RT_max"
            For Each name In sampleNames
                header &= "," & name
            Next
            writer.WriteLine(header)

            ' 写入每行数据
            For Each feature In expressionMatrix
                Dim line As String = String.Format("{0},{1:F6},{2:F6},{3:F6},{4:F2},{5:F2},{6:F2}",
                    feature.ID, feature.mz, feature.mzmin, feature.mzmax,
                    feature.rt, feature.rtmin, feature.rtmax)

                For Each name In sampleNames
                    Dim area As Double = 0.0
                    If feature.Properties.ContainsKey(name) Then
                        area = feature.Properties(name)
                    End If
                    line &= String.Format(",{0:F4}", area)
                Next

                writer.WriteLine(line)
            Next
        End Using

        Console.WriteLine("表达矩阵已导出到: {0}", outputPath)
    End Sub

    ''' <summary>
    ''' 示例7：算法比较——对同一数据集使用不同算法并比较结果
    ''' </summary>
    Public Sub Example7_CompareAlgorithms()
        Dim peaks As New Dictionary(Of String, PeakFeature())
        peaks("sample_01.raw") = LoadPeaksFromFile("sample_01.raw")
        peaks("sample_02.raw") = LoadPeaksFromFile("sample_02.raw")
        peaks("sample_03.raw") = LoadPeaksFromFile("sample_03.raw")

        Dim algorithms As AlignmentMethod() = {
            AlignmentMethod.DirectMatch,
            AlignmentMethod.LOESS,
            AlignmentMethod.Obiwarp,
            AlignmentMethod.DensityGroup
        }

        Dim algorithmNames As String() = {
            "直接匹配",
            "LOESS校正",
            "Obiwarp",
            "密度分组"
        }

        For i As Integer = 0 To algorithms.Length - 1
            Dim params As New AlignmentParameters()
            params.method = algorithms(i)
            params.mzTolerance = 0.025
            params.rtTolerance = 30.0
            params.minFraction = 0.5
            params.fillGaps = True

            Dim result As xcms2() = PeakAlignment.AlignPeaks(peaks, params)

            ' 统计缺失值比例
            Dim totalCells As Integer = result.Length * peaks.Count
            Dim missingCells As Integer = 0
            For Each feature In result
                For Each kv In feature.Properties
                    If kv.Value = 0.0 Then missingCells += 1
                Next
            Next

            Dim missingRate As Double = CDbl(missingCells) / totalCells * 100.0

            Console.WriteLine("{0}: {1} 个特征, 缺失率 {2:F1}%",
                algorithmNames(i), result.Length, missingRate)
        Next
    End Sub

    ''' <summary>
    ''' 从文件加载峰数据的占位函数
    ''' 实际使用时需要替换为具体的文件读取逻辑
    ''' </summary>
    Private Function LoadPeaksFromFile(rawfile As String) As PeakFeature()
        ' 此处应替换为实际的文件读取逻辑
        ' 例如从.mzML、.mzXML或自定义格式文件中读取峰数据
        Return New PeakFeature() {}
    End Function

End Class
