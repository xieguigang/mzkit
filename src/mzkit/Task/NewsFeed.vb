Imports System.Text
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class NewsFeed

    Const latestNewsPageUrl As String = "http://www.bionovogene.com/news/newsFeed.htm"

    Public Property title As String
    Public Property abstract As String
    Public Property [date] As String
    Public Property url As String

    Public Shared Iterator Function ParseLatest() As IEnumerable(Of NewsFeed)
        Dim html As String = latestNewsPageUrl.GET
        Dim items As String() = Strings.Split(html, "<div class=""product-set-item-info-inner-container"">")

        For Each item As String In items.Skip(1)
            Dim url As String = item.Match("[<]a.+?[<]/a[>]")
            Dim title As String = url.GetValue
            url = $"http://www.bionovogene.com/{url.href}"
            Dim dateTime = item.Match("发布日期：.+[<]/div[>]").Replace("</div>", "").Replace("发布日期：", "").Trim
            Dim abstract As New StringBuilder(item.Match("<div class[=]""product-medium-description.+?</div>").GetValue)
            Dim unicodes = abstract.ToString.Matches("[&][#]\d+[;]").Distinct.ToArray

            For Each code In unicodes
                Dim charCode As Integer = Integer.Parse(code.Match("\d+"))
                Dim [char] = Strings.ChrW(charCode)
                abstract.Replace(code, [char])
            Next

            Yield New NewsFeed With {
                .abstract = abstract.ToString,
                .[date] = dateTime,
                .title = title,
                .url = url
            }
        Next
    End Function

End Class
