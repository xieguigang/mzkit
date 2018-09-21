Imports System.Runtime.CompilerServices
Imports sys = System.Math

''' <summary>
''' The m/z tolerance methods
''' </summary>
Public MustInherit Class Tolerance

    Public ReadOnly Property [Interface] As Tolerance
        Get
            Return Me
        End Get
    End Property

    Public MustOverride Function Assert(mz1#, mz2#) As Boolean

    Public Shared Function DeltaMass(da#) As DAmethod
        Return New DAmethod(da)
    End Function

    Public Shared Function PPM(value#) As PPMmethod
        Return New PPMmethod(value)
    End Function

    ''' <summary>
    ''' 将当前的质量值减去给定的ppm质量差
    ''' </summary>
    ''' <param name="mass#"></param>
    ''' <param name="ppm#"></param>
    ''' <returns></returns>
    Public Shared Function SubPPM(mass#, ppm#) As Double
        Dim ppmd As Double = ppm / 1000000
        ' sys.Abs(measured - actualValue) 
        ppmd = ppmd * mass
        mass = mass - ppmd

        Return mass
    End Function

    ''' <summary>
    ''' 将当前的质量值加上给定的ppm质量差
    ''' </summary>
    ''' <param name="mass#"></param>
    ''' <param name="ppm#"></param>
    ''' <returns></returns>
    Public Shared Function AddPPM(mass#, ppm#) As Double
        Dim ppmd As Double = ppm / 1000000
        ' sys.Abs(measured - actualValue) 
        ppmd = ppmd * mass
        mass = mass + ppmd

        Return mass
    End Function
End Class

Public Class PPMmethod : Inherits Tolerance

    Public Property ppmValue As Double

    Sub New()
    End Sub

    Sub New(ppm#)
        ppmValue = ppm
    End Sub

    Public Overrides Function ToString() As String
        Return $"ppm(mz1, mz2) <= {ppmValue}"
    End Function

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
        Return ppm(mz1, mz2) <= ppmValue
    End Function
End Class

Public Class DAmethod : Inherits Tolerance

    Public Property da As Double

    Sub New(Optional da# = 0.3)
        Me.da = da
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function Assert(mz1 As Double, mz2 As Double) As Boolean
        Return sys.Abs(mz1 - mz2) <= da
    End Function

    Public Overrides Function ToString() As String
        Return $"|mz1 - mz2| <= {da}"
    End Function
End Class