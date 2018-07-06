Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports SMRUCC.MassSpectrum.Assembly
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzXML

Module read_mzXML

    Sub Main()
        Dim mzXML = "E:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\biodeepMSMS\test\lxy-CID30.mzXML"
        Dim scanes = MarkupData.mzXML.XML.LoadScans(mzXML).ToArray
        Dim file As New StreamWriter("./test_ms2_dump.txt")

        For Each scan In scanes.Where(Function(s) s.msLevel <> "1")
            Dim ms2Peaks = scan.ExtractMzI

            Call file.WriteLine(ms2Peaks.name)
            Call file.WriteLine(ms2Peaks.peaks.Print)
            Call file.WriteLine()
        Next

        Call file.Flush()
        Call file.Close()
        Call file.Dispose()

        Pause()
    End Sub
End Module
