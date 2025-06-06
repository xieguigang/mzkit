#Region "Microsoft.VisualBasic::ec030df7b3fa52ca149a47398d8a0093, metadb\Chemoinformatics\Extensions.vb"

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

    '   Total Lines: 18
    '    Code Lines: 14 (77.78%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (22.22%)
    '     File Size: 628 B


    ' Module Extensions
    ' 
    '     Function: HtmlView
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports FormulaVal = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula

<HideModuleName>
Public Module Extensions

    Public Function HtmlView(formulaStr As String) As String
        Dim f As FormulaVal = FormulaScanner.ScanFormula(formulaStr)
        Dim html As New StringBuilder

        For Each element As KeyValuePair(Of String, Integer) In f.CountsByElement
            html.Append($"<span class=""element"">{element.Key}</span><sub>{element.Value}</sub>")
        Next

        Return html.ToString
    End Function
End Module
