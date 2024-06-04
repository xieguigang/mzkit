Namespace Lipidomics

    Public Class BondPosition

        Public Property index As Integer
        ''' <summary>
        ''' E/Z
        ''' </summary>
        ''' <returns></returns>
        Public Property [structure] As String

        Public Overrides Function ToString() As String
            Return $"{index}{[structure]}"
        End Function

        Friend Shared Iterator Function ParseStructure(components As String) As IEnumerable(Of BondPosition)
            If components = "" Then
                Return
            End If

            Dim groupInfo As String = components.GetTagValue("-", failureNoName:=False).Value
            Dim tokens As String() = groupInfo.Split("-"c)
            Dim is_empty As Boolean = tokens.Length = 1 AndAlso tokens(Scan0) = ""

            components = components.Replace($"-{groupInfo}", "")

            If Not is_empty Then
                For Each token As String In tokens
                    Dim index = token.Match("\(\d+[a-zA-Z]\)")
                    Dim t As String = index.StringReplace("\d+", "").Trim("("c, ")"c)

                    token = token.Replace(index, "")
                    index = index.Match("\d+")

                    Yield New Group With {
                    .groupName = token,
                    .index = Integer.Parse(index),
                    .[structure] = t
                }
                Next
            End If

            If components <> "" Then
                tokens = components.GetStackValue("(", ")").Split(","c)

                For Each token As String In tokens
                    Dim index As String = token.Match("\d+")
                    Dim t As String = token.StringReplace("\d+", "")

                    Yield New BondPosition With {
                    .index = If(index.StringEmpty, 1, Integer.Parse(index)),
                    .[structure] = t
                }
                Next
            End If
        End Function
    End Class
End Namespace