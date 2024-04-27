#Region "Microsoft.VisualBasic::507d64108a249c4e02655174ccdf03f4, G:/mzkit/src/metadb/Chemoinformatics//Formula/SevenGoldenRulesCheck.vb"

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

    '   Total Lines: 404
    '    Code Lines: 329
    ' Comment Lines: 36
    '   Blank Lines: 39
    '     File Size: 21.88 KB


    '     Enum CoverRange
    ' 
    '         CommonRange, ExtendedRange, ExtremeRange
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class SevenGoldenRulesCheck
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Check, GetIsotopicDifference, GetIsotopicPeaks, GetM1IsotopicAbundance, GetM2IsotopicAbundance
    '                   HeteroAtomCheck, ProbabilityCheck, ValenceCheck, ValenceCheckByHydrogenShift
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports MassDiffDictionary = BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass

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

        Public Shared Function Check(formula As Formula, isValenceCheck As Boolean, coverRange As CoverRange, isElementProbabilityCheck As Boolean,
                                     Optional adduct As AdductIon = Nothing) As Boolean

            Static hydrogenShiftCheck As Index(Of String) = {"[M]+", "[M]-", "[M-2H]-"}

            If adduct IsNot Nothing AndAlso adduct.AdductIonName Like hydrogenShiftCheck Then
                If isValenceCheck AndAlso Not ValenceCheckByHydrogenShift(formula) Then
                    Return False
                End If
            Else
                If isValenceCheck AndAlso Not ValenceCheck(formula) Then
                    Return False
                End If
            End If

            If Not HeteroAtomCheck(formula, coverRange) Then
                Return False
            End If

            If isElementProbabilityCheck AndAlso Not ProbabilityCheck(formula) Then
                Return False
            End If

            Return True
        End Function

        Public Shared Function ValenceCheck(formula As Formula) As Boolean
            Dim atomTotal = formula!Br + formula!Cl + formula!C + formula!F + formula!H + formula!I + formula!N + formula!O + formula!P + formula!Si + formula!S
            Dim oddValenceAtomTotal = formula!Br + formula!Cl + formula!F + formula!H + formula!I + formula!N + formula!P
            Dim valenceTotal = ValenceDict("Br") * formula!Br + ValenceDict("Cl") * formula!Cl +
                ValenceDict("C") * formula!C + ValenceDict("F") * formula!F +
                ValenceDict("H") * formula!H + ValenceDict("I") * formula!I +
                ValenceDict("N") * formula!N + ValenceDict("O") * formula!O +
                ValenceDict("P") * formula!P + ValenceDict("Si") * formula!Si +
                ValenceDict("S") * formula!S

            If oddValenceAtomTotal Mod 2 = 1 AndAlso valenceTotal Mod 2 = 1 Then Return False
            If valenceTotal < 2 * (atomTotal - 1) Then Return False

            Return True
        End Function

        ''' <summary>
        ''' Rule #4 – Hydrogen/Carbon element ratio check
        ''' 
        ''' + In most cases the hydrogen/carbon ratio does not exceed ``H/C`` > 3 with rare exception such as in methylhydrazine (CH6N2).
        ''' + Conversely, the ``H/C`` ratio Is usually smaller than 2, And should Not be less than 0.125 Like in the case of tetracyanopyrrole (C8HN5).
        ''' + Most typical ratios are found between ``2.0 > H/C > 0.5``
        ''' + More than 99.7% Of all formulas were included With H/C ratios between ``0.2–3.1``. Consequently, we Call this range the 'common range'.
        ''' + However, a number of chemical classes fall out of this range, And we have hence enabled the user to select 'extended ranges' covering 99.99% of all formulas in this development database (H/C 0.1–6).
        ''' </summary>
        Public Shared Function ValenceCheckByHydrogenShift(formula As Formula) As Boolean
            Dim atomTotal = formula!Br + formula!Cl + formula!C + formula!F + formula!H + formula!I + formula!N + formula!O + formula!P + formula!Si + formula!S
            Dim oddValenceAtomTotal = formula!Br + formula!Cl + formula!F + formula!H + formula!I + formula!N + formula!P
            Dim valenceTotal = ValenceDict("Br") * formula!Br +
                ValenceDict("Cl") * formula!Cl + ValenceDict("C") * formula!C +
                ValenceDict("F") * formula!F + ValenceDict("H") * formula!H +
                ValenceDict("I") * formula!I + ValenceDict("N") * formula!N +
                ValenceDict("O") * formula!O + ValenceDict("P") * formula!P +
                ValenceDict("Si") * formula!Si + ValenceDict("S") * formula!S

            Dim shiftMinus = -1, shiftPlus = 1

            Dim atomTotalMinusShifted = atomTotal + shiftMinus
            Dim oddValenceAtomTotalMinusShifted = oddValenceAtomTotal + shiftMinus
            Dim valenceTotalMinusShifted = valenceTotal + shiftMinus

            Dim atomTotalPlusShifted = atomTotal + shiftPlus
            Dim oddValenceAtomTotalPlusShifted = oddValenceAtomTotal + shiftPlus
            Dim valenceTotalPlusShifted = valenceTotal + shiftPlus

            If oddValenceAtomTotalMinusShifted Mod 2 = 1 AndAlso
                valenceTotalMinusShifted Mod 2 = 1 AndAlso
                oddValenceAtomTotalPlusShifted Mod 2 = 1 AndAlso
                valenceTotalPlusShifted Mod 2 = 1 Then
                Return False
            End If

            If valenceTotalMinusShifted < 2 * (atomTotalMinusShifted - 1) AndAlso
                valenceTotalPlusShifted < 2 * (atomTotalPlusShifted - 1) Then
                Return False
            End If

            Return True
        End Function

        Public Shared Iterator Function GetIsotopicPeaks(formula As Formula) As IEnumerable(Of IsotopicPeak)
            Dim formulaMass As Double = formula.ExactMass

            Yield New IsotopicPeak() With {
                .RelativeAbundance = 1,
                .Mass = formulaMass,
                .Comment = formula.ToString
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * c13_c12,
                .Mass = formulaMass + MassDiffDictionary.C13_C12,
                .Comment = "13C"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!H * h2_h1,
                .Mass = formulaMass + MassDiffDictionary.H2_H1,
                .Comment = "2H"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!N * n15_n14,
                .Mass = formulaMass + MassDiffDictionary.N15_N14,
                .Comment = "15N"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!O * o17_o16,
                .Mass = formulaMass + MassDiffDictionary.O17_O16,
                .Comment = "17O"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!S * s33_s32,
                .Mass = formulaMass + MassDiffDictionary.S33_S32,
                .Comment = "33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!Si * si29_si28,
                .Mass = formulaMass + MassDiffDictionary.Si29_Si28,
                .Comment = "29Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * c13_c12 * formula!H * h2_h1,
                .Mass = formulaMass + MassDiffDictionary.C13_C12 + MassDiffDictionary.H2_H1,
                .Comment = "2H,13C"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * c13_c12 * formula!N * n15_n14,
                .Mass = formulaMass + MassDiffDictionary.C13_C12 + MassDiffDictionary.N15_N14,
                .Comment = "13C,15N"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * c13_c12 * formula!O * o17_o16,
                .Mass = formulaMass + MassDiffDictionary.C13_C12 + MassDiffDictionary.O17_O16,
                .Comment = "13C,17O"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * c13_c12 * formula!S * s33_s32,
                .Mass = formulaMass + MassDiffDictionary.C13_C12 + MassDiffDictionary.S33_S32,
                .Comment = "13C,33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * c13_c12 * formula!Si * si29_si28,
                .Mass = formulaMass + MassDiffDictionary.C13_C12 + MassDiffDictionary.Si29_Si28,
                .Comment = "13C,29Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!H * h2_h1 * formula!N * n15_n14,
                .Mass = formulaMass + MassDiffDictionary.H2_H1 + MassDiffDictionary.N15_N14,
                .Comment = "2H,15N"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!H * h2_h1 * formula!O * o17_o16,
                .Mass = formulaMass + MassDiffDictionary.H2_H1 + MassDiffDictionary.O17_O16,
                .Comment = "2H,17O"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!H * h2_h1 * formula!S * s33_s32,
                .Mass = formulaMass + MassDiffDictionary.H2_H1 + MassDiffDictionary.S33_S32,
                .Comment = "2H,33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!H * h2_h1 * formula!Si * si29_si28,
                .Mass = formulaMass + MassDiffDictionary.H2_H1 + MassDiffDictionary.Si29_Si28,
                .Comment = "2H,29Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!N * n15_n14 * formula!O * o17_o16,
                .Mass = formulaMass + MassDiffDictionary.N15_N14 + MassDiffDictionary.O17_O16,
                .Comment = "15N,17O"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!N * n15_n14 * formula!S * s33_s32,
                .Mass = formulaMass + MassDiffDictionary.N15_N14 + MassDiffDictionary.S33_S32,
                .Comment = "15N,33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!N * n15_n14 * formula!Si * si29_si28,
                .Mass = formulaMass + MassDiffDictionary.N15_N14 + MassDiffDictionary.Si29_Si28,
                .Comment = "15N,29Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!O * o17_o16 * formula!S * s33_s32,
                .Mass = formulaMass + MassDiffDictionary.O17_O16 + MassDiffDictionary.S33_S32,
                .Comment = "17O,33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!O * o17_o16 * formula!Si * si29_si28,
                .Mass = formulaMass + MassDiffDictionary.O17_O16 + MassDiffDictionary.Si29_Si28,
                .Comment = "17O,29Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!S * s33_s32 * formula!Si * si29_si28,
                .Mass = formulaMass + MassDiffDictionary.S33_S32 + MassDiffDictionary.Si29_Si28,
                .Comment = "29Si,33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!C * (formula!C - 1) * 0.5 * Math.Pow(c13_c12, 2),
                .Mass = formulaMass + MassDiffDictionary.C13_C12 * 2.0,
                .Comment = "13C,13C"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!H * (formula!H - 1) * 0.5 * Math.Pow(h2_h1, 2),
                .Mass = formulaMass + MassDiffDictionary.H2_H1 * 2.0,
                .Comment = "2H,2H"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!N * (formula!N - 1) * 0.5 * Math.Pow(n15_n14, 2),
                .Mass = formulaMass + MassDiffDictionary.N15_N14 * 2.0,
                .Comment = "15N,15N"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!O * (formula!O - 1) * 0.5 * Math.Pow(o17_o16, 2),
                .Mass = formulaMass + MassDiffDictionary.O17_O16 * 2.0,
                .Comment = "17O,17O"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!S * (formula!S - 1) * 0.5 * Math.Pow(s33_s32, 2),
                .Mass = formulaMass + MassDiffDictionary.S33_S32 * 2.0,
                .Comment = "33S,33S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!Si * (formula!Si - 1) * 0.5 * Math.Pow(si29_si28, 2),
                .Mass = formulaMass + MassDiffDictionary.Si29_Si28 * 2.0,
                .Comment = "29Si,29Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!O * o18_o16,
                .Mass = formulaMass + MassDiffDictionary.O18_O16,
                .Comment = "18O"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!S * s34_s32,
                .Mass = formulaMass + MassDiffDictionary.S34_S32,
                .Comment = "34S"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!Si * si30_si28,
                .Mass = formulaMass + MassDiffDictionary.Si30_Si28,
                .Comment = "30Si"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!Br * br81_br79,
                .Mass = formulaMass + MassDiffDictionary.Br81_Br79,
                .Comment = "81Br"
            }
            Yield New IsotopicPeak() With {
                .RelativeAbundance = formula!Cl * cl37_cl35,
                .Mass = formulaMass + MassDiffDictionary.Cl37_Cl35,
                .Comment = "37Cl"
            }
        End Function

        Public Shared Function GetM1IsotopicAbundance(formula As Formula) As Double
            Dim abundance = formula!C * c13_c12 + formula!H * h2_h1 + formula!N * n15_n14 + formula!O * o17_o16 + formula!S * s33_s32 + formula!Si * si29_si28
            Return abundance
        End Function

        Public Shared Function GetM2IsotopicAbundance(formula As Formula) As Double
            Dim abundance = formula!C * c13_c12 * formula!H * h2_h1 + formula!C * c13_c12 * formula!N * n15_n14 + formula!C * c13_c12 * formula!O * o17_o16 + formula!C * c13_c12 * formula!S * s33_s32 + formula!C * c13_c12 * formula!Si * si29_si28 + formula!H * h2_h1 * formula!N * n15_n14 + formula!H * h2_h1 * formula!O * o17_o16 + formula!H * h2_h1 * formula!S * s33_s32 + formula!H * h2_h1 * formula!Si * si29_si28 + formula!N * n15_n14 * formula!O * o17_o16 + formula!N * n15_n14 * formula!S * s33_s32 + formula!N * n15_n14 * formula!Si * si29_si28 + formula!O * o17_o16 * formula!S * s33_s32 + formula!O * o17_o16 * formula!Si * si29_si28 + formula!S * s33_s32 * formula!Si * si29_si28 + formula!C * (formula!C - 1) * 0.5 * Math.Pow(c13_c12, 2) + formula!H * (formula!H - 1) * 0.5 * Math.Pow(h2_h1, 2) + formula!N * (formula!N - 1) * 0.5 * Math.Pow(n15_n14, 2) + formula!O * (formula!O - 1) * 0.5 * Math.Pow(o17_o16, 2) + formula!S * (formula!S - 1) * 0.5 * Math.Pow(s33_s32, 2) + formula!Si * (formula!Si - 1) * 0.5 * Math.Pow(si29_si28, 2) + formula!O * o18_o16 + formula!S * s34_s32 + formula!Si * si30_si28 + formula!Br * br81_br79 + formula!Cl * cl37_cl35
            Return abundance
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetIsotopicDifference(tAbundance As Double, m1Intensity As Double) As Double
            Return tAbundance - m1Intensity
        End Function

        ''' <summary>
        ''' Rule #4 – Hydrogen/Carbon element ratio check
        ''' 
        ''' + In most cases the hydrogen/carbon ratio does not exceed ``H/C`` > 3 with rare exception such as in methylhydrazine (CH6N2).
        ''' + Conversely, the ``H/C`` ratio Is usually smaller than 2, And should Not be less than 0.125 Like in the case of tetracyanopyrrole (C8HN5).
        ''' + Most typical ratios are found between ``2.0 > H/C > 0.5``
        ''' + More than 99.7% Of all formulas were included With H/C ratios between ``0.2–3.1``. Consequently, we Call this range the 'common range'.
        ''' + However, a number of chemical classes fall out of this range, And we have hence enabled the user to select 'extended ranges' covering 99.99% of all formulas in this development database (H/C 0.1–6).
        ''' </summary>
        Public Shared Function HeteroAtomCheck(formula As Formula, coverRange As CoverRange) As Boolean
            Dim cnum As Double = formula!C, nnum As Double = formula!N, onum As Double = formula!O, pnum As Double = formula!P, snum As Double = formula!S, hnum As Double = formula!H, fnum As Double = formula!F, clnum As Double = formula!Cl, brnum As Double = formula!Br, inum As Double = formula!I, sinum As Double = formula!Si
            Dim n_c = nnum / cnum, o_c = onum / cnum, p_c = pnum / cnum, s_c = snum / cnum, h_c = hnum / cnum, f_c = fnum / cnum, cl_c = clnum / cnum, br_c = brnum / cnum, i_c = inum / cnum, si_c = sinum / cnum
            Dim o_p As Double

            If pnum > 0 Then
                o_p = onum / pnum
            Else
                o_p = 4
            End If

            Select Case coverRange
                Case CoverRange.CommonRange
                    If h_c <= 4.0 AndAlso f_c <= 1.5 AndAlso cl_c <= 1.0 AndAlso br_c <= 1.0 AndAlso si_c <= 0.5 AndAlso n_c <= 2.0 AndAlso o_c <= 2.5 AndAlso p_c <= 0.5 AndAlso s_c <= 1.0 AndAlso i_c <= 0.5 AndAlso o_p >= 2.0 Then
                        Return True
                    Else
                        Return False
                    End If
                Case CoverRange.ExtendedRange
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

        ''' <summary>
        ''' Rule #6 – element probability check
        ''' 
        ''' Multiple element count restriction for compounds &lt; 2000 Da, 
        ''' based on the examination of the Beilstein database and the 
        ''' Dictionary of Natural Products
        ''' </summary>
        Public Shared Function ProbabilityCheck(formula As Formula) As Boolean
            If formula!N > 1 AndAlso formula!O > 1 AndAlso formula!P > 1 AndAlso formula!S > 1 Then
                If formula!N >= 10 OrElse formula!O >= 20 OrElse formula!P >= 4 OrElse formula!S >= 3 Then Return False
            End If

            If formula!N > 3 AndAlso formula!O > 3 AndAlso formula!P > 3 Then
                If formula!N >= 11 OrElse formula!O >= 22 OrElse formula!P >= 6 Then Return False
            End If

            If formula!O > 1 AndAlso formula!P > 1 AndAlso formula!S > 1 Then
                If formula!O >= 14 OrElse formula!P >= 3 OrElse formula!S >= 3 Then Return False
            End If

            If formula!N > 1 AndAlso formula!P > 1 AndAlso formula!S > 1 Then
                If formula!N >= 4 OrElse formula!P >= 3 OrElse formula!S >= 3 Then Return False
            End If

            If formula!N > 6 AndAlso formula!O > 6 AndAlso formula!S > 6 Then
                If formula!N >= 19 OrElse formula!O >= 14 OrElse formula!S >= 8 Then Return False
            End If

            Return True
        End Function

    End Class
End Namespace
