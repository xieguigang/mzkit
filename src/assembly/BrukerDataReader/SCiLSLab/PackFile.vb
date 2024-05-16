#Region "Microsoft.VisualBasic::8a225349d49ed4a38135c7391ab06d3e, assembly\BrukerDataReader\SCiLSLab\PackFile.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 104
    '    Code Lines: 78
    ' Comment Lines: 10
    '   Blank Lines: 16
    '     File Size: 4.58 KB


    '     Delegate Function
    ' 
    ' 
    '     Class PackFile
    ' 
    '         Properties: comment, create_time, export_time, fullName, guid
    '                     metadata, raw, type
    ' 
    '         Function: GetGuid, (+2 Overloads) ParseHeader, ParseTable
    ' 
    '         Sub: fillByrefPack, ParseHeader
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Namespace SCiLSLab

    Public Delegate Function LineParser(Of T)(row As String(), headers As Index(Of String), println As Action(Of String)) As T

    Public Class PackFile

        Public Property comment As String
        Public Property export_time As Date
        ''' <summary>
        ''' the raw data source file path
        ''' </summary>
        ''' <returns></returns>
        Public Property raw As String
        Public Property fullName As String
        Public Property guid As String
        Public Property type As String
        Public Property create_time As Date
        Public Property metadata As Dictionary(Of String, String)

        Public Shared Function ParseHeader(file As String) As PackFile
            Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return ParseHeader(buffer)
            End Using
        End Function

        Public Shared Function ParseHeader(file As Stream) As PackFile
            Using reader As New StreamReader(file)
                Dim byrefPack As New PackFile
                Call ParseHeader(reader, byrefPack, Nothing)
                Return byrefPack
            End Using
        End Function

        Public Shared Function GetGuid(file As String) As String
            Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Dim header As PackFile = ParseHeader(buffer)
                Return header.guid
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
            ' 20220816
            ' using closure at here will cause the 
            ' file closed unexpected, and make the downstream
            ' data imports function crashed
            ' so removes the using closure at here
            Dim reader As New StreamReader(file)
            Dim headerLine As String = Nothing
            Dim line As Value(Of String) = ""

            Call ParseHeader(reader, byrefPack, headerLine)
            ' Call byrefPack.metadata.Add(".header", headerLine)

            Dim headers As Index(Of String) = headerLine.Split(";"c).Indexing

            Do While (line = reader.ReadLine) IsNot Nothing
                Yield parseLine(line.Split(";"c), headers, println)
            Loop
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
