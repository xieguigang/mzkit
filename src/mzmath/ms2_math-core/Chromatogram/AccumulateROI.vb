#Region "Microsoft.VisualBasic::6be5bdc49559431d1fde16ca0dc07057, src\mzmath\ms2_math-core\Chromatogram\AccumulateROI.vb"

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

    '     Module AccumulateROI
    ' 
    '         Function: getAccumulateLine, PopulateROI, SplitGCMSPeaks, SplitMRMPeaks
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting

Namespace Chromatogram

    ''' <summary>
    ''' 根据累加线来查找色谱峰的ROI
    ''' </summary>
    Public Module AccumulateROI

        ' 算法原理，每当出现一个峰的时候，累加线就会明显升高一个高度
        ' 当升高的时候，曲线的斜率大于零
        ' 当处于基线水平的时候，曲线的斜率接近于零
        ' 则可以利用这个特性将色谱峰给识别出来
        ' 这个方法仅局限于色谱峰都是各自相互独立的情况之下

        <Extension>
        Private Function getAccumulateLine(chromatogram As IVector(Of ChromatogramTick), baseline#) As PointF()
            Dim accumulate#
            Dim sumALL# = (chromatogram!intensity - baseline) _
                .Where(Function(x) x > 0) _
                .Sum
            Dim ay = Function(into As Double) As Double
                         into -= baseline
                         accumulate += If(into < 0, 0, into)
                         Return (accumulate / sumALL) * 100 ' * maxInto
                     End Function
            Dim accumulateLine = chromatogram _
                .Select(Function(tick)
                            Return New PointF(tick.Time, ay(tick.Intensity))
                        End Function) _
                .ToArray

            Return accumulateLine
        End Function

        ''' <summary>
        ''' 在这个函数之中，只是查找出了色谱峰的时间范围，但是并未对峰面积做积分计算
        ''' </summary>
        ''' <param name="angleThreshold#">区分色谱峰的累加线切线角度的阈值，单位为度</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个方法对于MRM的数据的处理结果比较可靠，但是对于GCMS的实验数据，
        ''' 由于GCMS实验数据的峰比较窄，这个函数不太适合处理GCMS的峰
        ''' </remarks>
        <Extension>
        Public Iterator Function PopulateROI(chromatogram As IVector(Of ChromatogramTick),
                                             Optional angleThreshold# = 5,
                                             Optional baselineQuantile# = 0.65,
                                             Optional MRMpeaks As Boolean = True) As IEnumerable(Of ROI)
            ' 先计算出基线和累加线
            Dim baseline# = chromatogram.Baseline(baselineQuantile)
            Dim time As Vector = chromatogram!time
            Dim intensity As Vector = chromatogram!intensity
            ' Dim maxInto# = intensity.Max - baseline
            ' 所有的基线下面的噪声加起来的积分面积总和
            Dim sumAllNoise# = baseline * chromatogram.Length
            ' 使用滑窗计算出切线的斜率
            Dim windows As SlideWindow(Of PointF)() =
                chromatogram _
                .getAccumulateLine(baseline) _
                .SlideWindows(winSize:=2) _
                .ToArray
            Dim peaks As IEnumerable(Of SlideWindow(Of PointF)())

            ' 2018-11-26
            ' 目前暂时使用一样的方法进行MRM和GCMS的峰的解析操作
            If MRMpeaks Then
                peaks = windows.SplitMRMPeaks(angleThreshold)
            Else
                ' peaks = windows.SplitGCMSPeaks(angleThreshold)
                peaks = windows.SplitMRMPeaks(angleThreshold)
            End If

            For Each window As SlideWindow(Of PointF)() In peaks
                Dim rtmin# = Fix(window.First()(0).X)
                Dim rtmax# = window.Last()(-1).X + 1
                Dim peak = chromatogram((time >= rtmin) & (time <= rtmax))
                ' 因为Y是累加曲线的值，所以可以近似的看作为峰面积积分值
                ' 在这里将区间的上限的积分值减去区间的下限的积分值即可得到当前的这个区间的积分值（近似于定积分）
                Dim integration = window.Last.Last.Y - window.First.First.Y
                Dim max#
                Dim rt#

                If peak.Length = 1 Then
                    With peak.First
                        max = .Intensity
                        rt = .Time
                    End With
                Else
                    With peak!intensity
                        max = .Max
                        rt = peak(index:=Which.Max(.ByRef)).Time
                    End With
                End If

                Yield New ROI With {
                    .Ticks = peak.ToArray,
                    .MaxInto = max,
                    .Baseline = baseline,
                    .Time = {rtmin, rtmax},
                    .Integration = integration,
                    .rt = rt,
                    .Noise = (peak.Length * baseline) / sumAllNoise
                }
            Next
        End Function

        ''' <summary>
        ''' 因为MRM只有一个峰，所以在这里按照切线角度小于角度阈值来进行切割，产生峰的ROI区域
        ''' </summary>
        ''' <param name="windows"></param>
        ''' <param name="angleThreshold#"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function SplitMRMPeaks(windows As SlideWindow(Of PointF)(), angleThreshold#) As IEnumerable(Of SlideWindow(Of PointF)())
            Return windows _
                .Split(Function(tangent)
                           Return (tangent.First, tangent.Last).Angle <= angleThreshold
                       End Function) _
                .Where(Function(p) p.Length > 1)
        End Function

        ''' <summary>
        ''' 因为GCMS的峰比较窄，所以在这里将所有相邻的，切线角度大于目标角度阈值的区域取出来
        ''' 产生峰的ROI区域
        ''' </summary>
        ''' <param name="windows"></param>
        ''' <param name="angleThreshold#"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function SplitGCMSPeaks(windows As SlideWindow(Of PointF)(), angleThreshold#) As IEnumerable(Of SlideWindow(Of PointF)())
            Return windows _
                .SplitMRMPeaks(angleThreshold) _
                .Select(Function(primaryRegion)
                            Return windows.SplitMRMPeaks(angleThreshold)
                        End Function) _
                .IteratesALL
        End Function
    End Module
End Namespace
