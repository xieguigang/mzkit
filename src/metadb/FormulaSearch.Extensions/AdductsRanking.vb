#Region "Microsoft.VisualBasic::b9447c8c6d2f05dd1492ab8db5bb1fc4, metadb\FormulaSearch.Extensions\AdductsRanking.vb"

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

    '   Total Lines: 176
    '    Code Lines: 115 (65.34%)
    ' Comment Lines: 25 (14.20%)
    '    - Xml Docs: 44.00%
    ' 
    '   Blank Lines: 36 (20.45%)
    '     File Size: 5.60 KB


    ' Class AdductsRanking
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetAdductsFormula, InvalidAdduct, Rank, RankAdducts, RankNegative
    '               RankPositive
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula

''' <summary>
''' A helper tools for make adducts ions ranking
''' </summary>
Public Class AdductsRanking

    ReadOnly ion As IonModes

    Const maxValue As Double = 10

    Sub New(ion As IonModes)
        Me.ion = ion
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula_str"></param>
    ''' <param name="adduct_str"></param>
    ''' <returns>
    ''' a score value for the adducts ranking based on current formula composition.
    ''' zero or negative value means the current given adducts is not a valid adducts
    ''' mode, should not use this adducts mode for the annotation result.
    ''' </returns>
    Public Function Rank(formula_str As String, adduct_str As String) As Double
        Dim formula As Formula = FormulaScanner.ScanFormula(formula_str)
        Dim adduct As MzCalculator = Provider.ParseAdductModel(adduct_str)

        If ion = IonModes.Positive Then
            Return RankPositive(formula, adduct)
        Else
            Return RankNegative(formula, adduct)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula_str"></param>
    ''' <param name="adducts"></param>
    ''' <returns>
    ''' the function only populates the valid adducts object and
    ''' sort these adducts object in desc ranking order.
    ''' </returns>
    Public Function RankAdducts(formula_str As String, adducts As IEnumerable(Of MzCalculator)) As IEnumerable(Of MzCalculator)
        Dim formula As Formula = FormulaScanner.ScanFormula(formula_str)
        Dim ranks As IEnumerable(Of (rank As Double, adduct As MzCalculator))

        If ion = IonModes.Positive Then
            ranks = adducts.Select(Function(adduct) (RankPositive(formula, adduct), adduct))
        Else
            ranks = adducts.Select(Function(adduct) (RankNegative(formula, adduct), adduct))
        End If

        Return ranks _
            .Where(Function(a) a.Item1 > 0) _
            .OrderByDescending(Function(a) a.Item1) _
            .Select(Function(a)
                        Return a.adduct
                    End Function)
    End Function

    Private Function GetAdductsFormula(adduct As MzCalculator) As (sign%, formula As Formula)()
        Dim adduct_str As String = adduct.ToString

        Static formulaCache As New Dictionary(Of String, (sign%, formula As Formula)())

        SyncLock formulaCache
            If formulaCache.ContainsKey(adduct_str) Then
                Return formulaCache(adduct_str)
            End If
        End SyncLock

        Dim tokens = Parser.Formula(adduct_str, raw:=True)

        If tokens Like GetType(String) Then
            Throw New InvalidProgramException(tokens.TryCast(Of String))
        End If

        Dim formulaParts = tokens _
            .TryCast(Of IEnumerable(Of (sign%, expression As String))) _
            .Select(Function(part)
                        Dim multiply = ExactMass.Mul(part.expression)
                        Dim f = FormulaScanner.ScanFormula(multiply.Name)

                        Return (part.sign, f * multiply.Value)
                    End Function) _
            .ToArray

        SyncLock formulaCache
            formulaCache(adduct_str) = formulaParts
        End SyncLock

        Return formulaParts
    End Function

    Private Function RankPositive(formula As Formula, adduct As MzCalculator) As Double
        If InvalidAdduct(formula, adduct) Then
            Return 0
        End If

        Dim adduct_str As String = adduct.ToString

        If adduct_str = "[M]+" Then
            Return 0.5
        End If

        If formula.CheckElement("Cl") Then
            If adduct_str = "[M-Cl]+" Then
                If formula!Cl = 1 Then
                    Return maxValue
                End If
            End If
        End If

        Return 1
    End Function

    Private Function InvalidAdduct(formula As Formula, adduct As MzCalculator) As Boolean
        Dim mz As Double = adduct.CalcMZ(formula.ExactMass)

        If mz <= 0 Then
            Return True
        End If

        Dim adduct_parts = GetAdductsFormula(adduct)
        Dim copy As New Formula(formula)

        For Each part In adduct_parts
            copy += part.sign * part.formula
        Next

        If copy.CountsByElement.Any(Function(a) a.Value < 0) Then
            Return True
        End If

        Return False
    End Function

    Private Function RankNegative(formula As Formula, adduct As MzCalculator) As Double
        If InvalidAdduct(formula, adduct) Then
            Return 0
        End If

        Dim adduct_str As String = adduct.ToString

        If adduct_str = "[M]-" Then
            Return 0.5
        End If

        ' deal with some special adducts type situation
        If formula.CheckElement("Na") Then
            If adduct_str = "[M-Na]-" Then
                If formula!Na = 1 Then
                    Return maxValue
                End If
            End If
        End If
        If formula.CheckElement("K") Then
            If adduct_str = "[M-K]-" Then
                If formula!K = 1 Then
                    Return maxValue
                End If
            End If
        End If
        If formula.CheckElement("Li") Then
            If adduct_str = "[M-Li]-" Then
                If formula!Li = 1 Then
                    Return maxValue
                End If
            End If
        End If

        Return 1
    End Function
End Class
