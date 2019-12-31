#Region "Microsoft.VisualBasic::4f198078b4634fa057c7535e8e3936fa, DATA\Massbank\MetaLib\Match\NameEquality.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    '     Class ChemicalNameEquality
    ' 
    '         Function: Distinct, Equals, GetHashCode, OxoName, RemoveChiralFlag
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace MetaLib

    ''' <summary>
    ''' 判断两个化学名称是否相同？
    ''' </summary>
    Public Class ChemicalNameEquality : Implements IEqualityComparer(Of String)

        ''' <summary>
        ''' 将化合物名称之中的手性标志移除
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Shared Function RemoveChiralFlag(name As String) As String
            If InStr(name, "L-") > 0 OrElse InStr(name, "D-") > 0 Then
                name = Mid(name, 3).Trim("-"c)
            End If

            Return name
        End Function

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
                If InStr(.ByRef(1), "-L-", CompareMethod.Text) > 0 Then
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
            Return Strings.LCase(obj).GetHashCode
        End Function

        ''' <summary>
        ''' 这个函数是为了将大小写不同的名字给去除掉重复
        ''' </summary>
        ''' <param name="names"></param>
        ''' <returns></returns>
        Public Function Distinct(names As IEnumerable(Of String)) As IEnumerable(Of String)
            Return names.DistinctIgnoreCase
        End Function
    End Class
End Namespace
