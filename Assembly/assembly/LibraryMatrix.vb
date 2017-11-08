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

    ''' <summary>
    ''' Molecular fragment m/z
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double
    ''' <summary>
    ''' quantity
    ''' </summary>
    ''' <returns></returns>
    Public Property quantity As Double
    ''' <summary>
    ''' Relative intensity.(percentage) 
    ''' </summary>
    ''' <returns></returns>
    Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return $"{mz} ({intensity * 100%}%)"
    End Function
End Class

Public Class LibraryMatrix

    ''' <summary>
    ''' The list of molecular fragment
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As ms2()

    Public Shared Operator /(matrix As LibraryMatrix, x#) As LibraryMatrix
        For Each ms2 As ms2 In matrix.ms2
            ms2.intensity = ms2.quantity / x
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

''' <summary>
''' Library matrix math
''' </summary>
Public Module LibraryMatrixExtensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Max(matrix As LibraryMatrix) As Double
        Return matrix.ms2.Max(Function(r) r.quantity)
    End Function
End Module