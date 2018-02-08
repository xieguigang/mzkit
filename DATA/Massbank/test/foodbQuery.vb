Imports Microsoft.VisualBasic.Data.csv
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.MassSpectrum.DATA.TMIC.FooDB
Imports SMRUCC.MassSpectrum.DATA.TMIC.HMDB

Module foodbQuery

    Sub Main()
        Dim mysql As MySqli = New ConnectionUri With {.IPAddress = "localhost", .Database = "foodb", .Password = "root", .Port = 3306, .User = "root"}
        Dim tests As metabolite() = "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\build_tools\CVD_kb\heart failure-hmdb.XML".LoadXml(Of metabolite())

        For Each metabolite In tests
            Dim foods = metabolite.GetAssociatedFoods(mysql).ToArray
            Call foods.SaveTo("./ffffff.csv")
        Next

        Pause()
    End Sub
End Module
