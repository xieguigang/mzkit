Imports CompMs.Common.DataObj.Property
Imports CompMs.Common.Enum
Imports CompMs.Common.Interfaces
Imports System.Collections.Generic
Imports System.Linq


Public Class FacadeLipidSpectrumGenerator
        Implements ILipidSpectrumGenerator
        Private ReadOnly map As Dictionary(Of LbmClass, List(Of ILipidSpectrumGenerator)) = New Dictionary(Of LbmClass, List(Of ILipidSpectrumGenerator))()

        Public Function CanGenerate(ByVal lipid As ILipid, ByVal adduct As AdductIon) As Boolean Implements ILipidSpectrumGenerator.CanGenerate
            Dim generators As List(Of ILipidSpectrumGenerator) = Nothing

            If map.TryGetValue(lipid.LipidClass, generators) Then
                Return generators.Any(Function(gen) gen.CanGenerate(lipid, adduct))
            End If
            Return False
        End Function

        Public Function Generate(ByVal lipid As Lipid, ByVal adduct As AdductIon, ByVal Optional molecule As IMoleculeProperty = Nothing) As IMSScanProperty Implements ILipidSpectrumGenerator.Generate
            Dim generators As List(Of ILipidSpectrumGenerator) = Nothing

            If map.TryGetValue(lipid.LipidClass, generators) Then
                Dim generator = generators.FirstOrDefault(Function(gen) gen.CanGenerate(lipid, adduct))
                Return generator?.Generate(lipid, adduct, molecule)
            End If
            Return Nothing
        End Function

        Public Sub Add(ByVal lipidClass As LbmClass, ByVal generator As ILipidSpectrumGenerator)
            If Not map.ContainsKey(lipidClass) Then
                map.Add(lipidClass, New List(Of ILipidSpectrumGenerator)())
            End If
            map(lipidClass).Add(generator)
        End Sub

        Public Sub Remove(ByVal lipidClass As LbmClass, ByVal generator As ILipidSpectrumGenerator)
            If map.ContainsKey(lipidClass) Then
                map(lipidClass).Remove(generator)
            End If
        End Sub

        Public Shared ReadOnly Property [Default] As ILipidSpectrumGenerator
            Get
                If defaultField Is Nothing Then
                    Dim generator = New FacadeLipidSpectrumGenerator()
                    generator.Add(LbmClass.EtherPC, New EtherPCSpectrumGenerator())
                    generator.Add(LbmClass.EtherPE, New EtherPESpectrumGenerator())
                    generator.Add(LbmClass.LPC, New LPCSpectrumGenerator())
                    generator.Add(LbmClass.LPE, New LPESpectrumGenerator())
                    generator.Add(LbmClass.LPG, New LPGSpectrumGenerator())
                    generator.Add(LbmClass.LPI, New LPISpectrumGenerator())
                    generator.Add(LbmClass.LPS, New LPSSpectrumGenerator())
                    generator.Add(LbmClass.PA, New PASpectrumGenerator())
                    generator.Add(LbmClass.PC, New PCSpectrumGenerator())
                    generator.Add(LbmClass.PE, New PESpectrumGenerator())
                    generator.Add(LbmClass.PG, New PGSpectrumGenerator())
                    generator.Add(LbmClass.PI, New PISpectrumGenerator())
                    generator.Add(LbmClass.PS, New PSSpectrumGenerator())
                    'generator.Add(LbmClass.MG, new MGSpectrumGenerator());
                    generator.Add(LbmClass.DG, New DGSpectrumGenerator())
                    generator.Add(LbmClass.TG, New TGSpectrumGenerator())
                    generator.Add(LbmClass.BMP, New BMPSpectrumGenerator())
                    generator.Add(LbmClass.HBMP, New HBMPSpectrumGenerator())
                    generator.Add(LbmClass.CL, New CLSpectrumGenerator())
                    generator.Add(LbmClass.SM, New SMSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NDS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NP, New CeramidePhytoSphSpectrumGenerator())
                    generator.Add(LbmClass.Cer_AS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_ADS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_AP, New CeramidePhytoSphSpectrumGenerator())
                    generator.Add(LbmClass.Cer_BS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_BDS, New CeramideSpectrumGenerator())
                    'generator.Add(LbmClass.Cer_HS, new CeramideSpectrumGenerator());
                    'generator.Add(LbmClass.Cer_HDS, new CeramideSpectrumGenerator());
                    generator.Add(LbmClass.HexCer_NS, New HexCerSpectrumGenerator())
                    generator.Add(LbmClass.HexCer_NDS, New HexCerSpectrumGenerator())
                    'generator.Add(LbmClass.Hex2Cer, new Hex2CerSpectrumGenerator());
                    generator.Add(LbmClass.DGTA, New DGTASpectrumGenerator())
                    generator.Add(LbmClass.DGTS, New DGTSSpectrumGenerator())
                    generator.Add(LbmClass.LDGTA, New LDGTASpectrumGenerator())
                    generator.Add(LbmClass.LDGTS, New LDGTSSpectrumGenerator())
                    generator.Add(LbmClass.GM3, New GM3SpectrumGenerator())
                    generator.Add(LbmClass.SHexCer, New SHexCerSpectrumGenerator())
                    generator.Add(LbmClass.CAR, New CARSpectrumGenerator())
                    generator.Add(LbmClass.DMEDFAHFA, New DMEDFAHFASpectrumGenerator())
                    generator.Add(LbmClass.DMEDFA, New DMEDFASpectrumGenerator())
                    generator.Add(LbmClass.DMEDOxFA, New DMEDFASpectrumGenerator())
                    'generator.Add(LbmClass.CE, new CESpectrumGenerator());
                    generator.Add(LbmClass.PC_d5, New PCd5SpectrumGenerator())
                    generator.Add(LbmClass.PE_d5, New PEd5SpectrumGenerator())
                    generator.Add(LbmClass.PG_d5, New PGd5SpectrumGenerator())
                    generator.Add(LbmClass.PI_d5, New PId5SpectrumGenerator())
                    generator.Add(LbmClass.PS_d5, New PSd5SpectrumGenerator())
                    generator.Add(LbmClass.LPC_d5, New LPCd5SpectrumGenerator())
                    generator.Add(LbmClass.LPE_d5, New LPEd5SpectrumGenerator())
                    generator.Add(LbmClass.LPG_d5, New LPGd5SpectrumGenerator())
                    generator.Add(LbmClass.LPI_d5, New LPId5SpectrumGenerator())
                    generator.Add(LbmClass.LPS_d5, New LPSd5SpectrumGenerator())
                    generator.Add(LbmClass.DG_d5, New DGd5SpectrumGenerator())
                    generator.Add(LbmClass.TG_d5, New TGd5SpectrumGenerator())
                    generator.Add(LbmClass.CE_d7, New CEd7SpectrumGenerator())
                    generator.Add(LbmClass.SM_d9, New SMd9SpectrumGenerator())
                    generator.Add(LbmClass.Cer_NS_d7, New CerNSd7SpectrumGenerator())

                    defaultField = generator
                End If
                Return defaultField
            End Get
        End Property
        Private Shared defaultField As ILipidSpectrumGenerator

        Public Shared ReadOnly Property OadLipidGenerator As ILipidSpectrumGenerator
            Get
                If oadlipidgeneratorField Is Nothing Then
                    Dim generator = New FacadeLipidSpectrumGenerator()
                    generator.Add(LbmClass.PC, New PCOadSpectrumGenerator())
                    generator.Add(LbmClass.LPC, New LPCOadSpectrumGenerator())
                    generator.Add(LbmClass.EtherPC, New EtherPCOadSpectrumGenerator())
                    generator.Add(LbmClass.EtherLPC, New OadDefaultSpectrumGenerator())
                    generator.Add(LbmClass.PE, New PEOadSpectrumGenerator())
                    generator.Add(LbmClass.LPE, New LPEOadSpectrumGenerator())
                    generator.Add(LbmClass.EtherPE, New OadDefaultSpectrumGenerator())
                    generator.Add(LbmClass.EtherLPE, New OadDefaultSpectrumGenerator())
                    generator.Add(LbmClass.PG, New PGOadSpectrumGenerator())
                    generator.Add(LbmClass.PI, New PIOadSpectrumGenerator())
                    generator.Add(LbmClass.PS, New OadDefaultSpectrumGenerator())
                    generator.Add(LbmClass.TG, New TGOadSpectrumGenerator())
                    generator.Add(LbmClass.DG, New DGOadSpectrumGenerator())
                    generator.Add(LbmClass.SM, New SMOadSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NDS, New CeramideOadSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NS, New CeramideOadSpectrumGenerator())
                    oadlipidgeneratorField = generator
                End If
                Return oadlipidgeneratorField
            End Get
        End Property
        Private Shared oadlipidgeneratorField As ILipidSpectrumGenerator
        Public Shared ReadOnly Property EidLipidGenerator As ILipidSpectrumGenerator
            Get
                If eidlipidgeneratorField Is Nothing Then
                    Dim generator = New FacadeLipidSpectrumGenerator()
                    generator.Add(LbmClass.PC, New PCEidSpectrumGenerator())
                    generator.Add(LbmClass.LPC, New LPCEidSpectrumGenerator())
                    generator.Add(LbmClass.EtherPC, New EtherPCEidSpectrumGenerator())
                    generator.Add(LbmClass.PE, New PEEidSpectrumGenerator())
                    generator.Add(LbmClass.LPE, New LPEEidSpectrumGenerator())
                    generator.Add(LbmClass.EtherPE, New EtherPEEidSpectrumGenerator())
                    generator.Add(LbmClass.PG, New PGEidSpectrumGenerator())
                    generator.Add(LbmClass.PI, New PIEidSpectrumGenerator())
                    generator.Add(LbmClass.PS, New PSEidSpectrumGenerator())
                    generator.Add(LbmClass.PA, New PAEidSpectrumGenerator())
                    generator.Add(LbmClass.LPG, New LPGEidSpectrumGenerator())
                    generator.Add(LbmClass.LPI, New LPIEidSpectrumGenerator())
                    generator.Add(LbmClass.LPS, New LPSEidSpectrumGenerator())
                    generator.Add(LbmClass.CL, New CLEidSpectrumGenerator())
                    generator.Add(LbmClass.MG, New MGEidSpectrumGenerator())
                    generator.Add(LbmClass.DG, New DGEidSpectrumGenerator())
                    generator.Add(LbmClass.TG, New TGEidSpectrumGenerator())
                    generator.Add(LbmClass.BMP, New BMPEidSpectrumGenerator())
                    generator.Add(LbmClass.HBMP, New HBMPEidSpectrumGenerator())
                    ' below here are EID not implemented
                    generator.Add(LbmClass.SM, New SMSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NDS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_NP, New CeramidePhytoSphSpectrumGenerator())
                    generator.Add(LbmClass.Cer_AS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_ADS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_AP, New CeramidePhytoSphSpectrumGenerator())
                    generator.Add(LbmClass.Cer_BS, New CeramideSpectrumGenerator())
                    generator.Add(LbmClass.Cer_BDS, New CeramideSpectrumGenerator())
                    'generator.Add(LbmClass.Cer_HS, new CeramideSpectrumGenerator());
                    'generator.Add(LbmClass.Cer_HDS, new CeramideSpectrumGenerator());
                    generator.Add(LbmClass.HexCer_NS, New HexCerSpectrumGenerator())
                    generator.Add(LbmClass.DGTA, New DGTASpectrumGenerator())
                    generator.Add(LbmClass.DGTS, New DGTSSpectrumGenerator())
                    generator.Add(LbmClass.LDGTA, New LDGTASpectrumGenerator())
                    generator.Add(LbmClass.LDGTS, New LDGTSSpectrumGenerator())
                    generator.Add(LbmClass.GM3, New GM3SpectrumGenerator())
                    generator.Add(LbmClass.SHexCer, New SHexCerSpectrumGenerator())
                    generator.Add(LbmClass.CAR, New CARSpectrumGenerator())
                    eidlipidgeneratorField = generator
                End If
                Return eidlipidgeneratorField
            End Get
        End Property
        Private Shared eidlipidgeneratorField As ILipidSpectrumGenerator

    End Class

