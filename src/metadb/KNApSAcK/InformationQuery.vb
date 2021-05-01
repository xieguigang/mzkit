Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class InformationQuery : Inherits WebQuery(Of String)

    Public Sub New(<CallerMemberName>
                   Optional cache As String = Nothing,
                   Optional interval As Integer = -1,
                   Optional offline As Boolean = False)

        Call MyBase.New(url:=AddressOf url,
                   contextGuid:=Function(cid) cid,
                   parser:=AddressOf parseHtml,
                   prefix:=Function(cid) cid,
                   cache:=cache,
                   interval:=interval,
                   offline:=offline
        )
    End Sub

    Private Shared Function url(cid As String) As String
        Return sprintf(My.Resources.knapsack_info, cid)
    End Function

    Private Shared Function parseHtml(html As String, type As Type) As Object
        Return parseInfo(html)
    End Function

    Private Shared Iterator Function parseOrgList(html As String) As IEnumerable(Of Organism)
        Dim rows = html.GetRowsHTML
        Dim cols As String()

        For Each rowText As String In rows
            cols = rowText.GetColumnsHTML

            Yield New Organism With {
                .Kingdom = cols(0).StripHTMLTags,
                .Family = cols(1).StripHTMLTags,
                .Species = cols(2).StripHTMLTags
            }
        Next
    End Function

    Private Shared Function parseInfo(html As String) As Information
        Dim tableRaw = html.GetTablesHTML(greedy:=True).FirstOrDefault
        Dim table As String = tableRaw.GetRowsHTML(greedy:=True).FirstOrDefault

        table = table.Replace(table.GetRowsHTML.First, "")

        Dim data = table.GetTablesHTML(greedy:=True).First.GetValue
        Dim orgHtml = data.GetTablesHTML.First
        Dim orgList = orgHtml.DoCall(AddressOf parseOrgList).ToArray
        Dim img As String = table.Replace(data, "").img.src
        Dim details = data _
            .Replace(orgHtml, "") _
            .GetRowsHTML _
            .Select(Function(r) r.GetColumnsHTML) _
            .ToDictionary(Function(r) r(Scan0).StripHTMLTags,
                          Function(r)
                              Return r(1) _
                                  .HtmlLines _
                                  .Select(Function(l) l.StripHTMLTags.Trim) _
                                  .Where(Function(s) Not s.StringEmpty) _
                                  .ToArray
                          End Function)

        Return New Information With {
            .name = details("Name"),
            .formula = details("Formula").FirstOrDefault,
            .mw = Val(details("Mw").FirstOrDefault),
            .CAS = details("CAS RN"),
            .CID = details("C_ID").First,
            .InChIKey = details("InChIKey").FirstOrDefault,
            .InChICode = details("InChICode").FirstOrDefault,
            .SMILES = details("SMILES").FirstOrDefault,
            .Biosynthesis = details("Start Substs in Alk. Biosynthesis (Prediction)").FirstOrDefault,
            .Organism = orgList,
            .img = img
        }
    End Function
End Class
