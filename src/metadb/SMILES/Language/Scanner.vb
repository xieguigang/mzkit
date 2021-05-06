Imports Microsoft.VisualBasic.Text.Parser

Public Class Scanner

    Dim SMILES As CharPtr
    Dim buf As New CharBuffer

    Sub New(SMILES As String)
        Me.SMILES = SMILES
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
                Else
                    Yield MeasureElement(New String(buf.PopAllChars))
                End If
            ElseIf c = "]"c Then
                buf += c
                Yield MeasureElement(New String(buf.PopAllChars))
                Return
            End If

            buf += c
        End If
    End Function

    Private Function MeasureElement(str As String) As Token
        If str.Length >= 3 AndAlso (str.First = "["c AndAlso str.Last = "]"c) Then
            ' [H]
            str = str.GetStackValue("[", "]")
        End If

        Select Case str
            Case "B", "C", "N", "O", "P", "S", "F", "Cl", "Br", "I", "Au", "H"
                Return New Token(ElementTypes.Element, str)
            Case "("
                Return New Token(ElementTypes.Open, str)
            Case ")"
                Return New Token(ElementTypes.Close, str)
            Case Else
                Throw New NotImplementedException(str)
        End Select
    End Function
End Class
