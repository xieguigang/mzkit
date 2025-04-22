Imports BioNovoGene.Analytical.MassSpectrometry.Lipidomics

Module Program

    Sub Main(args As String())
        Dim lipid = FacadeLipidParser.Default.Parse("PC 16:0_18:1")

        Pause()
    End Sub
End Module
