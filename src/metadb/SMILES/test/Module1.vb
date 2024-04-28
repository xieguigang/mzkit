#Region "Microsoft.VisualBasic::6f9b57d2ff0dccdce174b6c761c4bd22, E:/mzkit/src/metadb/SMILES//test/Module1.vb"

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

    '   Total Lines: 47
    '    Code Lines: 31
    ' Comment Lines: 4
    '   Blank Lines: 12
    '     File Size: 1.70 KB


    ' Module Module1
    ' 
    '     Sub: branchTest, Main, runTest, simpleTest
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES

Module Module1

    Sub simpleTest()
        Call runTest("CC", "ethane CH3CH3")
        Call runTest("C=O", "formaldehyde (CH2O)")
        Call runTest("C=C", "ethene (CH2=CH2)")
        Call runTest("O=C=O", "carbon dioxide (CO2)")
        Call runTest("COC", "dimethyl ether (CH3OCH3)")
        Call runTest("C#N", "hydrogen cyanide (HCN)")
        Call runTest("CCO", "ethanol (CH3CH2OH)")
        Call runTest("[H][H]", "molecular hydrogen (H2)")
        Call runTest("C=C-C-C=C-C-O", "6-hydroxy-1,4-hexadiene CH2=CH-CH2-CH=CH-CH2-OH")
    End Sub

    Sub Main()

        '  Call simpleTest()
        Call branchTest()

        Pause()
    End Sub

    Sub branchTest()
        ' Call runTest("CCN(CC)CC", "Triethylamine  C6H15N")
        '  Call runTest("CC(C)C(=O)O", "Isobutyric acid  C4H8O2")
        ' Call runTest("CCCCC(C)(CCC)C=C", "3-Methyl-3-propyl-1-heptene C11H22")

        Call runTest("COC1=CC(=CC(=C1O)OC)C2=C(C3=C4C(=CC(=[OH+])C=C4O2)OC(=C3)C5=CC(=C(C=C5)O)O)OC6C(C(C(C(O6)CO)O)O)O", "Malvidin 3-glucoside-4-vinylcatechol C31H29O14+")
    End Sub

    Sub runTest(SMILES As String, prompt As String)
        Call Console.WriteLine(prompt)

        Dim formula = ParseChain.ParseGraph(SMILES).GetFormula

        Call Console.WriteLine(formula.ToString)
        Call Console.WriteLine(Formula.BuildFormula(formula.CountsByElement))
        Call Console.WriteLine($"exact mass: {formula.ExactMass.ToString("F4")}")

        Call Console.WriteLine(New String("-"c, 32))
        Call Console.WriteLine()
    End Sub

End Module
