Namespace Formula

    Public Enum CoverRange
        CommonRange
        ExtendedRange
        ExtremeRange
    End Enum

    Public NotInheritable Class SevenGoldenRulesCheck
        Private Sub New()
        End Sub

        Private Shared c13_c12 As Double = 0.010815728
        Private Shared h2_h1 As Double = 0.000115013
        Private Shared n15_n14 As Double = 0.003653298
        Private Shared o17_o16 As Double = 0.000380926
        Private Shared o18_o16 As Double = 0.002054994
        Private Shared si29_si28 As Double = 0.050800776
        Private Shared si30_si28 As Double = 0.033527428
        Private Shared s33_s32 As Double = 0.007895568
        Private Shared s34_s32 As Double = 0.044741552
        Private Shared cl37_cl35 As Double = 0.319957761
        Private Shared br81_br79 As Double = 0.972775695

        'private static double c13_c12_MassDiff = 1.003354838;
        'private static double h2_h1_MassDiff = 1.006276746;
        'private static double n15_n14_MassDiff = 0.997034893;
        'private static double o17_o16_MassDiff = 1.00421708;
        'private static double o18_o16_MassDiff = 2.00424638;
        'private static double si29_si28_MassDiff = 0.999568168;
        'private static double si30_si28_MassDiff = 1.996843638;
        'private static double s33_s32_MassDiff = 0.99938776;
        'private static double s34_s32_MassDiff = 1.9957959;
        'private static double cl37_cl35_MassDiff = 1.99704991;
        'private static double br81_br79_MassDiff = 1.9979535;

        Shared ReadOnly ValenceDict As New Dictionary(Of String, Integer) From {
    {"C", 4},
    {"H", 1},
    {"N", 3},
    {"O", 2},
    {"S", 6},
    {"P", 5},
    {"Si", 4},
    {"F", 1},
    {"Cl", 1},
    {"Br", 1},
    {"I", 1}
}

        Public Shared Function Check(ByVal formula As Formula, ByVal isValenceCheck As Boolean, ByVal coverRange As CoverRange, ByVal isElementProbabilityCheck As Boolean, ByVal adduct As AdductIon) As Boolean
            If Equals(adduct.AdductIonName, "[M]+") OrElse Equals(adduct.AdductIonName, "[M]-") OrElse Equals(adduct.AdductIonName, "[M-2H]-") Then
                If isValenceCheck AndAlso Not ValenceCheckByHydrogenShift(formula) Then Return False
            Else
                If isValenceCheck AndAlso Not ValenceCheck(formula) Then Return False
            End If


            If Not HeteroAtomCheck(formula, coverRange) Then Return False
            If isElementProbabilityCheck AndAlso Not ProbabilityCheck(formula) Then Return False

            Return True
        End Function

        Public Shared Function ValenceCheck(ByVal formula As Formula) As Boolean
            Dim atomTotal = formula!Br + formula!Cl + formula!C + formula!F + formula!H + formula!I + formula!N + formula!O + formula!P + formula!Si + formula!S
            Dim oddValenceAtomTotal = formula!Br + formula!Cl + formula!F + formula!H + formula!I + formula!N + formula!P
            Dim valenceTotal = ValenceDict("Br") * formula!Br + ValenceDict("Cl") * formula!Cl + ValenceDict("C") * formula!C + ValenceDict("F") * formula!F + ValenceDict("H") * formula!H + ValenceDict("I") * formula!I + ValenceDict("N") * formula!N + ValenceDict("O") * formula!O + ValenceDict("P") * formula!P + ValenceDict("Si") * formula!Si + ValenceDict("S") * formula!S

            If oddValenceAtomTotal Mod 2 = 1 AndAlso valenceTotal Mod 2 = 1 Then Return False
            If valenceTotal < 2 * (atomTotal - 1) Then Return False

            Return True
        End Function

        Public Shared Function ValenceCheckByHydrogenShift(ByVal formula As Formula) As Boolean
            Dim atomTotal = formula!Br + formula!Cl + formula!C + formula!F + formula!H + formula!I + formula!N + formula!O + formula!P + formula!Si + formula!S
            Dim oddValenceAtomTotal = formula.Brnum + formula.Clnum + formula.Fnum + formula.Hnum + formula.Inum + formula.Nnum + formula.Pnum
            Dim valenceTotal = ValenceDict("Br") * formula.Brnum + ValenceDict("Cl") * formula.Clnum + ValenceDict("C") * formula.Cnum + ValenceDict("F") * formula.Fnum + ValenceDict("H") * formula.Hnum + ValenceDict("I") * formula.Inum + ValenceDict("N") * formula.Nnum + ValenceDict("O") * formula.Onum + ValenceDict("P") * formula.Pnum + ValenceDict("Si") * formula.Sinum + ValenceDict("S") * formula.Snum

            Dim shiftMinus = -1, shiftPlus = 1

            Dim atomTotalMinusShifted = atomTotal + shiftMinus
            Dim oddValenceAtomTotalMinusShifted = oddValenceAtomTotal + shiftMinus
            Dim valenceTotalMinusShifted = valenceTotal + shiftMinus

            Dim atomTotalPlusShifted = atomTotal + shiftPlus
            Dim oddValenceAtomTotalPlusShifted = oddValenceAtomTotal + shiftPlus
            Dim valenceTotalPlusShifted = valenceTotal + shiftPlus

            If oddValenceAtomTotalMinusShifted Mod 2 = 1 AndAlso valenceTotalMinusShifted Mod 2 = 1 AndAlso oddValenceAtomTotalPlusShifted Mod 2 = 1 AndAlso valenceTotalPlusShifted Mod 2 = 1 Then Return False
            If valenceTotalMinusShifted < 2 * (atomTotalMinusShifted - 1) AndAlso valenceTotalPlusShifted < 2 * (atomTotalPlusShifted - 1) Then Return False

            Return True
        End Function

        Public Shared Function GetIsotopicPeaks(ByVal formula As Formula) As List(Of IsotopicPeak)
            Dim isotopicPeaks = New List(Of IsotopicPeak)()

            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = 1,
            .Mass = formula.Mass,
            .Comment = formula.FormulaString
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * c13_c12,
            .Mass = formula.Mass + MassDiffDictionary.C13_C12,
            .Comment = "13C"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Hnum * h2_h1,
            .Mass = formula.Mass + MassDiffDictionary.H2_H1,
            .Comment = "2H"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Nnum * n15_n14,
            .Mass = formula.Mass + MassDiffDictionary.N15_N14,
            .Comment = "15N"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Onum * o17_o16,
            .Mass = formula.Mass + MassDiffDictionary.O17_O16,
            .Comment = "17O"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Snum * s33_s32,
            .Mass = formula.Mass + MassDiffDictionary.S33_S32,
            .Comment = "33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Sinum * si29_si28,
            .Mass = formula.Mass + MassDiffDictionary.Si29_Si28,
            .Comment = "29Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * c13_c12 * formula.Hnum * h2_h1,
            .Mass = formula.Mass + MassDiffDictionary.C13_C12 + MassDiffDictionary.H2_H1,
            .Comment = "2H,13C"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * c13_c12 * formula.Nnum * n15_n14,
            .Mass = formula.Mass + MassDiffDictionary.C13_C12 + MassDiffDictionary.N15_N14,
            .Comment = "13C,15N"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * c13_c12 * formula.Onum * o17_o16,
            .Mass = formula.Mass + MassDiffDictionary.C13_C12 + MassDiffDictionary.O17_O16,
            .Comment = "13C,17O"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * c13_c12 * formula.Snum * s33_s32,
            .Mass = formula.Mass + MassDiffDictionary.C13_C12 + MassDiffDictionary.S33_S32,
            .Comment = "13C,33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * c13_c12 * formula.Sinum * si29_si28,
            .Mass = formula.Mass + MassDiffDictionary.C13_C12 + MassDiffDictionary.Si29_Si28,
            .Comment = "13C,29Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Hnum * h2_h1 * formula.Nnum * n15_n14,
            .Mass = formula.Mass + MassDiffDictionary.H2_H1 + MassDiffDictionary.N15_N14,
            .Comment = "2H,15N"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Hnum * h2_h1 * formula.Onum * o17_o16,
            .Mass = formula.Mass + MassDiffDictionary.H2_H1 + MassDiffDictionary.O17_O16,
            .Comment = "2H,17O"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Hnum * h2_h1 * formula.Snum * s33_s32,
            .Mass = formula.Mass + MassDiffDictionary.H2_H1 + MassDiffDictionary.S33_S32,
            .Comment = "2H,33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Hnum * h2_h1 * formula.Sinum * si29_si28,
            .Mass = formula.Mass + MassDiffDictionary.H2_H1 + MassDiffDictionary.Si29_Si28,
            .Comment = "2H,29Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Nnum * n15_n14 * formula.Onum * o17_o16,
            .Mass = formula.Mass + MassDiffDictionary.N15_N14 + MassDiffDictionary.O17_O16,
            .Comment = "15N,17O"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Nnum * n15_n14 * formula.Snum * s33_s32,
            .Mass = formula.Mass + MassDiffDictionary.N15_N14 + MassDiffDictionary.S33_S32,
            .Comment = "15N,33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Nnum * n15_n14 * formula.Sinum * si29_si28,
            .Mass = formula.Mass + MassDiffDictionary.N15_N14 + MassDiffDictionary.Si29_Si28,
            .Comment = "15N,29Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Onum * o17_o16 * formula.Snum * s33_s32,
            .Mass = formula.Mass + MassDiffDictionary.O17_O16 + MassDiffDictionary.S33_S32,
            .Comment = "17O,33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Onum * o17_o16 * formula.Sinum * si29_si28,
            .Mass = formula.Mass + MassDiffDictionary.O17_O16 + MassDiffDictionary.Si29_Si28,
            .Comment = "17O,29Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Snum * s33_s32 * formula.Sinum * si29_si28,
            .Mass = formula.Mass + MassDiffDictionary.S33_S32 + MassDiffDictionary.Si29_Si28,
            .Comment = "29Si,33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Cnum * (formula.Cnum - 1) * 0.5 * Math.Pow(c13_c12, 2),
            .Mass = formula.Mass + MassDiffDictionary.C13_C12 * 2.0,
            .Comment = "13C,13C"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Hnum * (formula.Hnum - 1) * 0.5 * Math.Pow(h2_h1, 2),
            .Mass = formula.Mass + MassDiffDictionary.H2_H1 * 2.0,
            .Comment = "2H,2H"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Nnum * (formula.Nnum - 1) * 0.5 * Math.Pow(n15_n14, 2),
            .Mass = formula.Mass + MassDiffDictionary.N15_N14 * 2.0,
            .Comment = "15N,15N"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Onum * (formula.Onum - 1) * 0.5 * Math.Pow(o17_o16, 2),
            .Mass = formula.Mass + MassDiffDictionary.O17_O16 * 2.0,
            .Comment = "17O,17O"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Snum * (formula.Snum - 1) * 0.5 * Math.Pow(s33_s32, 2),
            .Mass = formula.Mass + MassDiffDictionary.S33_S32 * 2.0,
            .Comment = "33S,33S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Sinum * (formula.Sinum - 1) * 0.5 * Math.Pow(si29_si28, 2),
            .Mass = formula.Mass + MassDiffDictionary.Si29_Si28 * 2.0,
            .Comment = "29Si,29Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Onum * o18_o16,
            .Mass = formula.Mass + MassDiffDictionary.O18_O16,
            .Comment = "18O"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Snum * s34_s32,
            .Mass = formula.Mass + MassDiffDictionary.S34_S32,
            .Comment = "34S"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Sinum * si30_si28,
            .Mass = formula.Mass + MassDiffDictionary.Si30_Si28,
            .Comment = "30Si"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Brnum * br81_br79,
            .Mass = formula.Mass + MassDiffDictionary.Br81_Br79,
            .Comment = "81Br"
        })
            isotopicPeaks.Add(New IsotopicPeak() With {
            .RelativeAbundance = formula.Clnum * cl37_cl35,
            .Mass = formula.Mass + MassDiffDictionary.Cl37_Cl35,
            .Comment = "37Cl"
        })

            Return isotopicPeaks
        End Function

        Public Shared Function GetM1IsotopicAbundance(ByVal formula As Formula) As Double
            Dim abundance = formula.Cnum * c13_c12 + formula.Hnum * h2_h1 + formula.Nnum * n15_n14 + formula.Onum * o17_o16 + formula.Snum * s33_s32 + formula.Sinum * si29_si28
            Return abundance
        End Function

        Public Shared Function GetM2IsotopicAbundance(ByVal formula As Formula) As Double
            Dim abundance = formula.Cnum * c13_c12 * formula.Hnum * h2_h1 + formula.Cnum * c13_c12 * formula.Nnum * n15_n14 + formula.Cnum * c13_c12 * formula.Onum * o17_o16 + formula.Cnum * c13_c12 * formula.Snum * s33_s32 + formula.Cnum * c13_c12 * formula.Sinum * si29_si28 + formula.Hnum * h2_h1 * formula.Nnum * n15_n14 + formula.Hnum * h2_h1 * formula.Onum * o17_o16 + formula.Hnum * h2_h1 * formula.Snum * s33_s32 + formula.Hnum * h2_h1 * formula.Sinum * si29_si28 + formula.Nnum * n15_n14 * formula.Onum * o17_o16 + formula.Nnum * n15_n14 * formula.Snum * s33_s32 + formula.Nnum * n15_n14 * formula.Sinum * si29_si28 + formula.Onum * o17_o16 * formula.Snum * s33_s32 + formula.Onum * o17_o16 * formula.Sinum * si29_si28 + formula.Snum * s33_s32 * formula.Sinum * si29_si28 + formula.Cnum * (formula.Cnum - 1) * 0.5 * Math.Pow(c13_c12, 2) + formula.Hnum * (formula.Hnum - 1) * 0.5 * Math.Pow(h2_h1, 2) + formula.Nnum * (formula.Nnum - 1) * 0.5 * Math.Pow(n15_n14, 2) + formula.Onum * (formula.Onum - 1) * 0.5 * Math.Pow(o17_o16, 2) + formula.Snum * (formula.Snum - 1) * 0.5 * Math.Pow(s33_s32, 2) + formula.Sinum * (formula.Sinum - 1) * 0.5 * Math.Pow(si29_si28, 2) + formula.Onum * o18_o16 + formula.Snum * s34_s32 + formula.Sinum * si30_si28 + formula.Brnum * br81_br79 + formula.Clnum * cl37_cl35
            Return abundance
        End Function

        Public Shared Function GetIsotopicDifference(ByVal tAbundance As Double, ByVal m1Intensity As Double) As Double
            Dim diff = tAbundance - m1Intensity
            Return diff
        End Function

        Public Shared Function HeteroAtomCheck(ByVal formula As Formula, ByVal coverRange As CoverRange) As Boolean
            Dim cnum As Double = formula.Cnum, nnum As Double = formula.Nnum, onum As Double = formula.Onum, pnum As Double = formula.Pnum, snum As Double = formula.Snum, hnum As Double = formula.Hnum, fnum As Double = formula.Fnum, clnum As Double = formula.Clnum, brnum As Double = formula.Brnum, inum As Double = formula.Inum, sinum As Double = formula.Sinum
            Dim n_c = nnum / cnum, o_c = onum / cnum, p_c = pnum / cnum, s_c = snum / cnum, h_c = hnum / cnum, f_c = fnum / cnum, cl_c = clnum / cnum, br_c = brnum / cnum, i_c = inum / cnum, si_c = sinum / cnum
            Dim o_p As Double
            If pnum > 0 Then
                o_p = onum / pnum
            Else
                o_p = 4
            End If

            Select Case coverRange
                Case coverRange.CommonRange
                    If h_c <= 4.0 AndAlso f_c <= 1.5 AndAlso cl_c <= 1.0 AndAlso br_c <= 1.0 AndAlso si_c <= 0.5 AndAlso n_c <= 2.0 AndAlso o_c <= 2.5 AndAlso p_c <= 0.5 AndAlso s_c <= 1.0 AndAlso i_c <= 0.5 AndAlso o_p >= 2.0 Then
                        Return True
                    Else
                        Return False
                    End If
                Case coverRange.ExtendedRange
                    If h_c <= 6.0 AndAlso f_c <= 3.0 AndAlso cl_c <= 3.0 AndAlso br_c <= 2.0 AndAlso si_c <= 1.0 AndAlso n_c <= 4.0 AndAlso o_c <= 6.0 AndAlso p_c <= 1.9 AndAlso s_c <= 3.0 AndAlso i_c <= 1.9 Then
                        Return True
                    Else
                        Return False
                    End If

                Case Else
                    If h_c <= 8.0 AndAlso f_c <= 4.0 AndAlso cl_c <= 4.0 AndAlso br_c <= 4.0 AndAlso si_c <= 3.0 AndAlso n_c <= 4.0 AndAlso o_c <= 10.0 AndAlso p_c <= 3.0 AndAlso s_c <= 6.0 AndAlso i_c <= 3.0 Then
                        Return True
                    Else
                        Return False
                    End If
            End Select
        End Function

        Public Shared Function ProbabilityCheck(ByVal formula As Formula) As Boolean
            If formula.Nnum > 1 AndAlso formula.Onum > 1 AndAlso formula.Pnum > 1 AndAlso formula.Snum > 1 Then
                If formula.Nnum >= 10 OrElse formula.Onum >= 20 OrElse formula.Pnum >= 4 OrElse formula.Snum >= 3 Then Return False
            End If

            If formula.Nnum > 3 AndAlso formula.Onum > 3 AndAlso formula.Pnum > 3 Then
                If formula.Nnum >= 11 OrElse formula.Onum >= 22 OrElse formula.Pnum >= 6 Then Return False
            End If

            If formula.Onum > 1 AndAlso formula.Pnum > 1 AndAlso formula.Snum > 1 Then
                If formula.Onum >= 14 OrElse formula.Pnum >= 3 OrElse formula.Snum >= 3 Then Return False
            End If

            If formula.Nnum > 1 AndAlso formula.Pnum > 1 AndAlso formula.Snum > 1 Then
                If formula.Nnum >= 4 OrElse formula.Pnum >= 3 OrElse formula.Snum >= 3 Then Return False
            End If

            If formula.Nnum > 6 AndAlso formula.Onum > 6 AndAlso formula.Snum > 6 Then
                If formula.Nnum >= 19 OrElse formula.Onum >= 14 OrElse formula.Snum >= 8 Then Return False
            End If

            Return True
        End Function

    End Class
End Namespace