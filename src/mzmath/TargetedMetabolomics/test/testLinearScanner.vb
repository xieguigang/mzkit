Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.Serialization.JSON

Module testLinearScanner

    Sub Main()
        Dim raw = mzML.LoadChromatogramList("D:\test\111\cal\cal9.mzML").ToArray
        Dim ion As New IonPair With {.name = "3-Indoleacetic acid", .precursor = 173.95, .product = 129.9, .accession = "3-Indoleacetic acid"}

        Call raw.MRMSelector(ion, Tolerance.DeltaMass(0.1)).Plot.Save("D:\test\111\cal\test_IND.png")

        Dim target = raw.MRMSelector(ion, Tolerance.DeltaMass(0.1))
        Dim TIC = target.GetChromatogram
        Dim vector As IVector(Of ChromatogramTick) = TIC.Chromatogram.Shadows
        Dim ROIData As ROI() = vector _
            .PopulateROI(
                rt:={530, 580},
                baselineQuantile:=0.65,
                angleThreshold:=3,
                peakwidth:={8, 30},
                snThreshold:=-1
            ) _
            .ToArray

        Call Console.WriteLine(ROIData.GetJson)

        Pause()
    End Sub
End Module
