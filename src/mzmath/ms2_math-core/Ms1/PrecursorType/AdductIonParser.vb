#Region "Microsoft.VisualBasic::ce2fe4ff04c68b07a6338d4a5c487995, mzmath\ms2_math-core\Ms1\PrecursorType\AdductIonParser.vb"

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
    '    Code Lines: 357
    ' Comment Lines: 13
    '   Blank Lines: 34
    '     File Size: 22.20 KB


    '     Module AdductIonParser
    ' 
    '         Function: GetAdductContent, GetAdductIonXmer, GetChargeNumber, GetFormulaAndNumber, GetIonType
    '                   GetOrganicAdductFormulaAndMass, GetRadicalInfo, IonTypeFormatChecker, IsCommonAdduct
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports std = System.Math

Namespace Ms1.PrecursorType

    ''' <summary>
    ''' MS-DIAL AdductIonParser
    ''' </summary>
    Public Module AdductIonParser

        Private ReadOnly c13_c12 As Double = 0.010815728
        Private ReadOnly h2_h1 As Double = 0.000115013
        Private ReadOnly n15_n14 As Double = 0.003653298
        Private ReadOnly o17_o16 As Double = 0.000380926
        Private ReadOnly o18_o16 As Double = 0.002054994
        Private ReadOnly s33_s32 As Double = 0.007895568
        Private ReadOnly s34_s32 As Double = 0.044741552
        Private ReadOnly k40_k39 As Double = 0.000125458
        Private ReadOnly k41_k39 As Double = 0.072167458
        Private ReadOnly ni60_ni58 As Double = 0.385196175

        Public IronToMass As New ReadOnlyDictionary(Of String, Double)(New Dictionary(Of String, Double)() From {
            {"Be", 9.0121822},
            {"Mg", 23.9850417},
            {"Al", 26.98153863},
            {"Ca", 39.96259098},
            {"Sc", 44.9559119},
            {"Ti", 47.9479463},
            {"V", 50.9439595},
            {"Cr", 51.9405075},
            {"Mn", 54.9380451},
            {"Fe", 55.9349375},
            {"Cu", 62.9295975},
            {"Zn", 63.9291422},
            {"Ga", 68.9255736},
            {"Ge", 73.9211778},
            {"Se", 79.9165213},
            {"Kr", 83.911507},
            {"Rb", 84.911789738},
            {"Sr", 87.9056121},
            {"Zr", 89.9047044},
            {"Nb", 92.9063781},
            {"Mo", 97.9054082},
            {"Ru", 101.9043493},
            {"Pd", 105.903486},
            {"Ag", 106.905097},
            {"Cd", 113.9033585},
            {"In", 114.903878},
            {"Sn", 119.9021947},
            {"Sb", 120.9038157},
            {"Cs", 132.905451933},
            {"La", 138.9063533}
            })

        Public Function GetAdductContent(adduct As String) As String
            Dim trimedAdductName = adduct.Split("["c)(1).Split("]"c)(0).Trim()
            If Not trimedAdductName.Contains("+"c) AndAlso Not trimedAdductName.Contains("-"c) Then Return String.Empty

            Dim isCharacterMappeared = False
            Dim isNextEquationAppreared = False
            Dim contentString = String.Empty

            For i = 0 To trimedAdductName.Length - 1
                If trimedAdductName(i) = "M"c Then
                    isCharacterMappeared = True
                    Continue For
                End If

                'if (isCharacterMappeared && (trimedAdductName[i] == '-' || trimedAdductName[i] == '+')) {
                '    isNextEquationAppreared = true;
                '    continue;
                '}

                If isCharacterMappeared Then
                    contentString += trimedAdductName(i)
                End If
            Next
            Return contentString.Trim()
        End Function

        Private ReadOnly XmerCheckTemplate As New Regex("\[(?<x>\d+)?")

        Public Function GetAdductIonXmer(adductName As String) As Integer
            Dim match = XmerCheckTemplate.Match(adductName)
            If match.Success Then
                If match.Groups("x").Success Then
                    Return Integer.Parse(match.Groups("x").Value)
                End If
            End If
            Return 1
        End Function

        ReadOnly FormatCheckTemplate As New Regex("^\[[^\]\[\(\)]+\](?!.*[\]\[\(\)]).*[+-].*$")

        Public Function IonTypeFormatChecker(adductName As String) As Boolean
            Return Not Equals(adductName, Nothing) AndAlso FormatCheckTemplate.IsMatch(adductName)
        End Function

        Public Function GetIonType(adductName As String) As IonModes
            Dim chargeString = adductName.Split("]"c)(1)

            If chargeString.Contains("+"c) Then
                Return IonModes.Positive
            Else
                Return IonModes.Negative
            End If
        End Function

        Public Function GetRadicalInfo(adductName As String) As Boolean
            Dim chargeString = adductName.Split("]"c)(1)
            Return chargeString.Contains("."c)
        End Function


        Private ReadOnly ChargeNumberTemplate As Regex = New Regex("](?<charge>\d+)?[+-]")
        Public Function GetChargeNumber(adductName As String) As Integer
            Dim match = ChargeNumberTemplate.Match(adductName)
            If match.Success Then
                If match.Groups("charge").Success Then
                    Return Integer.Parse(match.Groups("charge").Value)
                Else
                    Return 1
                End If
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' 2H -> H, 2
        ''' 3Na -> Na, 3
        ''' </summary>
        ''' <param name="formula"></param>
        ''' <returns></returns>
        Public Function GetFormulaAndNumber(formula As String) As (formula As String, multipliedNum As Double)
            Dim numString = String.Empty

            For i As Integer = 0 To formula.Length - 1
                If Char.IsNumber(formula(i)) Then
                    numString += formula(i)
                Else
                    Exit For
                End If
            Next

            Return (formula.Substring(numString.Length), If(String.IsNullOrEmpty(numString), 1, Double.Parse(numString)))
        End Function

        Public Function IsCommonAdduct(formula As String, multipliedNumber As Double,
                                       <Out> ByRef acurateMass As Double,
                                       <Out> ByRef m1Intensity As Double,
                                       <Out> ByRef m2Intensity As Double) As Boolean

            If formula.Equals("Na") Then
                acurateMass = multipliedNumber * 22.9897692809
                m1Intensity = 0
                m2Intensity = 0
                Return True
            ElseIf formula.Equals("H") Then
                acurateMass = multipliedNumber * 1.00782503207
                m1Intensity = multipliedNumber * h2_h1
                m2Intensity = multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2)
                Return True
            ElseIf formula.Equals("K") Then
                acurateMass = multipliedNumber * 38.96370668
                m1Intensity = multipliedNumber * k40_k39
                m2Intensity = multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(k40_k39, 2) + multipliedNumber * k41_k39
                Return True
            ElseIf formula.Equals("Li") Then
                acurateMass = multipliedNumber * 7.01600455
                m1Intensity = 0
                m2Intensity = 0
                Return True
            ElseIf formula.Equals("ACN") OrElse formula.Equals("CH3CN") Then
                acurateMass = multipliedNumber * 41.0265491
                m1Intensity = multipliedNumber * (2 * c13_c12 + 3 * h2_h1 + n15_n14)
                m2Intensity = multipliedNumber * (2 * c13_c12 * 3 * h2_h1 + 2 * c13_c12 * n15_n14 + 3 * h2_h1 * n15_n14) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 3 * multipliedNumber * (3 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(n15_n14, 2)
                Return True
            ElseIf formula.Equals("NH4") Then
                acurateMass = multipliedNumber * 18.03437413
                m1Intensity = multipliedNumber * (4 * h2_h1 + n15_n14)
                m2Intensity = multipliedNumber * (4 * h2_h1 * n15_n14) + 4 * multipliedNumber * (4 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(n15_n14, 2)
                Return True
            ElseIf formula.Equals("CH3OH") Then
                acurateMass = multipliedNumber * 32.02621475
                m1Intensity = multipliedNumber * (c13_c12 + 4 * h2_h1 + o17_o16)
                m2Intensity = multipliedNumber * (c13_c12 * 4 * h2_h1 + c13_c12 * o17_o16 + 4 * h2_h1 * o17_o16) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 4 * multipliedNumber * (4 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("H2O") Then
                acurateMass = multipliedNumber * 18.01056468
                m1Intensity = multipliedNumber * (2 * h2_h1 + o17_o16)
                m2Intensity = multipliedNumber * (2 * h2_h1 * o17_o16) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("IsoProp") OrElse formula.Equals("C3H7OH") Then
                acurateMass = multipliedNumber * 60.05751488
                m1Intensity = multipliedNumber * (3 * c13_c12 + 8 * h2_h1 + o17_o16)
                m2Intensity = multipliedNumber * (3 * c13_c12 * 8 * h2_h1 + 3 * c13_c12 * o17_o16 + 8 * h2_h1 * o17_o16) + 3 * multipliedNumber * (3 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 8 * multipliedNumber * (8 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("DMSO") OrElse formula.Equals("C2H6OS") Then
                acurateMass = multipliedNumber * 78.013936
                m1Intensity = multipliedNumber * (2 * c13_c12 + 6 * h2_h1 + o17_o16 + s33_s32)
                m2Intensity = multipliedNumber * (2 * c13_c12 * 6 * h2_h1 + 2 * c13_c12 * o17_o16 + 2 * c13_c12 * s33_s32 + 6 * h2_h1 * o17_o16 + 6 * h2_h1 * s33_s32 + o17_o16 * s33_s32) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 6 * multipliedNumber * (6 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(s33_s32, 2) + multipliedNumber * o18_o16 + multipliedNumber * s34_s32
                Return True
            ElseIf formula.Equals("FA") OrElse formula.Equals("HCOOH") Then
                acurateMass = multipliedNumber * 46.005479
                m1Intensity = multipliedNumber * (c13_c12 + 2 * h2_h1 + 2 * o17_o16)
                m2Intensity = multipliedNumber * (c13_c12 * 2 * h2_h1 + c13_c12 * 2 * o17_o16 + 2 * h2_h1 * 2 * o17_o16) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + 2 * multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("HCOO") Then
                acurateMass = multipliedNumber * 44.997654
                m1Intensity = multipliedNumber * (c13_c12 + 1 * h2_h1 + 2 * o17_o16)
                m2Intensity = multipliedNumber * (c13_c12 * 1 * h2_h1 + c13_c12 * 2 * o17_o16 + 1 * h2_h1 * 2 * o17_o16) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 1 * multipliedNumber * (1 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + 2 * multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("Hac") OrElse formula.Equals("CH3COOH") OrElse formula.Equals("CH3CO2H") OrElse formula.Equals("C2H4O2") Then
                acurateMass = multipliedNumber * 60.021129
                m1Intensity = multipliedNumber * (2 * c13_c12 + 4 * h2_h1 + 2 * o17_o16)
                m2Intensity = multipliedNumber * (2 * c13_c12 * 4 * h2_h1 + 2 * c13_c12 * 2 * o17_o16 + 4 * h2_h1 * 2 * o17_o16) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 4 * multipliedNumber * (4 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + 2 * multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("CH3COO") Then
                acurateMass = multipliedNumber * 59.013305
                m1Intensity = multipliedNumber * (2 * c13_c12 + 3 * h2_h1 + 2 * o17_o16)
                m2Intensity = multipliedNumber * (2 * c13_c12 * 3 * h2_h1 + 2 * c13_c12 * 2 * o17_o16 + 3 * h2_h1 * 2 * o17_o16) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 3 * multipliedNumber * (3 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + 2 * multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("CH3COONa") Then
                acurateMass = multipliedNumber * 82.003075
                m1Intensity = multipliedNumber * (2 * c13_c12 + 3 * h2_h1 + 2 * o17_o16)
                m2Intensity = multipliedNumber * (2 * c13_c12 * 3 * h2_h1 + 2 * c13_c12 * 2 * o17_o16 + 3 * h2_h1 * 2 * o17_o16) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + 3 * multipliedNumber * (3 * multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + 2 * multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("TFA") OrElse formula.Equals("CF3COOH") Then
                acurateMass = multipliedNumber * 113.992864
                m1Intensity = multipliedNumber * (2 * c13_c12 + h2_h1 + 2 * o17_o16)
                m2Intensity = multipliedNumber * (2 * c13_c12 * h2_h1 + 2 * c13_c12 * 2 * o17_o16 + h2_h1 * 2 * o17_o16) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(c13_c12, 2) + multipliedNumber * (multipliedNumber - 1) * 0.5 * std.Pow(h2_h1, 2) + 2 * multipliedNumber * (2 * multipliedNumber - 1) * 0.5 * std.Pow(o17_o16, 2) + 2 * multipliedNumber * o18_o16
                Return True
            ElseIf formula.Equals("Co") Then
                acurateMass = multipliedNumber * 58.933195
                m1Intensity = 0
                m2Intensity = 0
                Return True
            ElseIf formula.Equals("Ni") Then
                acurateMass = multipliedNumber * 57.9353429
                m1Intensity = 0
                m2Intensity = multipliedNumber * ni60_ni58
                Return True
            ElseIf formula.Equals("Ba") Then
                acurateMass = multipliedNumber * 137.9052472
                m1Intensity = 0
                m2Intensity = 0
                Return True
            End If

            acurateMass = 0R
            m1Intensity = 0R
            m2Intensity = 0R
            Return False
        End Function

        Public Function GetOrganicAdductFormulaAndMass(formula As String, multipliedNumber As Double) As (Dictionary(Of String, Integer), Double)
            Dim formulaBean = New Dictionary(Of String, Integer)
            Dim acurateMass = 0R
            Dim mc = Regex.Matches(formula, "C(?!a|d|e|l|o|r|s|u)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 12.0 * multipliedNumber
                    formulaBean!C = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 12.0 * multipliedNumber
                        formulaBean!C = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "H(?!e|f|g|o)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 1.00782503207 * multipliedNumber
                    formulaBean!H = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 1.00782503207 * multipliedNumber
                        formulaBean!H = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "N(?!a|b|d|e|i)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 14.0030740048 * multipliedNumber
                    formulaBean!N = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 14.0030740048 * multipliedNumber
                        formulaBean!N = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "O(?!s)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 15.99491461956 * multipliedNumber
                    formulaBean!O = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 15.99491461956 * multipliedNumber
                        formulaBean!O = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "S(?!b|c|e|i|m|n|r)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 31.972071 * multipliedNumber
                    formulaBean!S = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 31.972071 * multipliedNumber
                        formulaBean!S = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "Br([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 78.9183371 * multipliedNumber
                    formulaBean!Br = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 78.9183371 * multipliedNumber
                        formulaBean!Br = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "Cl([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 34.96885268 * multipliedNumber
                    formulaBean!Cl = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 34.96885268 * multipliedNumber
                        formulaBean!Cl = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "F(?!e)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 18.99840322 * multipliedNumber
                    formulaBean!F = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 18.99840322 * multipliedNumber
                        formulaBean!F = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "I(?!n|r)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 126.904473 * multipliedNumber
                    formulaBean!I = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 126.904473 * multipliedNumber
                        formulaBean!I = CInt(num * multipliedNumber)
                    End If
                End If
            End If

            mc = Regex.Matches(formula, "P(?!d|t|b|r)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    acurateMass += 30.97376163 * multipliedNumber
                    formulaBean!P = CInt(multipliedNumber)
                Else
                    Dim num As Double
                    If Double.TryParse(mc(0).Groups(1).Value, num) Then
                        acurateMass += num * 30.97376163 * multipliedNumber
                        formulaBean!P = CInt(num + multipliedNumber)
                    End If
                End If
            End If
            Return (formulaBean, acurateMass)
        End Function
    End Module
End Namespace
