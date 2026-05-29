Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Language.Java

Namespace Spectra

    ''' <summary>
    ''' the spectrum fragment matches method 
    ''' </summary>
    Public Module Matches

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sortMz">
        ''' data must be sorted by the <see cref="ms2.mz"/> fragment mass value in asc order.
        ''' </param>
        ''' <param name="mz"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <Extension>
        Public Function BinarySearch(sortMz As ms2(), mz As Double, tolerance As Tolerance) As ms2
            Return sortMz.BinarySearch(mz, New ComparesMz(tolerance))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sortMz">
        ''' data must be sorted by the <see cref="ms2.mz"/> fragment mass value in asc order.
        ''' </param>
        ''' <param name="mz"></param>
        ''' <param name="tolerance"></param>
        ''' <returns></returns>
        <Extension>
        Public Function BinarySearch(sortMz As ms2(), mz As Double, tolerance As ComparesMz) As ms2
            Dim i As Integer = Collections.binarySearch(sortMz, New ms2(mz, 0), tolerance)

            If i < 0 Then
                Return Nothing
            Else
                Return sortMz(i)
            End If
        End Function
    End Module

    Public Class ComparesMz : Implements IComparer(Of ms2)

        ReadOnly mzdiff As Tolerance

        Sub New(mzdiff As Tolerance)
            Me.mzdiff = mzdiff
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Compare(x As ms2, y As ms2) As Integer Implements IComparer(Of ms2).Compare
            Return mzdiff.Compares(x.mz, y.mz)
        End Function

        Public Overrides Function ToString() As String
            Return mzdiff.ToString
        End Function
    End Class
End Namespace