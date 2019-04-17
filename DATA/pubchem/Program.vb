Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/unify.metalib")>
    <Usage("/unify.metalib /in <CID-Synonym-filtered.txt> [/out <out.Xml>]")>
    Public Function PubchemUnifyMetaLib(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.metlib.Xml"

        Using dataset As New DataSetWriter(Of MetaLib)(out)
            Dim i As VBInteger = 0

            For Each meta As MetaLib In CIDSynonym.LoadMetaInfo([in])
                Call dataset.Write(meta)

                If ++i Mod 100 = 0 Then
                    Call Console.Write(i)
                    Call Console.Write(vbTab)
                    Call dataset.Flush()
                End If
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/image.fly")>
    <Usage("/image.fly /cid <cid> [/out <save.png>]")>
    Public Function ImageFlyCLI(args As CommandLine) As Integer
        Dim cid$ = args("/cid")
        Dim out$ = args("/out") Or $"./{cid}.png"

        Return ImageFly.GetImage(cid) _
            .SaveAs(out) _
            .CLICode
    End Function
End Module
