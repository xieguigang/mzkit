Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
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

    ''' <summary>
    ''' Returns time range for the peak
    ''' </summary>
    ''' <param name="chromatogram">应该是按照时间升序排序了的</param>
    ''' <param name="threshold">Unit in degree</param>
    ''' <returns></returns>
    <Extension>
    Public Function MRMPeak(chromatogram As VectorModel(Of ChromatogramTick), Optional threshold# = 3, Optional winSize% = 3) As DoubleRange

        ' 先找到最高的信号，然后逐步分别往两边延伸
        ' 直到下降的速率小于阈值
        ' 因为MRM方法只出一个峰

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
        Dim maxIndex = Which.Max(chromatogram!Intensity)
        Dim timeRange#()

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
        Dim windows = chromatogram.SlideWindows(winSize).ToArray

        For Each block As SlideWindow(Of ChromatogramTick) In windows

            Dim t10 = block.Shadows!Time.Range.Length
            Dim t1 = block.Last
            Dim t0 = block.First
            Dim A = Abs(t0.Intensity - t1.Intensity)

            Dim C = (t0.Time, A).Distance(t1.Time, 0R)
            Dim cos# = t10 / C

            If cos <= threshold Then
                Return t1.Time
            End If
        Next

        Return chromatogram.Last.Time
    End Function
End Module

Public Class ChromatogramTick

    Public Property Time As Double
    Public Property Intensity As Double

    Sub New()
    End Sub

    Sub New(time#, into#)
        Me.Time = time
        Me.Intensity = into
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Intensity}@{Time}s"
    End Function
End Class