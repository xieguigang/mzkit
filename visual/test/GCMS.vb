Imports GCMS_quantify
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.MassSpectrum.Visualization

Module GCMS

    Sub Main()
        Dim gcData = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\scfa200ppmAIAEXPRT.AIA\200ppm-未处理.CDF")
        Dim tic = {gcData.GetTIC}

        Call tic.TICplot().AsGDIImage.SaveAs("./test_gcms_ticplot.png")

    End Sub
End Module
