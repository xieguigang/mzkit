Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports SMRUCC.MassSpectrum.DATA.NCBI.PubChem

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/unify.metalib")>
    <Usage("/unify.metalib /in <CID-Synonym-filtered.txt> /out <out.Xml>")>
    Public Function PubchemUnifyMetaLib(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.metlib.Xml"

        Using dataset As New DataSetWriter(Of MetaLib)(out)
            For Each meta As MetaLib In CIDSynonym.LoadMetaInfo([in])
                Call dataset.Write(meta)
            Next
        End Using

        Return 0
    End Function
End Module
