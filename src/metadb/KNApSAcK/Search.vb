Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class Search

    Public Shared Iterator Function Search(word As String, Optional type As Types = Types.metabolite) As IEnumerable(Of ResultEntry)
        Dim content = sprintf(My.Resources.knapsack_search, type.Description, word.UrlEncode) _
            .GET _
            .GetTablesHTML _
            .FirstOrDefault _
            .GetRowsHTML

        For Each rowText As String In content
            Dim cells As String() = rowText.GetColumnsHTML
            Dim names As String() = cells(2) _
                .HtmlLines _
                .Select(Function(str) str.StripHTMLTags) _
                .ToArray
            Dim entry As New ResultEntry With {
                .C_ID = cells(Scan0).StripHTMLTags,
                .CAS_ID = cells(1).StripHTMLTags,
                .Metabolite = names,
                .formula = cells(3).StripHTMLTags,
                .Mw = cells(4).StripHTMLTags.ParseDouble
            }

            Yield entry
        Next
    End Function
End Class

Public Class ResultEntry
    Public Property C_ID As String
    Public Property CAS_ID As String
    Public Property Metabolite As String()
    Public Property formula As String
    Public Property Mw As Double
End Class