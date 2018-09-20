Imports System.Runtime.CompilerServices

Namespace MSMS

    Public Structure PeakMs2

        Dim mz As Double
        Dim rt As Double
        Dim file As String
        Dim scan As Integer
        Dim mzInto As LibraryMatrix

        Public Shared Function RtInSecond(rt As String) As Double
            rt = rt.Substring(2)
            rt = rt.Substring(0, rt.Length - 1)
            Return Double.Parse(rt)
        End Function

        ''' <summary>
        ''' 当前的这个<see cref="PeakMs2"/>如果在<paramref name="ref"/>找不到对应的``m/z``
        ''' 则对应的部分的into为零
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AlignMatrix(ref As ms2(), tolerance As Tolerance) As ms2()
            Return mzInto.ms2.AlignMatrix(ref, tolerance)
        End Function
    End Structure
End Namespace