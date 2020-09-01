Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports Task
Imports REnv = SMRUCC.Rsharp.Runtime.Internal

Namespace My

    Partial Class MyApplication

        Shared Sub New()
            REnv.Object.Converts.makeDataframe.addHandler(GetType(Raw()), AddressOf rawDataFrame)
        End Sub

        Private Shared Function rawDataFrame(raws As Raw(), args As list, env As Environment) As dataframe
            Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

            table.columns(NameOf(Raw.source)) = raws.Select(Function(a) a.source).ToArray
            table.columns(NameOf(Raw.rtmin)) = raws.Select(Function(a) a.rtmin).ToArray
            table.columns(NameOf(Raw.rtmax)) = raws.Select(Function(a) a.rtmax).ToArray
            table.columns(NameOf(Raw.numOfScans)) = raws.Select(Function(a) a.numOfScans).ToArray
            table.columns(NameOf(Raw.cache)) = raws.Select(Function(a) a.cache).ToArray
            table.columns("file_size") = raws.Select(Function(a) Lanudry(a.source.FileLength)).ToArray

            table.rownames = raws.Select(Function(a) a.source.FileName).ToArray

            Return table
        End Function

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