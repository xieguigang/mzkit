Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep

Public NotInheritable Class MsScanMatching

    Private Sub New()
    End Sub

    Public Shared Function CompareMS2LipidomicsScanProperties(scanProp As IMSScanProperty, refSpec As MoleculeMsReference, param As MsRefSearchParameterBase) As MsScanMatchResult

        Dim result = MSEngine.MsScanMatching.CompareBasicMSScanProperties(scanProp, refSpec, param, param.Ms2Tolerance, param.MassRangeBegin, param.MassRangeEnd)
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
        result.TotalScore = CSng(MSEngine.MsScanMatching.GetTotalScore(result, param))

        Return result
    End Function


    Public Shared Function GetOadBasedLipidomicsMatchedPeaksScores(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As Double()

        Dim returnedObj = GetOadBasedLipidMoleculeAnnotationResult(scan, reference, tolerance, mzBegin, mzEnd)
        Return returnedObj.Item2
    End Function

    Public Shared Function GetOadBasedLipidMoleculeAnnotationResult(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
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


    Public Shared Function GetEidBasedLipidomicsMatchedPeaksScores(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As Double()

        Dim returnedObj = GetEidBasedLipidMoleculeAnnotationResult(scan, reference, tolerance, mzBegin, mzEnd)
        Return returnedObj.Item2
    End Function

    Public Shared Function GetEidBasedLipidMoleculeAnnotationResult(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
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

    Public Shared Function GetEieioBasedLipidomicsMatchedPeaksScores(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As Double()

        Dim returnedObj = GetEieioBasedLipidMoleculeAnnotationResult(scan, reference, tolerance, mzBegin, mzEnd)
        Return returnedObj.Item2
    End Function

    Public Shared Function GetEieioBasedLipidMoleculeAnnotationResult(scan As IMSScanProperty, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())
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


    Public Shared Function GetLipidMoleculerSpeciesLevelAnnotationResultForEIEIO(msScanProp As IMSScanProperty, molecule As LipidMolecule, ms2tol As Double) As LipidMolecule
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

                ' 2019/11/25 add
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

    Public Shared Function GetLipidMoleculeAnnotationResult(msScanProp As IMSScanProperty, molecule As LipidMolecule, ms2tol As Double) As LipidMolecule

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

                ' 2019/11/25 add
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


    Public Shared Function GetLipidNameFromReference(reference As MoleculeMsReference) As String
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

    Public Shared Function GetRefinedLipidAnnotationLevel(msScanProp As IMSScanProperty, molMsRef As MoleculeMsReference, bin As Double, <Out> ByRef isLipidClassMatched As Boolean, <Out> ByRef isLipidChainMatched As Boolean, <Out> ByRef isLipidPositionMatched As Boolean, <Out> ByRef isOthers As Boolean) As String

        isLipidClassMatched = False
        isLipidChainMatched = False
        isLipidPositionMatched = False
        isOthers = False
        If Not MSEngine.MsScanMatching.IsComparedAvailable(msScanProp, molMsRef) Then Return String.Empty

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


    ''' <summary>
    ''' This method returns the presence similarity (% of matched fragments) between the experimental MS/MS spectrum and the standard MS/MS spectrum.
    ''' So, this program will calculate how many fragments of library spectrum are found in the experimental spectrum and will return the %.
    ''' double[] [0]m/z[1]intensity
    ''' 
    ''' </summary>
    ''' <param name="msScanProp">
    ''' Add the experimental MS/MS spectrum.
    ''' </param>
    ''' <param name="molMsRef">
    ''' Add the theoretical MS/MS spectrum. The theoretical MS/MS spectrum is supposed to be retreived in MSP parcer.
    ''' </param>
    ''' <param name="bin">
    ''' Add the bin value to merge the abundance of m/z.
    ''' </param>
    ''' <returns>
    ''' [0] The similarity score which is standadized from 0 (no similarity) to 1 (consistency) will be return.
    ''' [1] MatchedPeaksCount is also returned.
    ''' </returns>
    Public Shared Function GetLipidomicsMatchedPeaksScores(msScanProp As IMSScanProperty, molMsRef As MoleculeMsReference, bin As Double, massBegin As Double, massEnd As Double) As Double()
        If Not MSEngine.MsScanMatching.IsComparedAvailable(msScanProp, molMsRef) Then
            Return New Double() {-1, -1}
        End If

        ' in lipidomics project, currently, the well-known lipid classes now including
        ' PC, PE, PI, PS, PG, BMP, SM, TAG are now evaluated.
        ' if the lipid class diagnostic fragment (like m/z 184 in PC and SM in ESI(+)) is observed, 
        ' the bonus 0.5 is added to the normal presence score

        Dim resultArray = MSEngine.MsScanMatching.GetMatchedPeaksScores(msScanProp, molMsRef, bin, massBegin, massEnd) ' [0] matched ratio [1] matched count
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

    Public Shared Function GetLipidomicsMoleculerSpeciesLevelAnnotationPeaksScoresForEIEIO(msScanProp As IMSScanProperty, molMsRef As MoleculeMsReference, bin As Double, massBegin As Double, massEnd As Double) As Double()

        If Not MSEngine.MsScanMatching.IsComparedAvailable(msScanProp, molMsRef) Then Return New Double() {-1, -1}

        ' in lipidomics project, currently, the well-known lipid classes now including
        ' PC, PE, PI, PS, PG, BMP, SM, TAG are now evaluated.
        ' if the lipid class diagnostic fragment (like m/z 184 in PC and SM in ESI(+)) is observed, 
        ' the bonus 0.5 is added to the normal presence score

        Dim resultArray = MSEngine.MsScanMatching.GetMatchedPeaksScores(msScanProp, molMsRef, bin, massBegin, massEnd) ' [0] matched ratio [1] matched count
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


    Public Shared Function GetRefinedLipidAnnotationLevelForEIEIO(msScanProp As IMSScanProperty, molMsRef As MoleculeMsReference, bin As Double, <Out> ByRef isLipidClassMatched As Boolean, <Out> ByRef isLipidChainMatched As Boolean, <Out> ByRef isLipidPositionMatched As Boolean, <Out> ByRef isOthers As Boolean) As String

        isLipidClassMatched = False
        isLipidChainMatched = False
        isLipidPositionMatched = False
        isOthers = False
        If Not MSEngine.MsScanMatching.IsComparedAvailable(msScanProp, molMsRef) Then Return String.Empty

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

End Class