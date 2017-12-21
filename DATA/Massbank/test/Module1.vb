Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()

        '  Call dumpPubChem()


        Dim keys = SMRUCC.MassSpectrum.DATA.File.SDF.ScanKeys("D:\smartnucl_integrative\DATA\NCBI\SDF").ToArray

        Call keys.GetJson.SaveTo("./keys.json")


        Pause()
    End Sub


    Sub dumpPubChem()

        Call SMRUCC.MassSpectrum.DATA.File.Extensions.DumpingPubChemAnnotations("D:\smartnucl_integrative\DATA\NCBI\SDF", "./ddddd.csv")


        Pause()
    End Sub
End Module
