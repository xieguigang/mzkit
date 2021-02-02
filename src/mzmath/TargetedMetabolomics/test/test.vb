#Region "Microsoft.VisualBasic::5d5ceb2f644f034ae56ebee38c69c714, TargetedMetabolomics\test\test.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::0e5d7c90756fb15e48960170fc7a3ff3, src\mzmath\TargetedMetabolomics\test\test.vb"

'' Author:
'' 
''       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
'' 
'' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
'' 
'' 
'' MIT License
'' 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy
'' of this software and associated documentation files (the "Software"), to deal
'' in the Software without restriction, including without limitation the rights
'' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'' copies of the Software, and to permit persons to whom the Software is
'' furnished to do so, subject to the following conditions:
'' 
'' The above copyright notice and this permission notice shall be included in all
'' copies or substantial portions of the Software.
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'' SOFTWARE.



'' /********************************************************************************/

'' Summaries:

'' Module test
'' 
''     Sub: Main, ROIvizTest
'' 
'' /********************************************************************************/

'#End Region

'Imports BioNovoGene.Analytical.MassSpectrometry.Math
'Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
'Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
'Imports Microsoft.VisualBasic.Data.csv

'Module test

'    Sub Main()

'        Dim std = "D:\ProteoWizard.d\MRM_Test\MetaCardio_STD-X.csv".LoadCsv(Of Standards)
'        Dim [IS] As [IS]() = "D:\ProteoWizard.d\MRM_Test\IS.csv".LoadCsv(Of [IS])
'        Dim ion_pairs = "D:\ProteoWizard.d\MRM_Test\ion_pairs.csv".LoadCsv(Of IonPair)

'        '   Call ROIvizTest(ion_pairs)


'        Dim fits As FitModel() = Nothing
'        Dim X As NamedValue(Of MRMStandards())() = Nothing

'        For Each method As PeakArea.Methods In {Methods.Integrator, Methods.MaxPeakHeight, Methods.NetPeakSum, Methods.SumAll}
'            Dim result = MRMSamples.QuantitativeAnalysis("D:\ProteoWizard.d\Data20180313\Data20180313.wiff", ion_pairs, std, [IS], fits, X, calibrationNamedPattern:=".+M1[-]L\d+", peakAreaMethod:=method).ToArray

'            Call Console.WriteLine(" ===> " & method.ToString)
'            Call Console.WriteLine(fits.Select(Function(f) f.ToString).JoinBy(vbCrLf))
'            Call Console.WriteLine()
'            Call Console.WriteLine()
'        Next

'        Pause()
'    End Sub

'    Sub ROIvizTest(ionpairs As IonPair())
'        For Each file As String In {"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L1.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L2.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L3.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L4.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L5.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L6.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M1-L7.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L1.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L2.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L3.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L4.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L5.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L6.mzML",
'"D:\ProteoWizard.d\Data20180313\Data20180313-M2-L7.mzML"} '  "D:\ProteoWizard.d\Data20180313".EnumerateFiles("*.mzML")
'            Dim ionData = LoadChromatogramList(path:=file) _
'                .MRMSelector(ionpairs) _
'                .Where(Function(ion) Not ion.chromatogram Is Nothing) _
'                .Select(Function(ion)
'                            Return New NamedValue(Of ChromatogramTick()) With {
'                                .Name = ion.ion.accession,
'                                .Description = ion.ion.ToString,
'                                .Value = ion.chromatogram.Ticks
'                            }
'                        End Function) _
'                .ToArray

'            Dim dir = file.TrimSuffix

'            For Each ion In ionData
'                Dim path = $"{dir}/{ion.Name}.png"
'                Dim ROI_list = ion.Value.Shadows.PopulateROI.ToArray

'                '  Call ion.Value.Plot(title:=ion.Description, showMRMRegion:=True, showAccumulateLine:=True).AsGDIImage.SaveAs(path)
'                '  Call path.__INFO_ECHO
'            Next
'        Next

'        Pause()
'    End Sub
'End Module
