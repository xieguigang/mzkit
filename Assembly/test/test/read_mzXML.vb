Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.MassSpectrum.Assembly
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzXML

Module read_mzXML

    Dim mzXML = "D:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\biodeepMSMS\test\lxy-CID30.mzXML"

    Sub Main()
        ' Call exportMs1()
        Call exportMs2()
    End Sub

    Sub exportMs1()
        Dim table = MarkupData.mzXML.XML.ExportPeaktable(mzXML).ToArray

        Call table.SaveTo("./ms1_peaktable.csv")
    End Sub

    Sub exportMs2()
        Dim scanes = MarkupData.mzXML.XML.LoadScans(mzXML).ToArray
        Dim file As New StreamWriter("./lxy-CID30.txt")

        For Each scan In scanes.Where(Function(s) s.msLevel <> "1")
            Dim ms2Peaks = scan.ExtractMzI

            Call file.WriteLine(ms2Peaks.name)
            Call file.WriteLine($"mz range: [{scan.lowMz}, {scan.highMz}]")
            Call file.WriteLine($"peaks: {scan.peaksCount}")
            Call file.WriteLine($"activation: {scan.precursorMz.activationMethod} @ {scan.collisionEnergy}V")
            Call file.WriteLine(ms2Peaks.peaks.Print(addBorder:=False))
            Call file.WriteLine()
        Next

        Call file.Flush()
        Call file.Close()
        Call file.Dispose()

        Pause()
    End Sub
End Module
