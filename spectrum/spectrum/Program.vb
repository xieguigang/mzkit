Imports Microsoft.VisualBasic.Imaging

Module Program

    Public Function Main() As Integer
        Call Test()
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub Test()
        Call Data.Load("G:\spectrum\SpectrumChart\Spectrum.json").Data(Scan0).Plot.SaveAs("G:\spectrum\spectrum\Plot.png")
    End Sub
End Module
