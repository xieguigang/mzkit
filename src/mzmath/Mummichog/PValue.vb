Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Module PValue

    ' ================================================================
    ' 辅助方法: 从样本数据计算p值
    ' ================================================================

    ''' <summary>
    ''' 从xcms2峰表的样本数据计算差异表达p值
    ''' <para>使用Welch's t检验比较对照组和处理组</para>
    ''' </summary>
    ''' <param name="peaks">离子峰列表</param>
    ''' <param name="controlSamples">对照组样本名</param>
    ''' <param name="treatmentSamples">处理组样本名</param>
    Public Function ComputePValues(peaks As IEnumerable(Of xcms2),
                                    controlSamples As IEnumerable(Of String),
                                    treatmentSamples As IEnumerable(Of String)) As Dictionary(Of String, Double)

        Dim controlSet As New HashSet(Of String)(controlSamples)
        Dim treatmentSet As New HashSet(Of String)(treatmentSamples)
        Dim pValues As New Dictionary(Of String, Double)

        For Each peak In peaks
            Dim controlValues As New List(Of Double)
            Dim treatmentValues As New List(Of Double)

            If peak.Properties IsNot Nothing Then
                For Each kvp In peak.Properties
                    If controlSet.Contains(kvp.Key) AndAlso kvp.Value > 0 Then
                        controlValues.Add(kvp.Value)
                    End If
                    If treatmentSet.Contains(kvp.Key) AndAlso kvp.Value > 0 Then
                        treatmentValues.Add(kvp.Value)
                    End If
                Next
            End If

            ' 样本量不足, p值设为1.0 (不显著)
            If controlValues.Count < 2 OrElse treatmentValues.Count < 2 Then
                pValues(peak.ID) = 1.0
            Else
                pValues(peak.ID) = t.Test(controlValues, treatmentValues).Pvalue
            End If
        Next

        Return pValues
    End Function

    ''' <summary>
    ''' 从xcms2峰表的样本数据计算差异表达p值
    ''' <para>使用ANOVA单因素F检验比较多个实验组别的数据</para>
    ''' </summary>
    ''' <param name="peaks">离子峰列表</param>
    Public Function ComputePValues(peaks As IEnumerable(Of xcms2), groups As DataGroup()) As Dictionary(Of String, Double)
        Dim pValues As New Dictionary(Of String, Double)

        For Each peak In peaks
            Dim data As IEnumerable(Of Double()) = From group As DataGroup In groups Select peak(group.sample_id)
            Dim anova As New AnovaFTest(data.ToArray)

            pValues(peak.ID) = anova.PValue
        Next

        Return pValues
    End Function
End Module
