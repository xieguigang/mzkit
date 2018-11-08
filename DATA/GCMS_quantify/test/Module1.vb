Imports GCMS_quantify
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()
        Dim data = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\scfa200ppmAIAEXPRT.AIA\20ppm-未处理.CDF")

        Call data.GetJson.SaveTo("./gcms.json")

        Pause()
    End Sub

End Module
