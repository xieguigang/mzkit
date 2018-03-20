Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language

Public Module FormulaCompare

    ' H2O
    ' (CH3)3CH
    ' (CH3)4C

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function SplitFormula(formula As String) As FormulaComposition
        Return New Pointer(Of Char)(formula).ScanFormula
    End Function

    <Extension>
    Private Function ScanFormula(scaner As Pointer(Of Char)) As FormulaComposition
        Dim composition As New Dictionary(Of String, Counter)
        Dim buf As New List(Of Char)
        Dim digits As New List(Of Char)
        Dim c As Char
        Dim formula As New List(Of Char)
        Dim push = Sub()
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
                           Call composition(buf.CharString).Add(digits.Val)
                       End If

                       buf *= 0
                       buf += c
                       digits *= 0
                   End Sub

        Do While Not scaner.EndRead
            c = ++scaner

            If Char.IsLetter(c) Then
                If Char.IsUpper(c) Then
                    If buf = 0 Then
                        buf += c
                    Else
                        Call push()
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
                Dim group = scaner.ScanFormula

                Call push()
                ' 后面必定会跟着数字
                c = ++scaner

                If Not Char.IsDigit(c) Then
                    Throw New SyntaxErrorException(scaner.RawBuffer.CharString)
                Else
                    digits += c
                End If

                Do While Char.IsDigit(c)
                    digits += ++scaner
                    c = digits(-1)
                Loop

                c = --scaner
                group *= digits.Val

                Call group.CountsByElement _
                          .DoEach(Sub(e)
                                      Call composition(e.Key).Add(e.Value)
                                  End Sub)

                formula += group.EmpiricalFormula

            ElseIf c = ")"c Then
                ' 结束当前的堆栈
                Exit Do
            End If
        Loop

        ' 把剩余的也添加进去
        Call push()

        Return New FormulaComposition(composition.AsInteger, formula.CharString)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function Val(digits As List(Of Char)) As Double
        Return Conversion.Val(digits.CharString)
    End Function
End Module
