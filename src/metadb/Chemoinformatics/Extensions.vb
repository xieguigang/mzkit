#Region "Microsoft.VisualBasic::aca06a371533b558cf87ca568a50f68c, metadb\Chemoinformatics\Extensions.vb"

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

'   Total Lines: 17
'    Code Lines: 14 (82.35%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 3 (17.65%)
'     File Size: 456 B


' Module Extensions
' 
'     Function: HtmlView
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports FormulaVal = BioNovoGene.BioDeep.Chemoinformatics.Formula.Formula
Imports std = System.Math

<HideModuleName>
Public Module Extensions

    Public Function HtmlView(formula As String) As String
        Dim numbers As String() = formula _
            .Matches("\d+") _
            .Distinct _
            .OrderByDescending(Function(d) d.Length) _
            .ToArray

        For Each n As String In numbers
            formula = formula.Replace(n, $"<sub>{n}</sub>")
        Next

        Return formula
    End Function

    <Extension>
    Public Function IonFormulaCalibration(formula As FormulaVal, adduct As MzCalculator) As FormulaVal
        Dim adducts = Parser.GetAdductParts(adduct.ToString)
        Dim f As FormulaVal

        For Each part As NamedValue(Of Integer) In adducts
            f = FormulaScanner.ScanFormula(part.Name)
            f = f * std.Abs(part.Value)

            ' reverse of the formula adducts
            If part.Value > 0 Then
                formula -= f
            Else
                formula += f
            End If
        Next

        Return formula
    End Function

End Module
