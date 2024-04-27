#Region "Microsoft.VisualBasic::545f6b5e181c2bb258cc739a762be4ac, G:/mzkit/src/metadb/Massbank/test//ChemOntTest.vb"

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

    '   Total Lines: 39
    '    Code Lines: 22
    ' Comment Lines: 8
    '   Blank Lines: 9
    '     File Size: 1.43 KB


    ' Module ChemOntTest
    ' 
    '     Sub: Main, meshTest
    ' 
    ' /********************************************************************************/

#End Region

'Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Imports BioNovoGene.BioDeep.Chemistry.NCBI.MeSH
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module ChemOntTest

    '    Sub Main()
    '        Dim ChemOnt As New ChemOntClassify("D:\MassSpectrum-toolkits\DATA\DATA\ChemOnt_2_1.obo.TXT")
    '        Dim anno = "D:\Database\CID-Synonym-filtered\class\HMDB_36_classyfire_21_annotations.csv".LoadCsv(Of ClassyfireAnnotation)
    '        Dim table = ClassyfireInfoTable.PopulateMolecules(anno, ChemOnt).GroupBy(Function(c) c.CompoundID).Select(Function(g) g.First).ToArray


    '        Call table.SaveTo("D:\Database\CID-Synonym-filtered\class\HMDB_36_classyfire_21_annotations_infoTable.csv")

    '        Pause()
    '    End Sub

    Sub meshTest()
        Dim tree = Reader.ParseTree("E:\mzkit\Rscript\Library\mzkit_app\data\mtrees2022.bin".OpenReader)
        Dim gsea = tree.ImportsTree(
            Function(t)
                Return New BackgroundGene With {
                    .accessionID = t.term,
                    .[alias] = {t.term},
                    .locus_tag = New NamedValue With {.name = t.term, .text = t.term},
                    .name = t.term,
                    .term_id = {t.term}
                }
            End Function)

        Pause()
    End Sub

    Sub Main()
        Call meshTest()
    End Sub
End Module
