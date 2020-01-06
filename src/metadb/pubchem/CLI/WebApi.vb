#Region "Microsoft.VisualBasic::021c1099d00b3d88d67545c573498540, src\metadb\pubchem\CLI\WebApi.vb"

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

    ' Module CLI
    ' 
    '     Function: ImageFlyCLI, QueryPubchem
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Threading
Imports BioNovoGene.BioDeep.Chemistry.NCBI
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging

Partial Module CLI

    <ExportAPI("/image.fly")>
    <Usage("/image.fly /cid <cid> [/transparent /size <w,h default=300,300> /out <save.png>]")>
    <Group(Program.WebApiCli)>
    <Argument("/transparent", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Make the image transparent?")>
    Public Function ImageFlyCLI(args As CommandLine) As Integer
        Dim cid$ = args("/cid")
        Dim size$ = args("/size") Or "300,300"
        Dim out$ = args("/out") Or $"./{cid}_size={size}.png"
        Dim doTransparent As Boolean = args("/transparent")

        Return ImageFly.GetImage(cid, size, doTransparent) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/query")>
    <Description("Do pubchem database query.")>
    <Usage("/query /terms <list.txt/json/xml> [/include.image /out <directory>]")>
    <Group(Program.WebApiCli)>
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
        Dim cid$()

        For Each term As String In terms
            cid = PubChem _
                .QueryPugViews(term, cacheFolder:=$"{out}/.cache/") _
                .Keys _
                .ToArray

            If includeImage Then
                For Each id As String In cid
                    Call ImageFly _
                        .GetImage(id, 500, 500, doBgTransparent:=False) _
                        .SaveAs($"{out}/{id}.png")
                    Call Thread.Sleep(1000)
                Next
            End If
        Next

        Return 0
    End Function
End Module
