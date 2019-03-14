Imports System.Runtime.CompilerServices
Imports sys = System.Math

Namespace Ms1

    ''' <summary>
    ''' PPM tolerance calculator
    ''' </summary>
    Public Class PPMmethod : Inherits Tolerance

        Sub New()
        End Sub

        Sub New(ppm#)
            Threshold = ppm
        End Sub

        ''' <summary>
        ''' 分子量差值
        ''' </summary>
        ''' <param name="measured#"></param>
        ''' <param name="actualValue#"></param>
        ''' <returns></returns>
        Public Overloads Shared Function ppm(measured#, actualValue#) As Double
            ' （测量值-实际分子量）/ 实际分子量
            ' |(实验数据 - 数据库结果)| / 实验数据 * 1000000
            Dim ppmd# = sys.Abs(measured - actualValue) / actualValue
            ppmd = ppmd * 1000000
            Return ppmd
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function Assert(mz1 As Double, mz2 As Double) As Boolean
            Return ppm(mz1, mz2) <= Threshold
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function AsScore(mz1 As Double, mz2 As Double) As Double
            Return 1 - (ppm(mz1, mz2) / Threshold)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function MassError(mz1 As Double, mz2 As Double) As Double
            Return ppm(mz1, mz2)
        End Function

        Public Overrides Function ToString() As String
            Return $"ppm(mz1, mz2) <= {Threshold}"
        End Function
    End Class
End Namespace