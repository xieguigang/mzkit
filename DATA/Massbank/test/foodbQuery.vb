Imports Microsoft.VisualBasic.Data.csv
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.MassSpectrum.DATA.TMIC.FooDB
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Module foodbQuery

    Sub Main()
        Dim mysql As MySqli = New ConnectionUri With {.IPAddress = "localhost", .Database = "foodb", .Password = "root", .Port = 3306, .User = "root"}

        For Each xml As String In "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb".EnumerateFiles("*.Xml")
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
