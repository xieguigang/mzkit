Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization

Module testLinearScanner

    Sub Main()
        Dim raw = mzML.LoadChromatogramList("D:\test\111\cal\cal1.mzML").ToArray
        Dim ion As New IonPair With {.name = "3-Indoleacetic acid", .precursor = 173.95, .product = 129.9, .accession = "3-Indoleacetic acid"}

        Call raw.MRMSelector(ion, Tolerance.DeltaMass(0.1)).Plot.Save("D:\test\111\cal\cal1.png")

        Pause()
    End Sub
End Module
