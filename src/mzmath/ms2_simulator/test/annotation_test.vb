#Region "Microsoft.VisualBasic::6bb7d81cb315306c88464a3345378033, mzmath\ms2_simulator\test\annotation_test.vb"

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

    '   Total Lines: 33
    '    Code Lines: 27 (81.82%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (18.18%)
    '     File Size: 1.44 KB


    ' Module annotation_test
    ' 
    '     Sub: annotation_test, Main, smiles_test
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Insilicon
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module annotation_test

    Sub Main()
        Call annotation_test()
    End Sub

    Sub annotation_test()
        Dim formuyla As Formula = FormulaScanner.ScanFormula("C7H11N3O2")
        Dim adducts As MzCalculator = Provider.ParseAdductModel("[M+H]+")
        Dim result = PeakAnnotation.DoPeakAnnotation(New LibraryMatrix(
               {170.16, formuyla.ExactMass, 124.2, 170.0923990201 - 17.001, 29.0027395999},
               {100, 45, 20, 5, 30}), adducts, formuyla, da:=0.3)

        Pause()
    End Sub

    Sub smiles_test()
        Dim formuyla As Formula = FormulaScanner.ScanFormula("C7H11N3O2")
        Dim smiles As ChemicalFormula = ChemicalFormula.ParseGraph("CN1C=NC(C[C@H](N)C(O)=O)=C1")
        Dim anno As New SMILESAnnotator(smiles, formuyla)

        Call Console.WriteLine(anno.Annotation(170.16, IonModes.Positive)) ' C7H11N3O2
        Call Console.WriteLine(anno.Annotation(170.52, IonModes.Positive)) ' C7H11N3O2
        Call Console.WriteLine(anno.Annotation(124.2, IonModes.Positive)) ' C6H6NO2
    End Sub
End Module

