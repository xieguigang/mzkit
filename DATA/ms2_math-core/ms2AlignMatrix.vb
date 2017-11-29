Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Scripting
Imports MathCore = Microsoft.VisualBasic.Math

Public Class SSM2MatrixFragment
    Public Property mz As Double
    Public Property query As Double
    Public Property ref As Double
    Public Property da As String
End Class

Public Class Ms2AlignMatrix : Inherits VectorModel(Of SSM2MatrixFragment)

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
End Class