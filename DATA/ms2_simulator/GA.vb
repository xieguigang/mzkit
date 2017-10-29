Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math

''' <summary>
''' The genetic algorithm core for the simulator.
''' </summary>
Module GA

    ''' <summary>
    ''' xy分别为预测或者标准品的结果数据，无顺序之分
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ms2Alignment(x As ms2(), y As ms2(), method As AlignMethod) As (forward#, reverse#)
        Return (GA.Align(x, y, method), GA.Align(y, x, method))
    End Function

    ''' <summary>
    ''' 以<paramref name="ref"/>为基准，从<paramref name="query"/>之中选择出对应的<see cref="ms2.mz"/>信号响应信息，完成对齐操作
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="ref"></param>
    ''' <returns></returns>
    Public Function Align(query As ms2(), ref As ms2(), method As AlignMethod) As Double
        Dim q As Vector = query.AlignMatrix(ref, method).Shadows!intensity
        Dim s As Vector = ref.Shadows!intensity

        Return SSM(q / q.Max, s / s.Max)
    End Function

    <Extension>
    Public Function AlignMatrix(query As ms2(), ref As ms2(), method As AlignMethod) As ms2()
        Return ref _
            .Select(Function(mz)

                        ' 2017-10-29
                        '
                        ' 当找不到的时候，会返回一个空的structure对象，这个时候intensity为零
                        ' 所以在这个Linq表达式中，后面不需要使用Where来删除对象了

                        Return query _
                            .Where(Function(q) method.Assert(q.mz, mz.mz)) _
                            .FirstOrDefault
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 分子量差值
    ''' </summary>
    ''' <param name="measured#"></param>
    ''' <param name="actualValue#"></param>
    ''' <returns></returns>
    Public Function ppm(measured#, actualValue#) As Double
        ' （测量值-实际分子量）/ 实际分子量
        ' |(实验数据 - 数据库结果)| / 实验数据 * 1000000
        Dim ppmd# = Math.Abs(measured - actualValue) / actualValue
        ppmd = ppmd * 1000000
        Return ppmd
    End Function
End Module

Public MustInherit Class AlignMethod

    Public MustOverride Function Assert(mz1#, mz2#) As Boolean
End Class

Public Class PPMmethod : Inherits AlignMethod

    Public Property ppm As Double

    Public Overrides Function Assert(mz1 As Double, mz2 As Double) As Boolean
        Return GA.ppm(mz1, mz2) <= ppm
    End Function
End Class

Public Class DAmethod : Inherits AlignMethod

    Public Property da As Double

    Public Overrides Function Assert(mz1 As Double, mz2 As Double) As Boolean
        Return Math.Abs(mz1 - mz2) <= da
    End Function
End Class

Public Structure ms2

    Public Property mz As Double
    Public Property quantity As Double
    Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return $"{mz} ({intensity * 100%}%)"
    End Function
End Structure