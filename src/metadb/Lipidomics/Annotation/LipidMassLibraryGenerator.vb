Imports System.IO
Imports System.Text
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSEngine

Public NotInheritable Class LipidMassLibraryGenerator

    Private Sub New()
    End Sub

    Public Shared Function Run(
                          lipidclass As LbmClass,
                          adduct As AdductIon,
                          minCarbonCount As Integer,
                          maxCarbonCount As Integer,
                          minDoubleBond As Integer,
                          maxDoubleBond As Integer,
                          maxOxygen As Integer)

        Select Case lipidclass
            Case LbmClass.PC
                generatePcSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PE
                generatePeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PS
                generatePsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PI
                generatePiSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PG
                generatePgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.TG
                generateTagSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SM
                generateSmSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LNAPE
                generateLnapeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                'add
            Case LbmClass.DG
                generateDagSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.MG
                generateMagSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.FA
                generateFaSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.FAHFA
                generateFahfaSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPC
                generateLpcSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPE
                generateLpeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPG
                generateLpgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPI
                generateLpiSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPS
                generateLpsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPA
                generateLpaSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PA
                generatePaSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.MGDG
                generateMgdgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SQDG
                generateSqdgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DGDG
                generateDgdgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DGTS
                generateDgtsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LDGTS
                generateLdgtsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HBMP
                generateHbmpSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.BMP
                generateBmpSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CAR
                generateAcarSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DGGA
                generateGlcadgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.ADGGA
                generateAcylglcadgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CL
                generateClSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CE
                generateCeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherPE
                generateEtherpeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherPC
                generateEtherpcSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                ' ceramide
            Case LbmClass.Cer_AP
                generateCerapSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_ADS
                generateCeradsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_AS
                generateCerasSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_NP
                generateCernpSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_NDS
                generateCerndsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_NS
                generateCernsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_BDS
                generateCerbdsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_BS
                generateCerbsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_AP
                generateHexcerapSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_NDS
                generateHexcerndsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_NS
                generateHexcernsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_EODS
                generateCereodsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_EOS
                generateCereosSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_EOS
                generateHexcereosSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                ' OxPls
            Case LbmClass.OxFA
                generateOxfaSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPC
                generateOxpcSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPE
                generateOxpeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPG
                generateOxpgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPI
                generateOxpiSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPS
                generateOxpsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.EtherOxPC
                generateOxpcEtherSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.EtherOxPE
                generateOxpeEtherSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.PEtOH
                generatePetohSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PMeOH
                generatePmeohSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PBtOH
                generatePbtohSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.GM3
                generateGm3Species(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LNAPS
                generateLnapsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherLPE
                generateEtherlpeSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherLPC
                generateEtherlpcSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Hex2Cer
                generateHexhexcernsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Hex3Cer
                generateHexhexhexcernsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_EBDS
                generateAcylcerbdsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexCer
                generateAcylhexcerasSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.ASM
                generateAcylsmSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.Cer_OS
                generateCerosSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.MLCL
                generateLclSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DLCL
                generateDlclSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PhytoSph
                generatePhytosphingosineSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Sph
                generateSphingosineSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DHSph
                generateSphinganineSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherMGDG
                generateEthermgdgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherDGDG
                generateEtherdgdgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherTG
                generateEthertagSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                'add 10/04/19
            Case LbmClass.EtherPI
                generateEtherpiSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherPS
                generateEtherpsSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PE_Cer
                generatePetceramideSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                generatePedceramideSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                ' add 13/05/19
            Case LbmClass.DCAE
                generateDcaesSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.GDCAE
                generateGdcaesSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.GLCAE
                generateGlcaesSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.TDCAE
                generateTdcaesSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.TLCAE
                generateTlcaesSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.NAE
                generateAnandamideSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.NAGly
                generateFAHFAmideGlySpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.NAGlySer
                generateFAHFAmideGlySerSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SL
                generateSulfonolipidSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.EtherPG
                generateEtherpgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherLPG
                generateEtherlpgSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PI_Cer
                generatePiceramideDihydroxySpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
                generatePiceramideTrihydroxySpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
                generatePiceramideOxDihydroxySpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.SHexCer
                generateShexcerSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                generateShexcerOxSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.CoQ
                generateCoenzymeqSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount)

            Case LbmClass.Vitamin_E
                generateVitaminESpecies(outputfolder, adduct)
                generateVitaminDSpecies(outputfolder, adduct)

            Case LbmClass.VAE
                generateVitaminASpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.NAOrn
                generateFAHFAmideOrnSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.BRSE
                generateBrseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CASE
                generateCaseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SISE
                generateSiseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.STSE
                generateStseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.AHexBRS
                generateAhexbrseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexCAS
                generateAhexcaseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexCS
                generateAhexceSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexSIS
                generateAhexsiseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexSTS
                generateAhexstseSpecies(outputfolder, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

        End Select
    End Function

    Private Shared Function generatePsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Return commonGlycerolipidsGenerator("PS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        commonGlycerolipidsGenerator("PE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        commonGlycerolipidsGenerator("PC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        commonGlycerolipidsGenerator("PI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        commonGlycerolipidsGenerator("PG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generateTagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "TAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H5O3")
        commonGlycerolipidsGenerator("TAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function


    Private Shared Function generateLnapeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LNAPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        commonGlycerolipidsGenerator("LNAPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    'add
    Private Shared Function generateDagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H6O3")
        commonGlycerolipidsGenerator("DAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateMagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "MAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H7O3")
        commonGlycerolipidsGenerator("MAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateFaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("HO")
        commonGlycerolipidsGenerator("FA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateFahfaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("HO")
        fahfaGenerator("FAHFA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateLpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H19NO6P")
        commonGlycerolipidsGenerator("LPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO6P")
        commonGlycerolipidsGenerator("LPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H14O8P")
        commonGlycerolipidsGenerator("LPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H18O11P")
        commonGlycerolipidsGenerator("LPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13NO8P")
        commonGlycerolipidsGenerator("LPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H8O6P")
        commonGlycerolipidsGenerator("LPA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generatePaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H7O6P")
        commonGlycerolipidsGenerator("PA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateMgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "MGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H16O8")
        commonGlycerolipidsGenerator("MGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateSqdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SQDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H16O10S")
        commonGlycerolipidsGenerator("SQDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateDgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C15H26O13")
        commonGlycerolipidsGenerator("DGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateDgtsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DGTS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C10H19NO5")
        commonGlycerolipidsGenerator("DGTS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateLdgtsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LDGTS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C10H20NO5")
        commonGlycerolipidsGenerator("LDGTS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateHbmpSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HBMP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        commonGlycerolipidsGenerator("HBMP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function
    Private Shared Function generateBmpSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "BMP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        commonGlycerolipidsGenerator("BMP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateAcarSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "ACar" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C7H15O3N")
        commonGlycerolipidsGenerator("ACar", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateGlcadgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GlcADG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H14O9")
        commonGlycerolipidsGenerator("GlcADG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateAcylglcadgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylGlcADG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H13O9")
        commonGlycerolipidsGenerator("AcylGlcADG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function
    Private Shared Function generateClSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CL" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H18O13P2")
        commonGlycerolipidsGenerator("CL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 4)
    End Function
    Private Shared Function generateCeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C27H45O")
        commonGlycerolipidsGenerator("CE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateEtherpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        commonGlycerolipidsEtherGenerator("EtherPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generateEtherpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        commonGlycerolipidsEtherGenerator("EtherPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    ' ceramide
    Private Shared Function generateSmSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SM" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("SM", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    'add MT
    Private Shared Function generateCerapSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_AP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("Cer_AP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCeradsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_ADS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("Cer_ADS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerasSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_AS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("Cer_AS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCernpSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_NP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("Cer_NP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerndsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_NDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("Cer_NDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("Cer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerbdsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_BDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("Cer_BDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerbsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_BS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("Cer_BS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcerapSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_AP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("HexCer_AP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcerndsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_NDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("HexCer_NDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("HexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCereodsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_EODS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator("Cer_EODS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCereosSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_EOS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator("Cer_EOS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcereosSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_EOS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator("HexCer_EOS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function

    '
    Private Shared Function generateOxfaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxFA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("HO")
        Dim acylCount = 1
        commonGlycerolipidsOxGenerator("OxFA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator("OxPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator("OxPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator("OxPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator("OxPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator("OxPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpcEtherSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherOxPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Dim acylCount = 2
        Dim etherCount = 1
        commonGlycerolipidsOxEtherGenerator("EtherOxPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount, etherCount)
    End Function
    Private Shared Function generateOxpeEtherSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherOxPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Dim acylCount = 2
        Dim etherCount = 1
        commonGlycerolipidsOxEtherGenerator("EtherOxPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount, etherCount)
    End Function
    'others
    Private Shared Function generatePetohSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PEtOH" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H11O6P")
        commonGlycerolipidsGenerator("PEtOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generatePmeohSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PMeOH" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C4H9O6P")
        commonGlycerolipidsGenerator("PMeOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generatePbtohSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PBtOH" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C7H15O6P")
        commonGlycerolipidsGenerator("PBtOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateGm3Species(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GM3" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C23H38NO18")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("GM3", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateShexcerSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SHexCer" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O8S")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("SHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateLnapsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LNAPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        commonGlycerolipidsGenerator("LNAPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateEtherlpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherLPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO6P")
        commonGlycerolipidsEtherGenerator("EtherLPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1, 1)
    End Function

    Private Shared Function generateEtherlpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherLPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H19NO6P")
        commonGlycerolipidsEtherGenerator("EtherLPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1, 1)
    End Function
    Private Shared Function generateHexhexcernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexHexCer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C12H21O10")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("HexHexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexhexhexcernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexHexHexCer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C18H31O15")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("HexHexHexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateAcylcerbdsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylCer_BDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator("AcylCer_BDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateAcylhexcerasSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylHexCer" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator("AcylHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateAcylsmSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylSM" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideEsterGenerator("AcylSM", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerosSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_OS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator("Cer_OS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateLclSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LCL" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H19O13P2")
        commonGlycerolipidsGenerator("LCL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function
    Private Shared Function generateDlclSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DLCL" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H20O13P2")
        commonGlycerolipidsGenerator("DLCL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generatePhytosphingosineSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Phytosphingosine" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 3
        commonSphingosineGenerator("Phytosphingosine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Function
    Private Shared Function generateSphingosineSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Sphingosine" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 2
        commonSphingosineGenerator("Sphingosine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Function
    Private Shared Function generateSphinganineSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Sphinganine" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 2
        commonSphingosineGenerator("Sphinganine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Function
    Private Shared Function generateEthermgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherMGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H16O8")
        commonGlycerolipidsEtherGenerator("EtherMGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    Private Shared Function generateEtherdgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherDGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C15H26O13")
        commonGlycerolipidsEtherGenerator("EtherDGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generateEthertagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherTAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H5O3")
        commonGlycerolipidsEtherGenerator("EtherTAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3, 1)
    End Function

    ' add 10/04/19
    Private Shared Function generateEtherpiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        commonGlycerolipidsEtherGenerator("EtherPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    Private Shared Function generateEtherpsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        commonGlycerolipidsEtherGenerator("EtherPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    Private Shared Function generatePetceramideSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PE-Cer(t)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H7NO3P")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("PE_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generatePedceramideSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PE-Cer(d)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H7NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("PE_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function

    ''' add 13/05/19
    Private Shared Function generateDcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C24H39O4")
        commonGlycerolipidsGenerator("DCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateGdcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GDCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H42NO5")
        commonGlycerolipidsGenerator("GDCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateGlcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GLCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H42NO4")
        commonGlycerolipidsGenerator("GLCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateTdcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "TDCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H44NO6S")
        commonGlycerolipidsGenerator("TDCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateTlcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "TLCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H44NO5S")
        commonGlycerolipidsGenerator("TLCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateAnandamideSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Anandamide" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H6NO")
        commonGlycerolipidsGenerator("NAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateFAHFAmideGlySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFAmide(Gly)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H4NO2")
        fahfaGenerator("NAAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateFAHFAmideGlySerSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFAmide(GlySer)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H9N2O4")
        fahfaGenerator("NAAGS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateSulfonolipidSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, oxygenCount As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Sulfonolipid" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("SO3H")
        commonSulfonolipidGenerator("SL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, oxygenCount)
    End Function

    Private Shared Function generateEtherpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        commonGlycerolipidsEtherGenerator("EtherPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generateEtherlpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherLPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H15O7P")
        commonGlycerolipidsEtherGenerator("EtherLPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generatePiceramideDihydroxySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI-Cer(d)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generatePiceramideTrihydroxySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI-Cer(t)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        commonCeramideGenerator("PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generatePiceramideOxDihydroxySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI-Cer(d_O)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideOxGenerator("PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateShexcerOxSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SHexCerO" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O8S")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideOxGenerator("SHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCoenzymeqSpecies(adduct As AdductIon, minRepeatCount As Integer, maxRepeatCount As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CoQ" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H10O4")
        Dim additionalFormula = OrganicElementsReader("C5H8")
        commonCoenzymeqlipidsGenerator("CoQ", headerFormula, adduct, minRepeatCount, maxRepeatCount, additionalFormula)
    End Function

    Private Shared Function generateVitaminESpecies(adduct As AdductIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString.Substring(adductString.Length - 1, 1), "-") Then
            Dim filepath = outputfolder & "\" & "VitaminE" & "_" & adductString & ".txt"
            Dim formula = OrganicElementsReader("C29H50O2")
            commonSinglemoleculeGenerator("Vitamin", formula, adduct)
        End If
    End Function
    Private Shared Function generateVitaminDSpecies(adduct As AdductIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString, "[M+H]+") Then
            Dim filepath = outputfolder & "\" & "VitaminD" & "_" & adductString & ".txt"
            Dim formula = OrganicElementsReader("C27H44O2")
            commonSinglemoleculeGenerator("Vitamin", formula, adduct)
        End If
    End Function
    Private Shared Function generateVitaminASpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString.Substring(adductString.Length - 1, 1), "+") Then
            Dim filepath = outputfolder & "\" & "VitaminA" & "_" & adductString & ".txt"
            Dim headerFormula = OrganicElementsReader("C20H29O")
            commonGlycerolipidsGenerator("VAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
        End If
    End Function

    Private Shared Function generateFAHFAmideOrnSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFAmide(Orn)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H11N2O2")
        fahfaGenerator("NAAO", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateBrseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "BRSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C28H45O")
        commonGlycerolipidsGenerator("BRSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateCaseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CASE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C28H47O")
        commonGlycerolipidsGenerator("CASE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateSiseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SISE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C29H49O")
        commonGlycerolipidsGenerator("SISE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateStseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "STSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C29H47O")
        commonGlycerolipidsGenerator("STSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateAhexbrseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexBRSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C34H55O6")
        commonGlycerolipidsGenerator("AHexBRSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateAhexcaseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexCASE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C34H57O6")
        commonGlycerolipidsGenerator("AHexCASE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateAhexceSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexCE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C33H55O6")
        commonGlycerolipidsGenerator("AHexCE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateAhexsiseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexSISE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C35H59O6")
        commonGlycerolipidsGenerator("AHexSISE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateAhexstseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        Dim headerFormula = OrganicElementsReader("C35H57O6")
        commonGlycerolipidsGenerator("AHexSTSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function



    Private Shared Iterator Function commonGlycerolipidsOxGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer, acylCount As Integer) As IEnumerable(Of LipidIon) As IEnumerable(Of LipidIon)

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                For k = 1 To maxOxygen

                    If j + 1 < k Then Continue For
                    'if (K + j > maxDoubleBond)        // extra Hydroxy + Doublebond < maxDoubleBond 
                    '    continue;

                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - acylCount - totalChainDoubleBond * 2
                        Dim totalChainOxygen = acylCount + k

                        Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                        Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "+" & k.ToString() & "O"

                        Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                    End If
                Next


            Next
        Next
    End Function
    Private Shared Iterator Function commonGlycerolipidsOxEtherGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer, acylCount As Integer, etherCount As Integer) As IEnumerable(Of LipidIon)

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                For k = 1 To maxOxygen

                    If j + 1 < k Then Continue For
                    'if (K + j > maxDoubleBond)        // extra Hydroxy + Doublebond < maxDoubleBond 
                    '    continue;

                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - acylCount - totalChainDoubleBond * 2 + etherCount * 2
                        Dim totalChainOxygen = acylCount + k - 1

                        Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                        Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "e+" & k.ToString() & "O"

                        Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                    End If
                Next
            Next
        Next
    End Function

    Private Shared Iterator Function fahfaGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                If isPracticalDoubleBondSize(i, j) Then

                    Dim totalChainCarbon = i
                    Dim totalChainDoubleBond = j
                    Dim totalChainHydrogen = totalChainCarbon * 2 - 2 * totalChainDoubleBond - 3 ' scafold FA
                    Dim totalChainOxygen = 3

                    Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                    Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                    Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                    Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                End If
            Next
        Next
    End Function


    Private Shared Iterator Function commonCeramideGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, sphingoHydroxyCount As Integer, acylHydroxyCount As Integer) As IEnumerable(Of LipidIon)

        Dim hydroHeader = "d"
        If sphingoHydroxyCount = 3 Then
            hydroHeader = "t"
        End If

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                If isPracticalDoubleBondSize(i, j) Then

                    Dim totalChainCarbon = i
                    Dim totalChainDoubleBond = j
                    Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2
                    Dim totalChainOxygen = 1 + sphingoHydroxyCount + acylHydroxyCount
                    Dim totalNitrogenCount = 1

                    Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N + totalNitrogenCount, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                    Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                    Dim lipidname = classString & " " & hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                    Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                End If
            Next
        Next
    End Function

    Private Shared Iterator Function commonGlycerolipidsGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, acylCount As Integer) As IEnumerable(Of LipidIon)

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                If isPracticalDoubleBondSize(i, j) Then

                    Dim totalChainCarbon = i
                    Dim totalChainDoubleBond = j
                    Dim totalChainHydrogen = totalChainCarbon * 2 - acylCount - totalChainDoubleBond * 2
                    Dim totalChainOxygen = acylCount

                    Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                    Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                    Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                    Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                End If
            Next
        Next
    End Function
    'add MT
    Private Shared Iterator Function commonGlycerolipidsEtherGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, acylCount As Integer, etherCount As Integer) As IEnumerable(Of LipidIon)

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                If isPracticalDoubleBondSize(i, j) Then

                    Dim totalChainCarbon = i
                    Dim totalChainDoubleBond = j
                    Dim totalChainHydrogen = totalChainCarbon * 2 - acylCount - totalChainDoubleBond * 2 + etherCount * 2
                    Dim totalChainOxygen = acylCount - 1

                    Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                    Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                    Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "e"

                    Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                End If
            Next
        Next
    End Function

    Private Shared Iterator Function commonCeramideEsterGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, sphingoHydroxyCount As Integer, acylHydroxyCount As Integer) As IEnumerable(Of LipidIon)
        Dim hydroHeader = "d"
        If sphingoHydroxyCount = 3 Then
            hydroHeader = "t"
        End If

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                If isPracticalDoubleBondSize(i, j) Then

                    Dim totalChainCarbon = i
                    Dim totalChainDoubleBond = j
                    Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2 - 2
                    Dim totalChainOxygen = 2 + sphingoHydroxyCount + acylHydroxyCount
                    Dim totalNitrogenCount = 1

                    Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N + totalNitrogenCount, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                    Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                    Dim lipidname = classString & " " & hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                    Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                End If
            Next
        Next
    End Function

    Private Shared Iterator Function commonSphingosineGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, HydroxyCount As Integer) As IEnumerable(Of LipidIon)

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                If isPracticalDoubleBondSize(i, j) Then

                    Dim totalChainCarbon = i
                    Dim totalChainDoubleBond = j
                    Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2 + 2
                    Dim totalChainOxygen = HydroxyCount
                    Dim totalNitrogenCount = 1

                    Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N + totalNitrogenCount, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                    Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                    Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                    Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                End If
            Next
        Next
    End Function

    Private Shared Iterator Function commonSulfonolipidGenerator(classString As String,
                                                                 headerFormula As DerivatizationFormula,
                                                                 adduct As AdductIon,
                                                                 minCarbonCount As Integer,
                                                                 maxCarbonCount As Integer,
                                                                 minDoubleBond As Integer,
                                                                 maxDoubleBond As Integer,
                                                                 acylHydroxyCount As Integer) As IEnumerable(Of LipidIon)
        Dim hydroHeader = "m"

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                For k = 0 To acylHydroxyCount
                    If isPracticalDoubleBondSize(i, j) Then
                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2
                        Dim totalChainOxygen = 1 + 1 + k
                        Dim totalNitrogenCount = 1

                        Dim totalString = String.Empty

                        totalString = If(k = 0, hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString(), hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "+" & k.ToString() & "O")

                        Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N + totalNitrogenCount, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                        Dim lipidname = classString & " " & totalString

                        Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                    End If
                Next
            Next
        Next
    End Function

    Private Shared Iterator Function commonCeramideOxGenerator(classString As String,
                                                               headerFormula As DerivatizationFormula,
                                                               adduct As AdductIon,
                                                               minCarbonCount As Integer,
                                                               maxCarbonCount As Integer,
                                                               minDoubleBond As Integer,
                                                               maxDoubleBond As Integer,
                                                               sphingoHydroxyCount As Integer,
                                                               acylHydroxyCount As Integer) As IEnumerable(Of LipidIon)
        Dim hydroHeader = "d"

        If sphingoHydroxyCount = 3 Then
            hydroHeader = "t"
        End If

        For i = minCarbonCount To maxCarbonCount
            For j = minDoubleBond To maxDoubleBond
                For k = 1 To acylHydroxyCount

                    If isPracticalDoubleBondSize(i, j) Then
                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2
                        Dim totalChainOxygen = 1 + sphingoHydroxyCount + k
                        Dim totalNitrogenCount = 1

                        Dim totalString = String.Empty

                        totalString = If(k = 1, hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "+" & "O", hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "+" & k.ToString() & "O")

                        Dim totalFormula = New DerivatizationFormula(headerFormula!C + totalChainCarbon, headerFormula!H + totalChainHydrogen, headerFormula!N + totalNitrogenCount, headerFormula!O + totalChainOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
                        Dim lipidname = classString & " " & totalString

                        Yield New LipidIon With {
                            .lipidname = lipidname,
                            .total_formula = totalFormula.EmpiricalFormula,
                            .mz = mz,
                            .adduct = adduct.AdductIonName
                        }
                    End If
                Next
            Next
        Next
    End Function

    Private Shared Iterator Function commonCoenzymeqlipidsGenerator(classString As String,
                                                                    headerFormula As DerivatizationFormula,
                                                                    adduct As AdductIon,
                                                                    minRepeatCount As Integer,
                                                                    maxRepeatCount As Integer,
                                                                    additionalFormula As DerivatizationFormula) As IEnumerable(Of LipidIon)

        For i = minRepeatCount To maxRepeatCount
            Dim totalCarbon = headerFormula!C + additionalFormula!C * i
            Dim totalHydrogen = headerFormula!H + additionalFormula!H * i
            Dim totalOxygen = headerFormula!O + additionalFormula!O * i
            Dim totalNitrogen = headerFormula!N + additionalFormula!N * i

            Dim totalFormula = New DerivatizationFormula(totalCarbon, totalHydrogen, totalNitrogen, totalOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
            Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
            Dim lipidname = classString

            Yield New LipidIon With {
                .lipidname = lipidname,
                .total_formula = totalFormula.EmpiricalFormula,
                .mz = mz,
                .adduct = adduct.AdductIonName
            }
        Next
    End Function

    Private Shared Iterator Function commonSinglemoleculeGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon) As IEnumerable(Of LipidIon)
        Dim totalCarbon = headerFormula!C
        Dim totalHydrogen = headerFormula!H
        Dim totalOxygen = headerFormula!O
        Dim totalNitrogen = headerFormula!N

        Dim totalFormula = New DerivatizationFormula(totalCarbon, totalHydrogen, totalNitrogen, totalOxygen, headerFormula!P, headerFormula!S, 0, 0, 0, 0, 0)
        Dim mz = adduct.ConvertToMz(totalFormula.ExactMass)
        Dim lipidname = classString

        Yield New LipidIon With {
            .adduct = adduct.AdductIonName,
            .lipidname = lipidname,
            .mz = mz,
            .total_formula = totalFormula.EmpiricalFormula
        }
    End Function

    Private Shared Function isPracticalDoubleBondSize(carbon As Integer, doublebond As Integer) As Boolean
        If doublebond = 0 Then
            Return True
        ElseIf carbon / doublebond < 3.5 Then
            Return False
        Else
            Return True
        End If
    End Function
End Class

Public Class LipidIon

    Public Property lipidname As String
    Public Property mz As Double
    Public Property adduct As String
    Public Property total_formula As String
    Public Property [class] As LbmClass

End Class

