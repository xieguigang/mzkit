#Region "Microsoft.VisualBasic::8f6bcfcef3dafde5786686737a30efb6, G:/mzkit/src/metadb/Lipidomics//LbmClass.vb"

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

    '   Total Lines: 217
    '    Code Lines: 206
    ' Comment Lines: 3
    '   Blank Lines: 8
    '     File Size: 2.46 KB


    ' Enum LbmClass
    ' 
    '     [CASE], Ac2PIM1, Ac2PIM2, Ac3PIM2, Ac4PIM2
    '     ADGGA, AHexBRS, AHexCAS, AHexCer, AHexCS
    '     AHexSIS, AHexSTS, ASHexCer, ASM, BAHex
    '     BASulfate, BileAcid, BisMeLPA, BMP, bmPC
    '     BRSE, BRSLPHex, BRSPHex, CAR, CASLPHex
    '     CASPHex, CE, CE_d7, Cer_ADS, Cer_AP
    '     Cer_AS, Cer_BDS, Cer_BS, Cer_EBDS, Cer_EODS
    '     Cer_EOS, Cer_HDS, Cer_HS, Cer_NDOS, Cer_NDS
    '     Cer_NP, Cer_NS, Cer_NS_d7, Cer_OS, CerP
    '     CL, CoQ, CSLPHex, CSPHex, DCAE
    '     DEGSE, DG, DG_d5, DGCC, DGDG
    '     DGGA, DGMG, DGTA, DGTS, DHSph
    '     DLCL, DMEDFA, DMEDFAHFA, DMEDOxFA, DMPE
    '     DSMSE, EGSE, EtherDG, EtherDGDG, EtherLPC
    '     EtherLPE, EtherLPG, EtherLPI, EtherLPS, EtherMGDG
    '     EtherOxPC, EtherOxPE, EtherPC, EtherPE, EtherPG
    '     EtherPI, EtherPS, EtherSMGDG, EtherTG, FA
    '     FAHFA, GD1a, GD1b, GD2, GD3
    '     GDCAE, GLCAE, GM1, GM3, GPNAE
    '     GQ1b, GT1b, HBMP, Hex2Cer, Hex3Cer
    '     HexCer_AP, HexCer_EOS, HexCer_HDS, HexCer_HS, HexCer_NDS
    '     HexCer_NS, KDCAE, KLCAE, LCAE, LDGCC
    '     LDGTA, LDGTS, LipidA, LNAPE, LNAPS
    '     LPA, LPC, LPC_d5, LPE, LPE_d5
    '     LPG, LPG_d5, LPI, LPI_d5, LPS
    '     LPS_d5, MG, MGDG, MGMG, MIPC
    '     MLCL, MMPE, NA5HT, NAAla, NAE
    '     NAGln, NAGly, NAGlySer, NALeu, NAOrn
    '     NAPhe, NASer, NATau, NATryA, NAVal
    '     NGcGM3, Others, OxFA, OxPA, OxPC
    '     OxPE, OxPG, OxPI, OxPS, OxTG
    '     PA, PBtOH, PC, PC_d5, PE
    '     PE_Cer, PE_d5, PEtOH, PG, PG_d5
    '     PhytoSph, PI, PI_Cer, PI_d5, PMeOH
    '     PS, PS_d5, PT, SHex, SHexCer
    '     SISE, SISLPHex, SISPHex, SL, SM
    '     SM_d9, SMGDG, SPE, SPEHex, SPGHex
    '     Sph, SPLASH, SQDG, SSulfate, ST
    '     STSE, STSLPHex, STSPHex, TDCAE, TG
    '     TG_d5, TG_EST, TLCAE, Undefined, Unknown
    '     VAE, Vitamin_D, Vitamin_E, WE
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' enumerates the lipid structure class
''' </summary>
Public Enum LbmClass
    Undefined
    Others
    Unknown
    SPLASH

    FA
    FAHFA
    OxFA
    CAR
    NAE
    NAGly
    NAGlySer
    NAOrn

    PA
    PC
    PE
    PG
    PI
    PS
    BMP
    HBMP
    EtherPC
    EtherPE
    EtherPI
    EtherPS
    EtherPG
    OxPA
    OxPC
    OxPS
    OxPE
    OxPG
    OxPI
    EtherOxPC
    EtherOxPE
    LPC
    LPE
    LPA
    LPS
    LPI
    LPG
    PMeOH
    PEtOH
    PBtOH
    EtherLPC
    EtherLPE
    EtherLPG
    EtherLPS
    EtherLPI
    LNAPE
    LNAPS
    MLCL
    DLCL
    CL

    MG
    DG
    TG
    MGDG
    DGDG
    SQDG
    DGTS
    DGGA
    ADGGA
    LDGTS
    LDGCC
    DGCC
    EtherMGDG
    EtherDGDG
    EtherTG
    EtherDG
    EtherSMGDG
    SMGDG

    Sph
    DHSph
    PhytoSph
    SM
    CerP
    Cer_ADS
    Cer_AS
    Cer_BDS
    Cer_BS
    Cer_OS
    Cer_EODS
    Cer_EOS
    Cer_NDS
    Cer_NS
    Cer_NP
    Cer_AP
    Cer_EBDS
    Cer_HS
    Cer_HDS
    Cer_NDOS
    HexCer_NS
    HexCer_NDS
    HexCer_AP
    HexCer_HS
    HexCer_HDS
    HexCer_EOS
    Hex2Cer
    Hex3Cer
    SHexCer
    GM3
    AHexCer
    ASM
    PE_Cer
    PI_Cer
    SL

    Ac2PIM1
    Ac2PIM2
    Ac3PIM2
    Ac4PIM2
    LipidA

    Vitamin_E
    Vitamin_D
    CoQ

    CE
    DCAE
    GDCAE
    GLCAE
    TDCAE
    TLCAE
    BileAcid
    VAE
    BRSE
    [CASE]
    SISE
    STSE
    AHexBRS
    AHexCAS
    AHexCS
    AHexSIS
    AHexSTS
    SPE
    SHex
    SPEHex
    SPGHex
    CSLPHex
    BRSLPHex
    CASLPHex
    SISLPHex
    STSLPHex
    CSPHex
    BRSPHex
    CASPHex
    SISPHex
    STSPHex
    SSulfate
    BAHex
    BASulfate
    LCAE
    KLCAE
    KDCAE
    MMPE
    DMPE
    MIPC
    EGSE
    DEGSE
    DSMSE
    OxTG
    TG_EST
    GPNAE
    DGMG
    MGMG
    GD1a
    GD1b
    GD2
    GD3
    GM1
    GQ1b
    GT1b
    NGcGM3
    ST
    DGTA
    LDGTA
    NATau
    NAPhe
    PT
    DMEDFAHFA
    DMEDFA
    DMEDOxFA
    CE_d7
    Cer_NS_d7
    PC_d5
    PE_d5
    PG_d5
    PS_d5
    PI_d5
    SM_d9
    TG_d5
    DG_d5
    LPC_d5
    LPE_d5
    LPG_d5
    LPS_d5
    LPI_d5
    NATryA
    NA5HT
    WE
    BisMeLPA
    NALeu
    NASer
    NAAla
    NAGln
    NAVal
    bmPC
    ASHexCer
End Enum
