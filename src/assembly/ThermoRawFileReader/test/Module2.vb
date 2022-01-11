Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader

Module Module2

    Sub Main()
        Dim wiff As New WiffScanFileReader("E:\mzkit\DATA\test\wiff\MTBLS691\KIT2-0-5504_1020804070_01_0_1_1_01_10000001.wiff")

        Dim ms = wiff.GetCentroidFromScanNum(10)

        Pause()
    End Sub
End Module
