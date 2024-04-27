#Region "Microsoft.VisualBasic::57c3aad748b63a21af887632d9cd2b81, G:/mzkit/src/metadb/Chemoinformatics/test//IsotopicTest.vb"

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

    '   Total Lines: 28
    '    Code Lines: 11
    ' Comment Lines: 12
    '   Blank Lines: 5
    '     File Size: 772 B


    ' Module IsotopicTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

Public Module IsotopicTest

    ' Formula: C1H1Cl3
    ' Formula weight :  119.369 g mol–1
    ' Isotope pattern
    '
    ' 118  100.00  __________________________________________________
    ' 119    1.09  _
    ' 120   95.78  ________________________________________________
    ' 121    1.04  _
    ' 122   30.58  _______________
    ' 123    0.33  
    ' 124    3.25  __
    ' 125    0.04  

    Sub Main()
        Dim formula = FormulaScanner.ScanFormula("C10H16N5O13P3")
        Dim dist = IsotopicPatterns.IsotopeDistribution.Distribution(formula)

        For Each item In dist
            Call Console.WriteLine(item.ToString)
        Next

        Pause()
    End Sub
End Module
