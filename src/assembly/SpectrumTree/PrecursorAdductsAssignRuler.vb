#Region "Microsoft.VisualBasic::2b9a0698c090a67096e93bdc837de426, mzkit\src\mzmath\ms2_math-core\Ms1\PrecursorType\PrecursorAdductsAssignRuler.vb"

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

'   Total Lines: 48
'    Code Lines: 30
' Comment Lines: 11
'   Blank Lines: 7
'     File Size: 1.52 KB


'     Class PrecursorAdductsAssignRuler
' 
'         Function: IonNegativeTypes, IonPositiveTypes, PossibleTypes
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class PrecursorAdductsAssignRuler

    ReadOnly ionMode As IonModes
    ReadOnly adductFormula As New Dictionary(Of String, Formula)
    ReadOnly no_test As Index(Of String) = {"[M]+", "[M]-", "[M+H]+", "[M-H]-"}

    Sub New(ionMode As IonModes)
        Me.ionMode = ionMode
    End Sub

    Public Function AssertAdducts(formula As String, ParamArray adducts As String()) As MzCalculator()
        Return AssertAdducts(formula, adducts.Select(Function(t) ParseMzCalculator(t, ionMode))).ToArray
    End Function

    Public Iterator Function AssertAdducts(formula As String, adducts As IEnumerable(Of MzCalculator)) As IEnumerable(Of MzCalculator)
        Dim composition As Formula = FormulaScanner.ScanFormula(formula)

        For Each adduct As MzCalculator In adducts
            Dim name As String = adduct.ToString

            If name Like no_test Then
                Yield adduct
                Continue For
            End If

            Dim adductParts = GetAdductFormula(precursor_type:=adduct.ToString)
            Dim invalid As Boolean = False

            For Each element In adductParts.CountsByElement
                If element.Value < 0 Then
                    If composition(element.Key) <= 0 Then
                        invalid = True
                        Exit For
                    End If
                End If
            Next

            If Not invalid Then
                Yield adduct
            End If
        Next
    End Function

    Private Function GetAdductFormula(precursor_type As String) As Formula
        If Not adductFormula.ContainsKey(precursor_type) Then
            Dim tokens = Parser.Formula(Strings.Trim(precursor_type), raw:=True) _
                .TryCast(Of IEnumerable(Of (sign%, expression As String))) _
                .Select(Function(t)
                            Return (t.sign, FormulaScanner.ScanFormula(t.expression))
                        End Function) _
                .ToArray
            Dim composition As Formula = Formula.Empty

            For Each part In tokens

            Next

            adductFormula(precursor_type) = composition
        End If

        Return adductFormula(precursor_type)
    End Function

    Public Overrides Function ToString() As String
        Return ionMode.Description
    End Function
End Class
