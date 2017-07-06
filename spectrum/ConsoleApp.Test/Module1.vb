Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Text

Module Module1

    Sub Main()

        Dim ddd = SMRUCC.proteomics.MS_Spectrum.DATA.Statistics.KEGGPathwayCoverages("C:\Users\gg.xie\Desktop\KEGG_ALL.csv".ReadAllLines, "D:\smartnucl_integrative\DATA\KEGG\br08901")
        Dim ff As New File

        ff += {"pathway", "cover", "ALL", "coverage%"}

        For Each row In ddd
            ff += New String() {row.Key, row.Value.cover, row.Value.ALL, If(row.Value.ALL = 0, 0, (row.Value.cover / row.Value.ALL) * 100%)}
        Next

        Call ff.Save("eeeee.csv", Encodings.ASCII)


        Pause()

        Dim unknown As Integer = 0
        Dim result = SMRUCC.proteomics.MS_Spectrum.DATA.Statistics.HMDBCoverages("C:\Users\xieguigang\Desktop\New folder\metlin.hmdb.txt".ReadAllLines, "C:\Users\xieguigang\Desktop\hmdb_metabolites.xml", unknown)

        Dim pie = result.Where(Function(item) Not item.Key.StringEmpty).Where(Function(n) n.Value.cover > 0).ToArray
        Dim csv As New File

        csv += {"Class", "Percentage"}

        For Each item In pie
            csv += {item.Key, CStr(item.Value.cover / item.Value.ALL)}
        Next

        Call csv.Save("x:\dsgfdfgdf.csv")

        Pause()

        For Each m In SMRUCC.proteomics.MS_Spectrum.DATA.HMDB.metabolite.Load("C:\Users\xieguigang\Desktop\hmdb_metabolites.xml")

            m.name.__DEBUG_ECHO

        Next

        Dim rs = SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.RecordIO.ScanLoad("D:\smartnucl_integrative\DATA\OpenData\record\Athens_Univ\")

        Pause()
    End Sub
End Module
