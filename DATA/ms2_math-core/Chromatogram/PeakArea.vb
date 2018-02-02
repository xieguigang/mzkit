Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Interpolation
Imports Microsoft.VisualBasic.Math.Scripting

Namespace Chromatogram

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
            ' gets all signals that its chromatogram time inside the peak time range
            ' time >= time range min andalso time <= time range max 
            Dim time = chromatogram!Time
            Dim S = chromatogram((time >= peak.Min) & (time <= peak.Max))  ' TPA
            Dim B = chromatogram.Baseline(quantile:=baseline)

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
        ''' 使用积分器来进行峰面积的精确计算：
        ''' 
        ''' 1. 首先对峰的线条进行插值计算
        ''' 2. 然后进行定积分计算，计算值的时候也是使用 A = S - B 净峰法来计算出面积。
        ''' 
        ''' 最后的积分计算结果就是峰面积
        ''' </summary>
        ''' <param name="chromatogram"></param>
        ''' <param name="peak"></param>
        ''' <param name="baselineQuantile#"></param>
        ''' <returns></returns>
        <Extension>
        Public Function PeakAreaIntegrator(chromatogram As IVector(Of ChromatogramTick), peak As DoubleRange, Optional baselineQuantile# = 0.65) As Double
            Dim time = chromatogram!Time
            Dim points As PointF() =
                chromatogram((time >= peak.Min) & (time <= peak.Max)) _
                .Select(Function(c)
                            Return New PointF With {
                                .X = c.Time,
                                .Y = c.Intensity
                            }
                        End Function) _
                .CubicSpline _
                .ToArray
            Dim baseline# = chromatogram.Baseline(baselineQuantile)
            Dim windows = points.

        End Function
    End Module
End Namespace