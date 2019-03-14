Imports System.Runtime.CompilerServices
Imports sys = System.Math

Namespace Ms1

    ''' <summary>
    ''' Mass tolerance in delta mass error
    ''' </summary>
    Public Class DAmethod : Inherits Tolerance

        Sub New(Optional da# = 0.3)
            Threshold = da
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function Assert(mz1 As Double, mz2 As Double) As Boolean
            Return sys.Abs(mz1 - mz2) <= Threshold
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function AsScore(mz1 As Double, mz2 As Double) As Double
            Return 1 - (sys.Abs(mz1 - mz2) / Threshold)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function MassError(mz1 As Double, mz2 As Double) As Double
            Return sys.Abs(mz1 - mz2)
        End Function

        Public Overrides Function ToString() As String
            Return $"|mz1 - mz2| <= {Threshold}"
        End Function
    End Class
End Namespace