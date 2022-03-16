#Region "Microsoft.VisualBasic::d962eee4634c16edc6e7e6659693bfcf, src\metadb\SMILES\Language\Scanner.vb"

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

    ' Class Scanner
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetTokens, MeasureElement, WalkChar
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Parser

Public Class Scanner

    Dim SMILES As CharPtr
    Dim buf As New CharBuffer

    Sub New(SMILES As String)
        Me.SMILES = SMILES.Replace("@", "")
    End Sub

    ''' <summary>
    ''' Parse SMILES tokens
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function GetTokens() As IEnumerable(Of Token)
        Do While Not SMILES.EndRead
            For Each t As Token In WalkChar(++SMILES)
                Yield t
            Next
        Loop

        If buf > 0 Then
            Yield MeasureElement(New String(buf.PopAllChars))
        End If
    End Function

    Private Iterator Function WalkChar(c As Char) As IEnumerable(Of Token)
        If c = "("c OrElse c = ")"c Then
            If buf > 0 Then
                Yield MeasureElement(New String(buf.PopAllChars))
            End If

            Yield MeasureElement(c)
        ElseIf c Like ChemicalBonds Then
            If buf > 0 Then
                Yield MeasureElement(New String(buf.PopAllChars))
            End If

            Yield New Token(ElementTypes.Key, c)
        Else
            If Char.IsLetter(c) AndAlso Char.IsUpper(c) AndAlso buf > 0 Then
                If buf = 1 AndAlso buf(Scan0) = "["c Then
                    Call Debug.WriteLine("[")
                ElseIf buf(0) = "["c Then
                    Call Debug.WriteLine("-")
                Else
                    Yield MeasureElement(New String(buf.PopAllChars))
                End If
            ElseIf c = "]"c Then
                buf += c

                Dim tmpStr = New String(buf.PopAllChars)

                tmpStr = tmpStr.GetStackValue("[", "]")
                tmpStr = tmpStr.StringReplace("[+-]$", "", RegexOptions.Multiline)

                Dim tmp As String = ""

                For Each c In tmpStr
                    If Char.IsUpper(c) Then
                        If tmp.Length > 0 Then
                            Yield MeasureElement(tmp)
                        End If

                        tmp = c
                    Else
                        tmp = tmp & c
                    End If
                Next

                If tmp.Length > 0 Then
                    Yield MeasureElement(tmp)
                End If

                Return
            End If

            buf += c
        End If
    End Function

    Private Function MeasureElement(str As String) As Token
        Dim ring As Integer? = Nothing

        If str.Length >= 3 AndAlso (str.First = "["c AndAlso str.Last = "]"c) Then
            ' [H]
            str = str.GetStackValue("[", "]")
        End If
        If str.IsPattern("[A-Za-z]+\d+") Then
            ' removes number
            ring = Integer.Parse(str.Match("\d+"))
            str = str.Match("[a-zA-Z]+")
        End If

        Select Case str
            Case "B", "C", "N", "O", "P", "S", "F", "Cl", "Br", "I", "Au", "H"
                Return New Token(ElementTypes.Element, str) With {
                    .ring = ring
                }
            Case "("
                Return New Token(ElementTypes.Open, str)
            Case ")"
                Return New Token(ElementTypes.Close, str)
            Case Else
                Throw New NotImplementedException(str)
        End Select
    End Function
End Class
