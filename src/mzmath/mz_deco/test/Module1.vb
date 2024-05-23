#Region "Microsoft.VisualBasic::2ce4250103d4c083d3df799650b72e66, mzmath\mz_deco\test\Module1.vb"

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


    ' Code Statistics:

    '   Total Lines: 81
    '    Code Lines: 61 (75.31%)
    ' Comment Lines: 1 (1.23%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 19 (23.46%)
    '     File Size: 2.86 KB


    ' Module Module1
    ' 
    '     Sub: align_test, Main, multipleTest, singleTest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq

Module Module1

    Sub Main()
        ' Call multipleTest()
        Call align_test()
    End Sub

    Sub align_test()
        Dim debugfiles = {
        "G:\tmp\pos_mzPack\QC3.dat",
"G:\tmp\pos_mzPack\QC1.dat",
"G:\tmp\pos_mzPack\QC2.dat"
        }
        Dim samples As New List(Of NamedCollection(Of PeakFeature))

        For Each file As String In debugfiles
            samples.Add(New NamedCollection(Of PeakFeature)(file.BaseName, SaveSample.ReadSample(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))))
        Next

        Dim xcms_peaks = PeakAlignment.CreateMatrix(samples).ToArray

        Call xcms_peaks.SaveTo("G:\tmp\pos.csv")
    End Sub

    Sub multipleTest()
        Dim filepath As String() = {"G:\tmp\pos_mzPack\QC3.mzPack",
"G:\tmp\pos_mzPack\QC1.mzPack",
"G:\tmp\pos_mzPack\QC2.mzPack"}

        Dim samples As New List(Of NamedCollection(Of PeakFeature))

        For Each file As String In filepath
            Dim bin As New BinaryStreamReader(file)
            Dim raw = bin.LoadAllScans(skipProducts:=True).ToArray
            Dim scans = raw.Select(Function(d) d.GetMs1Scans).IteratesALL.ToArray
            Dim peaktable As PeakFeature() = scans _
                .GetMzGroups(mzdiff:=Tolerance.DeltaMass(0.05)) _
                .DecoMzGroups(New Double() {3, 30}, quantile:=0.1, sn:=1) _
                .ToArray
            Dim debug As String = $"{file.ParentPath}/{file.BaseName}.dat"

            Call samples.Add(New NamedCollection(Of PeakFeature)(file.BaseName, peaktable))
            Call SaveSample.DumpSample(peaktable, debug.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        Next



        Pause()
    End Sub

    Sub singleTest()
        Using file As New BinaryStreamReader("E:\test.mzPack")
            Dim dataMS1 As ScanMS1() = file.EnumerateIndex _
                .Select(Function(id) file.ReadScan(id, skipProducts:=True)) _
                .ToArray
            Dim scans As ms1_scan() = dataMS1 _
                .Select(Function(d) d.GetMs1Scans) _
                .IteratesALL _
                .ToArray

            Dim peaktable As PeakFeature() = scans _
                .GetMzGroups(mzdiff:=Tolerance.DeltaMass(0.05)) _
                .DecoMzGroups(New Double() {3, 30}, quantile:=0.1, sn:=1) _
                .ToArray


            Call peaktable.SaveTo("E:/test_peaktable.csv")

            Pause()
        End Using
    End Sub

End Module
