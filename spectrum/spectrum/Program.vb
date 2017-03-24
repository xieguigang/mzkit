Imports Microsoft.VisualBasic.Imaging

Module Program

    Public Function Main() As Integer
        Call Test()
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub Test()
        Call Data.Load("G:\spectrum\SpectrumChart\Spectrum.json") _
            .Data(Scan0) _
            .Plot(title:="H<sub>2</sub>O<sub>5</sub>C<sub>3</sub>Ag</br><span style=""color:blue; font-size:20"">Test MS/MS spectra plot</span>") _
            .SaveAs("G:\spectrum\spectrum\Plot.png")
    End Sub
End Module
