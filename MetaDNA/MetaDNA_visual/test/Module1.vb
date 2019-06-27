Imports MetaDNA.visual

Module Module1

    Sub Main()

        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\human_blood.Xml")
        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\urine.Xml")
        Call dump("D:\MassSpectrum-toolkits\MetaDNA\test\human_brain_tissue.Xml")

        Pause()
    End Sub

    Private Sub dump(file As String)
        Dim model = MetaDNA.visual.XML.LoadDocument(file)

        Call model.TranslateAsTable.Save(file.TrimSuffix)
    End Sub

End Module
