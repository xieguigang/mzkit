Imports Microsoft.VisualBasic.Text.Parser

Public Class ParseChain

    ReadOnly SMILES As CharPtr
    ReadOnly graph As New ChemicalFormula
    ReadOnly chainStack As New Stack(Of ChemicalElement)

    Dim buf As New CharBuffer

    Sub New(SMILES As String)
        Me.SMILES = SMILES
    End Sub

    Public Function ParseGraph() As ChemicalFormula
        Dim tokens As Token() = GetTokens().ToArray
    End Function

    Private Iterator Function GetTokens() As IEnumerable(Of Token)
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
        If c = "("c Then
            If buf > 0 Then
                Yield MeasureElement(New String(buf.PopAllChars))
            End If

            Yield New Token(ElementTypes.Open, "(")
        ElseIf c = ")"c Then
            Yield New Token(ElementTypes.Close, ")")
        ElseIf c Like ChemicalBonds Then
            If buf > 0 Then
                Yield MeasureElement(New String(buf.PopAllChars))
            End If

            Yield New Token(ElementTypes.Key, c)
        Else
            If c = "C"c AndAlso buf > 0 Then
                Yield MeasureElement(New String(buf.PopAllChars))
            End If

            buf += c
        End If
    End Function

    Private Function MeasureElement(str As String) As Token
        If str.Length > 3 AndAlso (str.First = "["c AndAlso str.Last = "]"c) Then
            str = str.GetStackValue("[", "]")
        End If

        Select Case str
            Case "B", "C", "N", "O", "P", "S", "F", "Cl", "Br", "I", "Au"
                Return New Token(ElementTypes.Element, str)
            Case Else
                Throw New NotImplementedException(str)
        End Select
    End Function
End Class
