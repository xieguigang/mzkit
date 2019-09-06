Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.Runtime

Partial Module Program

    <ExportAPI("/selective.TIC")>
    <Usage("/selective.TIC /mz <mz.list> /raw <raw.mzXML> [/out <TIC.png>]")>
    <Description("Do TIC plot on a given list of selective parent ions.")>
    <Argument("/mz", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Extensions:="*.txt, *.csv",
              Description:="A list file for specific the m/z values.")>
    <Argument("/out", True, CLITypes.File,
              Extensions:="*.png, *.svg",
              Description:="The output TIC plot image file path.")>
    Public Function SelectiveTIC(args As CommandLine) As Integer
        Dim mz$ = args <= "/mz"
        Dim raw$ = args <= "/raw"
        Dim out$ = args("/out") Or $"{mz.TrimSuffix}-{raw.FileName}_TIC.png"
        Dim mzlist As Double() = mz.ReadAllLines.AsDouble


    End Function
End Module
