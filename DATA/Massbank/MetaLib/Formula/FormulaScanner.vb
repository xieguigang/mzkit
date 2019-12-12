Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser

Namespace MetaLib

    Public NotInheritable Class FormulaScanner

        Private Sub New()
        End Sub

        ' H2O
        ' (CH3)3CH
        ' (CH3)4C

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanFormula(formula As String) As FormulaComposition
            Return New FormulaScanner().ScanFormula(New CharPtr(formula))
        End Function

        Dim composition As New Dictionary(Of String, Counter)
        Dim buf As New List(Of Char)
        Dim digits As New List(Of Char)
        Dim formula As New List(Of Char)

        Private Sub push(c As Char)
            Dim element$ = buf.CharString

            If element.Length = 0 Then
                Return
            End If
            If Not composition.ContainsKey(element) Then
                composition(element) = New Counter
            End If

            ' 结束上一个元素
            If digits = 0 Then
                Call composition(buf.CharString).Hit()
            Else
                Call composition(buf.CharString).Add(digits.DoCall(AddressOf Val))
            End If

            formula += buf
            formula += digits
            buf *= 0
            digits *= 0

            If c <> "("c AndAlso c <> ")"c Then
                buf += c
            End If
        End Sub

        Private Function ScanFormula(scaner As CharPtr) As FormulaComposition
            Dim c As Char

            Do While Not scaner.EndRead
                c = ++scaner

                If Char.IsLetter(c) Then
                    If Char.IsUpper(c) Then
                        If buf = 0 Then
                            buf += c
                        Else
                            Call push(c)
                        End If
                    Else
                        ' 小写字母，则继续添加进入缓存
                        buf += c
                    End If
                ElseIf Char.IsDigit(c) Then
                    ' 可能是两位数或者更多
                    digits += c
                ElseIf c = "("c Then
                    ' 遇到了一个堆栈
                    ' 对这个分子基团进行函数递归
                    Dim group = scaner.DoCall(AddressOf ScanFormula)

                    Call push(c)
                    ' 后面必定会跟着数字
                    c = ++scaner

                    If Not Char.IsDigit(c) Then
                        Throw New SyntaxErrorException(scaner.RawBuffer.CharString)
                    Else
                        digits += c
                    End If

                    Do While Not scaner.EndRead
                        c = ++scaner

                        If Char.IsDigit(c) Then
                            digits += c
                        Else
                            Exit Do
                        End If
                    Loop

                    c = --scaner
                    group *= digits.DoCall(AddressOf Val)
                    digits *= 0

                    Call group.CountsByElement _
                          .DoEach(Sub(e)
                                      If Not composition.ContainsKey(e.Key) Then
                                          composition(e.Key) = New Counter
                                      End If

                                      Call composition(e.Key).Add(e.Value)
                                  End Sub)

                    formula += group.EmpiricalFormula

                ElseIf c = ")"c Then
                    ' 结束当前的堆栈
                    Exit Do
                End If
            Loop

            ' 把剩余的也添加进去
            Call push(c)

            Return New FormulaComposition(composition.AsInteger, formula.CharString)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function Val(digits As List(Of Char)) As Double
            Return Conversion.Val(digits.CharString)
        End Function
    End Class
End Namespace