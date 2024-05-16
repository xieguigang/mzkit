#Region "Microsoft.VisualBasic::5637fb712f250407ad03b41804ca5218, metadb\FormulaSearch.Extensions\test\peakTest.vb"

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

    '   Total Lines: 79
    '    Code Lines: 64
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 2.41 KB


    ' Module peakTest
    ' 
    '     Function: loadMs2
    ' 
    '     Sub: Main, peakAnnoFormula, peakAnnoNoFormula
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Serialization.JSON

Module peakTest

    Const formula As String = "C3H7NO2"
    Const mat = "
m/z	intensity	relative	annotation
56.9656677246094	4082.47802734375	7	
61.0290718078613	3402.3037109375	6	
65.0393295288086	7694.25634765625	14	
70.0658416748047	2943.96020507813	5	
72.0814743041992	55124.87890625	100	
73.0474624633789	11651.2001953125	21	
79.0215911865234	3701.81689453125	7	
81.000846862793	3707.09350585938	7	
89.0794830322266	6305.98046875	11	
90.0918731689453	37419.2578125	68	
91.05810546875	52178.4296875	95	
"

    Function loadMs2() As ms2()
        Dim lines = mat.Trim(vbCr, vbLf, " ", vbTab).LineTokens.Skip(1).ToArray
        Dim ms2 As ms2() = lines _
            .Select(Function(line)
                        Dim row = line.StringSplit("\s+").Select(AddressOf Val).ToArray
                        Dim ms As New ms2 With {
                            .mz = row(0),
                            .intensity = row(1)
                        }

                        Return ms
                    End Function) _
            .ToArray

        Return ms2
    End Function

    Sub Main()
        Call peakAnnoFormula()
        Call peakAnnoNoFormula()

        Pause()
    End Sub

    Sub peakAnnoFormula()
        Dim anno As New PeakAnnotation(0.1, True)
        Dim result = anno.RunAnnotation(90.0555, loadMs2, formula)

        Call Console.WriteLine("With formula test")
        Call Console.WriteLine()

        For Each row In result.products
            Call Console.WriteLine(row.ToString)
        Next

        Call Console.WriteLine(New String("-"c, 32))
        Call Console.WriteLine()
        Call Console.WriteLine()
    End Sub

    Sub peakAnnoNoFormula()
        Dim anno As New PeakAnnotation(0.1, True)
        Dim result = anno.RunAnnotation(90.0555, loadMs2)

        Call Console.WriteLine("No formula test")
        Call Console.WriteLine()

        For Each row In result.products
            Call Console.WriteLine(row.ToString)
        Next

        Call Console.WriteLine(New String("-"c, 32))
        Call Console.WriteLine()
        Call Console.WriteLine()
    End Sub
End Module
