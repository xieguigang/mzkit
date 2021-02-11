#Region "Microsoft.VisualBasic::a0734a905040b36933a760001c14c925, TargetedMetabolomics\GCMS\QuantifyAnalysis\ScanMode.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Module ScanModeWorker
    ' 
    '         Function: ConvertAsTabular, FitContent, ScanContents, ScanIons
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text

Namespace GCMS.QuantifyAnalysis

    ''' <summary>
    ''' GCMS自动化定量分析模块
    ''' 
    ''' https://github.com/cheminfo-js/netcdf-gcms
    ''' </summary>
    Public Module ScanModeWorker

        ''' <summary>
        ''' 利用标准品的信息从GCMS的实验数据之中找出对应的检测物质的检测结果
        ''' </summary>
        ''' <param name="standards">标准品数据</param>
        ''' <param name="data">实验数据</param>
        ''' <param name="sn#">信噪比阈值，低于这个阈值的信号都将会被抛弃</param>
        ''' <param name="winSize">
        ''' 进行查找的时间窗大小
        ''' </param>
        ''' <returns>
        ''' 这个函数所返回来的结果之中已经包含有必须的峰面积等信息了
        ''' </returns>
        <Extension>
        Public Iterator Function ScanIons(standards As IEnumerable(Of ROITable), data As Raw, peakwidth As DoubleRange,
                                          Optional sn# = 3,
                                          Optional winSize! = 3,
                                          Optional scoreCutoff# = 0.85,
                                          Optional angleCutoff# = 5,
                                          Optional all As Boolean = False) As IEnumerable(Of (ROITable, query As LibraryMatrix, ref As LibraryMatrix))

            Dim ROIlist As ROI() = data _
                .ExportROI(angleCutoff, peakwidth) _
                .Where(Function(ROI) ROI.snRatio >= sn) _
                .ToArray
            Dim resultTable As ROITable

            ' 先用时间窗，找出和参考相近的实验数据
            ' 然后做质谱图的比对操作
            For Each ref As ROITable In standards
                Dim timeRange As DoubleRange = {ref.rtmin - winSize, ref.rtmax + winSize}
                Dim refSpectrum As LibraryMatrix = ref.CreateMatrix

                refSpectrum.name = ref.ID

                Dim candidates = ROIlist _
                    .SkipWhile(Function(c) c.time.Max < timeRange.Min) _
                    .TakeWhile(Function(c) c.time.Min < timeRange.Max) _
                    .Select(Function(region As ROI)
                                ' 在这个循环之中的都是rt符合条件要求的
                                Dim matrixName$ = $"rt={region.rt}, [{Fix(region.time.Min)},{Fix(region.time.Max)}]"
                                Dim query = data.GetMsScan(region.time) _
                                    .GroupByMz() _
                                    .CreateLibraryMatrix(matrixName)
                                Dim score = GlobalAlignment.TwoDirectionSSM(
                                    x:=query.ms2,
                                    y:=refSpectrum.ms2,
                                    tolerance:=Tolerance.DefaultTolerance
                                )
                                Dim minScore# = {score.forward, score.reverse}.Min

                                Return (
                                    score:=score,
                                    minScore:=minScore,
                                    query:=query,
                                    region:=region
                                )
                            End Function) _
                    .Where(Function(candidate)
                               Return candidate.minScore >= scoreCutoff
                           End Function) _
                    .OrderByDescending(Function(candidate) candidate.minScore) _
                    .ToArray

                For Each candidate In candidates
                    ' 计算出峰面积
                    Dim TPA = candidate.region _
                        .ticks _
                        .Shadows _
                        .TPAIntegrator(candidate.region.time, 0.65, PeakAreaMethods.NetPeakSum)

                    resultTable = candidate.region.ConvertAsTabular(
                        raw:=data,
                        ri:=0,
                        title:=ref.ID
                    )
                    ' 将获取得到原始的峰面积信息
                    ' 在下一个步骤函数之中将会除以内标的峰面积得到X坐标轴的数据
                    resultTable.integration = TPA.area
                    resultTable.IS = ref.IS

                    Yield (resultTable, candidate.query, refSpectrum)

                    If Not all Then
                        Exit For
                    End If
                Next
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ROI"></param>
        ''' <param name="raw">需要通过raw数据来获取得到对应的质谱图结果数据</param>
        ''' <param name="ri#"></param>
        ''' <param name="title$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ConvertAsTabular(ROI As ROI, raw As Raw, ri#, title$) As ROITable
            Dim spectra = raw.GetMsScan(ROI.time).GroupByMz
            Dim base64 As String = spectra _
                .Select(Function(mz) $"{mz.mz} {mz.intensity}") _
                .JoinBy(ASCII.TAB) _
                .Base64String

            Return New ROITable With {
                .sn = ROI.snRatio,
                .baseline = ROI.baseline,
                .ID = title,
                .integration = ROI.integration,
                .maxInto = ROI.maxInto,
                .ri = ri,
                .rt = ROI.rt,
                .rtmax = ROI.time.Max,
                .rtmin = ROI.time.Min,
                .mass_spectra = base64
            }
        End Function

        ''' <summary>
        ''' 利用参考库来查找实验数据之中的目标物质
        ''' </summary>
        ''' <param name="ref">靶向GCMS的标准品库</param>
        ''' <param name="experiments$">实验数据的cdf文件路径</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function ScanContents(ref As ROITable(), experiments$,
                                     Optional peakwidth As DoubleRange = Nothing,
                                     Optional sn_threshold# = 0,
                                     Optional angle# = 5,
                                     Optional baselineQuantile# = 0.3,
                                     Optional scoreCutoff# = 0.8,
                                     Optional ByRef alignments As (ID$, query As LibraryMatrix, ref As LibraryMatrix)() = Nothing) As ROITable()

            Dim data As Raw = netCDFReader.Open(filePath:=experiments).ReadData
            Dim result As New List(Of ROITable)
            Dim alignList As New List(Of (String, LibraryMatrix, LibraryMatrix))

            For Each target In ref.ScanIons(
                data, peakwidth,
                sn:=sn_threshold,
                angleCutoff:=angle,
                scoreCutoff:=scoreCutoff
            )
                result += target.Item1
                alignList += (target.Item1.ID, target.query, target.ref)
            Next

            alignments = alignList.ToArray

            Return result.ToArray
        End Function

        <Extension>
        Public Iterator Function FitContent(Of T As ROITable)(ROIlist As IEnumerable(Of (fileName$, data As T)), standardCurves As Dictionary(Of String, LinearQuantitative.StandardCurve)) As IEnumerable(Of ChromatographyPeaktable)
            Dim fileTable = ROIlist.SafeQuery _
                .GroupBy(Function(file) file.fileName) _
                .ToDictionary(Function(file) file.Key,
                              Function(g)
                                  Return g.ToDictionary(Function(c) c.data.ID,
                                                        Function(c)
                                                            Return c.data
                                                        End Function)
                              End Function)

            For Each targetVal As KeyValuePair(Of String, Dictionary(Of String, T)) In fileTable
                Dim rawFile$ = targetVal.Key
                Dim detections As Dictionary(Of String, T) = targetVal.Value

                For Each target As T In detections _
                    .Values _
                    .Where(Function(tt)
                               Return Not tt.ID.TextEquals(tt.IS) AndAlso standardCurves.ContainsKey(tt.ID)
                           End Function)

                    Dim TPA#
                    Dim standardCurve As LinearQuantitative.StandardCurve = standardCurves(target.ID)

                    If Not standardCurve.IS Is Nothing Then
                        ' 需要做内标校正
                        TPA = target.integration / detections(target.IS).integration
                    Else
                        TPA = target.integration
                    End If

                    Yield New ChromatographyPeaktable With {
                        .ID = target.ID,
                        .baseline = target.baseline,
                        .integration = target.integration,
                        .content = standardCurve.linear.GetY(TPA),
                        .mass_spectra = target.mass_spectra,
                        .maxInto = target.maxInto,
                        .ri = target.ri,
                        .rt = target.rt,
                        .rtmax = target.rtmax,
                        .rtmin = target.rtmin,
                        .sn = target.sn,
                        .rawFile = rawFile,
                        .[IS] = target.IS,
                        .TPACalibration = TPA
                    }
                Next
            Next
        End Function
    End Module
End Namespace
