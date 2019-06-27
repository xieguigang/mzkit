Imports MetaDNA.visual

Module Module1

    Sub Main()

        Dim model = Global.MetaDNA.visual.XML.LoadDocument("D:\MassSpectrum-toolkits\MetaDNA\test\MetaDNA.Xml")
        Dim graph = model.CreateGraph

        Pause()
    End Sub

End Module
