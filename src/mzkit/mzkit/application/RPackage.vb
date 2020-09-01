Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Task

Namespace My

    Partial Class MyApplication

        Shared Sub New()

        End Sub

        ''' <summary>
        ''' Get a list of raw files that opened in current workspace
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("list.raw")>
        Public Shared Function ListFiles() As Raw()
            Dim list As New List(Of Raw)

            For i As Integer = 0 To host.fileExplorer.treeView1.Nodes.Count - 1
                list.Add(host.fileExplorer.treeView1.Nodes(i).Tag)
            Next

            Return list.ToArray
        End Function
    End Class
End Namespace