Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.MassSpectrum.DATA.TMIC.FooDB
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Module foodbQuery

    Dim mysql As MySqli = New ConnectionUri With {.IPAddress = "localhost", .Database = "foodb", .Password = "root", .Port = 3306, .User = "root"}

    Sub Main()
        Call subset()
        Call queryByContent()

        ' Call queryByID()
    End Sub

    Sub queryByID()


    End Sub

    Sub subset()
        Dim hmdb = metabolite.Load("D:\smartnucl_integrative\DATA\2017-12-22.MetaReference\hmdb_metabolites.xml")
        Dim index As Index(Of String) = {
            "HMDB0000043",
            "HMDB0000097",
            "HMDB0000925",
            "HMDB0000562",
            "HMDB0000062",
            "HMDB0000742",
            "HMDB0000172",
            "HMDB0000883",
            "HMDB0000687",
            "HMDB0000159",
            "HMDB0000158",
            "HMDB0000929",
"HMDB0000906"}.Indexing

        For Each m In hmdb
            If m.accession.IsOneOfA(index) Then
                Call {m}.GetXml.SaveTo($"./hmdb/{m.accession}.xml")
            ElseIf Not m.secondary_accessions.accession Is Nothing Then
                For Each id In m.secondary_accessions.accession
                    If id.IsOneOfA(index) Then
                        Call {m}.GetXml.SaveTo($"./hmdb/{m.accession}.xml")
                    End If
                Next
            End If
        Next
    End Sub

    Sub queryByContent()
        For Each xml As String In "D:\MassSpectrum-toolkits\DATA\Massbank\test\bin\x64\Release\hmdb".EnumerateFiles("*.Xml")
            Dim tests As metabolite() = xml.LoadXml(Of metabolite())

            For Each metabolite In tests
                Dim foods = metabolite.GetAssociatedFoods(mysql).ToArray
                If foods.Length > 0 Then
                    Call foods.SaveTo($"./foods/{metabolite.accession}-{foods.First.name.NormalizePathString}.csv")
                End If
            Next
        Next


        Pause()
    End Sub
End Module
