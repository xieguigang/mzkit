Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Node =
    System.Collections.Generic.Dictionary(Of
        String,
        Microsoft.VisualBasic.Language.List(Of String))

Public Module RecordIO

    Public Function LoadFile(txt$) As Record()
        Dim out As New List(Of Record)

        For Each data As String() In txt.ReadAllLines.Split("//")
            Dim nodes As Dictionary(Of String, Node) = data.__loadSection
            ' 文件区段读取完毕，开始生成数据对象
            Dim r As Record = nodes.__createObject

            out += r
        Next

        Return out
    End Function

    <Extension>
    Private Function __createObject(nodes As Dictionary(Of String, Node)) As Record
        Dim out As Record = DirectCast(GetType(Record).__createObject(nodes("$_")), Record)

        out.AC = DirectCast(GetType(AC).__createObject(nodes(NameOf(Record.AC))), AC)
        out.CH = DirectCast(GetType(CH).__createObject(nodes(NameOf(Record.CH))), CH)
        out.MS = DirectCast(GetType(MS).__createObject(nodes(NameOf(Record.MS))), MS)
        out.PK = nodes(NameOf(Record.PK)).__createPeaksData

        Return out
    End Function

    <Extension>
    Private Function __createPeaksData(node As Node) As PK
        Dim pk As New PK

        pk.NUM_PEAK = node(NameOf(pk.NUM_PEAK)).FirstOrDefault
        pk.SPLASH = node(NameOf(pk.SPLASH)).FirstOrDefault
        pk.ANNOTATION = node(NameOf(pk.ANNOTATION)) _
            .Select(Function(s$)
                        Dim t$() = s.Split
                        Dim i As int = Scan0

                        Return New AnnotationData With {
                            .mz = t(++i),
                            .tentative_formula = t(++i),
                            .formula_count = t(++i),
                            .mass = t(++i),
                            .delta_ppm = t(++i)
                        }
                    End Function) _
            .ToArray
        pk.PEAK = node(NameOf(pk.PEAK)) _
            .Select(Function(s$)
                        Dim t$() = s.Split
                        Dim i As int = Scan0

                        Return New PeakData With {
                            .mz = t(++i),
                            .int = t(++i),
                            .relint = t(++i)
                        }
                    End Function) _
            .ToArray

        Return pk
    End Function

    <Extension>
    Private Function __createObject(type As Type, node As Node) As Object
        Dim o As Object = Activator.CreateInstance(type)
        Dim schema = type.Schema(PropertyAccess.Writeable,, True)

        For Each name$ In node.Keys
            If schema(name).PropertyType Is GetType(String) Then
                Call schema(name).SetValue(o, node(name).FirstOrDefault)
            Else
                Call schema(name).SetValue(o, node(name).ToArray)
            End If
        Next

        Return o
    End Function

    <Extension>
    Private Function __loadSection(data$()) As Dictionary(Of String, Node)
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
                If Not Nodes("$_").ContainsKey(value.Name) Then
                    Nodes("$_")(value.Name) = New List(Of String)
                End If
                nodes("$_")(value.Name) += value.Value
            End If
        Next

        Return nodes
    End Function
End Module
