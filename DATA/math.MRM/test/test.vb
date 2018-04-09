Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.MassSpectrum.Assembly.MarkupData.mzML
Imports SMRUCC.MassSpectrum.Math.Chromatogram
Imports SMRUCC.MassSpectrum.Math.MRM
Imports SMRUCC.MassSpectrum.Math.MRM.Models
Imports SMRUCC.MassSpectrum.Visualization

Module test

    Sub Main()

        Dim std = "D:\ProteoWizard.d\MRM_Test\MetaCardio_STD-X.csv".LoadCsv(Of Standards)
        Dim [IS] As [IS]() = "D:\ProteoWizard.d\MRM_Test\IS.csv".LoadCsv(Of [IS])
        Dim ion_pairs = "D:\ProteoWizard.d\MRM_Test\ion_pairs.csv".LoadCsv(Of IonPair)

        Call ROIvizTest(ion_pairs)


        Dim fits As NamedValue(Of FitResult)() = Nothing
        Dim X As List(Of DataSet) = Nothing

        For Each method As PeakArea.Methods In {Methods.Integrator, Methods.MaxPeakHeight, Methods.NetPeakSum, Methods.SumAll}
            Dim result = MRMSamples.QuantitativeAnalysis("D:\ProteoWizard.d\Data20180313\Data20180313.wiff", ion_pairs, std, [IS], fits, X, calibrationNamedPattern:=".+M1[-]L\d+", peakAreaMethod:=method).ToArray

            Call Console.WriteLine(" ===> " & method.ToString)
            Call Console.WriteLine(fits.Select(Function(f) f.ToString).JoinBy(vbCrLf))
            Call Console.WriteLine()
            Call Console.WriteLine()
        Next

        Pause()
    End Sub

    Sub ROIvizTest(ionpairs As IonPair())
        For Each file As String In {"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L1.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L2.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L3.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L4.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L5.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L6.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L7.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L1.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L2.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L3.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L4.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L5.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L6.mzML",
"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L7.mzML"} '  "D:\ProteoWizard.d\Data20180313".EnumerateFiles("*.mzML")
            Dim ionData = LoadChromatogramList(path:=file) _
                .MRMSelector(ionpairs) _
                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
                .Select(Function(ion)
                            Return New NamedValue(Of ChromatogramTick()) With {
                                .Name = ion.ion.AccID,
                                .Description = ion.ion.ToString,
                                .Value = ion.chromatogram.Ticks
                            }
                        End Function) _
                .ToArray

            Dim dir = file.TrimSuffix

            For Each ion In ionData
                Dim path = $"{dir}/{ion.Name}.png"
                Dim ROI_list = ion.Value.Shadows.PopulateROI.ToArray

                Call ion.Value.Plot(title:=ion.Description, showMRMRegion:=True, showAccumulateLine:=True).AsGDIImage.SaveAs(path)
                Call path.__INFO_ECHO
            Next
        Next

        Pause()
    End Sub
End Module
