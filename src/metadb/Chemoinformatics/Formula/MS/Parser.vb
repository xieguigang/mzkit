Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType.AdductIonParser
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports std = System.Math

Module Parser

    ''' <summary>
    ''' This method returns the AdductIon class variable from the adduct string.
    ''' </summary>
    ''' <param name="adductName">Add the formula string such as "C6H12O6"</param>
    ''' <returns></returns>
    <Obsolete("Use AdductIon.GetAddutIon instead of this method.")>
    Public Function GetAdductIonBean(ByVal adductName As String) As AdductIon
        Return AdductIon.GetAdductIon(adductName)
    End Function

    Public Function ConvertDifferentChargedAdduct(ByVal adduct As AdductIon, ByVal chargeNumber As Integer) As AdductIon
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

    Public Function CalculateAccurateMassAndIsotopeRatioOfMolecularFormula(ByVal rawFormula As String) As (Double, Double, Double)
        Dim formula As String = Nothing, multipliedNum As Double = Nothing
        Dim nil As (formula As String, multipliedNum As Double) = GetFormulaAndNumber(rawFormula)

        formula = nil.formula
        multipliedNum = nil.multipliedNum

        If String.IsNullOrWhiteSpace(formula) Then
            Return (multipliedNum, 0, 0)
        End If

        'Common adduct check
        Dim commonAcurateMass As Double = Nothing, m1Intensity As Double = Nothing, m2Intensity As Double = Nothing

        If IsCommonAdduct(formula, multipliedNum, commonAcurateMass, m1Intensity, m2Intensity) Then
            Return (commonAcurateMass, m1Intensity, m2Intensity)
        End If

        ' check irons
        Dim iron As Double = Nothing

        If IronToMass.TryGetValue(formula, iron) Then
            Return (multipliedNum * iron, 0, 0)
        End If

        'Organic compound adduct check
        Dim formulaBean As Formula = Nothing, organicAcurateMass As Double = Nothing
        Dim nil2 As (formulaBean As Dictionary(Of String, Integer), organicAcurateMass As Double) = GetOrganicAdductFormulaAndMass(formula, multipliedNum)
        Return (organicAcurateMass, SevenGoldenRulesCheck.GetM1IsotopicAbundance(formulaBean), SevenGoldenRulesCheck.GetM2IsotopicAbundance(formulaBean))
    End Function

    Public Function CalculateAccurateMassAndIsotopeRatio(ByVal adductName As String) As (Double, Double, Double)
        adductName = adductName.Split("["c)(1).Split("]"c)(0).Trim()

        If Not adductName.Contains("+"c) AndAlso Not adductName.Contains("-"c) Then
            Return (0R, 0R, 0R)
        End If

        Dim equationNum = CountChar(adductName, "+"c) + CountChar(adductName, "-"c)
        Dim formula = String.Empty
        Dim accAccurateMass As Double = 0, accM1Intensity As Double = 0, accM2Intensity As Double = 0
        Dim accurateMass As Double = Nothing, m1Intensity As Double = Nothing, m2Intensity As Double = Nothing
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
            If equationNum <= 0 Then Exit For
        Next
        Return (accAccurateMass, accM1Intensity, accM2Intensity)
    End Function
End Module
