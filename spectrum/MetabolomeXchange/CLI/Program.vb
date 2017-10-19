Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language

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
End Module
