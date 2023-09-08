Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports CompMs.Common.Algorithm.PeakPick
Imports CompMs.Common.Components
Imports CompMs.Common.DataObj
Imports CompMs.Common.DataObj.Ion
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.DataObj
Imports std = System.Math
Imports System.Collections.Generic
Imports System.Linq


Public NotInheritable Class FragmentAssigner
    Private Sub New()
    End Sub
    Private Shared electron As Double = 0.0005485799

    ''' <summary>
    ''' peaklist should be centroid and refined. For peaklist refining, use GetRefinedPeaklist.
    ''' </summary>
    ''' <paramname="peaklist"></param>
    ''' <paramname="formula"></param>
    ''' <paramname="analysisParam"></param>
    Public Shared Function FastFragmnetAssigner(ByVal peaklist As List(Of SpectrumPeak), ByVal productIonDB As List(Of ProductIon), ByVal formula As Formula, ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType, ByVal adductIon As AdductIon) As List(Of ProductIon)
        Dim productIons = New List(Of ProductIon)()
        Dim eMass = electron
        If adductIon.IonMode = IonModes.Negative Then eMass = -1.0 * electron

        For Each peak In peaklist
            If Not Equals(peak.Comment, "M") Then Continue For

            Dim mass = peak.mz + eMass
            Dim minDiff = Double.MaxValue
            Dim massTol = ms2Tol
            If massTolType = MassToleranceType.Ppm Then massTol = PPMmethod.ConvertPpmToMassAccuracy(mass, ms2Tol)
            Dim minId = -1

            'for precursor annotation
            If std.Abs(mass - formula.Mass - adductIon.AdductIonAccurateMass) < massTol Then
                Dim nFormula = ConvertFormulaAdductPairToPrecursorAdduct(formula, adductIon)
                productIons.Add(New ProductIon() With {
                    .Formula = nFormula,
                    .Mass = peak.mz,
                    .MassDiff = formula.Mass + adductIon.AdductIonAccurateMass - mass,
                    .Intensity = peak.intensity
                })
                Continue For
            End If

            'library search
            Dim fragmentFormulas = getFormulaCandidatesbyLibrarySearch(formula, adductIon.IonMode, peak.mz, massTol, productIonDB)
            If fragmentFormulas Is Nothing OrElse fragmentFormulas.Count = 0 Then fragmentFormulas = getValenceCheckedFragmentFormulaList(formula, adductIon.IonMode, peak.mz, massTol)

            For i = 0 To fragmentFormulas.Count - 1
                If minDiff > std.Abs(mass - fragmentFormulas(i).Mass) Then
                    minId = i
                    minDiff = std.Abs(mass - fragmentFormulas(i).Mass)
                End If
            Next
            If minId >= 0 Then productIons.Add(New ProductIon() With {
.Formula = fragmentFormulas(minId),
.Mass = peak.Mass,
.MassDiff = fragmentFormulas(minId).Mass - mass,
.Intensity = peak.intensity
})
        Next

        For Each ion In productIons
            Dim startIndex = FragmentAssigner.getStartIndex(ion.Mass, 0.1, productIonDB)
            For i = startIndex To productIonDB.Count - 1
                Dim ionQuery = productIonDB(i)
                If ionQuery.IonMode <> adductIon.IonMode Then Continue For
                If ionQuery.Formula.Mass > ion.Mass + 0.1 Then Exit For

                If FragmentAssigner.isFormulaComposition(ion.Formula, ionQuery.Formula) Then
                    ion.CandidateInChIKeys = ionQuery.CandidateInChIKeys
                    ion.CandidateOntologies = ionQuery.CandidateOntologies
                    ion.Frequency = ionQuery.Frequency
                    Exit For
                End If
            Next
        Next

        Return productIons
    End Function

    Private Shared Function getFormulaCandidatesbyLibrarySearch(ByVal formula As Formula, ByVal ionMode As IonMode, ByVal mz As Double, ByVal massTol As Double, ByVal productIonDB As List(Of ProductIon)) As List(Of Formula)
        Dim candidates = New List(Of Formula)()
        Dim startIndex = getStartIndex(mz, massTol, productIonDB)
        For i = startIndex To productIonDB.Count - 1
            Dim ionQuery = productIonDB(i)
            If ionQuery.IonMode <> ionMode Then Continue For
            If ionQuery.Formula.Mass < mz - massTol Then Continue For
            If ionQuery.Formula.Mass > mz + massTol Then Exit For

            If isFormulaComposition(ionQuery.Formula, formula) Then
                candidates.Add(ionQuery.Formula)
            End If
        Next

        Return candidates
    End Function

    Private Shared Function getStartIndex(ByVal mass As Double, ByVal tol As Double, ByVal productIonDB As List(Of ProductIon)) As Integer
        If productIonDB Is Nothing OrElse productIonDB.Count = 0 Then Return 0
        Dim targetMass = mass - tol
        Dim startIndex = 0, endIndex = productIonDB.Count - 1
        Dim counter = 0

        While counter < 5
            If productIonDB(startIndex).Mass <= targetMass AndAlso targetMass < productIonDB((startIndex + endIndex) / 2).Mass Then
                endIndex = (startIndex + endIndex) / 2
            ElseIf productIonDB((startIndex + endIndex) / 2).Mass <= targetMass AndAlso targetMass < productIonDB(endIndex).Mass Then
                startIndex = (startIndex + endIndex) / 2
            End If
            counter += 1
        End While
        Return startIndex
    End Function

    ''' <summary>
    ''' The neutralLosslist can be made by GetNeutralLossList after using GetRefinedPeaklist.
    ''' </summary>
    ''' <paramname="neutralLosslist"></param>
    ''' <paramname="neutralLossDB"></param>
    ''' <paramname="originalFormula"></param>
    ''' <paramname="analysisParam"></param>
    ''' <paramname="adductIon"></param>
    ''' <returns></returns>
    Public Shared Function FastNeutralLossAssigner(ByVal neutralLosslist As List(Of NeutralLoss), ByVal neutralLossDB As List(Of NeutralLoss), ByVal originalFormula As Formula, ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType, ByVal adductIon As AdductIon) As List(Of NeutralLoss)
        Dim neutralLossResult = New List(Of NeutralLoss)()
        'double eMass = electron; if (adductIon.IonMode == IonMode.Negative) eMass = -1.0 * electron; 

        For Each nloss In neutralLosslist
            Dim mass = nloss.MassLoss
            Dim minDiff = Double.MaxValue
            Dim massTol = ms2Tol
            If massTolType = MassToleranceType.Ppm Then massTol = ConvertPpmToMassAccuracy(mass, ms2Tol)
            Dim minID = -1

            Dim startIndex = getStartIndex(mass, 0.1, neutralLossDB)

            For i = startIndex To neutralLossDB.Count - 1
                If mass - massTol > neutralLossDB(i).Formula.Mass Then Continue For
                If adductIon.IonMode <> neutralLossDB(i).Iontype Then Continue For
                If mass + massTol < neutralLossDB(i).Formula.Mass Then Exit For

                If isFormulaComposition(neutralLossDB(i).Formula, originalFormula) Then
                    If minDiff > std.Abs(mass - neutralLossDB(i).Formula.Mass) Then
                        minDiff = std.Abs(mass - neutralLossDB(i).Formula.Mass)
                        minID = i
                    End If
                End If
            Next

            If minID >= 0 Then
                neutralLossResult.Add(New NeutralLoss() With {
.Comment = neutralLossDB(minID).Comment,
.Formula = neutralLossDB(minID).Formula,
.Iontype = neutralLossDB(minID).Iontype,
.CandidateInChIKeys = neutralLossDB(minID).CandidateInChIKeys,
.CandidateOntologies = neutralLossDB(minID).CandidateOntologies,
.Frequency = neutralLossDB(minID).Frequency,
.MassLoss = nloss.MassLoss,
.PrecursorMz = nloss.PrecursorMz,
.ProductMz = nloss.ProductMz,
.PrecursorIntensity = nloss.PrecursorIntensity,
.ProductIntensity = nloss.ProductIntensity,
.MassError = neutralLossDB(minID).Formula.Mass - mass,
.Smiles = neutralLossDB(minID).Smiles
})
            End If
        Next
        Return neutralLossResult.OrderByDescending(Function(n) Math.Max(n.PrecursorIntensity, n.ProductIntensity)).ToList()
    End Function

    Private Shared Function getStartIndex(ByVal mass As Double, ByVal tol As Double, ByVal neutralLossDB As List(Of NeutralLoss)) As Integer
        If neutralLossDB Is Nothing OrElse neutralLossDB.Count = 0 Then Return 0
        Dim targetMass = mass - tol
        Dim startIndex = 0, endIndex = neutralLossDB.Count - 1
        Dim counter = 0

        While counter < 5
            If neutralLossDB(startIndex).Formula.Mass <= targetMass AndAlso targetMass < neutralLossDB((startIndex + endIndex) / 2).Formula.Mass Then
                endIndex = (startIndex + endIndex) / 2
            ElseIf neutralLossDB((startIndex + endIndex) / 2).Formula.Mass <= targetMass AndAlso targetMass < neutralLossDB(endIndex).Formula.Mass Then
                startIndex = (startIndex + endIndex) / 2
            End If
            counter += 1
        End While
        Return startIndex
    End Function

    ''' <summary>
    ''' peaklist should be centroid and refined. For peaklist refining, use GetRefinedPeaklist.
    ''' </summary>
    ''' <paramname="peaklist"></param>
    ''' <returns></returns>
    Public Shared Function GetNeutralLossList(ByVal peaklist As List(Of SpectrumPeak), ByVal precurosrMz As Double, ByVal masstol As Double) As List(Of NeutralLoss)
        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of NeutralLoss)()

        Dim neutralLosslist = New List(Of NeutralLoss)()
        Dim monoIsotopicPeaklist = New List(Of SpectrumPeak)()
        Dim maxNeutralLoss = 1000

        For Each peak In peaklist
            If Equals(peak.Comment, "M") Then monoIsotopicPeaklist.Add(peak)
        Next
        monoIsotopicPeaklist = monoIsotopicPeaklist.OrderByDescending(Function(n) n.Mass).ToList()

        Dim highestMz = monoIsotopicPeaklist(0).Mass
        If std.Abs(highestMz - precurosrMz) > masstol Then monoIsotopicPeaklist.Insert(0, New SpectrumPeak() With {
.Mass = precurosrMz,
.intensity = 1,
.Comment = "Insearted precursor"
})

        For i = 0 To monoIsotopicPeaklist.Count - 1
            For j = i + 1 To monoIsotopicPeaklist.Count - 1
                If j > monoIsotopicPeaklist.Count - 1 Then Exit For
                If monoIsotopicPeaklist(i).Mass - monoIsotopicPeaklist(j).Mass < 12 - masstol Then Continue For

                neutralLosslist.Add(New NeutralLoss() With {
.MassLoss = monoIsotopicPeaklist(i).Mass - monoIsotopicPeaklist(j).Mass,
.PrecursorMz = monoIsotopicPeaklist(i).Mass,
.ProductMz = monoIsotopicPeaklist(j).Mass,
.PrecursorIntensity = monoIsotopicPeaklist(i).intensity,
.ProductIntensity = monoIsotopicPeaklist(j).intensity
})
            Next
        Next

        If maxNeutralLoss < neutralLosslist.Count Then
            neutralLosslist = neutralLosslist.OrderByDescending(Function(n) n.ProductIntensity).ToList()
            Dim filteredList = New List(Of NeutralLoss)()
            For Each peak In neutralLosslist
                filteredList.Add(peak)
                If filteredList.Count > maxNeutralLoss Then Return filteredList.OrderByDescending(Function(n) n.PrecursorMz).ToList()
            Next
        End If

        Return neutralLosslist
    End Function

    ''' <summary>
    ''' peaklist should be centroid.
    ''' </summary>
    ''' <paramname="peaklist"></param>
    ''' <paramname="analysisParam"></param>
    ''' <paramname="precursorMz"></param>
    ''' <returns></returns>
    Public Shared Function GetRefinedPeaklist(ByVal peaklist As List(Of SpectrumPeak), ByVal relativeAbundanceCutOff As Double, ByVal absoluteAbundanceCutOff As Double, ByVal precursorMz As Double, ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType, ByVal Optional peakListMax As Integer = 1000, ByVal Optional isRemoveIsotopes As Boolean = False, ByVal Optional removeAfterPrecursor As Boolean = True) As List(Of SpectrumPeak)




        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim maxIntensity = getMaxIntensity(peaklist)
        Dim refinedPeaklist = New List(Of SpectrumPeak)()

        For Each peak In peaklist
            If removeAfterPrecursor AndAlso peak.Mass > precursorMz + 0.01 Then Exit For
            If peak.intensity < absoluteAbundanceCutOff Then Continue For
            If peak.intensity >= maxIntensity * relativeAbundanceCutOff * 0.01 Then
                refinedPeaklist.Add(New SpectrumPeak() With {
                    .Mass = peak.Mass,
                    .intensity = peak.intensity,
                    .Comment = String.Empty
                })
            End If
        Next

        refinedPeaklist = isotopicPeakAssignmnet(refinedPeaklist, ms2Tol, massTolType)

        If isRemoveIsotopes = False Then
            If refinedPeaklist.Count > peakListMax Then
                refinedPeaklist = refinedPeaklist.OrderByDescending(Function(n) n.intensity).ToList()
                Dim filteredList = New List(Of SpectrumPeak)()
                For Each peak In refinedPeaklist
                    filteredList.Add(peak)
                    If filteredList.Count > peakListMax Then Return filteredList.OrderBy(Function(n) n.Mass).ToList()
                Next
            Else
                Return refinedPeaklist
            End If
        Else
            Dim isotopeRemovedRefinedPeaklist = New List(Of SpectrumPeak)()

            For Each peak In refinedPeaklist
                If Equals(peak.Comment, "M") Then isotopeRemovedRefinedPeaklist.Add(peak)
            Next

            If isotopeRemovedRefinedPeaklist.Count > peakListMax Then
                isotopeRemovedRefinedPeaklist = isotopeRemovedRefinedPeaklist.OrderByDescending(Function(n) n.intensity).ToList()
                Dim filteredList = New List(Of SpectrumPeak)()
                For Each peak In isotopeRemovedRefinedPeaklist
                    filteredList.Add(peak)
                    If filteredList.Count > peakListMax Then
                        Return filteredList.OrderBy(Function(n) n.Mass).ToList()
                    End If
                Next
            Else
                Return isotopeRemovedRefinedPeaklist
            End If
        End If
        Return refinedPeaklist
    End Function

    Public Shared Function GetRefinedPeaklist(ByVal peaklist As List(Of SpectrumPeak), ByVal precursorMz As Double) As List(Of SpectrumPeak)
        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim refinedPeaklist = New List(Of SpectrumPeak)()

        For Each peak In peaklist
            If peak.Mass > precursorMz + 0.01 Then Exit For
            refinedPeaklist.Add(New SpectrumPeak() With {
                .Mass = peak.Mass,
                .intensity = peak.intensity,
                .Comment = peak.Comment
            })
        Next
        Return refinedPeaklist
    End Function

    ''' <summary>
    ''' peaklist should be centroid.
    ''' </summary>
    ''' <paramname="peaklist"></param>
    ''' <paramname="analysisParam"></param>
    ''' <paramname="precursorMz"></param>
    ''' <returns></returns>
    Public Shared Function GetRefinedPeaklist(ByVal peaklist As List(Of SpectrumPeak), ByVal relativeAbundanceCutOff As Double, ByVal absoluteAbundanceCutOff As Double, ByVal precursorMz As Double, ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType) As List(Of SpectrumPeak)
        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim maxIntensity = getMaxIntensity(peaklist)
        Dim refinedPeaklist = New List(Of SpectrumPeak)()

        For Each peak In peaklist
            If peak.Mass > precursorMz + 0.01 Then Exit For
            If peak.intensity >= maxIntensity * relativeAbundanceCutOff * 0.01 AndAlso peak.intensity > absoluteAbundanceCutOff Then
                refinedPeaklist.Add(New SpectrumPeak() With {
                    .Mass = peak.Mass,
                    .intensity = peak.intensity,
                    .Comment = String.Empty
                })
            End If
        Next

        refinedPeaklist = isotopicPeakAssignmnet(refinedPeaklist, ms2Tol, massTolType)
        Return refinedPeaklist
    End Function

    ''' <summary>
    ''' get centroid spectrum: judge if the type is profile or centroid
    ''' </summary>
    ''' <paramname="rawData"></param>
    ''' <returns></returns>
    Public Shared Function GetCentroidMsMsSpectrum(ByVal rawData As RawData) As List(Of SpectrumPeak)
        Dim ms2Peaklist As List(Of SpectrumPeak)

        If rawData.SpectrumType = MSDataType.Centroid Then
            ms2Peaklist = rawData.Ms2Spectrum
        Else
            ms2Peaklist = SpectralCentroiding.Centroid(rawData.Ms2Spectrum)
        End If

        If ms2Peaklist Is Nothing Then Return New List(Of SpectrumPeak)()

        Return ms2Peaklist
    End Function

    ''' <summary>
    ''' get centroid spectrum: judge if the type is profile or centroid
    ''' </summary>
    ''' <paramname="rawData"></param>
    ''' <returns></returns>
    Public Shared Function GetCentroidMs1Spectrum(ByVal rawData As RawData) As List(Of SpectrumPeak)
        Dim ms1Peaklist As List(Of SpectrumPeak)

        If rawData.SpectrumType = MSDataType.Centroid Then
            ms1Peaklist = rawData.Ms1Spectrum
        Else
            ms1Peaklist = SpectralCentroiding.Centroid(rawData.Ms1Spectrum)
        End If

        If ms1Peaklist Is Nothing Then Return New List(Of SpectrumPeak)()

        Return ms1Peaklist
    End Function

    ''' <summary>
    ''' get centroid spectrum: judge if the type is profile or centroid
    ''' </summary>
    ''' <paramname="peaklist"></param>
    ''' <paramname="dataType"></param>
    ''' <returns></returns>
    Public Shared Function GetCentroidSpectrum(ByVal peaklist As List(Of SpectrumPeak), ByVal dataType As MSDataType, ByVal threshold As Double) As List(Of SpectrumPeak)
        Dim cPeaklist As List(Of SpectrumPeak)

        If dataType = MSDataType.Centroid Then
            cPeaklist = peaklist
        Else
            cPeaklist = SpectralCentroiding.Centroid(peaklist, threshold)
        End If

        If cPeaklist Is Nothing Then Return New List(Of SpectrumPeak)()

        Return cPeaklist
    End Function

    'private static double hMass = 1.00782503207;
    'private static double cMass = 12;
    'private static double nMass = 14.0030740048;
    'private static double oMass = 15.99491461956;
    'private static double fMass = 18.99840322000;
    'private static double siMass = 27.97692653250;
    'private static double pMass = 30.97376163;
    'private static double sMass = 31.972071;
    'private static double clMass = 34.96885268000;
    'private static double brMass = 78.91833710000;
    'private static double iMass = 126.90447300000;

    Private Shared Function getValenceCheckedFragmentFormulaList(ByVal formula As Formula, ByVal ionMode As IonMode, ByVal mass As Double, ByVal massTol As Double) As List(Of Formula)
        Dim fragmentFormulas = New List(Of Formula)()
        Dim hydrogen = 1
        If ionMode = ionMode.Negative Then hydrogen = -1

        Dim maxHmass = hMass * (formula.Hnum + hydrogen)
        Dim maxCHmass = maxHmass + cMass * formula.Cnum
        Dim maxNCHmass = maxCHmass + nMass * formula.Nnum
        Dim maxONCHmass = maxNCHmass + oMass * formula.Onum
        Dim maxFONCHmass = maxONCHmass + fMass * formula.Fnum
        Dim maxSiFONCHmass = maxFONCHmass + siMass * formula.Sinum
        Dim maxPSiFONCHmass = maxSiFONCHmass + pMass * formula.Pnum
        Dim maxSPSiFONCHmass = maxPSiFONCHmass + sMass * formula.Snum
        Dim maxClSPSiFONCHmass = maxSPSiFONCHmass + clMass * formula.Clnum
        Dim maxBrClSPSiFONCHmass = maxClSPSiFONCHmass + brMass * formula.Brnum

        For inum = 0 To formula.Inum
            If inum * iMass + maxBrClSPSiFONCHmass < mass - massTol Then Continue For
            If inum * iMass > mass + massTol Then Exit For
            Dim uImass = inum * iMass

            For brnum = 0 To formula.Brnum
                If uImass + brnum * brMass + maxClSPSiFONCHmass < mass - massTol Then Continue For
                If uImass + brnum * brMass > mass + massTol Then Exit For
                Dim uBrmass = uImass + brnum * brMass

                For clnum = 0 To formula.Clnum
                    If uBrmass + clnum * clMass + maxSPSiFONCHmass < mass - massTol Then Continue For
                    If uBrmass + clnum * clMass > mass + massTol Then Exit For
                    Dim uClmass = uBrmass + clnum * clMass

                    For snum = 0 To formula.Snum
                        If uClmass + snum * sMass + maxPSiFONCHmass < mass - massTol Then Continue For
                        If uClmass + snum * sMass > mass + massTol Then Exit For
                        Dim uSmass = uClmass + snum * sMass

                        For pnum = 0 To formula.Pnum
                            If uSmass + pnum * pMass + maxSiFONCHmass < mass - massTol Then Continue For
                            If uSmass + pnum * pMass > mass + massTol Then Exit For
                            Dim uPmass = uSmass + pnum * pMass

                            For sinum = 0 To formula.Sinum
                                If uPmass + sinum * siMass + maxFONCHmass < mass - massTol Then Continue For
                                If uPmass + sinum * siMass > mass + massTol Then Exit For
                                Dim uSimass = uPmass + sinum * siMass

                                For fnum = 0 To formula.Fnum
                                    If uSimass + fnum * fMass + maxONCHmass < mass - massTol Then Continue For
                                    If uSimass + fnum * fMass > mass + massTol Then Exit For
                                    Dim uFmass = uSimass + fnum * fMass

                                    For onum = 0 To formula.Onum
                                        If uFmass + onum * oMass + maxNCHmass < mass - massTol Then Continue For
                                        If uFmass + onum * oMass > mass + massTol Then Exit For
                                        Dim uOmass = uFmass + onum * oMass

                                        For nnum = 0 To formula.Nnum
                                            If uOmass + nnum * nMass + maxCHmass < mass - massTol Then Continue For
                                            If uOmass + nnum * nMass > mass + massTol Then Exit For
                                            Dim uNmass = uOmass + nnum * nMass

                                            For cnum = 0 To formula.Cnum
                                                If uNmass + cnum * cMass + maxHmass < mass - massTol Then Continue For
                                                If uNmass + cnum * cMass > mass + massTol Then Exit For
                                                Dim uCmass = uNmass + cnum * cMass

                                                For hnum = 0 To formula.Hnum + hydrogen
                                                    If uCmass + hnum * hMass < mass - massTol Then Continue For
                                                    If uCmass + hnum * hMass > mass + massTol Then Exit For

                                                    Dim fragmentFormula = New Formula(cnum, hnum, nnum, onum, pnum, snum, fnum, clnum, brnum, inum, sinum)
                                                    If SevenGoldenRulesCheck.ValenceCheckByHydrogenShift(fragmentFormula) Then fragmentFormulas.Add(fragmentFormula)
                                                Next
                                            Next
                                        Next
                                    Next
                                Next
                            Next
                        Next
                    Next
                Next
            Next
        Next

        fragmentFormulas = fragmentFormulas.OrderBy(Function(n) n.Mass).ToList()
        Return fragmentFormulas
    End Function

    Private Shared Function isotopicPeakAssignmnet(ByVal peaklist As List(Of SpectrumPeak), ByVal massTol As Double, ByVal massTolType As MassToleranceType) As List(Of SpectrumPeak)
        Dim c13_c12_Ratio = 0.010815728

        For i = 0 To peaklist.Count - 1
            Dim mass = peaklist(i).Mass
            Dim m1Intensity = peaklist(i).intensity

            Dim comment = peaklist(i).Comment
            Dim maxCarbon = mass Mod 12
            Dim m1TheoreticalCarbonInt = m1Intensity * c13_c12_Ratio * maxCarbon
            Dim m2TheoreticalCarbonInt = m1Intensity * maxCarbon * (maxCarbon - 1) * 0.5 * Math.Pow(c13_c12_Ratio, 2)
            Dim ms2Tol = massTol
            If massTolType = MassToleranceType.Ppm Then ms2Tol = ConvertPpmToMassAccuracy(mass, massTol)

            If Not Equals(comment, String.Empty) Then
                Continue For
            Else
                peaklist(i).Comment = "M"
                If i = peaklist.Count - 1 Then Exit For
                Dim m2Intensity = 0.0
                For j = i To peaklist.Count - 1
                    If mass + C13_C12_Plus_C13_C12 + ms2Tol <= peaklist(j).Mass Then Exit For

                    'if (mass + MassDiffDictionary.C13_C12 - ms2Tol < peaklist[j].Mz && peaklist[j].Mz < mass + MassDiffDictionary.C13_C12 + ms2Tol && 
                    '    m1TheoreticalCarbonInt * 5.0 > peaklist[j].Intensity) peaklist[j].Comment = "M+1";
                    If mass + c13_c12 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + c13_c12 + ms2Tol AndAlso m1Intensity > peaklist(j).intensity Then
                        peaklist(j).Comment = "M+1"
                        m2Intensity += peaklist(j).intensity
                    End If

                    If mass + n15_n14 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + n15_n14 + ms2Tol AndAlso m1TheoreticalCarbonInt > peaklist(j).intensity Then
                        peaklist(j).Comment = "M+1"
                        m2Intensity += peaklist(j).intensity
                    End If

                    'if (mass + MassDiffDictionary.C13_C12_Plus_C13_C12 - ms2Tol < peaklist[j].Mz && peaklist[j].Mz < mass + MassDiffDictionary.C13_C12_Plus_C13_C12 + ms2Tol && 
                    '    m2TheoreticalCarbonInt * 1.3 > peaklist[j].Intensity) peaklist[j].Comment = "M+2";
                    If m2Intensity > 0.0 AndAlso mass + C13_C12_Plus_C13_C12 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + C13_C12_Plus_C13_C12 + ms2Tol AndAlso m2Intensity > peaklist(j).intensity Then

                        peaklist(j).Comment = "M+2"
                    End If

                    If mass + s34_s32 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + s34_s32 + ms2Tol AndAlso m1Intensity * 0.3 > peaklist(j).intensity Then peaklist(j).Comment = "M+2"


                    If mass + o18_o16 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + o18_o16 + ms2Tol AndAlso m1Intensity * 0.1 > peaklist(j).intensity Then peaklist(j).Comment = "M+2"

                    'if (mass + MassDiffDictionary.Cl37_Cl35 - ms2Tol < peaklist[j].Mz &&
                    '    peaklist[j].Mz < mass + MassDiffDictionary.Cl37_Cl35 + ms2Tol && 
                    '    m1Intensity * 0.8 > peaklist[j].Intensity)
                    '    peaklist[j].Comment = "M+2";
                    'if (mass + MassDiffDictionary.Br81_Br79 - ms2Tol < peaklist[j].Mz && 
                    '    peaklist[j].Mz < mass + MassDiffDictionary.Br81_Br79 + ms2Tol && 
                    '    m1Intensity * 1.3 > peaklist[j].Intensity)
                    '    peaklist[j].Comment = "M+2";
                Next
            End If
        Next
        Return peaklist
    End Function


    Public Shared Function GetAnnotatedIon(ByVal peaklist As List(Of SpectrumPeak), ByVal mainAdduct As AdductIon, ByVal referenceAdductTypeList As List(Of AdductIon), ByVal precursorMz As Double, ByVal massTol As Double, ByVal massTolType As MassToleranceType) As List(Of AnnotatedIon)
        Dim annotations = New List(Of AnnotatedIon)()
        For Each peak In peaklist
            annotations.Add(New AnnotatedIon() With {
                .AccurateMass = peak.Mass
            })
        Next
        AnnotateIsotopes(peaklist, annotations, massTol, massTolType)
        AnnotateAdducts(peaklist, annotations, mainAdduct, referenceAdductTypeList, precursorMz, massTol, massTolType)
        Return annotations
    End Function

    Public Shared Sub AnnotateIsotopes(ByVal peaklist As List(Of SpectrumPeak), ByVal annotations As List(Of AnnotatedIon), ByVal massTol As Double, ByVal massTolType As MassToleranceType)
        Dim c13_c12_Ratio = 0.010815728
        For i = 0 To peaklist.Count - 1
            Dim mass = peaklist(i).Mass
            Dim m1Intensity = peaklist(i).intensity

            Dim maxCarbon = mass Mod 12
            Dim m1TheoreticalCarbonInt = m1Intensity * c13_c12_Ratio * maxCarbon
            Dim m2TheoreticalCarbonInt = m1Intensity * maxCarbon * (maxCarbon - 1) * 0.5 * Math.Pow(c13_c12_Ratio, 2)
            Dim ms2Tol = massTol
            If massTolType = MassToleranceType.Ppm Then ms2Tol = ConvertPpmToMassAccuracy(mass, massTol)
            If annotations(i).PeakType = AnnotatedIon.AnnotationType.Product Then
                If i = peaklist.Count - 1 Then Exit For
                Dim m2Intensity = 0.0
                Dim m2mass = 0.0
                For j = i To peaklist.Count - 1
                    If mass + C13_C12_Plus_C13_C12 + ms2Tol <= peaklist(j).Mass Then Exit For
                    If m2mass > 0 AndAlso m2mass + C13_C12_Plus_C13_C12 + ms2Tol <= peaklist(j).Mass Then m2Intensity = 0

                    If mass + c13_c12 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + c13_c12 + ms2Tol AndAlso m1Intensity > peaklist(j).intensity Then
                        annotations(j).SetIsotope(mass, peaklist(j).intensity, m1Intensity, "C-13", 1)
                        m2Intensity = peaklist(j).intensity
                        m2mass = peaklist(j).Mass

                    ElseIf mass + n15_n14 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + n15_n14 + ms2Tol AndAlso m1Intensity > peaklist(j).intensity Then
                        annotations(j).SetIsotope(mass, peaklist(j).intensity, m1Intensity, "N-14", 1)
                        m2Intensity = peaklist(j).intensity
                        m2mass = peaklist(j).Mass
                    End If

                    If m2Intensity > 0.0 AndAlso mass + C13_C12_Plus_C13_C12 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + C13_C12_Plus_C13_C12 + ms2Tol AndAlso m2Intensity > peaklist(j).intensity Then
                        annotations(j).SetIsotope(mass, peaklist(j).intensity, m1Intensity, "2C-13", 2)

                    ElseIf mass + s34_s32 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + s34_s32 + ms2Tol AndAlso m1Intensity * 0.3 > peaklist(j).intensity Then
                        annotations(j).SetIsotope(mass, peaklist(j).intensity, m1Intensity, "S34", 2)

                    ElseIf mass + o18_o16 - ms2Tol < peaklist(j).Mass AndAlso peaklist(j).Mass < mass + o18_o16 + ms2Tol AndAlso m1Intensity * 0.1 > peaklist(j).intensity Then
                        annotations(j).SetIsotope(mass, peaklist(j).intensity, m1Intensity, "S34", 2)
                    End If
                Next
            End If
        Next
    End Sub


    Public Shared Sub AnnotateAdducts(ByVal peaklist As List(Of SpectrumPeak), ByVal annotations As List(Of AnnotatedIon), ByVal mainAdduct As AdductIon, ByVal referenceAdductTypeList As List(Of AdductIon), ByVal precursorMz As Double, ByVal massTol As Double, ByVal massTolType As MassToleranceType)

        For i = 0 To peaklist.Count - 1
            Dim peak = peaklist(i)
            If annotations(i).PeakType <> AnnotatedIon.AnnotationType.Product Then Continue For
            Dim centralExactMass = mainAdduct.ConvertToExactMass(peak.Mass)
            Dim ppm = massTol
            If massTolType <> MassToleranceType.Ppm Then ppm = PpmCalculator(200, 200 + massTol)

            Dim massTol2 = ConvertPpmToMassAccuracy(precursorMz, ppm)

            Dim referenceAdductIons = New List(Of AnnotatedIon)()
            For Each targetAdduct In referenceAdductTypeList
                If Equals(mainAdduct.AdductIonName, targetAdduct.AdductIonName) Then Continue For
                Dim targetMz = targetAdduct.ConvertToMz(centralExactMass)
                referenceAdductIons.Add(New AnnotatedIon() With {
                    .AccurateMass = targetMz,
                    .AdductIon = targetAdduct
                })
            Next

            For j = 0 To peaklist.Count - 1
                If j = i Then Continue For
                Dim targetPeak = peaklist(j)
                Dim targetAnnotation = annotations(j)
                If targetAnnotation.PeakType <> AnnotatedIon.AnnotationType.Product Then Continue For

                For Each targetAdduct In referenceAdductIons
                    If std.Abs(peak.Mass - precursorMz) > massTol2 Then
                        If targetAdduct.AdductIon.AdductIonXmer > 1 Then Continue For
                    End If

                    Dim adductTol = ConvertPpmToMassAccuracy(targetPeak.Mass, ppm)

                    If std.Abs(targetPeak.Mass - targetAdduct.AccurateMass) < adductTol Then
                        Dim searchedAdduct = targetAdduct.AdductIon

                        targetAnnotation.LinkedAccurateMass = peak.Mass
                        targetAnnotation.AdductIon = targetAdduct.AdductIon
                        targetAnnotation.PeakType = AnnotatedIon.AnnotationType.Adduct
                        Exit For
                    End If
                Next
            Next
        Next
    End Sub

    Private Shared Function getMaxIntensity(ByVal peaklist As List(Of SpectrumPeak)) As Double
        Dim maxInt = Double.MinValue
        For Each peak In peaklist
            If peak.intensity > maxInt Then maxInt = peak.intensity
        Next
        Return maxInt
    End Function

    Private Shared Function isFormulaComposition(ByVal nlFormula As Formula, ByVal originFormula As Formula) As Boolean
        If nlFormula.Cnum <= originFormula.Cnum AndAlso nlFormula.Hnum <= originFormula.Hnum AndAlso nlFormula.Nnum <= originFormula.Nnum AndAlso nlFormula.Onum <= originFormula.Onum AndAlso nlFormula.Pnum <= originFormula.Pnum AndAlso nlFormula.Snum <= originFormula.Snum AndAlso nlFormula.Fnum <= originFormula.Fnum AndAlso nlFormula.Clnum <= originFormula.Clnum AndAlso nlFormula.Brnum <= originFormula.Brnum AndAlso nlFormula.Inum <= originFormula.Inum AndAlso nlFormula.Sinum <= originFormula.Sinum Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
