Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Math.Scripting
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
    <DataFrameColumn(NameOf(mz))>
    Public Property mz As Double
    ''' <summary>
    ''' quantity
    ''' </summary>
    ''' <returns></returns>
    <DataFrameColumn(NameOf(quantity))>
    Public Property quantity As Double
    ''' <summary>
    ''' Relative intensity.(percentage) 
    ''' </summary>
    ''' <returns></returns>
    <DataFrameColumn(NameOf(intensity))>
    Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return $"{mz} ({intensity * 100%}%)"
    End Function
End Class

''' <summary>
''' The <see cref="ms2"/> library matrix
''' </summary>
Public Class LibraryMatrix : Inherits IVector(Of ms2)
    Implements INamedValue

    ''' <summary>
    ''' The list of molecular fragment
    ''' </summary>
    ''' <returns></returns>
    Public Property ms2 As ms2()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return buffer
        End Get
        Set(value As ms2())
            Call writeBuffer(value)
        End Set
    End Property

    Public Property Name As String Implements IKeyedEntity(Of String).Key

    Default Public Overloads ReadOnly Property Item(booleans As IEnumerable(Of Boolean)) As LibraryMatrix
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New LibraryMatrix() With {
                .ms2 = MyBase _
                    .Item(booleans) _
                    .ToArray,
                .Name = Name
            }
        End Get
    End Property

    Sub New()
        Call MyBase.New({})
    End Sub

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

    ''' <summary>
    ''' 将一个整形数列表转换为mz向量，这个转换函数为调试用的
    ''' </summary>
    ''' <param name="mzlist"></param>
    ''' <returns></returns>
    Public Shared Widening Operator CType(mzlist As Integer()) As LibraryMatrix
        Return New LibraryMatrix With {
            .ms2 = mzlist _
                .Select(Function(mz)
                            Return New ms2 With {
                                .mz = mz,
                                .intensity = 1,
                                .quantity = 1
                            }
                        End Function) _
                .ToArray
        }
    End Operator

    ''' <summary>
    ''' Library matrix to ``&lt;m/z, intensity>`` tuples.
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Shared Narrowing Operator CType(matrix As LibraryMatrix) As (mz#, into#)()
        Return matrix.ms2 _
            .Select(Function(r) (r.mz, r.intensity)) _
            .ToArray
    End Operator
End Class

''' <summary>
''' Library matrix math
''' </summary>
Public Module LibraryMatrixExtensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Max(matrix As LibraryMatrix) As Double
        Return matrix.ms2.Max(Function(r) r.quantity)
    End Function
End Module