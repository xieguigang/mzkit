#Region "Microsoft.VisualBasic::31f0f6c04cc77fd09f456ceb1365b98d, src\metadb\Massbank\MetaLib\Formula\FormulaScanner.vb"

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

    '     Class FormulaScanner
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) ScanFormula, Val
    ' 
    '         Sub: push
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser

Namespace MetaLib

    Public NotInheritable Class FormulaScanner

        ''' <summary>
        ''' 处理在化学式之中遇到的小字母``n``设置的一个默认值
        ''' 设置这个值主要是应用于化学式比较
        ''' </summary>
        Dim n As String

        Private Sub New(n As Integer)
            Me.n = n
        End Sub

        ' H2O
        ' (CH3)3CH
        ' (CH3)4C

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanFormula(formula$, Optional n% = 9999) As FormulaComposition
            Return New FormulaScanner(n).ScanFormula(New CharPtr(formula))
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
                    Dim group As FormulaComposition = New FormulaScanner(n).ScanFormula(scaner)

                    Call push(c)
                    ' 后面必定会跟着数字
                    c = ++scaner

                    If Char.IsDigit(c) Then
                        digits += c
                    ElseIf c = "n"c Then
                        digits += Me.n
                    Else
                        Throw New SyntaxErrorException(scaner.RawBuffer.CharString)
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
