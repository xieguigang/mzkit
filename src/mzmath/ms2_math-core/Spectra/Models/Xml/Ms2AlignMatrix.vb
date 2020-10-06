Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Scripting
Imports MathCore = Microsoft.VisualBasic.Math

Namespace Spectra.Xml

    Public Class Ms2AlignMatrix : Inherits IVector(Of SSM2MatrixFragment)

        ''' <summary>
        ''' 计算两个色谱矩阵之间的余弦相似度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SSM As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                With Me
                    Return MathCore.SSM(!query, !ref)
                End With
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(source As IEnumerable(Of SSM2MatrixFragment))
            Call MyBase.New(source)
        End Sub

        Public Function GetQueryMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .name = "query",
                .ms2 = Me _
                    .Where(Function(a) a.query > 0) _
                    .Select(Function(a)
                                Return New ms2 With {
                                    .mz = a.mz,
                                    .intensity = a.query,
                                    .quantity = a.query
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        Public Function GetReferenceMatrix() As LibraryMatrix
            Return New LibraryMatrix With {
                .name = "subject",
                .ms2 = Me _
                    .Where(Function(a) a.ref > 0) _
                    .Select(Function(a)
                                Return New ms2 With {
                                    .mz = a.mz,
                                    .intensity = a.ref,
                                    .quantity = a.ref
                                }
                            End Function) _
                    .ToArray
            }
        End Function
    End Class
End Namespace