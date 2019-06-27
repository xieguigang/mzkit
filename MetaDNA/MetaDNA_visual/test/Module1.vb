Imports MetaDNA.visual
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream

Module Module1

    Sub Main()

        Dim model = Global.MetaDNA.visual.XML.LoadDocument("D:\MassSpectrum-toolkits\MetaDNA\test\MetaDNA.Xml")
        Dim graph = model.CreateGraph
        Dim data = graph.Tabular

        Call data.Save("D:\MassSpectrum-toolkits\MetaDNA\test\MetaDNA\")

        Pause()
    End Sub

End Module
