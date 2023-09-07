Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Public Class MatchedPeak
    Public Property IsProductIonMatched As Boolean = False
    Public Property IsNeutralLossMatched As Boolean = False
    Public Property Mass As Double
    Public Property Intensity As Double
    Public Property MatchedIntensity As Double
End Class
Public NotInheritable Class MsScanMatching
    Private Sub New()
    End Sub

    Public Shared Function IsComparedAvailable(Of T)(obj1 As IReadOnlyCollection(Of T), obj2 As IReadOnlyCollection(Of T)) As Boolean
        If obj1 Is Nothing OrElse obj2 Is Nothing OrElse obj1.Count = 0 OrElse obj2.Count = 0 Then Return False
        Return True
    End Function

    Public Shared Function IsComparedAvailable(obj1 As IMSScanProperty, obj2 As IMSScanProperty) As Boolean
        If obj1.Spectrum Is Nothing OrElse obj2.Spectrum Is Nothing OrElse obj1.Spectrum.Count = 0 OrElse obj2.Spectrum.Count = 0 Then Return False
        Return True
    End Function


    'public static MsScanMatchResult CompareIMMS2ScanProperties(IMSScanProperty scanProp, MoleculeMsReference refSpec, 
    '    MsRefSearchParameterBase param, double scanCCS, List<IsotopicPeak> scanIsotopes = null, List<IsotopicPeak> refIsotopes = null) {
    '    var result = CompareMS2ScanProperties(scanProp, refSpec, param, scanIsotopes, refIsotopes);
    '    var isCcsMatch = false;
    '    var ccsSimilarity = GetGaussianSimilarity(scanCCS, refSpec.CollisionCrossSection, param.CcsTolerance, out isCcsMatch);

    '    result.CcsSimilarity = (float)ccsSimilarity;
    '    result.IsCcsMatch = isCcsMatch;

    '    result.TotalScore = (float)GetTotalScore(result, param);

    '    return result;
    '}


    'public static MsScanMatchResult CompareIMMS2LipidomicsScanProperties(IMSScanProperty scanProp, MoleculeMsReference refSpec,
    '    MsRefSearchParameterBase param, double scanCCS, List<IsotopicPeak> scanIsotopes = null, List<IsotopicPeak> refIsotopes = null) {
    '    var result = CompareMS2LipidomicsScanProperties(scanProp, refSpec, param, scanIsotopes, refIsotopes);
    '    var isCcsMatch = false;
    '    var ccsSimilarity = GetGaussianSimilarity(scanCCS, refSpec.CollisionCrossSection, param.CcsTolerance, out isCcsMatch);

    '    result.CcsSimilarity = (float)ccsSimilarity;
    '    result.IsCcsMatch = isCcsMatch;

    '    result.TotalScore = (float)GetTotalScore(result, param);

    '    return result;
    '}

    Public Shared Function CompareMS2ScanProperties(scanProp As IMSScanProperty, refSpec As MoleculeMsReference, param As MsRefSearchParameterBase, Optional targetOmics As TargetOmics = TargetOmics.Metabolomics, Optional scanCCS As Double = -1.0, Optional scanIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, Optional refIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, Optional andromedaDelta As Double = 100, Optional andromedaMaxPeaks As Integer = 12) As MsScanMatchResult

        Dim result As MsScanMatchResult = Nothing
        If targetOmics = TargetOmics.Metabolomics Then
            result = CompareMSScanProperties(scanProp, refSpec, param, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        ElseIf targetOmics = TargetOmics.Lipidomics Then
            ' result = CompareMS2LipidomicsScanProperties(scanProp, refSpec, param)
            Throw New NotImplementedException
        End If

        result.IsotopeSimilarity = CSng(GetIsotopeRatioSimilarity(scanIsotopes, refIsotopes, scanProp.PrecursorMz, param.Ms1Tolerance))

        Dim isCcsMatch = False
        Dim ccsSimilarity = GetGaussianSimilarity(scanCCS, refSpec.CollisionCrossSection, param.CcsTolerance, isCcsMatch)

        result.CcsSimilarity = CSng(ccsSimilarity)
        result.IsCcsMatch = isCcsMatch
        result.TotalScore = CSng(GetTotalScore(result, param))
        Return result
    End Function

    Public Shared Function CompareMS2ScanProperties(scanProp As IMSScanProperty, chargestate As Integer, refSpec As PeptideMsReference, param As MsRefSearchParameterBase, Optional targetOmics As TargetOmics = TargetOmics.Metabolomics, Optional scanCCS As Double = -1.0, Optional scanIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, Optional refIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, Optional andromedaDelta As Double = 100, Optional andromedaMaxPeaks As Integer = 12) As MsScanMatchResult

        Dim result As MsScanMatchResult = Nothing
        If targetOmics = TargetOmics.Proteomics Then
            result = CompareMS2ProteomicsScanProperties(scanProp, chargestate, refSpec, param, andromedaDelta, andromedaMaxPeaks)
        End If

        result.IsotopeSimilarity = CSng(GetIsotopeRatioSimilarity(scanIsotopes, refIsotopes, scanProp.PrecursorMz, param.Ms1Tolerance))

        Dim isCcsMatch = False
        Dim ccsSimilarity = GetGaussianSimilarity(scanCCS, refSpec.CollisionCrossSection, param.CcsTolerance, isCcsMatch)

        result.CcsSimilarity = CSng(ccsSimilarity)
        result.IsCcsMatch = isCcsMatch
        result.TotalScore = CSng(GetTotalScore(result, param))
        Return result
    End Function

    'public static MsScanMatchResult CompareMS2ScanProperties(IMSScanProperty scanProp, MoleculeMsReference refSpec, MsRefSearchParameterBase param, 
    '    IReadOnlyList<IsotopicPeak> scanIsotopes = null, IReadOnlyList<IsotopicPeak> refIsotopes = null) {
    '    var result = CompareMSScanProperties(scanProp, refSpec, param, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd);
    '    result.IsotopeSimilarity = (float)GetIsotopeRatioSimilarity(scanIsotopes, refIsotopes, scanProp.PrecursorMz, param.Ms1Tolerance);
    '    result.TotalScore = (float)GetTotalScore(result, param);
    '    return result;
    '}

    'public static MsScanMatchResult CompareMS2LipidomicsScanProperties(IMSScanProperty scanProp, MoleculeMsReference refSpec, MsRefSearchParameterBase param,
    '   IReadOnlyList<IsotopicPeak> scanIsotopes = null, IReadOnlyList<IsotopicPeak> refIsotopes = null) {

    '    var result = CompareMS2LipidomicsScanProperties(scanProp, refSpec, param);
    '    var isotopeSimilarity = (float)GetIsotopeRatioSimilarity(scanIsotopes, refIsotopes, scanProp.PrecursorMz, param.Ms1Tolerance);
    '    result.IsotopeSimilarity = (float)GetIsotopeRatioSimilarity(scanIsotopes, refIsotopes, scanProp.PrecursorMz, param.Ms1Tolerance);
    '    result.TotalScore = (float)GetTotalScore(result, param);
    '    return result;
    '}


    Public Shared Function CompareMS2ProteomicsScanProperties(scanProp As IMSScanProperty, chargestate As Integer, refSpec As PeptideMsReference, param As MsRefSearchParameterBase, andromedaDelta As Single, andromedaMaxPeaks As Single) As MsScanMatchResult

        Dim result = CompareBasicMSScanProperties(scanProp, refSpec, param, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        Dim matchedPeaks = GetMachedSpectralPeaks(scanProp, chargestate, refSpec, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)

        result.Name = refSpec.Peptide.ModifiedSequence
        result.AndromedaScore = CSng(GetAndromedaScore(matchedPeaks, andromedaDelta, andromedaMaxPeaks))
        result.MatchedPeaksCount = matchedPeaks.Where(Function(n) n.IsMatched).Count
        result.MatchedPeaksPercentage = CSng((result.MatchedPeaksCount / matchedPeaks.Count()))

        If result.WeightedDotProduct >= param.WeightedDotProductCutOff AndAlso result.SimpleDotProduct >= param.SimpleDotProductCutOff AndAlso result.ReverseDotProduct >= param.ReverseDotProductCutOff AndAlso result.MatchedPeaksPercentage >= param.MatchedPeaksPercentageCutOff AndAlso result.MatchedPeaksCount >= param.MinimumSpectrumMatch AndAlso result.AndromedaScore >= param.AndromedaScoreCutOff Then
            result.IsSpectrumMatch = True
        End If
        result.TotalScore = CSng(GetTotalScore(result, param))

        Return result
    End Function


    Public Shared Function CompareEIMSScanProperties(scanProp As IMSScanProperty, refSpec As MoleculeMsReference, param As MsRefSearchParameterBase, Optional isUseRetentionIndex As Boolean = False) As MsScanMatchResult
        Dim result = CompareMSScanProperties(scanProp, refSpec, param, param.Ms1Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        Dim msMatchedScore = GetIntegratedSpectraSimilarity(result)
        If isUseRetentionIndex Then
            result.TotalScore = CSng(GetTotalSimilarity(result.RiSimilarity, msMatchedScore, param.IsUseTimeForAnnotationScoring))
        Else
            result.TotalScore = CSng(GetTotalSimilarity(result.RtSimilarity, msMatchedScore, param.IsUseTimeForAnnotationScoring))
        End If
        Return result
    End Function

    Public Shared Function CompareEIMSScanProperties(scan1 As IMSScanProperty, scan2 As IMSScanProperty, param As MsRefSearchParameterBase, Optional isUseRetentionIndex As Boolean = False) As MsScanMatchResult
        Dim result = CompareMSScanProperties(scan1, scan2, param, param.Ms1Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        Dim msMatchedScore = GetIntegratedSpectraSimilarity(result)
        If isUseRetentionIndex Then
            result.TotalScore = CSng(GetTotalSimilarity(result.RiSimilarity, msMatchedScore))
        Else
            result.TotalScore = CSng(GetTotalSimilarity(result.RtSimilarity, msMatchedScore))
        End If
        Return result
    End Function

    Public Shared Function GetIntegratedSpectraSimilarity(result As MsScanMatchResult) As Double
        Dim dotproductFact = 3.0
        Dim revDotproductFact = 2.0
        Dim matchedRatioFact = 1.0
        Return (dotproductFact * result.WeightedDotProduct + revDotproductFact * result.ReverseDotProduct + matchedRatioFact * result.MatchedPeaksPercentage) / (dotproductFact + revDotproductFact + matchedRatioFact)
    End Function


    Public Shared Function CompareMSScanProperties(scanProp As IMSScanProperty, refSpec As MoleculeMsReference, param As MsRefSearchParameterBase, ms2Tol As Single, massRangeBegin As Single, massRangeEnd As Single) As MsScanMatchResult

        Dim result = CompareMSScanProperties(scanProp, CType(refSpec, IMSScanProperty), param, ms2Tol, massRangeBegin, massRangeEnd)
        result.Name = refSpec.Name
        result.LibraryID = refSpec.ScanID
        result.InChIKey = refSpec.InChIKey
        Return result
    End Function

    Public Shared Function CompareMSScanProperties(scanProp As IMSScanProperty, refSpec As IMSScanProperty, param As MsRefSearchParameterBase, ms2Tol As Single, massRangeBegin As Single, massRangeEnd As Single) As MsScanMatchResult

        Dim result = CompareBasicMSScanProperties(scanProp, refSpec, param, ms2Tol, massRangeBegin, massRangeEnd)
        Dim matchedPeaksScores = GetMatchedPeaksScores(scanProp, refSpec, ms2Tol, massRangeBegin, massRangeEnd)

        result.MatchedPeaksCount = CSng(matchedPeaksScores(1))
        result.MatchedPeaksPercentage = CSng(matchedPeaksScores(0))
        If result.WeightedDotProduct >= param.WeightedDotProductCutOff AndAlso result.SimpleDotProduct >= param.SimpleDotProductCutOff AndAlso result.ReverseDotProduct >= param.ReverseDotProductCutOff AndAlso result.MatchedPeaksPercentage >= param.MatchedPeaksPercentageCutOff AndAlso result.MatchedPeaksCount >= param.MinimumSpectrumMatch Then
            result.IsSpectrumMatch = True
        End If

        Return result
    End Function

    Public Shared Function CompareBasicMSScanProperties(scanProp As IMSScanProperty, refSpec As MoleculeMsReference, param As MsRefSearchParameterBase, ms2Tol As Single, massRangeBegin As Single, massRangeEnd As Single) As MsScanMatchResult
        Dim result = CompareMSScanProperties(scanProp, CType(refSpec, IMSScanProperty), param, ms2Tol, massRangeBegin, massRangeEnd)
        result.Name = refSpec.Name
        result.LibraryID = refSpec.ScanID
        result.InChIKey = refSpec.InChIKey
        Return result
    End Function

    Public Shared Function CompareBasicMSScanProperties(scanProp As IMSScanProperty, refSpec As IMSScanProperty, param As MsRefSearchParameterBase, ms2Tol As Single, massRangeBegin As Single, massRangeEnd As Single) As MsScanMatchResult

        Dim isRtMatch = False
        Dim isRiMatch = False
        Dim isMs1Match = False

        Dim weightedDotProduct = GetWeightedDotProduct(scanProp, refSpec, ms2Tol, massRangeBegin, massRangeEnd)
        Dim simpleDotProduct = GetSimpleDotProduct(scanProp, refSpec, ms2Tol, massRangeBegin, massRangeEnd)
        Dim reverseDotProduct = GetReverseDotProduct(scanProp, refSpec, ms2Tol, massRangeBegin, massRangeEnd)
        Dim rtSimilarity = GetGaussianSimilarity(scanProp.ChromXs.RT, refSpec.ChromXs.RT, param.RtTolerance, isRtMatch)
        Dim riSimilarity = GetGaussianSimilarity(scanProp.ChromXs.RI, refSpec.ChromXs.RI, param.RiTolerance, isRiMatch)

        Dim ms1Tol = param.Ms1Tolerance
        Dim ppm = std.Abs(PPMmethod.PPM(500.0, 500.0 + ms1Tol))
        If scanProp.PrecursorMz > 500 Then
            ms1Tol = CSng(PPMmethod.ConvertPpmToMassAccuracy(scanProp.PrecursorMz, ppm))
        End If
        Dim ms1Similarity = GetGaussianSimilarity(scanProp.PrecursorMz, refSpec.PrecursorMz, ms1Tol, isMs1Match)

        Dim result = New MsScanMatchResult() With {
            .LibraryID = refSpec.ScanID,
            .WeightedDotProduct = weightedDotProduct,
            .SimpleDotProduct = simpleDotProduct,
            .ReverseDotProduct = reverseDotProduct,
            .AcurateMassSimilarity = ms1Similarity,
            .RtSimilarity = rtSimilarity,
            .RiSimilarity = riSimilarity,
            .IsPrecursorMzMatch = isMs1Match,
            .IsRtMatch = isRtMatch,
            .IsRiMatch = isRiMatch
        }

        Return result
    End Function





    ''' <summary>
    ''' This method returns the similarity score between theoretical isotopic ratios and experimental isotopic patterns in MS1 axis.
    ''' This method will utilize up to [M+4] for their calculations.
    ''' </summary>
    ''' <param name="peaks1">
    ''' Add the MS1 spectrum with respect to the focused data point.
    ''' </param>
    ''' <param name="peaks2">
    ''' Add the theoretical isotopic abundances. The theoretical patterns are supposed to be calculated in MSP parcer.
    ''' </param>
    ''' <param name="targetedMz">
    ''' Add the experimental precursor mass.
    ''' </param>
    ''' <param name="tolerance">
    ''' Add the torelance to merge the spectrum of experimental MS1.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetIsotopeRatioSimilarity(peaks1 As IReadOnlyList(Of IsotopicPeak), peaks2 As IReadOnlyList(Of IsotopicPeak), targetedMz As Double, tolerance As Double) As Double
        If Not IsComparedAvailable(peaks1, peaks2) Then Return -1

        Dim similarity As Double = 0
        Dim ratio1 As Double = 0, ratio2 As Double = 0
        If peaks1(0).RelativeAbundance <= 0 OrElse peaks2(0).RelativeAbundance <= 0 Then Return -1

        Dim minimum = std.Min(peaks1.Count, peaks2.Count)
        For i = 1 To minimum - 1
            ratio1 = peaks1(i).RelativeAbundance / peaks1(0).RelativeAbundance
            ratio2 = peaks2(i).RelativeAbundance / peaks2(0).RelativeAbundance

            If ratio1 <= 1 AndAlso ratio2 <= 1 Then
                similarity += std.Abs(ratio1 - ratio2)
            Else
                If ratio1 > ratio2 Then
                    similarity += 1 - ratio2 / ratio1
                ElseIf ratio2 > ratio1 Then
                    similarity += 1 - ratio1 / ratio2
                End If
            End If
        Next
        Return 1 - similarity
    End Function

    ''' <summary>
    ''' This method returns the presence similarity (% of matched fragments) between the experimental MS/MS spectrum and the standard MS/MS spectrum.
    ''' So, this program will calculate how many fragments of library spectrum are found in the experimental spectrum and will return the %.
    ''' double[] [0]m/z[1]intensity
    ''' 
    ''' </summary>
    ''' <param name="prop1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <param name="prop2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <param name="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' [0] The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be returned.
    ''' [1] MatchedPeaksCount is also returned.
    ''' </returns>
    Public Shared Function GetMatchedPeaksScores(prop1 As IMSScanProperty, prop2 As IMSScanProperty, bin As Double, massBegin As Double, massEnd As Double) As Double()
        If Not IsComparedAvailable(prop1, prop2) Then Return New Double(1) {-1, -1}

        Dim peaks1 = prop1.Spectrum
        Dim peaks2 = prop2.Spectrum

        Return GetMatchedPeaksScores(peaks1, peaks2, bin, massBegin, massEnd)
    End Function

    Public Shared Function GetMatchedPeaksScores(peaks1 As List(Of SpectrumPeak), peaks2 As List(Of SpectrumPeak), bin As Double, massBegin As Double, massEnd As Double) As Double()
        If Not IsComparedAvailable(peaks1, peaks2) Then Return New Double(1) {-1, -1}

        Dim sumM As Double = 0, sumL As Double = 0
        Dim minMz = peaks2(0).Mass
        Dim maxMz = peaks2(peaks2.Count - 1).Mass

        If massBegin > minMz Then minMz = massBegin
        If maxMz > massEnd Then maxMz = massEnd

        Dim focusedMz = minMz
        Dim maxLibIntensity = peaks2.Max(Function(n) n.Intensity)
        Dim remaindIndexM = 0, remaindIndexL = 0
        Dim counter = 0
        Dim libCounter = 0

        Dim measuredMassList As List(Of Double()) = New List(Of Double())()
        Dim referenceMassList As List(Of Double()) = New List(Of Double())()

        While focusedMz <= maxMz
            sumL = 0
            For i = remaindIndexL To peaks2.Count - 1
                If peaks2(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks2(i).Mass AndAlso peaks2(i).Mass < focusedMz + bin Then
                    sumL += peaks2(i).Intensity
                Else
                    remaindIndexL = i
                    Exit For
                End If
            Next

            If sumL >= 0.01 * maxLibIntensity Then
                libCounter += 1
            End If

            sumM = 0
            For i = remaindIndexM To peaks1.Count - 1
                If peaks1(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks1(i).Mass AndAlso peaks1(i).Mass < focusedMz + bin Then
                    sumM += peaks1(i).Intensity
                Else
                    remaindIndexM = i
                    Exit For
                End If
            Next

            If sumM > 0 AndAlso sumL >= 0.01 * maxLibIntensity Then
                counter += 1
            End If

            If focusedMz + bin > peaks2(peaks2.Count - 1).Mass Then Exit While
            focusedMz = peaks2(remaindIndexL).Mass
        End While

        If libCounter = 0 Then
            Return New Double(1) {0, 0}
        Else
            Return New Double(1) {counter / libCounter, libCounter}
        End If
    End Function

    Public Shared Function GetSpetralEntropySimilarity(peaks1 As List(Of SpectrumPeak), peaks2 As List(Of SpectrumPeak), bin As Double) As Double
        Dim combinedSpectrum = SpectrumHandler.GetCombinedSpectrum(peaks1, peaks2, bin)
        Dim entropy12 = GetSpectralEntropy(combinedSpectrum)
        Dim entropy1 = GetSpectralEntropy(peaks1)
        Dim entropy2 = GetSpectralEntropy(peaks2)

        Return 1 - (2 * entropy12 - entropy1 - entropy2) * 0.5
    End Function

    Public Shared Function GetSpectralEntropy(peaks As List(Of SpectrumPeak)) As Double
        Dim sumIntensity = peaks.Sum(Function(n) n.Intensity)
        Return -1 * peaks.Sum(Function(n) n.Intensity / sumIntensity * std.Log(n.Intensity / sumIntensity, 2))
    End Function

    Public Shared Function GetModifiedDotProductScore(prop1 As IMSScanProperty, prop2 As IMSScanProperty, Optional massTolerance As Double = 0.05, Optional massToleranceType As MassToleranceType = MassToleranceType.Da) As Double()
        Dim matchedPeaks = New List(Of MatchedPeak)()
        If prop1.PrecursorMz < prop2.PrecursorMz Then
            SearchMatchedPeaks(prop1.Spectrum, prop1.PrecursorMz, prop2.Spectrum, prop2.PrecursorMz, massTolerance, massToleranceType, matchedPeaks)
        Else
            SearchMatchedPeaks(prop2.Spectrum, prop2.PrecursorMz, prop1.Spectrum, prop1.PrecursorMz, massTolerance, massToleranceType, matchedPeaks)
        End If

        If matchedPeaks.Count = 0 Then
            Return New Double() {0, 0}
        End If

        Dim product = matchedPeaks.Sum(Function(n) n.Intensity * n.MatchedIntensity)
        Dim scaler1 = matchedPeaks.Sum(Function(n) n.Intensity * n.Intensity)
        Dim scaler2 = matchedPeaks.Sum(Function(n) n.MatchedIntensity * n.MatchedIntensity)
        Return New Double() {product / (std.Sqrt(scaler1) * std.Sqrt(scaler2)), matchedPeaks.Count}
    End Function

    Public Shared Function GetBonanzaScore(prop1 As IMSScanProperty, prop2 As IMSScanProperty, Optional massTolerance As Double = 0.05, Optional massToleranceType As MassToleranceType = MassToleranceType.Da) As Double()
        Dim matchedPeaks = New List(Of MatchedPeak)()
        If prop1.PrecursorMz < prop2.PrecursorMz Then
            SearchMatchedPeaks(prop1.Spectrum, prop1.PrecursorMz, prop2.Spectrum, prop2.PrecursorMz, massTolerance, massToleranceType, matchedPeaks)
        Else
            SearchMatchedPeaks(prop2.Spectrum, prop2.PrecursorMz, prop1.Spectrum, prop1.PrecursorMz, massTolerance, massToleranceType, matchedPeaks)
        End If

        If matchedPeaks.Count = 0 Then
            Return New Double() {0, 0}
        End If

        Dim product = matchedPeaks.Sum(Function(n) n.Intensity * n.MatchedIntensity)
        Dim scaler1 = prop1.Spectrum.Where(Function(n) n.IsMatched = False).Sum(Function(n) std.Pow(n.Intensity, 2))
        Dim scaler2 = prop2.Spectrum.Where(Function(n) n.IsMatched = False).Sum(Function(n) std.Pow(n.Intensity, 2))
        Return New Double() {product / (product + scaler1 + scaler2), matchedPeaks.Count}
    End Function

    Public Shared Sub SearchMatchedPeaks(ePeaks As List(Of SpectrumPeak), ePrecursor As Double, rPeaks As List(Of SpectrumPeak), rPrecursor As Double, massTolerance As Double, massTolType As MassToleranceType, <Out> ByRef matchedPeaks As List(Of MatchedPeak)) ' small precursor
        ' large precursor
        matchedPeaks = New List(Of MatchedPeak)()
        For Each e In ePeaks
            e.IsMatched = False
        Next
        For Each e In rPeaks
            e.IsMatched = False
        Next

        'match definition: if product ion or neutral loss are within the mass tolerance, it will be recognized as MATCH.
        'The smallest intensity difference will be recognized as highest match.
        Dim precursorDiff = rPrecursor - ePrecursor
        For i = 0 To rPeaks.Count - 1
            Dim rPeak = rPeaks(i)
            Dim massTol = If(massTolType = MassToleranceType.Da, massTolerance, PPMmethod.ConvertPpmToMassAccuracy(rPeak.Mass, massTolerance))
            Dim minPeakID = -1
            Dim minIntensityDiff = Double.MaxValue
            Dim isProduct = False
            For j = 0 To ePeaks.Count - 1
                Dim ePeak = ePeaks(j)
                If ePeak.IsMatched = True Then Continue For
                If std.Abs(ePeak.Mass - rPeak.Mass) < massTol Then
                    Dim intensityDiff = std.Abs(ePeak.Intensity - rPeak.Intensity)
                    If intensityDiff < minIntensityDiff Then
                        minIntensityDiff = intensityDiff
                        minPeakID = j
                        isProduct = True
                    End If
                ElseIf std.Abs(precursorDiff + ePeak.Mass - rPeak.Mass) < massTol Then
                    Dim intensityDiff = std.Abs(ePeak.Intensity - rPeak.Intensity)
                    If intensityDiff < minIntensityDiff Then
                        minIntensityDiff = intensityDiff
                        minPeakID = j
                        isProduct = False
                    End If
                End If
            Next

            If minPeakID >= 0 Then
                rPeak.IsMatched = True
                ePeaks(minPeakID).IsMatched = True
                matchedPeaks.Add(New MatchedPeak() With {
    .Mass = rPeak.Mass,
    .Intensity = rPeak.Intensity,
    .MatchedIntensity = ePeaks(minPeakID).Intensity,
    .IsProductIonMatched = isProduct,
    .IsNeutralLossMatched = Not isProduct
})
            End If
        Next
    End Sub

    Public Shared Function GetProcessedSpectrum(peaks As List(Of SpectrumPeak), peakPrecursorMz As Double, Optional minMz As Double = 0.0, Optional maxMz As Double = 10000, Optional relativeAbundanceCutOff As Double = 0.1, Optional absoluteAbundanceCutOff As Double = 50.0, Optional massTolerance As Double = 0.05, Optional massBinningValue As Double = 1.0, Optional intensityScaleFactor As Double = 0.5, Optional scaledMaxValue As Double = 100, Optional massDelta As Double = 1, Optional maxPeakNumInDelta As Integer = 12, Optional massToleranceType As MassToleranceType = MassToleranceType.Da, Optional isBrClConsideredForIsotopes As Boolean = False, Optional isRemoveIsotopes As Boolean = False, Optional removeAfterPrecursor As Boolean = True) As List(Of SpectrumPeak) ' 0.1%

        'Console.WriteLine("Original peaks");
        'foreach (var peak in peaks) {
        '    Console.WriteLine(peak.Mass + "\t" + peak.Intensity);
        '}

        peaks = SpectrumHandler.GetRefinedPeaklist(peaks, relativeAbundanceCutOff, absoluteAbundanceCutOff, minMz, maxMz, peakPrecursorMz, massTolerance, massToleranceType, 1, isBrClConsideredForIsotopes, isRemoveIsotopes, removeAfterPrecursor)
        'Console.WriteLine("Refined peaks");
        'foreach (var peak in peaks) {
        '    Console.WriteLine(peak.Mass + "\t" + peak.Intensity);
        '}


        peaks = SpectrumHandler.GetBinnedSpectrum(peaks, massBinningValue)
        'Console.WriteLine("Binned peaks");
        'foreach (var peak in peaks) {
        '    Console.WriteLine(peak.Mass + "\t" + peak.Intensity);
        '}

        If massDelta > 1 Then ' meaning the peaks are selected by ordering the intensity values
            peaks = SpectrumHandler.GetBinnedSpectrum(peaks, massDelta, maxPeakNumInDelta)
        End If
        peaks = SpectrumHandler.GetNormalizedPeaks(peaks, intensityScaleFactor, scaledMaxValue)
        'Console.WriteLine("Normalized peaks");
        'foreach (var peak in peaks) {
        '    Console.WriteLine(peak.Mass + "\t" + peak.Intensity);
        '}

        Return peaks
    End Function

    ''' <summary>
    ''' please set the 'mached spectral peaks' list obtained from the method of GetMachedSpectralPeaks where isMatched property is set to each spectrum peak obj
    ''' </summary>
    ''' <param name="peaks"></param>
    ''' <returns></returns>
    Public Shared Function GetAndromedaScore(peaks As List(Of SpectrumPeak), andromedaDelta As Double, andromedaMaxPeak As Double) As Double
        Dim p = andromedaMaxPeak / andromedaDelta
        Dim q = 1 - p
        Dim n = peaks.Count
        Dim k = peaks.Where(Function(spec) spec.IsMatched = True).Count

        Dim sum = 0.0
        For j = k To n
            Dim bc = SpecialFunctions.BinomialCoefficient(n, j)
            Dim p_prob = std.Pow(p, j)
            Dim q_prob = std.Pow(q, n - j)
            sum += bc * p_prob * q_prob
        Next
        Dim andromeda = -10.0 * std.Log10(sum)
        Return If(andromeda < 0.000001, 0.000001, andromeda)
    End Function

    ''' <summary>
    ''' <param name="prop1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <param name="prop2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <param name="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' </summary>
    Public Shared Function GetMachedSpectralPeaks(prop1 As IMSScanProperty, chargeState As Integer, prop2 As IMSScanProperty, bin As Double, massBegin As Double, massEnd As Double) As List(Of SpectrumPeak)
        If Not IsComparedAvailable(prop1, prop2) Then Return New List(Of SpectrumPeak)()

        Dim peaks1 = prop1.Spectrum
        Dim peaks2 = prop2.Spectrum

        Dim searchedPeaks = GetMachedSpectralPeaks(peaks1, peaks2, bin, massBegin, massEnd)

        ' at this moment...
        Dim finalPeaks = New List(Of SpectrumPeak)()
        For Each group In searchedPeaks.GroupBy(Function(n) n.PeakID)
            Dim isParentExist = False
            For Each peak In group
                If peak.SpectrumComment.HasFlag(SpectrumComment.b) AndAlso peak.IsMatched Then
                    isParentExist = True
                End If
                If peak.SpectrumComment.HasFlag(SpectrumComment.y) AndAlso peak.IsMatched Then
                    isParentExist = True
                End If
            Next
            For Each peak In group
                If peak.SpectrumComment.HasFlag(SpectrumComment.precursor) Then Continue For ' exclude
                If chargeState <= 2 AndAlso (peak.SpectrumComment.HasFlag(SpectrumComment.b2) OrElse peak.SpectrumComment.HasFlag(SpectrumComment.y2)) Then Continue For ' exclude
                If Not isParentExist AndAlso (peak.SpectrumComment.HasFlag(SpectrumComment.b_h2o) OrElse peak.SpectrumComment.HasFlag(SpectrumComment.b_nh3) OrElse peak.SpectrumComment.HasFlag(SpectrumComment.y_h2o) OrElse peak.SpectrumComment.HasFlag(SpectrumComment.y_nh3)) Then
                    Continue For
                End If
                finalPeaks.Add(peak)
            Next
        Next

        Return finalPeaks
    End Function

    Public Shared Function GetMachedSpectralPeaks(peaks1 As List(Of SpectrumPeak), peaks2 As List(Of SpectrumPeak), bin As Double, massBegin As Double, massEnd As Double) As List(Of SpectrumPeak)
        If Not IsComparedAvailable(peaks1, peaks2) Then Return New List(Of SpectrumPeak)()
        Dim minMz = std.Max(peaks2(0).Mass, massBegin)
        Dim maxMz = std.Min(peaks2(peaks2.Count - 1).Mass, massEnd)
        Dim focusedMz = minMz

        Dim remaindIndexM = 0, remaindIndexL = 0
        Dim searchedPeaks = New List(Of SpectrumPeak)()

        While focusedMz <= maxMz
            Dim maxRefIntensity = Double.MinValue
            Dim maxRefID = -1
            For i = remaindIndexL To peaks2.Count - 1
                If peaks2(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf std.Abs(focusedMz - peaks2(i).Mass) < bin Then
                    If maxRefIntensity < peaks2(i).Intensity Then
                        maxRefIntensity = peaks2(i).Intensity
                        maxRefID = i
                    End If
                Else
                    remaindIndexL = i
                    Exit For
                End If
            Next

            Dim spectrumPeak As SpectrumPeak = If(maxRefID >= 0, peaks2(maxRefID).Clone(), Nothing)
            If spectrumPeak Is Nothing Then
                focusedMz = peaks2(remaindIndexL).Mass
                If remaindIndexL = peaks2.Count - 1 Then Exit While
                Continue While
            End If
            Dim sumintensity = 0.0
            For i = remaindIndexM To peaks1.Count - 1
                If peaks1(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf std.Abs(focusedMz - peaks1(i).Mass) < bin Then
                    sumintensity += peaks1(i).Intensity
                    spectrumPeak.IsMatched = True
                Else
                    remaindIndexM = i
                    Exit For
                End If
            Next

            spectrumPeak.Resolution = sumintensity
            searchedPeaks.Add(spectrumPeak)

            If focusedMz + bin > peaks2(peaks2.Count - 1).Mass Then Exit While
            focusedMz = peaks2(remaindIndexL).Mass
        End While

        Return searchedPeaks
    End Function

    ''' <summary>
    ''' This program will return so called reverse dot product similarity as described in the previous resport.
    ''' Stein, S. E. An Integrated Method for Spectrum Extraction. J.Am.Soc.Mass.Spectrom, 10, 770-781, 1999.
    ''' The spectrum similarity of MS/MS with respect to library spectrum will be calculated in this method.
    ''' </summary>
    ''' <param name="prop1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <param name="prop2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <param name="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetReverseDotProduct(prop1 As IMSScanProperty, prop2 As IMSScanProperty, bin As Double, massBegin As Double, massEnd As Double) As Double
        Dim scalarM As Double = 0, scalarR As Double = 0, covariance As Double = 0
        Dim sumM As Double = 0, sumL As Double = 0
        If Not IsComparedAvailable(prop1, prop2) Then Return -1

        Dim peaks1 = prop1.Spectrum
        Dim peaks2 = prop2.Spectrum

        Dim minMz = peaks2(0).Mass
        Dim maxMz = peaks2(peaks2.Count - 1).Mass

        If massBegin > minMz Then minMz = massBegin
        If maxMz > massEnd Then maxMz = massEnd

        Dim focusedMz = minMz
        Dim remaindIndexM = 0, remaindIndexL = 0
        Dim counter = 0

        Dim measuredMassList As List(Of Double()) = New List(Of Double())()
        Dim referenceMassList As List(Of Double()) = New List(Of Double())()

        Dim sumMeasure As Double = 0, sumReference As Double = 0, baseM = Double.MinValue, baseR = Double.MinValue

        While focusedMz <= maxMz
            sumL = 0
            For i = remaindIndexL To peaks2.Count - 1
                If peaks2(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks2(i).Mass AndAlso peaks2(i).Mass < focusedMz + bin Then
                    sumL += peaks2(i).Intensity
                Else
                    remaindIndexL = i
                    Exit For
                End If
            Next

            sumM = 0
            For i = remaindIndexM To peaks1.Count - 1
                If peaks1(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks1(i).Mass AndAlso peaks1(i).Mass < focusedMz + bin Then
                    sumM += peaks1(i).Intensity
                Else
                    remaindIndexM = i
                    Exit For
                End If
            Next

            If sumM <= 0 Then
                measuredMassList.Add(New Double() {focusedMz, sumM})
                If sumM > baseM Then baseM = sumM

                referenceMassList.Add(New Double() {focusedMz, sumL})
                If sumL > baseR Then baseR = sumL
            Else
                measuredMassList.Add(New Double() {focusedMz, sumM})
                If sumM > baseM Then baseM = sumM

                referenceMassList.Add(New Double() {focusedMz, sumL})
                If sumL > baseR Then baseR = sumL

                counter += 1
            End If

            If focusedMz + bin > peaks2(peaks2.Count - 1).Mass Then Exit While
            focusedMz = peaks2(remaindIndexL).Mass
        End While

        If baseM = 0 OrElse baseR = 0 Then Return 0

        Dim eSpectrumCounter = 0
        Dim lSpectrumCounter = 0
        For i = 0 To measuredMassList.Count - 1
            measuredMassList(i)(1) = measuredMassList(i)(1) / baseM
            referenceMassList(i)(1) = referenceMassList(i)(1) / baseR
            sumMeasure += measuredMassList(i)(1)
            sumReference += referenceMassList(i)(1)

            If measuredMassList(i)(1) > 0.1 Then eSpectrumCounter += 1
            If referenceMassList(i)(1) > 0.1 Then lSpectrumCounter += 1
        Next

        Dim peakCountPenalty = 1.0
        If lSpectrumCounter = 1 Then
            peakCountPenalty = 0.75
        ElseIf lSpectrumCounter = 2 Then
            peakCountPenalty = 0.88
        ElseIf lSpectrumCounter = 3 Then
            peakCountPenalty = 0.94
        ElseIf lSpectrumCounter = 4 Then
            peakCountPenalty = 0.97
        End If

        Dim wM, wR As Double

        If sumMeasure - 0.5 = 0 Then
            wM = 0
        Else
            wM = 1 / (sumMeasure - 0.5)
        End If

        If sumReference - 0.5 = 0 Then
            wR = 0
        Else
            wR = 1 / (sumReference - 0.5)
        End If

        Dim cutoff = 0.01

        For i = 0 To measuredMassList.Count - 1
            If referenceMassList(i)(1) < cutoff Then Continue For

            scalarM += measuredMassList(i)(1) * measuredMassList(i)(0)
            scalarR += referenceMassList(i)(1) * referenceMassList(i)(0)
            covariance += std.Sqrt(measuredMassList(i)(1) * referenceMassList(i)(1)) * measuredMassList(i)(0)

            'scalarM += measuredMassList[i][1];
            'scalarR += referenceMassList[i][1];
            'covariance += Math.Sqrt(measuredMassList[i][1] * referenceMassList[i][1]);
        Next

        If scalarM = 0 OrElse scalarR = 0 Then
            Return 0
        Else
            Return std.Pow(covariance, 2) / scalarM / scalarR * peakCountPenalty
        End If
    End Function

    ''' <summary>
    ''' This program will return so called dot product similarity as described in the previous resport.
    ''' Stein, S. E. An Integrated Method for Spectrum Extraction. J.Am.Soc.Mass.Spectrom, 10, 770-781, 1999.
    ''' The spectrum similarity of MS/MS will be calculated in this method.
    ''' </summary>
    ''' <param name="prop1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <param name="prop2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <param name="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetWeightedDotProduct(prop1 As IMSScanProperty, prop2 As IMSScanProperty, bin As Double, massBegin As Double, massEnd As Double) As Double
        Dim scalarM As Double = 0, scalarR As Double = 0, covariance As Double = 0
        Dim sumM As Double = 0, sumR As Double = 0

        If Not IsComparedAvailable(prop1, prop2) Then Return -1

        Dim peaks1 = prop1.Spectrum
        Dim peaks2 = prop2.Spectrum

        Dim minMz = std.Min(peaks1(0).Mass, peaks2(0).Mass)
        Dim maxMz = std.Max(peaks1(peaks1.Count - 1).Mass, peaks2(peaks2.Count - 1).Mass)

        If massBegin > minMz Then minMz = massBegin
        If maxMz > massEnd Then maxMz = massEnd

        Dim focusedMz = minMz
        Dim remaindIndexM = 0, remaindIndexL = 0

        Dim measuredMassList As List(Of Double()) = New List(Of Double())()
        Dim referenceMassList As List(Of Double()) = New List(Of Double())()

        Dim sumMeasure As Double = 0, sumReference As Double = 0, baseM = Double.MinValue, baseR = Double.MinValue

        While focusedMz <= maxMz
            sumM = 0
            For i = remaindIndexM To peaks1.Count - 1
                If peaks1(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks1(i).Mass AndAlso peaks1(i).Mass < focusedMz + bin Then
                    sumM += peaks1(i).Intensity
                Else
                    remaindIndexM = i
                    Exit For
                End If
            Next

            sumR = 0
            For i = remaindIndexL To peaks2.Count - 1
                If peaks2(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks2(i).Mass AndAlso peaks2(i).Mass < focusedMz + bin Then
                    sumR += peaks2(i).Intensity
                Else
                    remaindIndexL = i
                    Exit For
                End If
            Next

            If sumM <= 0 AndAlso sumR > 0 Then
                measuredMassList.Add(New Double() {focusedMz, sumM})
                If sumM > baseM Then baseM = sumM

                referenceMassList.Add(New Double() {focusedMz, sumR})
                If sumR > baseR Then baseR = sumR
            Else
                measuredMassList.Add(New Double() {focusedMz, sumM})
                If sumM > baseM Then baseM = sumM

                referenceMassList.Add(New Double() {focusedMz, sumR})
                If sumR > baseR Then baseR = sumR
            End If

            If focusedMz + bin > std.Max(peaks1(peaks1.Count - 1).Mass, peaks2(peaks2.Count - 1).Mass) Then Exit While
            If focusedMz + bin > peaks2(remaindIndexL).Mass AndAlso focusedMz + bin <= peaks1(remaindIndexM).Mass Then
                focusedMz = peaks1(remaindIndexM).Mass
            ElseIf focusedMz + bin <= peaks2(remaindIndexL).Mass AndAlso focusedMz + bin > peaks1(remaindIndexM).Mass Then
                focusedMz = peaks2(remaindIndexL).Mass
            Else
                focusedMz = std.Min(peaks1(remaindIndexM).Mass, peaks2(remaindIndexL).Mass)
            End If
        End While

        If baseM = 0 OrElse baseR = 0 Then Return 0


        Dim eSpectrumCounter = 0
        Dim lSpectrumCounter = 0
        For i = 0 To measuredMassList.Count - 1
            measuredMassList(i)(1) = measuredMassList(i)(1) / baseM
            referenceMassList(i)(1) = referenceMassList(i)(1) / baseR
            sumMeasure += measuredMassList(i)(1)
            sumReference += referenceMassList(i)(1)

            If measuredMassList(i)(1) > 0.1 Then eSpectrumCounter += 1
            If referenceMassList(i)(1) > 0.1 Then lSpectrumCounter += 1
        Next

        Dim peakCountPenalty = 1.0
        If lSpectrumCounter = 1 Then
            peakCountPenalty = 0.75
        ElseIf lSpectrumCounter = 2 Then
            peakCountPenalty = 0.88
        ElseIf lSpectrumCounter = 3 Then
            peakCountPenalty = 0.94
        ElseIf lSpectrumCounter = 4 Then
            peakCountPenalty = 0.97
        End If

        Dim wM, wR As Double

        If sumMeasure - 0.5 = 0 Then
            wM = 0
        Else
            wM = 1 / (sumMeasure - 0.5)
        End If

        If sumReference - 0.5 = 0 Then
            wR = 0
        Else
            wR = 1 / (sumReference - 0.5)
        End If

        Dim cutoff = 0.01
        For i = 0 To measuredMassList.Count - 1
            If measuredMassList(i)(1) < cutoff Then Continue For

            scalarM += measuredMassList(i)(1) * measuredMassList(i)(0)
            scalarR += referenceMassList(i)(1) * referenceMassList(i)(0)
            covariance += std.Sqrt(measuredMassList(i)(1) * referenceMassList(i)(1)) * measuredMassList(i)(0)

            'scalarM += measuredMassList[i][1];
            'scalarR += referenceMassList[i][1];
            'covariance += Math.Sqrt(measuredMassList[i][1] * referenceMassList[i][1]);
        Next

        If scalarM = 0 OrElse scalarR = 0 Then
            Return 0
        Else
            Return std.Pow(covariance, 2) / scalarM / scalarR * peakCountPenalty
        End If
    End Function

    Public Shared Function GetSimpleDotProduct(prop1 As IMSScanProperty, prop2 As IMSScanProperty, bin As Double, massBegin As Double, massEnd As Double) As Double
        Dim scalarM As Double = 0, scalarR As Double = 0, covariance As Double = 0
        Dim sumM As Double = 0, sumR As Double = 0

        If Not IsComparedAvailable(prop1, prop2) Then Return -1

        Dim peaks1 = prop1.Spectrum
        Dim peaks2 = prop2.Spectrum

        Dim minMz = std.Min(peaks1(0).Mass, peaks2(0).Mass)
        Dim maxMz = std.Max(peaks1(peaks1.Count - 1).Mass, peaks2(peaks2.Count - 1).Mass)
        Dim focusedMz = minMz
        Dim remaindIndexM = 0, remaindIndexL = 0

        If massBegin > minMz Then minMz = massBegin
        If maxMz > massEnd Then maxMz = massEnd


        Dim measuredMassList As List(Of Double()) = New List(Of Double())()
        Dim referenceMassList As List(Of Double()) = New List(Of Double())()

        Dim sumMeasure As Double = 0, sumReference As Double = 0, baseM = Double.MinValue, baseR = Double.MinValue

        While focusedMz <= maxMz
            sumM = 0
            For i = remaindIndexM To peaks1.Count - 1
                If peaks1(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks1(i).Mass AndAlso peaks1(i).Mass < focusedMz + bin Then
                    sumM += peaks1(i).Intensity
                Else
                    remaindIndexM = i
                    Exit For
                End If
            Next

            sumR = 0
            For i = remaindIndexL To peaks2.Count - 1
                If peaks2(i).Mass < focusedMz - bin Then
                    Continue For
                ElseIf focusedMz - bin <= peaks2(i).Mass AndAlso peaks2(i).Mass < focusedMz + bin Then
                    sumR += peaks2(i).Intensity
                Else
                    remaindIndexL = i
                    Exit For
                End If
            Next

            measuredMassList.Add(New Double() {focusedMz, sumM})
            If sumM > baseM Then baseM = sumM

            referenceMassList.Add(New Double() {focusedMz, sumR})
            If sumR > baseR Then baseR = sumR

            If focusedMz + bin > std.Max(peaks1(peaks1.Count - 1).Mass, peaks2(peaks2.Count - 1).Mass) Then Exit While
            If focusedMz + bin > peaks2(remaindIndexL).Mass AndAlso focusedMz + bin <= peaks1(remaindIndexM).Mass Then
                focusedMz = peaks1(remaindIndexM).Mass
            ElseIf focusedMz + bin <= peaks2(remaindIndexL).Mass AndAlso focusedMz + bin > peaks1(remaindIndexM).Mass Then
                focusedMz = peaks2(remaindIndexL).Mass
            Else
                focusedMz = std.Min(peaks1(remaindIndexM).Mass, peaks2(remaindIndexL).Mass)
            End If
        End While

        If baseM = 0 OrElse baseR = 0 Then Return 0

        For i = 0 To measuredMassList.Count - 1
            measuredMassList(i)(1) = measuredMassList(i)(1) / baseM * 999
            referenceMassList(i)(1) = referenceMassList(i)(1) / baseR * 999
        Next

        For i = 0 To measuredMassList.Count - 1
            scalarM += measuredMassList(i)(1)
            scalarR += referenceMassList(i)(1)
            covariance += std.Sqrt(measuredMassList(i)(1) * referenceMassList(i)(1))
        Next

        If scalarM = 0 OrElse scalarR = 0 Then
            Return 0
        Else
            Return std.Pow(covariance, 2) / scalarM / scalarR
        End If
    End Function

    Public Shared Function GetGaussianSimilarity(actual As IChromX, reference As IChromX, tolerance As Double, <Out> ByRef isInTolerance As Boolean) As Double
        isInTolerance = False
        If actual Is Nothing OrElse reference Is Nothing Then Return -1
        If actual.Value <= 0 OrElse reference.Value <= 0 Then Return -1
        If std.Abs(actual.Value - reference.Value) <= tolerance Then isInTolerance = True
        Dim similarity = GetGaussianSimilarity(actual.Value, reference.Value, tolerance)
        Return similarity
    End Function

    Public Shared Function GetGaussianSimilarity(actual As Double, reference As Double, tolerance As Double, <Out> ByRef isInTolerance As Boolean) As Double
        isInTolerance = False
        If actual <= 0 OrElse reference <= 0 Then Return -1
        If std.Abs(actual - reference) <= tolerance Then isInTolerance = True
        Dim similarity = GetGaussianSimilarity(actual, reference, tolerance)
        Return similarity
    End Function

    ''' <summary>
    ''' This method is to calculate the similarity of retention time differences or precursor ion difference from the library information as described in the previous report.
    ''' Tsugawa, H. et al. Anal.Chem. 85, 5191-5199, 2013.
    ''' </summary>
    ''' <param name="actual">
    ''' Add the experimental m/z or retention time.
    ''' </param>
    ''' <param name="reference">
    ''' Add the theoretical m/z or library's retention time.
    ''' </param>
    ''' <param name="tolrance">
    ''' Add the user-defined search tolerance.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetGaussianSimilarity(actual As Double, reference As Double, tolrance As Double) As Double
        Return std.Exp(-0.5 * std.Pow((actual - reference) / tolrance, 2))
    End Function

    ''' <summary>
    ''' MS-DIAL program utilizes the total similarity score to rank the compound candidates.
    ''' This method is to calculate it from four scores including RT, isotopic ratios, m/z, and MS/MS similarities.
    ''' </summary>
    ''' <param name="accurateMassSimilarity"></param>
    ''' <param name="rtSimilarity"></param>
    ''' <param name="isotopeSimilarity"></param>
    ''' <param name="spectraSimilarity"></param>
    ''' <param name="reverseSearchSimilarity"></param>
    ''' <param name="presenceSimilarity"></param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetTotalSimilarity(accurateMassSimilarity As Double, rtSimilarity As Double, isotopeSimilarity As Double, spectraSimilarity As Double, reverseSearchSimilarity As Double, presenceSimilarity As Double, spectrumPenalty As Boolean, targetOmics As TargetOmics, isUseRT As Boolean) As Double
        Dim dotProductFactor = 3.0
        Dim revesrseDotProdFactor = 2.0
        Dim presensePercentageFactor = 1.0

        Dim msmsFactor = 2.0
        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0

        If targetOmics = TargetOmics.Lipidomics Then
            dotProductFactor = 1.0
            revesrseDotProdFactor = 2.0
            presensePercentageFactor = 3.0
            msmsFactor = 1.5
            rtFactor = 0.5
        End If

        Dim msmsSimilarity = (dotProductFactor * spectraSimilarity + revesrseDotProdFactor * reverseSearchSimilarity + presensePercentageFactor * presenceSimilarity) / (dotProductFactor + revesrseDotProdFactor + presensePercentageFactor)

        If spectrumPenalty = True AndAlso targetOmics = TargetOmics.Metabolomics Then msmsSimilarity = msmsSimilarity * 0.5

        If Not isUseRT Then
            If isotopeSimilarity < 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity) / (msmsFactor + massFactor)
            Else
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + isotopeFactor)
            End If
        Else
            If rtSimilarity < 0 AndAlso isotopeSimilarity < 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity) / (msmsFactor + massFactor + rtFactor)
            ElseIf rtSimilarity < 0 AndAlso isotopeSimilarity >= 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + isotopeFactor + rtFactor)
            ElseIf isotopeSimilarity < 0 AndAlso rtSimilarity >= 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity) / (msmsFactor + massFactor + rtFactor)
            Else
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + rtFactor + isotopeFactor)
            End If
        End If
    End Function

    Public Shared Function GetTotalSimilarity(accurateMassSimilarity As Double, rtSimilarity As Double, ccsSimilarity As Double, isotopeSimilarity As Double, spectraSimilarity As Double, reverseSearchSimilarity As Double, presenceSimilarity As Double, spectrumPenalty As Boolean, targetOmics As TargetOmics, isUseRT As Boolean, isUseCcs As Boolean) As Double
        Dim dotProductFactor = 3.0
        Dim revesrseDotProdFactor = 2.0
        Dim presensePercentageFactor = 1.0

        Dim msmsFactor = 2.0
        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0
        Dim ccsFactor = 2.0

        If targetOmics = TargetOmics.Lipidomics Then
            dotProductFactor = 1.0
            revesrseDotProdFactor = 2.0
            presensePercentageFactor = 3.0
            msmsFactor = 1.5
            rtFactor = 0.5
            ccsFactor = 1.0F
        End If

        Dim msmsSimilarity = (dotProductFactor * spectraSimilarity + revesrseDotProdFactor * reverseSearchSimilarity + presensePercentageFactor * presenceSimilarity) / (dotProductFactor + revesrseDotProdFactor + presensePercentageFactor)

        If spectrumPenalty = True AndAlso targetOmics = TargetOmics.Metabolomics Then msmsSimilarity = msmsSimilarity * 0.5

        Dim useRtScore = True
        Dim useCcsScore = True
        Dim useIsotopicScore = True
        If Not isUseRT OrElse rtSimilarity < 0 Then useRtScore = False
        If Not isUseCcs OrElse ccsSimilarity < 0 Then useCcsScore = False
        If isotopeSimilarity < 0 Then useIsotopicScore = False

        If useRtScore = True AndAlso useCcsScore = True AndAlso useIsotopicScore = True Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity + ccsFactor * ccsSimilarity) / (msmsFactor + massFactor + rtFactor + isotopeFactor + ccsFactor)
        ElseIf useRtScore = True AndAlso useCcsScore = True AndAlso useIsotopicScore = False Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + ccsFactor * ccsSimilarity) / (msmsFactor + massFactor + rtFactor + ccsFactor)
        ElseIf useRtScore = True AndAlso useCcsScore = False AndAlso useIsotopicScore = True Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + rtFactor + isotopeFactor)
        ElseIf useRtScore = False AndAlso useCcsScore = True AndAlso useIsotopicScore = True Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity + ccsFactor * ccsSimilarity) / (msmsFactor + massFactor + isotopeFactor + ccsFactor)
        ElseIf useRtScore = False AndAlso useCcsScore = True AndAlso useIsotopicScore = False Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + ccsFactor * ccsSimilarity) / (msmsFactor + massFactor + ccsFactor)
        ElseIf useRtScore = True AndAlso useCcsScore = False AndAlso useIsotopicScore = False Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity) / (msmsFactor + massFactor + rtFactor)
        ElseIf useRtScore = False AndAlso useCcsScore = False AndAlso useIsotopicScore = True Then
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + isotopeFactor)
        Else
            Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity) / (msmsFactor + massFactor)
        End If
        'if (!isUseRT) {
        '    if (isotopeSimilarity < 0 && ccsSimilarity < 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity)
        '            / (msmsFactor + massFactor);
        '    }
        '    else if (isotopeSimilarity < 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + ccsFactor * ccsSimilarity)
        '            / (msmsFactor + massFactor + ccsFactor);
        '    }
        '    else {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity + ccsFactor * ccsSimilarity)
        '            / (msmsFactor + massFactor + isotopeFactor + ccsFactor);
        '    }
        '}
        'else {
        '    if (rtSimilarity < 0 && isotopeSimilarity < 0 && ccsSimilarity < 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity)
        '            / (msmsFactor + massFactor);
        '    }
        '    else if (rtSimilarity < 0 && isotopeSimilarity >= 0 && ccsSimilarity < 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity)
        '            / (msmsFactor + massFactor + isotopeFactor);
        '    }
        '    else if (isotopeSimilarity < 0 && rtSimilarity >= 0 && ccsSimilarity < 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity)
        '            / (msmsFactor + massFactor + rtFactor);
        '    }
        '    else if (isotopeSimilarity < 0 && rtSimilarity < 0 && ccsSimilarity >= 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + ccsFactor * ccsSimilarity)
        '            / (msmsFactor + massFactor + ccsFactor);
        '    }
        '    else if (rtSimilarity >= 0 && isotopeSimilarity >= 0 && ccsSimilarity < 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity + rtFactor * rtSimilarity)
        '            / (msmsFactor + massFactor + isotopeFactor + rtFactor);
        '    }
        '    else if (isotopeSimilarity < 0 && rtSimilarity >= 0 && ccsSimilarity >= 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + ccsFactor * ccsSimilarity)
        '            / (msmsFactor + massFactor + rtFactor + ccsFactor);
        '    }
        '    else if (isotopeSimilarity >= 0 && rtSimilarity < 0 && ccsSimilarity >= 0) {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + ccsFactor * ccsSimilarity + isotopeFactor * isotopeSimilarity)
        '            / (msmsFactor + massFactor + ccsFactor + isotopeFactor);
        '    }
        '    else {
        '        return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity + ccsFactor * ccsSimilarity)
        '            / (msmsFactor + massFactor + rtFactor + isotopeFactor + ccsFactor);
        '    }
        '}
    End Function

    ''' <summary>
    ''' MS-DIAL program also calculate the total similarity score without the MS/MS similarity scoring.
    ''' It means that the total score will be calculated from RT, m/z, and isotopic similarities.
    ''' </summary>
    ''' <param name="accurateMassSimilarity"></param>
    ''' <param name="rtSimilarity"></param>
    ''' <param name="isotopeSimilarity"></param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetTotalSimilarity(accurateMassSimilarity As Double, rtSimilarity As Double, isotopeSimilarity As Double, isUseRT As Boolean) As Double
        If Not isUseRT Then
            If isotopeSimilarity < 0 Then
                Return accurateMassSimilarity
            Else
                Return (accurateMassSimilarity + 0.5 * isotopeSimilarity) / 1.5
            End If
        Else
            If rtSimilarity < 0 AndAlso isotopeSimilarity < 0 Then
                Return accurateMassSimilarity * 0.5
            ElseIf rtSimilarity < 0 AndAlso isotopeSimilarity >= 0 Then
                Return (accurateMassSimilarity + 0.5 * isotopeSimilarity) / 2.5
            ElseIf isotopeSimilarity < 0 AndAlso rtSimilarity >= 0 Then
                Return (accurateMassSimilarity + rtSimilarity) * 0.5
            Else
                Return (accurateMassSimilarity + rtSimilarity + 0.5 * isotopeSimilarity) * 0.4
            End If
        End If
    End Function

    Public Shared Function GetTotalSimilarity(accurateMassSimilarity As Double, rtSimilarity As Double, ccsSimilarity As Double, isotopeSimilarity As Double, isUseRT As Boolean, isUseCcs As Boolean) As Double

        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0
        Dim ccsFactor = 2.0

        Dim useRtScore = True
        Dim useCcsScore = True
        Dim useIsotopicScore = True
        If Not isUseRT OrElse rtSimilarity < 0 Then useRtScore = False
        If Not isUseCcs OrElse ccsSimilarity < 0 Then useCcsScore = False
        If isotopeSimilarity < 0 Then useIsotopicScore = False

        If useRtScore = True AndAlso useCcsScore = True AndAlso useIsotopicScore = True Then
            Return (massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity + ccsFactor * ccsSimilarity) / (massFactor + rtFactor + isotopeFactor + ccsFactor)
        ElseIf useRtScore = True AndAlso useCcsScore = True AndAlso useIsotopicScore = False Then
            Return (massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + ccsFactor * ccsSimilarity) / (massFactor + rtFactor + ccsFactor)
        ElseIf useRtScore = True AndAlso useCcsScore = False AndAlso useIsotopicScore = True Then
            Return (massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity) / (massFactor + rtFactor + isotopeFactor)
        ElseIf useRtScore = False AndAlso useCcsScore = True AndAlso useIsotopicScore = True Then
            Return (massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity + ccsFactor * ccsSimilarity) / (massFactor + isotopeFactor + ccsFactor)
        ElseIf useRtScore = False AndAlso useCcsScore = True AndAlso useIsotopicScore = False Then
            Return (massFactor * accurateMassSimilarity + ccsFactor * ccsSimilarity) / (massFactor + ccsFactor)
        ElseIf useRtScore = True AndAlso useCcsScore = False AndAlso useIsotopicScore = False Then
            Return (massFactor * accurateMassSimilarity + rtFactor * rtSimilarity) / (massFactor + rtFactor)
        ElseIf useRtScore = False AndAlso useCcsScore = False AndAlso useIsotopicScore = True Then
            Return (massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity) / (massFactor + isotopeFactor)
        Else
            Return massFactor * accurateMassSimilarity / massFactor
        End If
    End Function

    Public Shared Function GetTotalSimilarityUsingSimpleDotProduct(accurateMassSimilarity As Double, rtSimilarity As Double, isotopeSimilarity As Double, dotProductSimilarity As Double, spectrumPenalty As Boolean, targetOmics As TargetOmics, isUseRT As Boolean) As Double
        Dim msmsFactor = 2.0
        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0

        Dim msmsSimilarity = dotProductSimilarity

        If spectrumPenalty = True AndAlso targetOmics = TargetOmics.Metabolomics Then msmsSimilarity = msmsSimilarity * 0.5

        If Not isUseRT Then
            If isotopeSimilarity < 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity) / (msmsFactor + massFactor)
            Else
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + isotopeFactor)
            End If
        Else
            If rtSimilarity < 0 AndAlso isotopeSimilarity < 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity) / (msmsFactor + massFactor + rtFactor)
            ElseIf rtSimilarity < 0 AndAlso isotopeSimilarity >= 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + isotopeFactor + rtFactor)
            ElseIf isotopeSimilarity < 0 AndAlso rtSimilarity >= 0 Then
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity) / (msmsFactor + massFactor + rtFactor)
            Else
                Return (msmsFactor * msmsSimilarity + massFactor * accurateMassSimilarity + rtFactor * rtSimilarity + isotopeFactor * isotopeSimilarity) / (msmsFactor + massFactor + rtFactor + isotopeFactor)
            End If
        End If
    End Function

    Public Shared Function GetTotalScore(result As MsScanMatchResult, param As MsRefSearchParameterBase) As Double
        Dim totalScore = 0.0
        If param.IsUseTimeForAnnotationScoring AndAlso result.RtSimilarity > 0 Then
            totalScore += result.RtSimilarity
        End If
        If param.IsUseCcsForAnnotationScoring AndAlso result.CcsSimilarity > 0 Then
            totalScore += result.CcsSimilarity
        End If
        If result.AcurateMassSimilarity > 0 Then
            totalScore += result.AcurateMassSimilarity
        End If
        If result.IsotopeSimilarity > 0 Then
            totalScore += result.IsotopeSimilarity
        End If
        If result.WeightedDotProduct > 0 Then
            totalScore += (result.WeightedDotProduct + result.SimpleDotProduct + result.ReverseDotProduct) / 3.0
        End If
        If result.MatchedPeaksPercentage > 0 Then
            totalScore += result.MatchedPeaksPercentage
        End If
        If result.AndromedaScore > 0 Then
            totalScore += result.AndromedaScore
        End If
        Return totalScore
    End Function

    Public Shared Function GetTotalSimilarity(rtSimilarity As Double, eiSimilarity As Double, Optional isUseRT As Boolean = True) As Double
        If rtSimilarity < 0 OrElse Not isUseRT Then
            Return eiSimilarity
        Else
            Return 0.6 * eiSimilarity + 0.4 * rtSimilarity
        End If
    End Function
End Class

