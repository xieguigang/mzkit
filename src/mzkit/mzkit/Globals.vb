Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Task

Module Globals

    ReadOnly cacheList As String = App.LocalData & "/cacheList.json"

    <Extension>
    Public Sub SaveRawFileCache(explorer As TreeView)
        Dim files As New List(Of Task.Raw)

        For Each node As TreeNode In explorer.Nodes
            files.Add(node.Tag)
        Next

        Call files.ToArray.GetJson.SaveTo(cacheList)
    End Sub

    <Extension>
    Public Sub LoadRawFileCache(explorer As TreeView)
        Dim files = cacheList.LoadJsonFile(Of Task.Raw())

        For Each raw As Raw In files.SafeQuery
            Call explorer.addRawFile(raw)
        Next
    End Sub

    <Extension>
    Public Sub addRawFile(explorer As TreeView, raw As Raw)
        Dim rawFileNode As New TreeNode($"{raw.source.FileName} [{raw.scans} Scans]") With {
                .Checked = True,
                .Tag = raw
            }

        explorer.Nodes.Add(rawFileNode)

        For Each id As String In raw.scanIDList
            rawFileNode.Nodes.Add(New TreeNode(id))
        Next
    End Sub
End Module
