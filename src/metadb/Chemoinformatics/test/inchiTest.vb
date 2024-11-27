#Region "Microsoft.VisualBasic::0e9df32a7e1ec22039250b268bedd3c1, metadb\Chemoinformatics\test\inchiTest.vb"

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

    '   Total Lines: 34
    '    Code Lines: 14 (41.18%)
    ' Comment Lines: 8 (23.53%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (35.29%)
    '     File Size: 851 B


    ' Module inchiTest
    ' 
    '     Sub: Main2
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.IUPAC
Imports BioNovoGene.BioDeep.Chemoinformatics.IUPAC.InChI
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models

Module inchiTest

    Sub Main2()

        '    8 9      10
        '    |  \    /
        ' 7  2   3--4
        '  \/ \ /   |
        '  1   5    6
        '     / \  / \
        '    H   12   11
        '

        Dim bounds As InchiInput = MainLayer.ParseBounds("7-1-2(8)5-3(9)4(10)6(11)12-5")
        Dim h = MainLayer.ParseHAtoms("2,5,7-10H,1H2").ToArray


        Dim ascorbicAcid As String = "InChI=1S/C6H8O6/c7-1-2(8)5-3(9)4(10)6(11)12-5/h2,5,7-10H,1H2/t2-,5+/m0/s1"

        Dim inchi As New InChI(ascorbicAcid)
        Dim strct As [Structure] = inchi.GetStruct

        Dim key As String = inchi.Key




        Pause()
    End Sub
End Module
