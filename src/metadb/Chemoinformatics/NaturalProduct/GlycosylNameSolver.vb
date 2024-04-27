#Region "Microsoft.VisualBasic::34905eedf1f912c0aa1e5c2bf6c8c829, G:/mzkit/src/metadb/Chemoinformatics//NaturalProduct/GlycosylNameSolver.vb"

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

    '   Total Lines: 285
    '    Code Lines: 237
    ' Comment Lines: 6
    '   Blank Lines: 42
    '     File Size: 10.56 KB


    '     Class GlycosylNameSolver
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GlycosylNameParser, (+2 Overloads) HandleComponents, HandleMultiple, HandleSingle, isNumber
    '                   JoinNames, SplitByTopLevelStack, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace NaturalProduct

    Public Class GlycosylNameSolver

        ReadOnly steric As Index(Of String) = {
            "alpha", "beta", "trans",
            "cis", "red", "acid",
            "bis", "ester", "cyclic",
            "bata", "hydroxy",
            "aero", "pro",
            "-", ","  ' delimiter symbols
        }
        ReadOnly rules As Dictionary(Of String, String())

        Friend Shared ReadOnly qprefix As Dictionary(Of String, Integer) = Enums(Of QuantityPrefix) _
            .ToDictionary(Function(a) a.Description,
                          Function(a)
                              Return CInt(a)
                          End Function)

        Sub New(Optional customRules As Dictionary(Of String, String()) = Nothing)
            Me.rules = customRules

            If rules.IsNullOrEmpty Then
                rules = New Dictionary(Of String, String())
            End If
        End Sub

        Private Iterator Function HandleComponents(token As String) As IEnumerable(Of String)
            Dim hitPrefix As Boolean = False

            If token.Length < 3 Then
                Return
            ElseIf token Like steric Then
                Return
            End If

            For Each q As KeyValuePair(Of String, Integer) In qprefix
                If token.StartsWith(q.Key) Then
                    hitPrefix = True
                    token = token.Substring(q.Key.Length)

                    If rules.ContainsKey(token) Then
                        For Each item As String In rules(token)
                            For i As Integer = 1 To q.Value
                                Yield item
                            Next
                        Next
                    Else
                        For i As Integer = 1 To q.Value
                            Yield token
                        Next
                    End If

                    Exit For
                End If
            Next

            If Not hitPrefix Then
                If rules.ContainsKey(token) Then
                    For Each item As String In rules(token)
                        Yield item
                    Next
                Else
                    Yield token
                End If
            End If
        End Function

        Private Shared Function Trim(glycosyl As String) As String
            glycosyl = glycosyl.StringReplace("\d+", " ")
            ' glycosyl = glycosyl.StringReplace("[()]", " ")
            glycosyl = glycosyl.Replace("'", "").Replace("[", " ").Replace("]", " ")
            glycosyl = glycosyl.StringReplace("[-]{2,}", "-")
            glycosyl = glycosyl.Trim(" "c, "-"c, ","c, "{"c, "}"c, "["c, "]"c)

            Return glycosyl
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="glycosyl"></param>
        ''' <returns></returns>
        Public Iterator Function GlycosylNameParser(glycosyl As String) As IEnumerable(Of String)
            Dim tokens As Token() = Trim(glycosyl) _
                .ToLower _
                .DoCall(AddressOf TokenScanner.ScanTokens) _
                .Where(Function(t)
                           If t.name = NameTokens.name Then
                               If t.text Like steric Then
                                   Return False
                               ElseIf t.length < 2 Then
                                   Return False
                               Else
                                   Return True
                               End If
                           Else
                               Return True
                           End If
                       End Function) _
                .ToArray
            Dim blocks As Token()()

            Try
                blocks = SplitByTopLevelStack(tokens) _
                    .DoCall(AddressOf JoinNames) _
                    .ToArray
            Catch ex As Exception
                Throw New InvalidExpressionException(glycosyl)
            End Try

            For Each tokenList As Token() In blocks
                Try
                    For Each part As String In HandleComponents(tokenList)
                        Yield part
                    Next
                Catch ex As Exception
                    Throw New InvalidExpressionException(glycosyl)
                End Try
            Next
        End Function

        Private Iterator Function JoinNames(blocks As IEnumerable(Of Token())) As IEnumerable(Of Token())
            For Each block In blocks
                Dim list As New List(Of Token)(block)

                For i As Integer = 0 To list.Count - 1
                    If list(i).text = "yl" Then
                        list(i - 1).text = $"{list(i - 1).text}yl"
                        list.RemoveAt(i)

                        If list.Count <= i + 1 Then
                            Exit For
                        End If
                    End If
                Next

                Yield list.PopAll
            Next
        End Function

        Private Function HandleSingle(tokenList As Token()) As IEnumerable(Of String)
            If tokenList(Scan0).name = NameTokens.name Then
                Return HandleComponents(tokenList(Scan0).text)
            Else
                Throw New SyntaxErrorException(tokenList.JoinBy(" "))
            End If
        End Function

        Private Function HandleMultiple(tokenList As Token()) As IEnumerable(Of String)
            Dim output As New List(Of String)

            If tokenList.Any(Function(a) a.name = NameTokens.open) Then
                Dim n As Integer = 1

                If tokenList.First.name = NameTokens.number Then
                    n = qprefix(tokenList(Scan0).text)
                    tokenList = tokenList.Skip(2).Take(tokenList.Length - 3).ToArray
                Else
                    tokenList = tokenList.Skip(1).Take(tokenList.Length - 2).ToArray
                End If

                If tokenList.Any(Function(a) a.name = NameTokens.open) Then
                    Dim blocks = SplitByTopLevelStack(tokenList).ToArray

                    For Each block As Token() In blocks
                        Dim allNames As String() = HandleComponents(block).ToArray

                        For i As Integer = 1 To n
                            output.AddRange(allNames)
                        Next
                    Next
                Else
                    Dim allNames = HandleComponents(tokenList).ToArray

                    For i As Integer = 1 To n
                        output.AddRange(allNames)
                    Next
                End If
            ElseIf tokenList.Length = 2 Then
                If tokenList(Scan0).name = NameTokens.number Then
                    Dim allNames = HandleComponents(tokenList(1).text).ToArray
                    Dim n As Integer = qprefix(tokenList(Scan0).text)

                    For i As Integer = 1 To n
                        output.AddRange(allNames)
                    Next
                Else
                    For Each item In tokenList
                        output.AddRange(HandleComponents(item.text))
                    Next
                End If
            Else
                For Each item In tokenList
                    output.AddRange(HandleComponents(item.text))
                Next
            End If

            Return output
        End Function

        Private Function HandleComponents(tokenList As Token()) As IEnumerable(Of String)
            If tokenList.Length = 1 Then
                Return HandleSingle(tokenList)
            ElseIf tokenList.Length > 0 Then
                Return HandleMultiple(tokenList)
            Else
                Return {}
            End If
        End Function

        Private Shared Function isNumber(buf As List(Of Token)) As Boolean
            Return buf = 1 AndAlso buf(Scan0).name = NameTokens.number
        End Function

        Private Iterator Function SplitByTopLevelStack(tokens As IEnumerable(Of Token)) As IEnumerable(Of Token())
            Dim buf As New List(Of Token)
            Dim stack As New Stack(Of String)

            For Each t As Token In tokens
                If t.name = NameTokens.open Then
                    stack.Push("(")

                    If Not isNumber(buf) AndAlso stack.Count = 0 Then
                        If buf > 0 Then
                            Yield buf.PopAll
                        End If
                    End If

                    buf.Add(t)
                ElseIf t.name = NameTokens.close Then
                    If stack.Count = 0 Then
                        If buf = 0 Then
                            Continue For
                        Else
                            Throw New InvalidProgramException
                        End If
                    Else
                        stack.Pop()
                        buf.Add(t)
                    End If

                    If stack.Count = 0 Then
                        Yield buf.PopAll
                    End If
                ElseIf t.name = NameTokens.number Then
                    buf.Add(t)
                Else
                    If stack.Count = 0 Then
                        If buf > 0 Then
                            buf += t
                            Yield buf.PopAll
                        Else
                            Yield {t}
                        End If
                    Else
                        buf.Add(t)
                    End If
                End If
            Next

            If stack.Count > 0 Then
                If buf > 0 Then
                    Dim message As String = $"name is broken: {buf.Select(Function(t) t.text).JoinBy(" ")}"

                    Call message.__DEBUG_ECHO

                    Do While stack.Count > 0
                        stack.Pop()
                        buf += New Token(NameTokens.close, ")")
                    Loop

                    Yield buf.PopAll
                End If
            ElseIf buf > 0 Then
                Yield buf.PopAll
            End If
        End Function
    End Class
End Namespace
