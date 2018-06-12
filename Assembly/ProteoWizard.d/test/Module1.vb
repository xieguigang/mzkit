Imports Microsoft.VisualBasic.ApplicationServices

Module Module1

    Sub Main()

        Call testExtractZip()


    End Sub

    Sub testExtractZip()
        Dim zip1 = "C:\Users\administrator\Desktop\95-1.D.zip"
        Dim test1 = GZip.IsSourceFolderZip(zip1)

        Dim zip2 = "C:\Users\administrator\Desktop\95-1.D-flat.zip"
        Dim test2 = GZip.IsSourceFolderZip(zip2)

        Call GZip.ImprovedExtractToDirectory(zip1, "./ddddddd", extractToFlat:=True)


    End Sub

End Module
