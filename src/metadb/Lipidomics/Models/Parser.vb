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
            (formula, multipliedNum) = GetFormulaAndNumber(rawFormula)

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
            (formulaBean, organicAcurateMass) = GetOrganicAdductFormulaAndMass(formula, multipliedNum)
            Return (organicAcurateMass, SevenGoldenRulesCheck.GetM1IsotopicAbundance(formulaBean), SevenGoldenRulesCheck.GetM2IsotopicAbundance(formulaBean))
    End Function
End Module
