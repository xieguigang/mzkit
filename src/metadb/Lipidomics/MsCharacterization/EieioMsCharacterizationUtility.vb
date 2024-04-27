#Region "Microsoft.VisualBasic::1829bf35cafc02de8412de08ef3bdd55, G:/mzkit/src/metadb/Lipidomics//MsCharacterization/EieioMsCharacterizationUtility.vb"

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

    '   Total Lines: 400
    '    Code Lines: 318
    ' Comment Lines: 20
    '   Blank Lines: 62
    '     File Size: 22.45 KB


    ' Class LipidMsCharacterizationResult
    ' 
    '     Properties: ChainIonScore, ChainIonsDetected, ClassIonScore, ClassIonsDetected, DoubleBondIonScore
    '                 DoubleBondIonsDetected, DoubleBondMatchedPercent, IsChainIonsExisted, IsClassIonsExisted, IsDoubleBondIonsExisted
    '                 IsPositionIonsExisted, PositionIonScore, PositionIonsDetected, TotalMatchedIonCount, TotalScore
    ' 
    ' Class DiagnosticIon
    ' 
    '     Properties: IonAbundanceCutOff, Mz, MzTolerance
    ' 
    ' Module StandardMsCharacterizationUtility
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CountDetectedIons, GetDefaultCharacterizationResultForAlkylAcylGlycerols, GetDefaultCharacterizationResultForCeramides, GetDefaultCharacterizationResultForGlycerophospholipid, GetDefaultCharacterizationResultForSingleAcylChainLipid
    '               GetDefaultCharacterizationResultForTriacylGlycerols, GetMatchedCoefficient, IsDiagnosticFragmentExist, IsDiagnosticFragmentExist_ResolutionUsed4Intensity, (+2 Overloads) IsDiagnosticFragmentsExist
    ' 
    ' Module OadMsCharacterizationUtility
    ' 
    '     Function: GetDefaultScore
    ' 
    ' Module EieioMsCharacterizationUtility
    ' 
    '     Function: GetDefaultScore
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep
Imports BioNovoGene.BioDeep.MSEngine
Imports std = System.Math
Public Class LipidMsCharacterizationResult
    Public Property ClassIonsDetected As Integer
    Public Property ChainIonsDetected As Integer
    Public Property PositionIonsDetected As Integer
    Public Property DoubleBondIonsDetected As Integer
    Public Property DoubleBondMatchedPercent As Double
    Public Property IsClassIonsExisted As Boolean
    Public Property IsChainIonsExisted As Boolean
    Public Property IsPositionIonsExisted As Boolean
    Public Property IsDoubleBondIonsExisted As Boolean
    Public Property ClassIonScore As Double
    Public Property ChainIonScore As Double
    Public Property PositionIonScore As Double
    Public Property DoubleBondIonScore As Double
    Public Property TotalMatchedIonCount As Double
    Public Property TotalScore As Double
End Class

Public Class DiagnosticIon
    Public Property Mz As Double
    Public Property MzTolerance As Double
    Public Property IonAbundanceCutOff As Double
End Class

Public Module StandardMsCharacterizationUtility
    Private ReadOnly SPECIES_LEVEL, POSITION_AND_DOUBLEBOND_LEVEL, POSITION_LEVEL, MOLECULAR_SPECIES_AND_DOUBLEBOND_LEVEL, MOLECULAR_SPECIES_LEVEL, CERAMIDE_POSITION_LEVEL As IVisitor(Of ILipid, ILipid)

    Sub New()
        Dim builder = New LipidConverterBuilder()
        Dim director = New ShorthandNotationDirector(builder)
        director.SetSpeciesLevel()
        SPECIES_LEVEL = builder.Create()

        director.SetPositionLevel()
        director.SetDoubleBondPositionLevel()
        director.SetOxidizedPositionLevel()
        POSITION_AND_DOUBLEBOND_LEVEL = builder.Create()

        director.SetPositionLevel()
        director.SetDoubleBondNumberLevel()
        director.SetOxidizedNumberLevel()
        POSITION_LEVEL = builder.Create()

        director.SetMolecularSpeciesLevel()
        director.SetDoubleBondPositionLevel()
        director.SetOxidizedPositionLevel()
        MOLECULAR_SPECIES_AND_DOUBLEBOND_LEVEL = builder.Create()

        director.SetMolecularSpeciesLevel()
        director.SetDoubleBondNumberLevel()
        director.SetOxidizedNumberLevel()
        MOLECULAR_SPECIES_LEVEL = builder.Create()

        director.SetPositionLevel()
        director.SetDoubleBondNumberLevel()
        director.SetOxidizedNumberLevel()
        CType(builder, ILipidomicsVisitorBuilder).SetSphingoOxidized(OxidizedIndeterminateState.Identity)
        CERAMIDE_POSITION_LEVEL = builder.Create()
    End Sub

    Public Function GetDefaultCharacterizationResultForAlkylAcylGlycerols(molecule As ILipid, result As LipidMsCharacterizationResult) As (ILipid, Double())
        Dim converter As IVisitor(Of ILipid, ILipid)
        If Not result.IsChainIonsExisted Then ' chain existed expected: PC O-36:2
            converter = SPECIES_LEVEL
        ElseIf result.IsPositionIonsExisted AndAlso result.IsDoubleBondIonsExisted Then ' chain existed expected: PC O-18:0/18:2(9,12)
            converter = POSITION_AND_DOUBLEBOND_LEVEL
        ElseIf result.IsPositionIonsExisted Then ' chain existed expected: PC O-18:0/18:2
            converter = POSITION_LEVEL
        ElseIf result.IsDoubleBondIonsExisted Then ' chain existed expected: PC O-18:0_18:2(9,12)
            converter = MOLECULAR_SPECIES_AND_DOUBLEBOND_LEVEL ' chain existed expected: PC O-18:0_18:2
        Else
            converter = MOLECULAR_SPECIES_LEVEL
        End If
        Return (molecule.Accept(converter, IdentityDecomposer(Of ILipid, ILipid).Instance), New Double(1) {result.TotalScore, result.TotalMatchedIonCount})
    End Function

    Public Function GetDefaultCharacterizationResultForCeramides(molecule As ILipid, result As LipidMsCharacterizationResult) As (ILipid, Double())
        Dim converter As IVisitor(Of ILipid, ILipid)
        If Not result.IsChainIonsExisted Then ' chain cannot determine
            converter = SPECIES_LEVEL
        ElseIf Not result.IsDoubleBondIonsExisted Then ' chain existed expected: SM 18:1;2O/18:1
            converter = CERAMIDE_POSITION_LEVEL
        Else
            converter = POSITION_AND_DOUBLEBOND_LEVEL
        End If
        Return (molecule.Accept(converter, IdentityDecomposer(Of ILipid, ILipid).Instance), New Double(1) {result.TotalScore, result.TotalMatchedIonCount})
    End Function

    Public Function GetDefaultCharacterizationResultForGlycerophospholipid(molecule As ILipid, result As LipidMsCharacterizationResult) As (ILipid, Double())
        Dim converter As IVisitor(Of ILipid, ILipid)
        If Not result.IsChainIonsExisted Then ' chain existed expected: PC 36:2
            converter = SPECIES_LEVEL
        ElseIf result.IsPositionIonsExisted AndAlso result.IsDoubleBondIonsExisted Then ' chain existed expected: PC 18:0/18:2(9,12)
            converter = POSITION_AND_DOUBLEBOND_LEVEL
        ElseIf result.IsPositionIonsExisted Then ' chain existed expected: PC 18:0/18:2
            converter = POSITION_LEVEL
        ElseIf result.IsDoubleBondIonsExisted Then ' chain existed expected: PC 18:0_18:2(9,12)
            converter = MOLECULAR_SPECIES_AND_DOUBLEBOND_LEVEL ' chain existed expected: PC 18:0_18:2
        Else
            converter = MOLECULAR_SPECIES_LEVEL
        End If
        Return (molecule.Accept(converter, IdentityDecomposer(Of ILipid, ILipid).Instance), New Double(1) {result.TotalScore, result.TotalMatchedIonCount})
    End Function

    Public Function GetDefaultCharacterizationResultForTriacylGlycerols(molecule As ILipid, result As LipidMsCharacterizationResult) As (ILipid, Double())
        Dim converter As IVisitor(Of ILipid, ILipid)
        If Not result.IsChainIonsExisted Then ' chain existed expected: TG 52:3
            converter = SPECIES_LEVEL
        ElseIf result.IsPositionIonsExisted AndAlso result.IsDoubleBondIonsExisted Then ' chain existed expected: TG 16:0/18:1(11)/18:2(9,12)
            converter = POSITION_AND_DOUBLEBOND_LEVEL
        ElseIf result.IsPositionIonsExisted Then ' chain existed expected: TG 16:0_18:1(sn2)_18:2 for TG, HBMP/DCL 16:0/18:0_20:4
            converter = POSITION_LEVEL
        ElseIf result.IsDoubleBondIonsExisted Then ' chain existed expected:TG 16:0_18:1(11)_18:2(9,12)
            converter = MOLECULAR_SPECIES_AND_DOUBLEBOND_LEVEL ' chain existed expected: TG 16:0_18:1_18:2
        Else
            converter = MOLECULAR_SPECIES_LEVEL
        End If
        Return (molecule.Accept(converter, IdentityDecomposer(Of ILipid, ILipid).Instance), New Double(1) {result.TotalScore, result.TotalMatchedIonCount})
    End Function

    Public Function GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule As ILipid, result As LipidMsCharacterizationResult) As (ILipid, Double()) ' CAR, steroidal ether etc.
        Dim converter = SPECIES_LEVEL
        If result.IsDoubleBondIonsExisted Then
            converter = MOLECULAR_SPECIES_AND_DOUBLEBOND_LEVEL
            If molecule.Chains.OxidizedCount > 0 Then 'TBC
                converter = MOLECULAR_SPECIES_LEVEL
            End If
        End If
        Return (molecule.Accept(converter, IdentityDecomposer(Of ILipid, ILipid).Instance), New Double(1) {result.TotalScore, result.TotalMatchedIonCount})
    End Function

    Public Function GetMatchedCoefficient(peaks As List(Of SpectrumPeak)) As Double
        Dim sum1 As Double = 0, sum2 As Double = 0, mean1 As Double = 0, mean2 As Double = 0, covariance As Double = 0, sqrt1 As Double = 0, sqrt2 As Double = 0, counter As Double = 0
        For i = 0 To peaks.Count - 1
            If Not peaks(i).IsMatched Then Continue For
            counter += 1
            sum1 += peaks(i).Resolution
            sum2 += peaks(i).Intensity
        Next
        If counter = 0 Then Return 0
        mean1 = sum1 / counter
        mean2 = sum2 / counter

        For i = 0 To peaks.Count - 1
            If Not peaks(i).IsMatched Then Continue For
            covariance += (peaks(i).Resolution - mean1) * (peaks(i).Intensity - mean2)
            sqrt1 += std.Pow(peaks(i).Resolution - mean1, 2)
            sqrt2 += std.Pow(peaks(i).Intensity - mean2, 2)
        Next
        If sqrt1 = 0 OrElse sqrt2 = 0 Then
            Return 0
        Else
            Return covariance / std.Sqrt(sqrt1 * sqrt2)
        End If
    End Function

    Public Function IsDiagnosticFragmentsExist(spectrum As IReadOnlyList(Of SpectrumPeak), refSpectrum As IReadOnlyList(Of SpectrumPeak), mzTolerance As Double) As Boolean
        Dim isAllExisted = True
        If refSpectrum.IsNullOrEmpty Then Return True
        For Each refpeak In refSpectrum
            If Not IsDiagnosticFragmentExist(spectrum, mzTolerance, refpeak.mz, refpeak.Intensity * 0.01) Then
                isAllExisted = False
                Exit For
            End If
        Next
        Return isAllExisted
    End Function

    Public Function IsDiagnosticFragmentsExist(spectrum As IReadOnlyList(Of SpectrumPeak), dIons As IReadOnlyList(Of DiagnosticIon)) As Boolean
        Dim missedCounter = 0
        Dim isAllExisted = True
        If dIons.IsNullOrEmpty Then Return True
        For Each ion In dIons
            If Not IsDiagnosticFragmentExist_ResolutionUsed4Intensity(spectrum, ion.MzTolerance, ion.Mz, ion.IonAbundanceCutOff) Then
                missedCounter += 1
                If dIons.Count < 4 AndAlso missedCounter = 1 Then
                    isAllExisted = False
                    Exit For
                End If
                If dIons.Count >= 4 AndAlso missedCounter = 2 Then
                    isAllExisted = False
                    Exit For
                End If
            End If
        Next
        Return isAllExisted
    End Function

    Public Function IsDiagnosticFragmentExist(spectrum As IReadOnlyList(Of SpectrumPeak), mzTolerance As Double, diagnosticMz As Double, threshold As Double) As Boolean
        For i = 0 To spectrum.Count - 1
            Dim mz = spectrum(i).mz
            Dim intensity = spectrum(i).Intensity ' should be normalized by max intensity to 100

            If intensity > threshold AndAlso std.Abs(mz - diagnosticMz) < mzTolerance Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function IsDiagnosticFragmentExist_ResolutionUsed4Intensity(spectrum As IReadOnlyList(Of SpectrumPeak), mzTolerance As Double, diagnosticMz As Double, threshold As Double) As Boolean
        For i = 0 To spectrum.Count - 1
            Dim mz = spectrum(i).mz
            Dim intensity = spectrum(i).Resolution ' should be normalized by max intensity to 100

            If intensity > threshold AndAlso std.Abs(mz - diagnosticMz) < mzTolerance Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function CountDetectedIons(exp_spectrum As List(Of SpectrumPeak), ref_spectrum As List(Of SpectrumPeak), tolerance As Double) As Integer
        Dim ionDetectedCounter = 0
        For Each ion In ref_spectrum
            If IsDiagnosticFragmentExist(exp_spectrum, tolerance, ion.mz, ion.Intensity * 0.0001) Then
                ionDetectedCounter += 1
            End If
        Next
        Return ionDetectedCounter
    End Function
End Module

Public Module OadMsCharacterizationUtility
    Public Function GetDefaultScore(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single, classIonCutoff As Double, chainIonCutoff As Double, positionIonCutoff As Double, doublebondIonCutoff As Double) As LipidMsCharacterizationResult

        Dim exp_spectrum = scan.Spectrum
        Dim ref_spectrum = reference.Spectrum
        Dim adduct = reference.AdductType

        Dim result = New LipidMsCharacterizationResult()

        Dim matchedpeaks = MSEngine.MsScanMatching.GetMachedSpectralPeaks(exp_spectrum, ref_spectrum, tolerance, mzBegin, mzEnd)

        ' check lipid class ion's existence
        Dim classions = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.metaboliteclass)).ToList()
        Dim isClassMustIonsExisted = classions.All(Function(ion) Not ion.IsAbsolutelyRequiredFragmentForAnnotation OrElse ion.IsMatched)
        Dim classionsDetected = classions.Where(Function(n) n.IsMatched).Count
        Dim isClassIonExisted = If(isClassMustIonsExisted AndAlso classionsDetected >= classIonCutoff, True, False)

        result.ClassIonsDetected = classionsDetected
        result.IsClassIonsExisted = isClassIonExisted


        ' check lipid chain ion's existence
        Dim chainIons = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.acylchain)).ToList()
        Dim isChainMustIonsExisted = chainIons.All(Function(ion) Not ion.IsAbsolutelyRequiredFragmentForAnnotation OrElse ion.IsMatched)
        Dim chainIonsDetected = chainIons.Where(Function(n) n.IsMatched).Count
        Dim isChainIonExisted = If(isChainMustIonsExisted AndAlso chainIonsDetected >= chainIonCutoff, True, False)

        result.ChainIonsDetected = chainIonsDetected
        result.IsChainIonsExisted = isChainIonExisted

        ' check lipid position ion's existence
        'var positionIons = matchedpeaks.Where(n => n.SpectrumComment.HasFlag(SpectrumComment.snposition)).ToList();
        'var isPositionMustIonsExisted = positionIons.All(ion => !ion.IsAbsolutelyRequiredFragmentForAnnotation || ion.IsMatched); 
        'var positionIonsDetected = positionIons.Count(n => n.IsMatched);
        'var isPositionIonExisted = isPositionMustIonsExisted && positionIonsDetected >= positionIonCutoff
        '    ? true : false;
        Dim positionIonsDetected = 0
        Dim isPositionIonExisted = False

        result.PositionIonsDetected = positionIonsDetected
        result.IsPositionIonsExisted = isPositionIonExisted

        ' check the dtected ion nudouble bond position
        Dim doublebondIons = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.doublebond)).ToList()
        Dim doublebondIons_matched = doublebondIons.Where(Function(n) n.IsMatched).ToList()
        Dim matchedCount = doublebondIons_matched.Count
        Dim matchedPercent = matchedCount / (doublebondIons.Count + 0.0000000001)
        Dim matchedCoefficient = GetMatchedCoefficient(doublebondIons_matched)

        Dim essentialDBIons = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.doublebond_high)).ToList()
        Dim essentialDBIons_matched = essentialDBIons.Where(Function(n) n.IsMatched).ToList()
        If essentialDBIons.Count = essentialDBIons_matched.Count Then
            matchedCoefficient += 1.5
        End If

        Dim isDoubleBondIdentified = If(essentialDBIons.Count = essentialDBIons_matched.Count, True, False)

        result.DoubleBondIonsDetected = matchedCount
        result.DoubleBondMatchedPercent = matchedPercent
        result.IsDoubleBondIonsExisted = isDoubleBondIdentified

        ' total score
        result.ClassIonScore = If(isClassIonExisted, 1.0, 0.0)
        result.ChainIonScore = If(isChainIonExisted, 1.0, 0.0)
        result.PositionIonScore = If(isPositionIonExisted, 1.0, 0.0)
        result.DoubleBondIonScore = matchedPercent + matchedCoefficient

        Dim score = result.ClassIonScore + result.ChainIonScore + result.PositionIonScore + result.DoubleBondIonScore
        Dim counter = classionsDetected + chainIonsDetected + positionIonsDetected + matchedCount
        result.TotalScore = score
        result.TotalMatchedIonCount = counter

        Return result
    End Function
End Module

Public Module EieioMsCharacterizationUtility
    Public Function GetDefaultScore(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single, classIonCutoff As Double, chainIonCutoff As Double, positionIonCutoff As Double, doublebondIonCutoff As Double, Optional dIons4class As IReadOnlyList(Of DiagnosticIon) = Nothing, Optional dIons4chain As IReadOnlyList(Of DiagnosticIon) = Nothing, Optional dIons4position As IReadOnlyList(Of DiagnosticIon) = Nothing, Optional dIons4db As IReadOnlyList(Of DiagnosticIon) = Nothing) As LipidMsCharacterizationResult

        Dim exp_spectrum = scan.Spectrum
        Dim ref_spectrum = reference.Spectrum
        Dim adduct = reference.AdductType

        Dim result = New LipidMsCharacterizationResult()

        Dim matchedpeaks = MSEngine.MsScanMatching.GetMachedSpectralPeaks(exp_spectrum, ref_spectrum, tolerance, mzBegin, mzEnd)

        ' check lipid class ion's existence
        Dim classions = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.metaboliteclass)).ToList()
        Dim isClassMustIonsExisted = classions.All(Function(ion) Not ion.IsAbsolutelyRequiredFragmentForAnnotation OrElse ion.IsMatched)
        Dim isClassAdvancedFilter = IsDiagnosticFragmentsExist(classions, dIons4class)
        Dim classions_matched = classions.Where(Function(n) n.IsMatched).ToList()
        Dim classionsDetected = classions_matched.Count()
        Dim isClassIonExisted = If(isClassMustIonsExisted AndAlso isClassAdvancedFilter AndAlso classionsDetected >= classIonCutoff, True, False)

        result.ClassIonsDetected = classionsDetected
        result.IsClassIonsExisted = isClassIonExisted
        'result.ClassIonScore = isClassIonExisted ? classions_matched.Sum(n => n.Resolution) / 100.0 : 0.0;
        result.ClassIonScore = If(isClassIonExisted, classions_matched.Count / classions.Count, 0.0)


        ' check lipid chain ion's existence
        Dim chainIons = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.acylchain)).ToList()
        Dim isChainMustIonsExisted = chainIons.All(Function(ion) Not ion.IsAbsolutelyRequiredFragmentForAnnotation OrElse ion.IsMatched)
        Dim isChainAdvancedFilter = IsDiagnosticFragmentsExist(chainIons, dIons4chain)
        Dim chainIons_matched = chainIons.Where(Function(n) n.IsMatched).ToList()
        Dim chainIonsDetected = chainIons_matched.Count()
        Dim isChainIonExisted = If(isChainMustIonsExisted AndAlso isChainAdvancedFilter AndAlso chainIonsDetected >= chainIonCutoff, True, False)

        result.ChainIonsDetected = chainIonsDetected
        result.IsChainIonsExisted = isChainIonExisted
        'result.ChainIonScore = isChainIonExisted ? chainIons_matched.Sum(n => n.Resolution) / 100.0 : 0.0;
        result.ChainIonScore = If(isChainIonExisted, chainIons_matched.Count / chainIons.Count, 0.0)

        ' check lipid position ion's existence
        Dim isPositionIonExisted = False
        Dim positionIonsDetected = 0
        If positionIonCutoff > 0 Then
            Dim positionIons = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.snposition)).ToList()
            Dim isPositionMustIonsExisted = positionIons.All(Function(ion) Not ion.IsAbsolutelyRequiredFragmentForAnnotation OrElse ion.IsMatched)
            Dim isPositionAdvancedFilter = IsDiagnosticFragmentsExist(positionIons, dIons4position)
            Dim positionIons_matched = positionIons.Where(Function(n) n.IsMatched).ToList()

            positionIonsDetected = positionIons_matched.Count()
            isPositionIonExisted = If(isPositionMustIonsExisted AndAlso isPositionAdvancedFilter AndAlso positionIonsDetected >= positionIonCutoff, True, False)
            result.PositionIonsDetected = positionIonsDetected
            result.IsPositionIonsExisted = isPositionIonExisted
            'result.PositionIonScore = isPositionIonExisted ? positionIons_matched.Sum(n => n.Resolution) / 100.0 : 0.0;
            result.PositionIonScore = If(isPositionIonExisted, positionIons_matched.Count / positionIons.Count, 0.0)
        End If

        ' check the dtected ion nudouble bond position
        Dim doublebondIons = matchedpeaks.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.doublebond)).ToList()
        Dim doublebondIons_matched = doublebondIons.Where(Function(n) n.IsMatched).ToList()

        Dim doublebondHighIons = ref_spectrum.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.doublebond_high)).[Select](Function(n) New DiagnosticIon() With {
.Mz = n.mz,
.IonAbundanceCutOff = 0.0000001,
.MzTolerance = tolerance
}).ToList()
        Dim doublebondHighAndLowIons = ref_spectrum.Where(Function(n) n.SpectrumComment.HasFlag(SpectrumComment.doublebond_high) OrElse n.SpectrumComment.HasFlag(SpectrumComment.doublebond_low)).[Select](Function(n) New DiagnosticIon() With {
.Mz = n.mz,
.IonAbundanceCutOff = 0.0000001,
.MzTolerance = tolerance
}).ToList()

        Dim isDoublebondAdvancedFilter = IsDiagnosticFragmentsExist(doublebondIons_matched, doublebondHighIons)
        'var isDoublebondAdvancedFilter = StandardMsCharacterizationUtility.IsDiagnosticFragmentsExist(doublebondIons_matched, doublebondHighAndLowIons);
        'var isDoublebondAdvancedFilter = StandardMsCharacterizationUtility.IsDiagnosticFragmentsExist(doublebondIons_matched, dIons4db);
        Dim matchedCount = doublebondIons_matched.Count
        Dim matchedPercent = matchedCount / (doublebondIons.Count + 0.0000000001)
        Dim matchedCoefficient = GetMatchedCoefficient(doublebondIons_matched)

        Dim isDoubleBondIdentified = If(isDoublebondAdvancedFilter AndAlso matchedPercent > doublebondIonCutoff * 0.5, True, False)

        result.DoubleBondIonsDetected = matchedCount
        result.DoubleBondMatchedPercent = matchedPercent
        result.IsDoubleBondIonsExisted = isDoubleBondIdentified
        result.DoubleBondIonScore = If(isDoubleBondIdentified, matchedCoefficient + matchedPercent, 0)

        ' total score

        Dim score = result.ClassIonScore + result.ChainIonScore + result.PositionIonScore + result.DoubleBondIonScore
        Dim counter = classionsDetected + chainIonsDetected + positionIonsDetected + matchedCount
        result.TotalScore = score
        result.TotalMatchedIonCount = counter

        Return result

    End Function

End Module


