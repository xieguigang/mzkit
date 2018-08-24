Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace MSMS

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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function AsMatrix(lib_ms2 As IEnumerable(Of Library)) As LibraryMatrix
            Return lib_ms2 _
                .Select(Function(l)
                            Return New ms2 With {
                                .mz = l.ProductMz,
                                .quantity = l.LibraryIntensity,
                                .intensity = l.LibraryIntensity
                            }
                        End Function) _
                .ToArray
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
End Namespace