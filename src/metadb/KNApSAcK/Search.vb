Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class Search

    Public Shared Function Search(word As String, Optional type As Types = Types.metabolite) As ResultEntry()
        Dim content = sprintf(My.Resources.knapsack_search, type.Description, word.UrlEncode) _
            .GET _
            .GetTablesHTML _
            .FirstOrDefault

    End Function
End Class

Public Class ResultEntry
    Public Property C_ID As String
    Public Property CAS_ID As String
    Public Property Metabolite As String()
    Public Property formula As String
    Public Property Mw As Double
End Class