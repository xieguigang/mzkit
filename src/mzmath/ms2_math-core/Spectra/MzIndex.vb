
Imports System.Runtime.CompilerServices

Namespace Spectra

    ''' <summary>
    ''' represents the ion m/z as a index
    ''' </summary>
    Public Class MzIndex

        Public Property mz As Double

        ''' <summary>
        ''' the index value
        ''' </summary>
        ''' <returns></returns>
        Public Property index As Integer

        Sub New()
        End Sub

        Sub New(mz As Double, Optional index As Integer = 0)
            Me.mz = mz
            Me.index = index
        End Sub

        ''' <summary>
        ''' get the fallback tuple data
        ''' </summary>
        ''' <returns></returns>
        Public Function Tuple() As (mz As Double, Integer)
            Return (mz, index)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{index}] {mz.ToString}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Widening Operator CType(mzVal As (mz As Double, index As Integer)) As MzIndex
            Return New MzIndex(mzVal.mz, mzVal.index)
        End Operator

        ''' <summary>
        ''' calculate the binary data file offset
        ''' </summary>
        ''' <param name="sizeof"></param>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Shared Operator *(sizeof As Integer, index As MzIndex) As Integer
            Return sizeof * index.index
        End Operator

    End Class

End Namespace