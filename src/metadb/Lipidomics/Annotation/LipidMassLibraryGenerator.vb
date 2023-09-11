Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSFinder
Imports Microsoft.VisualBasic.Linq

Public NotInheritable Class LipidMassLibraryGenerator

    Private Sub New()
    End Sub

    Public Shared Function GetIons(lipidclass As LbmClass,
                                   adduct As AdductIon,
                                   minCarbonCount As Integer,
                                   maxCarbonCount As Integer,
                                   minDoubleBond As Integer,
                                   maxDoubleBond As Integer,
                                   maxOxygen As Integer) As IEnumerable(Of LipidIon)

        Select Case lipidclass
            Case LbmClass.PC
                Return generatePcSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PE
                Return generatePeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PS
                Return generatePsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PI
                Return generatePiSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PG
                Return generatePgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.TG
                Return generateTagSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SM
                Return generateSmSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LNAPE
                Return generateLnapeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                'add
            Case LbmClass.DG
                Return generateDagSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.MG
                Return generateMagSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.FA
                Return generateFaSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.FAHFA
                Return generateFahfaSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPC
                Return generateLpcSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPE
                Return generateLpeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPG
                Return generateLpgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPI
                Return generateLpiSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPS
                Return generateLpsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LPA
                Return generateLpaSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PA
                Return generatePaSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.MGDG
                Return generateMgdgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SQDG
                Return generateSqdgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DGDG
                Return generateDgdgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DGTS
                Return generateDgtsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LDGTS
                Return generateLdgtsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HBMP
                Return generateHbmpSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.BMP
                Return generateBmpSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CAR
                Return generateAcarSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DGGA
                Return generateGlcadgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.ADGGA
                Return generateAcylglcadgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CL
                Return generateClSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CE
                Return generateCeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherPE
                Return generateEtherpeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherPC
                Return generateEtherpcSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                ' ceramide
            Case LbmClass.Cer_AP
                Return generateCerapSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_ADS
                Return generateCeradsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_AS
                Return generateCerasSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_NP
                Return generateCernpSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_NDS
                Return generateCerndsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_NS
                Return generateCernsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_BDS
                Return generateCerbdsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_BS
                Return generateCerbsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_AP
                Return generateHexcerapSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_NDS
                Return generateHexcerndsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_NS
                Return generateHexcernsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_EODS
                Return generateCereodsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_EOS
                Return generateCereosSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.HexCer_EOS
                Return generateHexcereosSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                ' OxPls
            Case LbmClass.OxFA
                Return generateOxfaSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPC
                Return generateOxpcSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPE
                Return generateOxpeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPG
                Return generateOxpgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPI
                Return generateOxpiSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.OxPS
                Return generateOxpsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.EtherOxPC
                Return generateOxpcEtherSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.EtherOxPE
                Return generateOxpeEtherSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.PEtOH
                Return generatePetohSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PMeOH
                Return generatePmeohSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PBtOH
                Return generatePbtohSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.GM3
                Return generateGm3Species(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.LNAPS
                Return generateLnapsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherLPE
                Return generateEtherlpeSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherLPC
                Return generateEtherlpcSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Hex2Cer
                Return generateHexhexcernsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Hex3Cer
                Return generateHexhexhexcernsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Cer_EBDS
                Return generateAcylcerbdsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexCer
                Return generateAcylhexcerasSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.ASM
                Return generateAcylsmSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.Cer_OS
                Return generateCerosSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.MLCL
                Return generateLclSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DLCL
                Return generateDlclSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PhytoSph
                Return generatePhytosphingosineSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.Sph
                Return generateSphingosineSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.DHSph
                Return generateSphinganineSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherMGDG
                Return generateEthermgdgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherDGDG
                Return generateEtherdgdgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherTG
                Return generateEthertagSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
                'add 10/04/19
            Case LbmClass.EtherPI
                Return generateEtherpiSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherPS
                Return generateEtherpsSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PE_Cer
                Return generatePetceramideSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond).JoinIterates(
                generatePedceramideSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond))
                ' add 13/05/19
            Case LbmClass.DCAE
                Return generateDcaesSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.GDCAE
                Return generateGdcaesSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.GLCAE
                Return generateGlcaesSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.TDCAE
                Return generateTdcaesSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.TLCAE
                Return generateTlcaesSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.NAE
                Return generateAnandamideSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.NAGly
                Return generateFAHFAmideGlySpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.NAGlySer
                Return generateFAHFAmideGlySerSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SL
                Return generateSulfonolipidSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)
            Case LbmClass.EtherPG
                Return generateEtherpgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.EtherLPG
                Return generateEtherlpgSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.PI_Cer
                Return generatePiceramideDihydroxySpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen).JoinIterates(
                generatePiceramideTrihydroxySpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen)).JoinIterates(
                generatePiceramideOxDihydroxySpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen))
            Case LbmClass.SHexCer
                Return generateShexcerSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond).JoinIterates(
                generateShexcerOxSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond))

            Case LbmClass.CoQ
                Return generateCoenzymeqSpecies(adduct, minCarbonCount, maxCarbonCount)

            Case LbmClass.Vitamin_E
                Return generateVitaminESpecies(adduct).JoinIterates(generateVitaminDSpecies(adduct))

            Case LbmClass.VAE
                Return generateVitaminASpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.NAOrn
                Return generateFAHFAmideOrnSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.BRSE
                Return generateBrseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.CASE
                Return generateCaseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.SISE
                Return generateSiseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.STSE
                Return generateStseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)

            Case LbmClass.AHexBRS
                Return generateAhexbrseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexCAS
                Return generateAhexcaseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexCS
                Return generateAhexceSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexSIS
                Return generateAhexsiseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case LbmClass.AHexSTS
                Return generateAhexstseSpecies(adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
            Case Else
                Return {}
        End Select
    End Function

    Private Shared Function generatePsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Return commonGlycerolipidsGenerator("PS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Return commonGlycerolipidsGenerator("PE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Return commonGlycerolipidsGenerator("PC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        Return commonGlycerolipidsGenerator("PI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generatePgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        Return commonGlycerolipidsGenerator("PG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function

    Private Shared Function generateTagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C3H5O3")
        Return commonGlycerolipidsGenerator("TAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function


    Private Shared Function generateLnapeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Return commonGlycerolipidsGenerator("LNAPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    'add
    Private Shared Function generateDagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C3H6O3")
        Return commonGlycerolipidsGenerator("DAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateMagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C3H7O3")
        Return commonGlycerolipidsGenerator("MAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateFaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("HO")
        Return commonGlycerolipidsGenerator("FA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateFahfaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("HO")
        Return fahfaGenerator("FAHFA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateLpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C8H19NO6P")
        Return commonGlycerolipidsGenerator("LPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H13NO6P")
        Return commonGlycerolipidsGenerator("LPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H14O8P")
        Return commonGlycerolipidsGenerator("LPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H18O11P")
        Return commonGlycerolipidsGenerator("LPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H13NO8P")
        Return commonGlycerolipidsGenerator("LPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateLpaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C3H8O6P")
        Return commonGlycerolipidsGenerator("LPA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generatePaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C3H7O6P")
        Return commonGlycerolipidsGenerator("PA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateMgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H16O8")
        Return commonGlycerolipidsGenerator("MGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateSqdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H16O10S")
        Return commonGlycerolipidsGenerator("SQDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateDgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C15H26O13")
        Return commonGlycerolipidsGenerator("DGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateDgtsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C10H19NO5")
        Return commonGlycerolipidsGenerator("DGTS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateLdgtsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C10H20NO5")
        Return commonGlycerolipidsGenerator("LDGTS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateHbmpSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Return commonGlycerolipidsGenerator("HBMP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function
    Private Shared Function generateBmpSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        Return commonGlycerolipidsGenerator("BMP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateAcarSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C7H15O3N")
        Return commonGlycerolipidsGenerator("ACar", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateGlcadgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H14O9")
        Return commonGlycerolipidsGenerator("GlcADG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateAcylglcadgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H13O9")
        Return commonGlycerolipidsGenerator("AcylGlcADG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function
    Private Shared Function generateClSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H18O13P2")
        Return commonGlycerolipidsGenerator("CL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 4)
    End Function
    Private Shared Function generateCeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C27H45O")
        Return commonGlycerolipidsGenerator("CE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateEtherpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Return commonGlycerolipidsEtherGenerator("EtherPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generateEtherpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Return commonGlycerolipidsEtherGenerator("EtherPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    ' ceramide
    Private Shared Function generateSmSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H13NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("SM", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    'add MT
    Private Shared Function generateCerapSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("Cer_AP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCeradsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("Cer_ADS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerasSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("Cer_AS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCernpSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("Cer_NP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerndsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("Cer_NDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("Cer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerbdsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("Cer_BDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerbsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("Cer_BS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcerapSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("HexCer_AP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcerndsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("HexCer_NDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("HexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCereodsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideEsterGenerator("Cer_EODS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCereosSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideEsterGenerator("Cer_EOS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexcereosSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideEsterGenerator("HexCer_EOS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function

    '
    Private Shared Function generateOxfaSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("HO")
        Dim acylCount = 1
        Return commonGlycerolipidsOxGenerator("OxFA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Dim acylCount = 2
        Return commonGlycerolipidsOxGenerator("OxPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Dim acylCount = 2
        Return commonGlycerolipidsOxGenerator("OxPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        Dim acylCount = 2
        Return commonGlycerolipidsOxGenerator("OxPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        Dim acylCount = 2
        Return commonGlycerolipidsOxGenerator("OxPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Dim acylCount = 2
        Return commonGlycerolipidsOxGenerator("OxPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Function
    Private Shared Function generateOxpcEtherSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Dim acylCount = 2
        Dim etherCount = 1
        Return commonGlycerolipidsOxEtherGenerator("EtherOxPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount, etherCount)
    End Function
    Private Shared Function generateOxpeEtherSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Dim acylCount = 2
        Dim etherCount = 1
        Return commonGlycerolipidsOxEtherGenerator("EtherOxPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount, etherCount)
    End Function
    'others
    Private Shared Function generatePetohSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H11O6P")
        Return commonGlycerolipidsGenerator("PEtOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generatePmeohSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C4H9O6P")
        Return commonGlycerolipidsGenerator("PMeOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generatePbtohSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C7H15O6P")
        Return commonGlycerolipidsGenerator("PBtOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateGm3Species(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C23H38NO18")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("GM3", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateShexcerSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O8S")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("SHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateLnapsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Return commonGlycerolipidsGenerator("LNAPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generateEtherlpeSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H13NO6P")
        Return commonGlycerolipidsEtherGenerator("EtherLPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1, 1)
    End Function

    Private Shared Function generateEtherlpcSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C8H19NO6P")
        Return commonGlycerolipidsEtherGenerator("EtherLPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1, 1)
    End Function
    Private Shared Function generateHexhexcernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C12H21O10")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("HexHexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateHexhexhexcernsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C18H31O15")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("HexHexHexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateAcylcerbdsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideEsterGenerator("AcylCer_BDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateAcylhexcerasSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideEsterGenerator("AcylHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateAcylsmSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H13NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideEsterGenerator("AcylSM", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCerosSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideGenerator("Cer_OS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateLclSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H19O13P2")
        Return commonGlycerolipidsGenerator("LCL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Function
    Private Shared Function generateDlclSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H20O13P2")
        Return commonGlycerolipidsGenerator("DLCL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Function
    Private Shared Function generatePhytosphingosineSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 3
        Return commonSphingosineGenerator("Phytosphingosine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Function
    Private Shared Function generateSphingosineSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 2
        Return commonSphingosineGenerator("Sphingosine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Function
    Private Shared Function generateSphinganineSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 2
        Return commonSphingosineGenerator("Sphinganine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Function
    Private Shared Function generateEthermgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H16O8")
        Return commonGlycerolipidsEtherGenerator("EtherMGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    Private Shared Function generateEtherdgdgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C15H26O13")
        Return commonGlycerolipidsEtherGenerator("EtherDGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generateEthertagSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C3H5O3")
        Return commonGlycerolipidsEtherGenerator("EtherTAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3, 1)
    End Function

    ' add 10/04/19
    Private Shared Function generateEtherpiSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        Return commonGlycerolipidsEtherGenerator("EtherPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    Private Shared Function generateEtherpsSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Return commonGlycerolipidsEtherGenerator("EtherPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function
    Private Shared Function generatePetceramideSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C2H7NO3P")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("PE_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generatePedceramideSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C2H7NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("PE_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function

    ''' add 13/05/19
    Private Shared Function generateDcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C24H39O4")
        Return commonGlycerolipidsGenerator("DCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateGdcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C26H42NO5")
        Return commonGlycerolipidsGenerator("GDCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateGlcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C26H42NO4")
        Return commonGlycerolipidsGenerator("GLCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateTdcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C26H44NO6S")
        Return commonGlycerolipidsGenerator("TDCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateTlcaesSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C26H44NO5S")
        Return commonGlycerolipidsGenerator("TLCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateAnandamideSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C2H6NO")
        Return commonGlycerolipidsGenerator("NAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateFAHFAmideGlySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C2H4NO2")
        Return fahfaGenerator("NAAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateFAHFAmideGlySerSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H9N2O4")
        Return fahfaGenerator("NAAGS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateSulfonolipidSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, oxygenCount As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("SO3H")
        Return commonSulfonolipidGenerator("SL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, oxygenCount)
    End Function

    Private Shared Function generateEtherpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        Return commonGlycerolipidsEtherGenerator("EtherPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generateEtherlpgSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H15O7P")
        Return commonGlycerolipidsEtherGenerator("EtherLPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Function

    Private Shared Function generatePiceramideDihydroxySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generatePiceramideTrihydroxySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        Return commonCeramideGenerator("PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generatePiceramideOxDihydroxySpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideOxGenerator("PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateShexcerOxSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C6H11O8S")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        Return commonCeramideOxGenerator("SHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Function
    Private Shared Function generateCoenzymeqSpecies(adduct As AdductIon, minRepeatCount As Integer, maxRepeatCount As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C9H10O4")
        Dim additionalFormula = OrganicElementsReader("C5H8")
        Return commonCoenzymeqlipidsGenerator("CoQ", headerFormula, adduct, minRepeatCount, maxRepeatCount, additionalFormula)
    End Function

    Private Shared Function generateVitaminESpecies(adduct As AdductIon) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString.Substring(adductString.Length - 1, 1), "-") Then
            Dim formula = OrganicElementsReader("C29H50O2")
            Return commonSinglemoleculeGenerator("Vitamin", formula, adduct)
        Else
            Return {}
        End If
    End Function
    Private Shared Function generateVitaminDSpecies(adduct As AdductIon) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString, "[M+H]+") Then
            Dim formula = OrganicElementsReader("C27H44O2")
            Return commonSinglemoleculeGenerator("Vitamin", formula, adduct)
        Else
            Return {}
        End If
    End Function
    Private Shared Function generateVitaminASpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim adductString = adduct.AdductIonName

        If Equals(adductString.Substring(adductString.Length - 1, 1), "+") Then
            Dim headerFormula = OrganicElementsReader("C20H29O")
            Return commonGlycerolipidsGenerator("VAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
        Else
            Return {}
        End If
    End Function

    Private Shared Function generateFAHFAmideOrnSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C5H11N2O2")
        Return fahfaGenerator("NAAO", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Function

    Private Shared Function generateBrseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C28H45O")
        Return commonGlycerolipidsGenerator("BRSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateCaseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C28H47O")
        Return commonGlycerolipidsGenerator("CASE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateSiseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C29H49O")
        Return commonGlycerolipidsGenerator("SISE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateStseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C29H47O")
        Return commonGlycerolipidsGenerator("STSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateAhexbrseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C34H55O6")
        Return commonGlycerolipidsGenerator("AHexBRSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function

    Private Shared Function generateAhexcaseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C34H57O6")
        Return commonGlycerolipidsGenerator("AHexCASE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateAhexceSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C33H55O6")
        Return commonGlycerolipidsGenerator("AHexCE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateAhexsiseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C35H59O6")
        Return commonGlycerolipidsGenerator("AHexSISE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function
    Private Shared Function generateAhexstseSpecies(adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer) As IEnumerable(Of LipidIon)
        Dim headerFormula = OrganicElementsReader("C35H57O6")
        Return commonGlycerolipidsGenerator("AHexSTSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Function



    Private Shared Iterator Function commonGlycerolipidsOxGenerator(classString As String, headerFormula As DerivatizationFormula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer, acylCount As Integer) As IEnumerable(Of LipidIon)

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

