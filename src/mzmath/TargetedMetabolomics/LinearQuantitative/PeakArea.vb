#Region "Microsoft.VisualBasic::c71caf5674aada4d43316b39148e3aa4, TargetedMetabolomics\LinearQuantitative\PeakArea.vb"

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

    '     Module PeakArea
    ' 
    '         Function: MaxPeakHeight, PeakArea, PeakAreaIntegrator, PickArea, Simpson
    '                   SumAll
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Calculus
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Scripting
Imports stdNum = System.Math


Namespace LinearQuantitative

    Public Module PeakArea

        ''' <summary>
        ''' ``B + A = S``
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="peak">The time range of the peak</param>
        ''' <param name="baseline">The quantile threshold of the baseline</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 简单的净峰法计算出峰面积
        ''' </remarks>
        <Extension>
        Public Function PeakArea(chromatogram As IVector(Of ChromatogramTick), peak As DoubleRange, Optional baseline# = 0.65) As Double
            Dim S = chromatogram.PickArea(range:=peak) ' TPA
            Dim B = chromatogram.Baseline(quantile:=baseline)

            ' 2018-4-10 不需要减去基线？？？
            ' B = 0

            ' 2018-1-18 
            ' 下面的聚合表达式只会计算去除本底之后的信号量大于零的信号量的和
            ' 取大于零是为了解决类似于 Homocysteine_chromatogram.png 这类基线过高的问题
            ' 因为可能峰检测可能会将本底的部分也计算在内，在这些额外被计算在内的基线信号之中，由于有些信号量低于baseline基线的，所以会出现负值
            ' 很明显这个是不需要的
            Dim A = Aggregate signal As ChromatogramTick
                    In S
                    Let PA = signal.Intensity - B
                    Where PA > 0
                    Into Sum(PA)

            Return A
        End Function

        ''' <summary>
        ''' Gets all signals that its chromatogram time inside the peak time range
        ''' time >= time range min andalso time &lt;= time range max 
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="range"></param>
        ''' <returns></returns>
        <Extension>
        Public Function PickArea(chromatogram As IVector(Of ChromatogramTick), range As DoubleRange) As IVector(Of ChromatogramTick)
            Dim time As Vector = chromatogram!Time
            Dim region = chromatogram((time >= range.Min) & (time <= range.Max))

            Return region
        End Function

        <Extension>
        Public Function SumAll(chromatogram As IVector(Of ChromatogramTick)) As Double
            Return Aggregate t As ChromatogramTick
                   In chromatogram
                   Into Sum(t.Intensity)
        End Function

        ''' <summary>
        ''' 这个函数返回所给定的色谱图区域之中的最大的峰高的数值
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function MaxPeakHeight(chromatogram As IVector(Of ChromatogramTick)) As Double
            Return chromatogram!intensity.Max
        End Function

        ''' <summary>
        ''' 使用积分器来进行峰面积的精确计算：
        ''' 
        ''' 1. 首先对峰的线条进行插值计算
        ''' 2. 然后进行定积分计算，计算值的时候也是使用``A = S - B``净峰法来计算出面积。
        ''' 
        ''' 最后的积分计算结果就是峰面积
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="peak"></param>
        ''' <param name="baselineQuantile#"></param>
        ''' <param name="resolution">进行积分计算的时候的精密度，精密度越大越好，但是计算时间会被相应的加长</param>
        ''' <returns></returns>
        <Extension>
        Public Function PeakAreaIntegrator(chromatogram As IVector(Of ChromatogramTick),
                                           peak As DoubleRange,
                                           Optional baselineQuantile# = 0.65,
                                           Optional resolution% = 1000,
                                           Optional bsplineDensity% = 100,
                                           Optional bsplineDegree% = 2,
                                           ByRef Optional peakRaw As PointF() = Nothing,
                                           ByRef Optional curve As PointF() = Nothing) As Double

            Dim rawPoints As List(Of PointF) = chromatogram _
                .PickArea(range:=peak) _
                .Select(Function(c)
                            Return New PointF With {
                                .X = CSng(c.Time),
                                .Y = CSng(c.Intensity)
                            }
                        End Function) _
                .AsList

            ' CubicSpline required at least 3 points
            If rawPoints = 2 Then
                If rawPoints(0).Y > rawPoints(1).Y Then
                    ' \
                    Dim t0 = rawPoints(0)
                    Dim i% = chromatogram _
                        .Which(Function(t)
                                   Return stdNum.Abs(t0.X - t.Time) <= 0.1
                               End Function)(0)

                    With chromatogram(i - 1)
                        rawPoints = New PointF(CSng(.Time), CSng(.Intensity)) + rawPoints
                    End With
                Else
                    ' /
                    Dim t1 = rawPoints(1)
                    Dim i% = chromatogram _
                        .Which(Function(t)
                                   Return stdNum.Abs(t1.X - t.Time) <= 0.1
                               End Function)(0)

                    With chromatogram(i + 1)
                        rawPoints += New Point(CInt(.Time), CInt(.Intensity))
                    End With
                End If
            End If

            Dim points As PointF() = B_Spline.BSpline(rawPoints, bsplineDegree, bsplineDensity).ToArray
            Dim baseline# = chromatogram.Baseline(baselineQuantile)
            Dim windows = points.SlideWindows(2).ToArray
            Dim p As i32 = 0
            Dim current As DoubleRange = Nothing
            Dim tangent As fx = Nothing
            Dim moveNext = Sub()
                               With windows(++p)
                                   current = .X.Range
                                   tangent = .Tangent
                               End With
                           End Sub

            Call moveNext()

            curve = points
            peakRaw = rawPoints.ToArray

            ' 2018-4-10 不需要减去基线？？
            ' baseline = 0

            ' 积分函数
            Dim ft As df = Function(x#, y#) As Double
                               ' 先根据x找出time range
                               ' 然后计算出Y
                               ' 最后减去基线值，既得最终的Y结果值
                               If Not current.IsInside(x) Then
                                   Call moveNext()
                               End If

                               y = tangent(x) '- baseline

                               Return y
                           End Function

            ' 面积积分器
            Dim integrator As New ODE With {
                .df = ft,
                .ID = MethodBase.GetCurrentMethod.Name,
                .y0 = points.First.Y
            }

            Dim area As ODEOutput = integrator.RK4(n:=resolution, a:=points.First.X, b:=points.Last.X)
            Dim areaValue = area.Y.vector.Last

            Return areaValue
        End Function

        ''' <summary>
        ''' https://en.wikipedia.org/wiki/Simpson%27s_rule
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function Simpson(a#, b#, f As fx) As Double
            Return (b - a) / 6 * (f(a) + 4 * f((a + b) / 2) + f(b))
        End Function
    End Module
End Namespace
