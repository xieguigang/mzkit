Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 判断两个化学名称是否相同？
''' </summary>
Public Class ChemicalNameEquality : Implements IEqualityComparer(Of String)

    Public Overloads Function Equals(x As String, y As String) As Boolean Implements IEqualityComparer(Of String).Equals
        If x.TextEquals(y) Then
            Return True
        End If

        ' 可能是一些同分异构体的名称字符串
        '
        ' 通常所使用的化合物的俗称都是L-手性的？
        If ("L-" & x).TextEquals(y) Then
            Return True
        ElseIf x.TextEquals("L-" & y) Then
            Return True
        End If

        If InStr(x, "Oxo", CompareMethod.Text) > 0 Then
            If OxoName(x).TextEquals(y) Then
                Return True
            ElseIf x.TextEquals(OxoName(y)) Then
                Return True
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' 氧代,氧络的,含氧的
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Private Shared Function OxoName(name As String) As String
        With Strings.Split(name, "Oxo", Compare:=CompareMethod.Text)
            If InStr(.ref(1), "-L-", CompareMethod.Text) > 0 Then
                ' 不需要再拓展了
                ' 5-Oxo-L-proline;
                Return name
            Else
                Return $"{ .First}Oxo-L-{ .Skip(1).JoinBy("oxo")}"
            End If
        End With
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Function GetHashCode(obj As String) As Integer Implements IEqualityComparer(Of String).GetHashCode
        Return obj.GetHashCode
    End Function
End Class
