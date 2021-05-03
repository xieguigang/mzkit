Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader

Module Module1

    Sub Main()
        Using file As FileStream = "E:\mzkit\DATA\test\Angiotensin_AllScans.mzPack".Open
            Call New MSFileReader("E:\mzkit\DATA\test\Angiotensin_AllScans.raw") _
                .LoadFromXRaw _
                .Write(file)
        End Using

        Pause()
    End Sub

End Module
