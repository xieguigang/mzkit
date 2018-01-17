Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra

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
        Return $"[{Time}, {Intensity}]"
    End Function
End Class