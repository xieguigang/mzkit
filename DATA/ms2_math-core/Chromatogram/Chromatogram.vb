Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.Scripting

Public Module Chromatogram

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TimeArray(chromatogram As IEnumerable(Of ChromatogramTick)) As Vector
        Return chromatogram.Select(Function(c) c.Time).AsVector
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function IntensityArray(chromatogram As IEnumerable(Of ChromatogramTick)) As Vector
        Return chromatogram.Select(Function(c) c.Intensity).AsVector
    End Function

    <Extension>
    Public Function Base(chromatogram As IEnumerable(Of ChromatogramTick), Optional quantile# = 0.65) As Double
        Dim q As QuantileEstimationGK = chromatogram.Shadows!Intensity.GKQuantile
        Dim baseValue = q.Query(quantile)

        Return baseValue
    End Function

    ''' <summary>
    ''' Returns time range for the peak
    ''' </summary>
    ''' <param name="chromatogram">应该是按照时间升序排序了的</param>
    ''' <param name="threshold">Unit in degree, values in range ``[0-90]``</param>
    ''' <returns></returns>
    <Extension>
    Public Function MRMPeak(chromatogram As VectorModel(Of ChromatogramTick), Optional threshold# = 45, Optional winSize% = 5) As DoubleRange

        ' 先找到最高的信号，然后逐步分别往两边延伸
        ' 直到下降的速率小于阈值
        ' 因为MRM方法在一个色谱之中只出一个峰，所以在这里仅仅实现一个非常简单的峰的边界检测的算法

        Dim maxIndex
        Dim timeRange#()

        ' 2018-1-18 如果事先将基线移除的话，或导致峰的范围扩大
        ' 取消掉
        ' removes all of the ticks that intensity value less than baseline.
        ' chromatogram = chromatogram(chromatogram!Intensity >= chromatogram.Base(baselineQuantile))
        maxIndex = Which.Max(chromatogram!Intensity)

        With chromatogram.ToArray

            threshold = Cos(threshold.ToRadians)
            ' split
            timeRange = {
                .Take(maxIndex).Reverse.ToArray.MakeExtension(threshold, winSize),   ' left
                .Skip(maxIndex).ToArray.MakeExtension(threshold, winSize)            ' right
            }
        End With

        Return New DoubleRange(timeRange)
    End Function

    ''' <summary>
    ''' t1 -> t2
    ''' </summary>
    ''' <param name="chromatogram"></param>
    ''' <param name="threshold#">cos value</param>
    ''' <param name="winSize%"></param>
    ''' <returns></returns>
    <Extension>
    Private Function MakeExtension(chromatogram As ChromatogramTick(), threshold#, winSize%) As Double
        Dim vector = chromatogram.Shadows
        Dim timeRange As DoubleRange = vector!Time
        Dim normInto As Vector = vector!Intensity _
            .RangeTransform(timeRange) _
            .AsVector
        Dim windows = chromatogram _
            .Select(Function(c, i)
                        Return New ChromatogramTick With {
                            .Time = c.Time,
                            .Intensity = normInto(i)
                        }
                    End Function) _
            .SlideWindows(winSize) _
            .ToArray

        For Each block As SlideWindow(Of ChromatogramTick) In windows

            '      A
            '     /|
            '    / |
            '   /  |
            '  /   |
            ' ------
            ' t0   t1
            ' 
            ' cos(threshold) = (t1 - t0) / ( distance((t0, 0), (t1, A)) )
            '

            Dim t10 = block.Shadows!Time.Range.Length
            Dim t1 = block.First
            Dim t0 = block.Last
            Dim A = Abs(t0.Intensity - t1.Intensity)

            Dim C = (t1.Time, A).Distance(t0.Time, 0R)
            Dim cos# = t10 / C

            If threshold <= cos Then
                Return t0.Time
            End If
        Next

        ' using the entire area???
        Return chromatogram.Last.Time
    End Function
End Module