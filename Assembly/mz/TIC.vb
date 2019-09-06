Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.Runtime

Partial Module Program

    <ExportAPI("/selective.TIC")>
    <Usage("/selective.TIC /mz <mz.list> /raw <raw.mzXML> [/out <TIC.png>]")>
    Public Function SelectiveTIC(args As CommandLine) As Integer
        Dim mz$ = args <= "/mz"
        Dim raw$ = args <= "/raw"
        Dim out$ = args("/out") Or $"{mz.TrimSuffix}-{raw.FileName}_TIC.png"
        Dim mzlist As Double() = mz.ReadAllLines.AsDouble


    End Function
End Module
