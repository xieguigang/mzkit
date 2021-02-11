#Region "Microsoft.VisualBasic::68d2aa7bb3e3315255f64e76a00631d8, Massbank\test\oboTest.vb"

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

    ' Module oboTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.foundation.OBO_Foundry
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.MassSpectrum.DATA.MetaLib
Imports Microsoft.VisualBasic.Data.csv.IO.Linq

Module oboTest

    Sub Main()
        Dim tree = New OBOFile("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT").GetRawTerms _
            .BuildTree _
            .Values _
            .Select(Function(node) node.GetTermsByLevel(3)) _
            .Where(Function(a) Not a.IsNullOrEmpty) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(o) o.name) _
            .ToArray


        Dim classify As New ChemOntClassify("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT")

        Dim annos = classify.FilterByLevel("C:\Users\gg.xie\Downloads\ChEBI_126_classyfire_21_annotations.csv\ChEBI_126_classyfire_21_annotations.csv".OpenHandle.AsLinq(Of ClassyfireAnnotation), 2).ToArray


        Pause()
    End Sub
End Module
