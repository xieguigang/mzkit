Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON

Module testTablkeReader

    Sub Main()
        Dim table = "E:\biodeep\metauhealth\mzkit-desktop\demo\demo_test20241125\903医院初始数据示例.csv".LoadCsv(Of ScalarPeakReport)
        Dim samples = ScalarPeakReport.ExtractSampleData(table).ToArray

        Call samples.GetJson.SaveTo("./aaaa.json")

        Pause()
    End Sub
End Module
