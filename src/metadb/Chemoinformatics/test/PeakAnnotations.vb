#Region "Microsoft.VisualBasic::996f36f1e42c405a56a48eef6c3e8f11, src\metadb\Chemoinformatics\test\PeakAnnotations.vb"

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

    ' Module PeakAnnotationsTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Module PeakAnnotationsTest

    Const test As String = "
mz,quantity,intensity,Annotation
69.2259826660156,9,9,1
111.165267944336,100,100,10
112.91716003418,6,6,11
152.87385559082,12,12,23
155.02458190918,12,12,24
156.917953491211,15,15,25
163.152679443359,16,16,27
167.261703491211,12,12,28
169.124862670898,42,42,29
171.184555053711,13,13,30
181.092025756836,64,64,31
199.728469848633,8,8,33
"

    Sub Main()

        Dim precursor As Double = 199.1682
        Dim charge As Double = 1
        Dim products As ms2() = test.Trim() _
            .LineTokens _
            .Skip(1) _
            .Select(Function(a) a.Trim.Split(","c)) _
            .Select(Function(a) New ms2 With {.mz = Val(a(0)), .intensity = Val(a(1))}) _
            .ToArray

        Dim result = New PeakAnnotation().RunAnnotations(precursor, products, charge).toarray

        Pause()
    End Sub
End Module
