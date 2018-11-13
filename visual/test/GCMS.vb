#Region "Microsoft.VisualBasic::c81856ca709a8d02ed53f36a2d29465b, test\GCMS.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module GCMS
    ' 
    '     Sub: batchExport, Main
    ' 
    ' /********************************************************************************/

#End Region

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
        ' Call batchExport()


        Dim gcData = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\250ppm.CDF")
        Dim tic = {gcData.GetTIC}
        Dim ROIlist = gcData.GetTIC.Shadows.PopulateROI.ToArray

        Call tic.TICplot().AsGDIImage.SaveAs("./test_gcms_ticplot.png")
        Call ROIlist.Select(Function(ROI) ROI.GetChromatogramData).ToArray.TICplot.AsGDIImage.SaveAs("./gcms_ions.png")
        Call ROIlist.ToTable.SaveTo("./ROI.csv")
        Call ROIlist.ExportReferenceROITable(
            names:={"乙酸", "丙酸", "异丁酸", "丁酸", "异戊酸", "戊酸", "异己酸", "己酸"}
        ).SaveTo("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA.csv", Encodings.UTF8)

        Dim aaaa = gcData.GetMsScan(ROIlist(Scan0))


        Pause()
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
                names:={"乙酸", "丙酸", "异丁酸", "丁酸", "异戊酸", "戊酸", "异己酸", "己酸"}
            ).SaveTo($"{directory}\ROI.csv", Encodings.UTF8)
        Next

        Pause()
    End Sub
End Module

