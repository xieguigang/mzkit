Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Serialization.JSON

Module testTablkeReader

    Sub Main()
        Dim table = "E:\biodeep\metauhealth\mzkit-desktop\demo\demo_test20241125\903医院初始数据示例.csv".LoadCsv(Of ScalarPeakReport)
        Dim samples As DataFile() = ScalarPeakReport.ExtractSampleData(table).ToArray

        Call samples.GetJson.SaveTo("./aaaa.json")
        Call New Experiment With {.DataFiles = samples}.GetXml.SaveTo("./aaaaaa.xml")

        Pause()
    End Sub
End Module
