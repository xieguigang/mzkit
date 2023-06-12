#Region "Microsoft.VisualBasic::079dc907878ee6374a3c510b071dbcd4, mzkit\src\metadb\Chemoinformatics\Formula\FormulaScanner.vb"

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


' Code Statistics:

'   Total Lines: 211
'    Code Lines: 140
' Comment Lines: 33
'   Blank Lines: 38
'     File Size: 7.24 KB


'     Class FormulaScanner
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: EvaluateExactMass, (+2 Overloads) ScanFormula, Val
' 
'         Sub: push
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser

Namespace Formula

    Public NotInheritable Class FormulaScanner

        ''' <summary>
        ''' 处理在化学式之中遇到的小字母``n``设置的一个默认值
        ''' 设置这个值主要是应用于化学式比较
        ''' </summary>
        Dim n As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <DebuggerStepThrough>
        Private Sub New(n As Integer)
            Me.n = n
        End Sub

        Shared Sub New()
            Call ExactMass.SetExactMassParser(Function(f) EvaluateExactMass(f))
        End Sub

        Public Shared Function EvaluateExactMass(formula$, Optional n% = 9999) As Double
            Static cache As New Dictionary(Of String, Double)

            Dim key As String = $"{formula} ~ {n}"
            Dim mass As Double

            If cache.ContainsKey(key) Then
                Return cache(key)
            Else
                mass = CDbl(ScanFormula(formula, n))

                SyncLock cache
                    If Not cache.ContainsKey(key) Then
                        Call cache.Add(key, mass)
                    End If
                End SyncLock

                Return mass
            End If
        End Function

        Public Const Pattern As String = "([A-Z][a-z]?\d*)+"

        ' H2O
        ' (CH3)3CH
        ' (CH3)4C

        ''' <summary>
        ''' Parse the given formula string value as the <see cref="Chemoinformatics.Formula.Formula"/> object
        ''' </summary>
        ''' <param name="formula">
        ''' The formula string, A chemical formula is a notation used by scientists 
        ''' to show the number and type of atoms present in a molecule, using the 
        ''' atomic symbols and numerical subscripts. A chemical formula is a simple 
        ''' representation, in writing, of a three dimensional molecule that exists.
        ''' A chemical formula describes a substance, down to the exact atoms which 
        ''' make it up.
        ''' </param>
        ''' <param name="n">for counting polymers atoms</param>
        ''' <returns>
        ''' the formula string keeps the same order with the input 
        ''' <paramref name="formula"/>; and also this function will
        ''' returns nothing if the given <paramref name="formula"/>
        ''' string is empty or null
        ''' </returns>
        ''' <remarks>
        ''' The ``CType/CDbl`` convertor operator could be used for get extract mass value
        ''' from the generated formula object, due to the reason of this function may 
        ''' returns nothing if the formula input string is empty or incorrect. the CType
        ''' and CDbl operator will handling such null reference error safely
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ScanFormula(formula$, Optional n% = 9999) As Formula
            Dim key As String, formula2 As Formula

            Static cache As New Dictionary(Of String, Formula)

            If formula.StringEmpty Then
                Return Nothing
            Else
                key = $"{formula} ~ {n}"
            End If

            If cache.ContainsKey(key) Then
                formula2 = cache(key)
                formula2 = New Formula(formula2.CountsByElement, formula2.m_formula)
            Else
                formula2 = New FormulaScanner(n).ScanFormula(New CharPtr(formula))

                SyncLock cache
                    If Not cache.ContainsKey(key) Then
                        Call cache.Add(key, formula2)
                    End If
                End SyncLock
            End If

            Return formula2
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

        Private Function ScanFormula(scaner As CharPtr) As Formula
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
                    Dim group As Formula = New FormulaScanner(n).ScanFormula(scaner)
                    Dim skipNextNumber As Boolean = False

                    Call push(c)
                    ' 后面必定会跟着数字
                    c = ++scaner

                    If Char.IsDigit(c) Then
                        digits += c
                    ElseIf c = "n"c Then
                        digits += Me.n
                    Else
                        digits += "1"c
                        skipNextNumber = True
                    End If

                    If Not skipNextNumber Then
                        Do While Not scaner.EndRead
                            c = ++scaner

                            If Char.IsDigit(c) Then
                                digits += c
                            Else
                                Exit Do
                            End If
                        Loop
                    End If

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

            Return New Formula(composition.AsInteger, formula.CharString)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function Val(digits As List(Of Char)) As Double
            Return Conversion.Val(digits.CharString)
        End Function
    End Class
End Namespace
