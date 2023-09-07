Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports BioNovoGene.BioDeep.MSEngine
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

    Private Shared Function IsComparedAvailable(Of T)(ByVal obj1 As IReadOnlyCollection(Of T), ByVal obj2 As IReadOnlyCollection(Of T)) As Boolean
        If obj1 Is Nothing OrElse obj2 Is Nothing OrElse obj1.Count = 0 OrElse obj2.Count = 0 Then Return False
        Return True
    End Function

    Private Shared Function IsComparedAvailable(ByVal obj1 As IMSScanProperty, ByVal obj2 As IMSScanProperty) As Boolean
        If obj1.Spectrum Is Nothing OrElse obj2.Spectrum Is Nothing OrElse obj1.Spectrum.Count = 0 OrElse obj2.Spectrum.Count = 0 Then Return False
        Return True
    End Function

    Public Shared Function GetEieioBasedLipidomicsMatchedPeaksScores(ByVal scan As IMSScanProperty, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As Double()

        Dim returnedObj = GetEieioBasedLipidMoleculeAnnotationResult(scan, reference, tolerance, mzBegin, mzEnd)
        Return returnedObj.Item2
    End Function

    Public Shared Function GetEieioBasedLipidMoleculeAnnotationResult(ByVal scan As IMSScanProperty, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())
        Dim lipid = FacadeLipidParser.Default.Parse(reference.Name)
        Select Case lipid.LipidClass
            Case LbmClass.PC
                Return PCEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PE
                Return PEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PS
                Return PSEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PG
                Return PGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PI
                Return PIEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PA
                Return PAEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.DG
                Return DGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.BMP
                Return BMPEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPC
                Return LPCEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPS
                Return LPSEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPE
                Return LPEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPG
                Return LPGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPI
                Return LPIEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.DGTA
                Return DGTAEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.DGTS
                Return DGTSEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LDGTA
                Return LDGTAEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LDGTS
                Return LDGTSEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.SM
                Return SMEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.Cer_NS, LbmClass.Cer_NDS, LbmClass.Cer_AS, LbmClass.Cer_ADS, LbmClass.Cer_BS, LbmClass.Cer_BDS, LbmClass.Cer_NP, LbmClass.Cer_AP
                Return CeramideEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.HexCer_NS
                Return HexCerEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.Hex2Cer
                Return Hex2CerEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.HBMP
                Return HBMPEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.TG
                Return TGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.EtherPC
                Return EtherPCEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.EtherPE

                Return EtherPEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.SHexCer
                Return SHexCerEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.GM3
                Return GM3EadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.CE
                Return CEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.MG
                Return MGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.CAR
                Return CAREadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.DMEDFAHFA
                Return DMEDFAHFAEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.DMEDFA, LbmClass.DMEDOxFA
                Return DMEDFAEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.PC_d5
                Return PCEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PE_d5
                Return PEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PS_d5
                Return PSEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PG_d5
                Return PGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.PI_d5
                Return PIEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.Cer_NS_d7
                Return CeramideEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.SM_d9
                Return SMEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.TG_d5
                Return TGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.CE_d7
                Return CEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.DG_d5
                Return DGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPC_d5
                Return LPCEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPS_d5
                Return LPSEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPE_d5
                Return LPEEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPG_d5
                Return LPGEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.LPI_d5
                Return LPIEadMsCharacterization.Characterize(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case Else


                Return (Nothing, New Double(1) {0.0, 0.0})
        End Select
    End Function

    Public Shared Function GetEidBasedLipidomicsMatchedPeaksScores(ByVal scan As IMSScanProperty, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As Double()

        Dim returnedObj = GetEidBasedLipidMoleculeAnnotationResult(scan, reference, tolerance, mzBegin, mzEnd)
        Return returnedObj.Item2
    End Function

    Public Shared Function GetEidBasedLipidMoleculeAnnotationResult(ByVal scan As IMSScanProperty, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())
        Dim lipid = FacadeLipidParser.Default.Parse(reference.Name)
        Select Case lipid.LipidClass
            Case LbmClass.PC, LbmClass.PE, LbmClass.PS, LbmClass.PG, LbmClass.PI, LbmClass.PA, LbmClass.DG, LbmClass.BMP, LbmClass.LPC, LbmClass.LPS, LbmClass.LPE, LbmClass.LPG, LbmClass.LPI, LbmClass.DGTA, LbmClass.DGTS, LbmClass.LDGTA, LbmClass.LDGTS, LbmClass.DMEDFAHFA, LbmClass.PC_d5, LbmClass.PE_d5, LbmClass.PS_d5, LbmClass.PG_d5, LbmClass.PI_d5, LbmClass.LPC_d5, LbmClass.LPE_d5, LbmClass.LPS_d5, LbmClass.LPG_d5, LbmClass.LPI_d5, LbmClass.DG_d5
                Return EidDefaultCharacterization.Characterize4DiacylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.MG
                Return EidDefaultCharacterization.Characterize4MonoacylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.CAR, LbmClass.DMEDFA, LbmClass.DMEDOxFA, LbmClass.CE, LbmClass.CE_d7
                Return EidDefaultCharacterization.Characterize4SingleAcylChain(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.SM, LbmClass.Cer_NS, LbmClass.Cer_NDS, LbmClass.Cer_AS, LbmClass.Cer_ADS, LbmClass.Cer_BS, LbmClass.Cer_BDS, LbmClass.Cer_NP, LbmClass.Cer_AP, LbmClass.HexCer_NS, LbmClass.SHexCer, LbmClass.GM3, LbmClass.SM_d9, LbmClass.Cer_NS_d7
                Return EidDefaultCharacterization.Characterize4Ceramides(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.HBMP, LbmClass.TG, LbmClass.TG_d5
                Return EidDefaultCharacterization.Characterize4TriacylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.EtherPC, LbmClass.EtherPE
                Return EidDefaultCharacterization.Characterize4AlkylAcylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case Else

                Return (Nothing, New Double(1) {0.0, 0.0})
        End Select
    End Function


    Public Shared Function GetOadBasedLipidomicsMatchedPeaksScores(ByVal scan As IMSScanProperty, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As Double()

        Dim returnedObj = GetOadBasedLipidMoleculeAnnotationResult(scan, reference, tolerance, mzBegin, mzEnd)
        Return returnedObj.Item2
    End Function

    Public Shared Function GetOadBasedLipidMoleculeAnnotationResult(ByVal scan As IMSScanProperty, ByVal reference As MoleculeMsReference, ByVal tolerance As Single, ByVal mzBegin As Single, ByVal mzEnd As Single) As (ILipid, Double())
        Dim lipid = FacadeLipidParser.Default.Parse(reference.Name)
        Select Case lipid.LipidClass
            Case LbmClass.PC, LbmClass.PE, LbmClass.PS, LbmClass.PG, LbmClass.PI, LbmClass.PA, LbmClass.DG, LbmClass.BMP, LbmClass.LPC, LbmClass.LPS, LbmClass.LPE, LbmClass.LPG, LbmClass.LPI, LbmClass.DGTA, LbmClass.DGTS, LbmClass.LDGTA, LbmClass.LDGTS, LbmClass.DMEDFAHFA
                Return OadDefaultCharacterization.Characterize4DiacylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.SM, LbmClass.Cer_NS, LbmClass.Cer_NDS, LbmClass.Cer_AS, LbmClass.Cer_ADS, LbmClass.Cer_BS, LbmClass.Cer_BDS, LbmClass.Cer_NP, LbmClass.Cer_AP, LbmClass.HexCer_NS
                Return OadDefaultCharacterization.Characterize4Ceramides(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.HBMP, LbmClass.TG
                Return OadDefaultCharacterization.Characterize4TriacylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.EtherPC, LbmClass.EtherPE
                Return OadDefaultCharacterization.Characterize4AlkylAcylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.EtherLPC
                Return OadDefaultCharacterization.Characterize4AlkylAcylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.EtherLPE
                Return OadDefaultCharacterization.Characterize4AlkylAcylGlycerols(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.SHexCer
                Return OadDefaultCharacterization.Characterize4Ceramides(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.GM3
                Return OadDefaultCharacterization.Characterize4Ceramides(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)

            Case LbmClass.MG
                Return OadDefaultCharacterization.Characterize4SingleAcylChainLiipid(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case LbmClass.CAR, LbmClass.DMEDFA, LbmClass.DMEDOxFA
                Return OadDefaultCharacterization.Characterize4SingleAcylChainLiipid(scan, CType(lipid, Lipid), reference, tolerance, mzBegin, mzEnd)
            Case Else

                Return (Nothing, New Double(1) {0.0, 0.0})
        End Select
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

    Public Shared Function CompareMS2ScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As MoleculeMsReference, ByVal param As MsRefSearchParameterBase, ByVal Optional targetOmics As TargetOmics = TargetOmics.Metabolomics, ByVal Optional scanCCS As Double = -1.0, ByVal Optional scanIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, ByVal Optional refIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, ByVal Optional andromedaDelta As Double = 100, ByVal Optional andromedaMaxPeaks As Integer = 12) As MsScanMatchResult

        Dim result As MsScanMatchResult = Nothing
        If targetOmics = targetOmics.Metabolomics Then
            result = CompareMSScanProperties(scanProp, refSpec, param, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        ElseIf targetOmics = targetOmics.Lipidomics Then
            result = CompareMS2LipidomicsScanProperties(scanProp, refSpec, param)
        End If

        result.IsotopeSimilarity = CSng(GetIsotopeRatioSimilarity(scanIsotopes, refIsotopes, scanProp.PrecursorMz, param.Ms1Tolerance))

        Dim isCcsMatch = False
        Dim ccsSimilarity = GetGaussianSimilarity(scanCCS, refSpec.CollisionCrossSection, param.CcsTolerance, isCcsMatch)

        result.CcsSimilarity = CSng(ccsSimilarity)
        result.IsCcsMatch = isCcsMatch
        result.TotalScore = CSng(GetTotalScore(result, param))
        Return result
    End Function

    Public Shared Function CompareMS2ScanProperties(ByVal scanProp As IMSScanProperty, ByVal chargestate As Integer, ByVal refSpec As PeptideMsReference, ByVal param As MsRefSearchParameterBase, ByVal Optional targetOmics As TargetOmics = TargetOmics.Metabolomics, ByVal Optional scanCCS As Double = -1.0, ByVal Optional scanIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, ByVal Optional refIsotopes As IReadOnlyList(Of IsotopicPeak) = Nothing, ByVal Optional andromedaDelta As Double = 100, ByVal Optional andromedaMaxPeaks As Integer = 12) As MsScanMatchResult

        Dim result As MsScanMatchResult = Nothing
        If targetOmics = targetOmics.Proteomics Then
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

    Public Shared Function CompareMS2LipidomicsScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As MoleculeMsReference, ByVal param As MsRefSearchParameterBase) As MsScanMatchResult

        Dim result = CompareBasicMSScanProperties(scanProp, refSpec, param, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        Dim matchedPeaksScores = GetLipidomicsMatchedPeaksScores(scanProp, refSpec, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        result.MatchedPeaksCount = CSng(matchedPeaksScores(1))
        result.MatchedPeaksPercentage = CSng(matchedPeaksScores(0))

        If result.WeightedDotProduct >= param.WeightedDotProductCutOff AndAlso result.SimpleDotProduct >= param.SimpleDotProductCutOff AndAlso result.ReverseDotProduct >= param.ReverseDotProductCutOff AndAlso result.MatchedPeaksPercentage >= param.MatchedPeaksPercentageCutOff AndAlso result.MatchedPeaksCount >= param.MinimumSpectrumMatch Then
            result.IsSpectrumMatch = True
        End If

        Dim isLipidClassMatch = False
        Dim isLipidChainsMatch = False
        Dim isLipidPositionMatch = False
        Dim isOtherLipidMatch = False

        Dim name = GetRefinedLipidAnnotationLevel(scanProp, refSpec, param.Ms2Tolerance, isLipidClassMatch, isLipidChainsMatch, isLipidPositionMatch, isOtherLipidMatch)
        If Equals(name, String.Empty) Then Return Nothing

        result.Name = name
        result.IsLipidChainsMatch = isLipidChainsMatch
        result.IsLipidClassMatch = isLipidClassMatch
        result.IsLipidPositionMatch = isLipidPositionMatch
        result.IsOtherLipidMatch = isOtherLipidMatch
        result.TotalScore = CSng(GetTotalScore(result, param))

        Return result
    End Function

    Public Shared Function CompareMS2ProteomicsScanProperties(ByVal scanProp As IMSScanProperty, ByVal chargestate As Integer, ByVal refSpec As PeptideMsReference, ByVal param As MsRefSearchParameterBase, ByVal andromedaDelta As Single, ByVal andromedaMaxPeaks As Single) As MsScanMatchResult

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


    Public Shared Function CompareEIMSScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As MoleculeMsReference, ByVal param As MsRefSearchParameterBase, ByVal Optional isUseRetentionIndex As Boolean = False) As MsScanMatchResult
        Dim result = CompareMSScanProperties(scanProp, refSpec, param, param.Ms1Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        Dim msMatchedScore = GetIntegratedSpectraSimilarity(result)
        If isUseRetentionIndex Then
            result.TotalScore = CSng(GetTotalSimilarity(result.RiSimilarity, msMatchedScore, param.IsUseTimeForAnnotationScoring))
        Else
            result.TotalScore = CSng(GetTotalSimilarity(result.RtSimilarity, msMatchedScore, param.IsUseTimeForAnnotationScoring))
        End If
        Return result
    End Function

    Public Shared Function CompareEIMSScanProperties(ByVal scan1 As IMSScanProperty, ByVal scan2 As IMSScanProperty, ByVal param As MsRefSearchParameterBase, ByVal Optional isUseRetentionIndex As Boolean = False) As MsScanMatchResult
        Dim result = CompareMSScanProperties(scan1, scan2, param, param.Ms1Tolerance, param.MassRangeBegin, param.MassRangeEnd)
        Dim msMatchedScore = GetIntegratedSpectraSimilarity(result)
        If isUseRetentionIndex Then
            result.TotalScore = CSng(GetTotalSimilarity(result.RiSimilarity, msMatchedScore))
        Else
            result.TotalScore = CSng(GetTotalSimilarity(result.RtSimilarity, msMatchedScore))
        End If
        Return result
    End Function

    Public Shared Function GetIntegratedSpectraSimilarity(ByVal result As MsScanMatchResult) As Double
        Dim dotproductFact = 3.0
        Dim revDotproductFact = 2.0
        Dim matchedRatioFact = 1.0
        Return (dotproductFact * result.WeightedDotProduct + revDotproductFact * result.ReverseDotProduct + matchedRatioFact * result.MatchedPeaksPercentage) / (dotproductFact + revDotproductFact + matchedRatioFact)
    End Function


    Public Shared Function CompareMSScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As MoleculeMsReference, ByVal param As MsRefSearchParameterBase, ByVal ms2Tol As Single, ByVal massRangeBegin As Single, ByVal massRangeEnd As Single) As MsScanMatchResult

        Dim result = CompareMSScanProperties(scanProp, CType(refSpec, IMSScanProperty), param, ms2Tol, massRangeBegin, massRangeEnd)
        result.Name = refSpec.Name
        result.LibraryID = refSpec.ScanID
        result.InChIKey = refSpec.InChIKey
        Return result
    End Function

    Public Shared Function CompareMSScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As IMSScanProperty, ByVal param As MsRefSearchParameterBase, ByVal ms2Tol As Single, ByVal massRangeBegin As Single, ByVal massRangeEnd As Single) As MsScanMatchResult

        Dim result = CompareBasicMSScanProperties(scanProp, refSpec, param, ms2Tol, massRangeBegin, massRangeEnd)
        Dim matchedPeaksScores = GetMatchedPeaksScores(scanProp, refSpec, ms2Tol, massRangeBegin, massRangeEnd)

        result.MatchedPeaksCount = CSng(matchedPeaksScores(1))
        result.MatchedPeaksPercentage = CSng(matchedPeaksScores(0))
        If result.WeightedDotProduct >= param.WeightedDotProductCutOff AndAlso result.SimpleDotProduct >= param.SimpleDotProductCutOff AndAlso result.ReverseDotProduct >= param.ReverseDotProductCutOff AndAlso result.MatchedPeaksPercentage >= param.MatchedPeaksPercentageCutOff AndAlso result.MatchedPeaksCount >= param.MinimumSpectrumMatch Then
            result.IsSpectrumMatch = True
        End If

        Return result
    End Function

    Public Shared Function CompareBasicMSScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As MoleculeMsReference, ByVal param As MsRefSearchParameterBase, ByVal ms2Tol As Single, ByVal massRangeBegin As Single, ByVal massRangeEnd As Single) As MsScanMatchResult
        Dim result = CompareMSScanProperties(scanProp, CType(refSpec, IMSScanProperty), param, ms2Tol, massRangeBegin, massRangeEnd)
        result.Name = refSpec.Name
        result.LibraryID = refSpec.ScanID
        result.InChIKey = refSpec.InChIKey
        Return result
    End Function

    Public Shared Function CompareBasicMSScanProperties(ByVal scanProp As IMSScanProperty, ByVal refSpec As IMSScanProperty, ByVal param As MsRefSearchParameterBase, ByVal ms2Tol As Single, ByVal massRangeBegin As Single, ByVal massRangeEnd As Single) As MsScanMatchResult

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
            .weightedDotProduct = weightedDotProduct,
            .simpleDotProduct = simpleDotProduct,
            .reverseDotProduct = reverseDotProduct,
            .AcurateMassSimilarity = ms1Similarity,
            .rtSimilarity = rtSimilarity,
            .riSimilarity = riSimilarity,
            .IsPrecursorMzMatch = isMs1Match,
            .isRtMatch = isRtMatch,
            .isRiMatch = isRiMatch
        }

        Return result
    End Function





    ''' <summary>
    ''' This method returns the similarity score between theoretical isotopic ratios and experimental isotopic patterns in MS1 axis.
    ''' This method will utilize up to [M+4] for their calculations.
    ''' </summary>
    ''' <paramname="peaks1">
    ''' Add the MS1 spectrum with respect to the focused data point.
    ''' </param>
    ''' <paramname="peaks2">
    ''' Add the theoretical isotopic abundances. The theoretical patterns are supposed to be calculated in MSP parcer.
    ''' </param>
    ''' <paramname="targetedMz">
    ''' Add the experimental precursor mass.
    ''' </param>
    ''' <paramname="tolerance">
    ''' Add the torelance to merge the spectrum of experimental MS1.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetIsotopeRatioSimilarity(ByVal peaks1 As IReadOnlyList(Of IsotopicPeak), ByVal peaks2 As IReadOnlyList(Of IsotopicPeak), ByVal targetedMz As Double, ByVal tolerance As Double) As Double
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
    ''' <paramname="peaks1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <paramname="peaks2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <paramname="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' [0] The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be returned.
    ''' [1] MatchedPeaksCount is also returned.
    ''' </returns>
    Public Shared Function GetMatchedPeaksScores(ByVal prop1 As IMSScanProperty, ByVal prop2 As IMSScanProperty, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double()
        If Not IsComparedAvailable(prop1, prop2) Then Return New Double(1) {-1, -1}

        Dim peaks1 = prop1.Spectrum
        Dim peaks2 = prop2.Spectrum

        Return GetMatchedPeaksScores(peaks1, peaks2, bin, massBegin, massEnd)
    End Function

    Public Shared Function GetMatchedPeaksScores(ByVal peaks1 As List(Of SpectrumPeak), ByVal peaks2 As List(Of SpectrumPeak), ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double()
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

    Public Shared Function GetSpetralEntropySimilarity(ByVal peaks1 As List(Of SpectrumPeak), ByVal peaks2 As List(Of SpectrumPeak), ByVal bin As Double) As Double
        Dim combinedSpectrum = SpectrumHandler.GetCombinedSpectrum(peaks1, peaks2, bin)
        Dim entropy12 = GetSpectralEntropy(combinedSpectrum)
        Dim entropy1 = GetSpectralEntropy(peaks1)
        Dim entropy2 = GetSpectralEntropy(peaks2)

        Return 1 - (2 * entropy12 - entropy1 - entropy2) * 0.5
    End Function

    Public Shared Function GetSpectralEntropy(ByVal peaks As List(Of SpectrumPeak)) As Double
        Dim sumIntensity = peaks.Sum(Function(n) n.Intensity)
        Return -1 * peaks.Sum(Function(n) n.Intensity / sumIntensity * std.Log(n.Intensity / sumIntensity, 2))
    End Function

    Public Shared Function GetModifiedDotProductScore(ByVal prop1 As IMSScanProperty, ByVal prop2 As IMSScanProperty, ByVal Optional massTolerance As Double = 0.05, ByVal Optional massToleranceType As MassToleranceType = MassToleranceType.Da) As Double()
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

    Public Shared Function GetBonanzaScore(ByVal prop1 As IMSScanProperty, ByVal prop2 As IMSScanProperty, ByVal Optional massTolerance As Double = 0.05, ByVal Optional massToleranceType As MassToleranceType = MassToleranceType.Da) As Double()
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

    Public Shared Sub SearchMatchedPeaks(ByVal ePeaks As List(Of SpectrumPeak), ByVal ePrecursor As Double, ByVal rPeaks As List(Of SpectrumPeak), ByVal rPrecursor As Double, ByVal massTolerance As Double, ByVal massTolType As MassToleranceType, <Out> ByRef matchedPeaks As List(Of MatchedPeak)) ' small precursor
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
            Dim massTol = If(massTolType = MassToleranceType.Da, massTolerance, ConvertPpmToMassAccuracy(rPeak.Mass, massTolerance))
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

    Public Shared Function GetProcessedSpectrum(ByVal peaks As List(Of SpectrumPeak), ByVal peakPrecursorMz As Double, ByVal Optional minMz As Double = 0.0, ByVal Optional maxMz As Double = 10000, ByVal Optional relativeAbundanceCutOff As Double = 0.1, ByVal Optional absoluteAbundanceCutOff As Double = 50.0, ByVal Optional massTolerance As Double = 0.05, ByVal Optional massBinningValue As Double = 1.0, ByVal Optional intensityScaleFactor As Double = 0.5, ByVal Optional scaledMaxValue As Double = 100, ByVal Optional massDelta As Double = 1, ByVal Optional maxPeakNumInDelta As Integer = 12, ByVal Optional massToleranceType As MassToleranceType = MassToleranceType.Da, ByVal Optional isBrClConsideredForIsotopes As Boolean = False, ByVal Optional isRemoveIsotopes As Boolean = False, ByVal Optional removeAfterPrecursor As Boolean = True) As List(Of SpectrumPeak) ' 0.1%

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
    ''' <paramname="peaks"></param>
    ''' <returns></returns>
    Public Shared Function GetAndromedaScore(ByVal peaks As List(Of SpectrumPeak), ByVal andromedaDelta As Double, ByVal andromedaMaxPeak As Double) As Double
        Dim p = andromedaMaxPeak / andromedaDelta
        Dim q = 1 - p
        Dim n = peaks.Count
        Dim k = peaks.Where(Function(spec) spec.IsMatched = True).Count

        Dim sum = 0.0
        For j = k To n
            Dim bc = Mathematics.Basic.BasicMathematics.BinomialCoefficient(n, j)
            Dim p_prob = std.Pow(p, j)
            Dim q_prob = std.Pow(q, n - j)
            sum += bc * p_prob * q_prob
        Next
        Dim andromeda = -10.0 * std.Log10(sum)
        Return If(andromeda < 0.000001, 0.000001, andromeda)
    End Function

    ''' </summary>
    ''' <param name="peaks1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <param name="peaks2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <param name="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    Public Shared Function GetMachedSpectralPeaks(ByVal prop1 As IMSScanProperty, ByVal chargeState As Integer, ByVal prop2 As IMSScanProperty, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As List(Of SpectrumPeak)
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

    Public Shared Function GetMachedSpectralPeaks(ByVal peaks1 As List(Of SpectrumPeak), ByVal peaks2 As List(Of SpectrumPeak), ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As List(Of SpectrumPeak)
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
    ''' This method returns the presence similarity (% of matched fragments) between the experimental MS/MS spectrum and the standard MS/MS spectrum.
    ''' So, this program will calculate how many fragments of library spectrum are found in the experimental spectrum and will return the %.
    ''' double[] [0]m/z[1]intensity
    ''' 
    ''' </summary>
    ''' <paramname="peaks1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <paramname="refSpec">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <paramname="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' [0] The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' [1] MatchedPeaksCount is also returned.
    ''' </returns>
    Public Shared Function GetLipidomicsMatchedPeaksScores(ByVal msScanProp As IMSScanProperty, ByVal molMsRef As MoleculeMsReference, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double()

        If Not IsComparedAvailable(msScanProp, molMsRef) Then Return New Double() {-1, -1}

        ' in lipidomics project, currently, the well-known lipid classes now including
        ' PC, PE, PI, PS, PG, BMP, SM, TAG are now evaluated.
        ' if the lipid class diagnostic fragment (like m/z 184 in PC and SM in ESI(+)) is observed, 
        ' the bonus 0.5 is added to the normal presence score

        Dim resultArray = GetMatchedPeaksScores(msScanProp, molMsRef, bin, massBegin, massEnd) ' [0] matched ratio [1] matched count
        Dim compClass = molMsRef.CompoundClass
        Dim comment = molMsRef.Comment
        If Not Equals(comment, "SPLASH") AndAlso Not Equals(compClass, "Unknown") AndAlso Not Equals(compClass, "Others") Then
            Dim molecule = ConvertMsdialLipidnameToLipidMoleculeObjectVS2(molMsRef)
            If molecule Is Nothing OrElse molecule.Adduct Is Nothing Then Return resultArray
            If molecule.LipidClass = LbmClass.EtherPE AndAlso molMsRef.Spectrum.Count = 3 AndAlso msScanProp.IonMode = IonModes.Positive Then Return resultArray

            Dim result = GetLipidMoleculeAnnotationResult(msScanProp, molecule, bin)
            If result IsNot Nothing Then
                If result.AnnotationLevel = 1 Then
                    If Equals(compClass, "SM") AndAlso (molecule.LipidName.Contains("3O") OrElse molecule.LipidName.Contains("O3")) Then
                        resultArray(0) += 1.0
                        Return resultArray ' add bonus
                    Else
                        resultArray(0) += 0.5
                        Return resultArray ' add bonus
                    End If
                ElseIf result.AnnotationLevel = 2 Then
                    resultArray(0) += 1.0
                    Return resultArray ' add bonus
                Else
                    Return resultArray
                End If
            Else
                Return resultArray
            End If ' currently default value is retured for other lipids
        Else
            Return resultArray
        End If
    End Function

    Public Shared Function GetLipidomicsMoleculerSpeciesLevelAnnotationPeaksScoresForEIEIO(ByVal msScanProp As IMSScanProperty, ByVal molMsRef As MoleculeMsReference, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double()

        If Not IsComparedAvailable(msScanProp, molMsRef) Then Return New Double() {-1, -1}

        ' in lipidomics project, currently, the well-known lipid classes now including
        ' PC, PE, PI, PS, PG, BMP, SM, TAG are now evaluated.
        ' if the lipid class diagnostic fragment (like m/z 184 in PC and SM in ESI(+)) is observed, 
        ' the bonus 0.5 is added to the normal presence score

        Dim resultArray = GetMatchedPeaksScores(msScanProp, molMsRef, bin, massBegin, massEnd) ' [0] matched ratio [1] matched count
        Dim compClass = molMsRef.CompoundClass
        Dim comment = molMsRef.Comment
        If Not Equals(comment, "SPLASH") AndAlso Not Equals(compClass, "Unknown") AndAlso Not Equals(compClass, "Others") Then
            Dim molecule = ConvertMsdialLipidnameToLipidMoleculeObjectVS2(molMsRef)
            If molecule Is Nothing OrElse molecule.Adduct Is Nothing Then Return resultArray
            'if (molecule.LipidClass == LbmClass.EtherPE && molMsRef.Spectrum.Count == 3 && msScanProp.IonMode == IonMode.Positive) return resultArray;

            Dim result = GetLipidMoleculerSpeciesLevelAnnotationResultForEIEIO(msScanProp, molecule, bin)
            If result IsNot Nothing Then
                If result.AnnotationLevel = 1 Then
                    If Equals(compClass, "SM") AndAlso (molecule.LipidName.Contains("3O") OrElse molecule.LipidName.Contains("O3")) Then
                        resultArray(0) += 1.0
                        Return resultArray ' add bonus
                    Else
                        resultArray(0) += 0.5
                        Return resultArray ' add bonus
                    End If
                ElseIf result.AnnotationLevel = 2 Then
                    resultArray(0) += 1.0
                    Return resultArray ' add bonus
                Else
                    Return resultArray
                End If
            Else
                Return resultArray
            End If ' currently default value is retured for other lipids
        Else
            Return resultArray
        End If
    End Function

    Public Shared Function GetLipidNameFromReference(ByVal reference As MoleculeMsReference) As String
        Dim compClass = reference.CompoundClass
        Dim comment = reference.Comment
        If Not Equals(comment, "SPLASH") AndAlso Not Equals(compClass, "Unknown") AndAlso Not Equals(compClass, "Others") Then
            Dim molecule = ConvertMsdialLipidnameToLipidMoleculeObjectVS2(reference)
            If molecule Is Nothing OrElse molecule.Adduct Is Nothing Then
                Return reference.Name
            End If
            Dim refinedName = String.Empty
            If Equals(molecule.SublevelLipidName, molecule.LipidName) Then
                Return molecule.SublevelLipidName
            Else
                Return molecule.SublevelLipidName & "|" & molecule.LipidName
            End If
        Else
            Return reference.Name
        End If
    End Function

    Public Shared Function GetRefinedLipidAnnotationLevel(ByVal msScanProp As IMSScanProperty, ByVal molMsRef As MoleculeMsReference, ByVal bin As Double, <Out> ByRef isLipidClassMatched As Boolean, <Out> ByRef isLipidChainMatched As Boolean, <Out> ByRef isLipidPositionMatched As Boolean, <Out> ByRef isOthers As Boolean) As String

        isLipidClassMatched = False
        isLipidChainMatched = False
        isLipidPositionMatched = False
        isOthers = False
        If Not IsComparedAvailable(msScanProp, molMsRef) Then Return String.Empty

        ' in lipidomics project, currently, the well-known lipid classes now including
        ' PC, PE, PI, PS, PG, SM, TAG are now evaluated.
        ' if the lipid class diagnostic fragment (like m/z 184 in PC and SM in ESI(+)) is observed, 
        ' the bonus 0.5 is added to the normal presence score

        Dim compClass = molMsRef.CompoundClass
        Dim comment = molMsRef.Comment

        If Not Equals(comment, "SPLASH") AndAlso Not Equals(compClass, "Unknown") AndAlso Not Equals(compClass, "Others") Then

            If Equals(compClass, "Cholesterol") OrElse Equals(compClass, "CholesterolSulfate") OrElse Equals(compClass, "Undefined") OrElse Equals(compClass, "BileAcid") OrElse Equals(compClass, "Ac2PIM1") OrElse Equals(compClass, "Ac2PIM2") OrElse Equals(compClass, "Ac3PIM2") OrElse Equals(compClass, "Ac4PIM2") OrElse Equals(compClass, "LipidA") Then
                isOthers = True
                Return molMsRef.Name ' currently default value is retured for these lipids
            End If

            Dim molecule = ConvertMsdialLipidnameToLipidMoleculeObjectVS2(molMsRef)
            If molecule Is Nothing OrElse molecule.Adduct Is Nothing Then
                isOthers = True
                Return molMsRef.Name
            End If

            Dim result = GetLipidMoleculeAnnotationResult(msScanProp, molecule, bin)
            If result IsNot Nothing Then
                Dim refinedName = String.Empty
                If result.AnnotationLevel = 1 Then
                    refinedName = result.SublevelLipidName
                    isLipidClassMatched = True
                    isLipidChainMatched = False
                    isLipidPositionMatched = False
                ElseIf result.AnnotationLevel = 2 Then
                    isLipidClassMatched = True
                    isLipidChainMatched = True
                    isLipidPositionMatched = False
                    If Equals(result.SublevelLipidName, result.LipidName) Then
                        refinedName = result.SublevelLipidName
                    Else
                        refinedName = result.SublevelLipidName & "|" & result.LipidName
                    End If
                Else
                    Return String.Empty
                End If

                Return refinedName
            Else
                Return String.Empty
            End If ' currently default value is retured for other lipids
        Else
            isOthers = True
            Return molMsRef.Name
        End If
    End Function

    Public Shared Function GetRefinedLipidAnnotationLevelForEIEIO(ByVal msScanProp As IMSScanProperty, ByVal molMsRef As MoleculeMsReference, ByVal bin As Double, <Out> ByRef isLipidClassMatched As Boolean, <Out> ByRef isLipidChainMatched As Boolean, <Out> ByRef isLipidPositionMatched As Boolean, <Out> ByRef isOthers As Boolean) As String

        isLipidClassMatched = False
        isLipidChainMatched = False
        isLipidPositionMatched = False
        isOthers = False
        If Not IsComparedAvailable(msScanProp, molMsRef) Then Return String.Empty

        ' in lipidomics project, currently, the well-known lipid classes now including
        ' PC, PE, PI, PS, PG, SM, TAG are now evaluated.
        ' if the lipid class diagnostic fragment (like m/z 184 in PC and SM in ESI(+)) is observed, 
        ' the bonus 0.5 is added to the normal presence score

        Dim compClass = molMsRef.CompoundClass
        Dim comment = molMsRef.Comment

        If Not Equals(comment, "SPLASH") AndAlso Not Equals(compClass, "Unknown") AndAlso Not Equals(compClass, "Others") Then

            If Equals(compClass, "Cholesterol") OrElse Equals(compClass, "CholesterolSulfate") OrElse Equals(compClass, "Undefined") OrElse Equals(compClass, "BileAcid") OrElse Equals(compClass, "Ac2PIM1") OrElse Equals(compClass, "Ac2PIM2") OrElse Equals(compClass, "Ac3PIM2") OrElse Equals(compClass, "Ac4PIM2") OrElse Equals(compClass, "LipidA") Then
                isOthers = True
                Return molMsRef.Name ' currently default value is retured for these lipids
            End If

            Dim molecule = ConvertMsdialLipidnameToLipidMoleculeObjectVS2(molMsRef)
            If molecule Is Nothing OrElse molecule.Adduct Is Nothing Then
                isOthers = True
                Return molMsRef.Name
            End If

            Dim result = GetLipidMoleculerSpeciesLevelAnnotationResultForEIEIO(msScanProp, molecule, bin)
            If result IsNot Nothing Then
                Dim refinedName = String.Empty
                If result.AnnotationLevel = 1 Then
                    refinedName = result.SublevelLipidName
                    isLipidClassMatched = True
                    isLipidChainMatched = False
                    isLipidPositionMatched = False
                ElseIf result.AnnotationLevel = 2 Then
                    isLipidClassMatched = True
                    isLipidChainMatched = True
                    isLipidPositionMatched = False
                    If Equals(result.SublevelLipidName, result.LipidName) Then
                        refinedName = result.SublevelLipidName
                    Else
                        refinedName = result.SublevelLipidName & "|" & result.LipidName
                    End If
                Else
                    Return String.Empty
                End If

                Return refinedName
            Else
                Return String.Empty
            End If ' currently default value is retured for other lipids
        Else
            isOthers = True
            Return molMsRef.Name
        End If
    End Function


    Public Shared Function GetLipidMoleculerSpeciesLevelAnnotationResultForEIEIO(ByVal msScanProp As IMSScanProperty, ByVal molecule As LipidMolecule, ByVal ms2tol As Double) As LipidMolecule
        Dim lipidclass = molecule.LipidClass
        Dim refMz = molecule.Mz
        Dim adduct = molecule.Adduct

        Dim totalCarbon = molecule.TotalCarbonCount
        Dim totalDbBond = molecule.TotalDoubleBondCount
        Dim totalOxidized = molecule.TotalOxidizedCount

        Dim sn1Carbon = molecule.Sn1CarbonCount
        Dim sn1DbBond = molecule.Sn1DoubleBondCount
        Dim sn1Oxidized = molecule.Sn1Oxidizedount
        Dim sn2Oxidized = molecule.Sn2Oxidizedount

        ' Console.WriteLine(molecule.LipidName);
        Dim lipidheader = GetLipidHeaderString(molecule.LipidName)
        ' Console.WriteLine(lipidheader + "\t" + lipidclass.ToString());

        Select Case lipidclass
            Case LbmClass.PC 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylcholine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PE 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylethanolamine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PS 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylserine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PG 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.BMP 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfBismonoacylglycerophosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PI 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylinositol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.SM 'EIEIO
                If molecule.TotalChainString.Contains("O3") Then
                    Return LipidMsmsCharacterization.JudgeIfSphingomyelinPhyto(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                Else
                    Return LipidEieioMsmsCharacterization.JudgeIfSphingomyelin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
                End If
            Case LbmClass.LNAPE
                Return LipidMsmsCharacterization.JudgeIfNacylphosphatidylethanolamine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LNAPS
                Return LipidMsmsCharacterization.JudgeIfNacylphosphatidylserine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.CE 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfCholesterylEster(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.CAR 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfAcylcarnitine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.DG 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfDag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)

            Case LbmClass.MG 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfMag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MGDG
                Return LipidMsmsCharacterization.JudgeIfMgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DGDG
                Return LipidMsmsCharacterization.JudgeIfDgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PMeOH
                Return LipidMsmsCharacterization.JudgeIfPmeoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PEtOH
                Return LipidMsmsCharacterization.JudgeIfPetoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PBtOH
                Return LipidMsmsCharacterization.JudgeIfPbtoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPC 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)

            Case LbmClass.LPE 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysope(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)

            Case LbmClass.PA 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfPhosphatidicacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPA
                Return LipidMsmsCharacterization.JudgeIfLysopa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPG 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)

            Case LbmClass.LPI 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)

            Case LbmClass.LPS 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysops(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)

            Case LbmClass.EtherPC 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfEtherpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)

            Case LbmClass.EtherPE 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfEtherpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)

            Case LbmClass.EtherLPC
                Return LipidMsmsCharacterization.JudgeIfEtherlysopc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherLPE
                Return LipidMsmsCharacterization.JudgeIfEtherlysope(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.OxPC
                Return LipidMsmsCharacterization.JudgeIfOxpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPE
                Return LipidMsmsCharacterization.JudgeIfOxpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPG
                Return LipidMsmsCharacterization.JudgeIfOxpg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPI
                Return LipidMsmsCharacterization.JudgeIfOxpi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPS
                Return LipidMsmsCharacterization.JudgeIfOxps(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.EtherMGDG
                Return LipidMsmsCharacterization.JudgeIfEthermgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherDGDG
                Return LipidMsmsCharacterization.JudgeIfEtherdgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DGTS 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfDgts(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LDGTS 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLdgts(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.DGCC
                Return LipidMsmsCharacterization.JudgeIfDgcc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LDGCC
                Return LipidMsmsCharacterization.JudgeIfLdgcc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DGGA
                Return LipidMsmsCharacterization.JudgeIfGlcadg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.SQDG
                Return LipidMsmsCharacterization.JudgeIfSqdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DLCL
                Return LipidMsmsCharacterization.JudgeIfDilysocardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.FA
                Return LipidMsmsCharacterization.JudgeIfFattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.OxFA
                Return LipidMsmsCharacterization.JudgeIfOxfattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.FAHFA
                Return LipidMsmsCharacterization.JudgeIfFahfa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DMEDFAHFA 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfFahfaDMED(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DMEDFA 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfDmedFattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.DMEDOxFA 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfDmedOxfattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.EtherOxPC
                Return LipidMsmsCharacterization.JudgeIfEtheroxpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.EtherOxPE
                Return LipidMsmsCharacterization.JudgeIfEtheroxpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.Cer_NS
                Return LipidMsmsCharacterization.JudgeIfCeramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_NDS
                Return LipidMsmsCharacterization.JudgeIfCeramidends(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_NS
                Return LipidMsmsCharacterization.JudgeIfHexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_NDS
                Return LipidMsmsCharacterization.JudgeIfHexceramidends(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Hex2Cer
                Return LipidMsmsCharacterization.JudgeIfHexhexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Hex3Cer
                Return LipidMsmsCharacterization.JudgeIfHexhexhexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_AP
                Return LipidMsmsCharacterization.JudgeIfCeramideap(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_AP
                Return LipidMsmsCharacterization.JudgeIfHexceramideap(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)


            Case LbmClass.SHexCer 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfShexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct, totalOxidized)

            Case LbmClass.GM3
                Return LipidMsmsCharacterization.JudgeIfGm3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DHSph
                Return LipidMsmsCharacterization.JudgeIfSphinganine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

            Case LbmClass.Sph
                Return LipidEieioMsmsCharacterization.JudgeIfSphingosine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

            Case LbmClass.PhytoSph
                Return LipidMsmsCharacterization.JudgeIfPhytosphingosine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

            Case LbmClass.TG 'EIEIO
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.ADGGA
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylglcadg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)
            Case LbmClass.HBMP 'EIEIO
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfHemiismonoacylglycerophosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.EtherTG
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfEthertag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.MLCL
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfLysocardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.Cer_EOS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfCeramideeos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.Cer_EODS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfCeramideeods(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.HexCer_EOS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfHexceramideeos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.ASM
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidEieioMsmsCharacterization.JudgeIfAcylsm(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn2Carbon, sn1DbBond, sn2DbBond, adduct)

            Case LbmClass.Cer_EBDS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylcerbds(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.AHexCer
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylhexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.ASHexCer
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAshexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.CL 'EIEIO
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Dim sn3Carbon = molecule.Sn3CarbonCount
                Dim sn3DbBond = molecule.Sn3DoubleBondCount
                If sn3Carbon < 1 Then
                    Return LipidMsmsCharacterization.JudgeIfCardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfCardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, sn3Carbon, sn3Carbon, sn3DbBond, sn3DbBond, adduct)
                End If

                'add 10/04/19
            Case LbmClass.EtherPI
                Return LipidMsmsCharacterization.JudgeIfEtherpi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.EtherPS
                Return LipidMsmsCharacterization.JudgeIfEtherps(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.EtherDG
                Return LipidMsmsCharacterization.JudgeIfEtherDAG(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PI_Cer
                Return LipidEieioMsmsCharacterization.JudgeIfPicermide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct, totalOxidized)

            Case LbmClass.PE_Cer
                Return LipidMsmsCharacterization.JudgeIfPecermide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

                'add 13/5/19
            Case LbmClass.DCAE
                Return LipidMsmsCharacterization.JudgeIfDcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.GDCAE
                Return LipidMsmsCharacterization.JudgeIfGdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.GLCAE
                Return LipidMsmsCharacterization.JudgeIfGlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.TDCAE
                Return LipidMsmsCharacterization.JudgeIfTdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.TLCAE
                Return LipidMsmsCharacterization.JudgeIfTlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.NAE
                Return LipidMsmsCharacterization.JudgeIfAnandamide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.NAGly
                If totalCarbon < 29 Then
                    Return LipidEieioMsmsCharacterization.JudgeIfNAcylGlyOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfFahfamidegly(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If


            Case LbmClass.NAGlySer
                If totalCarbon < 29 Then
                    Return LipidMsmsCharacterization.JudgeIfNAcylGlySerOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfFahfamideglyser(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If


            Case LbmClass.SL
                Return LipidMsmsCharacterization.JudgeIfSulfonolipid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.EtherPG
                Return LipidMsmsCharacterization.JudgeIfEtherpg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherLPG
                Return LipidMsmsCharacterization.JudgeIfEtherlysopg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.CoQ
                Return LipidMsmsCharacterization.JudgeIfCoenzymeq(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.Vitamin_E
                Return LipidMsmsCharacterization.JudgeIfVitaminEmolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.Vitamin_D
                Return LipidMsmsCharacterization.JudgeIfVitaminDmolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.VAE
                Return LipidMsmsCharacterization.JudgeIfVitaminaestermolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.NAOrn
                If totalCarbon < 29 Then
                    Return LipidMsmsCharacterization.JudgeIfNAcylOrnOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfFahfamideorn(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If


            Case LbmClass.BRSE
                Return LipidMsmsCharacterization.JudgeIfBrseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.CASE
                Return LipidMsmsCharacterization.JudgeIfCaseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.SISE
                Return LipidMsmsCharacterization.JudgeIfSiseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.STSE
                Return LipidMsmsCharacterization.JudgeIfStseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.AHexBRS
                Return LipidMsmsCharacterization.JudgeIfAhexbrseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexCAS
                Return LipidMsmsCharacterization.JudgeIfAhexcaseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexCS
                Return LipidMsmsCharacterization.JudgeIfAhexceSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexSIS
                Return LipidMsmsCharacterization.JudgeIfAhexsiseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexSTS
                Return LipidMsmsCharacterization.JudgeIfAhexstseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


                ' add 27/05/19
            Case LbmClass.Cer_AS 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfCeramideas(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_ADS 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfCeramideads(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_BS 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfCeramidebs(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_BDS 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfCeramidebds(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_NP 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfCeramidenp(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_OS
                Return LipidMsmsCharacterization.JudgeIfCeramideos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                'add 190528
            Case LbmClass.Cer_HS
                Return LipidMsmsCharacterization.JudgeIfCeramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_HDS
                Return LipidMsmsCharacterization.JudgeIfCeramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_NDOS
                Return LipidMsmsCharacterization.JudgeIfCeramidedos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_HS 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfHexceramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_HDS 'EIEIO
                Return LipidMsmsCharacterization.JudgeIfHexceramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                '190801
            Case LbmClass.SHex
                Return LipidMsmsCharacterization.JudgeIfSterolHexoside(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.BAHex
                Return LipidMsmsCharacterization.JudgeIfSterolHexoside(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.SSulfate
                Return LipidMsmsCharacterization.JudgeIfSterolSulfate(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.BASulfate
                Return LipidMsmsCharacterization.JudgeIfSterolSulfate(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                ' added 190811
            Case LbmClass.CerP
                Return LipidMsmsCharacterization.JudgeIfCeramidePhosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                ''' 2019/11/25 add
            Case LbmClass.SMGDG
                Return LipidMsmsCharacterization.JudgeIfSmgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.EtherSMGDG
                Return LipidMsmsCharacterization.JudgeIfEtherSmgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                'add 20200218
            Case LbmClass.LCAE
                Return LipidMsmsCharacterization.JudgeIfLcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.KLCAE
                Return LipidMsmsCharacterization.JudgeIfKlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.KDCAE
                Return LipidMsmsCharacterization.JudgeIfKdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

                'add 20200714
            Case LbmClass.DMPE
                Return LipidMsmsCharacterization.JudgeIfDiMethylPE(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MMPE
                Return LipidMsmsCharacterization.JudgeIfMonoMethylPE(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MIPC
                Return LipidMsmsCharacterization.JudgeIfMipc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                'add 20200720
            Case LbmClass.EGSE
                Return LipidMsmsCharacterization.JudgeIfErgoSESpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.DEGSE
                Return LipidMsmsCharacterization.JudgeIfDehydroErgoSESpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                'add 20200812
            Case LbmClass.OxTG
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfOxTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, totalOxidized, adduct)
            Case LbmClass.TG_EST
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Dim sn3Carbon = molecule.Sn3CarbonCount
                Dim sn3DbBond = molecule.Sn3DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfFahfaTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, sn3Carbon, sn3Carbon, sn3DbBond, sn3DbBond, adduct)
                'add 20200923
            Case LbmClass.DSMSE
                Return LipidMsmsCharacterization.JudgeIfDesmosterolSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                'add20210216
            Case LbmClass.GPNAE
                Return LipidMsmsCharacterization.JudgeIfGpnae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.MGMG
                Return LipidMsmsCharacterization.JudgeIfMgmg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.DGMG
                Return LipidMsmsCharacterization.JudgeIfDgmg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                'add 20210315-
            Case LbmClass.GD1a
                Return LipidMsmsCharacterization.JudgeIfGD1a(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GD1b
                Return LipidMsmsCharacterization.JudgeIfGD1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GD2
                Return LipidMsmsCharacterization.JudgeIfGD2(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GD3
                Return LipidMsmsCharacterization.JudgeIfGD3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GM1
                Return LipidMsmsCharacterization.JudgeIfGM1(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GQ1b
                Return LipidMsmsCharacterization.JudgeIfGQ1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GT1b
                Return LipidMsmsCharacterization.JudgeIfGT1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.NGcGM3
                Return LipidMsmsCharacterization.JudgeIfNGcGM3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.ST
                Return LipidMsmsCharacterization.JudgeIfnoChainSterol(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.CSLPHex, LbmClass.BRSLPHex, LbmClass.CASLPHex, LbmClass.SISLPHex, LbmClass.STSLPHex
                Return LipidMsmsCharacterization.JudgeIfSteroidWithLpa(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.CSPHex, LbmClass.BRSPHex, LbmClass.CASPHex, LbmClass.SISPHex, LbmClass.STSPHex
                Return LipidMsmsCharacterization.JudgeIfSteroidWithPa(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20220201
            Case LbmClass.SPE
                Return LipidMsmsCharacterization.JudgeIfSpeSpecies(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                '20220322
            Case LbmClass.NAPhe
                Return LipidMsmsCharacterization.JudgeIfNAcylPheFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NATau
                Return LipidMsmsCharacterization.JudgeIfNAcylTauFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                '20221019
            Case LbmClass.PT
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylThreonine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                '20230407
            Case LbmClass.PC_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylcholineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PE_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylethanolamineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PS_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylserineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PG_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylglycerolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.PI_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfPhosphatidylinositolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.LPC_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopcD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)
            Case LbmClass.LPE_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopeD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)
            Case LbmClass.LPG_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopgD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)
            Case LbmClass.LPI_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopiD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)
            Case LbmClass.LPS_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfLysopsD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1DbBond, adduct)
            Case LbmClass.TG_d5 'EIEIO
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidEieioMsmsCharacterization.JudgeIfTriacylglycerolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn2Carbon, totalCarbon - sn1Carbon - sn2Carbon, sn1DbBond, sn2DbBond, totalDbBond - sn1DbBond - sn2DbBond, adduct)

            Case LbmClass.DG_d5 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfDagD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.SM_d9 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfSphingomyelinD9(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
            Case LbmClass.CE_d7 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfCholesterylEsterD7(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.Cer_NS_d7 'EIEIO
                Return LipidEieioMsmsCharacterization.JudgeIfCeramidensD7(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
                '20230424
            Case LbmClass.bmPC
                Return LipidMsmsCharacterization.JudgeIfBetaMethylPhosphatidylcholine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20230612
            Case LbmClass.NATryA
                Return LipidMsmsCharacterization.JudgeIfNAcylTryA(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NA5HT
                Return LipidMsmsCharacterization.JudgeIfNAcyl5HT(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.WE
                Return LipidMsmsCharacterization.JudgeIfWaxEster(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1Carbon, totalCarbon - sn1Carbon, sn1DbBond, totalDbBond - sn1DbBond, adduct)
                '20230626
            Case LbmClass.NAAla
                Return LipidMsmsCharacterization.JudgeIfNAcylAla(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NAGln
                Return LipidMsmsCharacterization.JudgeIfNAcylGln(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NALeu
                Return LipidMsmsCharacterization.JudgeIfNAcylLeu(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NAVal
                Return LipidMsmsCharacterization.JudgeIfNAcylVal(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NASer
                Return LipidMsmsCharacterization.JudgeIfNAcylSer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.BisMeLPA
                Return LipidMsmsCharacterization.JudgeIfBismelpa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Shared Function GetLipidMoleculeAnnotationResult(ByVal msScanProp As IMSScanProperty, ByVal molecule As LipidMolecule, ByVal ms2tol As Double) As LipidMolecule

        Dim lipidclass = molecule.LipidClass
        Dim refMz = molecule.Mz
        Dim adduct = molecule.Adduct

        Dim totalCarbon = molecule.TotalCarbonCount
        Dim totalDbBond = molecule.TotalDoubleBondCount
        Dim totalOxidized = molecule.TotalOxidizedCount

        Dim sn1Carbon = molecule.Sn1CarbonCount
        Dim sn1DbBond = molecule.Sn1DoubleBondCount
        Dim sn1Oxidized = molecule.Sn1Oxidizedount
        Dim sn2Oxidized = molecule.Sn2Oxidizedount

        ' Console.WriteLine(molecule.LipidName);
        Dim lipidheader = GetLipidHeaderString(molecule.LipidName)
        ' Console.WriteLine(lipidheader + "\t" + lipidclass.ToString());

        Select Case lipidclass
            Case LbmClass.PC
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylcholine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PE
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylethanolamine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PS
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylserine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PG
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.BMP
                Return LipidMsmsCharacterization.JudgeIfBismonoacylglycerophosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PI
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylinositol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.SM
                If molecule.TotalChainString.Contains("O3") Then
                    Return LipidMsmsCharacterization.JudgeIfSphingomyelinPhyto(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfSphingomyelin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If
            Case LbmClass.LNAPE
                Return LipidMsmsCharacterization.JudgeIfNacylphosphatidylethanolamine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LNAPS
                Return LipidMsmsCharacterization.JudgeIfNacylphosphatidylserine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.CE
                Return LipidMsmsCharacterization.JudgeIfCholesterylEster(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.CAR
                Return LipidMsmsCharacterization.JudgeIfAcylcarnitine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.DG
                Return LipidMsmsCharacterization.JudgeIfDag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MG
                Return LipidMsmsCharacterization.JudgeIfMag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MGDG
                Return LipidMsmsCharacterization.JudgeIfMgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DGDG
                Return LipidMsmsCharacterization.JudgeIfDgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PMeOH
                Return LipidMsmsCharacterization.JudgeIfPmeoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PEtOH
                Return LipidMsmsCharacterization.JudgeIfPetoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PBtOH
                Return LipidMsmsCharacterization.JudgeIfPbtoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPC
                Return LipidMsmsCharacterization.JudgeIfLysopc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPE
                Return LipidMsmsCharacterization.JudgeIfLysope(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.PA
                Return LipidMsmsCharacterization.JudgeIfPhosphatidicacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPA
                Return LipidMsmsCharacterization.JudgeIfLysopa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPG
                Return LipidMsmsCharacterization.JudgeIfLysopg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPI
                Return LipidMsmsCharacterization.JudgeIfLysopi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LPS
                Return LipidMsmsCharacterization.JudgeIfLysops(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherPC
                Return LipidMsmsCharacterization.JudgeIfEtherpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherPE
                Return LipidMsmsCharacterization.JudgeIfEtherpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherLPC
                Return LipidMsmsCharacterization.JudgeIfEtherlysopc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherLPE
                Return LipidMsmsCharacterization.JudgeIfEtherlysope(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.OxPC
                Return LipidMsmsCharacterization.JudgeIfOxpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPE
                Return LipidMsmsCharacterization.JudgeIfOxpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPG
                Return LipidMsmsCharacterization.JudgeIfOxpg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPI
                Return LipidMsmsCharacterization.JudgeIfOxpi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.OxPS
                Return LipidMsmsCharacterization.JudgeIfOxps(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.EtherMGDG
                Return LipidMsmsCharacterization.JudgeIfEthermgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherDGDG
                Return LipidMsmsCharacterization.JudgeIfEtherdgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DGTS
                Return LipidMsmsCharacterization.JudgeIfDgts(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LDGTS
                Return LipidMsmsCharacterization.JudgeIfLdgts(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.DGCC
                Return LipidMsmsCharacterization.JudgeIfDgcc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.LDGCC
                Return LipidMsmsCharacterization.JudgeIfLdgcc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DGGA
                Return LipidMsmsCharacterization.JudgeIfGlcadg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.SQDG
                Return LipidMsmsCharacterization.JudgeIfSqdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DLCL
                Return LipidMsmsCharacterization.JudgeIfDilysocardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.FA
                Return LipidMsmsCharacterization.JudgeIfFattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.OxFA
                Return LipidMsmsCharacterization.JudgeIfOxfattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.FAHFA
                Return LipidMsmsCharacterization.JudgeIfFahfa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DMEDFAHFA
                Return LipidMsmsCharacterization.JudgeIfFahfaDMED(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DMEDFA
                Return LipidMsmsCharacterization.JudgeIfDmedFattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.DMEDOxFA
                Return LipidMsmsCharacterization.JudgeIfDmedOxfattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.EtherOxPC
                Return LipidMsmsCharacterization.JudgeIfEtheroxpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.EtherOxPE
                Return LipidMsmsCharacterization.JudgeIfEtheroxpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized, sn1Oxidized, sn2Oxidized)

            Case LbmClass.Cer_NS
                Return LipidMsmsCharacterization.JudgeIfCeramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_NDS
                Return LipidMsmsCharacterization.JudgeIfCeramidends(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_NS
                Return LipidMsmsCharacterization.JudgeIfHexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_NDS
                Return LipidMsmsCharacterization.JudgeIfHexceramidends(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Hex2Cer
                Return LipidMsmsCharacterization.JudgeIfHexhexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Hex3Cer
                Return LipidMsmsCharacterization.JudgeIfHexhexhexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_AP
                Return LipidMsmsCharacterization.JudgeIfCeramideap(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_AP
                Return LipidMsmsCharacterization.JudgeIfHexceramideap(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)


            Case LbmClass.SHexCer
                Return LipidMsmsCharacterization.JudgeIfShexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.GM3
                Return LipidMsmsCharacterization.JudgeIfGm3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.DHSph
                Return LipidMsmsCharacterization.JudgeIfSphinganine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

            Case LbmClass.Sph
                Return LipidMsmsCharacterization.JudgeIfSphingosine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

            Case LbmClass.PhytoSph
                Return LipidMsmsCharacterization.JudgeIfPhytosphingosine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

            Case LbmClass.TG
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.ADGGA
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylglcadg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)
            Case LbmClass.HBMP
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfHemiismonoacylglycerophosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.EtherTG
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfEthertag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.MLCL
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfLysocardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.Cer_EOS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfCeramideeos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.Cer_EODS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfCeramideeods(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.HexCer_EOS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfHexceramideeos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.ASM
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylsm(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.Cer_EBDS
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylcerbds(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.AHexCer
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAcylhexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.ASHexCer
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfAshexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)

            Case LbmClass.CL
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Dim sn3Carbon = molecule.Sn3CarbonCount
                Dim sn3DbBond = molecule.Sn3DoubleBondCount
                If sn3Carbon < 1 Then
                    Return LipidMsmsCharacterization.JudgeIfCardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfCardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, sn3Carbon, sn3Carbon, sn3DbBond, sn3DbBond, adduct)
                End If

                'add 10/04/19
            Case LbmClass.EtherPI
                Return LipidMsmsCharacterization.JudgeIfEtherpi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.EtherPS
                Return LipidMsmsCharacterization.JudgeIfEtherps(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.EtherDG
                Return LipidMsmsCharacterization.JudgeIfEtherDAG(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PI_Cer
                Return LipidMsmsCharacterization.JudgeIfPicermide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)
            Case LbmClass.PE_Cer
                Return LipidMsmsCharacterization.JudgeIfPecermide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

                'add 13/5/19
            Case LbmClass.DCAE
                Return LipidMsmsCharacterization.JudgeIfDcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.GDCAE
                Return LipidMsmsCharacterization.JudgeIfGdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.GLCAE
                Return LipidMsmsCharacterization.JudgeIfGlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.TDCAE
                Return LipidMsmsCharacterization.JudgeIfTdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.TLCAE
                Return LipidMsmsCharacterization.JudgeIfTlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.NAE
                Return LipidMsmsCharacterization.JudgeIfAnandamide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.NAGly
                If totalCarbon < 29 Then
                    Return LipidMsmsCharacterization.JudgeIfNAcylGlyOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfFahfamidegly(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If


            Case LbmClass.NAGlySer
                If totalCarbon < 29 Then
                    Return LipidMsmsCharacterization.JudgeIfNAcylGlySerOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfFahfamideglyser(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If


            Case LbmClass.SL
                Return LipidMsmsCharacterization.JudgeIfSulfonolipid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct, totalOxidized)

            Case LbmClass.EtherPG
                Return LipidMsmsCharacterization.JudgeIfEtherpg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.EtherLPG
                Return LipidMsmsCharacterization.JudgeIfEtherlysopg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.CoQ
                Return LipidMsmsCharacterization.JudgeIfCoenzymeq(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.Vitamin_E
                Return LipidMsmsCharacterization.JudgeIfVitaminEmolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.Vitamin_D
                Return LipidMsmsCharacterization.JudgeIfVitaminDmolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.VAE
                Return LipidMsmsCharacterization.JudgeIfVitaminaestermolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.NAOrn
                If totalCarbon < 29 Then
                    Return LipidMsmsCharacterization.JudgeIfNAcylOrnOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Else
                    Return LipidMsmsCharacterization.JudgeIfFahfamideorn(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                End If


            Case LbmClass.BRSE
                Return LipidMsmsCharacterization.JudgeIfBrseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.CASE
                Return LipidMsmsCharacterization.JudgeIfCaseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.SISE
                Return LipidMsmsCharacterization.JudgeIfSiseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.STSE
                Return LipidMsmsCharacterization.JudgeIfStseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


            Case LbmClass.AHexBRS
                Return LipidMsmsCharacterization.JudgeIfAhexbrseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexCAS
                Return LipidMsmsCharacterization.JudgeIfAhexcaseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexCS
                Return LipidMsmsCharacterization.JudgeIfAhexceSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexSIS
                Return LipidMsmsCharacterization.JudgeIfAhexsiseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.AHexSTS
                Return LipidMsmsCharacterization.JudgeIfAhexstseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


                ' add 27/05/19
            Case LbmClass.Cer_AS
                Return LipidMsmsCharacterization.JudgeIfCeramideas(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_ADS
                Return LipidMsmsCharacterization.JudgeIfCeramideads(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_BS
                Return LipidMsmsCharacterization.JudgeIfCeramidebs(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_BDS
                Return LipidMsmsCharacterization.JudgeIfCeramidebds(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_NP
                Return LipidMsmsCharacterization.JudgeIfCeramidenp(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_OS
                Return LipidMsmsCharacterization.JudgeIfCeramideos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                'add 190528
            Case LbmClass.Cer_HS
                Return LipidMsmsCharacterization.JudgeIfCeramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_HDS
                Return LipidMsmsCharacterization.JudgeIfCeramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.Cer_NDOS
                Return LipidMsmsCharacterization.JudgeIfCeramidedos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_HS
                Return LipidMsmsCharacterization.JudgeIfHexceramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.HexCer_HDS
                Return LipidMsmsCharacterization.JudgeIfHexceramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                '190801
            Case LbmClass.SHex
                Return LipidMsmsCharacterization.JudgeIfSterolHexoside(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.BAHex
                Return LipidMsmsCharacterization.JudgeIfSterolHexoside(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.SSulfate
                Return LipidMsmsCharacterization.JudgeIfSterolSulfate(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.BASulfate
                Return LipidMsmsCharacterization.JudgeIfSterolSulfate(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                ' added 190811
            Case LbmClass.CerP
                Return LipidMsmsCharacterization.JudgeIfCeramidePhosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                ''' 2019/11/25 add
            Case LbmClass.SMGDG
                Return LipidMsmsCharacterization.JudgeIfSmgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.EtherSMGDG
                Return LipidMsmsCharacterization.JudgeIfEtherSmgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                'add 20200218
            Case LbmClass.LCAE
                Return LipidMsmsCharacterization.JudgeIfLcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.KLCAE
                Return LipidMsmsCharacterization.JudgeIfKlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

            Case LbmClass.KDCAE
                Return LipidMsmsCharacterization.JudgeIfKdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

                'add 20200714
            Case LbmClass.DMPE
                Return LipidMsmsCharacterization.JudgeIfDiMethylPE(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MMPE
                Return LipidMsmsCharacterization.JudgeIfMonoMethylPE(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.MIPC
                Return LipidMsmsCharacterization.JudgeIfMipc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

                'add 20200720
            Case LbmClass.EGSE
                Return LipidMsmsCharacterization.JudgeIfErgoSESpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.DEGSE
                Return LipidMsmsCharacterization.JudgeIfDehydroErgoSESpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                'add 20200812
            Case LbmClass.OxTG
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfOxTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, totalOxidized, adduct)
            Case LbmClass.TG_EST
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Dim sn3Carbon = molecule.Sn3CarbonCount
                Dim sn3DbBond = molecule.Sn3DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfFahfaTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, sn3Carbon, sn3Carbon, sn3DbBond, sn3DbBond, adduct)
                'add 20200923
            Case LbmClass.DSMSE
                Return LipidMsmsCharacterization.JudgeIfDesmosterolSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                'add20210216
            Case LbmClass.GPNAE
                Return LipidMsmsCharacterization.JudgeIfGpnae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.MGMG
                Return LipidMsmsCharacterization.JudgeIfMgmg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.DGMG
                Return LipidMsmsCharacterization.JudgeIfDgmg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                'add 20210315-
            Case LbmClass.GD1a
                Return LipidMsmsCharacterization.JudgeIfGD1a(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GD1b
                Return LipidMsmsCharacterization.JudgeIfGD1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GD2
                Return LipidMsmsCharacterization.JudgeIfGD2(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GD3
                Return LipidMsmsCharacterization.JudgeIfGD3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GM1
                Return LipidMsmsCharacterization.JudgeIfGM1(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GQ1b
                Return LipidMsmsCharacterization.JudgeIfGQ1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.GT1b
                Return LipidMsmsCharacterization.JudgeIfGT1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.NGcGM3
                Return LipidMsmsCharacterization.JudgeIfNGcGM3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)

            Case LbmClass.ST
                Return LipidMsmsCharacterization.JudgeIfnoChainSterol(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.CSLPHex, LbmClass.BRSLPHex, LbmClass.CASLPHex, LbmClass.SISLPHex, LbmClass.STSLPHex
                Return LipidMsmsCharacterization.JudgeIfSteroidWithLpa(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

            Case LbmClass.CSPHex, LbmClass.BRSPHex, LbmClass.CASPHex, LbmClass.SISPHex, LbmClass.STSPHex
                Return LipidMsmsCharacterization.JudgeIfSteroidWithPa(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20220201
            Case LbmClass.SPE
                Return LipidMsmsCharacterization.JudgeIfSpeSpecies(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                '20220322
            Case LbmClass.NAPhe
                Return LipidMsmsCharacterization.JudgeIfNAcylPheFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NATau
                Return LipidMsmsCharacterization.JudgeIfNAcylTauFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                '20221019
            Case LbmClass.PT
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylThreonine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20230407
            Case LbmClass.PC_d5
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylcholineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PE_d5
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylethanolamineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PS_d5
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylserineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PG_d5
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylglycerolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.PI_d5
                Return LipidMsmsCharacterization.JudgeIfPhosphatidylinositolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LPC_d5
                Return LipidMsmsCharacterization.JudgeIfLysopcD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LPE_d5
                Return LipidMsmsCharacterization.JudgeIfLysopeD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LPG_d5
                Return LipidMsmsCharacterization.JudgeIfLysopgD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LPI_d5
                Return LipidMsmsCharacterization.JudgeIfLysopiD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.LPS_d5
                Return LipidMsmsCharacterization.JudgeIfLysopsD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.TG_d5
                Dim sn2Carbon = molecule.Sn2CarbonCount
                Dim sn2DbBond = molecule.Sn2DoubleBondCount
                Return LipidMsmsCharacterization.JudgeIfTriacylglycerolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, sn2Carbon, sn2Carbon, sn2DbBond, sn2DbBond, adduct)
            Case LbmClass.DG_d5
                Return LipidMsmsCharacterization.JudgeIfDagD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.SM_d9
                Return LipidMsmsCharacterization.JudgeIfSphingomyelinD9(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
            Case LbmClass.CE_d7
                Return LipidMsmsCharacterization.JudgeIfCholesterylEsterD7(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
            Case LbmClass.Cer_NS_d7
                Return LipidMsmsCharacterization.JudgeIfCeramidensD7(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20230424
            Case LbmClass.bmPC
                Return LipidMsmsCharacterization.JudgeIfBetaMethylPhosphatidylcholine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20230612
            Case LbmClass.NATryA
                Return LipidMsmsCharacterization.JudgeIfNAcylTryA(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NA5HT
                Return LipidMsmsCharacterization.JudgeIfNAcyl5HT(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.WE
                Return LipidMsmsCharacterization.JudgeIfWaxEster(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1Carbon, sn1Carbon, sn1DbBond, sn1DbBond, adduct)
                '20230626
            Case LbmClass.NAAla
                Return LipidMsmsCharacterization.JudgeIfNAcylAla(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NAGln
                Return LipidMsmsCharacterization.JudgeIfNAcylGln(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NALeu
                Return LipidMsmsCharacterization.JudgeIfNAcylLeu(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NAVal
                Return LipidMsmsCharacterization.JudgeIfNAcylVal(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.NASer
                Return LipidMsmsCharacterization.JudgeIfNAcylSer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case LbmClass.BisMeLPA
                Return LipidMsmsCharacterization.JudgeIfBismelpa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' This program will return so called reverse dot product similarity as described in the previous resport.
    ''' Stein, S. E. An Integrated Method for Spectrum Extraction. J.Am.Soc.Mass.Spectrom, 10, 770-781, 1999.
    ''' The spectrum similarity of MS/MS with respect to library spectrum will be calculated in this method.
    ''' </summary>
    ''' <paramname="peaks1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <paramname="peaks2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <paramname="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetReverseDotProduct(ByVal prop1 As IMSScanProperty, ByVal prop2 As IMSScanProperty, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double
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
    ''' <paramname="peaks1">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <paramname="peaks2">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <paramname="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetWeightedDotProduct(ByVal prop1 As IMSScanProperty, ByVal prop2 As IMSScanProperty, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double
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

    Public Shared Function GetSimpleDotProduct(ByVal prop1 As IMSScanProperty, ByVal prop2 As IMSScanProperty, ByVal bin As Double, ByVal massBegin As Double, ByVal massEnd As Double) As Double
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

    Public Shared Function GetGaussianSimilarity(ByVal actual As IChromX, ByVal reference As IChromX, ByVal tolerance As Double, <Out> ByRef isInTolerance As Boolean) As Double
        isInTolerance = False
        If actual Is Nothing OrElse reference Is Nothing Then Return -1
        If actual.Value <= 0 OrElse reference.Value <= 0 Then Return -1
        If std.Abs(actual.Value - reference.Value) <= tolerance Then isInTolerance = True
        Dim similarity = GetGaussianSimilarity(actual.Value, reference.Value, tolerance)
        Return similarity
    End Function

    Public Shared Function GetGaussianSimilarity(ByVal actual As Double, ByVal reference As Double, ByVal tolerance As Double, <Out> ByRef isInTolerance As Boolean) As Double
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
    ''' <paramname="actual">
    ''' Add the experimental m/z or retention time.
    ''' </param>
    ''' <paramname="reference">
    ''' Add the theoretical m/z or library's retention time.
    ''' </param>
    ''' <paramname="tolrance">
    ''' Add the user-defined search tolerance.
    ''' </param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetGaussianSimilarity(ByVal actual As Double, ByVal reference As Double, ByVal tolrance As Double) As Double
        Return std.Exp(-0.5 * std.Pow((actual - reference) / tolrance, 2))
    End Function

    ''' <summary>
    ''' MS-DIAL program utilizes the total similarity score to rank the compound candidates.
    ''' This method is to calculate it from four scores including RT, isotopic ratios, m/z, and MS/MS similarities.
    ''' </summary>
    ''' <paramname="accurateMassSimilarity"></param>
    ''' <paramname="rtSimilarity"></param>
    ''' <paramname="isotopeSimilarity"></param>
    ''' <paramname="spectraSimilarity"></param>
    ''' <paramname="reverseSearchSimilarity"></param>
    ''' <paramname="presenceSimilarity"></param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetTotalSimilarity(ByVal accurateMassSimilarity As Double, ByVal rtSimilarity As Double, ByVal isotopeSimilarity As Double, ByVal spectraSimilarity As Double, ByVal reverseSearchSimilarity As Double, ByVal presenceSimilarity As Double, ByVal spectrumPenalty As Boolean, ByVal targetOmics As TargetOmics, ByVal isUseRT As Boolean) As Double
        Dim dotProductFactor = 3.0
        Dim revesrseDotProdFactor = 2.0
        Dim presensePercentageFactor = 1.0

        Dim msmsFactor = 2.0
        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0

        If targetOmics = targetOmics.Lipidomics Then
            dotProductFactor = 1.0
            revesrseDotProdFactor = 2.0
            presensePercentageFactor = 3.0
            msmsFactor = 1.5
            rtFactor = 0.5
        End If

        Dim msmsSimilarity = (dotProductFactor * spectraSimilarity + revesrseDotProdFactor * reverseSearchSimilarity + presensePercentageFactor * presenceSimilarity) / (dotProductFactor + revesrseDotProdFactor + presensePercentageFactor)

        If spectrumPenalty = True AndAlso targetOmics = targetOmics.Metabolomics Then msmsSimilarity = msmsSimilarity * 0.5

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

    Public Shared Function GetTotalSimilarity(ByVal accurateMassSimilarity As Double, ByVal rtSimilarity As Double, ByVal ccsSimilarity As Double, ByVal isotopeSimilarity As Double, ByVal spectraSimilarity As Double, ByVal reverseSearchSimilarity As Double, ByVal presenceSimilarity As Double, ByVal spectrumPenalty As Boolean, ByVal targetOmics As TargetOmics, ByVal isUseRT As Boolean, ByVal isUseCcs As Boolean) As Double
        Dim dotProductFactor = 3.0
        Dim revesrseDotProdFactor = 2.0
        Dim presensePercentageFactor = 1.0

        Dim msmsFactor = 2.0
        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0
        Dim ccsFactor = 2.0

        If targetOmics = targetOmics.Lipidomics Then
            dotProductFactor = 1.0
            revesrseDotProdFactor = 2.0
            presensePercentageFactor = 3.0
            msmsFactor = 1.5
            rtFactor = 0.5
            ccsFactor = 1.0F
        End If

        Dim msmsSimilarity = (dotProductFactor * spectraSimilarity + revesrseDotProdFactor * reverseSearchSimilarity + presensePercentageFactor * presenceSimilarity) / (dotProductFactor + revesrseDotProdFactor + presensePercentageFactor)

        If spectrumPenalty = True AndAlso targetOmics = targetOmics.Metabolomics Then msmsSimilarity = msmsSimilarity * 0.5

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
    ''' <paramname="accurateMassSimilarity"></param>
    ''' <paramname="rtSimilarity"></param>
    ''' <paramname="isotopeSimilarity"></param>
    ''' <returns>
    ''' The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' </returns>
    Public Shared Function GetTotalSimilarity(ByVal accurateMassSimilarity As Double, ByVal rtSimilarity As Double, ByVal isotopeSimilarity As Double, ByVal isUseRT As Boolean) As Double
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

    Public Shared Function GetTotalSimilarity(ByVal accurateMassSimilarity As Double, ByVal rtSimilarity As Double, ByVal ccsSimilarity As Double, ByVal isotopeSimilarity As Double, ByVal isUseRT As Boolean, ByVal isUseCcs As Boolean) As Double

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

    Public Shared Function GetTotalSimilarityUsingSimpleDotProduct(ByVal accurateMassSimilarity As Double, ByVal rtSimilarity As Double, ByVal isotopeSimilarity As Double, ByVal dotProductSimilarity As Double, ByVal spectrumPenalty As Boolean, ByVal targetOmics As TargetOmics, ByVal isUseRT As Boolean) As Double
        Dim msmsFactor = 2.0
        Dim rtFactor = 1.0
        Dim massFactor = 1.0
        Dim isotopeFactor = 0.0

        Dim msmsSimilarity = dotProductSimilarity

        If spectrumPenalty = True AndAlso targetOmics = targetOmics.Metabolomics Then msmsSimilarity = msmsSimilarity * 0.5

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

    Public Shared Function GetTotalScore(ByVal result As MsScanMatchResult, ByVal param As MsRefSearchParameterBase) As Double
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

    Public Shared Function GetTotalSimilarity(ByVal rtSimilarity As Double, ByVal eiSimilarity As Double, ByVal Optional isUseRT As Boolean = True) As Double
        If rtSimilarity < 0 OrElse Not isUseRT Then
            Return eiSimilarity
        Else
            Return 0.6 * eiSimilarity + 0.4 * rtSimilarity
        End If
    End Function
End Class

