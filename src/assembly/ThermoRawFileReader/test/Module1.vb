Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader

Module Module1

    Sub Main()
        Dim raw As New MSFileReader("E:\mzkit\DATA\test\Angiotensin_AllScans.raw")

        Call raw.LoadFile()

        For Each scan As RawLabelData In raw.GetLabelData
            Call Console.WriteLine(scan.ToString)
        Next

        Pause()
    End Sub

End Module
