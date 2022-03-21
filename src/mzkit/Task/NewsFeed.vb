#Region "Microsoft.VisualBasic::87f72e1f3b1989379e841a5d9d73db55, mzkit\src\mzkit\Task\NewsFeed.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 45
    '    Code Lines: 35
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.75 KB


    ' Class NewsFeed
    ' 
    '     Properties: [date], abstract, html, title, url
    ' 
    '     Function: ParseLatest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class NewsFeed

    Const latestNewsPageUrl As String = "http://www.bionovogene.com/news/newsFeed.htm"

    Public Property title As String
    Public Property abstract As String
    Public Property [date] As String
    Public Property url As String

    Public Shared ReadOnly Property html As String

    Public Shared Iterator Function ParseLatest() As IEnumerable(Of NewsFeed)
        Dim html As String = latestNewsPageUrl.GET
        Dim items As String() = Strings.Split(html, "<div class=""product-set-item-info-inner-container"">")

        NewsFeed._html = html

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
