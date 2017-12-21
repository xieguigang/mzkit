Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()
        Dim keys = SMRUCC.MassSpectrum.DATA.File.SDF.ScanKeys("D:\smartnucl_integrative\DATA\NCBI\SDF").ToArray

        Call keys.GetJson.SaveTo("./keys.json")


        Pause()
    End Sub
End Module
