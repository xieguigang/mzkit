Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Library matrix math
''' </summary>
Public Module LibraryMatrixExtensions

    ''' <summary>
    ''' MAX(<see cref="ms2.quantity"/>)
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Max(matrix As LibraryMatrix) As Double
        Return matrix.ms2.Max(Function(r) r.quantity)
    End Function

    ''' <summary>
    ''' 将符合误差范围的二级碎片合并在一起
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Shrink(matrix As LibraryMatrix, tolerance As Tolerance) As LibraryMatrix
        Return matrix.GroupBy(Function(ms2) ms2.mz, AddressOf tolerance.Assert, False) _
                     .Select(Function(g)
                                 Return g(Which.Max(g.Select(Function(m) m.intensity)))
                             End Function) _
                     .ToArray
    End Function
End Module