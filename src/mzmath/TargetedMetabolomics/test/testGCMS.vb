﻿#Region "Microsoft.VisualBasic::7276b31e20263bfe271235c0f1463339, mzmath\TargetedMetabolomics\test\testGCMS.vb"

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

    '   Total Lines: 54
    '    Code Lines: 38 (70.37%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 16 (29.63%)
    '     File Size: 1.79 KB


    ' Module testGCMS
    ' 
    '     Sub: GCMS2, Main, unitConvertTest
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.QuantifyAnalysis
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Module testGCMS

    Sub GCMS2()
        Dim raw = mzMLReader.LoadFile("F:\sssssssssssssssssssssss\1ppm.mzML")

        Pause()
    End Sub

    Sub Main()
        Call GCMS2()
        Call unitConvertTest()


        Dim data = GCMS.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\25ppm.CDF")
        Dim ref = "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA.csv".LoadCsv(Of ROITable)
        Dim result As New List(Of ROITable)

        For Each target In ref.ScanIons(data, New DoubleRange({5, 10}))
            result += target.Item1

        Next

        Call result.SaveTo("./metabolites.csv", Encodings.UTF8)
        Call data.GetJson(indent:=True).SaveTo("./gcms.json")

        Pause()
    End Sub

    Sub unitConvertTest()

        Dim ppm2ppb = 999.0#.Unit(ContentUnits.ppm).ScaleTo(ContentUnits.ppb)
        Dim ppt2ppm = 100.0#.Unit(ContentUnits.ppt).ScaleTo(ContentUnits.ppm)


        Dim c200ppm = "200 ppm".ParseContent
        Dim c1ppt = "1ppt".ParseContent
        Dim c99999ppb = "99999ppb".ParseContent

        Pause()
    End Sub

End Module
