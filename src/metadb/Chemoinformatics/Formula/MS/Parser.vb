﻿#Region "Microsoft.VisualBasic::d512084251c4ca0a2c11285b43d95f30, metadb\Chemoinformatics\Formula\MS\Parser.vb"

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

    '   Total Lines: 103
    '    Code Lines: 76 (73.79%)
    ' Comment Lines: 3 (2.91%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 24 (23.30%)
    '     File Size: 4.81 KB


    '     Module Parser
    ' 
    '         Function: CalculateAccurateMassAndIsotopeRatio, CalculateAccurateMassAndIsotopeRatioOfMolecularFormula, ConvertDifferentChargedAdduct
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType.AdductIonParser
Imports std = System.Math

Namespace Formula.MS

    Public Module Parser

        Public Function ConvertDifferentChargedAdduct(adduct As AdductIon, chargeNumber As Integer) As AdductIon
            If chargeNumber = 0 Then Return adduct
            If adduct.FormatCheck = False Then Return adduct
            If std.Abs(adduct.ChargeNumber) = chargeNumber Then Return adduct

            Dim adductContent = GetAdductContent(adduct.AdductIonName)

            Dim xMerString = If(adduct.AdductIonXmer = 1, String.Empty, adduct.AdductIonXmer.ToString())
            Dim chargeString = If(chargeNumber = 1, String.Empty, chargeNumber.ToString())
            Dim radicalString = If(adduct.IsRadical = False, String.Empty, ".")
            Dim ionString = If(adduct.IonMode = IonModes.Positive, "+", "-")

            Dim newAdductString = "[" & xMerString & "M" & adductContent & "]" & chargeString & ionString & radicalString

            Dim newAdduct = AdductIon.GetAdductIon(newAdductString)
            Return newAdduct
        End Function

        Public Function CalculateAccurateMassAndIsotopeRatioOfMolecularFormula(rawFormula As String) As (Double, Double, Double)
            Dim formula As String = Nothing, multipliedNum As Double = Nothing

            With GetFormulaAndNumber(rawFormula)
                formula = .formula
                multipliedNum = .multipliedNum
            End With

            If String.IsNullOrWhiteSpace(formula) Then
                Return (multipliedNum, 0, 0)
            End If

            ' Common adduct check
            Dim commonAcurateMass As Double = Nothing, m1Intensity As Double = Nothing, m2Intensity As Double = Nothing

            If IsCommonAdduct(formula, multipliedNum, commonAcurateMass, m1Intensity, m2Intensity) Then
                Return (commonAcurateMass, m1Intensity, m2Intensity)
            End If

            ' check irons
            Dim iron As Double = Nothing

            If IronToMass.TryGetValue(formula, iron) Then
                Return (multipliedNum * iron, 0, 0)
            End If

            ' Organic compound adduct check
            Dim formulaBean As Formula = Nothing, organicAcurateMass As Double = Nothing
            Dim nil2 As (formulaBean As Dictionary(Of String, Integer), organicAcurateMass As Double) = GetOrganicAdductFormulaAndMass(formula, multipliedNum)

            Return (organicAcurateMass, SevenGoldenRulesCheck.GetM1IsotopicAbundance(formulaBean), SevenGoldenRulesCheck.GetM2IsotopicAbundance(formulaBean))
        End Function

        Public Function CalculateAccurateMassAndIsotopeRatio(adductName As String) As (Double, Double, Double)
            adductName = adductName.Split("["c)(1).Split("]"c)(0).Trim()

            If Not adductName.Contains("+"c) AndAlso Not adductName.Contains("-"c) Then
                Return (0R, 0R, 0R)
            End If

            Dim equationNum = adductName.Count("+"c) + adductName.Count("-"c)
            Dim formula = String.Empty
            Dim accAccurateMass As Double = 0, accM1Intensity As Double = 0, accM2Intensity As Double = 0
            Dim accurateMass As Double = 0
            Dim m1Intensity As Double = 0
            Dim m2Intensity As Double = 0

            For i = adductName.Length - 1 To 0 Step -1
                If adductName(i).Equals("+"c) Then
                    Dim nul As (accurateMass As Double, m1Intensity As Double, m2Intensity As Double) = CalculateAccurateMassAndIsotopeRatioOfMolecularFormula(formula)
                    accAccurateMass += nul.accurateMass
                    accM1Intensity += nul.m1Intensity
                    accM2Intensity += nul.m2Intensity

                    formula = String.Empty
                    equationNum -= 1
                ElseIf adductName(i).Equals("-"c) Then
                    Dim nul As (accurateMass As Double, m1Intensity As Double, m2Intensity As Double) = CalculateAccurateMassAndIsotopeRatioOfMolecularFormula(formula)
                    accAccurateMass -= nul.accurateMass
                    accM1Intensity -= nul.m1Intensity
                    accM2Intensity -= nul.m2Intensity

                    formula = String.Empty
                    equationNum -= 1
                Else
                    formula = adductName(i).ToString() & formula
                End If

                If equationNum <= 0 Then
                    Exit For
                End If
            Next

            Return (accAccurateMass, accM1Intensity, accM2Intensity)
        End Function
    End Module
End Namespace
