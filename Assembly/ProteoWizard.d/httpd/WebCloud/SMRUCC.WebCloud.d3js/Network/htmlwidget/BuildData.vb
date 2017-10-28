Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports NetGraphData = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkTables

Namespace Network.htmlwidget

    ''' <summary>
    ''' 将``htmlwidget``之中的D3.js网络模型解析为scibasic的标准网络模型
    ''' </summary>
    Public Module BuildData

        Const JSON$ = "<script type[=]""application/json"".+?</script>"

        ''' <summary>
        ''' 参数为html文本或者url路径
        ''' </summary>
        ''' <param name="html$"></param>
        ''' <returns></returns>
        Public Function ParseHTML(html$) As String
            If html.FileExists Then
                html = html.GET
            End If

            html = Regex.Match(html, BuildData.JSON, RegexICSng).Value
            html = html.GetStackValue(">", "<")

            Return html
        End Function

        Public Function BuildGraph(html$) As NetGraphData
            Dim json$ = BuildData.ParseHTML(html)
            Dim data As htmlwidget.NetGraph = json.LoadObject(Of htmlwidget.JSON).x
            Dim nodes As New List(Of Node)
            Dim edges As New List(Of NetworkEdge)

            For i As Integer = 0 To data.nodes.name.Length - 1
                Dim name$ = data.nodes.name(i)
                Dim type$ = data.nodes.group(i)

                nodes += New Node With {
                    .ID = name,
                    .NodeType = type
                }
            Next

            Dim nodesVector As Node() = nodes.ToArray

            For i As Integer = 0 To data.links.source.Length - 1
                Dim src = nodesVector(data.links.source(i)).ID
                Dim tar = nodesVector(data.links.target(i)).ID
                Dim type = data.links.colour(i)

                edges += New NetworkEdge With {
                    .FromNode = src, 
                    .ToNode = tar, 
                    .value = 1, 
                    .Interaction = type
                }
            Next

            Dim net As New NetGraphData With {
                .Nodes = nodes,
                .Edges = edges
            }
            Return net
        End Function
    End Module
End Namespace