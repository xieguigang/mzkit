#Region "Microsoft.VisualBasic::47e7e547b831a28c9f621ec3a4a2495d, TargetedMetabolomics\GCMS\QuantifyAnalysis\SIMMode.vb"

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

    '     Module SIMModeWorker
    ' 
    '         Function: convert, GetSimIon
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.application.netCDF
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.GCMS
Imports SMRUCC.MassSpectrum.Math.GCMS.QuantifyAnalysis

Namespace GCMS.QuantifyAnalysis

    Public Module SIMModeWorker

        ''' <summary>
        ''' 在这里只是读取所有的离子的原始数据出来，并没有进行内标校正
        ''' </summary>
        ''' <param name="refs"></param>
        ''' <param name="experiments"></param>
        ''' <param name="angle!"></param>
        ''' <param name="baselineQuantile!"></param>
        ''' <param name="winSize!"></param>
        ''' <param name="tolerance"></param>
        ''' <param name="scoreCutoff#"></param>
        ''' <param name="top"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function GetSimIon(refs As ROITable(), experiments As String,
                                           Optional angle! = 5,
                                           Optional baselineQuantile! = 0.65,
                                           Optional winSize! = 3,
                                           Optional tolerance As Tolerance = Nothing,
                                           Optional scoreCutoff# = 0.8,
                                           Optional top As Boolean = True) As IEnumerable(Of (ion As ROITable, query As Spectra.LibraryMatrix, ref As Spectra.LibraryMatrix))

            Dim data As Raw = netCDFReader.Open(filePath:=experiments).ReadData
            Dim result As New List(Of ROITable)
            Dim output$ = experiments.TrimSuffix
            Dim i As int = 1
            Dim TIC = data.GetTIC
            Dim ROIlist As ROI() = TIC.Shadows _
                .PopulateROI(angle, MRMpeaks:=False, baselineQuantile:=baselineQuantile) _
                .ToArray
            Dim resultTable As ROITable

            tolerance = tolerance Or Tolerance.DefaultTolerance

            For Each ref As ROITable In refs
                Dim timeRange As DoubleRange = {ref.rtmin - winSize, ref.rtmax + winSize}
                Dim refSpectrum As Spectra.LibraryMatrix = ref.CreateMatrix
                Dim refMz As Spectra.ms2 = refSpectrum.GetMaxInto
                Dim candidates = ROIlist _
                    .SkipWhile(Function(c) c.Time.Max < timeRange.Min) _
                    .TakeWhile(Function(c) c.Time.Min < timeRange.Max) _
                    .Select(Function(region As ROI)
                                ' 在这个循环之中的都是rt符合条件要求的
                                Dim matrixName$ = $"rt={region.rt}, [{Fix(region.Time.Min)},{Fix(region.Time.Max)}]"
                                Dim query = data.GetMsScan(region.Time) _
                                    .GroupByMz() _
                                    .CreateLibraryMatrix(matrixName)
                                Dim maxInto As Spectra.ms2 = query.GetMaxInto
                                Dim score#

                                ' 比较参考的最大的into的mz是否与这个maxInto碎片一致？
                                If tolerance(refMz, maxInto) Then
                                    score = tolerance.AsScore(refMz.mz, maxInto.mz)
                                End If

                                Return (
                                    score:=score,
                                    minScore:=score,
                                    query:=query,
                                    region:=region,
                                    maxInto:=maxInto
                                )
                            End Function) _
                    .Where(Function(candidate)
                               Return candidate.minScore >= scoreCutoff
                           End Function) _
                    .OrderByDescending(Function(candidate) candidate.minScore) _
                    .ToArray

                refSpectrum.Name = ref.ID

                For Each candidate In candidates
                    ' 在这里的峰面积就是定量离子的信号响应强度
                    resultTable = candidate.region.convert(
                        raw:=data,
                        ri:=0,
                        title:=ref.ID,
                        maxInto:=candidate.maxInto
                    )
                    ' 将获取得到原始的峰面积信息
                    ' 在下一个步骤函数之中将会除以内标的峰面积得到X坐标轴的数据
                    resultTable.IS = ref.IS

                    Yield (resultTable, candidate.query, refSpectrum)

                    If top Then
                        Exit For
                    End If
                Next
            Next
        End Function

        <Extension>
        Private Function convert(ROI As ROI, raw As Raw, ri#, maxInto As Spectra.ms2, title$) As ROITable
            Dim spectra = raw.GetMsScan(ROI.Time).GroupByMz
            Dim base64 As String = spectra _
                .Select(Function(mz) $"{mz.mz} {mz.intensity}") _
                .JoinBy(ASCII.TAB) _
                .Base64String

            Return New ROITable With {
                .sn = ROI.snRatio,
                .baseline = ROI.Baseline,
                .ID = title,
                .integration = maxInto.intensity,
                .maxInto = ROI.MaxInto,
                .ri = ri,
                .rt = ROI.rt,
                .rtmax = ROI.Time.Max,
                .rtmin = ROI.Time.Min,
                .mass_spectra = base64
            }
        End Function
    End Module
End Namespace
