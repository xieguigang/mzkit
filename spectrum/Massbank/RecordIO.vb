Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Node =
    System.Collections.Generic.Dictionary(Of
        String,
        Microsoft.VisualBasic.Language.List(Of String))

Public Module RecordIO

    Public Function LoadFile(txt$) As Record()
        Dim out As New List(Of Record)

        Static type As Type = GetType(Record)

        For Each data As String() In txt.ReadAllLines.Split("//")
            Dim nodes As New Dictionary(Of String, Node) From {
                {"$_", New Node},
                {"CH", New Node},
                {"AC", New Node},
                {"MS", New Node},
                {"PK", New Node}
            }
            Dim table$ = ""
            Dim readTable As Boolean = False
            Dim appendNodeData =
                Sub(path$, value$)
                    Dim nodeName As NamedValue(Of String) = path.GetTagValue("$")
                    Dim node As Node = nodes(nodeName.Name)

                    If Not node.ContainsKey(nodeName.Value) Then
                        node(nodeName.Value) = New List(Of String)
                    End If

                    node(nodeName.Value) += value
                End Sub

            For Each line$ In data
                Dim value As NamedValue(Of String) = line.GetTagValue(":", trim:=True)

                If readTable Then
                    If line.First = " "c Then
                        Call appendNodeData(table, value:=line.Trim)
                        Continue For
                    Else
                        readTable = False
                    End If
                End If

                If value.Name = "PK$ANNOTATION" OrElse value.Name = "PK$PEAK" Then
                    table = value.Name
                    readTable = True
                    Continue For
                End If

                If value.Name.Contains("$") Then
                    With value
                        Call appendNodeData(
                            path:= .Name,
                            value:= .Value)
                    End With
                Else
                    If Not nodes("$_").ContainsKey(value.Name) Then
                        nodes("$_")(value.Name) = New List(Of String)
                    End If

                    nodes("$_")(value.Name) += value.Value
                End If
            Next

            ' 文件区段读取完毕，开始生成数据对象
            Dim o As Object = Activator.CreateInstance(GetType(Record))
            Dim schema = DataFramework.Schema(
                GetType(Record),
                PropertyAccess.Writeable,,
                True)
        Next

        Return out
    End Function
End Module
