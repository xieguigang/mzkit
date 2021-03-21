#Region "Microsoft.VisualBasic::a3bc7f86e27f7eeae0338ae0ef63cc4b, src\mzmath\TargetedMetabolomics\TPAExtensions.vb"

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

    ' Module TPAExtensions
    ' 
    '     Function: findPeakWithoutRtRange, (+2 Overloads) findPeakWithRtRange, ionTPA, isContactWith, ProcessingIonPeakArea
    '               TPAIntegrator
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports stdNum = System.Math

<HideModuleName>
Public Module TPAExtensions

    ''' <summary>
    ''' 对某一个色谱区域进行峰面积的积分计算
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ionTPA(ion As IonChromatogram, TPAFactor As Double, args As MRMArguments) As IonTPA
        Dim vector As IVector(Of ChromatogramTick) = ion.chromatogram.Shadows
        Dim ROIData As ROI() = vector _
            .PopulateROI(
                baselineQuantile:=args.baselineQuantile,
                angleThreshold:=args.angleThreshold,
                peakwidth:=args.peakwidth,
                snThreshold:=args.sn_threshold
            ) _
            .ToArray
        Dim result As IonTPA

        If ROIData.Length = 0 Then
            result = New IonTPA With {
                .name = ion.name,
                .peakROI = New DoubleRange(0, 0)
            }
        Else
            result = ion.ProcessingIonPeakArea(
                vector:=vector,
                ROIData:=ROIData,
                baselineQuantile:=args.baselineQuantile,
                peakAreaMethod:=args.peakAreaMethod,
                integratorTicks:=args.integratorTicks,
                TPAFactor:=TPAFactor,
                timeWindowSize:=args.timeWindowSize,
                bsplineDensity:=args.bspline_density,
                bsplineDegree:=args.bspline_degree,
                timeshiftMethod:=False
            )
        End If

        Return result
    End Function

    Private Function isContactWith(a As DoubleRange, b As DoubleRange) As Boolean
        Return a.IsOverlapping(b) OrElse b.IsOverlapping(a) OrElse a.Contains(b) OrElse b.Contains(a)
    End Function

    ''' <summary>
    ''' 这个函数是建立在没有保留时间漂移的基础上的
    ''' </summary>
    ''' <param name="ROIData"></param>
    ''' <param name="timeWindowSize">时间窗口的大小</param>
    ''' <returns>
    ''' 返回空值表示ND结果
    ''' </returns>
    <Extension>
    Private Function findPeakWithRtRange(target As IonPair, ROIData As ROI(), timeWindowSize#) As ROI
        ' 20200304
        ' System.InvalidOperationException: Nullable object must have a value.
        Dim find As DoubleRange = {
            CDbl(target.rt) - timeWindowSize,
            CDbl(target.rt) + timeWindowSize
        }
        Dim region As ROI = ROIData _
           .Where(Function(r)
                      Return find.IsInside(r.rt)
                      ' Return isContactWith(r.time, find)
                  End Function) _
           .OrderByDescending(Function(r)
                                  ' 20200309 因为噪声的积分面积可能会大于目标物质峰
                                  ' 的面积，所以在这里应该是使用峰高进行ROI的排序操
                                  ' 作
                                  Return stdNum.Log10(r.maxInto)
                              End Function) _
           .FirstOrDefault

        Return region
    End Function

    ''' <summary>
    ''' 这个是针对有保留时间漂移的情况
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <param name="ROIData"></param>
    ''' <param name="timeWindowSize#"></param>
    ''' <returns></returns>
    <Extension>
    Private Function findPeakWithRtRange(ion As IsomerismIonPairs, ROIData As ROI(), timeWindowSize#) As ROI
        Dim ionOrders = ion.OrderBy(Function(i) i.rt).ToArray
        Dim peakOrders = ROIData.OrderBy(Function(r) r.rt).ToArray
        Dim rt As Double
        Dim dt As Double
        Dim index As i32
        Dim peakIndex As Integer()
        Dim rt_alignments As New List(Of Integer())

        ' 计算保留时间漂移
        For pi As Integer = 0 To peakOrders.Length - 1
            peakIndex = New Integer(ionOrders.Length - 1) {}

            For i As Integer = 0 To peakIndex.Length - 1
                peakIndex(i) = -1
            Next

            rt = peakOrders(pi).rt
            dt = ionOrders(Scan0).rt - rt
            index = 1
            peakIndex(Scan0) = pi

            For Each target In ionOrders.Skip(1)
                For j As Integer = pi + 1 To peakOrders.Length - 1
                    ' dt1 - dt2 <= tolerance
                    If stdNum.Abs((CDbl(target.rt) - peakOrders(j).rt) - dt) <= timeWindowSize Then
                        peakIndex(CInt(++index) - 1) = j
                    End If
                Next
            Next

            If peakIndex.Count(Function(x) x >= 0) >= 1 Then
                rt_alignments.Add(peakIndex)
            End If
        Next

        If rt_alignments.Count > 0 Then
            With rt_alignments _
                .OrderByDescending(Function(r)
                                       Return r.Count(Function(x) x >= 0)
                                   End Function) _
                .First

                If DirectCast(.GetValue(ion.index), Integer) = -1 Then
                    Return Nothing
                Else
                    Return peakOrders(.GetValue(ion.index))
                End If
            End With
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 因为当前的<paramref name="ROIData"/>都是当前的这个离子对的结果数据
    ''' 所以在这里直接返回最大的峰高度的那个结果？
    ''' </summary>
    ''' <param name="ROIData"></param>
    ''' <returns></returns>
    <Extension>
    Private Function findPeakWithoutRtRange(ROIData As ROI()) As ROI
        Return ROIData _
            .OrderByDescending(Function(r)
                                   ' 这个积分值只是用来查找最大的峰面积的ROI区域
                                   ' 并不是最后的峰面积结果
                                   ' 还需要在下面的代码之中做峰面积积分才可以得到最终的结果
                                   Return r.maxInto
                               End Function) _
            .First
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <param name="vector"></param>
    ''' <param name="ROIData">The largest ROI is the first element.</param>
    ''' <param name="baselineQuantile#"></param>
    ''' <param name="peakAreaMethod"></param>
    ''' <param name="integratorTicks%"></param>
    ''' <param name="TPAFactor#"></param>
    ''' <returns></returns>
    <Extension>
    Private Function ProcessingIonPeakArea(ion As IonChromatogram, vector As IVector(Of ChromatogramTick), ROIData As ROI(),
                                           baselineQuantile#,
                                           peakAreaMethod As PeakAreaMethods,
                                           integratorTicks%,
                                           TPAFactor#,
                                           timeWindowSize#,
                                           bsplineDensity%,
                                           bsplineDegree%,
                                           timeshiftMethod As Boolean) As IonTPA

        Dim ionTarget As IsomerismIonPairs = ion.ion
        Dim region As ROI = Nothing
        Dim data As (area#, baseline#, maxPeakHeight#)
        Dim peak As DoubleRange

        If ionTarget.hasIsomerism Then
            'If ionTarget _
            '    .Where(Function(i)
            '               Return Not ROIData.Where(Function(r) stdNum.Abs(CDbl(i.rt) - r.rt) <= timeWindowSize).FirstOrDefault Is Nothing
            '           End Function) _
            '    .Count > 1 Then
            If Not timeshiftMethod Then
                region = ionTarget.target.findPeakWithRtRange(ROIData, timeWindowSize)
            Else
                region = ionTarget.findPeakWithRtRange(ROIData, timeWindowSize)
            End If
        Else
            If ionTarget.target.rt Is Nothing Then
                region = ROIData.findPeakWithoutRtRange
            Else
                region = ionTarget.target.findPeakWithRtRange(ROIData, timeWindowSize)
            End If
        End If

        If region Is Nothing Then
            Return New IonTPA With {
                .name = ion.name,
                .peakROI = New DoubleRange(0, 0)
            }
        Else
            peak = region.time
        End If

        With vector.TPAIntegrator(
            peak, baselineQuantile, peakAreaMethod,
            resolution:=integratorTicks,
            bsplineDegree:=bsplineDegree,
            bsplineDensity:=bsplineDensity,
            TPAFactor:=TPAFactor
        )
            data = (.Item1, .Item2, .Item3)
        End With

        Return New IonTPA With {
            .name = ion.name,
            .peakROI = peak,
            .area = If(data.area < 0, 0, data.area),
            .baseline = data.baseline,
            .maxPeakHeight = data.maxPeakHeight,
            .rt = region.rt
        }
    End Function

    ''' <summary>
    ''' 对某一个色谱区域进行峰面积的积分计算
    ''' </summary>
    ''' <param name="vector">完整的色谱图，计算基线的时候会需要使用到的</param>
    ''' <param name="peak">
    ''' 目标峰的时间范围
    ''' </param>
    ''' <param name="baselineQuantile">推荐0.65</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function TPAIntegrator(vector As IVector(Of ChromatogramTick),
                                  peak As DoubleRange,
                                  baselineQuantile As Double,
                                  Optional peakAreaMethod As PeakAreaMethods = PeakAreaMethods.Integrator,
                                  Optional resolution% = 5000,
                                  Optional bsplineDensity% = 100,
                                  Optional bsplineDegree% = 2,
                                  Optional TPAFactor# = 1) As (area#, baseline#, maxPeakHeight#)
        Dim area#, baseline#

        If vector.Length = 1 Then
            ' GCMS quantification by ion max intensity
            Return (vector(Scan0).Intensity, 0, vector(Scan0).Intensity)
        Else
            ' baseline = vector.Baseline(quantile:=baselineQuantile)
            baseline = (vector.First.Intensity + vector.Last.Intensity) / 2
        End If

        Select Case peakAreaMethod
            Case PeakAreaMethods.NetPeakSum
                area = vector.PeakArea
            Case PeakAreaMethods.SumAll
                area = vector.SumAll
            Case PeakAreaMethods.MaxPeakHeight
                area = vector.MaxPeakHeight
            Case Else
                ' 默认是使用积分器方法
                area = vector.PeakAreaIntegrator(
                    peak:=peak,
                    baselineQuantile:=baselineQuantile,
                    resolution:=resolution,
                    bsplineDegree:=bsplineDegree,
                    bsplineDensity:=bsplineDensity
                )
        End Select

        area *= TPAFactor

        Return (area, baseline, vector.PickArea(range:=peak).MaxPeakHeight)
    End Function
End Module
