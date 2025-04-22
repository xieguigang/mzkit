Imports BioNovoGene.Analytical.MassSpectrometry.Lipidomics

Module Program

    Sub Main(args As String())
        Dim lipid = FacadeLipidParser.Default.Parse("PC 16:1COOH_16:0")

        Pause()
    End Sub
End Module
