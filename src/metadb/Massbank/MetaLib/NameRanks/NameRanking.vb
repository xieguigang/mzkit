Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Namespace MetaLib.CommonNames

    Public Module NameRanking

        ''' <summary>
        ''' the white space is exclude from the symbol list, due to 
        ''' the reason of the white space is recommended in the 
        ''' common name, example as: "Magneson I"
        ''' </summary>
        ReadOnly symbols As Char() = {"-", "/", "\", ":", "<", ">", "?", "(", ")", "[", "]", "{", "}", "|", ";", ",", "'", """"c, ".", "_"}
        ReadOnly rules As RankingRule() = {
            New ChemicalFormulaRule,
            New NumberRule,
            New DatabaseIdRule,
            New InchiKeyRule,
            New UpperCaseName
        }
        ReadOnly empty_symbols As Index(Of String) = symbols _
            .Select(Function(c) c.ToString) _
            .Indexing

        Public Function Score(name As String, Optional maxLen As Integer = 32, Optional minLen As Integer = 5) As (score As Double, penalty As String())
            If name.StringEmpty(testEmptyFactor:=True) OrElse name Like empty_symbols Then
                Return (-1, {"empty_string"})
            End If

            Dim eval As Double
            Dim penalty As New List(Of String)
            Dim count As Integer = Aggregate c As Char
                                   In symbols
                                   Into Sum(name.Count(c))

            ' score the name by length
            If name.Length < minLen Then
                eval = 1
            ElseIf name.Length < maxLen Then
                eval = 10 * (maxLen / name.Length)
            Else
                eval = 10 / name.Length
            End If

            eval /= (count / 3 + 1)

            For Each rule As RankingRule In rules
                If rule.Match(name) Then
                    eval /= rule.Penalty
                    penalty.Add(rule.Type)
                End If
            Next

            Return (eval, penalty.ToArray)
        End Function

        ''' <summary>
        ''' ranking the input names with score order in desc
        ''' </summary>
        ''' <param name="names"></param>
        ''' <param name="maxLen"></param>
        ''' <param name="minLen"></param>
        ''' <returns></returns>
        Public Function Ranking(names As IEnumerable(Of String),
                                Optional maxLen As Integer = 32,
                                Optional minLen As Integer = 5) As IEnumerable(Of NumericTagged(Of String))

            Return From name As String
                   In names
                   Let score = NameRanking.Score(name, maxLen, minLen)
                   Select out = New NumericTagged(Of String)(score, name)
                   Order By out.tag Descending
        End Function

    End Module
End Namespace