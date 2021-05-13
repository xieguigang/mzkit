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
            Dim blocks As Token()() = SplitByTopLevelStack(tokens).ToArray

            For Each tokenList As Token() In blocks
                For Each part As String In HandleComponents(tokenList)
                    Yield part
                Next
            Next
        End Function

        Private Iterator Function HandleComponents(tokenList As Token()) As IEnumerable(Of String)
            If tokenList.Length = 1 Then
                If tokenList(Scan0).name = NameTokens.name Then
                    For Each component As String In HandleComponents(tokenList(Scan0).text)
                        Yield component
                    Next
                Else
                    Throw New SyntaxErrorException
                End If
            Else
                Dim n As Integer = 1

                If tokenList.First.name = NameTokens.number Then
                    n = qprefix(tokenList(Scan0).text)
                End If

                tokenList = tokenList.Skip(2).Take(tokenList.Length - 3).ToArray

                Dim blocks = SplitByTopLevelStack(tokenList).ToArray

                For Each block In blocks
                    Dim allNames As String() = HandleComponents(block).ToArray

                    For i As Integer = 1 To n
                        For Each part As String In allNames
                            Yield part
                        Next
                    Next
                Next
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

                    If Not isNumber(buf) Then
                        Yield buf.PopAll
                    End If

                    buf.Add(t)
                ElseIf t.name = NameTokens.close Then
                    stack.Pop()
                    buf.Add(t)
                ElseIf t.name = NameTokens.number Then
                    buf.Add(t)
                Else
                    If stack.Count = 0 Then
                        Yield {t}
                    Else
                        buf.Add(t)
                    End If
                End If
            Next

            If stack.Count > 0 Then
                Throw New SyntaxErrorException("name is broken!")
            ElseIf buf > 0 Then
                Yield buf.PopAll
            End If
        End Function
    End Class
End Namespace