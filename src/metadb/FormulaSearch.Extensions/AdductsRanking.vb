#Region "Microsoft.VisualBasic::caacd331b1f5ab24387a677458e569ee, metadb\FormulaSearch.Extensions\AdductsRanking.vb"

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

    '   Total Lines: 244
    '    Code Lines: 161 (65.98%)
    ' Comment Lines: 44 (18.03%)
    '    - Xml Docs: 81.82%
    ' 
    '   Blank Lines: 39 (15.98%)
    '     File Size: 8.67 KB


    ' Class AdductsRanking
    ' 
    '     Function: Filter, GetAdductsFormula, InvalidAdduct, Rank, (+3 Overloads) RankAdducts
    '               RankNegative, RankPositive
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' A helper tools for make adducts ions ranking
''' </summary>
Public Class AdductsRanking

    ReadOnly maxValue As Double = 10

    Sub New(Optional maxScore As Double = 10)
        maxValue = maxScore
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula_str"></param>
    ''' <param name="adduct"></param>
    ''' <returns>
    ''' a score value for the adducts ranking based on current formula composition.
    ''' zero or negative value means the current given adducts is not a valid adducts
    ''' mode, should not use this adducts mode for the annotation result.
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Rank(formula_str As String, adduct As String) As Double
        Return Rank(FormulaScanner.ScanFormula(formula_str), adduct)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <param name="adduct_str"></param>
    ''' <returns>
    ''' a score value for the adducts ranking based on current formula composition.
    ''' zero or negative value means the current given adducts is not a valid adducts
    ''' mode, should not use this adducts mode for the annotation result.
    ''' </returns>
    Public Function Rank(formula As Formula, adduct_str As String) As Double
        Dim adduct As MzCalculator = Provider.ParseAdductModel(adduct_str)
        Dim ion As IonModes = adduct.GetIonMode

        If ion = IonModes.Positive Then
            Return RankPositive(formula, adduct)
        Else
            Return RankNegative(formula, adduct)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <param name="adducts"></param>
    ''' <returns>
    ''' the function only populates the valid adducts object and
    ''' sort these adducts object in desc ranking order. top is better.
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function RankAdducts(formula As Formula, adducts As IEnumerable(Of String)) As IEnumerable(Of MzCalculator)
        Return RankAdducts(formula, adducts:=adducts.SafeQuery.Select(Function(type) Provider.ParseAdductModel(type)))
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <param name="adducts"></param>
    ''' <returns>
    ''' the function only populates the valid adducts object and
    ''' sort these adducts object in desc ranking order. top is better.
    ''' </returns>
    Public Function RankAdducts(formula As Formula, adducts As IEnumerable(Of MzCalculator)) As IEnumerable(Of MzCalculator)
        Dim ranks As IEnumerable(Of (rank As Double, adduct As MzCalculator)) = adducts _
            .Select(Function(adduct)
                        Dim ion As IonModes = adduct.GetIonMode

                        If ion = IonModes.Positive Then
                            Return (rank:=RankPositive(formula, adduct), adduct)
                        Else
                            Return (rank:=RankNegative(formula, adduct), adduct)
                        End If
                    End Function) _
            .ToArray

        Return ranks _
            .Where(Function(a) a.rank > 0) _
            .OrderByDescending(Function(a) a.rank) _
            .Select(Function(a)
                        Return a.adduct
                    End Function)
    End Function

    Public Iterator Function Filter(Of T)(formula As String, data As IEnumerable(Of T), adduct As Func(Of T, String)) As IEnumerable(Of T)
        Dim formulaObj As Formula = FormulaScanner.ScanFormula(formula)
        Dim cache As New Dictionary(Of String, Double)

        For Each metabolite As T In data.SafeQuery
            Dim score As Double = cache.ComputeIfAbsent(adduct(metabolite),
                lazyValue:=Function(type)
                               Dim adduct_type As MzCalculator = Provider.ParseAdductModel(type)
                               Dim ion As IonModes = adduct_type.GetIonMode

                               If ion = IonModes.Positive Then
                                   Return RankPositive(formulaObj, adduct_type)
                               Else
                                   Return RankNegative(formulaObj, adduct_type)
                               End If
                           End Function)
            If score > 0 Then
                Yield metabolite
            End If
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="formula_str"></param>
    ''' <param name="adducts"></param>
    ''' <returns>
    ''' the function only populates the valid adducts object and
    ''' sort these adducts object in desc ranking order. top is better.
    ''' </returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function RankAdducts(formula_str As String, adducts As IEnumerable(Of MzCalculator)) As IEnumerable(Of MzCalculator)
        Return RankAdducts(formula:=FormulaScanner.ScanFormula(formula_str), adducts)
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

        If formula.CheckElement("Cl") Then
            If adduct_str = "[M-Cl]+" Then
                If formula!Cl = 1 Then
                    Return maxValue
                End If
            End If
        End If

        If adduct_str = "[M]+" Then
            ' check for Anthocyanin
            If AnthocyaninValidator.CheckRules(formula.CountsByElement) > 40 Then
                Return maxValue
            Else
                Return 0.5
            End If
        End If

        If adduct_str = "[M+H]+" Then
            Return maxValue / 2
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

        If adduct_str = "[M-H]-" Then
            Return maxValue / 2
        End If

        Return 1
    End Function
End Class
