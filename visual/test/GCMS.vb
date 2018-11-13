Imports GCMS_quantify
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Visualization

Module GCMS

    Sub Main()
        Call batchExport()


        Dim gcData = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\1ppm-2.CDF")
        Dim tic = {gcData.GetTIC}
        Dim ROIlist = gcData.GetTIC.Shadows.PopulateROI.ToArray

        Call tic.TICplot().AsGDIImage.SaveAs("./test_gcms_ticplot.png")
        Call ROIlist.Select(Function(ROI) ROI.GetChromatogramData).ToArray.TICplot.AsGDIImage.SaveAs("./gcms_ions.png")
        Call ROIlist.ToTable.SaveTo("./ROI.csv")
        Call ROIlist.ExportReferenceROITable(
            names:={"乙酸", "丙酸", "异丁酸", "丁酸", "异戊酸", "戊酸", "己酸"}
        ).SaveTo("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA.csv")
    End Sub

    Sub batchExport()
        For Each file As String In ls - l - r - "*.cdf" <= "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA"
            Dim gcData = QuantifyAnalysis.ReadData(file, "agilentGCMS")
            Dim tic = {gcData.GetTIC}
            Dim ROIlist = gcData.GetTIC.Shadows.PopulateROI.ToArray
            Dim directory$ = file.TrimSuffix

            Call tic.TICplot().AsGDIImage.SaveAs($"{directory}/gcms_TICplot.png")
            Call ROIlist.Select(Function(ROI) ROI.GetChromatogramData).ToArray.TICplot.AsGDIImage.SaveAs($"{directory}/ions.png")
            Call ROIlist.ExportReferenceROITable(
                names:={"乙酸", "丙酸", "异丁酸", "丁酸", "异戊酸", "戊酸", "己酸"}
            ).SaveTo($"{directory}\ROI.csv", Encodings.UTF8)
        Next

        Pause()
    End Sub
End Module
