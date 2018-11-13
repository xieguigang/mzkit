Imports GCMS_quantify
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Visualization

Module GCMS

    Sub Main()
        Dim gcData = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\scfa200ppmAIAEXPRT.AIA\200ppm-未处理.CDF")
        Dim tic = {gcData.GetTIC}
        Dim ROIlist = gcData.GetTIC.Shadows.PopulateROI.ToArray

        Call tic.TICplot().AsGDIImage.SaveAs("./test_gcms_ticplot.png")
        Call ROIlist.Select(Function(ROI) ROI.GetChromatogramData).ToArray.TICplot.AsGDIImage.SaveAs("./gcms_ions.png")
        Call ROIlist.ToTable.SaveTo("./ROI.csv")
    End Sub
End Module
