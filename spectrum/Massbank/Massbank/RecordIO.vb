Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.DATA
Imports Node =
    System.Collections.Generic.Dictionary(Of
        String,
        Microsoft.VisualBasic.Language.List(Of String))

Public Module RecordIO

    Public Function ScanLoad(DIR$) As Record()
        Dim out As New List(Of Record)

        For Each record$ In ls - l - r - "*.txt" <= DIR
            out += RecordIO.LoadFile(txt:=record)
        Next

        Return out
    End Function

    Public Function LoadFile(txt$) As Record()
        Dim out As New List(Of Record)

        For Each data As String() In txt.ReadAllLines.Split("//")
            Dim annotationHeaders$() = Nothing
            Dim nodes As Dictionary(Of String, Node) = data.__loadSection(annotationHeaders)
            ' 文件区段读取完毕，开始生成数据对象
            Dim r As Record = nodes.__createObject(annotationHeaders)

            out += r
        Next

        Return out
    End Function

    <Extension>
    Private Function __createObject(nodes As Dictionary(Of String, Node), annotationHeaders$()) As Record
        Dim out As Record = DirectCast(GetType(Record).__createObject(nodes("$_")), Record)

        out.AC = DirectCast(GetType(AC).__createObject(nodes(NameOf(Record.AC))), AC)
        out.CH = DirectCast(GetType(CH).__createObject(nodes(NameOf(Record.CH))), CH)
        out.MS = DirectCast(GetType(DATA.MS).__createObject(nodes(NameOf(Record.MS))), DATA.MS)
        out.SP = DirectCast(GetType(SP).__createObject(nodes.TryGetValue(NameOf(Record.SP))), SP)
        out.PK = nodes(NameOf(Record.PK)).__createPeaksData(annotationHeaders)

        Return out
    End Function

    <Extension>
    Private Function __createPeaksData(node As Node, annotationHeaders$()) As PK
        Dim pk As New PK

        pk.NUM_PEAK = node.TryGetValue(NameOf(pk.NUM_PEAK)).DefaultFirst
        pk.SPLASH = node.TryGetValue(NameOf(pk.SPLASH)).DefaultFirst
        Try
            pk.ANNOTATION = node.TryGetValue(NameOf(pk.ANNOTATION)) _
                .SafeQuery _
                .SeqIterator _
                .Select(Function(s)
                            Dim t$() = (+s).Split
                            Dim table As PropertyValue() =
                                t _
                                .Where(Function(ss) Not ss.StringEmpty) _
                                .SeqIterator _
                                .Select(Function(k)
                                            Return New PropertyValue With {
                                                .Key = k.i,
                                                .Property = annotationHeaders(k),
                                                .Value = +k
                                            }
                                        End Function) _
                                .ToArray

                            Return New Entity With {
                                .ID = s.i,
                                .Properties = table
                            }
                        End Function) _
                .ToArray
        Catch ex As Exception

        End Try
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

        If node Is Nothing Then
            Return o
        End If

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
    Private Function __loadSection(data$(), ByRef annotationHeaders$()) As Dictionary(Of String, Node)
        Dim nodes As New Dictionary(Of String, Node) From {
            {"$_", New Node},
            {"CH", New Node},
            {"AC", New Node},
            {"MS", New Node},
            {"PK", New Node},
            {"SP", New Node}
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
                If line.First = " "c OrElse Not (line.Contains("$") OrElse line.Contains(":")) Then
                    Call appendNodeData(table, value:=line.Trim)
                    Continue For
                Else
                    readTable = False
                End If
            End If

            If value.Name = "PK$ANNOTATION" OrElse value.Name = "PK$PEAK" Then
                table = value.Name
                readTable = True

                If value.Name = "PK$ANNOTATION" Then
                    annotationHeaders = value.Value.Split
                End If

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

        Return nodes
    End Function
End Module
