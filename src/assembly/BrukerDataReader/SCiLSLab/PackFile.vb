﻿Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Namespace SCiLSLab

    Public Delegate Function LineParser(Of T)(row As String(), headers As Index(Of String), println As Action(Of String)) As T

    Public Class PackFile

        Public Property comment As String
        Public Property export_time As Date
        Public Property raw As String
        Public Property fullName As String
        Public Property guid As String
        Public Property type As String
        Public Property create_time As Date
        Public Property metadata As Dictionary(Of String, String)

        Public Shared Function ParseHeader(file As Stream) As PackFile
            Using reader As New StreamReader(file)
                Dim byrefPack As New PackFile
                Call ParseHeader(reader, byrefPack, Nothing)
                Return byrefPack
            End Using
        End Function

        Public Shared Sub ParseHeader(reader As StreamReader, ByRef byrefPack As PackFile, ByRef headerLine As String)
            Dim line As Value(Of String) = ""
            Dim comments As New List(Of String)

            Do While (line = reader.ReadLine).StartsWith("#")
                Call comments.Add(line)
            Loop

            headerLine = line
            fillByrefPack(comments, byrefPack)
            byrefPack.metadata.Add(".header", headerLine)
        End Sub

        Protected Shared Iterator Function ParseTable(Of T)(file As Stream,
                                                            byrefPack As PackFile,
                                                            parseLine As LineParser(Of T),
                                                            println As Action(Of String)) As IEnumerable(Of T)
            Using reader As New StreamReader(file)
                Dim headerLine As String = Nothing
                Dim line As Value(Of String) = ""

                Call ParseHeader(reader, byrefPack, headerLine)
                ' Call byrefPack.metadata.Add(".header", headerLine)

                Dim headers As Index(Of String) = headerLine.Split(";"c).Indexing

                Do While (line = reader.ReadLine) IsNot Nothing
                    Yield parseLine(line.Split(";"c), headers, println)
                Loop
            End Using
        End Function

        Private Shared Sub fillByrefPack(comments As IEnumerable(Of String), [byref] As PackFile)
            Dim meta As Dictionary(Of String, String) = comments _
                .Where(Function(c) c.IndexOf(":"c) > -1) _
                .Select(Function(str)
                            Return str.GetTagValue(":", trim:=True)
                        End Function) _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return a.Value
                              End Function)

            [byref].comment = comments.Where(Function(c) c.IndexOf(":"c) = -1).JoinBy(vbCrLf)
            [byref].export_time = meta.TryPopOut("# Export time", [default]:=Now.ToString).ParseDate
            [byref].raw = meta.TryPopOut("# Generated from file")
            [byref].fullName = meta.TryPopOut("# Object Full Name")
            [byref].guid = meta.TryPopOut("# Object ID")
            [byref].type = meta.TryPopOut("# Object type")
            [byref].create_time = meta.TryPopOut("# Object creation time", [default]:=Now.ToString).ParseDate
            [byref].metadata = meta
        End Sub
    End Class
End Namespace