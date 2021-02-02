#Region "Microsoft.VisualBasic::5c7f79742dcd8cfae46af5ccd5ba5c8d, assembly\test\read_mzXML.vb"

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

    ' Module read_mzXML
    ' 
    '     Sub: exportMs1, exportMs2, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports Microsoft.VisualBasic.ApplicationServices.Terminal

Module read_mzXML

    Dim mzXML = "D:\smartnucl_integrative\biodeepDB\protocols\biodeepMSMS1\biodeepMSMS\test\lxy-CID30.mzXML"

    Sub Main()
        ' Call exportMs1()
        Call exportMs2()
    End Sub

    Sub exportMs1()
        Dim table = MarkupData.mzXML.XML.ExportPeaktable(mzXML).ToArray

        ' Call table.SaveTo("./ms1_peaktable.csv")
    End Sub

    Sub exportMs2()
        Dim scanes = MarkupData.mzXML.XML.LoadScans(mzXML).ToArray
        Dim file As New StreamWriter("./lxy-CID30.txt")

        For Each scan In scanes.Where(Function(s) s.msLevel <> "1")
            Dim ms2Peaks = scan.ExtractMzI

            Call file.WriteLine(ms2Peaks.name)
            Call file.WriteLine($"mz range: [{scan.lowMz}, {scan.highMz}]")
            Call file.WriteLine($"peaks: {scan.peaksCount}")
            Call file.WriteLine($"activation: {scan.precursorMz.activationMethod} @ {scan.collisionEnergy}V")
            Call file.WriteLine(ms2Peaks.peaks.Print(addBorder:=False))
            Call file.WriteLine()
        Next

        Call file.Flush()
        Call file.Close()
        Call file.Dispose()

        Pause()
    End Sub
End Module
