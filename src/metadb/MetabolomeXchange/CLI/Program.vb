#Region "Microsoft.VisualBasic::7941edf2f17daf33613c020629c2fd2a, src\metadb\MetabolomeXchange\CLI\Program.vb"

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

' Module Program
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: DumpTable, GetJson, Main, ToTable
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports MetabolomeXchange
Imports MetabolomeXchange.Json
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

    Sub New()
        'Dim data As New DataSet With {
        '    .accession = "123",
        '    .date = Now.ToString,
        '    .description = {"AAAA"},
        '    .publications = {New publication With {.pubmed = "444444", .doi = "doi_string", .title = "ffffffffffff"}},
        '    .title = "AAAAAAsd",
        '    .submitter = {"QQQQQ", "DDDDDD"},
        '    .timestamp = 1234567,
        '    .url = "ASDFFFFFFF",
        '    .meta = New meta With {
        '        .analysis = "AAAAA",
        '        .metabolites = {"X", "FGFFFF"},
        '        .organism = {"PPPP", "OOOO"},
        '        .organism_parts = {"DDCCCC", "KKKKKK"},
        '        .platform = "XXXXXXXXXXXXXXXXXXX"
        '    }
        '}

        'Call data.GetJson.SaveTo("./test.json")
    End Sub

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/dataset.json")>
    <Usage("/dataset.json [/provider <shortname, default=""mtbls""> /out <save.json>]")>
    Public Function GetJson(args As CommandLine) As Integer
        Dim provider$ = (args <= "/provider") Or "mtbls".AsDefault
        Dim out$ = (args <= "/out") Or $"{App.HOME}/metabolomexchange_dataset.json".AsDefault

        Return MetabolomeXchange _
            .GetAllDataSetJson(provider) _
            .SaveTo(out, Encoding.UTF8) _
            .CLICode
    End Function

    <ExportAPI("/dump.table")>
    <Usage("/dump.table /in <json.txt> [/out <out.csv>]")>
    Public Function DumpTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".csv").AsDefault

        Dim model = [in].ReadAllText.ParseJson
        Return model _
            .ToTable() _
            .ToArray _
            .SaveTo(out, encoding:=Encoding.UTF8) _
            .CLICode

        Dim json = [in].ReadAllText.LoadJSON(Of Dictionary(Of String, DataSet))
        Return json _
            .Values _
            .ToTable _
            .SaveTo(out, encoding:=Encoding.UTF8) _
            .CLICode
    End Function

    <Extension>
    Public Iterator Function ToTable(json As JsonElement) As IEnumerable(Of DataTable)
        Dim datasets As JsonObject = DirectCast(json, JsonObject)!datasets

        For Each node As JsonObject In datasets.Values
            Dim meta As JsonObject = node!meta
            Dim metabolites$()() = meta!metabolites.AsStringVector().Split(100)
            Dim i As i32 = 0

            Yield New DataTable With {
                .analysis = meta!analysis.AsString,
                .ID = node!accession.AsString,
                .date = Date.Parse(node!date.AsString),
                .metabolites1 = metabolites.ElementAtOrDefault(++i),
                .metabolites2 = metabolites.ElementAtOrDefault(++i),
                .metabolites3 = metabolites.ElementAtOrDefault(++i),
                .metabolites4 = metabolites.ElementAtOrDefault(++i),
                .metabolites5 = metabolites.ElementAtOrDefault(++i),
                .metabolites6 = metabolites.ElementAtOrDefault(++i),
                .metabolites7 = metabolites.ElementAtOrDefault(++i),
                .metabolites8 = metabolites.ElementAtOrDefault(++i),
                .metabolites9 = metabolites.ElementAtOrDefault(++i),
                .metabolites10 = metabolites.ElementAtOrDefault(++i),
                .metabolites11 = metabolites.ElementAtOrDefault(++i),
                .metabolites12 = metabolites.ElementAtOrDefault(++i),
                .metabolites13 = metabolites.ElementAtOrDefault(++i),
                .metabolites14 = metabolites.ElementAtOrDefault(++i),
                .metabolites15 = metabolites.ElementAtOrDefault(++i),
                .metabolites16 = metabolites.ElementAtOrDefault(++i),
                .metabolites17 = metabolites.ElementAtOrDefault(++i),
                .metabolites18 = metabolites.ElementAtOrDefault(++i),
                .metabolites19 = metabolites.ElementAtOrDefault(++i),
                .metabolites20 = metabolites.ElementAtOrDefault(++i),
                .platform = meta!platform.AsString,
                .submitter = node!submitter.AsStringVector,
                .organism = meta!organism.AsStringVector,
                .organism_parts = meta!organism_parts.AsStringVector,
                .title = node!title.AsString
            }
        Next
    End Function
End Module
