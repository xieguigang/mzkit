Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports MassDiffDictionary = BioNovoGene.BioDeep.Chemoinformatics.Formula.ElementsExactMass
Public Class IsotopeTemp
    Public Property WeightNumber As Integer
    Public Property Mz As Double
    Public Property MzClBr As Double
    Public Property Intensity As Double
    Public Property PeakID As Integer
End Class

Public NotInheritable Class SpectrumHandler

    Public Shared Function GetCombinedSpectrum(ByVal peaks1 As List(Of SpectrumPeak), ByVal peaks2 As List(Of SpectrumPeak), ByVal bin As Double) As List(Of SpectrumPeak)

        Dim peaks = New List(Of SpectrumPeak)()
        Dim range2Peaks = New Dictionary(Of Integer, List(Of SpectrumPeak))()

        For Each peak In peaks1
            Dim mass = peak.Mass
            Dim massframe = CInt(mass / bin)
            If range2Peaks.ContainsKey(massframe) Then
                range2Peaks(massframe).Add(peak)
            Else
                range2Peaks(massframe) = New List(Of SpectrumPeak)() From {
                    peak
                }
            End If
        Next

        For Each peak In peaks2
            Dim mass = peak.Mass
            Dim massframe = CInt(mass / bin)
            If range2Peaks.ContainsKey(massframe) Then
                range2Peaks(massframe).Add(peak)
            Else
                range2Peaks(massframe) = New List(Of SpectrumPeak)() From {
                    peak
                }
            End If
        Next

        For Each pair In range2Peaks
            Dim maxMass As Double = pair.Value.OrderByDescending(Function(p) p.Intensity).First.Mass
            Dim sumIntensity = pair.Value.Sum(Function(n) n.Intensity) * 0.5
            peaks.Add(New SpectrumPeak(maxMass, sumIntensity))
        Next

        Return peaks
    End Function

    Public Shared Function GetNormalizedPeaks(ByVal spectrum As List(Of SpectrumPeak), ByVal powFactor As Double, ByVal maxValue As Double) As List(Of SpectrumPeak)
        If spectrum.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim maxIntensity = Math.Pow(spectrum.Max(Function(n) n.Intensity), powFactor)
        Return spectrum.[Select](Function(n) New SpectrumPeak With {
            .Mass = n.Mass,
            .Intensity = Math.Pow(n.Intensity, powFactor) / maxIntensity * maxValue
        }).ToList()
    End Function

    Public Shared Function GetBinnedSpectrum(ByVal spectrum As List(Of SpectrumPeak), ByVal Optional delta As Double = 100, ByVal Optional maxPeaks As Integer = 12) As List(Of SpectrumPeak)

        Dim peaks = New List(Of SpectrumPeak)()
        Dim range2Peaks = New Dictionary(Of Integer, List(Of SpectrumPeak))()

        For Each peak In spectrum
            Dim mass = peak.Mass
            Dim massframe = CInt(mass / delta)
            If range2Peaks.ContainsKey(massframe) Then
                range2Peaks(massframe).Add(peak)
            Else
                range2Peaks(massframe) = New List(Of SpectrumPeak)() From {
                    peak
                }
            End If
        Next

        For Each pair In range2Peaks
            Dim counter = 1
            For Each peak In pair.Value.OrderByDescending(Function(n) n.Intensity)
                If counter > maxPeaks Then Exit For
                peaks.Add(peak)
                counter += 1
            Next
        Next
        Return peaks
    End Function

    Public Shared Function GetBinnedSpectrum(ByVal spectrum As List(Of SpectrumPeak), ByVal bin As Double) As List(Of SpectrumPeak)
        Dim peaks = New List(Of SpectrumPeak)()
        Dim range2Peaks = New Dictionary(Of Integer, List(Of SpectrumPeak))()

        For Each peak In spectrum
            Dim mass = peak.Mass
            Dim massframe = CInt(mass / bin)
            If range2Peaks.ContainsKey(massframe) Then
                range2Peaks(massframe).Add(peak)
            Else
                range2Peaks(massframe) = New List(Of SpectrumPeak)() From {
                    peak
                }
            End If
        Next

        For Each pair In range2Peaks
            Dim maxMass = pair.Value.OrderByDescending(Function(n) n.Intensity).First.Mass
            Dim sumIntensity = pair.Value.Sum(Function(n) n.Intensity)
            peaks.Add(New SpectrumPeak(maxMass, sumIntensity))
        Next
        Return peaks
    End Function

    Public Shared Function GetNormalizedPeak4SpectralEntropyCalc(ByVal peaklist As List(Of SpectrumPeak), ByVal precursorMz As Double, ByVal Optional ms2Tol As Double = 0.05, ByVal Optional relativeAbundanceCutOff As Double = 0.1, ByVal Optional absoluteAbundanceCutOff As Double = 3, ByVal Optional minMz As Double = 0, ByVal Optional maxMz As Double = 100000) As List(Of SpectrumPeak)
        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim maxIntensity = peaklist.Max(Function(n) n.Intensity)
        Dim refinedPeaklist = New List(Of SpectrumPeak)()

        For Each peak In peaklist
            If peak.Mass < minMz Then Continue For
            If peak.Mass > maxMz Then Continue For
            If peak.Mass > precursorMz + ms2Tol Then Continue For
            If peak.Intensity < absoluteAbundanceCutOff Then Continue For
            If peak.Intensity >= maxIntensity * relativeAbundanceCutOff * 0.01 Then
                refinedPeaklist.Add(peak)
            End If
        Next
        Dim sumIntensity = refinedPeaklist.Sum(Function(n) n.Intensity)
        Return refinedPeaklist.[Select](Function(n) New SpectrumPeak() With {
            .Mass = n.Mass,
            .Intensity = n.Intensity / sumIntensity
        }).ToList()
    End Function

    Public Shared Function GetNormalizedPeak4SpectralEntropySimilarityCalc(ByVal peaklist As List(Of SpectrumPeak), ByVal precursorMz As Double, ByVal Optional ms2Tol As Double = 0.05, ByVal Optional relativeAbundanceCutOff As Double = 0.1, ByVal Optional absoluteAbundanceCutOff As Double = 3, ByVal Optional minMz As Double = 0, ByVal Optional maxMz As Double = 100000) As List(Of SpectrumPeak)
        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim maxIntensity = peaklist.Max(Function(n) n.Intensity)
        Dim refinedPeaklist = New List(Of SpectrumPeak)()

        For Each peak In peaklist
            If peak.Mass < minMz Then Continue For
            If peak.Mass > maxMz Then Continue For
            If peak.Mass > precursorMz + ms2Tol Then Continue For
            If peak.Intensity < absoluteAbundanceCutOff Then Continue For
            If peak.Intensity >= maxIntensity * relativeAbundanceCutOff * 0.01 Then
                refinedPeaklist.Add(peak)
            End If
        Next

        Dim entropy = MsScanMatching.GetSpectralEntropy(refinedPeaklist)
        If entropy < 3 Then
            For Each peak In refinedPeaklist
                peak.Intensity = Math.Pow(peak.Intensity, 0.25 + entropy * 0.25)
            Next
        End If
        Dim sumIntensity = refinedPeaklist.Sum(Function(n) n.Intensity)
        Return refinedPeaklist.[Select](Function(n) New SpectrumPeak() With {
            .Mass = n.Mass,
            .Intensity = n.Intensity / sumIntensity
        }).ToList()
    End Function

    Public Shared Function GetRefinedPeaklist(ByVal peaklist As List(Of SpectrumPeak), ByVal relativeAbundanceCutOff As Double, ByVal absoluteAbundanceCutOff As Double, ByVal minMz As Double, ByVal maxMz As Double, ByVal precursorMz As Double, ByVal ms2Tol As Double, ByVal massTolType As MassToleranceType, ByVal precursorCharge As Integer, ByVal Optional isBrClConsideredForIsotopes As Boolean = False, ByVal Optional isRemoveIsotopes As Boolean = False, ByVal Optional removeAfterPrecursor As Boolean = True) As List(Of SpectrumPeak)
        If peaklist Is Nothing OrElse peaklist.Count = 0 Then Return New List(Of SpectrumPeak)()
        Dim maxIntensity = peaklist.Max(Function(n) n.Intensity)
        Dim refinedPeaklist = New List(Of SpectrumPeak)()

        For Each peak In peaklist
            If peak.Mass < minMz Then Continue For
            If peak.Mass > maxMz Then Continue For
            If removeAfterPrecursor AndAlso peak.Mass > precursorMz + ms2Tol Then Continue For
            If peak.Intensity < absoluteAbundanceCutOff Then Continue For
            If peak.Intensity >= maxIntensity * relativeAbundanceCutOff * 0.01 Then
                refinedPeaklist.Add(New SpectrumPeak() With {
                    .Mass = peak.Mass,
                    .Intensity = peak.Intensity,
                    .Comment = String.Empty
                })
            End If
        Next
        If isRemoveIsotopes Then
            EstimateIsotopes(refinedPeaklist, ms2Tol, isBrClConsideredForIsotopes, precursorCharge)
            Return refinedPeaklist.Where(Function(n) n.IsotopeWeightNumber = 0).ToList()
        Else
            Return refinedPeaklist
        End If
    End Function

    ''' <summary>
    ''' peak list must be sorted by m/z (ordering)
    ''' peak should be initialized by new Peak() { Mz = spec[0], Intensity = spec[1], Charge = 1, IsotopeFrag = false  }
    ''' </summary>
    Public Shared Sub EstimateIsotopes(ByVal peaks As List(Of SpectrumPeak), ByVal mztolerance As Double, ByVal Optional isBrClConsideredForIsotopes As Boolean = False, ByVal Optional maxChargeNumber As Integer = 0)

        Dim c13_c12Diff = C13_C12  '1.003355F;
        Dim br81_br79 = MassDiffDictionary.Br81_Br79 '1.9979535; also to be used for S34_S32 (1.9957959), Cl37_Cl35 (1.99704991)
        Dim tolerance = mztolerance
        For i = 0 To peaks.Count - 1
            Dim peak = peaks(i)
            peak.PeakID = i
            If peak.IsotopeWeightNumber >= 0 Then Continue For

            peak.IsotopeWeightNumber = 0
            peak.IsotopeParentPeakID = i

            ' charge state checking at M + 1
            Dim predChargeNumber = 1
            For j = i + 1 To peaks.Count - 1
                Dim isotopePeak = peaks(j)
                If isotopePeak.Mass > peak.Mass + c13_c12Diff + tolerance Then Exit For
                If isotopePeak.IsotopeWeightNumber >= 0 Then Continue For

                For k = maxChargeNumber To 1 Step -1
                    Dim predIsotopeMass = peak.Mass + c13_c12Diff / k
                    Dim diff = Math.Abs(predIsotopeMass - isotopePeak.Mass)
                    If diff < tolerance Then
                        predChargeNumber = k
                        If k <= 3 Then
                            Exit For
                        ElseIf k = 4 OrElse k = 5 Then
                            Dim predNextIsotopeMass = peak.Mass + c13_c12Diff / (k - 1)
                            Dim nextDiff = Math.Abs(predNextIsotopeMass - isotopePeak.Mass)
                            If diff > nextDiff Then predChargeNumber = k - 1
                            Exit For
                        ElseIf k >= 6 Then
                            Dim predNextIsotopeMass = peak.Mass + c13_c12Diff / (k - 1)
                            Dim nextDiff = Math.Abs(predNextIsotopeMass - isotopePeak.Mass)
                            If diff > nextDiff Then
                                predChargeNumber = k - 1
                                diff = nextDiff

                                predNextIsotopeMass = peak.Mass + c13_c12Diff / (k - 2)
                                nextDiff = Math.Abs(predNextIsotopeMass - isotopePeak.Mass)

                                If diff > nextDiff Then
                                    predChargeNumber = k - 2
                                    diff = nextDiff
                                End If
                            End If
                            Exit For
                        End If
                    End If
                Next
                If predChargeNumber <> 1 Then Exit For
            Next
            peak.Charge = predChargeNumber

            ' isotope grouping till M + 8
            Dim maxTraceNumber = 15
            Dim isotopeTemps = New IsotopeTemp(maxTraceNumber + 1 - 1) {}
            isotopeTemps(0) = New IsotopeTemp() With {
                .WeightNumber = 0,
                .Mz = peak.Mass,
                .Intensity = peak.Intensity,
                .PeakID = i,
                .MzClBr = peak.Mass
            }

            For j = 1 To isotopeTemps.Length - 1
                isotopeTemps(j) = New IsotopeTemp() With {
                    .WeightNumber = j,
                    .Mz = peak.Mass + j * c13_c12Diff / predChargeNumber,
                    .MzClBr = If(j Mod 2 = 0, peak.Mass + j * c13_c12Diff / predChargeNumber, peak.Mass + j * br81_br79 * 0.5 / predChargeNumber),
                    .Intensity = 0,
                    .PeakID = -1
                }
            Next

            Dim reminderIndex = i + 1
            Dim isFinished = False
            Dim mzFocused = peak.Mass
            For j = 1 To maxTraceNumber
                Dim predIsotopicMass = mzFocused + c13_c12Diff / predChargeNumber
                Dim predClBrIsotopicMass = mzFocused + br81_br79 * 0.5 / predChargeNumber

                For k = reminderIndex To peaks.Count - 1
                    Dim isotopePeak = peaks(k)
                    If isotopePeak.IsotopeWeightNumber >= 0 Then Continue For

                    Dim isotopeMz = isotopePeak.Mass
                    Dim diffMz = Math.Abs(predIsotopicMass - isotopeMz)
                    Dim diffMzClBr = Math.Abs(predClBrIsotopicMass - isotopeMz)

                    If diffMz < tolerance Then

                        If isotopeTemps(j).PeakID = -1 Then
                            isotopeTemps(j) = New IsotopeTemp() With {
                                .WeightNumber = j,
                                .Mz = isotopeMz,
                                .Intensity = isotopePeak.Intensity,
                                .PeakID = k
                            }
                            mzFocused = isotopeMz
                        Else
                            If Math.Abs(isotopeTemps(j).Mz - predIsotopicMass) > Math.Abs(isotopeMz - predIsotopicMass) Then
                                isotopeTemps(j).Mz = isotopeMz
                                isotopeTemps(j).Intensity = isotopePeak.Intensity
                                isotopeTemps(j).PeakID = k

                                mzFocused = isotopeMz
                            End If
                        End If
                    ElseIf isBrClConsideredForIsotopes AndAlso j Mod 2 = 0 AndAlso diffMzClBr < tolerance Then
                        If isotopeTemps(j).PeakID = -1 Then
                            isotopeTemps(j) = New IsotopeTemp() With {
                                .WeightNumber = j,
                                .Mz = isotopeMz,
                                .MzClBr = isotopeMz,
                                .Intensity = isotopePeak.Intensity,
                                .PeakID = k
                            }
                            mzFocused = isotopeMz
                        Else
                            If Math.Abs(isotopeTemps(j).Mz - predIsotopicMass) > Math.Abs(isotopeMz - predIsotopicMass) Then
                                isotopeTemps(j).Mz = isotopeMz
                                isotopeTemps(j).MzClBr = isotopeMz
                                isotopeTemps(j).Intensity = isotopePeak.Intensity
                                isotopeTemps(j).PeakID = k

                                mzFocused = isotopeMz
                            End If
                        End If
                    ElseIf isotopePeak.Mass >= predIsotopicMass + tolerance Then
                        If k = peaks.Count - 1 Then Exit For
                        reminderIndex = k
                        If isotopeTemps(j - 1).PeakID = -1 AndAlso isotopeTemps(j).PeakID = -1 Then
                            isFinished = True
                        ElseIf isotopeTemps(j).PeakID = -1 Then
                            mzFocused += c13_c12Diff / predChargeNumber
                        End If
                        Exit For
                    End If

                Next
                If isFinished Then Exit For
            Next

            ' finalize and store
            Dim monoisotopicMass = peak.Mass * predChargeNumber
            Dim simulatedFormulaByAlkane = getSimulatedFormulaByAlkane(monoisotopicMass)

            'from here, simple decreasing will be expected for <= 800 Da
            'simulated profiles by alkane formula will be projected to the real abundances for the peaks of more than 800 Da
            Dim simulatedIsotopicPeaks As IsotopeProperty = Nothing
            Dim isIsotopeDetected = False
            Dim iupac = New IupacDatabase
            If monoisotopicMass > 800 Then simulatedIsotopicPeaks = GetNominalIsotopeProperty(simulatedFormulaByAlkane, maxTraceNumber + 1, iupac)

            For j = 1 To maxTraceNumber
                If isotopeTemps(j).PeakID = -1 Then Continue For
                If isotopeTemps(j - 1).PeakID = -1 AndAlso isotopeTemps(j).PeakID = -1 Then Exit For

                If monoisotopicMass <= 800 Then
                    If isotopeTemps(j - 1).Intensity > isotopeTemps(j).Intensity AndAlso isBrClConsideredForIsotopes = False Then
                        peaks(isotopeTemps(j).PeakID).IsotopeParentPeakID = peak.PeakID
                        peaks(isotopeTemps(j).PeakID).IsotopeWeightNumber = j
                        peaks(isotopeTemps(j).PeakID).Charge = peak.Charge
                        isIsotopeDetected = True
                    ElseIf isBrClConsideredForIsotopes = True Then
                        peaks(isotopeTemps(j).PeakID).IsotopeParentPeakID = peak.PeakID
                        peaks(isotopeTemps(j).PeakID).IsotopeWeightNumber = j
                        peaks(isotopeTemps(j).PeakID).Charge = peak.Charge
                        isIsotopeDetected = True
                    Else
                        Exit For
                    End If
                Else
                    If isotopeTemps(j - 1).Intensity <= 0 Then Exit For
                    Dim expRatio = isotopeTemps(j).Intensity / isotopeTemps(j - 1).Intensity
                    Dim simRatio = simulatedIsotopicPeaks.IsotopeProfile(j).RelativeAbundance / simulatedIsotopicPeaks.IsotopeProfile(j - 1).RelativeAbundance

                    If Math.Abs(expRatio - simRatio) < 5.0 Then
                        peaks(isotopeTemps(j).PeakID).IsotopeParentPeakID = peak.PeakID
                        peaks(isotopeTemps(j).PeakID).IsotopeWeightNumber = j
                        peaks(isotopeTemps(j).PeakID).Charge = peak.Charge
                        isIsotopeDetected = True
                    Else
                        Exit For
                    End If
                End If
            Next
            If Not isIsotopeDetected Then
                peak.Charge = 1
            End If
        Next
    End Sub

    Private Shared Function getSimulatedFormulaByAlkane(ByVal mass As Double) As String

        Dim ch2Mass = 14.0
        Dim carbonCount = CInt(mass / ch2Mass)
        Dim hCount = carbonCount * 2

        If carbonCount = 0 OrElse carbonCount = 1 Then
            Return "CH2"
        Else
            Return "C" & carbonCount.ToString() & "H" & hCount.ToString()
        End If

    End Function
End Class
