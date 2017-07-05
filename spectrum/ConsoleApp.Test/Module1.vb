Imports Microsoft.VisualBasic.Data.csv.IO

Module Module1

    Sub Main()

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
