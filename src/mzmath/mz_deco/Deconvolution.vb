Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports scan = BioNovoGene.Analytical.MassSpectrometry.Math.IMs1Scan
Imports stdNum = System.Math

''' <summary>
''' #### 解卷积计算步骤
''' 
''' 1. 首先对每一个原始数据点按照mz进行数据分组
''' 2. 对每一个mz数据分组按照rt进行升序排序
''' 3. 对每一个mz数据分组进行解卷积，得到峰列表
''' 4. 输出peaktable结果，完成解卷积操作
''' </summary>
Public Module Deconvolution

    ''' <summary>
    ''' All of the mz value in <paramref name="mzpoints"/> should be equals
    ''' </summary>
    ''' <param name="mzpoints"></param>
    ''' <returns></returns>
    ''' <remarks>实际的解卷积操作步骤：应用于处理复杂的样本数据</remarks>
    <Extension>
    Public Iterator Function GetPeakGroups(mzpoints As MzGroup, Optional quantile# = 0.65) As IEnumerable(Of PeakFeature)
        For Each ROI In mzpoints.XIC.Shadows.PopulateROI(, baselineQuantile:=quantile)
            Yield New PeakFeature With {
                .mz = mzpoints.mz,
                .Baseline = ROI.Baseline,
                .Integration = ROI.Integration,
                .MaxInto = ROI.MaxInto,
                .Noise = ROI.Noise,
                .rt = ROI.rt,
                .rtmax = ROI.Time.Max,
                .rtmin = ROI.Time.Min
            }
        Next
    End Function


    <Extension>
    Private Function localMax(window As IEnumerable(Of ChromatogramTick)) As ChromatogramTick
        Return window.OrderByDescending(Function(t) t.Intensity).First
    End Function

    <Extension>
    Private Function localMin(window As IEnumerable(Of ChromatogramTick)) As ChromatogramTick
        Return window.OrderBy(Function(t) t.Intensity).First
    End Function

    ''' <summary>
    ''' Separation of mass signals.
    ''' (进行原始数据的mz分组操作，然后进行rt的升序排序)
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function GetMzGroups(scans As IEnumerable(Of scan), Optional tolerance As Tolerance = Nothing) As IEnumerable(Of MzGroup)
        For Each group As NamedCollection(Of scan) In scans.GroupBy(Function(t) t.mz, AddressOf (tolerance Or Tolerance.DefaultTolerance).Assert)
            Dim rawGroup As scan() = group.ToArray
            Dim timePoints As NamedCollection(Of scan)() = rawGroup.GroupBy(Function(t) t.rt, Function(a, b) stdNum.Abs(a - b) <= 0.05).ToArray
            Dim xic As ChromatogramTick() = timePoints _
                .Select(Function(t)
                            Dim rt As Double = Aggregate p As scan In t Into Average(p.rt)
                            Dim into As Double = Aggregate p As scan In t Into Max(p.intensity)

                            Return New ChromatogramTick With {
                                .Time = rt,
                                .Intensity = into
                            }
                        End Function) _
                .OrderBy(Function(t) t.Time) _
                .ToArray
            Dim mz As Double = Aggregate t As scan
                               In rawGroup
                               Into Average(t.mz)

            Yield New MzGroup With {
                .mz = mz,
                .XIC = xic
            }
        Next
    End Function

    <Extension>
    Public Iterator Function DecoMzGroups(mzgroups As IEnumerable(Of MzGroup), Optional quantile# = 0.65) As IEnumerable(Of PeakFeature)
        Dim mzfeatures = mzgroups.AsParallel.Select(Function(mz) mz.GetPeakGroups(quantile)).IteratesALL.GroupBy(Function(m) stdNum.Round(m.mz).ToString).ToArray
        Dim guid As New Dictionary(Of String, Counter)

        For Each mzidgroup In mzfeatures
            Dim mId = mzidgroup.Key
            Dim rtIdgroup = mzidgroup.GroupBy(Function(m) stdNum.Round(m.rt).ToString).ToArray

            For Each rtgroup In rtIdgroup
                Dim uid = $"M{mId}T{rtgroup.Key}"
                guid(uid) = 0

                For Each feature In rtgroup
                    If guid(uid).Value = 0 Then
                        feature.xcms_id = uid
                    Else
                        feature.xcms_id = uid & "_" & guid(uid).ToString
                    End If

                    guid(uid).Hit()

                    Yield feature
                Next
            Next
        Next
    End Function
End Module
