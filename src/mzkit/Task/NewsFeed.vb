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
        Dim items As String() = Splitter.Split(html, "<div class[=]""product-set-item-info-inner-container"">", False)

        For Each item As String In items
            Dim url As String = item.Match("[<]a[>].+?[<]/a[>]")
            Dim title As String = url.GetValue
            url = $"http://www.bionovogene.com/{url.href}"
            Dim dateTime = item.Match("发布日期：.+[<]/div[>]").Replace("</div>", "").Replace("发布日期：", "").Trim
            Dim abstract = item.Match("<div class[=]""product-medium-description.+?</div>").GetValue

            Yield New NewsFeed With {
                .abstract = abstract,
                .[date] = dateTime,
                .title = title,
                .url = url
            }
        Next
    End Function

End Class
