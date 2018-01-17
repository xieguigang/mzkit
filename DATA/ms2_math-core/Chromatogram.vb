Imports System.Runtime.CompilerServices
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
    ''' <returns></returns>
    <Extension>
    Public Function MRMPeak(chromatogram As VectorModel(Of ChromatogramTick), Optional theshold# = 3, Optional winSize% = 3) As DoubleRange
        ' 先找到最高的信号，然后逐步分别往两边延伸
        ' 直到下降的速率小于阈值
        ' 因为MRM方法只出一个峰

        '     A
        '    /|
        '   / |
        '  /  |
        ' /   |
        '------
        't0   t1
        ' 
        Dim maxIndex = Which.Max(chromatogram!Intensity)

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