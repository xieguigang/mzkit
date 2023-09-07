Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.FormulaGenerator.Parser
Imports System.IO
Imports System.Text


Public NotInheritable Class LipidMassLibraryGenerator
    Private Sub New()
    End Sub

    Public Shared Sub Integrate(inputfolder As String, output As String)
        Dim files = Directory.GetFiles(inputfolder, "*.txt", SearchOption.TopDirectoryOnly)
        Using sw = New StreamWriter(output, False, Encoding.ASCII)
            sw.WriteLine("Name" & Microsoft.VisualBasic.Constants.vbTab & "MZ" & Microsoft.VisualBasic.Constants.vbTab & "Adduct")
            For Each file In files
                Using sr = New StreamReader(file, Encoding.ASCII)
                    sr.ReadLine()
                    While sr.Peek() > -1
                        sw.WriteLine(sr.ReadLine())
                    End While
                End Using
            Next
        End Using
    End Sub

    Public Shared Sub Run(outputfolder As String, lipidclass As LbmClass, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)

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
    End Sub


    Private Shared Sub generatePsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        commonGlycerolipidsGenerator(filepath, "PS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub

    Private Shared Sub generatePeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        commonGlycerolipidsGenerator(filepath, "PE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub

    Private Shared Sub generatePcSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        commonGlycerolipidsGenerator(filepath, "PC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub

    Private Shared Sub generatePiSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        commonGlycerolipidsGenerator(filepath, "PI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub

    Private Shared Sub generatePgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        commonGlycerolipidsGenerator(filepath, "PG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub

    Private Shared Sub generateTagSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "TAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H5O3")
        commonGlycerolipidsGenerator(filepath, "TAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Sub


    Private Shared Sub generateLnapeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LNAPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        commonGlycerolipidsGenerator(filepath, "LNAPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    'add
    Private Shared Sub generateDagSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H6O3")
        commonGlycerolipidsGenerator(filepath, "DAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateMagSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "MAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H7O3")
        commonGlycerolipidsGenerator(filepath, "MAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateFaSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("HO")
        commonGlycerolipidsGenerator(filepath, "FA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateFahfaSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("HO")
        fahfaGenerator(filepath, "FAHFA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Sub

    Private Shared Sub generateLpcSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H19NO6P")
        commonGlycerolipidsGenerator(filepath, "LPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateLpeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO6P")
        commonGlycerolipidsGenerator(filepath, "LPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateLpgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H14O8P")
        commonGlycerolipidsGenerator(filepath, "LPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateLpiSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H18O11P")
        commonGlycerolipidsGenerator(filepath, "LPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateLpsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13NO8P")
        commonGlycerolipidsGenerator(filepath, "LPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateLpaSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LPA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H8O6P")
        commonGlycerolipidsGenerator(filepath, "LPA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generatePaSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H7O6P")
        commonGlycerolipidsGenerator(filepath, "PA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateMgdgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "MGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H16O8")
        commonGlycerolipidsGenerator(filepath, "MGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateSqdgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SQDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H16O10S")
        commonGlycerolipidsGenerator(filepath, "SQDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateDgdgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C15H26O13")
        commonGlycerolipidsGenerator(filepath, "DGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateDgtsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DGTS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C10H19NO5")
        commonGlycerolipidsGenerator(filepath, "DGTS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateLdgtsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LDGTS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C10H20NO5")
        commonGlycerolipidsGenerator(filepath, "LDGTS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateHbmpSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HBMP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        commonGlycerolipidsGenerator(filepath, "HBMP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Sub
    Private Shared Sub generateBmpSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "BMP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        commonGlycerolipidsGenerator(filepath, "BMP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateAcarSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "ACar" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C7H15O3N")
        commonGlycerolipidsGenerator(filepath, "ACar", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateGlcadgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GlcADG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H14O9")
        commonGlycerolipidsGenerator(filepath, "GlcADG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateAcylglcadgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylGlcADG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H13O9")
        commonGlycerolipidsGenerator(filepath, "AcylGlcADG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Sub
    Private Shared Sub generateClSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CL" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H18O13P2")
        commonGlycerolipidsGenerator(filepath, "CL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 4)
    End Sub
    Private Shared Sub generateCeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C27H45O")
        commonGlycerolipidsGenerator(filepath, "CE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub

    Private Shared Sub generateEtherpeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub

    Private Shared Sub generateEtherpcSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub
    ' ceramide
    Private Shared Sub generateSmSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SM" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "SM", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    'add MT
    Private Shared Sub generateCerapSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_AP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "Cer_AP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCeradsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_ADS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "Cer_ADS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCerasSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_AS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "Cer_AS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCernpSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_NP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "Cer_NP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCerndsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_NDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "Cer_NDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCernsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "Cer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCerbdsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_BDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "Cer_BDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCerbsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_BS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "Cer_BS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateHexcerapSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_AP" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "HexCer_AP", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateHexcerndsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_NDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "HexCer_NDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateHexcernsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "HexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCereodsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_EODS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator(filepath, "Cer_EODS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCereosSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_EOS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator(filepath, "Cer_EOS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateHexcereosSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexCer_EOS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator(filepath, "HexCer_EOS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub

    '
    Private Shared Sub generateOxfaSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxFA" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("HO")
        Dim acylCount = 1
        commonGlycerolipidsOxGenerator(filepath, "OxFA", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Sub
    Private Shared Sub generateOxpcSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator(filepath, "OxPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Sub
    Private Shared Sub generateOxpeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator(filepath, "OxPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Sub
    Private Shared Sub generateOxpgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator(filepath, "OxPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Sub
    Private Shared Sub generateOxpiSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator(filepath, "OxPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Sub
    Private Shared Sub generateOxpsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "OxPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        Dim acylCount = 2
        commonGlycerolipidsOxGenerator(filepath, "OxPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount)
    End Sub
    Private Shared Sub generateOxpcEtherSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherOxPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H18NO6P")
        Dim acylCount = 2
        Dim etherCount = 1
        commonGlycerolipidsOxEtherGenerator(filepath, "EtherOxPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount, etherCount)
    End Sub
    Private Shared Sub generateOxpeEtherSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherOxPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H12NO6P")
        Dim acylCount = 2
        Dim etherCount = 1
        commonGlycerolipidsOxEtherGenerator(filepath, "EtherOxPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen, acylCount, etherCount)
    End Sub
    'others
    Private Shared Sub generatePetohSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PEtOH" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H11O6P")
        commonGlycerolipidsGenerator(filepath, "PEtOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generatePmeohSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PMeOH" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C4H9O6P")
        commonGlycerolipidsGenerator(filepath, "PMeOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generatePbtohSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PBtOH" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C7H15O6P")
        commonGlycerolipidsGenerator(filepath, "PBtOH", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateGm3Species(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GM3" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C23H38NO18")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "GM3", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateShexcerSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SHexCer" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O8S")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "SHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateLnapsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LNAPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        commonGlycerolipidsGenerator(filepath, "LNAPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generateEtherlpeSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherLPE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO6P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherLPE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1, 1)
    End Sub

    Private Shared Sub generateEtherlpcSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherLPC" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C8H19NO6P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherLPC", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1, 1)
    End Sub
    Private Shared Sub generateHexhexcernsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexHexCer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C12H21O10")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "HexHexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateHexhexhexcernsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "HexHexHexCer_NS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C18H31O15")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "HexHexHexCer_NS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateAcylcerbdsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylCer_BDS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator(filepath, "AcylCer_BDS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateAcylhexcerasSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylHexCer" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O5")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideEsterGenerator(filepath, "AcylHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateAcylsmSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AcylSM" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H13NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideEsterGenerator(filepath, "AcylSM", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCerosSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Cer_OS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideGenerator(filepath, "Cer_OS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateLclSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "LCL" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H19O13P2")
        commonGlycerolipidsGenerator(filepath, "LCL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3)
    End Sub
    Private Shared Sub generateDlclSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DLCL" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H20O13P2")
        commonGlycerolipidsGenerator(filepath, "DLCL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2)
    End Sub
    Private Shared Sub generatePhytosphingosineSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Phytosphingosine" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 3
        commonSphingosineGenerator(filepath, "Phytosphingosine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Sub
    Private Shared Sub generateSphingosineSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Sphingosine" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 2
        commonSphingosineGenerator(filepath, "Sphingosine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Sub
    Private Shared Sub generateSphinganineSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Sphinganine" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("H")
        Dim hydroxyCount = 2
        commonSphingosineGenerator(filepath, "Sphinganine", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, hydroxyCount)
    End Sub
    Private Shared Sub generateEthermgdgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherMGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H16O8")
        commonGlycerolipidsEtherGenerator(filepath, "EtherMGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub
    Private Shared Sub generateEtherdgdgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherDGDG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C15H26O13")
        commonGlycerolipidsEtherGenerator(filepath, "EtherDGDG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub

    Private Shared Sub generateEthertagSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherTAG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C3H5O3")
        commonGlycerolipidsEtherGenerator(filepath, "EtherTAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 3, 1)
    End Sub

    ' add 10/04/19
    Private Shared Sub generateEtherpiSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPI" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H17O11P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherPI", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub
    Private Shared Sub generateEtherpsSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPS" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12NO8P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherPS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub
    Private Shared Sub generatePetceramideSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PE-Cer(t)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H7NO3P")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "PE_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generatePedceramideSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PE-Cer(d)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H7NO3P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "PE_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub

    ''' add 13/05/19
    Private Shared Sub generateDcaesSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "DCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C24H39O4")
        commonGlycerolipidsGenerator(filepath, "DCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateGdcaesSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GDCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H42NO5")
        commonGlycerolipidsGenerator(filepath, "GDCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateGlcaesSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "GLCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H42NO4")
        commonGlycerolipidsGenerator(filepath, "GLCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateTdcaesSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "TDCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H44NO6S")
        commonGlycerolipidsGenerator(filepath, "TDCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateTlcaesSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "TLCAE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C26H44NO5S")
        commonGlycerolipidsGenerator(filepath, "TLCAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub

    Private Shared Sub generateAnandamideSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Anandamide" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H6NO")
        commonGlycerolipidsGenerator(filepath, "NAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub

    Private Shared Sub generateFAHFAmideGlySpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFAmide(Gly)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C2H4NO2")
        fahfaGenerator(filepath, "NAAG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Sub

    Private Shared Sub generateFAHFAmideGlySerSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFAmide(GlySer)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H9N2O4")
        fahfaGenerator(filepath, "NAAGS", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Sub

    Private Shared Sub generateSulfonolipidSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, oxygenCount As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "Sulfonolipid" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("SO3H")
        commonSulfonolipidGenerator(filepath, "SL", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, oxygenCount)
    End Sub

    Private Shared Sub generateEtherpgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H13O8P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub

    Private Shared Sub generateEtherlpgSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "EtherLPG" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H15O7P")
        commonGlycerolipidsEtherGenerator(filepath, "EtherLPG", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 2, 1)
    End Sub

    Private Shared Sub generatePiceramideDihydroxySpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI-Cer(d)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generatePiceramideTrihydroxySpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI-Cer(t)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 3
        Dim acylHydroxyCount = 0
        commonCeramideGenerator(filepath, "PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generatePiceramideOxDihydroxySpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxydized As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "PI-Cer(d_O)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H12O8P")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideOxGenerator(filepath, "PI_Cer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateShexcerOxSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SHexCerO" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C6H11O8S")
        Dim sphingoHydroxyCount = 2
        Dim acylHydroxyCount = 1
        commonCeramideOxGenerator(filepath, "SHexCer", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, sphingoHydroxyCount, acylHydroxyCount)
    End Sub
    Private Shared Sub generateCoenzymeqSpecies(outputfolder As String, adduct As AdductIon, minRepeatCount As Integer, maxRepeatCount As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CoQ" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C9H10O4")
        Dim additionalFormula = OrganicElementsReader("C5H8")
        commonCoenzymeqlipidsGenerator(filepath, "CoQ", headerFormula, adduct, minRepeatCount, maxRepeatCount, additionalFormula)
    End Sub

    Private Shared Sub generateVitaminESpecies(outputfolder As String, adduct As AdductIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString.Substring(adductString.Length - 1, 1), "-") Then
            Dim filepath = outputfolder & "\" & "VitaminE" & "_" & adductString & ".txt"
            Dim formula = OrganicElementsReader("C29H50O2")
            commonSinglemoleculeGenerator(filepath, "Vitamin", formula, adduct)
        End If
    End Sub
    Private Shared Sub generateVitaminDSpecies(outputfolder As String, adduct As AdductIon)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString, "[M+H]+") Then
            Dim filepath = outputfolder & "\" & "VitaminD" & "_" & adductString & ".txt"
            Dim formula = OrganicElementsReader("C27H44O2")
            commonSinglemoleculeGenerator(filepath, "Vitamin", formula, adduct)
        End If
    End Sub
    Private Shared Sub generateVitaminASpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        If Equals(adductString.Substring(adductString.Length - 1, 1), "+") Then
            Dim filepath = outputfolder & "\" & "VitaminA" & "_" & adductString & ".txt"
            Dim headerFormula = OrganicElementsReader("C20H29O")
            commonGlycerolipidsGenerator(filepath, "VAE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
        End If
    End Sub

    Private Shared Sub generateFAHFAmideOrnSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "FAHFAmide(Orn)" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C5H11N2O2")
        fahfaGenerator(filepath, "NAAO", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond)
    End Sub

    Private Shared Sub generateBrseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "BRSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C28H45O")
        commonGlycerolipidsGenerator(filepath, "BRSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateCaseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "CASE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C28H47O")
        commonGlycerolipidsGenerator(filepath, "CASE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateSiseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "SISE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C29H49O")
        commonGlycerolipidsGenerator(filepath, "SISE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub

    Private Shared Sub generateStseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "STSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C29H47O")
        commonGlycerolipidsGenerator(filepath, "STSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub

    Private Shared Sub generateAhexbrseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexBRSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C34H55O6")
        commonGlycerolipidsGenerator(filepath, "AHexBRSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub

    Private Shared Sub generateAhexcaseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexCASE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C34H57O6")
        commonGlycerolipidsGenerator(filepath, "AHexCASE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateAhexceSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexCE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C33H55O6")
        commonGlycerolipidsGenerator(filepath, "AHexCE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateAhexsiseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexSISE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C35H59O6")
        commonGlycerolipidsGenerator(filepath, "AHexSISE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub
    Private Shared Sub generateAhexstseSpecies(outputfolder As String, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Dim adductString = adduct.AdductIonName
        Dim filepath = outputfolder & "\" & "AHexSTSE" & "_" & adductString & ".txt"
        Dim headerFormula = OrganicElementsReader("C35H57O6")
        commonGlycerolipidsGenerator(filepath, "AHexSTSE", headerFormula, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, 1)
    End Sub



    Private Shared Sub commonGlycerolipidsOxGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer, acylCount As Integer)

        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
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

                            Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                            Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                            Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "+" & k.ToString() & "O"

                            sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                        End If
                    Next


                Next
            Next
        End Using
    End Sub
    Private Shared Sub commonGlycerolipidsOxEtherGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, maxOxygen As Integer, acylCount As Integer, etherCount As Integer)

        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
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

                            Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                            Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                            Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "e+" & k.ToString() & "O"

                            sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                        End If
                    Next


                Next
            Next
        End Using
    End Sub


    Private Shared Sub fahfaGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer)
        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minCarbonCount To maxCarbonCount
                For j = minDoubleBond To maxDoubleBond
                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - 2 * totalChainDoubleBond - 3 ' scafold FA
                        Dim totalChainOxygen = 3

                        Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                        Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()


                        sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                    End If
                Next
            Next
        End Using
    End Sub


    Private Shared Sub commonCeramideGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, sphingoHydroxyCount As Integer, acylHydroxyCount As Integer)

        Dim hydroHeader = "d"
        If sphingoHydroxyCount = 3 Then hydroHeader = "t"
        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minCarbonCount To maxCarbonCount
                For j = minDoubleBond To maxDoubleBond
                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2
                        Dim totalChainOxygen = 1 + sphingoHydroxyCount + acylHydroxyCount
                        Dim totalNitrogenCount = 1

                        Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum + totalNitrogenCount, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                        Dim lipidname = classString & " " & hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                        sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                    End If
                Next
            Next
        End Using
    End Sub

    Private Shared Sub commonGlycerolipidsGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, acylCount As Integer)

        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minCarbonCount To maxCarbonCount
                For j = minDoubleBond To maxDoubleBond
                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - acylCount - totalChainDoubleBond * 2
                        Dim totalChainOxygen = acylCount

                        Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                        Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                        sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                    End If
                Next
            Next
        End Using
    End Sub
    'add MT
    Private Shared Sub commonGlycerolipidsEtherGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, acylCount As Integer, etherCount As Integer)

        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minCarbonCount To maxCarbonCount
                For j = minDoubleBond To maxDoubleBond
                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - acylCount - totalChainDoubleBond * 2 + etherCount * 2
                        Dim totalChainOxygen = acylCount - 1

                        Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                        Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString() & "e"

                        sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                    End If
                Next
            Next
        End Using
    End Sub
    Private Shared Sub commonCeramideEsterGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, sphingoHydroxyCount As Integer, acylHydroxyCount As Integer)
        Dim hydroHeader = "d"
        If sphingoHydroxyCount = 3 Then hydroHeader = "t"
        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minCarbonCount To maxCarbonCount
                For j = minDoubleBond To maxDoubleBond
                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2 - 2
                        Dim totalChainOxygen = 2 + sphingoHydroxyCount + acylHydroxyCount
                        Dim totalNitrogenCount = 1

                        Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum + totalNitrogenCount, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                        Dim lipidname = classString & " " & hydroHeader & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                        sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                    End If
                Next
            Next
        End Using
    End Sub
    Private Shared Sub commonSphingosineGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, HydroxyCount As Integer)

        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minCarbonCount To maxCarbonCount
                For j = minDoubleBond To maxDoubleBond
                    If isPracticalDoubleBondSize(i, j) Then

                        Dim totalChainCarbon = i
                        Dim totalChainDoubleBond = j
                        Dim totalChainHydrogen = totalChainCarbon * 2 - totalChainDoubleBond * 2 + 2
                        Dim totalChainOxygen = HydroxyCount
                        Dim totalNitrogenCount = 1

                        Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum + totalNitrogenCount, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                        Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                        Dim lipidname = classString & " " & totalChainCarbon.ToString() & ":" & totalChainDoubleBond.ToString()

                        sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                    End If
                Next
            Next
        End Using
    End Sub

    Private Shared Sub commonSulfonolipidGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, acylHydroxyCount As Integer)

        Dim hydroHeader = "m"
        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
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

                            Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum + totalNitrogenCount, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                            Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                            Dim lipidname = classString & " " & totalString

                            sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                        End If
                    Next
                Next
            Next
        End Using
    End Sub

    Private Shared Sub commonCeramideOxGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minCarbonCount As Integer, maxCarbonCount As Integer, minDoubleBond As Integer, maxDoubleBond As Integer, sphingoHydroxyCount As Integer, acylHydroxyCount As Integer)

        Dim hydroHeader = "d"
        If sphingoHydroxyCount = 3 Then hydroHeader = "t"
        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
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

                            Dim totalFormula = New Formula(headerFormula.Cnum + totalChainCarbon, headerFormula.Hnum + totalChainHydrogen, headerFormula.Nnum + totalNitrogenCount, headerFormula.Onum + totalChainOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                            Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                            Dim lipidname = classString & " " & totalString

                            sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)
                        End If
                    Next
                Next
            Next
        End Using
    End Sub

    Private Shared Sub commonCoenzymeqlipidsGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon, minRepeatCount As Integer, maxRepeatCount As Integer, additionalFormula As Formula)

        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            For i = minRepeatCount To maxRepeatCount
                Dim totalCarbon = headerFormula.Cnum + additionalFormula.Cnum * i
                Dim totalHydrogen = headerFormula.Hnum + additionalFormula.Hnum * i
                Dim totalOxygen = headerFormula.Onum + additionalFormula.Onum * i
                Dim totalNitrogen = headerFormula.Nnum + additionalFormula.Nnum * i

                Dim totalFormula = New Formula(totalCarbon, totalHydrogen, totalNitrogen, totalOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
                Dim mz = adduct.ConvertToMz(totalFormula.Mass)
                Dim lipidname = classString

                sw.WriteLine(lipidname & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)

            Next
        End Using
    End Sub

    Private Shared Sub commonSinglemoleculeGenerator(filepath As String, classString As String, headerFormula As Formula, adduct As AdductIon)
        Using sw = New StreamWriter(filepath, False, Encoding.ASCII)
            writeHeader(sw)
            Dim totalCarbon = headerFormula.Cnum
            Dim totalHydrogen = headerFormula.Hnum
            Dim totalOxygen = headerFormula.Onum
            Dim totalNitrogen = headerFormula.Nnum

            Dim totalFormula = New Formula(totalCarbon, totalHydrogen, totalNitrogen, totalOxygen, headerFormula.Pnum, headerFormula.Snum, 0, 0, 0, 0, 0)
            Dim mz = adduct.ConvertToMz(totalFormula.Mass)
            Dim lipidname = classString

            sw.WriteLine(lipidname & " " & Microsoft.VisualBasic.Constants.vbTab & mz.ToString() & Microsoft.VisualBasic.Constants.vbTab & adduct.AdductIonName)

        End Using
    End Sub


    Private Shared Sub writeHeader(sw As StreamWriter)
        sw.WriteLine("Name" & Microsoft.VisualBasic.Constants.vbTab & "Mz" & Microsoft.VisualBasic.Constants.vbTab & "Adduct")
    End Sub

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

