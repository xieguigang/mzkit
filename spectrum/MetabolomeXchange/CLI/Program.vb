Imports System.Text
Imports MetabolomeXchange
Imports MetabolomeXchange.Json
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Module Program

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
        Dim json As response = [in].ReadAllText.LoadObject(Of response)
        Return json.datasets _
            .Values _
            .ToTable _
            .SaveTo(out, encoding:=Encoding.UTF8) _
            .CLICode
    End Function
End Module
