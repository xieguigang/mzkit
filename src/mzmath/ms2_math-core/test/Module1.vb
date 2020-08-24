#Region "Microsoft.VisualBasic::2f9d18a1256d746c78a1c96def306f30, src\mzmath\ms2_math-core\test\Module1.vb"

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
    '     Sub: Main, parserTest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Module Module1

    Sub parserTest()
        Dim t1 = Parser.ParseMzCalculator("M+H")
        Dim t2 = Parser.ParseMzCalculator("[M+H]+")
        Dim t3 = Parser.ParseMzCalculator("[M]")
        Dim t4 = Parser.ParseMzCalculator("[M+H]2+")
        Dim t5 = Parser.ParseMzCalculator("[-H]?")
        Dim t6 = Parser.ParseMzCalculator("[99M-235H+33C]1000+")

        Pause()
    End Sub

    Sub Main()

        Call parserTest()

        Dim mass = 853.33089

        Dim mz = Provider.Positive("2M+H").CalcMZ(mass)


        Dim html As New StringBuilder

        Using dev As New StringWriter(html)
            Call MzCalculator.EvaluateAll(mass, "-").PrintTable(dev)
        End Using

        Dim display As String = html.ToString

        Call display.SaveTo("./test.html")

        Pause()
    End Sub
End Module
