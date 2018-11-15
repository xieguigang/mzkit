#Region "Microsoft.VisualBasic::b1f536ad3f88ce80a2bcd43fc8d668ca, GCMS_quantify\test\Module1.vb"

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

' Module Module1
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.GCMS

Module Module1

    Sub Main()
        ' Call unitConvertTest()


        Dim data = QuantifyAnalysis.ReadData("D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA测试标曲.AIA\25ppm.CDF")
        Dim ref = "D:\smartnucl_integrative\biodeepDB\smartnucl_integrative\16s_contents\SCFA\SCFA.csv".LoadCsv(Of ROITable)
        Dim result As New List(Of ROITable)

        For Each target In ref.ScanContents(data)
            result += target.Item1

        Next

        Call result.SaveTo("./metabolites.csv", Encodings.UTF8)
        Call data.GetJson(indent:=True).SaveTo("./gcms.json")

        Pause()
    End Sub

    Sub unitConvertTest()

        Dim ppm2ppb = 999.0#.Unit(ContentUnits.ppm).ScaleTo(ContentUnits.ppb)
        Dim ppt2ppm = 100.0#.Unit(ContentUnits.ppt).ScaleTo(ContentUnits.ppm)

        Pause()
    End Sub

End Module
