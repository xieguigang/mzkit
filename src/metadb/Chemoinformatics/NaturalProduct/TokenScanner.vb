Imports Microsoft.VisualBasic.Text.Parser

Namespace NaturalProduct

    Public Class TokenScanner

        Dim name As CharPtr
        Dim buf As New CharBuffer

        Sub New(name As String)
            Me.name = name
        End Sub

        Public Shared Function ScanTokens(name As String) As IEnumerable(Of Token)
            Return New TokenScanner(name).GetTokens
        End Function

        Public Iterator Function GetTokens() As IEnumerable(Of Token)
            Do While name
                For Each token As Token In WalkChar(++name)
                    Yield token
                Next
            Loop

            If buf > 0 Then
                Yield MeasureToken()
            End If
        End Function

        Private Iterator Function WalkChar(c As Char) As IEnumerable(Of Token)
            If c = "("c Then
                If buf > 0 Then
                    Yield MeasureToken()
                End If

                Yield New Token(NameTokens.open, "(")
            ElseIf c = ")"c Then
                If buf > 0 Then
                    Yield MeasureToken()
                End If

                Yield New Token(NameTokens.close, ")")
            Else
                buf += c
            End If
        End Function

        Private Function MeasureToken() As Token
            Dim text As String = buf.PopAllChars.CharString

            If GlycosylNameSolver.qprefix.ContainsKey(text) Then
                Return New Token(NameTokens.number, text)
            Else
                Return New Token(NameTokens.name, text)
            End If
        End Function

    End Class
End Namespace