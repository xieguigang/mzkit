Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace NaturalProduct

    Public Class GlycosylNameSolver

        ReadOnly steric As Index(Of String) = {"alpha", "beta", "trans"}
        ReadOnly rules As Dictionary(Of String, String())
        ReadOnly qprefix As NamedValue(Of Integer)() = Enums(Of QuantityPrefix) _
            .Select(Function(n)
                        Return New NamedValue(Of Integer)(n.Description, CInt(n))
                    End Function) _
            .ToArray

        Sub New(customRules As Dictionary(Of String, String()))
            Me.rules = customRules
        End Sub

        Private Iterator Function HandleComponents(token As String) As IEnumerable(Of String)
            Dim hitPrefix As Boolean = False

            If token.Length < 3 Then
                Return
            ElseIf token Like steric Then
                Return
            End If

            For Each q As NamedValue(Of Integer) In qprefix
                If token.StartsWith(q.Name) Then
                    token = token.Substring(q.Name.Length)
                    hitPrefix = True

                    For i As Integer = 1 To q.Value
                        Yield token
                    Next

                    Exit For
                End If
            Next

            If Not hitPrefix Then
                Yield token
            End If
        End Function

        Public Iterator Function GlycosylNameParser(glycosyl As String) As IEnumerable(Of String)
            glycosyl = glycosyl.StringReplace("\d+", " ").ToLower
            glycosyl = glycosyl.StringReplace("[()]", " ")
            glycosyl = glycosyl.Replace("'", "").Replace("[", " ").Replace("]", " ")
            glycosyl = glycosyl.StringReplace("[-]{2,}", "-")
            glycosyl = glycosyl.Trim(" "c, "-"c)

            For Each token As String In glycosyl.StringSplit("([-])|\s+")
                For Each part As String In HandleComponents(token)
                    Yield part
                Next
            Next
        End Function
    End Class
End Namespace