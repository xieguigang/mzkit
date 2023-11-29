Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Namespace MetaLib

    Public Module NameRanking

        ReadOnly empty_symbols As Index(Of String) = {".", "_", "?"}
        ReadOnly symbols As Char() = {"-", "/", "\", ":", "<", ">", "?", "(", ")", "[", "]", "{", "}", "|", ";", ",", "'", """"c, "."}

        Public Function Score(name As String, Optional maxLen As Integer = 24) As Double
            If name.StringEmpty(testEmptyFactor:=True) OrElse name Like empty_symbols Then
                Return -1
            End If

            Dim eval As Double

            If name.Length < 3 Then
                eval = 1
            ElseIf name.Length < maxLen Then
                eval = 10
            Else
                eval = 10 / name.Length
            End If

            ' avoid the database id
            If name.IsPattern("[a-zA-Z]+\d+") Then
                eval /= 2.3
            End If

            Dim count As Integer = Aggregate c As Char
                                   In symbols
                                   Into Sum(name.Count(c))
            eval /= (count + 1)

            Return eval
        End Function

        Public Function Ranking(names As IEnumerable(Of String)) As IEnumerable(Of NumericTagged(Of String))
            Return From name As String
                   In names
                   Let score As Double = NameRanking.Score(name)
                   Select out = New NumericTagged(Of String)(score, name)
                   Order By out.tag Descending
        End Function

    End Module
End Namespace