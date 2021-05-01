Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class SearchQuery : Inherits WebQuery(Of QueryInput)

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        MyBase.New(
            url:=AddressOf url,
            contextGuid:=AddressOf guid,
            parser:=AddressOf parser,
            prefix:=Function(str) str.MD5.First,
            cache:=cache,
            interval:=interval,
            offline:=offline
        )
    End Sub

    Private Shared Function guid(q As QueryInput) As String
        Return q.ToString
    End Function

    Private Shared Function parser(html As String, type As Type) As Object
        Return parseList(html).ToArray
    End Function

    Private Shared Iterator Function parseList(html As String) As IEnumerable(Of ResultEntry)
        Dim content As String() = html _
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

    Private Shared Function url(q As QueryInput) As String
        Return sprintf(My.Resources.knapsack_search, q.type.Description, q.word.UrlEncode)
    End Function
End Class

Public Structure QueryInput

    Dim word As String
    Dim type As Types

    Public Overrides Function ToString() As String
        Return $"{type.Description}+{word.UrlEncode}"
    End Function
End Structure