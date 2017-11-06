Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Library

    Public Property ID As String
    Public Property PrecursorMz As String
    Public Property ProductMz As String
    Public Property LibraryIntensity As String
    Public Property Name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class ms2
    Public Property mz As Double
    Public Property intensity As Double
End Class

Public Class LibraryMatrix

    Public Property ms2 As ms2()

    Public Shared Operator /(matrix As LibraryMatrix, x#) As LibraryMatrix
        For Each ms2 As ms2 In matrix.ms2
            ms2.intensity /= x
        Next

        Return matrix
    End Operator

    Public Shared Operator *(matrix As LibraryMatrix, x#) As LibraryMatrix
        For Each ms2 As ms2 In matrix.ms2
            ms2.intensity *= x
        Next

        Return matrix
    End Operator

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Widening Operator CType(ms2 As ms2()) As LibraryMatrix
        Return New LibraryMatrix With {
            .ms2 = ms2
        }
    End Operator

    Public Shared Narrowing Operator CType(matrix As LibraryMatrix) As (mz#, into#)()
        Return matrix.ms2.Select(Function(r) (r.mz, r.intensity)).ToArray
    End Operator
End Class

Public Module LibraryMatrixExtensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Max(matrix As LibraryMatrix) As Double
        Return matrix.ms2.Max(Function(r) r.intensity)
    End Function
End Module