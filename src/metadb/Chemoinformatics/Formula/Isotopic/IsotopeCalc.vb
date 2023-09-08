Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Math.Statistics

Namespace Formula.IsotopicPatterns
    ''' <summary>
    ''' This class will be used to calculate the theoretical isotopic abundances from formula string.
    ''' </summary>
    Public Module IsotopeCalculator
        ''' <summary>
        ''' This method calculate the theoretical isotopic abundances with the exact m/z from the molecular formula string such as C6H12O6.
        ''' </summary>
        ''' <param name="elementName">Put the formula string.</param>
        ''' <param name="massFilter">Put the integar value that you want ot calculate until the isotopic value. Ex. if you put 3, this method calculate the isotopic abundances until M+3. </param>
        ''' <param name="iupacReferenceBean">Put the iupac bean which can be retrived with IupacParcer.cs.</param>
        ''' <returns>This program returns the theoretical isotopic abundances with the exact m/z values.</returns>
        Public Function GetAccurateIsotopeProperty(elementName As String, massFilter As Integer, iupacReferenceBean As IupacDatabase) As IsotopeProperty
            Dim compoundPropertyBean As IsotopeProperty = New IsotopeProperty()
            compoundPropertyBean.Formula = FormulaScanner.Convert2FormulaObjV2(elementName)
            compoundPropertyBean.ElementProfile = GetBasicCompoundElementProfile(elementName)

            If compoundPropertyBean.ElementProfile Is Nothing Then Return Nothing

            setIupacReferenceInformation(compoundPropertyBean, iupacReferenceBean)

            If compoundPropertyBean.ElementProfile Is Nothing Then Return Nothing

            setAccurateIsotopePropertyInformation(compoundPropertyBean, massFilter)
            setFinalAccurateIsotopeProfile(compoundPropertyBean, massFilter)

            Return compoundPropertyBean
        End Function

        ''' <summary>
        ''' This method calculate the theoretical isotopic abundances with the nominal m/z from the molecular formula string such as C6H12O6.
        ''' </summary>
        ''' <param name="elementName">Put the formula string.</param>
        ''' <param name="massFilter">Put the integar value that you want ot calculate until the isotopic value. Ex. if you put 3, this method calculate the isotopic abundances until M+3. </param>
        ''' <param name="iupacReferenceBean">Put the iupac bean which can be retrived with IupacParcer.cs.</param>
        ''' <returns>This program returns the theoretical isotopic abundances with the nominal m/z values.</returns>
        Public Function GetNominalIsotopeProperty(elementName As String, massFilter As Integer, iupacReferenceBean As IupacDatabase) As IsotopeProperty
            Dim compoundPropertyBean As IsotopeProperty = New IsotopeProperty()
            compoundPropertyBean.Formula = FormulaScanner.Convert2FormulaObjV2(elementName)
            compoundPropertyBean.ElementProfile = GetBasicCompoundElementProfile(elementName)

            If compoundPropertyBean.ElementProfile Is Nothing Then Return Nothing

            setIupacReferenceInformation(compoundPropertyBean, iupacReferenceBean)

            If compoundPropertyBean.ElementProfile Is Nothing Then Return Nothing

            setNominalIsotopePropertyInformation(compoundPropertyBean, massFilter)
            setFinalNominalIsotopeProfile(compoundPropertyBean, massFilter)

            Return compoundPropertyBean
        End Function

        Public Function GetBasicCompoundElementProfile(formula As String) As List(Of AtomProperty)
            Dim elementProfileList As List(Of AtomProperty) = New List(Of AtomProperty)()
            Dim elementPropertyBean As AtomProperty = New AtomProperty()
            Dim mc As MatchCollection

            If Char.IsNumber(formula(0)) Then
                Console.WriteLine("The element composition name format is incorrect.")
                Return Nothing
            End If

            mc = Regex.Matches(formula, "C(?!a|d|e|l|o|r|s|u)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "C"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "H(?!e|f|g|o)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "H"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "N(?!a|b|d|e|i)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "N"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "O(?!s)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "O"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "S(?!b|c|e|i|m|n|r)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "S"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "Br([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "Br"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "Cl([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "Cl"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "F(?!e)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "F"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            mc = Regex.Matches(formula, "I(?!n|r)([0-9]*)", RegexOptions.None)
            If mc.Count > 0 Then
                elementPropertyBean = New AtomProperty()
                elementPropertyBean.ElementName = "I"
                If Equals(mc(0).Groups(1).Value, String.Empty) Then
                    elementPropertyBean.ElementNumber = 1
                Else
                    elementPropertyBean.ElementNumber = Integer.Parse(mc(0).Groups(1).Value)
                End If
                elementProfileList.Add(elementPropertyBean)
            End If

            If elementProfileList.Count = 0 Then Return Nothing

            Return elementProfileList
        End Function

        Private Sub setIupacReferenceInformation(compoundPropertyBean As IsotopeProperty, iupacReferenceBean As IupacDatabase)
            Dim accurateMass As Double = 0
            For i = 0 To compoundPropertyBean.ElementProfile.Count - 1
                If iupacReferenceBean.ElementName2AtomElementProperties.ContainsKey(compoundPropertyBean.ElementProfile(i).ElementName) Then
                    compoundPropertyBean.ElementProfile(i).AtomElementProperties = iupacReferenceBean.ElementName2AtomElementProperties(compoundPropertyBean.ElementProfile(i).ElementName)
                    compoundPropertyBean.ElementProfile(i).IupacID = compoundPropertyBean.ElementProfile(i).AtomElementProperties(0).ID
                    accurateMass += compoundPropertyBean.ElementProfile(i).AtomElementProperties(0).ExactMass * compoundPropertyBean.ElementProfile(i).ElementNumber
                Else
                    Console.WriteLine(compoundPropertyBean.ElementProfile(i).ElementName & " is not included in IUPAC reference.")
                    compoundPropertyBean.ElementProfile = Nothing
                    Return
                End If
            Next
            compoundPropertyBean.ExactMass = accurateMass
        End Sub

        Private Sub setAccurateIsotopePropertyInformation(compoundPropertyBean As IsotopeProperty, massFilter As Integer)
            For i = 0 To compoundPropertyBean.ElementProfile.Count - 1
                compoundPropertyBean.ElementProfile(i).IsotopicPeaks = getIsotopeElementProperty(compoundPropertyBean.ElementProfile(i).AtomElementProperties)
                compoundPropertyBean.ElementProfile(i).IsotopicPeaks = getAccurateIsotopeElementProperty(compoundPropertyBean.ElementProfile(i).IsotopicPeaks, compoundPropertyBean.ElementProfile(i).ElementNumber, massFilter)
                compoundPropertyBean.ElementProfile(i).IsotopicPeaks = compoundPropertyBean.ElementProfile(i).IsotopicPeaks.OrderBy(Function(n) n.MassDifferenceFromMonoisotopicIon).ToList()
            Next
        End Sub

        Private Sub setFinalAccurateIsotopeProfile(compoundPropertyBean As IsotopeProperty, massFilter As Integer)
            Dim abundanceElementPropertyBean As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            abundanceElementPropertyBean = compoundPropertyBean.ElementProfile(0).IsotopicPeaks

            For i = 1 To compoundPropertyBean.ElementProfile.Count - 1
                abundanceElementPropertyBean = getAccurateMultiplatedIsotopeElement(abundanceElementPropertyBean, compoundPropertyBean.ElementProfile(i).IsotopicPeaks, massFilter)
            Next

            compoundPropertyBean.IsotopeProfile = abundanceElementPropertyBean.OrderBy(Function(n) n.MassDifferenceFromMonoisotopicIon).ToList()

            For i = 0 To compoundPropertyBean.IsotopeProfile.Count - 1
                compoundPropertyBean.IsotopeProfile(i).MassDifferenceFromMonoisotopicIon += compoundPropertyBean.ExactMass
                compoundPropertyBean.IsotopeProfile(i).RelativeAbundance *= 100
            Next
        End Sub

        Private Sub setNominalIsotopePropertyInformation(compoundPropertyBean As IsotopeProperty, massFilter As Integer)
            For i = 0 To compoundPropertyBean.ElementProfile.Count - 1
                compoundPropertyBean.ElementProfile(i).IsotopicPeaks = getIsotopeElementProperty(compoundPropertyBean.ElementProfile(i).AtomElementProperties)
                compoundPropertyBean.ElementProfile(i).IsotopicPeaks = getNominalIsotopeElementProperty(compoundPropertyBean.ElementProfile(i).IsotopicPeaks, compoundPropertyBean.ElementProfile(i).ElementNumber, massFilter)
                compoundPropertyBean.ElementProfile(i).IsotopicPeaks = compoundPropertyBean.ElementProfile(i).IsotopicPeaks.OrderBy(Function(n) n.MassDifferenceFromMonoisotopicIon).ToList()
            Next
        End Sub

        Private Sub setFinalNominalIsotopeProfile(compoundPropertyBean As IsotopeProperty, massFilter As Integer)
            Dim abundanceElementPropertyBean As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            abundanceElementPropertyBean = compoundPropertyBean.ElementProfile(0).IsotopicPeaks

            For i = 1 To compoundPropertyBean.ElementProfile.Count - 1
                abundanceElementPropertyBean = getNominalMultiplatedisotopeElement(abundanceElementPropertyBean, compoundPropertyBean.ElementProfile(i).IsotopicPeaks, massFilter)
            Next

            compoundPropertyBean.IsotopeProfile = abundanceElementPropertyBean.OrderBy(Function(n) n.MassDifferenceFromMonoisotopicIon).ToList()

            For i = 0 To compoundPropertyBean.IsotopeProfile.Count - 1
                compoundPropertyBean.IsotopeProfile(i).MassDifferenceFromMonoisotopicIon += compoundPropertyBean.ExactMass
                compoundPropertyBean.IsotopeProfile(i).RelativeAbundance *= 100
            Next
        End Sub

        Private Function getIsotopeElementProperty(iupacElementPropertyBeanList As AtomElementProperty()) As List(Of IsotopicPeak)
            Dim isotopeElementPropertyBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim relativeAbundance, massDifference As Double

            For i = 0 To iupacElementPropertyBeanList.Length - 1
                relativeAbundance = iupacElementPropertyBeanList(i).NaturalRelativeAbundance / iupacElementPropertyBeanList(0).NaturalRelativeAbundance
                massDifference = iupacElementPropertyBeanList(i).ExactMass - iupacElementPropertyBeanList(0).ExactMass
                isotopeElementPropertyBeanList.Add(New IsotopicPeak() With {
                    .RelativeAbundance = relativeAbundance,
                    .MassDifferenceFromMonoisotopicIon = massDifference
                })
            Next

            Return isotopeElementPropertyBeanList
        End Function

        Private Function getAccurateIsotopeElementProperty(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            If isotopeElementPropertyBeanList.Count = 1 Then Return isotopeElementPropertyBeanList

            Dim accurateIsotopeElementPropertyBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Select Case isotopeElementPropertyBeanList.Count
                Case 2
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForTwoIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 3
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForThreeIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 4
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForFourIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 5
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForFiveIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 6
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForSixIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 7
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForSevenIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 8
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForEightIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 9
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForNineIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 10
                    accurateIsotopeElementPropertyBeanList = getAllIsotopeElementPropertyForTenIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case Else
            End Select
            Return accurateIsotopeElementPropertyBeanList
        End Function

        Private Function getNominalIsotopeElementProperty(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            If isotopeElementPropertyBeanList.Count = 1 Then Return isotopeElementPropertyBeanList

            Dim nominalIsotopeElementPropertyBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Select Case isotopeElementPropertyBeanList.Count
                Case 2
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForTwoIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 3
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForThreeIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 4
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForFourIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 5
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForFiveIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 6
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForSixIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 7
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForSevenIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 8
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForEightIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 9
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForNineIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case 10
                    nominalIsotopeElementPropertyBeanList = getNominalIsotopeElementPropertyForTenIsotopomerElement(isotopeElementPropertyBeanList, n, filterMass)
                Case Else
            End Select
            Return nominalIsotopeElementPropertyBeanList
        End Function

        Private Function getAllIsotopeElementPropertyForTenIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList7 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList8 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList9 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim g_f_relativeAbundance = isotopeElementPropertyBeanList(7).RelativeAbundance / isotopeElementPropertyBeanList(6).RelativeAbundance
            Dim g_f_massDifference = isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon

            Dim h_g_relativeAbundance = isotopeElementPropertyBeanList(8).RelativeAbundance / isotopeElementPropertyBeanList(7).RelativeAbundance
            Dim h_g_massDifference = isotopeElementPropertyBeanList(8).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon

            Dim i_h_relativeAbundance = isotopeElementPropertyBeanList(9).RelativeAbundance / isotopeElementPropertyBeanList(8).RelativeAbundance
            Dim i_h_massDifference = isotopeElementPropertyBeanList(9).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(8).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double
            Dim relativeAbundance_gf, massDifference_gf As Double
            Dim relativeAbundance_hg, massDifference_hg As Double
            Dim relativeAbundance_ih, massDifference_ih As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = d_c_massDifference * l

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = e_d_massDifference * m

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = f_e_massDifference * o

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList7 = New List(Of IsotopicPeak)()

                                    For p = 0 To o
                                        relativeAbundance_gf = SpecialFunctions.BinomialCoefficient(o, p) * Math.Pow(g_f_relativeAbundance, p)
                                        massDifference_gf = g_f_massDifference * p

                                        If std.Abs(massDifference_gf) > filterMass - 1 Then Exit For

                                        abundanceElementPropertyBeanList8 = New List(Of IsotopicPeak)()

                                        For q = 0 To p
                                            relativeAbundance_hg = SpecialFunctions.BinomialCoefficient(p, q) * Math.Pow(h_g_relativeAbundance, q)
                                            massDifference_hg = h_g_massDifference * q

                                            If std.Abs(massDifference_hg) > filterMass - 1 Then Exit For

                                            abundanceElementPropertyBeanList9 = New List(Of IsotopicPeak)()

                                            For r = 0 To q
                                                relativeAbundance_ih = SpecialFunctions.BinomialCoefficient(q, r) * Math.Pow(i_h_relativeAbundance, r)
                                                massDifference_ih = i_h_massDifference * r

                                                If std.Abs(massDifference_ih) > filterMass - 1 Then Exit For

                                                abundanceElementPropertyBeanList9.Add(New IsotopicPeak() With {
                                                    .RelativeAbundance = relativeAbundance_ih,
                                                    .MassDifferenceFromMonoisotopicIon = massDifference_ih
                                                })
                                            Next

                                            abundanceElementPropertyBeanList9 = getAccurateMultiplatedIsotopeElement(relativeAbundance_hg, massDifference_hg, abundanceElementPropertyBeanList9, filterMass)

                                            abundanceElementPropertyBeanList8 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList8, abundanceElementPropertyBeanList9)
                                        Next

                                        abundanceElementPropertyBeanList8 = getAccurateMultiplatedIsotopeElement(relativeAbundance_gf, massDifference_gf, abundanceElementPropertyBeanList8, filterMass)

                                        abundanceElementPropertyBeanList7 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList7, abundanceElementPropertyBeanList8)
                                    Next


                                    abundanceElementPropertyBeanList7 = getAccurateMultiplatedIsotopeElement(relativeAbundance_fe, massDifference_fe, abundanceElementPropertyBeanList7, filterMass)

                                    abundanceElementPropertyBeanList6 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList6, abundanceElementPropertyBeanList7)
                                Next

                                abundanceElementPropertyBeanList6 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getAccurateMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getAccurateMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForNineIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList7 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList8 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim g_f_relativeAbundance = isotopeElementPropertyBeanList(7).RelativeAbundance / isotopeElementPropertyBeanList(6).RelativeAbundance
            Dim g_f_massDifference = isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon

            Dim h_g_relativeAbundance = isotopeElementPropertyBeanList(8).RelativeAbundance / isotopeElementPropertyBeanList(7).RelativeAbundance
            Dim h_g_massDifference = isotopeElementPropertyBeanList(8).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double
            Dim relativeAbundance_gf, massDifference_gf As Double
            Dim relativeAbundance_hg, massDifference_hg As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = d_c_massDifference * l

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = e_d_massDifference * m

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = f_e_massDifference * o

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList7 = New List(Of IsotopicPeak)()

                                    For p = 0 To o
                                        relativeAbundance_gf = SpecialFunctions.BinomialCoefficient(o, p) * Math.Pow(g_f_relativeAbundance, p)
                                        massDifference_gf = g_f_massDifference * p

                                        If std.Abs(massDifference_gf) > filterMass - 1 Then Exit For

                                        abundanceElementPropertyBeanList8 = New List(Of IsotopicPeak)()

                                        For q = 0 To p
                                            relativeAbundance_hg = SpecialFunctions.BinomialCoefficient(p, q) * Math.Pow(h_g_relativeAbundance, q)
                                            massDifference_hg = h_g_massDifference * q

                                            If std.Abs(massDifference_hg) > filterMass - 1 Then Exit For

                                            abundanceElementPropertyBeanList8.Add(New IsotopicPeak() With {
                                                .RelativeAbundance = relativeAbundance_hg,
                                                .MassDifferenceFromMonoisotopicIon = massDifference_hg
                                            })
                                        Next

                                        abundanceElementPropertyBeanList8 = getAccurateMultiplatedIsotopeElement(relativeAbundance_gf, massDifference_gf, abundanceElementPropertyBeanList8, filterMass)

                                        abundanceElementPropertyBeanList7 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList7, abundanceElementPropertyBeanList8)
                                    Next


                                    abundanceElementPropertyBeanList7 = getAccurateMultiplatedIsotopeElement(relativeAbundance_fe, massDifference_fe, abundanceElementPropertyBeanList7, filterMass)

                                    abundanceElementPropertyBeanList6 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList6, abundanceElementPropertyBeanList7)
                                Next

                                abundanceElementPropertyBeanList6 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getAccurateMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getAccurateMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForEightIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList7 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim g_f_relativeAbundance = isotopeElementPropertyBeanList(7).RelativeAbundance / isotopeElementPropertyBeanList(6).RelativeAbundance
            Dim g_f_massDifference = isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double
            Dim relativeAbundance_gf, massDifference_gf As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = d_c_massDifference * l

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = e_d_massDifference * m

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = f_e_massDifference * o

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList7 = New List(Of IsotopicPeak)()

                                    For p = 0 To o
                                        relativeAbundance_gf = SpecialFunctions.BinomialCoefficient(o, p) * Math.Pow(g_f_relativeAbundance, p)
                                        massDifference_gf = g_f_massDifference * p

                                        If std.Abs(massDifference_gf) > filterMass - 1 Then Exit For

                                        abundanceElementPropertyBeanList7.Add(New IsotopicPeak() With {
                                            .RelativeAbundance = relativeAbundance_gf,
                                            .MassDifferenceFromMonoisotopicIon = massDifference_gf
                                        })
                                    Next

                                    abundanceElementPropertyBeanList7 = getAccurateMultiplatedIsotopeElement(relativeAbundance_fe, massDifference_fe, abundanceElementPropertyBeanList7, filterMass)

                                    abundanceElementPropertyBeanList6 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList6, abundanceElementPropertyBeanList7)
                                Next

                                abundanceElementPropertyBeanList6 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getAccurateMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getAccurateMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForSevenIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = d_c_massDifference * l

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = e_d_massDifference * m

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = f_e_massDifference * o

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList6.Add(New IsotopicPeak() With {
                                        .RelativeAbundance = relativeAbundance_fe,
                                        .MassDifferenceFromMonoisotopicIon = massDifference_fe
                                    })
                                Next

                                abundanceElementPropertyBeanList6 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getAccurateMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getAccurateMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForSixIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = d_c_massDifference * l

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = e_d_massDifference * m

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList5.Add(New IsotopicPeak() With {
                                    .RelativeAbundance = relativeAbundance_ed,
                                    .MassDifferenceFromMonoisotopicIon = massDifference_ed
                                })
                            Next

                            abundanceElementPropertyBeanList5 = getAccurateMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getAccurateMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForFiveIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For


                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = d_c_massDifference * l

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = relativeAbundance_dc,
                                .MassDifferenceFromMonoisotopicIon = massDifference_dc
                            })
                        Next

                        abundanceElementPropertyBeanList4 = getAccurateMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForFourIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = b_a_massDifference * j

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = c_b_massDifference * k

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = relativeAbundance_cb,
                            .MassDifferenceFromMonoisotopicIon = massDifference_cb
                        })
                    Next

                    abundanceElementPropertyBeanList3 = getAccurateMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next
            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForThreeIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()

                For k = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, k) * Math.Pow(b_a_relativeAbundance, k)
                    massDifference_ba = b_a_massDifference * k

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = relativeAbundance_ba,
                        .MassDifferenceFromMonoisotopicIon = massDifference_ba
                    })
                Next

                abundanceElementPropertyBeanList2 = getAccurateMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)
                abundanceElementPropertyBeanList1 = getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAllIsotopeElementPropertyForTwoIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = a_massDifference * i

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = relativeAbundance_a,
                    .MassDifferenceFromMonoisotopicIon = massDifference_a
                })
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getAccurateMultiplatedIsotopeElement(abundanceElementPropertyBeanList1 As List(Of IsotopicPeak), abundanceElementPropertyBeanList2 As List(Of IsotopicPeak), filterMass As Integer) As List(Of IsotopicPeak)
            Dim multiplatedAbundanceElementBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim relativeAbundance, massDifference As Double
            For i = 0 To abundanceElementPropertyBeanList1.Count - 1
                For j = 0 To abundanceElementPropertyBeanList2.Count - 1
                    relativeAbundance = abundanceElementPropertyBeanList1(i).RelativeAbundance * abundanceElementPropertyBeanList2(j).RelativeAbundance
                    massDifference = abundanceElementPropertyBeanList1(i).MassDifferenceFromMonoisotopicIon + abundanceElementPropertyBeanList2(j).MassDifferenceFromMonoisotopicIon
                    If std.Abs(massDifference) <= filterMass Then multiplatedAbundanceElementBeanList.Add(New IsotopicPeak() With {
.RelativeAbundance = relativeAbundance,
.MassDifferenceFromMonoisotopicIon = massDifference
})
                Next
            Next
            Return multiplatedAbundanceElementBeanList
        End Function

        Private Function getAccurateMultiplatedIsotopeElement(relativeAbund As Double, massDiff As Double, abundanceElementPropertyBeanList As List(Of IsotopicPeak), filterMass As Integer) As List(Of IsotopicPeak)
            Dim multiplatedAbundanceElementBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim relativeAbundance, massDifference As Double
            For i = 0 To abundanceElementPropertyBeanList.Count - 1
                relativeAbundance = relativeAbund * abundanceElementPropertyBeanList(i).RelativeAbundance
                massDifference = massDiff + abundanceElementPropertyBeanList(i).MassDifferenceFromMonoisotopicIon
                If std.Abs(massDifference) <= filterMass Then multiplatedAbundanceElementBeanList.Add(New IsotopicPeak() With {
.RelativeAbundance = relativeAbundance,
.MassDifferenceFromMonoisotopicIon = massDifference
})
            Next
            Return multiplatedAbundanceElementBeanList
        End Function

        Private Function getAccurateMargedIsotopeElement(abundanceElementPropertyBeanList1 As List(Of IsotopicPeak), abundanceElementPropertyBeanList2 As List(Of IsotopicPeak)) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To abundanceElementPropertyBeanList1.Count - 1
                abundanceElementPropertyBeanList.Add(abundanceElementPropertyBeanList1(i))
            Next

            For i = 0 To abundanceElementPropertyBeanList2.Count - 1
                abundanceElementPropertyBeanList.Add(abundanceElementPropertyBeanList2(i))
            Next

            Return abundanceElementPropertyBeanList
        End Function

        Private Function getNominalIsotopeElementPropertyForTenIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList7 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList8 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList9 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim g_f_relativeAbundance = isotopeElementPropertyBeanList(7).RelativeAbundance / isotopeElementPropertyBeanList(6).RelativeAbundance
            Dim g_f_massDifference = isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon

            Dim h_g_relativeAbundance = isotopeElementPropertyBeanList(8).RelativeAbundance / isotopeElementPropertyBeanList(7).RelativeAbundance
            Dim h_g_massDifference = isotopeElementPropertyBeanList(8).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon

            Dim i_h_relativeAbundance = isotopeElementPropertyBeanList(9).RelativeAbundance / isotopeElementPropertyBeanList(8).RelativeAbundance
            Dim i_h_massDifference = isotopeElementPropertyBeanList(9).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(8).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double
            Dim relativeAbundance_gf, massDifference_gf As Double
            Dim relativeAbundance_hg, massDifference_hg As Double
            Dim relativeAbundance_ih, massDifference_ih As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()
                        For l = 0 To filterMass - 1
                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = 0,
                                .MassDifferenceFromMonoisotopicIon = 0
                            })
                        Next

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = Math.Round(d_c_massDifference * l, 0)

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()
                            For m = 0 To filterMass - 1
                                abundanceElementPropertyBeanList5.Add(New IsotopicPeak() With {
                                    .RelativeAbundance = 0,
                                    .MassDifferenceFromMonoisotopicIon = 0
                                })
                            Next

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = Math.Round(e_d_massDifference * m, 0)

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()
                                For o = 0 To filterMass - 1
                                    abundanceElementPropertyBeanList6.Add(New IsotopicPeak() With {
                                        .RelativeAbundance = 0,
                                        .MassDifferenceFromMonoisotopicIon = 0
                                    })
                                Next

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = Math.Round(f_e_massDifference * o, 0)

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList7 = New List(Of IsotopicPeak)()
                                    For p = 0 To filterMass - 1
                                        abundanceElementPropertyBeanList7.Add(New IsotopicPeak() With {
                                            .RelativeAbundance = 0,
                                            .MassDifferenceFromMonoisotopicIon = 0
                                        })
                                    Next

                                    For p = 0 To o
                                        relativeAbundance_gf = SpecialFunctions.BinomialCoefficient(o, p) * Math.Pow(g_f_relativeAbundance, p)
                                        massDifference_gf = Math.Round(g_f_massDifference * p, 0)

                                        If std.Abs(massDifference_gf) > filterMass - 1 Then Exit For

                                        abundanceElementPropertyBeanList8 = New List(Of IsotopicPeak)()
                                        For q = 0 To filterMass - 1
                                            abundanceElementPropertyBeanList8.Add(New IsotopicPeak() With {
                                                .RelativeAbundance = 0,
                                                .MassDifferenceFromMonoisotopicIon = 0
                                            })
                                        Next

                                        For q = 0 To p
                                            relativeAbundance_hg = SpecialFunctions.BinomialCoefficient(p, q) * Math.Pow(h_g_relativeAbundance, q)
                                            massDifference_hg = Math.Round(h_g_massDifference * q, 0)

                                            If std.Abs(massDifference_hg) > filterMass - 1 Then Exit For

                                            abundanceElementPropertyBeanList9 = New List(Of IsotopicPeak)()
                                            For r = 0 To filterMass - 1
                                                abundanceElementPropertyBeanList9.Add(New IsotopicPeak() With {
                                                    .RelativeAbundance = 0,
                                                    .MassDifferenceFromMonoisotopicIon = 0
                                                })
                                            Next

                                            For r = 0 To q
                                                relativeAbundance_ih = SpecialFunctions.BinomialCoefficient(q, r) * Math.Pow(i_h_relativeAbundance, r)
                                                massDifference_ih = Math.Round(i_h_massDifference * r, 0)

                                                If std.Abs(massDifference_ih) > filterMass - 1 Then Exit For

                                                abundanceElementPropertyBeanList9(massDifference_ih).RelativeAbundance += relativeAbundance_ih
                                                abundanceElementPropertyBeanList9(massDifference_ih).MassDifferenceFromMonoisotopicIon = massDifference_ih
                                            Next

                                            abundanceElementPropertyBeanList9 = getNominalMultiplatedIsotopeElement(relativeAbundance_hg, massDifference_hg, abundanceElementPropertyBeanList9, filterMass)

                                            abundanceElementPropertyBeanList8 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList8, abundanceElementPropertyBeanList9)
                                        Next

                                        abundanceElementPropertyBeanList8 = getNominalMultiplatedIsotopeElement(relativeAbundance_gf, massDifference_gf, abundanceElementPropertyBeanList8, filterMass)

                                        abundanceElementPropertyBeanList7 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList7, abundanceElementPropertyBeanList8)
                                    Next


                                    abundanceElementPropertyBeanList7 = getNominalMultiplatedIsotopeElement(relativeAbundance_fe, massDifference_fe, abundanceElementPropertyBeanList7, filterMass)

                                    abundanceElementPropertyBeanList6 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList6, abundanceElementPropertyBeanList7)
                                Next

                                abundanceElementPropertyBeanList6 = getNominalMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getNominalMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getNominalMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForNineIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList7 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList8 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim g_f_relativeAbundance = isotopeElementPropertyBeanList(7).RelativeAbundance / isotopeElementPropertyBeanList(6).RelativeAbundance
            Dim g_f_massDifference = isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon

            Dim h_g_relativeAbundance = isotopeElementPropertyBeanList(8).RelativeAbundance / isotopeElementPropertyBeanList(7).RelativeAbundance
            Dim h_g_massDifference = isotopeElementPropertyBeanList(8).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double
            Dim relativeAbundance_gf, massDifference_gf As Double
            Dim relativeAbundance_hg, massDifference_hg As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()
                        For l = 0 To filterMass - 1
                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = 0,
                                .MassDifferenceFromMonoisotopicIon = 0
                            })
                        Next

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = Math.Round(d_c_massDifference * l, 0)

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()
                            For m = 0 To filterMass - 1
                                abundanceElementPropertyBeanList5.Add(New IsotopicPeak() With {
                                    .RelativeAbundance = 0,
                                    .MassDifferenceFromMonoisotopicIon = 0
                                })
                            Next

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = Math.Round(e_d_massDifference * m, 0)

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()
                                For o = 0 To filterMass - 1
                                    abundanceElementPropertyBeanList6.Add(New IsotopicPeak() With {
                                        .RelativeAbundance = 0,
                                        .MassDifferenceFromMonoisotopicIon = 0
                                    })
                                Next

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = Math.Round(f_e_massDifference * o, 0)

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList7 = New List(Of IsotopicPeak)()
                                    For p = 0 To filterMass - 1
                                        abundanceElementPropertyBeanList7.Add(New IsotopicPeak() With {
                                            .RelativeAbundance = 0,
                                            .MassDifferenceFromMonoisotopicIon = 0
                                        })
                                    Next

                                    For p = 0 To o
                                        relativeAbundance_gf = SpecialFunctions.BinomialCoefficient(o, p) * Math.Pow(g_f_relativeAbundance, p)
                                        massDifference_gf = Math.Round(g_f_massDifference * p, 0)

                                        If std.Abs(massDifference_gf) > filterMass - 1 Then Exit For

                                        abundanceElementPropertyBeanList8 = New List(Of IsotopicPeak)()
                                        For q = 0 To filterMass - 1
                                            abundanceElementPropertyBeanList8.Add(New IsotopicPeak() With {
                                                .RelativeAbundance = 0,
                                                .MassDifferenceFromMonoisotopicIon = 0
                                            })
                                        Next

                                        For q = 0 To p
                                            relativeAbundance_hg = SpecialFunctions.BinomialCoefficient(p, q) * Math.Pow(h_g_relativeAbundance, q)
                                            massDifference_hg = Math.Round(h_g_massDifference * q, 0)

                                            If std.Abs(massDifference_hg) > filterMass - 1 Then Exit For

                                            abundanceElementPropertyBeanList8(massDifference_hg).RelativeAbundance += relativeAbundance_hg
                                            abundanceElementPropertyBeanList8(massDifference_hg).MassDifferenceFromMonoisotopicIon = massDifference_hg
                                        Next

                                        abundanceElementPropertyBeanList8 = getNominalMultiplatedIsotopeElement(relativeAbundance_gf, massDifference_gf, abundanceElementPropertyBeanList8, filterMass)

                                        abundanceElementPropertyBeanList7 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList7, abundanceElementPropertyBeanList8)
                                    Next


                                    abundanceElementPropertyBeanList7 = getNominalMultiplatedIsotopeElement(relativeAbundance_fe, massDifference_fe, abundanceElementPropertyBeanList7, filterMass)

                                    abundanceElementPropertyBeanList6 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList6, abundanceElementPropertyBeanList7)
                                Next

                                abundanceElementPropertyBeanList6 = getNominalMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getNominalMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getNominalMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForEightIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList7 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim g_f_relativeAbundance = isotopeElementPropertyBeanList(7).RelativeAbundance / isotopeElementPropertyBeanList(6).RelativeAbundance
            Dim g_f_massDifference = isotopeElementPropertyBeanList(7).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double
            Dim relativeAbundance_gf, massDifference_gf As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For


                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For


                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()
                        For l = 0 To filterMass - 1
                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = 0,
                                .MassDifferenceFromMonoisotopicIon = 0
                            })
                        Next

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = Math.Round(d_c_massDifference * l, 0)

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For


                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()
                            For m = 0 To filterMass - 1
                                abundanceElementPropertyBeanList5.Add(New IsotopicPeak() With {
                                    .RelativeAbundance = 0,
                                    .MassDifferenceFromMonoisotopicIon = 0
                                })
                            Next

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = Math.Round(e_d_massDifference * m, 0)

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()
                                For o = 0 To filterMass - 1
                                    abundanceElementPropertyBeanList6.Add(New IsotopicPeak() With {
                                        .RelativeAbundance = 0,
                                        .MassDifferenceFromMonoisotopicIon = 0
                                    })
                                Next

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = Math.Round(f_e_massDifference * o, 0)

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList7 = New List(Of IsotopicPeak)()
                                    For p = 0 To filterMass - 1
                                        abundanceElementPropertyBeanList7.Add(New IsotopicPeak() With {
                                            .RelativeAbundance = 0,
                                            .MassDifferenceFromMonoisotopicIon = 0
                                        })
                                    Next

                                    For p = 0 To o
                                        relativeAbundance_gf = SpecialFunctions.BinomialCoefficient(o, p) * Math.Pow(g_f_relativeAbundance, p)
                                        massDifference_gf = Math.Round(g_f_massDifference * p, 0)

                                        If std.Abs(massDifference_gf) > filterMass - 1 Then Exit For

                                        abundanceElementPropertyBeanList7(massDifference_gf).RelativeAbundance += relativeAbundance_gf
                                        abundanceElementPropertyBeanList7(massDifference_gf).MassDifferenceFromMonoisotopicIon = massDifference_gf
                                    Next

                                    abundanceElementPropertyBeanList7 = getNominalMultiplatedIsotopeElement(relativeAbundance_fe, massDifference_fe, abundanceElementPropertyBeanList7, filterMass)

                                    abundanceElementPropertyBeanList6 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList6, abundanceElementPropertyBeanList7)
                                Next

                                abundanceElementPropertyBeanList6 = getNominalMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getNominalMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getNominalMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForSevenIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList6 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim f_e_relativeAbundance = isotopeElementPropertyBeanList(6).RelativeAbundance / isotopeElementPropertyBeanList(5).RelativeAbundance
            Dim f_e_massDifference = isotopeElementPropertyBeanList(6).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double
            Dim relativeAbundance_fe, massDifference_fe As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For


                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For


                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For


                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()
                        For l = 0 To filterMass - 1
                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = 0,
                                .MassDifferenceFromMonoisotopicIon = 0
                            })
                        Next

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = Math.Round(d_c_massDifference * l, 0)

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For


                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()
                            For m = 0 To filterMass - 1
                                abundanceElementPropertyBeanList5.Add(New IsotopicPeak() With {
                                    .RelativeAbundance = 0,
                                    .MassDifferenceFromMonoisotopicIon = 0
                                })
                            Next

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = Math.Round(e_d_massDifference * m, 0)

                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For


                                abundanceElementPropertyBeanList6 = New List(Of IsotopicPeak)()
                                For o = 0 To filterMass - 1
                                    abundanceElementPropertyBeanList6.Add(New IsotopicPeak() With {
                                        .RelativeAbundance = 0,
                                        .MassDifferenceFromMonoisotopicIon = 0
                                    })
                                Next

                                For o = 0 To m
                                    relativeAbundance_fe = SpecialFunctions.BinomialCoefficient(m, o) * Math.Pow(f_e_relativeAbundance, o)
                                    massDifference_fe = Math.Round(f_e_massDifference * o, 0)

                                    If std.Abs(massDifference_fe) > filterMass - 1 Then Exit For

                                    abundanceElementPropertyBeanList6(massDifference_fe).RelativeAbundance += relativeAbundance_fe
                                    abundanceElementPropertyBeanList6(massDifference_fe).MassDifferenceFromMonoisotopicIon = massDifference_fe
                                Next

                                abundanceElementPropertyBeanList6 = getNominalMultiplatedIsotopeElement(relativeAbundance_ed, massDifference_ed, abundanceElementPropertyBeanList6, filterMass)

                                abundanceElementPropertyBeanList5 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList5, abundanceElementPropertyBeanList6)
                            Next

                            abundanceElementPropertyBeanList5 = getNominalMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getNominalMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForSixIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList5 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim e_d_relativeAbundance = isotopeElementPropertyBeanList(5).RelativeAbundance / isotopeElementPropertyBeanList(4).RelativeAbundance
            Dim e_d_massDifference = isotopeElementPropertyBeanList(5).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double
            Dim relativeAbundance_ed, massDifference_ed As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For


                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For


                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()
                        For l = 0 To filterMass - 1
                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = 0,
                                .MassDifferenceFromMonoisotopicIon = 0
                            })
                        Next

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = Math.Round(d_c_massDifference * l, 0)

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For


                            abundanceElementPropertyBeanList5 = New List(Of IsotopicPeak)()
                            For m = 0 To filterMass - 1
                                abundanceElementPropertyBeanList5.Add(New IsotopicPeak() With {
                                    .RelativeAbundance = 0,
                                    .MassDifferenceFromMonoisotopicIon = 0
                                })
                            Next

                            For m = 0 To l
                                relativeAbundance_ed = SpecialFunctions.BinomialCoefficient(l, m) * Math.Pow(e_d_relativeAbundance, m)
                                massDifference_ed = Math.Round(e_d_massDifference * m, 0)



                                If std.Abs(massDifference_ed) > filterMass - 1 Then Exit For

                                abundanceElementPropertyBeanList5(massDifference_ed).RelativeAbundance += relativeAbundance_ed
                                abundanceElementPropertyBeanList5(massDifference_ed).MassDifferenceFromMonoisotopicIon = massDifference_ed

                            Next

                            abundanceElementPropertyBeanList5 = getNominalMultiplatedIsotopeElement(relativeAbundance_dc, massDifference_dc, abundanceElementPropertyBeanList5, filterMass)

                            abundanceElementPropertyBeanList4 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList4, abundanceElementPropertyBeanList5)
                        Next

                        abundanceElementPropertyBeanList4 = getNominalMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForFiveIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList4 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim d_c_relativeAbundance = isotopeElementPropertyBeanList(4).RelativeAbundance / isotopeElementPropertyBeanList(3).RelativeAbundance
            Dim d_c_massDifference = isotopeElementPropertyBeanList(4).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double
            Dim relativeAbundance_dc, massDifference_dc As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next


                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For


                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList4 = New List(Of IsotopicPeak)()
                        For l = 0 To filterMass - 1
                            abundanceElementPropertyBeanList4.Add(New IsotopicPeak() With {
                                .RelativeAbundance = 0,
                                .MassDifferenceFromMonoisotopicIon = 0
                            })
                        Next

                        For l = 0 To k
                            relativeAbundance_dc = SpecialFunctions.BinomialCoefficient(k, l) * Math.Pow(d_c_relativeAbundance, l)
                            massDifference_dc = Math.Round(d_c_massDifference * l, 0)

                            If std.Abs(massDifference_dc) > filterMass - 1 Then Exit For

                            abundanceElementPropertyBeanList4(massDifference_dc).RelativeAbundance += relativeAbundance_dc
                            abundanceElementPropertyBeanList4(massDifference_dc).MassDifferenceFromMonoisotopicIon = massDifference_dc

                        Next

                        abundanceElementPropertyBeanList4 = getNominalMultiplatedIsotopeElement(relativeAbundance_cb, massDifference_cb, abundanceElementPropertyBeanList4, filterMass)

                        abundanceElementPropertyBeanList3 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList3, abundanceElementPropertyBeanList4)
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForFourIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList3 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim c_b_relativeAbundance = isotopeElementPropertyBeanList(3).RelativeAbundance / isotopeElementPropertyBeanList(2).RelativeAbundance
            Dim c_b_massDifference = isotopeElementPropertyBeanList(3).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double
            Dim relativeAbundance_cb, massDifference_cb As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList3 = New List(Of IsotopicPeak)()
                    For k = 0 To filterMass - 1
                        abundanceElementPropertyBeanList3.Add(New IsotopicPeak() With {
                            .RelativeAbundance = 0,
                            .MassDifferenceFromMonoisotopicIon = 0
                        })
                    Next

                    For k = 0 To j
                        relativeAbundance_cb = SpecialFunctions.BinomialCoefficient(j, k) * Math.Pow(c_b_relativeAbundance, k)
                        massDifference_cb = Math.Round(c_b_massDifference * k, 0)

                        If std.Abs(massDifference_cb) > filterMass - 1 Then Exit For

                        abundanceElementPropertyBeanList3(massDifference_cb).RelativeAbundance += relativeAbundance_cb
                        abundanceElementPropertyBeanList3(massDifference_cb).MassDifferenceFromMonoisotopicIon = massDifference_cb
                    Next

                    abundanceElementPropertyBeanList3 = getNominalMultiplatedIsotopeElement(relativeAbundance_ba, massDifference_ba, abundanceElementPropertyBeanList3, filterMass)

                    abundanceElementPropertyBeanList2 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList2, abundanceElementPropertyBeanList3)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)

                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next
            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForThreeIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            Dim abundanceElementPropertyBeanList2 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()

            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim b_a_relativeAbundance = isotopeElementPropertyBeanList(2).RelativeAbundance / isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim b_a_massDifference = isotopeElementPropertyBeanList(2).MassDifferenceFromMonoisotopicIon - isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double
            Dim relativeAbundance_ba, massDifference_ba As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList2 = New List(Of IsotopicPeak)()
                For j = 0 To filterMass - 1
                    abundanceElementPropertyBeanList2.Add(New IsotopicPeak() With {
                        .RelativeAbundance = 0,
                        .MassDifferenceFromMonoisotopicIon = 0
                    })
                Next

                For j = 0 To i
                    relativeAbundance_ba = SpecialFunctions.BinomialCoefficient(i, j) * Math.Pow(b_a_relativeAbundance, j)
                    massDifference_ba = Math.Round(b_a_massDifference * j, 0)

                    If std.Abs(massDifference_ba) > filterMass - 1 Then Exit For

                    abundanceElementPropertyBeanList2(massDifference_ba).RelativeAbundance += relativeAbundance_ba
                    abundanceElementPropertyBeanList2(massDifference_ba).MassDifferenceFromMonoisotopicIon = CInt(massDifference_ba)
                Next

                abundanceElementPropertyBeanList2 = getNominalMultiplatedIsotopeElement(relativeAbundance_a, massDifference_a, abundanceElementPropertyBeanList2, filterMass)
                abundanceElementPropertyBeanList1 = getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1, abundanceElementPropertyBeanList2)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalIsotopeElementPropertyForTwoIsotopomerElement(isotopeElementPropertyBeanList As List(Of IsotopicPeak), n As Integer, filterMass As Integer) As List(Of IsotopicPeak)
            Dim abundanceElementPropertyBeanList1 As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                abundanceElementPropertyBeanList1.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim a_relativeAbundance = isotopeElementPropertyBeanList(1).RelativeAbundance
            Dim a_massDifference = isotopeElementPropertyBeanList(1).MassDifferenceFromMonoisotopicIon

            Dim relativeAbundance_a, massDifference_a As Double

            For i = 0 To n
                relativeAbundance_a = SpecialFunctions.BinomialCoefficient(n, i) * Math.Pow(a_relativeAbundance, i)
                massDifference_a = Math.Round(a_massDifference * i, 0)

                If std.Abs(massDifference_a) > filterMass - 1 Then Exit For

                abundanceElementPropertyBeanList1(massDifference_a).RelativeAbundance += relativeAbundance_a
                abundanceElementPropertyBeanList1(massDifference_a).MassDifferenceFromMonoisotopicIon = CInt(massDifference_a)
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalMargedIsotopeElement(abundanceElementPropertyBeanList1 As List(Of IsotopicPeak), abundanceElementPropertyBeanList2 As List(Of IsotopicPeak)) As List(Of IsotopicPeak)
            For i = 0 To abundanceElementPropertyBeanList1.Count - 1
                abundanceElementPropertyBeanList1(i).RelativeAbundance += abundanceElementPropertyBeanList2(i).RelativeAbundance
                abundanceElementPropertyBeanList1(i).MassDifferenceFromMonoisotopicIon = i
            Next

            Return abundanceElementPropertyBeanList1
        End Function

        Private Function getNominalMultiplatedisotopeElement(abundanceElementPropertyBeanList1 As List(Of IsotopicPeak), abundanceElementPropertyBeanList2 As List(Of IsotopicPeak), filterMass As Integer) As List(Of IsotopicPeak)
            Dim massDiff As Integer
            Dim multipliedIsotopeElementList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                multipliedIsotopeElementList.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = i
                })
            Next

            For i = 0 To abundanceElementPropertyBeanList1.Count - 1
                For j = 0 To abundanceElementPropertyBeanList2.Count - 1
                    massDiff = CInt(abundanceElementPropertyBeanList1(i).MassDifferenceFromMonoisotopicIon + abundanceElementPropertyBeanList2(j).MassDifferenceFromMonoisotopicIon)
                    If massDiff <= filterMass - 1 Then
                        multipliedIsotopeElementList(massDiff).RelativeAbundance += abundanceElementPropertyBeanList1(i).RelativeAbundance * abundanceElementPropertyBeanList2(j).RelativeAbundance
                    End If
                Next
            Next

            Return multipliedIsotopeElementList
        End Function

        Private Function getNominalMultiplatedIsotopeElement(relativeAbund As Double, massDiff As Double, abundanceElementPropertyBeanList As List(Of IsotopicPeak), filterMass As Integer) As List(Of IsotopicPeak)
            Dim multiplatedAbundanceElementBeanList As List(Of IsotopicPeak) = New List(Of IsotopicPeak)()
            For i = 0 To filterMass - 1
                multiplatedAbundanceElementBeanList.Add(New IsotopicPeak() With {
                    .RelativeAbundance = 0,
                    .MassDifferenceFromMonoisotopicIon = 0
                })
            Next

            Dim relativeAbundance, massDifference As Double
            For i = 0 To abundanceElementPropertyBeanList.Count - 1
                relativeAbundance = relativeAbund * abundanceElementPropertyBeanList(i).RelativeAbundance
                massDifference = Math.Round(massDiff + abundanceElementPropertyBeanList(i).MassDifferenceFromMonoisotopicIon, 0)

                If std.Abs(massDifference) <= filterMass - 1 Then
                    multiplatedAbundanceElementBeanList(massDifference).RelativeAbundance += relativeAbundance
                    multiplatedAbundanceElementBeanList(massDifference).MassDifferenceFromMonoisotopicIon = CInt(massDifference)
                End If
            Next
            Return multiplatedAbundanceElementBeanList
        End Function

    End Module
End Namespace
