Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Partial Module CLI

    <ExportAPI("/image.fly")>
    <Usage("/image.fly /cid <cid> [/out <save.png>]")>
    Public Function ImageFlyCLI(args As CommandLine) As Integer
        Dim cid$ = args("/cid")
        Dim out$ = args("/out") Or $"./{cid}.png"

        Return ImageFly.GetImage(cid) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/query")>
    <Description("Do pubchem database query.")>
    <Usage("/query /terms <list.txt/json/xml> [/include.image /out <directory>]")>
    <Argument("/terms", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String())},
              Extensions:="*.txt, *.json, *.xml",
              Description:="This parameter should be a string list of names")>
    <Argument("/include.image", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Also includes 2D structure images.")>
    Public Function QueryPubchem(args As CommandLine) As Integer
        Dim in$ = args <= "/terms"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.query_result/"
        Dim terms$() = [in].SolveListStream.ToArray
        Dim includeImage As Boolean = args("/include.image")
    End Function
End Module