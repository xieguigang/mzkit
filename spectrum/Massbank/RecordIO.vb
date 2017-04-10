Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Node = System.Collections.Generic.Dictionary(Of String, Microsoft.VisualBasic.Language.List(Of String))

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
            Dim o As Object = Activator.CreateInstance(GetType(Record))
            Dim schema = DataFramework.Schema(
                GetType(Record),
                PropertyAccess.Writeable,,
                True)

            For Each line$ In data
                Dim value As NamedValue(Of String) = line.GetTagValue(":", trim:=True)

                If value.Name.Contains("$") Then
                    Dim nodeName = value.Name.GetTagValue("$")
                    Dim node As Node = nodes(nodeName.Name)

                    If Not node.ContainsKey(nodeName.Value) Then
                        node(nodeName.Value) = New List(Of String)
                    End If

                    node(nodeName.Value) += value.Value
                Else
                    If Not nodes("$_").ContainsKey(value.Name) Then
                        nodes("$_")(value.Name) = New List(Of String)
                    End If

                    nodes("$_")(value.Name) += value.Value
                End If
            Next
        Next

        Return out
    End Function
End Module
