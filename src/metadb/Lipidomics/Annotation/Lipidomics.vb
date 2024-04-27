#Region "Microsoft.VisualBasic::202107cb49d4dc98d5c8b4eae0ffd73b, G:/mzkit/src/metadb/Lipidomics//Annotation/Lipidomics.vb"

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

    '   Total Lines: 693
    '    Code Lines: 560
    ' Comment Lines: 56
    '   Blank Lines: 77
    '     File Size: 52.96 KB


    ' Class LipidMolecule
    ' 
    '     Properties: Adduct, AnnotationLevel, Formula, InChIKey, IonMode
    '                 IsValidatedFormat, LipidCategory, LipidClass, LipidName, LipidSubclass
    '                 Mz, Rt, Score, Smiles, Sn1AcylChainString
    '                 Sn1CarbonCount, Sn1DoubleBondCount, Sn1Oxidizedount, Sn2AcylChainString, Sn2CarbonCount
    '                 Sn2DoubleBondCount, Sn2Oxidizedount, Sn3AcylChainString, Sn3CarbonCount, Sn3DoubleBondCount
    '                 Sn3Oxidizedount, Sn4AcylChainString, Sn4CarbonCount, Sn4DoubleBondCount, Sn4Oxidizedount
    '                 SublevelLipidName, TotalCarbonCount, TotalChainString, TotalDoubleBondCount, TotalOxidizedCount
    ' 
    ' Class LipidAnnotation
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Characterize, ConvertToRequiredSpectrumFormat, GetDatabaseStartIndex
    ' 
    ' Class LipidLibraryParser
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getLipidClassEnum, ReadLibrary
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS


Public Class LipidMolecule

    Public Property LipidName As String
    Public Property SublevelLipidName As String
    Public Property LipidClass As LbmClass

    ''' <summary>
    ''' 0: incorrect, 
    ''' 1: submolecular level, 
    ''' 2: acyl level, 
    ''' 3: acyl &amp; positional level 
    ''' </summary>
    ''' <returns></returns>
    Public Property AnnotationLevel As Integer
    Public Property Adduct As AdductIon
    Public Property Mz As Single
    Public Property Rt As Single
    Public Property Smiles As String
    Public Property Formula As String
    Public Property InChIKey As String
    Public Property IonMode As IonModes
    Public Property LipidSubclass As String
    Public Property LipidCategory As String
    Public Property IsValidatedFormat As Boolean

    Public Property Score As Double

    Public Property TotalChainString As String

    ' glycero lipids
    Public Property Sn1AcylChainString As String
    Public Property Sn2AcylChainString As String
    Public Property Sn3AcylChainString As String
    Public Property Sn4AcylChainString As String

    Public Property TotalCarbonCount As Integer
    Public Property TotalDoubleBondCount As Integer
    Public Property TotalOxidizedCount As Integer

    Public Property Sn1CarbonCount As Integer ' it becomes sphingobase in ceramide species
    Public Property Sn1DoubleBondCount As Integer ' it becomes sphingobase in ceramide species
    Public Property Sn1Oxidizedount As Integer ' it becomes sphingobase in ceramide species
    Public Property Sn2CarbonCount As Integer ' it becomes acylchain in ceramide species
    Public Property Sn2DoubleBondCount As Integer ' it becomes acylchain in ceramide species
    Public Property Sn2Oxidizedount As Integer ' it becomes acylchain in ceramide species
    Public Property Sn3CarbonCount As Integer ' it becomes additional acyl chain in ceramide species like Cer-EOS
    Public Property Sn3DoubleBondCount As Integer ' it becomes additional acyl chain in ceramide species like Cer-EOS
    Public Property Sn3Oxidizedount As Integer ' it becomes additional acyl chain in ceramide species like Cer-EOS
    Public Property Sn4CarbonCount As Integer
    Public Property Sn4DoubleBondCount As Integer
    Public Property Sn4Oxidizedount As Integer
End Class

Public NotInheritable Class LipidAnnotation
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="queryMz"></param>
    ''' <param name="queryRt"></param>
    ''' <param name="msScanProp"></param>
    ''' <param name="RefMolecules">ref molecules must be sorted by mz before using this program</param>
    ''' <param name="ionMode"></param>
    ''' <param name="ms1tol"></param>
    ''' <param name="ms2tol"></param>
    ''' <returns></returns>
    Public Shared Function Characterize(queryMz As Double, queryRt As Double, msScanProp As IMSScanProperty, RefMolecules As List(Of LipidMolecule), ionMode As IonModes, ms1tol As Double, ms2tol As Double) As LipidMolecule

        Dim startID = GetDatabaseStartIndex(queryMz, ms1tol, RefMolecules)
        Dim molecules = New List(Of LipidMolecule)()
        For i = startID To RefMolecules.Count - 1
            Dim molecule = RefMolecules(i)
            Dim refMz = molecule.Mz
            Dim refClass = molecule.LipidClass
            Dim adduct = molecule.Adduct
            Dim lipidclass = refClass

            If refMz < queryMz - ms1tol Then Continue For
            If refMz > queryMz + ms1tol Then Exit For
            'Console.WriteLine(molecule.LipidName + molecule.Adduct);
            Dim result As LipidMolecule = Nothing

            Dim totalCarbon = molecule.TotalCarbonCount
            Dim totalDbBond = molecule.TotalDoubleBondCount
            Dim totalOxidized = molecule.TotalOxidizedCount

            'add MT
            If molecule.LipidName.Contains("+O") Then totalOxidized = 1
            '

            Dim sn1MaxCarbon = 36
            Dim sn1MaxDbBond = 12
            Dim sn1MinCarbon = 2
            Dim sn1MinDbBond = 0
            Dim sn1MaxOxidized = 0

            Dim sn2MaxCarbon = 36
            Dim sn2MaxDbBond = 12
            Dim sn2MinCarbon = 2
            Dim sn2MinDbBond = 0
            Dim sn2MaxOxidized = 6

            Dim sn3MaxCarbon = 36
            Dim sn3MaxDbBond = 12
            Dim sn3MinCarbon = 2
            Dim sn3MinDbBond = 0
            'var sn3Oxidized = 6;

            Select Case lipidclass
                Case LbmClass.PC
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylcholine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PE
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylethanolamine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PS
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylserine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PG
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PI
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylinositol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.BMP
                    result = LipidMsmsCharacterization.JudgeIfBismonoacylglycerophosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LNAPE
                    result = LipidMsmsCharacterization.JudgeIfNacylphosphatidylethanolamine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LNAPS
                    result = LipidMsmsCharacterization.JudgeIfNacylphosphatidylserine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.SM
                    result = LipidMsmsCharacterization.JudgeIfSphingomyelin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.TG
                    result = LipidMsmsCharacterization.JudgeIfTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.CAR
                    result = LipidMsmsCharacterization.JudgeIfAcylcarnitine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.CE
                    result = LipidMsmsCharacterization.JudgeIfCholesterylEster(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                    ' add MT, single or double chains pattern
                Case LbmClass.DG
                    result = LipidMsmsCharacterization.JudgeIfDag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.MG
                    result = LipidMsmsCharacterization.JudgeIfMag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.MGDG
                    result = LipidMsmsCharacterization.JudgeIfMgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DGDG
                    result = LipidMsmsCharacterization.JudgeIfDgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PMeOH
                    result = LipidMsmsCharacterization.JudgeIfPmeoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PEtOH
                    result = LipidMsmsCharacterization.JudgeIfPetoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PBtOH
                    result = LipidMsmsCharacterization.JudgeIfPbtoh(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.LPC
                    result = LipidMsmsCharacterization.JudgeIfLysopc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPE
                    result = LipidMsmsCharacterization.JudgeIfLysope(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PA
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidicacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPA
                    result = LipidMsmsCharacterization.JudgeIfLysopa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPG
                    result = LipidMsmsCharacterization.JudgeIfLysopg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPI
                    result = LipidMsmsCharacterization.JudgeIfLysopi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPS
                    result = LipidMsmsCharacterization.JudgeIfLysops(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherPC
                    result = LipidMsmsCharacterization.JudgeIfEtherpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherPE
                    result = LipidMsmsCharacterization.JudgeIfEtherpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherLPC
                    result = LipidMsmsCharacterization.JudgeIfEtherlysopc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherLPE
                    result = LipidMsmsCharacterization.JudgeIfEtherlysope(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.OxPC
                    result = LipidMsmsCharacterization.JudgeIfOxpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.OxPE
                    result = LipidMsmsCharacterization.JudgeIfOxpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.OxPG
                    result = LipidMsmsCharacterization.JudgeIfOxpg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.OxPI
                    result = LipidMsmsCharacterization.JudgeIfOxpi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.OxPS
                    result = LipidMsmsCharacterization.JudgeIfOxps(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.EtherMGDG
                    result = LipidMsmsCharacterization.JudgeIfEthermgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherDGDG
                    result = LipidMsmsCharacterization.JudgeIfEtherdgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DGTS
                    result = LipidMsmsCharacterization.JudgeIfDgts(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LDGTS
                    result = LipidMsmsCharacterization.JudgeIfLdgts(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DGCC
                    result = LipidMsmsCharacterization.JudgeIfDgcc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LDGCC
                    result = LipidMsmsCharacterization.JudgeIfLdgcc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DGGA
                    result = LipidMsmsCharacterization.JudgeIfGlcadg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.SQDG
                    result = LipidMsmsCharacterization.JudgeIfSqdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DLCL
                    result = LipidMsmsCharacterization.JudgeIfDilysocardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.FA
                    result = LipidMsmsCharacterization.JudgeIfFattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DMEDFA
                    result = LipidMsmsCharacterization.JudgeIfDmedFattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.FAHFA
                    result = LipidMsmsCharacterization.JudgeIfFahfa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DMEDFAHFA
                    result = LipidMsmsCharacterization.JudgeIfFahfaDMED(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.OxFA
                    result = LipidMsmsCharacterization.JudgeIfOxfattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized)
                Case LbmClass.DMEDOxFA
                    result = LipidMsmsCharacterization.JudgeIfDmedOxfattyacid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized)
                Case LbmClass.EtherOxPC
                    result = LipidMsmsCharacterization.JudgeIfEtheroxpc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.EtherOxPE
                    result = LipidMsmsCharacterization.JudgeIfEtheroxpe(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized, sn1MaxOxidized, sn2MaxOxidized)
                Case LbmClass.Cer_NS
                    result = LipidMsmsCharacterization.JudgeIfCeramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_NDS
                    result = LipidMsmsCharacterization.JudgeIfCeramidends(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.HexCer_NS
                    result = LipidMsmsCharacterization.JudgeIfHexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.HexCer_NDS
                    result = LipidMsmsCharacterization.JudgeIfHexceramidends(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Hex2Cer
                    result = LipidMsmsCharacterization.JudgeIfHexhexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Hex3Cer
                    result = LipidMsmsCharacterization.JudgeIfHexhexhexceramidens(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_AP
                    result = LipidMsmsCharacterization.JudgeIfCeramideap(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.HexCer_AP
                    result = LipidMsmsCharacterization.JudgeIfHexceramideap(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_AS
                    result = LipidMsmsCharacterization.JudgeIfCeramideas(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_ADS
                    result = LipidMsmsCharacterization.JudgeIfCeramideads(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_BS
                    result = LipidMsmsCharacterization.JudgeIfCeramidebs(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_BDS
                    result = LipidMsmsCharacterization.JudgeIfCeramidebds(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_NP
                    result = LipidMsmsCharacterization.JudgeIfCeramidenp(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.Cer_OS
                    result = LipidMsmsCharacterization.JudgeIfCeramideos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.SHexCer
                    result = LipidMsmsCharacterization.JudgeIfShexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized)
                Case LbmClass.GM3
                    result = LipidMsmsCharacterization.JudgeIfGm3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.DHSph
                    result = LipidMsmsCharacterization.JudgeIfSphinganine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)
                Case LbmClass.Sph
                    result = LipidMsmsCharacterization.JudgeIfSphingosine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)
                Case LbmClass.PhytoSph
                    result = LipidMsmsCharacterization.JudgeIfPhytosphingosine(msScanProp, ms2tol, refMz, molecule.TotalCarbonCount, molecule.TotalDoubleBondCount, adduct)

                    ' add mikiko takahashi, hetero type chains- and 3 or 4 chains pattern
                Case LbmClass.ADGGA
                    result = LipidMsmsCharacterization.JudgeIfAcylglcadg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.HBMP
                    result = LipidMsmsCharacterization.JudgeIfHemiismonoacylglycerophosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.EtherTG
                    result = LipidMsmsCharacterization.JudgeIfEthertag(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.CL
                    result = LipidMsmsCharacterization.JudgeIfCardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, sn3MinCarbon, sn3MaxCarbon, sn3MinDbBond, sn3MaxDbBond, adduct)
                Case LbmClass.MLCL
                    result = LipidMsmsCharacterization.JudgeIfLysocardiolipin(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.Cer_EOS
                    result = LipidMsmsCharacterization.JudgeIfCeramideeos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.Cer_EODS
                    result = LipidMsmsCharacterization.JudgeIfCeramideeods(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.HexCer_EOS
                    result = LipidMsmsCharacterization.JudgeIfHexceramideeos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.ASM
                    result = LipidMsmsCharacterization.JudgeIfAcylsm(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.Cer_EBDS
                    result = LipidMsmsCharacterization.JudgeIfAcylcerbds(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.AHexCer
                    result = LipidMsmsCharacterization.JudgeIfAcylhexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn3MinCarbon, sn3MaxCarbon, sn3MinDbBond, sn3MaxDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.ASHexCer
                    result = LipidMsmsCharacterization.JudgeIfAshexcer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn3MinCarbon, sn3MaxCarbon, sn3MinDbBond, sn3MaxDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                    'add 10/04/19
                Case LbmClass.EtherPI
                    result = LipidMsmsCharacterization.JudgeIfEtherpi(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherPS
                    result = LipidMsmsCharacterization.JudgeIfEtherps(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PI_Cer
                    result = LipidMsmsCharacterization.JudgeIfPicermide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized)
                Case LbmClass.PE_Cer
                    result = LipidMsmsCharacterization.JudgeIfPecermide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized)

                    'add 13/5/19 modified 20200218
                Case LbmClass.DCAE
                    result = LipidMsmsCharacterization.JudgeIfDcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)
                Case LbmClass.GDCAE
                    result = LipidMsmsCharacterization.JudgeIfGdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)
                Case LbmClass.GLCAE
                    result = LipidMsmsCharacterization.JudgeIfGlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)
                Case LbmClass.TDCAE
                    result = LipidMsmsCharacterization.JudgeIfTdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)
                Case LbmClass.TLCAE
                    result = LipidMsmsCharacterization.JudgeIfTlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

                Case LbmClass.NAE
                    result = LipidMsmsCharacterization.JudgeIfAnandamide(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                Case LbmClass.NAGly
                    If totalCarbon < 29 Then
                        result = LipidMsmsCharacterization.JudgeIfNAcylGlyOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                    Else
                        result = LipidMsmsCharacterization.JudgeIfFahfamidegly(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    End If

                Case LbmClass.NAGlySer
                    If totalCarbon < 29 Then
                        result = LipidMsmsCharacterization.JudgeIfNAcylGlySerOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                    Else
                        result = LipidMsmsCharacterization.JudgeIfFahfamideglyser(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    End If

                Case LbmClass.SL
                    result = LipidMsmsCharacterization.JudgeIfSulfonolipid(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct, totalOxidized)
                Case LbmClass.EtherPG
                    result = LipidMsmsCharacterization.JudgeIfEtherpg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherLPG
                    result = LipidMsmsCharacterization.JudgeIfEtherlysopg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.CoQ
                    result = LipidMsmsCharacterization.JudgeIfCoenzymeq(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                Case LbmClass.Vitamin_E
                    result = LipidMsmsCharacterization.JudgeIfVitaminEmolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.Vitamin_D
                    result = LipidMsmsCharacterization.JudgeIfVitaminDmolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                Case LbmClass.VAE
                    result = LipidMsmsCharacterization.JudgeIfVitaminaestermolecules(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                Case LbmClass.NAOrn
                    If totalCarbon < 29 Then
                        result = LipidMsmsCharacterization.JudgeIfNAcylOrnOxFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                    Else
                        result = LipidMsmsCharacterization.JudgeIfFahfamideorn(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    End If

                Case LbmClass.BRSE
                    result = LipidMsmsCharacterization.JudgeIfBrseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.CASE
                    result = LipidMsmsCharacterization.JudgeIfCaseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.SISE
                    result = LipidMsmsCharacterization.JudgeIfSiseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.STSE
                    result = LipidMsmsCharacterization.JudgeIfStseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                Case LbmClass.AHexBRS
                    result = LipidMsmsCharacterization.JudgeIfAhexbrseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.AHexCAS
                    result = LipidMsmsCharacterization.JudgeIfAhexcaseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.AHexCS
                    result = LipidMsmsCharacterization.JudgeIfAhexceSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.AHexSIS
                    result = LipidMsmsCharacterization.JudgeIfAhexsiseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.AHexSTS
                    result = LipidMsmsCharacterization.JudgeIfAhexstseSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                    'add 190528
                Case LbmClass.Cer_HS
                    result = LipidMsmsCharacterization.JudgeIfCeramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.Cer_HDS
                    result = LipidMsmsCharacterization.JudgeIfCeramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.Cer_NDOS
                    result = LipidMsmsCharacterization.JudgeIfCeramidedos(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.HexCer_HS
                    result = LipidMsmsCharacterization.JudgeIfHexceramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.HexCer_HDS
                    result = LipidMsmsCharacterization.JudgeIfHexceramideo(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                    '190801
                Case LbmClass.SHex
                    result = LipidMsmsCharacterization.JudgeIfSterolHexoside(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.BAHex
                    result = LipidMsmsCharacterization.JudgeIfSterolHexoside(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.SSulfate
                    result = LipidMsmsCharacterization.JudgeIfSterolSulfate(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.BASulfate
                    result = LipidMsmsCharacterization.JudgeIfSterolSulfate(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                    ' added 190811
                Case LbmClass.CerP
                    result = LipidMsmsCharacterization.JudgeIfCeramidePhosphate(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                    ' 2019/11/25 add
                Case LbmClass.SMGDG
                    result = LipidMsmsCharacterization.JudgeIfSmgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.EtherSMGDG
                    result = LipidMsmsCharacterization.JudgeIfEtherSmgdg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                    'add 20200218
                Case LbmClass.LCAE
                    result = LipidMsmsCharacterization.JudgeIfLcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

                Case LbmClass.KLCAE
                    result = LipidMsmsCharacterization.JudgeIfKlcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

                Case LbmClass.KDCAE
                    result = LipidMsmsCharacterization.JudgeIfKdcae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct, totalOxidized)

                    'add 20200714
                Case LbmClass.DMPE
                    result = LipidMsmsCharacterization.JudgeIfDiMethylPE(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.MMPE
                    result = LipidMsmsCharacterization.JudgeIfMonoMethylPE(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                Case LbmClass.MIPC
                    result = LipidMsmsCharacterization.JudgeIfMipc(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)

                    'add 20200720
                Case LbmClass.EGSE
                    result = LipidMsmsCharacterization.JudgeIfErgoSESpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.DEGSE
                    result = LipidMsmsCharacterization.JudgeIfDehydroErgoSESpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                    'add 20200812
                Case LbmClass.OxTG
                    result = LipidMsmsCharacterization.JudgeIfOxTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, totalOxidized, adduct)
                Case LbmClass.TG_EST
                    result = LipidMsmsCharacterization.JudgeIfFahfaTriacylglycerol(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, sn3MinCarbon, sn3MaxCarbon, sn3MinDbBond, sn3MaxDbBond, adduct)
                    'add 20200923
                Case LbmClass.DSMSE
                    result = LipidMsmsCharacterization.JudgeIfDesmosterolSpecies(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                    'add20210216
                Case LbmClass.GPNAE
                    result = LipidMsmsCharacterization.JudgeIfGpnae(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.MGMG
                    result = LipidMsmsCharacterization.JudgeIfMgmg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.DGMG

                    'add 20210315
                    result = LipidMsmsCharacterization.JudgeIfDgmg(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.GD1a
                    result = LipidMsmsCharacterization.JudgeIfGD1a(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.GD1b
                    result = LipidMsmsCharacterization.JudgeIfGD1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.GD2
                    result = LipidMsmsCharacterization.JudgeIfGD2(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.GD3
                    result = LipidMsmsCharacterization.JudgeIfGD3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.GM1
                    result = LipidMsmsCharacterization.JudgeIfGM1(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.GQ1b
                    result = LipidMsmsCharacterization.JudgeIfGQ1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.GT1b
                    result = LipidMsmsCharacterization.JudgeIfGT1b(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.NGcGM3
                    result = LipidMsmsCharacterization.JudgeIfNGcGM3(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.ST
                    result = LipidMsmsCharacterization.JudgeIfnoChainSterol(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)

                Case LbmClass.CSLPHex, LbmClass.BRSLPHex, LbmClass.CASLPHex, LbmClass.SISLPHex, LbmClass.STSLPHex
                    result = LipidMsmsCharacterization.JudgeIfSteroidWithLpa(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)


                Case LbmClass.CSPHex, LbmClass.BRSPHex, LbmClass.CASPHex, LbmClass.SISPHex, LbmClass.STSPHex
                    result = LipidMsmsCharacterization.JudgeIfSteroidWithPa(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    '20220201
                Case LbmClass.SPE
                    result = LipidMsmsCharacterization.JudgeIfSpeSpecies(molecule.LipidName, molecule.LipidClass, msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                    '20220322
                Case LbmClass.NAPhe
                    result = LipidMsmsCharacterization.JudgeIfNAcylPheFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.NATau
                    result = LipidMsmsCharacterization.JudgeIfNAcylTauFa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.PT
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylThreonine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    '20230407
                Case LbmClass.PC_d5
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylcholineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PE_d5
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylethanolamineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PS_d5
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylserineD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PG_d5
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylglycerolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.PI_d5
                    result = LipidMsmsCharacterization.JudgeIfPhosphatidylinositolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPC_d5
                    result = LipidMsmsCharacterization.JudgeIfLysopcD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPE_d5
                    result = LipidMsmsCharacterization.JudgeIfLysopeD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPG_d5
                    result = LipidMsmsCharacterization.JudgeIfLysopgD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPI_d5
                    result = LipidMsmsCharacterization.JudgeIfLysopiD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.LPS_d5
                    result = LipidMsmsCharacterization.JudgeIfLysopsD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.TG_d5
                    result = LipidMsmsCharacterization.JudgeIfTriacylglycerolD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, sn2MinCarbon, sn2MaxCarbon, sn2MinDbBond, sn2MaxDbBond, adduct)
                Case LbmClass.DG_d5
                    result = LipidMsmsCharacterization.JudgeIfDagD5(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.SM_d9
                    result = LipidMsmsCharacterization.JudgeIfSphingomyelinD9(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                Case LbmClass.CE_d7
                    result = LipidMsmsCharacterization.JudgeIfCholesterylEsterD7(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, adduct)
                Case LbmClass.Cer_NS_d7
                    result = LipidMsmsCharacterization.JudgeIfCeramidensD7(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    '20230424
                Case LbmClass.bmPC
                    Return LipidMsmsCharacterization.JudgeIfBetaMethylPhosphatidylcholine(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    '20230612
                Case LbmClass.NATryA
                    result = LipidMsmsCharacterization.JudgeIfNAcylTryA(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.NA5HT
                    result = LipidMsmsCharacterization.JudgeIfNAcyl5HT(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.WE
                    result = LipidMsmsCharacterization.JudgeIfWaxEster(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, sn1MinCarbon, sn1MaxCarbon, sn1MinDbBond, sn1MaxDbBond, adduct)
                    '20230626
                Case LbmClass.NAAla
                    result = LipidMsmsCharacterization.JudgeIfNAcylAla(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.NAGln
                    result = LipidMsmsCharacterization.JudgeIfNAcylGln(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.NALeu
                    result = LipidMsmsCharacterization.JudgeIfNAcylLeu(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.NAVal
                    result = LipidMsmsCharacterization.JudgeIfNAcylVal(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.NASer
                    result = LipidMsmsCharacterization.JudgeIfNAcylSer(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case LbmClass.BisMeLPA
                    result = LipidMsmsCharacterization.JudgeIfBismelpa(msScanProp, ms2tol, refMz, totalCarbon, totalDbBond, totalOxidized, adduct)
                Case Else
                    Return Nothing
            End Select

            If result IsNot Nothing Then
                molecules.Add(result)
            End If

            'Console.WriteLine("candidate {0}, suggested {1}, score {2}", molecule.LipidName, result.LipidName, result.Score);
            If result IsNot Nothing AndAlso result.AnnotationLevel = 2 Then
            Else
                'Console.WriteLine("candidate {0}, suggested {1}, score {2}", molecule.LipidName, "NA", "-1");
            End If
        Next

        If molecules.Count > 0 Then
            Return molecules.OrderByDescending(Function(n) n.Score).ToList()(0)
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' peaks are converted to 
    ''' 1. normalized spectrum where maximum intensity is normalized to 100
    ''' 2. ordered as higher intensity -> lower intensity
    ''' </summary>
    Public Shared Function ConvertToRequiredSpectrumFormat(peaks As List(Of SpectrumPeak)) As List(Of SpectrumPeak)
        Dim spectrum = New List(Of SpectrumPeak)()
        Dim maxintensity = peaks.Max(Function(n) n.Intensity)
        For Each peak In peaks
            spectrum.Add(New SpectrumPeak With {
                .mz = peak.mz,
                .Intensity = peak.Intensity / maxintensity * 100.0
            })
        Next
        Return spectrum.OrderByDescending(Function(n) n.Intensity).ToList()
    End Function

    Public Shared Function GetDatabaseStartIndex(mz As Double, tolerance As Double, molecules As List(Of LipidMolecule)) As Integer
        Dim targetMass = mz - tolerance
        Dim startIndex = 0, endIndex = molecules.Count - 1
        If targetMass > molecules(endIndex).Mz Then Return endIndex

        Dim counter = 0
        While counter < 10
            If molecules(startIndex).Mz <= targetMass AndAlso targetMass < molecules((startIndex + endIndex) / 2).Mz Then
                endIndex = (startIndex + endIndex) / 2
            ElseIf molecules((startIndex + endIndex) / 2).Mz <= targetMass AndAlso targetMass < molecules(endIndex).Mz Then
                startIndex = (startIndex + endIndex) / 2
            End If
            counter += 1
        End While
        Return startIndex
    End Function
End Class

Public NotInheritable Class LipidLibraryParser
    Private Sub New()
    End Sub

    '[0] Name [1] m/z [2] adduct
    Public Shared Function ReadLibrary(file As String) As List(Of LipidMolecule)
        Dim molecules = New List(Of LipidMolecule)()
        Using sr = New StreamReader(file, Encoding.ASCII)
            sr.ReadLine() ' header pathed
            While sr.Peek() > -1
                Dim line = sr.ReadLine()
                Dim lineArray = line.Split(Microsoft.VisualBasic.Strings.ChrW(9)) ' e.g. [0] PC 28:2+3O [1] 301.000 [2] [M+HCOO]-

                Dim nameString = lineArray(0) ' PC 28:2+3O

                ' add MT
                If nameString.Contains(":") = Not True Then
                    nameString = nameString & " :"
                End If

                Dim lipidClass = nameString.Split(" "c)(0) ' PC
                Dim lipidClassEnum = getLipidClassEnum(lipidClass)
                If lipidClassEnum = LbmClass.Undefined Then Continue While


                Dim mzString = lineArray(1) ' 301.000
                Dim mzValue = -1.0F
                If Not Single.TryParse(mzString, mzValue) Then Continue While

                Dim adductString = lineArray(2) ' [M+HCOO]-
                Dim adduct = AdductIon.GetAdductIon(adductString)
                If Not adduct.FormatCheck Then Continue While

                Dim chainString = nameString.Split(" "c)(1) ' case 18:2, d18:2, t18:2, 28:2+3O
                Dim totalCarbonString = chainString.Split(":"c)(0)
                If totalCarbonString.Contains("d") Then
                    totalCarbonString = totalCarbonString.Replace("d", "")
                End If
                If totalCarbonString.Contains("t") Then
                    totalCarbonString = totalCarbonString.Replace("t", "")
                End If
                If totalCarbonString.Contains("m") Then
                    totalCarbonString = totalCarbonString.Replace("m", "")
                End If


                'imagin 28:2+3O case
                Dim bondString = nameString.Split(":"c)(1) ' 2+3O
                Dim totalDoubleBondString = bondString.Split("+"c)(0) ' 2
                Dim totalOxidizedString = "0"
                If bondString.Split("+"c).Length > 1 Then
                    totalOxidizedString = bondString.Split("+"c)(1) '3O
                    totalOxidizedString = totalOxidizedString.Replace("O", "") '3
                End If

                ' Etheryzed case
                If totalDoubleBondString.Contains("e") Then
                    totalDoubleBondString = totalDoubleBondString.Replace("e", "")
                End If


                Dim totalOxidized As Integer, totalCarbon = -1, totalDoubleBond = -1
                Integer.TryParse(totalCarbonString, totalCarbon)
                Integer.TryParse(totalDoubleBondString, totalDoubleBond)
                Integer.TryParse(totalOxidizedString, totalOxidized)

                'if (totalCarbon <= 0 || totalDoubleBond < 0) continue;
                Dim molecule = New LipidMolecule() With {
.LipidName = lineArray(0),
.SublevelLipidName = lineArray(0),
.LipidClass = lipidClassEnum,
.Adduct = adduct,
.Mz = mzValue,
.TotalChainString = chainString,
.TotalCarbonCount = totalCarbon,
.TotalDoubleBondCount = totalDoubleBond,
.TotalOxidizedCount = totalOxidized
}
                molecules.Add(molecule)
            End While
        End Using
        Return molecules.OrderBy(Function(n) n.Mz).ToList()
    End Function

    Private Shared Function getLipidClassEnum(lipidClass As String) As LbmClass
        For Each lipid In System.Enum.GetValues(GetType(LbmClass)).Cast(Of LbmClass)()
            If Equals(lipid.ToString(), lipidClass) Then
                Return lipid
            End If
        Next
        Return LbmClass.Undefined
    End Function
End Class


