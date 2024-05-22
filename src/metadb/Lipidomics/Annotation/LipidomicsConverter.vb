#Region "Microsoft.VisualBasic::1adc550849d8e94bdb67df9193fec692, metadb\Lipidomics\Annotation\LipidomicsConverter.vb"

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

    '   Total Lines: 5506
    '    Code Lines: 1791 (32.53%)
    ' Comment Lines: 3288 (59.72%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 427 (7.76%)
    '     File Size: 219.33 KB


    ' Module LipidomicsConverter
    ' 
    '     Function: acylChainStringSeparatorVS2, ConvertLbmClassEnumToMsdialClassDefinitionVS2, ConvertMsdialClassDefinitionToLbmClassEnumVS2, ConvertMsdialClassDefinitionToSuperClassVS2, (+2 Overloads) ConvertMsdialLipidnameToLipidMoleculeObjectVS2
    '               GetLipidClasses, GetLipidHeaderString, getTotalChainString
    ' 
    '     Sub: RetrieveSterolHeaderChainStrings, SetBasicMoleculerProperties, setChainPropertiesVS2, (+2 Overloads) SetCoqMolecule, setDiAcylChainProperty
    '          (+2 Overloads) SetLipidAcylChainProperties, setMonoAcylChainProperty, (+2 Overloads) SetSingleLipidStructure, setTetraAcylChainProperty, setTriAcylChainProperty
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports BioNovoGene.BioDeep.MSEngine

Public Module LipidomicsConverter

    'public static LipidMolecule ConvertMsdialLipidnameToLipidMoleculeObject(MoleculeMsReference query) {
    '    var molecule = new LipidMolecule();

    '    switch (query.CompoundClass) {

    '        //Glycerolipid
    '        case "MAG":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "DAG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "TAG":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherTAG":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherDAG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Lyso phospholipid
    '        case "LPC":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LPE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LPG":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LPI":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LPS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LPA":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LDGTS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LDGCC":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Phospholipid
    '        case "PC":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PE":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PI":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PA":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "BMP":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HBMP":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "CL":
    '            setQuadAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "DLCL":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LCL":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Ether linked Lyso phospholipid
    '        case "EtherLPC":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherLPE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherLPG":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherLPI":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherLPS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherPC":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherPE":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        //Ether linked phospholipid
    '        case "EtherPG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherPI":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherPS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherMGDG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherDGDG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;


    '        //Oxidized phospholipid
    '        case "OxPC":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "OxPE":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "OxPG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "OxPI":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "OxPS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;


    '        //Oxidized ether linked phospholipid
    '        case "EtherOxPC":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "EtherOxPE":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Oxidized ether linked phospholipid
    '        case "PMeOH":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PEtOH":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PBtOH":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //N-acyl lipids
    '        case "LNAPE":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "LNAPS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "NAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "NAAG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "NAAGS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "NAAO":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Plantlipid
    '        case "MGDG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "DGDG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "SQDG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "DGTS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "DGCC":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "GlcADG":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AcylGlcADG":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "DGGA":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "ADGGA":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Sterols
    '        case "CE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "ACar":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "FA":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "FAHFA":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "CoQ":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;
    '        case "DCAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "GDCAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "GLCAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "TDCAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "TLCAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Vitamin":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;
    '        case "VAE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "BileAcid":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;
    '        case "BRSE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "CASE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "SISE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "STSE":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AHexCS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AHexBRS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AHexCAS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AHexSIS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AHexSTS":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "SHex":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;
    '        case "SSulfate":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;
    '        case "BAHex":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;
    '        case "BASulfate":
    '            setSingleMetaboliteInfor(molecule, query);
    '            break;


    '        //Sphingomyelin
    '        case "SM":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        //Ceramide
    '        case "CerP":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_ADS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_AS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_BDS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_BS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_EODS":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_EOS":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_NDS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_NS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_NP":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_AP":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_OS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_O":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_DOS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_NS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_NDS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_AP":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_O":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_HDS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_HS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexCer_EOS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexHexCer_NS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexHexHexCer_NS":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexHexCer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "HexHexHexCer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Hex2Cer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Hex3Cer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PE_Cer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "PI_Cer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        case "SHexCer":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "SL":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        case "GM3":
    '            setDoubleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cholesterol":
    '            setSingleMetaboliteInfor(molecule, query);
    '            //setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "CholesterolSulfate":
    '            setSingleMetaboliteInfor(molecule, query);
    '            //setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Phytosphingosine":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Sphinganine":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Sphingosine":
    '            setSingleAcylChainsLipidAnnotation(molecule, query);
    '            break;

    '        case "AcylCer_BDS":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AcylHexCer":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AcylSM":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "Cer_EBDS":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "AHexCer":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        case "ASM":
    '            setTripleAcylChainsLipidAnnotation(molecule, query);
    '            break;
    '        default:
    '            return null;
    '    }
    '    return molecule;
    '}

    Public Function ConvertMsdialLipidnameToLipidMoleculeObjectVS2(query As MoleculeMsReference) As LipidMolecule
        Dim molecule = New LipidMolecule()
        Dim lipidclass = query.CompoundClass

        ' FattyAcyls [FA], Glycerolipids [GL], Glycerophospholipids [GP], Sphingolipids [SP]
        '  SterolLipids [ST], PrenolLipids [PR], Saccharolipids [SL], Polyketides [PK]


        Dim lipidcategory = ConvertMsdialClassDefinitionToSuperClassVS2(lipidclass)
        Dim lbmclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(lipidclass)

        If Equals(lipidcategory, "FattyAcyls") OrElse Equals(lipidcategory, "Glycerolipids") OrElse Equals(lipidcategory, "Glycerophospholipids") OrElse Equals(lipidcategory, "Sphingolipids") OrElse lbmclass = LbmClass.VAE Then
            SetLipidAcylChainProperties(molecule, query)
        ElseIf Equals(lipidcategory, "SterolLipids") OrElse Equals(lipidcategory, "PrenolLipids") Then
            If lbmclass = LbmClass.Vitamin_D OrElse lbmclass = LbmClass.Vitamin_E OrElse lbmclass = LbmClass.SHex OrElse lbmclass = LbmClass.SSulfate OrElse lbmclass = LbmClass.BAHex OrElse lbmclass = LbmClass.BASulfate OrElse lbmclass = LbmClass.BileAcid OrElse Equals(query.Name, "Cholesterol") OrElse Equals(query.Name, "CholesterolSulfate") Then
                SetSingleLipidStructure(molecule, query)
            ElseIf lbmclass = LbmClass.CoQ Then
                SetCoqMolecule(molecule, query)
            Else
                SetLipidAcylChainProperties(molecule, query)
            End If
        End If

        molecule.LipidClass = lbmclass
        molecule.LipidCategory = lipidcategory
        molecule.LipidSubclass = lipidclass

        If Equals(molecule.LipidName, Nothing) OrElse Equals(molecule.LipidName, String.Empty) OrElse molecule.Adduct Is Nothing Then
            molecule.IsValidatedFormat = False
        Else
            molecule.IsValidatedFormat = True
        End If
        Return molecule
    End Function

    Public Function ConvertMsdialLipidnameToLipidMoleculeObjectVS2(lipidname As String, ontology As String) As LipidMolecule
        Dim molecule = New LipidMolecule()
        Dim lipidclass = ontology

        ' FattyAcyls [FA], Glycerolipids [GL], Glycerophospholipids [GP], Sphingolipids [SP]
        '  SterolLipids [ST], PrenolLipids [PR], Saccharolipids [SL], Polyketides [PK]


        Dim lipidcategory = ConvertMsdialClassDefinitionToSuperClassVS2(lipidclass)
        Dim lbmclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(ontology)

        'Console.WriteLine(lipidcategory + "\t" + lbmclass.ToString() + "\t" + ontology);
        If Equals(lipidcategory, "FattyAcyls") OrElse Equals(lipidcategory, "Glycerolipids") OrElse Equals(lipidcategory, "Glycerophospholipids") OrElse Equals(lipidcategory, "Sphingolipids") OrElse lbmclass = LbmClass.VAE Then
            SetLipidAcylChainProperties(molecule, lipidname, ontology)
        ElseIf Equals(lipidcategory, "SterolLipids") OrElse Equals(lipidcategory, "PrenolLipids") Then
            If lbmclass = LbmClass.Vitamin_D OrElse lbmclass = LbmClass.Vitamin_E OrElse lbmclass = LbmClass.SHex OrElse lbmclass = LbmClass.SSulfate OrElse lbmclass = LbmClass.BAHex OrElse lbmclass = LbmClass.BASulfate OrElse lbmclass = LbmClass.BileAcid OrElse Equals(lipidname, "Cholesterol") OrElse Equals(lipidname, "CholesterolSulfate") Then
                SetSingleLipidStructure(molecule, lipidname, ontology)
            ElseIf lbmclass = LbmClass.CoQ Then
                SetCoqMolecule(molecule, lipidname, ontology)
            Else
                SetLipidAcylChainProperties(molecule, lipidname, ontology)
            End If
        End If

        molecule.LipidClass = lbmclass
        molecule.LipidCategory = lipidcategory
        molecule.LipidSubclass = lipidclass

        If Equals(molecule.LipidName, Nothing) OrElse Equals(molecule.LipidName, String.Empty) Then
            molecule.IsValidatedFormat = False
        Else
            molecule.IsValidatedFormat = True
        End If
        Return molecule
    End Function


    'public static string LipidomicsAnnotationLevel(string lipidname, MoleculeMsReference query, string adduct)
    '{
    '    var lipidclass = lipidname.Split(' ')[0];
    '    if (lipidname.Contains("e;") || lipidname.Contains("p;") || lipidname.Contains("e+") || lipidname.Contains("e/"))
    '        lipidclass = "Ether" + lipidclass;

    '    var lbmLipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(query.CompoundClass);
    '    var querylipidClass = ConvertLbmClassEnumToMsdialClassDefinition(lbmLipidclass);
    '    var isInconsistency = false;
    '    if (lipidclass != querylipidClass) isInconsistency = true;
    '    if (lipidclass == "Cer-HS" && (querylipidClass == "Cer-AS" || querylipidClass == "Cer-BS")) isInconsistency = false;
    '    if (lipidclass == "Cer-HDS" && (querylipidClass == "Cer-ADS" || querylipidClass == "Cer-BDS")) isInconsistency = false;


    '    var level = "Class resolved";
    '    var lipidsemicoronCount = lipidname.Length - lipidname.Replace(";", "").Length;
    '    var querysemicoronCount = query.Name.Length - query.Name.Replace(";", "").Length;


    '    switch (query.CompoundClass) {

    '        //Glycerolipid
    '        case "MAG":
    '            level = "Chain resolved";
    '            break;
    '        case "DAG":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "TAG":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "EtherTAG":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "EtherDAG":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Lyso phospholipid
    '        case "LPC":
    '            level = "Chain resolved";
    '            break;
    '        case "LPE":
    '            level = "Chain resolved";
    '            break;
    '        case "LPG":
    '            level = "Chain resolved";
    '            break;
    '        case "LPI":
    '            level = "Chain resolved";
    '            break;
    '        case "LPS":
    '            level = "Chain resolved";
    '            break;
    '        case "LPA":
    '            level = "Chain resolved";
    '            break;
    '        case "LDGTS":
    '            level = "Chain resolved";
    '            break;
    '        case "LDGCC":
    '            level = "Chain resolved";
    '            break;

    '        //Phospholipid
    '        case "PC":
    '            if (adduct == "[M+H]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PE":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PG":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PS":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PA":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "BMP":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HBMP":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "CL":
    '            if (lipidsemicoronCount == querysemicoronCount) {
    '                if (lipidname.Split(';')[1].Split('-').Length < 4)
    '                    level = "Chain semi-resolved";
    '                else
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DLCL":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "LCL":
    '            if (lipidsemicoronCount == querysemicoronCount) {
    '                if (lipidname.Split(';')[1].Split('-').Length < 3)
    '                    level = "Chain semi-resolved";
    '                else
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ether linked Lyso phospholipid
    '        case "EtherLPC":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPE":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPG":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPI":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPS":
    '            level = "Chain resolved";
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPE":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        //Ether linked phospholipid
    '        case "EtherPG":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherMGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherDGDG":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;


    '        //Oxidized phospholipid
    '        case "OxPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPG":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;


    '        //Oxidized ether linked phospholipid
    '        case "EtherOxPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherOxPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Oxidized ether linked phospholipid
    '        case "PMeOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PEtOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PBtOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //N-acyl lipids
    '        case "LNAPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "LNAPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "NAE":
    '            level = "Chain resolved";
    '            break;
    '        case "NAAG":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "NAAGS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "NAAO":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;

    '        //Plantlipid
    '        case "MGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "SQDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DGTS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "DGCC":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "DGGA":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "ADGGA":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;

    '        //Sterols
    '        case "CE":
    '            level = "Chain resolved";
    '            break;
    '        case "ACar":
    '            level = "Chain resolved";
    '            break;
    '        case "FA":
    '            level = "Chain resolved";
    '            break;
    '        case "FAHFA":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "CoQ":
    '            level = "Chain resolved";
    '            break;
    '        case "DCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "GDCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "GLCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "TDCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "TLCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "Vitamin":
    '            level = "Lipid assigned";
    '            break;
    '        case "VAE":
    '            level = "Chain resolved";
    '            break;
    '        case "BileAcid":
    '            level = "Lipid assigned";
    '            break;
    '        case "BRSE":
    '            level = "Chain resolved";
    '            break;
    '        case "CASE":
    '            level = "Chain resolved";
    '            break;
    '        case "SISE":
    '            level = "Chain resolved";
    '            break;
    '        case "STSE":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexCS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexBRS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexCAS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexSIS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexSTS":
    '            level = "Chain resolved";
    '            break;
    '        case "SHex":
    '            level = "Lipid assigned";
    '            break;
    '        case "SSulfate":
    '            level = "Lipid assigned";
    '            break;
    '        case "BAHex":
    '            level = "Lipid assigned";
    '            break;
    '        case "BASulfate":
    '            level = "Lipid assigned";
    '            break;


    '        //Sphingomyelin
    '        case "SM":
    '            if (query.Name.Contains("t")) {

    '            }
    '            else if (adduct == "[M+H]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ceramide
    '        case "CerP":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_ADS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_AS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_BDS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_BS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_EODS":
    '            if (lipidsemicoronCount == querysemicoronCount) {
    '                if (lipidname.Split(';')[1].Contains("-O-") && lipidname.Split(';')[1].Contains("/"))
    '                    level = "Chain resolved";
    '                else
    '                    level = "Chain semi-resolved";
    '            }
    '            break;
    '        case "Cer_EOS":
    '            if (lipidsemicoronCount == querysemicoronCount) {
    '                if (lipidname.Split(';')[1].Contains("-O-") && lipidname.Split(';')[1].Contains("/"))
    '                    level = "Chain resolved";
    '                else
    '                    level = "Chain semi-resolved";
    '            }
    '            break;
    '        case "Cer_NDS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_NS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_NP":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_AP":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_OS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_O":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_DOS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_NS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_NDS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_AP":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_O":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_HDS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_HS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_EOS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain semi-resolved";
    '            break;
    '        case "HexHexCer_NS":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HexHexHexCer_NS":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HexHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HexHexHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Hex2Cer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Hex3Cer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PE_Cer":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PI_Cer":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        case "SHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "SL":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;

    '        case "GM3":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == querysemicoronCount)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Cholesterol":
    '            level = "Lipid assigned";
    '            break;
    '        case "CholesterolSulfate":
    '            level = "Lipid assigned";
    '            break;
    '        case "Phytosphingosine":
    '            level = "Chain resolved";
    '            break;
    '        case "Sphinganine":
    '            level = "Chain resolved";
    '            break;
    '        case "Sphingosine":
    '            level = "Chain resolved";
    '            break;
    '        case "AcylHexCer":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_EBDS":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "AHexCer":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain resolved";
    '            break;
    '        case "ASM":
    '            if (lipidsemicoronCount == querysemicoronCount)
    '                level = "Chain semi-resolved";
    '            break;
    '        case "Others":
    '            level = "Annotated by experimental MS/MS";
    '            break;
    '        default:
    '            break;
    '    }

    '    if (isInconsistency) {
    '        return level + "; inconsistency with MSP file";
    '    }
    '    else
    '        return level;
    '}

    'public static string LipidomicsAnnotationLevel(string lipidname, string ontology, string adduct) {
    '    var lipidclass = lipidname.Split(' ')[0];
    '    if (lipidname.Contains("e;") || lipidname.Contains("p;") || lipidname.Contains("e+") || lipidname.Contains("e/"))
    '        lipidclass = "Ether" + lipidclass;

    '    var level = "Class resolved";
    '    var lipidsemicoronCount = lipidname.Length - lipidname.Replace(";", "").Length;

    '    switch (ontology) {

    '        //Glycerolipid
    '        case "MAG":
    '            level = "Chain resolved";
    '            break;
    '        case "DAG":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "TAG":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "EtherTAG":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "EtherDAG":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Lyso phospholipid
    '        case "LPC":
    '            level = "Chain resolved";
    '            break;
    '        case "LPE":
    '            level = "Chain resolved";
    '            break;
    '        case "LPG":
    '            level = "Chain resolved";
    '            break;
    '        case "LPI":
    '            level = "Chain resolved";
    '            break;
    '        case "LPS":
    '            level = "Chain resolved";
    '            break;
    '        case "LPA":
    '            level = "Chain resolved";
    '            break;
    '        case "LDGTS":
    '            level = "Chain resolved";
    '            break;
    '        case "LDGCC":
    '            level = "Chain resolved";
    '            break;

    '        //Phospholipid
    '        case "PC":
    '            if (adduct == "[M+H]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PE":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PG":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PS":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PA":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "BMP":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HBMP":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "CL":
    '            if (lipidsemicoronCount == 2) {
    '                if (lipidname.Split(';')[1].Split('-').Length < 4)
    '                    level = "Chain semi-resolved";
    '                else
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DLCL":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "LCL":
    '            if (lipidsemicoronCount == 3) {
    '                if (lipidname.Split(';')[1].Split('-').Length < 3)
    '                    level = "Chain semi-resolved";
    '                else
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ether linked Lyso phospholipid
    '        case "EtherLPC":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPE":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPG":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPI":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPS":
    '            level = "Chain resolved";
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPE(Plasmalogen)": {
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        }
    '        case "EtherPE":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        //Ether linked phospholipid
    '        case "EtherPG":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherMGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherDGDG":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;


    '        //Oxidized phospholipid
    '        case "OxPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPG":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;


    '        //Oxidized ether linked phospholipid
    '        case "EtherOxPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherOxPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Oxidized ether linked phospholipid
    '        case "PMeOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PEtOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PBtOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //N-acyl lipids
    '        case "LNAPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "LNAPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "NAE":
    '            level = "Chain resolved";
    '            break;
    '        case "NAAG":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "NAAGS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "NAAO":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;

    '        //Plantlipid
    '        case "MGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "SQDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DGTS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "DGCC":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "DGGA":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "ADGGA":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;

    '        //Sterols
    '        case "CE":
    '            level = "Chain resolved";
    '            break;
    '        case "ACar":
    '            level = "Chain resolved";
    '            break;
    '        case "FA":
    '            level = "Chain resolved";
    '            break;
    '        case "FAHFA":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "CoQ":
    '            level = "Chain resolved";
    '            break;
    '        case "DCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "GDCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "GLCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "TDCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "TLCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "Vitamin":
    '            level = "Lipid assigned";
    '            break;
    '        case "VAE":
    '            level = "Chain resolved";
    '            break;
    '        case "BileAcid":
    '            level = "Lipid assigned";
    '            break;
    '        case "BRSE":
    '            level = "Chain resolved";
    '            break;
    '        case "CASE":
    '            level = "Chain resolved";
    '            break;
    '        case "SISE":
    '            level = "Chain resolved";
    '            break;
    '        case "STSE":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexCS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexBRS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexCAS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexSIS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexSTS":
    '            level = "Chain resolved";
    '            break;
    '        case "SHex":
    '            level = "Lipid assigned";
    '            break;
    '        case "SSulfate":
    '            level = "Lipid assigned";
    '            break;
    '        case "BAHex":
    '            level = "Lipid assigned";
    '            break;
    '        case "BASulfate":
    '            level = "Lipid assigned";
    '            break;


    '        //Sphingomyelin
    '        case "SM":
    '            if (lipidname.Contains("t")) {

    '            }
    '            else if (adduct == "[M+H]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ceramide
    '        case "CerP":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_ADS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_AS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_BDS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_BS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_EODS":
    '            if (lipidsemicoronCount == 2) {
    '                if (lipidname.Split(';')[1].Contains("-O-") && lipidname.Split(';')[1].Contains("/"))
    '                    level = "Chain resolved";
    '                else
    '                    level = "Chain semi-resolved";
    '            }
    '            break;
    '        case "Cer_EOS":
    '            if (lipidsemicoronCount == 2) {
    '                if (lipidname.Split(';')[1].Contains("-O-") && lipidname.Split(';')[1].Contains("/"))
    '                    level = "Chain resolved";
    '                else
    '                    level = "Chain semi-resolved";
    '            }
    '            break;
    '        case "Cer_NDS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_NS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_NP":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_AP":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_OS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_O":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_DOS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_NS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_NDS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_AP":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_O":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_HDS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_HS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_EOS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain semi-resolved";
    '            break;
    '        case "HexHexCer_NS":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HexHexHexCer_NS":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HexHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HexHexHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Hex2Cer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Hex3Cer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PE_Cer":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PI_Cer":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        case "SHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "SL":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;

    '        case "GM3":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 2)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Cholesterol":
    '            level = "Lipid assigned";
    '            break;
    '        case "CholesterolSulfate":
    '            level = "Lipid assigned";
    '            break;
    '        case "Phytosphingosine":
    '            level = "Chain resolved";
    '            break;
    '        case "Sphinganine":
    '            level = "Chain resolved";
    '            break;
    '        case "Sphingosine":
    '            level = "Chain resolved";
    '            break;
    '        case "AcylHexCer":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_EBDS":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "AHexCer":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain resolved";
    '            break;
    '        case "ASM":
    '            if (lipidsemicoronCount == 2)
    '                level = "Chain semi-resolved";
    '            break;
    '        case "Others":
    '            level = "Annotated by experimental MS/MS";
    '            break;
    '        default:
    '            break;
    '    }
    '    return level;
    '}

    'public static string LipidomicsAnnotationLevelVS2(string lipidname, string ontology, string adduct) {
    '    var level = "Class resolved";
    '    var lipidsemicoronCount = lipidname.Length - lipidname.Replace("|", "").Length;

    '    switch (ontology) {
    '        //Glycerolipid
    '        case "MG":
    '            level = "Chain resolved";
    '            break;
    '        case "DG":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "TG":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "EtherTG":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "EtherDG":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Lyso phospholipid
    '        case "LPC":
    '            level = "Chain resolved";
    '            break;
    '        case "LPE":
    '            level = "Chain resolved";
    '            break;
    '        case "LPG":
    '            level = "Chain resolved";
    '            break;
    '        case "LPI":
    '            level = "Chain resolved";
    '            break;
    '        case "LPS":
    '            level = "Chain resolved";
    '            break;
    '        case "LPA":
    '            level = "Chain resolved";
    '            break;
    '        case "LDGTS":
    '            level = "Chain resolved";
    '            break;
    '        case "LDGCC":
    '            level = "Chain resolved";
    '            break;

    '        //Phospholipid
    '        case "PC":
    '            if (adduct == "[M+H]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PE":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PG":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PS":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PA":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "BMP":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "HBMP":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "CL":
    '            if (lipidsemicoronCount == 1) {
    '                if (lipidname.Split('|')[1].Split('_').Length < 4)
    '                    level = "Chain semi-resolved";
    '                else
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DLCL":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "MLCL":
    '            if (lipidsemicoronCount == 1) {
    '                if (lipidname.Split('|')[1].Split('_').Length < 3)
    '                    level = "Chain semi-resolved";
    '                else
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ether linked Lyso phospholipid
    '        case "EtherLPC":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPE":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPG":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPI":
    '            level = "Chain resolved";
    '            break;
    '        case "EtherLPS":
    '            level = "Chain resolved";
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPE(Plasmalogen)": {
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        }
    '        case "EtherPE":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        //Ether linked phospholipid
    '        case "EtherPG":
    '            if (adduct == "[M+H]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ether linked phospholipid
    '        case "EtherMGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherSMGDG":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherDGDG":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;


    '        //Oxidized phospholipid
    '        case "OxPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPG":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPI":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "OxPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;


    '        //Oxidized ether linked phospholipid
    '        case "EtherOxPC":
    '            if (adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "EtherOxPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Oxidized ether linked phospholipid
    '        case "PMeOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PEtOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PBtOH":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //N-acyl lipids
    '        case "LNAPE":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "LNAPS":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "NAE":
    '            level = "Chain resolved";
    '            break;
    '        case "NAGly":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "NAGlySer":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "NAOrn":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;

    '        //Plantlipid
    '        case "MGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DGDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "SQDG":
    '            if (adduct == "[M+NH4]+" || adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "DGTS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "DGCC":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "DGGA":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "ADGGA":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;

    '        //Sterols
    '        case "CE":
    '            level = "Chain resolved";
    '            break;
    '        case "CAR":
    '            level = "Chain resolved";
    '            break;
    '        case "FA":
    '            level = "Chain resolved";
    '            break;
    '        case "FAHFA":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "CoQ":
    '            level = "Chain resolved";
    '            break;
    '        case "DCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "GDCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "GLCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "TDCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "TLCAE":
    '            level = "Chain resolved";
    '            break;
    '        case "Vitamin_E":
    '            level = "Lipid assigned";
    '            break;
    '        case "VitaminDE":
    '            level = "Lipid assigned";
    '            break;
    '        case "VAE":
    '            level = "Chain resolved";
    '            break;
    '        case "BileAcid":
    '            level = "Lipid assigned";
    '            break;
    '        case "BRSE":
    '            level = "Chain resolved";
    '            break;
    '        case "CASE":
    '            level = "Chain resolved";
    '            break;
    '        case "SISE":
    '            level = "Chain resolved";
    '            break;
    '        case "STSE":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexCS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexBRS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexCAS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexSIS":
    '            level = "Chain resolved";
    '            break;
    '        case "AHexSTS":
    '            level = "Chain resolved";
    '            break;
    '        case "SHex":
    '            level = "Lipid assigned";
    '            break;
    '        case "SSulfate":
    '            level = "Lipid assigned";
    '            break;
    '        case "BAHex":
    '            level = "Lipid assigned";
    '            break;
    '        case "BASulfate":
    '            level = "Lipid assigned";
    '            break;


    '        //Sphingomyelin
    '        case "SM":
    '            if (lipidname.Contains("t")) {

    '            }
    '            else if (adduct == "[M+H]+" || adduct == "[M+CH3COO]-" || adduct == "[M+HCOO]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        //Ceramide
    '        case "CerP":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_ADS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_AS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_BDS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_BS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_EODS":
    '            if (lipidsemicoronCount == 1) {
    '                if (lipidname.Split('|')[1].Contains("(FA") && lipidname.Split('|')[1].Contains("/"))
    '                    level = "Chain resolved";
    '                else
    '                    level = "Chain semi-resolved";
    '            }
    '            break;
    '        case "Cer_EOS":
    '            if (lipidsemicoronCount == 1) {
    '                if (lipidname.Split('|')[1].Contains("(FA") && lipidname.Split('|')[1].Contains("/"))
    '                    level = "Chain resolved";
    '                else
    '                    level = "Chain semi-resolved";
    '            }
    '            break;
    '        case "Cer_NDS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_NS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_NP":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_AP":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_OS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_O":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "Cer_DOS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_NS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_NDS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_AP":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_O":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_HDS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_HS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "HexCer_EOS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain semi-resolved";
    '            break;
    '        case "Hex2Cer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Hex3Cer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PE_Cer":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "PI_Cer":
    '            if (adduct == "[M-H]-") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;

    '        case "SHexCer":
    '            if (adduct == "[M+H]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "SL":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;

    '        case "GM3":
    '            if (adduct == "[M+NH4]+") {
    '                if (lipidsemicoronCount == 1)
    '                    level = "Chain resolved";
    '            }
    '            break;
    '        case "Cholesterol":
    '            level = "Lipid assigned";
    '            break;
    '        case "CholesterolSulfate":
    '            level = "Lipid assigned";
    '            break;
    '        case "PhytoSph":
    '            level = "Chain resolved";
    '            break;
    '        case "DHSph":
    '            level = "Chain resolved";
    '            break;
    '        case "Sph":
    '            level = "Chain resolved";
    '            break;
    '        case "Cer_EBDS":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "AHexCer":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain resolved";
    '            break;
    '        case "ASM":
    '            if (lipidsemicoronCount == 1)
    '                level = "Chain semi-resolved";
    '            break;
    '        case "Others":
    '            level = "Annotated by experimental MS/MS";
    '            break;
    '        default:
    '            break;
    '    }
    '    return level;
    '}


    'public static string GetLipoqualityDatabaseLinkUrl(MoleculeMsReference query) {
    '    var molecule = ConvertMsdialLipidnameToLipidMoleculeObject(query);
    '    if (molecule == null) return string.Empty;
    '    return "http://jcbl.jp/wiki/Lipoquality:Resource?lc=" + molecule.LipidName + "&ct=c";
    '}

    ' now for Cholesterol and CholesterolSulfate
    'private static void setSingleMetaboliteInfor(LipidMolecule molecule, MoleculeMsReference query) {

    '    var name = query.Name;
    '    var nameArray = name.Split(';').ToArray();

    '    var lipidinfo = nameArray[0].Trim();
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(query.CompoundClass);

    '    molecule.Mz = (float)query.PrecursorMz;
    '    molecule.Smiles = query.SMILES;
    '    molecule.InChIKey = query.InChIKey;
    '    molecule.Formula = query.Formula.FormulaString;
    '    molecule.Adduct = query.AdductType;
    '    molecule.IonMode = query.IonMode;

    '    molecule.LipidName = lipidinfo;
    '    molecule.SublevelLipidName = lipidinfo;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = "0:0";
    '    molecule.TotalCarbonCount = 0;
    '    molecule.TotalDoubleBondCount = 0;
    '    molecule.Sn1AcylChainString = "0:0";
    '    molecule.Sn1CarbonCount = 0;
    '    molecule.Sn1DoubleBondCount = 0;
    '}

    ' now for Cholesterol and CholesterolSulfate
    Public Sub SetSingleLipidStructure(molecule As LipidMolecule, query As MoleculeMsReference)

        Dim lipidinfo = query.Name
        SetBasicMoleculerProperties(molecule, query)

        molecule.LipidName = lipidinfo
        molecule.SublevelLipidName = lipidinfo
        molecule.TotalChainString = "0:0"
        molecule.TotalCarbonCount = 0
        molecule.TotalDoubleBondCount = 0
        molecule.Sn1AcylChainString = "0:0"
        molecule.Sn1CarbonCount = 0
        molecule.Sn1DoubleBondCount = 0
    End Sub

    Public Sub SetSingleLipidStructure(molecule As LipidMolecule, lipidname As String, ontology As String)

        molecule.LipidName = lipidname
        molecule.SublevelLipidName = lipidname
        molecule.TotalChainString = "0:0"
        molecule.TotalCarbonCount = 0
        molecule.TotalDoubleBondCount = 0
        molecule.Sn1AcylChainString = "0:0"
        molecule.Sn1CarbonCount = 0
        molecule.Sn1DoubleBondCount = 0
    End Sub

    Public Sub SetCoqMolecule(molecule As LipidMolecule, query As MoleculeMsReference)

        SetBasicMoleculerProperties(molecule, query)

        Dim lipidinfo = query.Name
        Dim carbonCountString = lipidinfo.Substring(3) ' CoQ3 -> 3
        Dim carbonCount = 0
        Integer.TryParse(carbonCountString, carbonCount)

        molecule.LipidName = lipidinfo
        molecule.SublevelLipidName = lipidinfo
        molecule.TotalChainString = carbonCountString
        molecule.TotalCarbonCount = carbonCount
        molecule.TotalDoubleBondCount = 0
        molecule.Sn1AcylChainString = carbonCountString
        molecule.Sn1CarbonCount = carbonCount
        molecule.Sn1DoubleBondCount = 0
    End Sub

    Public Sub SetCoqMolecule(molecule As LipidMolecule, lipidname As String, ontology As String)

        Dim carbonCountString = lipidname.Substring(3) ' CoQ3 -> 3
        Dim carbonCount = 0
        Integer.TryParse(carbonCountString, carbonCount)

        molecule.LipidName = lipidname
        molecule.SublevelLipidName = lipidname
        molecule.TotalChainString = carbonCountString
        molecule.TotalCarbonCount = carbonCount
        molecule.TotalDoubleBondCount = 0
        molecule.Sn1AcylChainString = carbonCountString
        molecule.Sn1CarbonCount = carbonCount
        molecule.Sn1DoubleBondCount = 0
    End Sub

    'public static void setSingleMetaboliteInfor(LipidMolecule molecule, string lipidname, string lipidclassString) {

    '    var name = lipidname;
    '    var nameArray = name.Split(';').ToArray();

    '    var lipidinfo = name;
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(lipidclassString);

    '    molecule.LipidName = lipidinfo;
    '    molecule.SublevelLipidName = lipidinfo;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = "0:0";
    '    molecule.TotalCarbonCount = 0;
    '    molecule.TotalDoubleBondCount = 0;
    '    molecule.Sn1AcylChainString = "0:0";
    '    molecule.Sn1CarbonCount = 0;
    '    molecule.Sn1DoubleBondCount = 0;
    '}


    'private static void setSingleAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    MoleculeMsReference query) {
    '    var name = query.Name;
    '    var nameArray = name.Split(';').ToArray();

    '    var lipidinfo = nameArray[0].Trim();
    '    //if (lipidinfo == "PI-Cer d34:0+O") {
    '    //    Console.WriteLine();
    '    //}

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(query.CompoundClass);

    '    if (lipidinfo.Split(' ').Length < 2) return;
    '    var totalChain = lipidinfo.Split(' ')[1];
    '    var sn1AcylChainString = lipidinfo.Split(' ')[1];
    '    if (totalChain == null || totalChain == string.Empty || !totalChain.Contains(':') || (totalChain.Contains('(') && !totalChain.Contains('+'))) return;

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;

    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);
    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);

    '    //var totalCarbon = int.Parse(totalChain.Split(':')[0]);
    '    //var totalDouble = int.Parse(totalChain.Split(':')[1]);
    '    //var sn1Carbon = int.Parse(sn1AcylChain.Split(':')[0]);
    '    //var sn1DoubleBond = int.Parse(sn1AcylChain.Split(':')[1]);

    '    molecule.Mz = (float)query.PrecursorMz;
    '    molecule.Smiles = query.SMILES;
    '    molecule.InChIKey = query.InChIKey;
    '    molecule.Formula = query.Formula.FormulaString;
    '    molecule.Adduct = query.AdductType;
    '    molecule.IonMode = query.IonMode;

    '    molecule.LipidName = lipidinfo;
    '    molecule.SublevelLipidName = lipidinfo;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '}

    Public Sub SetLipidAcylChainProperties(molecule As LipidMolecule, query As MoleculeMsReference)
        Dim lipidname = query.Name.Trim() ' e.g. ST 28:2;O;Hex;PA 12:0_12:0, SE 28:2/8:0
        Dim chainStrings = acylChainStringSeparatorVS2(lipidname)
        Dim ontology = query.CompoundClass
        'var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClassVS2(ontology);

        ' set basic properties
        SetBasicMoleculerProperties(molecule, query)
        If chainStrings Is Nothing Then Return

        Select Case chainStrings.Count()
            Case 1
                setMonoAcylChainProperty(molecule, lipidname, ontology, chainStrings)
            Case 2
                setDiAcylChainProperty(molecule, lipidname, ontology, chainStrings)
            Case 3
                setTriAcylChainProperty(molecule, lipidname, ontology, chainStrings)
            Case 4
                setTetraAcylChainProperty(molecule, lipidname, ontology, chainStrings)
        End Select

        Return
    End Sub

    Public Sub SetLipidAcylChainProperties(molecule As LipidMolecule, lipidname As String, ontology As String)
        Dim chainStrings = acylChainStringSeparatorVS2(lipidname)
        If chainStrings Is Nothing Then Return
        Select Case chainStrings.Count()
            Case 1
                setMonoAcylChainProperty(molecule, lipidname, ontology, chainStrings)
            Case 2
                setDiAcylChainProperty(molecule, lipidname, ontology, chainStrings)
            Case 3
                setTriAcylChainProperty(molecule, lipidname, ontology, chainStrings)
            Case 4
                setTetraAcylChainProperty(molecule, lipidname, ontology, chainStrings)
        End Select

        Return
    End Sub


    Public Sub SetBasicMoleculerProperties(molecule As LipidMolecule, query As MoleculeMsReference)
        ' var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(query.CompoundClass);
        molecule.Mz = CSng(query.PrecursorMz)
        molecule.Smiles = query.SMILES
        molecule.InChIKey = query.InChIKey
        molecule.Formula = query.Formula.ToString

        'Console.WriteLine(query.Name + "\t" + query.AdductIonBean.AdductIonName);

        molecule.Adduct = query.AdductType
        molecule.IonMode = query.IonMode
        ' molecule.LipidClass = lipidclass;
    End Sub

    Private Sub setMonoAcylChainProperty(molecule As LipidMolecule, lipidname As String, ontology As String, chainStrings As List(Of String))
        If chainStrings.Count <> 1 Then Return
        Dim lipidclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(ontology)
        Dim sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount As Integer
        setChainPropertiesVS2(chainStrings(0), sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount)
        Dim totalCarbonCount = sn1CarbonCount
        Dim totalDoubleBond = sn1DoubleBond
        Dim totalOxidizedCount = sn1OxidizedCount
        'var totalChain = getTotalChainString(totalCarbonCount, totalDoubleBond, totalOxidizedCount, lipidclass);
        Dim totalChain = chainStrings(0)

        molecule.LipidName = lipidname
        molecule.SublevelLipidName = lipidname
        molecule.TotalChainString = totalChain
        molecule.TotalCarbonCount = totalCarbonCount
        molecule.TotalDoubleBondCount = totalDoubleBond
        molecule.TotalOxidizedCount = totalOxidizedCount
        molecule.Sn1AcylChainString = chainStrings(0)
        molecule.Sn1CarbonCount = sn1CarbonCount
        molecule.Sn1DoubleBondCount = sn1DoubleBond
        molecule.Sn1Oxidizedount = sn1OxidizedCount
    End Sub

    'public static LipidMolecule GetLipidMoleculeNameProperties(string lipidname) {
    '    if (lipidname.Split(' ').Length == 1) return new LipidMolecule() { SublevelLipidName = lipidname };

    '    var lipidheader = lipidname.Split(' ')[0];
    '    var acylchains = lipidname.Split(' ')[1];
    '    var chainStrings = acylChainStringSeparator(lipidname);
    '    var chainsCount = chainStrings.Count();
    '    if (chainsCount == 0) return new LipidMolecule() { SublevelLipidName = lipidname };

    '    var molecule = new LipidMolecule();
    '    switch (chainsCount) {
    '        case 1: setSingleAcylChainsLipidAnnotation(molecule, lipidname, lipidheader); break;
    '        case 2: setDoubleAcylChainsLipidAnnotation(molecule, lipidname, lipidheader); break;
    '        case 3: setTripleAcylChainsLipidAnnotation(molecule, lipidname, lipidheader); break;
    '        case 4: setQuadAcylChainsLipidAnnotation(molecule, lipidname, lipidheader); break;
    '    }
    '    return molecule;
    '}


    'public static void setSingleAcylChainsLipidAnnotation(LipidMolecule molecule, 
    '    string lipidname, string lipidclassString) {
    '    var name = lipidname;
    '    var lipidinfo = name;
    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(lipidclassString);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(lipidclassString);

    '    if (lipidinfo.Split(' ').Length < 2) return;
    '    var totalChain = lipidinfo.Split(' ')[1];
    '    var sn1AcylChainString = lipidinfo.Split(' ')[1];
    '    if (totalChain == null || totalChain == string.Empty || !totalChain.Contains(':') || (totalChain.Contains('(') && !totalChain.Contains('+'))) return;

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;

    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);
    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);

    '    molecule.LipidName = lipidinfo;
    '    molecule.SublevelLipidName = lipidinfo;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '}

    'private static void setDoubleAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    MoleculeMsReference query) {

    '    var nameArray = query.Name.Split(';').ToArray();
    '    if (nameArray.Length == 2) { // e.g. in positive PC Na adduct, only sublevel information is resigtered like PC 36:3; [M+Na]+
    '        setSingleAcylChainsLipidAnnotation(molecule, query);
    '        return;
    '    }

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(query.CompoundClass);

    '    //if (lipidclass == LbmClass.Cer_O) {
    '    //    Console.WriteLine();
    '    //}


    '    var sublevelLipidName = nameArray[0].Trim(); // SM d48:2
    '    var totalChain = sublevelLipidName.Split(' ')[1]; // d48:2
    '    var lipidName = nameArray[1].Trim(); // SM d18:1/30:1
    '    var adductInfo = nameArray[2].Trim();

    '    if (totalChain == null || totalChain == string.Empty || !totalChain.Contains(':') || (totalChain.Contains('(') && !totalChain.Contains('+'))) return;

    '    var chainStrings = acylChainStringSeparator(lipidName);
    '    if (chainStrings.Count() != 2) return;

    '    var sn1AcylChainString = chainStrings[0];
    '    var sn2AcylChainString = chainStrings[1];

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;
    '    int sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount;
    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);
    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);
    '    setChainProperties(sn2AcylChainString, out sn2CarbonCount, out sn2DoubleBond, out sn2OxidizedCount);

    '    molecule.Mz = (float)query.PrecursorMz;
    '    molecule.Smiles = query.SMILES;
    '    molecule.InChIKey = query.InChIKey;
    '    molecule.Formula = query.Formula.FormulaString;
    '    molecule.Adduct = query.AdductType;
    '    molecule.IonMode = query.IonMode;

    '    molecule.SublevelLipidName = sublevelLipidName;
    '    molecule.LipidName = lipidName;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '    molecule.Sn2AcylChainString = sn2AcylChainString;
    '    molecule.Sn2CarbonCount = sn2CarbonCount;
    '    molecule.Sn2DoubleBondCount = sn2DoubleBond;
    '    molecule.Sn2Oxidizedount = sn2OxidizedCount;
    '}

    Private Sub setDiAcylChainProperty(molecule As LipidMolecule, lipidname As String, ontology As String, chainStrings As List(Of String)) ' e.g. SM 18:1;2O/30:1, PE 16:0_18:0;O, MGDG 2:0_2:0, ST 28:2;O;Hex;PA 12:0_12:0

        If chainStrings.Count <> 2 Then Return
        Dim lipidclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(ontology)
        Dim lipidHeader = GetLipidHeaderString(lipidname)

        Dim sn1AcylChainString = chainStrings(0)
        Dim sn2AcylChainString = chainStrings(1)

        Dim sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount As Integer
        Dim sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount As Integer
        setChainPropertiesVS2(sn1AcylChainString, sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount)
        setChainPropertiesVS2(sn2AcylChainString, sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount)

        Dim totalCarbonCount = sn1CarbonCount + sn2CarbonCount
        Dim totalDoubleBond = sn1DoubleBond + sn2DoubleBond
        Dim totalOxidizedCount = sn1OxidizedCount + sn2OxidizedCount
        Dim chainPrefix = If(sn1AcylChainString.StartsWith("O-"), "O-", If(sn1AcylChainString.StartsWith("P-"), "P-", String.Empty))
        Dim totalChain = getTotalChainString(totalCarbonCount, totalDoubleBond, totalOxidizedCount, lipidclass, chainPrefix, 2)
        Dim sublevelLipidName = lipidHeader & " " & totalChain

        molecule.SublevelLipidName = sublevelLipidName
        molecule.LipidName = lipidname
        molecule.TotalChainString = totalChain
        molecule.TotalCarbonCount = totalCarbonCount
        molecule.TotalDoubleBondCount = totalDoubleBond
        molecule.TotalOxidizedCount = totalOxidizedCount
        molecule.Sn1AcylChainString = sn1AcylChainString
        molecule.Sn1CarbonCount = sn1CarbonCount
        molecule.Sn1DoubleBondCount = sn1DoubleBond
        molecule.Sn1Oxidizedount = sn1OxidizedCount
        molecule.Sn2AcylChainString = sn2AcylChainString
        molecule.Sn2CarbonCount = sn2CarbonCount
        molecule.Sn2DoubleBondCount = sn2DoubleBond
        molecule.Sn2Oxidizedount = sn2OxidizedCount
    End Sub

    Private Function getTotalChainString(carbon As Integer, rdb As Integer, oxidized As Integer, lipidclass As LbmClass, chainPrefix As String, acylChainCount As Integer) As String
        Dim rdbString = rdb.ToString()

        If lipidclass = LbmClass.Cer_EODS OrElse lipidclass = LbmClass.Cer_EBDS OrElse lipidclass = LbmClass.ASM OrElse lipidclass = LbmClass.FAHFA OrElse lipidclass = LbmClass.NAGly OrElse lipidclass = LbmClass.NAGlySer OrElse lipidclass = LbmClass.NAGlySer OrElse lipidclass = LbmClass.TG_EST OrElse lipidclass = LbmClass.DMEDFAHFA Then
            rdbString = (rdb + 1).ToString()
            oxidized = oxidized + 1
        End If

        If lipidclass = LbmClass.Cer_EOS OrElse lipidclass = LbmClass.HexCer_EOS Then
            If acylChainCount <> 2 Then
                rdbString = (rdb + 1).ToString()
                oxidized = oxidized + 1
            End If
        End If

        If Equals(chainPrefix, "P-") Then
            rdbString = (rdb - 1).ToString()
        End If
        Dim oxString = If(oxidized = 0, String.Empty, If(oxidized = 1, ";O", ";O" & oxidized.ToString()))
        Return chainPrefix & carbon.ToString() & ":" & rdbString & oxString
    End Function

    'public static void setDoubleAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    string lipidname, string lipidclassString) {

    '    var lipidheader = lipidname.Split(' ')[0];
    '    var acylchains = lipidname.Split(' ')[1];
    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(lipidclassString);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(lipidclassString);
    '    var lipidName = lipidname; // SM d18:1/30:1
    '    var chainStrings = acylChainStringSeparator(lipidName);
    '    if (chainStrings.Count() != 2) return;

    '    var sn1AcylChainString = chainStrings[0];
    '    var sn2AcylChainString = chainStrings[1];

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;
    '    int sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount;
    '    string sn1Prefix, sn1Suffix;
    '    string sn2Prefix, sn2Suffix;

    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);
    '    setChainProperties(sn2AcylChainString, out sn2CarbonCount, out sn2DoubleBond, out sn2OxidizedCount);

    '    getPrefixSuffix(sn1AcylChainString, out sn1Prefix, out sn1Suffix);
    '    getPrefixSuffix(sn2AcylChainString, out sn2Prefix, out sn2Suffix);

    '    var oxtotal = sn1OxidizedCount + sn2OxidizedCount;
    '    var oxString = oxtotal == 0 ? string.Empty : "+" + oxtotal + "O";

    '    var totalChain = sn1Prefix + (sn1CarbonCount + sn2CarbonCount).ToString() + ":" +
    '        (sn1DoubleBond + sn2DoubleBond).ToString() + sn1Suffix + oxString; // d48:2
    '    var sublevelLipidName = lipidheader + " " + totalChain; // SM d48:2
    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);

    '    molecule.SublevelLipidName = sublevelLipidName;
    '    molecule.LipidName = lipidName;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '    molecule.Sn2AcylChainString = sn2AcylChainString;
    '    molecule.Sn2CarbonCount = sn2CarbonCount;
    '    molecule.Sn2DoubleBondCount = sn2DoubleBond;
    '    molecule.Sn2Oxidizedount = sn2OxidizedCount;
    '}

    'private static void setTripleAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    MoleculeMsReference query) {

    '    var nameArray = query.Name.Split(';').ToArray();
    '    if (nameArray.Length == 2) { // e.g. in positive PC Na adduct, only sublevel information is resigtered like PC 36:3; [M+Na]+
    '        setSingleAcylChainsLipidAnnotation(molecule, query);
    '        return;
    '    }

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(query.CompoundClass);
    '    var sublevelLipidName = nameArray[0].Trim(); // TAG 64:6
    '    var totalChain = sublevelLipidName.Split(' ')[1]; // 64:6
    '    var lipidName = nameArray[1].Trim(); // TAG 18:0-22:0-24:6
    '    var adductInfo = nameArray[2].Trim();

    '    var chainStrings = acylChainStringSeparator(lipidName);
    '    if (chainStrings.Count() == 2) {
    '        setDoubleAcylChainsLipidAnnotation(molecule, query);
    '        return;
    '    }
    '    if (chainStrings.Count() != 3) return;
    '    if (totalChain == null || totalChain == string.Empty || !totalChain.Contains(':') || (totalChain.Contains('(') && !totalChain.Contains('+'))) return;

    '    var sn1AcylChainString = chainStrings[0];
    '    var sn2AcylChainString = chainStrings[1];
    '    var sn3AcylChainString = chainStrings[2];

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;
    '    int sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount;
    '    int sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount;
    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);
    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);
    '    setChainProperties(sn2AcylChainString, out sn2CarbonCount, out sn2DoubleBond, out sn2OxidizedCount);
    '    setChainProperties(sn3AcylChainString, out sn3CarbonCount, out sn3DoubleBond, out sn3OxidizedCount);

    '    molecule.Mz = (float)query.PrecursorMz;
    '    molecule.Smiles = query.SMILES;
    '    molecule.InChIKey = query.InChIKey;
    '    molecule.Formula = query.Formula.FormulaString;
    '    molecule.Adduct = query.AdductType;
    '    molecule.IonMode = query.IonMode;

    '    molecule.SublevelLipidName = sublevelLipidName;
    '    molecule.LipidName = lipidName;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '    molecule.Sn2AcylChainString = sn2AcylChainString;
    '    molecule.Sn2CarbonCount = sn2CarbonCount;
    '    molecule.Sn2DoubleBondCount = sn2DoubleBond;
    '    molecule.Sn2Oxidizedount = sn2OxidizedCount;
    '    molecule.Sn3AcylChainString = sn3AcylChainString;
    '    molecule.Sn3CarbonCount = sn3CarbonCount;
    '    molecule.Sn3DoubleBondCount = sn3DoubleBond;
    '    molecule.Sn3Oxidizedount = sn3OxidizedCount;
    '}

    Private Sub setTriAcylChainProperty(molecule As LipidMolecule, lipidname As String, ontology As String, chainStrings As List(Of String))

        If chainStrings.Count <> 3 Then Return

        Dim lipidclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(ontology)
        Dim lipidHeader = GetLipidHeaderString(lipidname)

        Dim sn1AcylChainString = chainStrings(0)
        Dim sn2AcylChainString = chainStrings(1)
        Dim sn3AcylChainString = chainStrings(2)

        Dim sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount As Integer
        Dim sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount As Integer
        Dim sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount As Integer

        setChainPropertiesVS2(sn1AcylChainString, sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount)
        setChainPropertiesVS2(sn2AcylChainString, sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount)
        setChainPropertiesVS2(sn3AcylChainString, sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount)

        Dim totalCarbonCount = sn1CarbonCount + sn2CarbonCount + sn3CarbonCount
        Dim totalDoubleBond = sn1DoubleBond + sn2DoubleBond + sn3DoubleBond
        Dim totalOxidizedCount = sn1OxidizedCount + sn2OxidizedCount + sn3OxidizedCount
        Dim chainPrefix = String.Empty
        If lipidclass.ToString().Contains("Ether") Then
            chainPrefix = If(sn1AcylChainString.StartsWith("O-"), "O-", If(sn1AcylChainString.StartsWith("P-"), "P-", String.Empty))
        End If
        Dim totalChain = getTotalChainString(totalCarbonCount, totalDoubleBond, totalOxidizedCount, lipidclass, chainPrefix, 3)

        Dim sublevelLipidName = lipidHeader & " " & totalChain

        molecule.SublevelLipidName = sublevelLipidName
        molecule.LipidName = lipidname
        molecule.TotalChainString = totalChain
        molecule.TotalCarbonCount = totalCarbonCount
        molecule.TotalDoubleBondCount = totalDoubleBond
        molecule.TotalOxidizedCount = totalOxidizedCount
        molecule.Sn1AcylChainString = sn1AcylChainString
        molecule.Sn1CarbonCount = sn1CarbonCount
        molecule.Sn1DoubleBondCount = sn1DoubleBond
        molecule.Sn1Oxidizedount = sn1OxidizedCount
        molecule.Sn2AcylChainString = sn2AcylChainString
        molecule.Sn2CarbonCount = sn2CarbonCount
        molecule.Sn2DoubleBondCount = sn2DoubleBond
        molecule.Sn2Oxidizedount = sn2OxidizedCount
        molecule.Sn3AcylChainString = sn3AcylChainString
        molecule.Sn3CarbonCount = sn3CarbonCount
        molecule.Sn3DoubleBondCount = sn3DoubleBond
        molecule.Sn3Oxidizedount = sn3OxidizedCount
    End Sub

    Private Sub setTetraAcylChainProperty(molecule As LipidMolecule, lipidname As String, ontology As String, chainStrings As List(Of String))

        If chainStrings.Count <> 4 Then Return

        Dim lipidclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(ontology)
        Dim lipidHeader = GetLipidHeaderString(lipidname)

        Dim sn1AcylChainString = chainStrings(0)
        Dim sn2AcylChainString = chainStrings(1)
        Dim sn3AcylChainString = chainStrings(2)
        Dim sn4AcylChainString = chainStrings(3)

        Dim sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount As Integer
        Dim sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount As Integer
        Dim sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount As Integer
        Dim sn4CarbonCount, sn4DoubleBond, sn4OxidizedCount As Integer

        setChainPropertiesVS2(sn1AcylChainString, sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount)
        setChainPropertiesVS2(sn2AcylChainString, sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount)
        setChainPropertiesVS2(sn3AcylChainString, sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount)
        setChainPropertiesVS2(sn4AcylChainString, sn4CarbonCount, sn4DoubleBond, sn4OxidizedCount)

        Dim totalCarbonCount = sn1CarbonCount + sn2CarbonCount + sn3CarbonCount + sn4CarbonCount
        Dim totalDoubleBond = sn1DoubleBond + sn2DoubleBond + sn3DoubleBond + sn4DoubleBond
        Dim totalOxidizedCount = sn1OxidizedCount + sn2OxidizedCount + sn3OxidizedCount + sn4OxidizedCount
        Dim chainPrefix = If(sn1AcylChainString.StartsWith("O-"), "O-", If(sn1AcylChainString.StartsWith("P-"), "P-", String.Empty))
        Dim totalChain = getTotalChainString(totalCarbonCount, totalDoubleBond, totalOxidizedCount, lipidclass, chainPrefix, 4)
        Dim sublevelLipidName = lipidHeader & " " & totalChain

        molecule.SublevelLipidName = sublevelLipidName
        molecule.LipidName = lipidname
        molecule.TotalChainString = totalChain
        molecule.TotalCarbonCount = totalCarbonCount
        molecule.TotalDoubleBondCount = totalDoubleBond
        molecule.TotalOxidizedCount = totalOxidizedCount
        molecule.Sn1AcylChainString = sn1AcylChainString
        molecule.Sn1CarbonCount = sn1CarbonCount
        molecule.Sn1DoubleBondCount = sn1DoubleBond
        molecule.Sn1Oxidizedount = sn1OxidizedCount
        molecule.Sn2AcylChainString = sn2AcylChainString
        molecule.Sn2CarbonCount = sn2CarbonCount
        molecule.Sn2DoubleBondCount = sn2DoubleBond
        molecule.Sn2Oxidizedount = sn2OxidizedCount
        molecule.Sn3AcylChainString = sn3AcylChainString
        molecule.Sn3CarbonCount = sn3CarbonCount
        molecule.Sn3DoubleBondCount = sn3DoubleBond
        molecule.Sn3Oxidizedount = sn3OxidizedCount
        molecule.Sn4AcylChainString = sn4AcylChainString
        molecule.Sn4CarbonCount = sn4CarbonCount
        molecule.Sn4DoubleBondCount = sn4DoubleBond
        molecule.Sn4Oxidizedount = sn4OxidizedCount
    End Sub

    Public Function GetLipidHeaderString(lipidname As String) As String
        Dim lipidHeader = lipidname.Split(" "c)(0)
        If Equals(lipidHeader, "SE") OrElse Equals(lipidHeader, "ST") OrElse Equals(lipidHeader, "SG") OrElse Equals(lipidHeader, "BA") OrElse Equals(lipidHeader, "ASG") Then
            Dim dummyString = String.Empty
            RetrieveSterolHeaderChainStrings(lipidname, lipidHeader, dummyString)
        End If
        Return lipidHeader
    End Function

    'public static void setTripleAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    string lipidname, string lipidclassString) {

    '    var lipidheader = lipidname.Split(' ')[0];
    '    var acylchains = lipidname.Split(' ')[1];
    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(lipidclassString);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(lipidclassString);
    '    var lipidName = lipidname; // SM d18:1/30:1
    '    var chainStrings = acylChainStringSeparator(lipidName);
    '    if (chainStrings.Count() != 3) return;

    '    var sn1AcylChainString = chainStrings[0];
    '    var sn2AcylChainString = chainStrings[1];
    '    var sn3AcylChainString = chainStrings[2];

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;
    '    int sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount;
    '    int sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount;

    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);
    '    setChainProperties(sn2AcylChainString, out sn2CarbonCount, out sn2DoubleBond, out sn2OxidizedCount);
    '    setChainProperties(sn3AcylChainString, out sn3CarbonCount, out sn3DoubleBond, out sn3OxidizedCount);

    '    string sn1Prefix, sn1Suffix;
    '    string sn2Prefix, sn2Suffix;
    '    string sn3Prefix, sn3Suffix;

    '    getPrefixSuffix(sn1AcylChainString, out sn1Prefix, out sn1Suffix);
    '    getPrefixSuffix(sn2AcylChainString, out sn2Prefix, out sn2Suffix);
    '    getPrefixSuffix(sn3AcylChainString, out sn3Prefix, out sn3Suffix);

    '    var oxtotal = sn1OxidizedCount + sn2OxidizedCount + sn3OxidizedCount;
    '    var oxString = oxtotal == 0 ? string.Empty : "+" + oxtotal + "O";
    '    var totalChain = sn1Prefix + (sn1CarbonCount + sn2CarbonCount + sn3CarbonCount).ToString() + ":" +
    '        (sn1DoubleBond + sn2DoubleBond + sn3DoubleBond).ToString() + sn1Suffix + oxString; // d48:2
    '    var sublevelLipidName = lipidheader + " " + totalChain; // SM d48:2
    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);

    '    molecule.SublevelLipidName = sublevelLipidName;
    '    molecule.LipidName = lipidName;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '    molecule.Sn2AcylChainString = sn2AcylChainString;
    '    molecule.Sn2CarbonCount = sn2CarbonCount;
    '    molecule.Sn2DoubleBondCount = sn2DoubleBond;
    '    molecule.Sn2Oxidizedount = sn2OxidizedCount;
    '    molecule.Sn3AcylChainString = sn3AcylChainString;
    '    molecule.Sn3CarbonCount = sn3CarbonCount;
    '    molecule.Sn3DoubleBondCount = sn3DoubleBond;
    '    molecule.Sn3Oxidizedount = sn3OxidizedCount;
    '}

    'private static void setQuadAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    MoleculeMsReference query) {

    '    var nameArray = query.Name.Split(';').ToArray();
    '    if (nameArray.Length == 2) { // e.g. in positive PC Na adduct, only sublevel information is resigtered like PC 36:3; [M+Na]+
    '        setSingleAcylChainsLipidAnnotation(molecule, query);
    '        return;
    '    }

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(query.CompoundClass);
    '    var sublevelLipidName = nameArray[0].Trim(); // TAG 64:6
    '    var totalChain = sublevelLipidName.Split(' ')[1]; // 64:6
    '    var lipidName = nameArray[1].Trim(); // TAG 18:0-22:0-24:6
    '    var adductInfo = nameArray[2].Trim();

    '    var chainStrings = acylChainStringSeparator(lipidName);
    '    if (chainStrings.Count() == 2) { //e.g. in positive CL, the chains are defined as double acyls
    '        setDoubleAcylChainsLipidAnnotation(molecule, query);
    '        return;
    '    }
    '    if (chainStrings.Count() != 4) return;
    '    if (totalChain == null || totalChain == string.Empty || !totalChain.Contains(':') || (totalChain.Contains('(') && !totalChain.Contains('+'))) return;

    '    var sn1AcylChainString = chainStrings[0];
    '    var sn2AcylChainString = chainStrings[1];
    '    var sn3AcylChainString = chainStrings[2];
    '    var sn4AcylChainString = chainStrings[3];

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;
    '    int sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount;
    '    int sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount;
    '    int sn4CarbonCount, sn4DoubleBond, sn4OxidizedCount;

    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);
    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);
    '    setChainProperties(sn2AcylChainString, out sn2CarbonCount, out sn2DoubleBond, out sn2OxidizedCount);
    '    setChainProperties(sn3AcylChainString, out sn3CarbonCount, out sn3DoubleBond, out sn3OxidizedCount);
    '    setChainProperties(sn4AcylChainString, out sn4CarbonCount, out sn4DoubleBond, out sn4OxidizedCount);

    '    molecule.Mz = (float)query.PrecursorMz;
    '    molecule.Smiles = query.SMILES;
    '    molecule.InChIKey = query.InChIKey;
    '    molecule.Formula = query.Formula.FormulaString;
    '    molecule.Adduct = query.AdductType;
    '    molecule.IonMode = query.IonMode;

    '    molecule.SublevelLipidName = sublevelLipidName;
    '    molecule.LipidName = lipidName;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '    molecule.Sn2AcylChainString = sn2AcylChainString;
    '    molecule.Sn2CarbonCount = sn2CarbonCount;
    '    molecule.Sn2DoubleBondCount = sn2DoubleBond;
    '    molecule.Sn2Oxidizedount = sn2OxidizedCount;
    '    molecule.Sn3AcylChainString = sn3AcylChainString;
    '    molecule.Sn3CarbonCount = sn3CarbonCount;
    '    molecule.Sn3DoubleBondCount = sn3DoubleBond;
    '    molecule.Sn3Oxidizedount = sn3OxidizedCount;
    '    molecule.Sn4AcylChainString = sn4AcylChainString;
    '    molecule.Sn4CarbonCount = sn4CarbonCount;
    '    molecule.Sn4DoubleBondCount = sn4DoubleBond;
    '    molecule.Sn4Oxidizedount = sn4OxidizedCount;
    '}

    'public static void setQuadAcylChainsLipidAnnotation(LipidMolecule molecule,
    '    string lipidname, string lipidclassString) {

    '    var lipidheader = lipidname.Split(' ')[0];
    '    var acylchains = lipidname.Split(' ')[1];
    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(lipidclassString);
    '    var lipidclass = ConvertMsdialClassDefinitionToLbmClassEnum(lipidclassString);
    '    var lipidName = lipidname; // SM d18:1/30:1
    '    var chainStrings = acylChainStringSeparator(lipidName);
    '    if (chainStrings.Count() != 4) return;

    '    var sn1AcylChainString = chainStrings[0];
    '    var sn2AcylChainString = chainStrings[1];
    '    var sn3AcylChainString = chainStrings[2];
    '    var sn4AcylChainString = chainStrings[3];

    '    int totalCarbonCount, totalDoubleBond, totalOxidizedCount;
    '    int sn1CarbonCount, sn1DoubleBond, sn1OxidizedCount;
    '    int sn2CarbonCount, sn2DoubleBond, sn2OxidizedCount;
    '    int sn3CarbonCount, sn3DoubleBond, sn3OxidizedCount;
    '    int sn4CarbonCount, sn4DoubleBond, sn4OxidizedCount;

    '    setChainProperties(sn1AcylChainString, out sn1CarbonCount, out sn1DoubleBond, out sn1OxidizedCount);
    '    setChainProperties(sn2AcylChainString, out sn2CarbonCount, out sn2DoubleBond, out sn2OxidizedCount);
    '    setChainProperties(sn3AcylChainString, out sn3CarbonCount, out sn3DoubleBond, out sn3OxidizedCount);
    '    setChainProperties(sn4AcylChainString, out sn4CarbonCount, out sn4DoubleBond, out sn4OxidizedCount);

    '    string sn1Prefix, sn1Suffix;
    '    string sn2Prefix, sn2Suffix;
    '    string sn3Prefix, sn3Suffix;
    '    string sn4Prefix, sn4Suffix;

    '    getPrefixSuffix(sn1AcylChainString, out sn1Prefix, out sn1Suffix);
    '    getPrefixSuffix(sn2AcylChainString, out sn2Prefix, out sn2Suffix);
    '    getPrefixSuffix(sn3AcylChainString, out sn3Prefix, out sn3Suffix);
    '    getPrefixSuffix(sn4AcylChainString, out sn4Prefix, out sn4Suffix);

    '    var oxtotal = sn1OxidizedCount + sn2OxidizedCount + sn3OxidizedCount + sn4OxidizedCount;
    '    var oxString = oxtotal == 0 ? string.Empty : "+" + oxtotal + "O";
    '    var totalChain = sn1Prefix + (sn1CarbonCount + sn2CarbonCount + sn3CarbonCount + sn4CarbonCount).ToString() + ":" +
    '        (sn1DoubleBond + sn2DoubleBond + sn3DoubleBond + sn4DoubleBond).ToString() + sn1Suffix + oxString; // d48:2
    '    var sublevelLipidName = lipidheader + " " + totalChain; // SM d48:2
    '    setChainProperties(totalChain, out totalCarbonCount, out totalDoubleBond, out totalOxidizedCount);

    '    molecule.SublevelLipidName = sublevelLipidName;
    '    molecule.LipidName = lipidName;
    '    molecule.LipidClass = lipidclass;
    '    molecule.TotalChainString = totalChain;
    '    molecule.TotalCarbonCount = totalCarbonCount;
    '    molecule.TotalDoubleBondCount = totalDoubleBond;
    '    molecule.TotalOxidizedCount = totalOxidizedCount;
    '    molecule.Sn1AcylChainString = sn1AcylChainString;
    '    molecule.Sn1CarbonCount = sn1CarbonCount;
    '    molecule.Sn1DoubleBondCount = sn1DoubleBond;
    '    molecule.Sn1Oxidizedount = sn1OxidizedCount;
    '    molecule.Sn2AcylChainString = sn2AcylChainString;
    '    molecule.Sn2CarbonCount = sn2CarbonCount;
    '    molecule.Sn2DoubleBondCount = sn2DoubleBond;
    '    molecule.Sn2Oxidizedount = sn2OxidizedCount;
    '    molecule.Sn3AcylChainString = sn3AcylChainString;
    '    molecule.Sn3CarbonCount = sn3CarbonCount;
    '    molecule.Sn3DoubleBondCount = sn3DoubleBond;
    '    molecule.Sn3Oxidizedount = sn3OxidizedCount;
    '    molecule.Sn4AcylChainString = sn4AcylChainString;
    '    molecule.Sn4CarbonCount = sn4CarbonCount;
    '    molecule.Sn4DoubleBondCount = sn4DoubleBond;
    '    molecule.Sn4Oxidizedount = sn4OxidizedCount;
    '}

    Private Sub setChainPropertiesVS2(chainString As String, <Out> ByRef carbonCount As Integer, <Out> ByRef doubleBondCount As Integer, <Out> ByRef oxidizedCount As Integer)

        carbonCount = 0
        doubleBondCount = 0
        oxidizedCount = 0

        'pattern: 18:1, 18:1e, 18:1p d18:1, t20:0, n-18:0, N-19:0, 20:3+3O, 18:2-SN1, O-18:1, P-18:1, N-18:1, 16:0;O, 18:2;2O, 18:2;(2OH)
        'try convertion
        Dim isPlasmenyl = If(chainString.Contains("P-"), True, False)
        chainString = chainString.Replace("O-", "").Replace("P-", "").Replace("N-", "").Replace("e", "").Replace("p", "").Replace("m", "").Replace("n-", "").Replace("d", "").Replace("t", "")

        'remove double bond position  add 2023/06/02
        chainString = Regex.Replace(chainString, "\([\d+?(E|Z)*,*]+\)", String.Empty)

        ' for oxidized moiety parser
        If chainString.Contains(";") Then ' e.g. 18:2;2O, 18:2;(2OH)
            Dim chain = chainString.Split(";"c)(0)
            Dim oxidizedmoiety = chainString.Split(";"c)(1) '2O, O2
            'modified by MT 2020/12/11 & 2021/01/12
            Dim expectedOxCount = oxidizedmoiety.Replace("O", String.Empty).Replace("H", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty)
            If Equals(expectedOxCount, String.Empty) OrElse Equals(expectedOxCount, "") Then
                expectedOxCount = "1"
            ElseIf oxidizedmoiety.Contains("(2OH)") OrElse oxidizedmoiety.Contains("(3OH)") Then
                expectedOxCount = "1"
            End If
            Integer.TryParse(expectedOxCount, oxidizedCount)
            chainString = chain
        ElseIf chainString.Contains("+") Then '20:3+3O
            Dim chain = chainString.Split("+"c)(0) ' 20:3
            Dim expectedOxCount = chainString.Split("+"c)(1).Replace("O", "") '3
            If Equals(expectedOxCount, String.Empty) OrElse Equals(expectedOxCount, "") Then
                expectedOxCount = "1"
            End If
            Integer.TryParse(expectedOxCount, oxidizedCount)
            chainString = chain
        ElseIf chainString.Contains("(") Then ' e.g. 18:1(1OH,3OH), 18:2(2OH)
            Dim chain = chainString.Split("("c)(0)
            Dim oxidizedmoiety = chainString.Split("("c)(1).Replace(")", String.Empty) '2OH 
            'modified by MT 2020/12/11 & 2021/01/12
            'var expectedOxCount = oxidizedmoiety.Replace("O", string.Empty).Replace("H", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
            'if (expectedOxCount == string.Empty || expectedOxCount == "")
            '{
            '    expectedOxCount = "1";
            '}
            'else if (oxidizedmoiety.Contains("2OH)") || oxidizedmoiety.Contains("3OH)"))
            '{
            '    expectedOxCount = "1";
            '}
            'int.TryParse(expectedOxCount, out oxidizedCount);
            oxidizedCount = oxidizedmoiety.Split(","c).Length

            chainString = chain
        End If


        ' for SN1/SN2 string parser 
        If chainString.Contains("-SN") Then
            If chainString.Contains("-SN1") Then
                chainString = chainString.Replace("-SN1", "")
            ElseIf chainString.Contains("-SN2") Then
                chainString = chainString.Replace("-SN2", "")
            End If
        End If

        ' here all string expected as (carbon):(double)
        Dim carbon = chainString.Split(":"c)(0)
        Dim doublebond = chainString.Split(":"c)(1)
        Integer.TryParse(carbon, carbonCount)
        Integer.TryParse(doublebond, doubleBondCount)
        If isPlasmenyl Then doubleBondCount += 1
    End Sub

    'private static void setChainProperties(string chainString, out int carbonCount, out int doubleBondCount, out int oxidizedCount) {

    '    carbonCount = -1;
    '    doubleBondCount = -1;
    '    oxidizedCount = -1;

    '    //pattern: 18:1, 18:1e, 18:1p d18:1, t20:0, n-18:0, N-19:0, 20:3+3O, 20:3+2O(2Cyc), 18:2-SN1, O-18:1, P-18:1, N-18:1, 16:0;O, 18:2;2O, 18:2;(2OH)

    '    if (chainString.Contains("e") || chainString.Contains("p") || chainString.Contains("m") ||
    '        chainString.StartsWith("d") || chainString.StartsWith("t") || chainString.StartsWith("n-") || chainString.StartsWith("N-") ||
    '        chainString.StartsWith("O-") || chainString.StartsWith("P-")) {
    '        chainString = chainString.Replace("O-","").Replace("P-", "").Replace("N-", "").Replace("e", "").Replace("p", "").Replace("m", "").Replace("n-", "").Replace("d", "").Replace("t", "");

    '        var carbon = chainString.Split(':')[0];
    '        var doublebond = chainString.Split(':')[1];
    '        int.TryParse(carbon, out carbonCount);
    '        int.TryParse(doublebond, out doubleBondCount);
    '        oxidizedCount = 0;
    '    }

    '    if (chainString.Contains("-SN1") || chainString.Contains("-SN2")) {
    '        if (chainString.Contains("-SN1")) {
    '            chainString = chainString.Replace("-SN1", "");
    '        } else if (chainString.Contains("-SN2")) {
    '            chainString = chainString.Replace("-SN2", "");
    '        }
    '        var carbon = chainString.Split(':')[0];
    '        var doublebond = chainString.Split(':')[1];
    '        int.TryParse(carbon, out carbonCount);
    '        int.TryParse(doublebond, out doubleBondCount);
    '        oxidizedCount = 0;
    '    }

    '    if (chainString.Contains("+") && chainString.Contains("(")) { // it means 20:3+2O(2Cyc) case
    '        var separatedArray = chainString.Replace("Cyc)", "").Split('(');
    '        if (separatedArray.Length == 2) {
    '            int cycleCount;
    '            if (int.TryParse(separatedArray[1], out cycleCount)) {
    '                var oxAcylString = separatedArray[0]; // should be 20:3+2O
    '                var acylString = oxAcylString.Split('+')[0]; // 20:3
    '                var carbonString = acylString.Split(':')[0]; //20
    '                var doublebondString = acylString.Split(':')[1]; //3
    '                var oxCountString = oxAcylString.Split('+')[1].Replace("O", ""); //2

    '                if (oxCountString == string.Empty || oxCountString == "") {
    '                    oxCountString = "1";
    '                }

    '                int.TryParse(carbonString, out carbonCount);
    '                int.TryParse(oxCountString, out oxidizedCount);
    '                int.TryParse(doublebondString, out doubleBondCount);
    '                doubleBondCount += cycleCount;
    '            }
    '        }
    '    }
    '    else if (chainString.Contains("+")) { // it means 20:3+2O case
    '        var acylString = chainString.Split('+')[0]; // 20:3
    '        var carbonString = acylString.Split(':')[0]; //20
    '        var doublebondString = acylString.Split(':')[1]; //3
    '        var oxCountString = chainString.Split('+')[1].Replace("O", ""); //2
    '        if (oxCountString == string.Empty || oxCountString == "") {
    '            oxCountString = "1";
    '        }

    '        int.TryParse(carbonString, out carbonCount);
    '        int.TryParse(oxCountString, out oxidizedCount);
    '        int.TryParse(doublebondString, out doubleBondCount);
    '    }
    '    else {
    '        var carbon = chainString.Split(':')[0];
    '        var doublebond = chainString.Split(':')[1];
    '        int.TryParse(carbon, out carbonCount);
    '        int.TryParse(doublebond, out doubleBondCount);
    '        oxidizedCount = 0;
    '    }
    '}

    'private static void getPrefixSuffix(string chainString, out string prefix, out string suffix) {

    '    prefix = string.Empty;
    '    suffix = string.Empty;

    '    if (chainString.Contains("e")) {
    '        suffix = "e";
    '    }
    '    else if (chainString.Contains("p")) {
    '        suffix = "p";
    '    }

    '    if (chainString.Contains("m")) {
    '        prefix = "m";
    '    }
    '    else if (chainString.Contains("d")) {
    '        prefix = "d";
    '    }
    '    else if (chainString.Contains("t")) {
    '        prefix = "t";
    '    }
    '    else if (chainString.Contains("n-")) {
    '        prefix = "n-";
    '    }
    '}


    ' this method is not capable of oxidized form.
    'private static List<string> acylChainStringSeparator(string moleculeString) {
    '    var chains = new List<string>();
    '    string[] acylArray = null;

    '    if (moleculeString.Contains("-O-")) {
    '        var cMolString = moleculeString.Replace("-O-", "-");
    '        if (cMolString.Contains("/")) {
    '            cMolString = cMolString.Replace("/", "-");
    '        }

    '        acylArray = cMolString.Split(' ')[1].Split('-');
    '    }
    '    else if (moleculeString.Contains("(") && !moleculeString.Contains("Cyc")) {

    '        if (moleculeString.Contains("/")) {
    '            acylArray = moleculeString.Split('(')[1].Split(')')[0].Split('/');
    '        }
    '        else {
    '            acylArray = moleculeString.Split('(')[1].Split(')')[0].Split('-');
    '        }
    '    }
    '    else if (moleculeString.Contains("/n-")) {
    '        var cMolString = moleculeString.Replace("/n-", "-");
    '        if (cMolString.Contains("/")) {
    '            cMolString = cMolString.Replace("/", "-");
    '        }

    '        acylArray = cMolString.Split(' ')[1].Split('-');
    '    }
    '    else {
    '        var cMolString = moleculeString;
    '        if (cMolString.Contains("/")) {
    '            cMolString = cMolString.Replace("/", "-");
    '        }
    '        acylArray = cMolString.Split(' ')[1].Split('-');
    '    }

    '    for (int i = 0; i < acylArray.Length; i++) {
    '        if (i == 0 && acylArray[i] != string.Empty) chains.Add(acylArray[i]);
    '        if (i == 1 && acylArray[i] != string.Empty) chains.Add(acylArray[i]);
    '        if (i == 2 && acylArray[i] != string.Empty) chains.Add(acylArray[i]);
    '        if (i == 3 && acylArray[i] != string.Empty) chains.Add(acylArray[i]);
    '    }
    '    return chains;
    '}

    Private Function acylChainStringSeparatorVS2(moleculeString As String) As List(Of String)

        If moleculeString.Split(" "c).Length = 1 Then Return Nothing

        ' pattern [1] ADGGA 12:0_12:0_12:0
        ' pattern [2] AHexCer (O-14:0)16:1;2O/14:0;O , ADGGA (O-24:0)17:2_22:6  
        ' pattern [3] SM 30:1;2O(FA 14:0)
        ' pattern [4] Cer 14:0;2O/12:0;(3OH)(FA 12:0) -> [0]14:0;2O, [1]12:0;(3OH), [3]12:0
        ' pattern [5] Cer 14:1;2O/12:0;(2OH)
        ' pattern [6] DGDG O-8:0_2:0
        ' pattern [7] LNAPS 2:0/N-2:0 
        ' pattern [8] SM 30:1;2O(FA 14:0)
        ' pattern [9] ST 28:2;O;Hex;PA 12:0_12:0
        ' pattern [10] SE 28:2/8:0
        ' pattern [11] TG 16:0_16:1_18:0;O(FA 16:0)
        ' pattern [12]  LPE-N (FA 16:0)18:1  (LNAPE,LNAPS)
        Dim headerString = moleculeString.Split(" "c)(0).Trim()
        Dim chainString = String.Empty
        If Equals(headerString, "SE") OrElse Equals(headerString, "ST") OrElse Equals(headerString, "SG") OrElse Equals(headerString, "BA") OrElse Equals(headerString, "ASG") Then
            RetrieveSterolHeaderChainStrings(moleculeString, headerString, chainString)
        Else
            chainString = moleculeString.Substring(headerString.Length + 1)
        End If
        Dim chains As List(Of String) = Nothing
        Dim acylArray As String() = Nothing

        ' d-substituted compound support 20220920
        If chainString.Contains("|"c) Then Return Nothing
        Dim reg As Regex = New Regex("\(d([0-9]*)\)")
        chainString = reg.Replace(chainString, "")

        Dim pattern2 = "(\()(?<chain1>.+?)(\))(?<chain2>.+?)([/_])(?<chain3>.+?$)"
        Dim pattern3 = "(?<chain1>.+?)(\(FA )(?<chain2>.+?)(\))"
        Dim pattern4 = "(?<chain1>.+?)(/)(?<chain2>.+?)(\(FA )(?<chain3>.+?)(\))"
        Dim pattern12 = "(\(FA )(?<chain2>.+?)(\))(?<chain1>.+?$)"

        If chainString.Contains("/") AndAlso chainString.Contains("(FA") Then ' pattern 4
            Dim regexes = Regex.Match(chainString, pattern4).Groups
            chains = New List(Of String)() From {
                regexes("chain1").Value,
                regexes("chain2").Value,
                regexes("chain3").Value
            }
        ElseIf chainString.Contains("(FA") Then  ' pattern 3
            Dim regexes = Regex.Match(chainString, pattern3).Groups
            Dim chain1strings = regexes("chain1").Value
            If chain1strings.Contains("_") Then
                chains = New List(Of String)()
                For Each chainString In chain1strings.Split("_"c).ToArray()
                    chains.Add(chainString)
                Next
                chains.Add(regexes("chain2").Value)
            ElseIf Equals(chain1strings, "") Then ' pattern 12
                regexes = Regex.Match(chainString, pattern12).Groups
                'Console.WriteLine();
                chains = New List(Of String)() From {
                        regexes("chain1").Value,
                        regexes("chain2").Value
                    }
            Else
                chains = New List(Of String)() From {
                    regexes("chain1").Value,
                    regexes("chain2").Value
                }
                'Console.WriteLine();
            End If
        ElseIf chainString.Contains("(O-") AndAlso Regex.IsMatch(chainString, "[/_]") Then ' pattern 2
            Dim regexes = Regex.Match(chainString, pattern2).Groups
            'Console.WriteLine();
            chains = New List(Of String)() From {
                    regexes("chain1").Value,
                    regexes("chain2").Value,
                    regexes("chain3").Value
                }
        Else
            chainString = chainString.Replace("/"c, "_"c)
            acylArray = chainString.Split("_"c)
            chains = New List(Of String)()
            For i = 0 To acylArray.Length - 1
                If i = 0 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
                If i = 1 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
                If i = 2 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
                If i = 3 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
            Next
        End If

        Return chains
    End Function

    Public Sub RetrieveSterolHeaderChainStrings(moleculeString As String, <Out> ByRef headerString As String, <Out> ByRef chainString As String)

        headerString = String.Empty
        chainString = String.Empty
        If moleculeString.Contains("/") Then
            Dim splitterArray = moleculeString.Split("/"c)
            chainString = splitterArray(splitterArray.Length - 1)
        Else
            Dim splitterArray = moleculeString.Split(" "c)
            chainString = splitterArray(splitterArray.Length - 1)
        End If
    End Sub

    Public Function GetLipidClasses() As List(Of String)
        Dim names = New List(Of String)()
        For Each lipid In System.Enum.GetValues(GetType(LbmClass)).Cast(Of LbmClass)()
            Dim cName = ConvertLbmClassEnumToMsdialClassDefinitionVS2(lipid)
            If Not Equals(cName, "Undefined") AndAlso Not names.Contains(cName) Then names.Add(cName)
        Next
        Return names
    End Function

    'public static string ConvertLbmClassEnumToMsdialClassDefinition(LbmClass lipidclass) {
    '    switch (lipidclass) {
    '        case LbmClass.MG: return "MAG";
    '        case LbmClass.DG: return "DAG";
    '        case LbmClass.TG: return "TAG";
    '        case LbmClass.EtherTG: return "EtherTAG";
    '        case LbmClass.EtherDG: return "EtherDAG";
    '        case LbmClass.LPC: return "LPC";
    '        case LbmClass.LPA: return "LPA";
    '        case LbmClass.LPE: return "LPE";
    '        case LbmClass.LPG: return "LPG";
    '        case LbmClass.LPI: return "LPI";
    '        case LbmClass.LPS: return "LPS";
    '        case LbmClass.LDGTS: return "LDGTS";
    '        case LbmClass.LDGCC: return "LDGCC";
    '        case LbmClass.PC: return "PC";
    '        case LbmClass.PA: return "PA";
    '        case LbmClass.PE: return "PE";
    '        case LbmClass.PG: return "PG";
    '        case LbmClass.PI: return "PI";
    '        case LbmClass.PS: return "PS";
    '        case LbmClass.BMP: return "BMP";
    '        case LbmClass.HBMP: return "HBMP";
    '        case LbmClass.CL: return "CL";
    '        case LbmClass.DLCL: return "DLCL";
    '        case LbmClass.MLCL: return "LCL";
    '        case LbmClass.EtherPC: return "EtherPC";
    '        case LbmClass.EtherPE: return "EtherPE";
    '        case LbmClass.EtherPS: return "EtherPS";
    '        case LbmClass.EtherPI: return "EtherPI";
    '        case LbmClass.EtherPG: return "EtherPG";
    '        case LbmClass.EtherLPC: return "EtherLPC";
    '        case LbmClass.EtherLPE: return "EtherLPE";
    '        case LbmClass.EtherLPS: return "EtherLPS";
    '        case LbmClass.EtherLPI: return "EtherLPI";
    '        case LbmClass.EtherLPG: return "EtherLPG";
    '        case LbmClass.OxPC: return "OxPC";
    '        case LbmClass.OxPE: return "OxPE";
    '        case LbmClass.OxPG: return "OxPG";
    '        case LbmClass.OxPI: return "OxPI";
    '        case LbmClass.OxPS: return "OxPS";
    '        case LbmClass.EtherOxPC: return "EtherOxPC";
    '        case LbmClass.EtherOxPE: return "EtherOxPE";
    '        case LbmClass.PMeOH: return "PMeOH";
    '        case LbmClass.PEtOH: return "PEtOH";
    '        case LbmClass.PBtOH: return "PBtOH";
    '        case LbmClass.LNAPE: return "LNAPE";
    '        case LbmClass.LNAPS: return "LNAPS";
    '        case LbmClass.DGDG: return "DGDG";
    '        case LbmClass.MGDG: return "MGDG";
    '        case LbmClass.SQDG: return "SQDG";
    '        case LbmClass.DGTS: return "DGTS";
    '        case LbmClass.DGCC: return "DGCC";
    '        case LbmClass.DGGA: return "DGGA";
    '        case LbmClass.ADGGA: return "ADGGA";
    '        case LbmClass.EtherMGDG: return "EtherMGDG";
    '        case LbmClass.EtherDGDG: return "EtherDGDG";
    '        case LbmClass.CE: return "CE";
    '        case LbmClass.BRSE: return "BRSE";
    '        case LbmClass.CASE: return "CASE";
    '        case LbmClass.SISE: return "SISE";
    '        case LbmClass.STSE: return "STSE";
    '        case LbmClass.AHexCS: return "AHexCS";
    '        case LbmClass.AHexBRS: return "AHexBRS";
    '        case LbmClass.AHexCAS: return "AHexCAS";
    '        case LbmClass.AHexSIS: return "AHexSIS";
    '        case LbmClass.AHexSTS: return "AHexSTS";
    '        case LbmClass.DCAE: return "DCAE";
    '        case LbmClass.GDCAE: return "GDCAE";
    '        case LbmClass.GLCAE: return "GLCAE";
    '        case LbmClass.TDCAE: return "TDCAE";
    '        case LbmClass.TLCAE: return "TLCAE";
    '        case LbmClass.SHex: return "SHex";
    '        case LbmClass.SSulfate: return "SSulfate";
    '        case LbmClass.BAHex: return "BAHex";
    '        case LbmClass.BASulfate: return "BASulfate";
    '        case LbmClass.Vitamin_E: return "Vitamin";
    '        case LbmClass.VAE: return "VAE";
    '        case LbmClass.BileAcid: return "BileAcid";
    '        case LbmClass.CoQ: return "CoQ";
    '        case LbmClass.CAR: return "ACar";
    '        case LbmClass.FA: return "FA";
    '        case LbmClass.NAE: return "NAE";
    '        case LbmClass.NAGly: return "NAAG";
    '        case LbmClass.NAGlySer: return "NAAGS";
    '        case LbmClass.NAOrn: return "NAAO";
    '        case LbmClass.FAHFA: return "FAHFA";
    '        case LbmClass.PhytoSph: return "Phytosphingosine";
    '        case LbmClass.DHSph: return "Sphinganine";
    '        case LbmClass.Sph: return "Sphingosine";
    '        case LbmClass.Cer_ADS: return "Cer-ADS";
    '        case LbmClass.Cer_AS: return "Cer-AS";
    '        case LbmClass.Cer_BS: return "Cer-BS";
    '        case LbmClass.Cer_BDS: return "Cer-BDS";
    '        case LbmClass.Cer_NDS: return "Cer-NDS";
    '        case LbmClass.Cer_NS: return "Cer-NS";
    '        case LbmClass.Cer_NP: return "Cer-NP";
    '        case LbmClass.Cer_AP: return "Cer-AP";
    '        case LbmClass.Cer_EODS: return "Cer-EODS";
    '        case LbmClass.Cer_EOS: return "Cer-EOS";
    '        case LbmClass.Cer_OS: return "Cer-OS";
    '        case LbmClass.Cer_HS: return "Cer-HS";
    '        case LbmClass.Cer_HDS: return "Cer-HDS";
    '        case LbmClass.Cer_NDOS: return "Cer-NDOS";
    '        case LbmClass.HexCer_NS: return "HexCer-NS";
    '        case LbmClass.HexCer_NDS: return "HexCer-NDS";
    '        case LbmClass.HexCer_AP: return "HexCer-AP";
    '        case LbmClass.HexCer_HS: return "HexCer-HS";
    '        case LbmClass.HexCer_HDS: return "HexCer-HDS";
    '        case LbmClass.Hex2Cer: return "Hex2Cer";
    '        case LbmClass.Hex3Cer: return "Hex3Cer";
    '        case LbmClass.PE_Cer: return "PE-Cer";
    '        case LbmClass.PI_Cer: return "PI-Cer";
    '        case LbmClass.CerP: return "CerP";
    '        case LbmClass.SM: return "SM";
    '        case LbmClass.SHexCer: return "SHexCer";
    '        case LbmClass.SL: return "SL";
    '        case LbmClass.GM3: return "GM3";
    '        case LbmClass.Ac2PIM1: return "Ac2PIM1";
    '        case LbmClass.Ac2PIM2: return "Ac2PIM2";
    '        case LbmClass.Ac3PIM2: return "Ac3PIM2";
    '        case LbmClass.Ac4PIM2: return "Ac4PIM2";
    '        case LbmClass.Cer_EBDS: return "Cer-EBDS";
    '        case LbmClass.AHexCer: return "AHexCer";
    '        case LbmClass.ASM: return "ASM";

    '        default: return "Undefined";
    '    }
    '}

    Public Function ConvertLbmClassEnumToMsdialClassDefinitionVS2(lipidclass As LbmClass) As String
        Select Case lipidclass
            Case LbmClass.MG
                Return "MG"
            Case LbmClass.DG
                Return "DG"
            Case LbmClass.TG
                Return "TG"
            Case LbmClass.OxTG
                Return "OxTG"
            Case LbmClass.TG_EST
                Return "TG_EST"
            Case LbmClass.EtherTG
                Return "EtherTG"
            Case LbmClass.EtherDG
                Return "EtherDG"
            Case LbmClass.LPC
                Return "LPC"
            Case LbmClass.LPA
                Return "LPA"
            Case LbmClass.LPE
                Return "LPE"
            Case LbmClass.LPG
                Return "LPG"
            Case LbmClass.LPI
                Return "LPI"
            Case LbmClass.LPS
                Return "LPS"
            Case LbmClass.LDGTS
                Return "LDGTS"
            Case LbmClass.LDGTA
                Return "LDGTA"
            Case LbmClass.LDGCC
                Return "LDGCC"
            Case LbmClass.PC
                Return "PC"
            Case LbmClass.PA
                Return "PA"
            Case LbmClass.PE
                Return "PE"
            Case LbmClass.PG
                Return "PG"
            Case LbmClass.PI
                Return "PI"
            Case LbmClass.PS
                Return "PS"
            Case LbmClass.PT
                Return "PT"
            Case LbmClass.BMP
                Return "BMP"
            Case LbmClass.HBMP
                Return "HBMP"
            Case LbmClass.CL
                Return "CL"
            Case LbmClass.DLCL
                Return "DLCL"
            Case LbmClass.MLCL
                Return "MLCL"
            Case LbmClass.EtherPC
                Return "EtherPC"
            Case LbmClass.EtherPE
                Return "EtherPE"
            Case LbmClass.EtherPS
                Return "EtherPS"
            Case LbmClass.EtherPI
                Return "EtherPI"
            Case LbmClass.EtherPG
                Return "EtherPG"
            Case LbmClass.EtherLPC
                Return "EtherLPC"
            Case LbmClass.EtherLPE
                Return "EtherLPE"
            Case LbmClass.EtherLPS
                Return "EtherLPS"
            Case LbmClass.EtherLPI
                Return "EtherLPI"
            Case LbmClass.EtherLPG
                Return "EtherLPG"
            Case LbmClass.OxPC
                Return "OxPC"
            Case LbmClass.OxPE
                Return "OxPE"
            Case LbmClass.OxPG
                Return "OxPG"
            Case LbmClass.OxPI
                Return "OxPI"
            Case LbmClass.OxPS
                Return "OxPS"
            Case LbmClass.EtherOxPC
                Return "EtherOxPC"
            Case LbmClass.EtherOxPE
                Return "EtherOxPE"
            Case LbmClass.PMeOH
                Return "PMeOH"
            Case LbmClass.PEtOH
                Return "PEtOH"
            Case LbmClass.PBtOH
                Return "PBtOH"
            Case LbmClass.MMPE
                Return "MMPE"
            Case LbmClass.DMPE
                Return "DMPE"
            Case LbmClass.LNAPE
                Return "LNAPE"
            Case LbmClass.LNAPS
                Return "LNAPS"
            Case LbmClass.DGDG
                Return "DGDG"
            Case LbmClass.MGDG
                Return "MGDG"
            Case LbmClass.SQDG
                Return "SQDG"
            Case LbmClass.DGTS
                Return "DGTS"
            Case LbmClass.DGTA
                Return "DGTA"
            Case LbmClass.DGCC
                Return "DGCC"
            Case LbmClass.DGGA
                Return "DGGA"
            Case LbmClass.ADGGA
                Return "ADGGA"
            Case LbmClass.EtherMGDG
                Return "EtherMGDG"
            Case LbmClass.EtherDGDG
                Return "EtherDGDG"
            Case LbmClass.CE
                Return "CE"
            Case LbmClass.BRSE
                Return "BRSE"
            Case LbmClass.CASE
                Return "CASE"
            Case LbmClass.SISE
                Return "SISE"
            Case LbmClass.STSE
                Return "STSE"
            Case LbmClass.EGSE
                Return "EGSE"
            Case LbmClass.DEGSE
                Return "DEGSE"
            Case LbmClass.DSMSE
                Return "DSMSE"
            Case LbmClass.AHexCS
                Return "AHexCS"
            Case LbmClass.AHexBRS
                Return "AHexBRS"
            Case LbmClass.AHexCAS
                Return "AHexCAS"
            Case LbmClass.AHexSIS
                Return "AHexSIS"
            Case LbmClass.AHexSTS
                Return "AHexSTS"
            Case LbmClass.DCAE
                Return "DCAE"
            Case LbmClass.GDCAE
                Return "GDCAE"
            Case LbmClass.GLCAE
                Return "GLCAE"
            Case LbmClass.TDCAE
                Return "TDCAE"
            Case LbmClass.TLCAE
                Return "TLCAE"
            Case LbmClass.LCAE
                Return "LCAE"
            Case LbmClass.KLCAE
                Return "KLCAE"
            Case LbmClass.KDCAE
                Return "KDCAE"
            Case LbmClass.SHex
                Return "SHex"
            Case LbmClass.SSulfate
                Return "SSulfate"
            Case LbmClass.BAHex
                Return "BAHex"
            Case LbmClass.SPEHex
                Return "SPEHex"
            Case LbmClass.SPGHex
                Return "SPGHex"
            Case LbmClass.CSLPHex
                Return "CSLPHex"
            Case LbmClass.BRSLPHex
                Return "BRSLPHex"
            Case LbmClass.CASLPHex
                Return "CASLPHex"
            Case LbmClass.SISLPHex
                Return "SISLPHex"
            Case LbmClass.STSLPHex
                Return "STSLPHex"
            Case LbmClass.CSPHex
                Return "CSPHex"
            Case LbmClass.BRSPHex
                Return "BRSPHex"
            Case LbmClass.CASPHex
                Return "CASPHex"
            Case LbmClass.SISPHex
                Return "SISPHex"
            Case LbmClass.STSPHex
                Return "STSPHex"
            Case LbmClass.SPE
                Return "SPE"
            Case LbmClass.BASulfate
                Return "BASulfate"
            Case LbmClass.Vitamin_E
                Return "Vitamin_E"
            Case LbmClass.Vitamin_D
                Return "Vitamin_D"
            Case LbmClass.VAE
                Return "VAE"
            Case LbmClass.BileAcid
                Return "BileAcid"
            Case LbmClass.CoQ
                Return "CoQ"
            Case LbmClass.CAR
                Return "CAR"
            Case LbmClass.FA
                Return "FA"
            Case LbmClass.OxFA
                Return "OxFA"
            Case LbmClass.DMEDFA
                Return "DMEDFA"
            Case LbmClass.DMEDOxFA
                Return "DMEDOxFA"
            Case LbmClass.NAE
                Return "NAE"
            Case LbmClass.NAGly
                Return "NAGly"
            Case LbmClass.NAGlySer
                Return "NAGlySer"
            Case LbmClass.NAOrn
                Return "NAOrn"
            Case LbmClass.NATryA
                Return "NATryA"
            Case LbmClass.NA5HT
                Return "NA5HT"
            Case LbmClass.NAAla
                Return "NAAla"
            Case LbmClass.NAGln
                Return "NAGln"
            Case LbmClass.NALeu
                Return "NALeu"
            Case LbmClass.NAVal
                Return "NAVal"
            Case LbmClass.NASer
                Return "NASer"
            Case LbmClass.WE
                Return "WE"

            Case LbmClass.FAHFA
                Return "FAHFA"
            Case LbmClass.DMEDFAHFA
                Return "DMEDFAHFA"
            Case LbmClass.PhytoSph
                Return "PhytoSph"
            Case LbmClass.DHSph
                Return "DHSph"
            Case LbmClass.Sph
                Return "Sph"
            Case LbmClass.Cer_ADS
                Return "Cer-ADS"
            Case LbmClass.Cer_AS
                Return "Cer-AS"
            Case LbmClass.Cer_BS
                Return "Cer-BS"
            Case LbmClass.Cer_BDS
                Return "Cer-BDS"
            Case LbmClass.Cer_NDS
                Return "Cer-NDS"
            Case LbmClass.Cer_NS
                Return "Cer-NS"
            Case LbmClass.Cer_NP
                Return "Cer-NP"
            Case LbmClass.Cer_AP
                Return "Cer-AP"
            Case LbmClass.Cer_EODS
                Return "Cer-EODS"
            Case LbmClass.Cer_EOS
                Return "Cer-EOS"
            Case LbmClass.Cer_OS
                Return "Cer-OS"
            Case LbmClass.Cer_HS
                Return "Cer-HS"
            Case LbmClass.Cer_HDS
                Return "Cer-HDS"
            Case LbmClass.Cer_NDOS
                Return "Cer-NDOS"
            Case LbmClass.HexCer_NS
                Return "HexCer-NS"
            Case LbmClass.HexCer_NDS
                Return "HexCer-NDS"
            Case LbmClass.HexCer_AP
                Return "HexCer-AP"
            Case LbmClass.HexCer_HS
                Return "HexCer-HS"
            Case LbmClass.HexCer_HDS
                Return "HexCer-HDS"
            Case LbmClass.HexCer_EOS
                Return "HexCer-EOS"
            Case LbmClass.Hex2Cer
                Return "Hex2Cer"
            Case LbmClass.Hex3Cer
                Return "Hex3Cer"
            Case LbmClass.PE_Cer
                Return "PE-Cer"
            Case LbmClass.PI_Cer
                Return "PI-Cer"
            Case LbmClass.MIPC
                Return "MIPC"
            Case LbmClass.CerP
                Return "CerP"
            Case LbmClass.SM
                Return "SM"
            Case LbmClass.SHexCer
                Return "SHexCer"
            Case LbmClass.SL
                Return "SL"
            Case LbmClass.GM3
                Return "GM3"
            Case LbmClass.Ac2PIM1
                Return "Ac2PIM1"
            Case LbmClass.Ac2PIM2
                Return "Ac2PIM2"
            Case LbmClass.Ac3PIM2
                Return "Ac3PIM2"
            Case LbmClass.Ac4PIM2
                Return "Ac4PIM2"
            Case LbmClass.Cer_EBDS
                Return "Cer-EBDS"
            Case LbmClass.AHexCer
                Return "AHexCer"
            Case LbmClass.ASHexCer
                Return "ASHexCer"
            Case LbmClass.ASM
                Return "ASM"
            Case LbmClass.EtherSMGDG
                Return "EtherSMGDG"
            Case LbmClass.SMGDG
                Return "SMGDG"
            Case LbmClass.GPNAE
                Return "GPNAE"
            Case LbmClass.MGMG
                Return "MGMG"
            Case LbmClass.DGMG
                Return "DGMG"
            Case LbmClass.GD1a
                Return "GD1a"
            Case LbmClass.GD1b
                Return "GD1b"
            Case LbmClass.GD2
                Return "GD2"
            Case LbmClass.GD3
                Return "GD3"
            Case LbmClass.GM1
                Return "GM1"
            Case LbmClass.GT1b
                Return "GT1b"
            Case LbmClass.GQ1b
                Return "GQ1b"
            Case LbmClass.NGcGM3
                Return "NGcGM3"
            Case LbmClass.ST
                Return "ST"
            Case LbmClass.NAPhe
                Return "NAPhe"
            Case LbmClass.NATau
                Return "NATau"

            Case LbmClass.LPC_d5
                Return "LPC_d5"
            Case LbmClass.LPE_d5
                Return "LPE_d5"
            Case LbmClass.LPG_d5
                Return "LPG_d5"
            Case LbmClass.LPI_d5
                Return "LPI_d5"
            Case LbmClass.LPS_d5
                Return "LPS_d5"

            Case LbmClass.PC_d5
                Return "PC_d5"
            Case LbmClass.PE_d5
                Return "PE_d5"
            Case LbmClass.PG_d5
                Return "PG_d5"
            Case LbmClass.PI_d5
                Return "PI_d5"
            Case LbmClass.PS_d5
                Return "PS_d5"

            Case LbmClass.DG_d5
                Return "DG_d5"
            Case LbmClass.TG_d5
                Return "TG_d5"

            Case LbmClass.CE_d7
                Return "CE_d7"
            Case LbmClass.Cer_NS_d7
                Return "Cer_NS_d7"
            Case LbmClass.SM_d9
                Return "SM_d9"

            Case LbmClass.bmPC
                Return "bmPC"
            Case LbmClass.BisMeLPA
                Return "BisMeLPA"
            Case Else
                Return "Undefined"
        End Select
    End Function

    'public static string ConvertMsdialLbmStringToMsdialOfficialOntology(string lipidclass) {
    '    var lbmclass = ConvertMsdialClassDefinitionToLbmClassEnumVS2(lipidclass);
    '    return ConvertLbmClassEnumToMsdialClassDefinitionVS2(lbmclass);
    '}


    'public static LbmClass ConvertMsdialClassDefinitionToLbmClassEnum(string lipidclass) {
    '    switch (lipidclass) {
    '        case "MAG": return LbmClass.MG;
    '        case "DAG": return LbmClass.DG;
    '        case "TAG": return LbmClass.TG;
    '        case "EtherDAG": return LbmClass.EtherDG;
    '        case "EtherTAG": return LbmClass.EtherTG;

    '        case "LPC": return LbmClass.LPC;
    '        case "LPA": return LbmClass.LPA;
    '        case "LPE": return LbmClass.LPE;
    '        case "LPG": return LbmClass.LPG;
    '        case "LPI": return LbmClass.LPI;
    '        case "LPS": return LbmClass.LPS;
    '        case "LDGTS": return LbmClass.LDGTS;
    '        case "LDGCC": return LbmClass.LDGCC;

    '        case "EtherLPC": return LbmClass.EtherLPC;
    '        case "EtherLPE": return LbmClass.EtherLPE;
    '        case "EtherLPG": return LbmClass.EtherLPG;
    '        case "EtherLPI": return LbmClass.EtherLPI;
    '        case "EtherLPS": return LbmClass.EtherLPS;

    '        case "PC": return LbmClass.PC;
    '        case "PA": return LbmClass.PA;
    '        case "PE": return LbmClass.PE;
    '        case "PG": return LbmClass.PG;
    '        case "PI": return LbmClass.PI;
    '        case "PS": return LbmClass.PS;
    '        case "BMP": return LbmClass.BMP;
    '        case "HBMP": return LbmClass.HBMP;
    '        case "CL": return LbmClass.CL;
    '        case "DLCL": return LbmClass.DLCL;
    '        case "LCL": return LbmClass.MLCL;

    '        case "EtherPC": return LbmClass.EtherPC;
    '        case "EtherPE": return LbmClass.EtherPE;
    '        case "EtherPG": return LbmClass.EtherPG;
    '        case "EtherPI": return LbmClass.EtherPI;
    '        case "EtherPS": return LbmClass.EtherPS;
    '        case "EtherMGDG": return LbmClass.EtherMGDG;
    '        case "EtherDGDG": return LbmClass.EtherDGDG;

    '        case "OxPC": return LbmClass.OxPC;
    '        case "OxPE": return LbmClass.OxPE;
    '        case "OxPG": return LbmClass.OxPG;
    '        case "OxPI": return LbmClass.OxPI;
    '        case "OxPS": return LbmClass.OxPS;

    '        case "EtherOxPC": return LbmClass.EtherOxPC;
    '        case "EtherOxPE": return LbmClass.EtherOxPE;

    '        case "PMeOH": return LbmClass.PMeOH;
    '        case "PEtOH": return LbmClass.PEtOH;
    '        case "PBtOH": return LbmClass.PBtOH;

    '        case "LNAPE": return LbmClass.LNAPE;
    '        case "LNAPS": return LbmClass.LNAPS;

    '        case "DGDG": return LbmClass.DGDG;
    '        case "MGDG": return LbmClass.MGDG;
    '        case "SQDG": return LbmClass.SQDG;
    '        case "DGTS": return LbmClass.DGTS;
    '        case "DGCC": return LbmClass.DGCC;
    '        case "GlcADG": return LbmClass.DGGA;
    '        case "AcylGlcADG": return LbmClass.ADGGA;
    '        case "DGGA": return LbmClass.DGGA;
    '        case "ADGGA": return LbmClass.ADGGA;

    '        case "CE": return LbmClass.CE;
    '        case "SHex": return LbmClass.SHex;
    '        case "SSulfate": return LbmClass.SSulfate;
    '        case "BAHex": return LbmClass.BAHex;
    '        case "BASulfate": return LbmClass.BASulfate;

    '        case "BRSE": return LbmClass.BRSE;
    '        case "CASE": return LbmClass.CASE;
    '        case "SISE": return LbmClass.SISE;
    '        case "STSE": return LbmClass.STSE;

    '        case "AHexCS": return LbmClass.AHexCS;
    '        case "AHexBRS": return LbmClass.AHexBRS;
    '        case "AHexCAS": return LbmClass.AHexCAS;
    '        case "AHexSIS": return LbmClass.AHexSIS;
    '        case "AHexSTS": return LbmClass.AHexSTS;

    '        case "DCAE": return LbmClass.DCAE;
    '        case "GDCAE": return LbmClass.GDCAE;
    '        case "GLCAE": return LbmClass.GLCAE;
    '        case "TDCAE": return LbmClass.TDCAE;
    '        case "TLCAE": return LbmClass.TLCAE;

    '        case "Vitamin": return LbmClass.Vitamin_E;
    '        case "VAE": return LbmClass.VAE;
    '        case "BileAcid": return LbmClass.BileAcid;
    '        case "CoQ": return LbmClass.CoQ;

    '        case "ACar": return LbmClass.CAR;
    '        case "FA": return LbmClass.FA;
    '        case "FAHFA": return LbmClass.FAHFA;

    '        case "NAE": return LbmClass.NAE;
    '        case "NAAG": return LbmClass.NAGly;
    '        case "NAAGS": return LbmClass.NAGlySer;
    '        case "NAAO": return LbmClass.NAOrn;

    '        case "Phytosphingosine": return LbmClass.PhytoSph;
    '        case "Sphinganine": return LbmClass.DHSph;
    '        case "Sphingosine": return LbmClass.Sph;


    '        case "Cer-ADS": return LbmClass.Cer_ADS;
    '        case "Cer-AS": return LbmClass.Cer_AS;
    '        case "Cer-BDS": return LbmClass.Cer_BDS;
    '        case "Cer-BS": return LbmClass.Cer_BS;
    '        case "Cer-NDS": return LbmClass.Cer_NDS;
    '        case "Cer-NS": return LbmClass.Cer_NS;
    '        case "Cer-NP": return LbmClass.Cer_NP;
    '        case "Cer-AP": return LbmClass.Cer_AP;
    '        case "Cer-EODS": return LbmClass.Cer_EODS;
    '        case "Cer-EOS": return LbmClass.Cer_EOS;
    '        case "Cer-OS": return LbmClass.Cer_OS;
    '        case "Cer-HS": return LbmClass.Cer_HS;
    '        case "Cer-HDS": return LbmClass.Cer_HDS;
    '        case "Cer-NDOS": return LbmClass.Cer_NDOS;

    '        case "HexCer-NS": return LbmClass.HexCer_NS;
    '        case "HexCer-NDS": return LbmClass.HexCer_NDS;
    '        case "HexCer-AP": return LbmClass.HexCer_AP;
    '        case "HexCer-HS": return LbmClass.HexCer_HS;
    '        case "HexCer-HDS": return LbmClass.HexCer_HDS;
    '        case "HexCer-EOS": return LbmClass.HexCer_EOS;
    '        case "HexHexCer-NS": return LbmClass.Hex2Cer;
    '        case "HexHexHexCer-NS": return LbmClass.Hex3Cer;
    '        case "HexHexCer": return LbmClass.Hex2Cer;
    '        case "HexHexHexCer": return LbmClass.Hex3Cer;
    '        case "Hex2Cer": return LbmClass.Hex2Cer;
    '        case "Hex3Cer": return LbmClass.Hex3Cer;
    '        case "PE-Cer": return LbmClass.PE_Cer;
    '        case "PI-Cer": return LbmClass.PI_Cer;

    '        case "Cer_ADS": return LbmClass.Cer_ADS;
    '        case "Cer_AS": return LbmClass.Cer_AS;
    '        case "Cer_BDS": return LbmClass.Cer_BDS;
    '        case "Cer_BS": return LbmClass.Cer_BS;
    '        case "Cer_NDS": return LbmClass.Cer_NDS;
    '        case "Cer_NS": return LbmClass.Cer_NS;
    '        case "Cer_NP": return LbmClass.Cer_NP;
    '        case "Cer_AP": return LbmClass.Cer_AP;
    '        case "Cer_EODS": return LbmClass.Cer_EODS;
    '        case "Cer_EOS": return LbmClass.Cer_EOS;
    '        case "Cer_OS": return LbmClass.Cer_OS;
    '        case "Cer_HS": return LbmClass.Cer_HS;
    '        case "Cer_HDS": return LbmClass.Cer_HDS;
    '        case "Cer_NDOS": return LbmClass.Cer_NDOS;

    '        case "HexCer_NS": return LbmClass.HexCer_NS;
    '        case "HexCer_NDS": return LbmClass.HexCer_NDS;
    '        case "HexCer_AP": return LbmClass.HexCer_AP;
    '        case "HexCer_EOS": return LbmClass.HexCer_EOS;
    '        case "HexCer_HS": return LbmClass.HexCer_HS;
    '        case "HexCer_HDS": return LbmClass.HexCer_HDS;
    '        case "HexHexCer_NS": return LbmClass.Hex2Cer;
    '        case "HexHexHexCer_NS": return LbmClass.Hex3Cer;
    '        case "PE_Cer": return LbmClass.PE_Cer;
    '        case "PI_Cer": return LbmClass.PI_Cer;

    '        case "CerP": return LbmClass.CerP;

    '        case "SM": return LbmClass.SM;
    '        case "SHexCer": return LbmClass.SHexCer;
    '        case "GM3": return LbmClass.GM3;
    '        case "SL": return LbmClass.SL;

    '        case "Ac2PIM1": return LbmClass.Ac2PIM1;
    '        case "Ac2PIM2": return LbmClass.Ac2PIM2;
    '        case "Ac3PIM2": return LbmClass.Ac3PIM2;
    '        case "Ac4PIM2": return LbmClass.Ac4PIM2;

    '        case "AcylCer-BDS": return LbmClass.Cer_EBDS;
    '        case "AcylCer_BDS": return LbmClass.Cer_EBDS;
    '        case "AcylHexCer": return LbmClass.AHexCer;
    '        case "AcylSM": return LbmClass.ASM;
    '        case "Cer_EBDS": return LbmClass.Cer_EBDS;
    '        case "Cer-EBDS": return LbmClass.Cer_EBDS;
    '        case "AHexCer": return LbmClass.AHexCer;
    '        case "ASM": return LbmClass.ASM;

    '        default: return LbmClass.Undefined;
    '    }
    '}

    Public Function ConvertMsdialClassDefinitionToLbmClassEnumVS2(lipidclass As String) As LbmClass
        Select Case lipidclass
            Case "MG"
                Return LbmClass.MG
            Case "DG"
                Return LbmClass.DG
            Case "TG"
                Return LbmClass.TG
            Case "OxTG"
                Return LbmClass.OxTG
            Case "TG_EST"
                Return LbmClass.TG_EST
            Case "EtherDG"
                Return LbmClass.EtherDG
            Case "EtherTG"
                Return LbmClass.EtherTG

            Case "LPC"
                Return LbmClass.LPC
            Case "LPA"
                Return LbmClass.LPA
            Case "LPE"
                Return LbmClass.LPE
            Case "LPG"
                Return LbmClass.LPG
            Case "LPI"
                Return LbmClass.LPI
            Case "LPS"
                Return LbmClass.LPS
            Case "LDGTS"
                Return LbmClass.LDGTS
            Case "LDGTA"
                Return LbmClass.LDGTA
            Case "LDGCC"
                Return LbmClass.LDGCC
            Case "BisMeLPA"
                Return LbmClass.BisMeLPA

            Case "EtherLPC"
                Return LbmClass.EtherLPC
            Case "EtherLPE"
                Return LbmClass.EtherLPE
            Case "EtherLPG"
                Return LbmClass.EtherLPG
            Case "EtherLPI"
                Return LbmClass.EtherLPI
            Case "EtherLPS"
                Return LbmClass.EtherLPS

            Case "PC"
                Return LbmClass.PC
            Case "PA"
                Return LbmClass.PA
            Case "PE"
                Return LbmClass.PE
            Case "PG"
                Return LbmClass.PG
            Case "PI"
                Return LbmClass.PI
            Case "PS"
                Return LbmClass.PS
            Case "PT"
                Return LbmClass.PT
            Case "BMP"
                Return LbmClass.BMP
            Case "HBMP"
                Return LbmClass.HBMP
            Case "CL"
                Return LbmClass.CL
            Case "DLCL"
                Return LbmClass.DLCL
            Case "MLCL"
                Return LbmClass.MLCL

            Case "Ac2PIM1"
                Return LbmClass.Ac2PIM1
            Case "Ac2PIM2"
                Return LbmClass.Ac2PIM2
            Case "Ac3PIM2"
                Return LbmClass.Ac3PIM2
            Case "Ac4PIM2"
                Return LbmClass.Ac4PIM2

            Case "EtherPC"
                Return LbmClass.EtherPC
            Case "EtherPE"
                Return LbmClass.EtherPE
            Case "EtherPE_O"
                Return LbmClass.EtherPE
            Case "EtherPE_P"
                Return LbmClass.EtherPE
            Case "EtherPG"
                Return LbmClass.EtherPG
            Case "EtherPI"
                Return LbmClass.EtherPI
            Case "EtherPS"
                Return LbmClass.EtherPS
            Case "EtherMGDG"
                Return LbmClass.EtherMGDG
            Case "EtherDGDG"
                Return LbmClass.EtherDGDG
            Case "EtherSMGDG"
                Return LbmClass.EtherSMGDG

            Case "OxPC"
                Return LbmClass.OxPC
            Case "OxPE"
                Return LbmClass.OxPE
            Case "OxPG"
                Return LbmClass.OxPG
            Case "OxPI"
                Return LbmClass.OxPI
            Case "OxPS"
                Return LbmClass.OxPS

            Case "EtherOxPC"
                Return LbmClass.EtherOxPC
            Case "EtherOxPE"
                Return LbmClass.EtherOxPE

            Case "PMeOH"
                Return LbmClass.PMeOH
            Case "PEtOH"
                Return LbmClass.PEtOH
            Case "PBtOH"
                Return LbmClass.PBtOH
            Case "MMPE"
                Return LbmClass.MMPE
            Case "DMPE"
                Return LbmClass.DMPE

            Case "LNAPE"
                Return LbmClass.LNAPE
            Case "LNAPS"
                Return LbmClass.LNAPS

            Case "DGDG"
                Return LbmClass.DGDG
            Case "MGDG"
                Return LbmClass.MGDG
            Case "SMGDG"
                Return LbmClass.SMGDG
            Case "SQDG"
                Return LbmClass.SQDG
            Case "DGTS"
                Return LbmClass.DGTS
            Case "DGTA"
                Return LbmClass.DGTA
            Case "DGCC"
                Return LbmClass.DGCC
            Case "DGGA"
                Return LbmClass.DGGA
            Case "ADGGA"
                Return LbmClass.ADGGA

            Case "CE"
                Return LbmClass.CE
            Case "SHex"
                Return LbmClass.SHex
            Case "SSulfate"
                Return LbmClass.SSulfate
            Case "BAHex"
                Return LbmClass.BAHex
            Case "BASulfate"
                Return LbmClass.BASulfate

            Case "BRSE"
                Return LbmClass.BRSE
            Case "CASE"
                Return LbmClass.CASE
            Case "SISE"
                Return LbmClass.SISE
            Case "STSE"
                Return LbmClass.STSE
            Case "EGSE"
                Return LbmClass.EGSE
            Case "DEGSE"
                Return LbmClass.DEGSE
            Case "DSMSE"
                Return LbmClass.DSMSE

            Case "AHexCS"
                Return LbmClass.AHexCS
            Case "AHexBRS"
                Return LbmClass.AHexBRS
            Case "AHexCAS"
                Return LbmClass.AHexCAS
            Case "AHexSIS"
                Return LbmClass.AHexSIS
            Case "AHexSTS"
                Return LbmClass.AHexSTS

            Case "BRSLPHex"
                Return LbmClass.BRSLPHex
            Case "BRSPHex"
                Return LbmClass.BRSPHex
            Case "CASLPHex"
                Return LbmClass.CASLPHex
            Case "CASPHex"
                Return LbmClass.CASPHex
            Case "CSLPHex"
                Return LbmClass.CSLPHex
            Case "CSPHex"
                Return LbmClass.CSPHex
            Case "SISLPHex"
                Return LbmClass.SISLPHex
            Case "SISPHex"
                Return LbmClass.SISPHex
            Case "STSLPHex"
                Return LbmClass.STSLPHex
            Case "STSPHex"
                Return LbmClass.STSPHex

            Case "SPE"
                Return LbmClass.SPE
            Case "SPEHex"
                Return LbmClass.SPEHex
            Case "SPGHex"
                Return LbmClass.SPGHex

            Case "DCAE"
                Return LbmClass.DCAE
            Case "GDCAE"
                Return LbmClass.GDCAE
            Case "GLCAE"
                Return LbmClass.GLCAE
            Case "TDCAE"
                Return LbmClass.TDCAE
            Case "TLCAE"
                Return LbmClass.TLCAE
            Case "LCAE"
                Return LbmClass.LCAE
            Case "KLCAE"
                Return LbmClass.KLCAE
            Case "KDCAE"
                Return LbmClass.KDCAE

            Case "Vitamin_E"
                Return LbmClass.Vitamin_E
            Case "Vitamin E"
                Return LbmClass.Vitamin_E
            Case "Vitamin_D"
                Return LbmClass.Vitamin_D
            Case "Vitamin D"
                Return LbmClass.Vitamin_D
            Case "VAE"
                Return LbmClass.VAE
            Case "BileAcid"
                Return LbmClass.BileAcid
            Case "CoQ"
                Return LbmClass.CoQ

            Case "CAR"
                Return LbmClass.CAR
            Case "FA"
                Return LbmClass.FA
            Case "OxFA"
                Return LbmClass.OxFA
            Case "FAHFA"
                Return LbmClass.FAHFA
            Case "DMEDFAHFA"
                Return LbmClass.DMEDFAHFA
            Case "DMEDFA"
                Return LbmClass.DMEDFA
            Case "DMEDOxFA"
                Return LbmClass.DMEDOxFA

            Case "NAE"
                Return LbmClass.NAE
            Case "NAGly"
                Return LbmClass.NAGly
            Case "NAGlySer"
                Return LbmClass.NAGlySer
            Case "NAOrn"
                Return LbmClass.NAOrn
            Case "NAPhe"
                Return LbmClass.NAPhe
            Case "NATau"
                Return LbmClass.NATau
            Case "NATryA"
                Return LbmClass.NATryA
            Case "NA5HT"
                Return LbmClass.NA5HT
            Case "NAAla"
                Return LbmClass.NAAla
            Case "NAGln"
                Return LbmClass.NAGln
            Case "NALeu"
                Return LbmClass.NALeu
            Case "NAVal"
                Return LbmClass.NAVal
            Case "NASer"
                Return LbmClass.NASer
            Case "WE"
                Return LbmClass.WE

            Case "PhytoSph"
                Return LbmClass.PhytoSph
            Case "DHSph"
                Return LbmClass.DHSph
            Case "Sph"
                Return LbmClass.Sph


            Case "Cer-ADS"
                Return LbmClass.Cer_ADS
            Case "Cer-AS"
                Return LbmClass.Cer_AS
            Case "Cer-BDS"
                Return LbmClass.Cer_BDS
            Case "Cer-BS"
                Return LbmClass.Cer_BS
            Case "Cer-NDS"
                Return LbmClass.Cer_NDS
            Case "Cer-NS"
                Return LbmClass.Cer_NS
            Case "Cer-NP"
                Return LbmClass.Cer_NP
            Case "Cer-AP"
                Return LbmClass.Cer_AP
            Case "Cer-EODS"
                Return LbmClass.Cer_EODS
            Case "Cer-EOS"
                Return LbmClass.Cer_EOS
            Case "Cer-OS"
                Return LbmClass.Cer_OS
            Case "Cer-HS"
                Return LbmClass.Cer_HS
            Case "Cer-HDS"
                Return LbmClass.Cer_HDS
            Case "Cer-NDOS"
                Return LbmClass.Cer_NDOS

            Case "HexCer-NS"
                Return LbmClass.HexCer_NS
            Case "HexCer-NDS"
                Return LbmClass.HexCer_NDS
            Case "HexCer-AP"
                Return LbmClass.HexCer_AP
            Case "HexCer-HS"
                Return LbmClass.HexCer_HS
            Case "HexCer-HDS"
                Return LbmClass.HexCer_HDS
            Case "HexCer-EOS"
                Return LbmClass.HexCer_EOS
            Case "Hex2Cer"
                Return LbmClass.Hex2Cer
            Case "Hex3Cer"
                Return LbmClass.Hex3Cer
            Case "PE-Cer"
                Return LbmClass.PE_Cer
            Case "PI-Cer"
                Return LbmClass.PI_Cer
            Case "PE-Cer+O"
                Return LbmClass.PE_Cer
            Case "PI-Cer+O"
                Return LbmClass.PI_Cer
            Case "MIPC"
                Return LbmClass.MIPC

            Case "Cer_ADS"
                Return LbmClass.Cer_ADS
            Case "Cer_AS"
                Return LbmClass.Cer_AS
            Case "Cer_BDS"
                Return LbmClass.Cer_BDS
            Case "Cer_BS"
                Return LbmClass.Cer_BS
            Case "Cer_NDS"
                Return LbmClass.Cer_NDS
            Case "Cer_NS"
                Return LbmClass.Cer_NS
            Case "Cer_NP"
                Return LbmClass.Cer_NP
            Case "Cer_AP"
                Return LbmClass.Cer_AP
            Case "Cer_EODS"
                Return LbmClass.Cer_EODS
            Case "Cer_EOS"
                Return LbmClass.Cer_EOS
            Case "Cer_OS"
                Return LbmClass.Cer_OS
            Case "Cer_HS"
                Return LbmClass.Cer_HS
            Case "Cer_HDS"
                Return LbmClass.Cer_HDS
            Case "Cer_NDOS"
                Return LbmClass.Cer_NDOS

            Case "HexCer_NS"
                Return LbmClass.HexCer_NS
            Case "HexCer_NDS"
                Return LbmClass.HexCer_NDS
            Case "HexCer_AP"
                Return LbmClass.HexCer_AP
            Case "HexCer_EOS"
                Return LbmClass.HexCer_EOS
            Case "HexCer_HS"
                Return LbmClass.HexCer_HS
            Case "HexCer_HDS"
                Return LbmClass.HexCer_HDS
            Case "PE_Cer"
                Return LbmClass.PE_Cer
            Case "PI_Cer"
                Return LbmClass.PI_Cer
            Case "PE_Cer+O"
                Return LbmClass.PE_Cer
            Case "PI_Cer+O"
                Return LbmClass.PI_Cer

            Case "CerP"
                Return LbmClass.CerP

            Case "SM"
                Return LbmClass.SM
            Case "SHexCer"
                Return LbmClass.SHexCer
            Case "SL"
                Return LbmClass.SL
            Case "SM+O"
                Return LbmClass.SM
            Case "SHexCer+O"
                Return LbmClass.SHexCer
            Case "SL+O"
                Return LbmClass.SL
            Case "GM3"
                Return LbmClass.GM3

            Case "Cer_EBDS"
                Return LbmClass.Cer_EBDS
            Case "Cer-EBDS"
                Return LbmClass.Cer_EBDS
            Case "AHexCer"
                Return LbmClass.AHexCer
            Case "ASHexCer"
                Return LbmClass.ASHexCer
            Case "ASM"
                Return LbmClass.ASM

            Case "GPNAE"
                Return LbmClass.GPNAE
            Case "MGMG"
                Return LbmClass.MGMG
            Case "DGMG"
                Return LbmClass.DGMG

            Case "GD1a"
                Return LbmClass.GD1a
            Case "GD1b"
                Return LbmClass.GD1b
            Case "GD2"
                Return LbmClass.GD2
            Case "GD3"
                Return LbmClass.GD3
            Case "GM1"
                Return LbmClass.GM1
            Case "GT1b"
                Return LbmClass.GT1b
            Case "GQ1b"
                Return LbmClass.GQ1b
            Case "NGcGM3"
                Return LbmClass.NGcGM3
            Case "ST"
                Return LbmClass.ST

            Case "LPC_d5"
                Return LbmClass.LPC_d5
            Case "LPE_d5"
                Return LbmClass.LPE_d5
            Case "LPG_d5"
                Return LbmClass.LPG_d5
            Case "LPI_d5"
                Return LbmClass.LPI_d5
            Case "LPS_d5"
                Return LbmClass.LPS_d5

            Case "PC_d5"
                Return LbmClass.PC_d5
            Case "PE_d5"
                Return LbmClass.PE_d5
            Case "PG_d5"
                Return LbmClass.PG_d5
            Case "PI_d5"
                Return LbmClass.PI_d5
            Case "PS_d5"
                Return LbmClass.PS_d5

            Case "DG_d5"
                Return LbmClass.DG_d5
            Case "TG_d5"
                Return LbmClass.TG_d5

            Case "CE_d7"
                Return LbmClass.CE_d7
            Case "Cer_NS_d7"
                Return LbmClass.Cer_NS_d7
            Case "SM_d9"
                Return LbmClass.SM_d9

            Case "bmPC"
                Return LbmClass.bmPC
            Case Else

                Return LbmClass.Undefined
        End Select
    End Function


    'public static string ConvertMsdialClassDefinitionToSuperClass(string lipidclass) {
    '    switch (lipidclass) {
    '        case "MAG": return "Glycerolipid";
    '        case "DAG": return "Glycerolipid";
    '        case "TAG": return "Glycerolipid";
    '        case "EtherDAG": return "Ether linked glycerolipid";
    '        case "EtherTAG": return "Ether linked glycerolipid";

    '        case "LPC": return "Lyso phospholipid";
    '        case "LPA": return "Lyso phospholipid";
    '        case "LPE": return "Lyso phospholipid";
    '        case "LPG": return "Lyso phospholipid";
    '        case "LPI": return "Lyso phospholipid";
    '        case "LPS": return "Lyso phospholipid";
    '        case "LDGTS": return "Lyso algal lipid";
    '        case "LDGCC": return "Lyso algal lipid";

    '        case "PC": return "Phospholipid";
    '        case "PA": return "Phospholipid";
    '        case "PE": return "Phospholipid";
    '        case "PG": return "Phospholipid";
    '        case "PI": return "Phospholipid";
    '        case "PS": return "Phospholipid";
    '        case "BMP": return "Phospholipid";
    '        case "HBMP": return "Phospholipid";
    '        case "CL": return "Phospholipid";
    '        case "DLCL": return "Phospholipid";
    '        case "LCL": return "Phospholipid";

    '        case "EtherPC": return "Ether linked phospholipid";
    '        case "EtherPE": return "Ether linked phospholipid";
    '        case "EtherPS": return "Ether linked phospholipid";
    '        case "EtherPG": return "Ether linked phospholipid";
    '        case "EtherPI": return "Ether linked phospholipid";

    '        case "EtherLPC": return "Ether linked lyso phospholipid";
    '        case "EtherLPE": return "Ether linked lyso phospholipid";
    '        case "EtherLPS": return "Ether linked lyso phospholipid";
    '        case "EtherLPG": return "Ether linked lyso phospholipid";
    '        case "EtherLPI": return "Ether linked lyso phospholipid";

    '        case "OxPC": return "Oxidized phospholipid";
    '        case "OxPE": return "Oxidized phospholipid";
    '        case "OxPG": return "Oxidized phospholipid";
    '        case "OxPI": return "Oxidized phospholipid";
    '        case "OxPS": return "Oxidized phospholipid";

    '        case "EtherOxPC": return "Oxidized ether linked phospholipid";
    '        case "EtherOxPE": return "Oxidized ether linked phospholipid";

    '        case "PMeOH": return "Header modified phospholipid";
    '        case "PEtOH": return "Header modified phospholipid";
    '        case "PBtOH": return "Header modified phospholipid";

    '        case "LNAPE": return "N-acyl phospholipid";
    '        case "LNAPS": return "N-acyl phospholipid";

    '        case "NAE": return "N-acyl amide";
    '        case "NAAG": return "N-acyl amide";
    '        case "NAAGS": return "N-acyl amide";
    '        case "NAAO": return "N-acyl amide";

    '        case "DGDG": return "Plant lipid";
    '        case "MGDG": return "Plant lipid";
    '        case "SQDG": return "Plant lipid";
    '        case "DGTS": return "Algal lipid";
    '        case "DGCC": return "Algal lipid";
    '        case "GlcADG": return "Plant lipid";
    '        case "AcylGlcADG": return "Plant lipid";
    '        case "DGGA": return "Plant lipid";
    '        case "ADGGA": return "Plant lipid";
    '        case "EtherMGDG": return "Ether linked plant lipid";
    '        case "EtherDGDG": return "Ether linked plant lipid";

    '        case "CE": return "Cholesterol ester";
    '        case "Cholesterol": return "Cholesterol";
    '        case "CholesterolSulfate": return "Sterol sulfate";

    '        case "SHex": return "Sterol hexoside";
    '        case "SSulfate": return "Sterol sulfate";
    '        case "BAHex": return "Sterol hexoside";
    '        case "BASulfate": return "Sterol sulfate";

    '        case "BRSE": return "Steryl ester";
    '        case "CASE": return "Steryl ester";
    '        case "SISE": return "Steryl ester";
    '        case "STSE": return "Steryl ester";
    '        case "AHexCS": return "Steryl acyl hexoside";
    '        case "AHexBRS": return "Steryl acyl hexoside";
    '        case "AHexCAS": return "Steryl acyl hexoside";
    '        case "AHexSIS": return "Steryl acyl hexoside";
    '        case "AHexSTS": return "Steryl acyl hexoside";

    '        case "DCAE": return "Bile acid ester";
    '        case "GDCAE": return "Bile acid ester";
    '        case "GLCAE": return "Bile acid ester";
    '        case "TDCAE": return "Bile acid ester";
    '        case "TLCAE": return "Bile acid ester";

    '        case "CoQ": return "Coenzyme Q";
    '        case "Vitamin": return "Vitamin";
    '        case "VAE": return "Vitamin";
    '        case "BileAcid": return "Bile acid";

    '        case "ACar": return "Acyl carnitine";
    '        case "FA": return "Free fatty acid";
    '        case "FAHFA": return "Fatty acid ester of hydroxyl fatty acid";

    '        case "Phytosphingosine": return "Phytosphingosine";
    '        case "Sphinganine": return "Sphinganine";
    '        case "Sphingosine": return "Sphingosine";

    '        case "Cer_ADS": return "Ceramide";
    '        case "Cer_AS": return "Ceramide";
    '        case "Cer_BDS": return "Ceramide";
    '        case "Cer_BS": return "Ceramide";
    '        case "Cer_NDS": return "Ceramide";
    '        case "Cer_NS": return "Ceramide";
    '        case "Cer_NP": return "Ceramide";
    '        case "Cer_AP": return "Ceramide";
    '        case "Cer_OS": return "Ceramide";
    '        case "Cer_HS": return "Ceramide";
    '        case "Cer_HDS": return "Ceramide";
    '        case "Cer_NDOS": return "Ceramide";
    '        case "Cer_EODS": return "Acyl ceramide";
    '        case "Cer_EOS": return "Acyl ceramide";

    '        case "Cer-ADS": return "Ceramide";
    '        case "Cer-AS": return "Ceramide";
    '        case "Cer-BDS": return "Ceramide";
    '        case "Cer-BS": return "Ceramide";
    '        case "Cer-NDS": return "Ceramide";
    '        case "Cer-NS": return "Ceramide";
    '        case "Cer-NP": return "Ceramide";
    '        case "Cer-AP": return "Ceramide";
    '        case "Cer-OS": return "Ceramide";
    '        case "Cer-HS": return "Ceramide";
    '        case "Cer-HDS": return "Ceramide";
    '        case "Cer-NDOS": return "Ceramide";
    '        case "CerP": return "Ceramide phosphate";
    '        case "Cer-EODS": return "Acyl ceramide";
    '        case "Cer-EOS": return "Acyl ceramide";

    '        case "GlcCer_NS": return "Hexosyl ceramide";
    '        case "GlcCer_NDS": return "Hexosyl ceramide";
    '        case "GlcCer_AP": return "Hexosyl ceramide";

    '        case "HexCer-NS": return "Hexosyl ceramide";
    '        case "HexCer-NDS": return "Hexosyl ceramide";
    '        case "HexCer-AP": return "Hexosyl ceramide";
    '        case "HexCer-HS": return "Hexosyl ceramide";
    '        case "HexCer-HDS": return "Hexosyl ceramide";
    '        case "HexCer-EOS": return "Hexosyl ceramide";
    '        case "HexHexCer-NS": return "Hexosyl ceramide";
    '        case "HexHexHexCer-NS": return "Hexosyl ceramide";
    '        case "HexHexCer": return "Hexosyl ceramide";
    '        case "HexHexHexCer": return "Hexosyl ceramide";
    '        case "Hex2Cer": return "Hexosyl ceramide";
    '        case "Hex3Cer": return "Hexosyl ceramide";

    '        case "Cer_EBDS": return "Acyl ceramide";
    '        case "Cer-EBDS": return "Acyl ceramide";
    '        case "AHexCer": return "Acyl ceramide";
    '        case "ASM": return "Acyl sphingomyelin";

    '        case "PI-Cer": return "Ceramide";
    '        case "PE-Cer": return "Ceramide";

    '        case "SM": return "Sphingomyelin";
    '        case "SHexCer": return "Sulfatide";
    '        case "SL": return "Sulfonolipid";
    '        case "GM3": return "Ganglioside";
    '        case "GM3[NeuAc]": return "Ganglioside";

    '        default: return "Unassigned lipid";
    '    }
    '}

    Public Function ConvertMsdialClassDefinitionToSuperClassVS2(lipidclass As String) As String

        ' 
        '  FattyAcyls [FA]
        '  Glycerolipids [GL]
        '  Glycerophospholipids [GP]
        '  Sphingolipids [SP]
        '  SterolLipids [ST]
        '  PrenolLipids [PR]
        '  Saccharolipids [SL]
        '  Polyketides [PK]
        ' 


        Select Case lipidclass

            Case "NAE"
                Return "FattyAcyls"
            Case "NAGly"
                Return "FattyAcyls"
            Case "NAGlySer"
                Return "FattyAcyls"
            Case "NAOrn"
                Return "FattyAcyls"
            Case "NAPhe"
                Return "FattyAcyls"
            Case "NATau"
                Return "FattyAcyls"
            Case "NATryA"
                Return "FattyAcyls"
            Case "NA5HT"
                Return "FattyAcyls"
            Case "NAAla"
                Return "FattyAcyls"
            Case "NAGln"
                Return "FattyAcyls"
            Case "NALeu"
                Return "FattyAcyls"
            Case "NAVal"
                Return "FattyAcyls"
            Case "NASer"
                Return "FattyAcyls"
            Case "WE"
                Return "FattyAcyls"

            Case "CAR"
                Return "FattyAcyls"
            Case "FA"
                Return "FattyAcyls"
            Case "OxFA"
                Return "FattyAcyls"
            Case "FAHFA"
                Return "FattyAcyls"
            Case "DMEDFAHFA"
                Return "FattyAcyls"
            Case "DMEDFA"
                Return "FattyAcyls"
            Case "DMEDOxFA"
                Return "FattyAcyls"

            Case "MG"
                Return "Glycerolipids"
            Case "DG"
                Return "Glycerolipids"
            Case "TG"
                Return "Glycerolipids"
            Case "OxTG"
                Return "Glycerolipids"
            Case "TG_EST"
                Return "Glycerolipids"
            Case "EtherDG"
                Return "Glycerolipids"
            Case "EtherTG"
                Return "Glycerolipids"
            Case "LDGTS"
                Return "Glycerolipids"
            Case "LDGTA"
                Return "Glycerolipids"
            Case "LDGCC"
                Return "Glycerolipids"

            Case "DGDG"
                Return "Glycerolipids"
            Case "MGDG"
                Return "Glycerolipids"
            Case "SQDG"
                Return "Glycerolipids"
            Case "DGTS"
                Return "Glycerolipids"
            Case "DGTA"
                Return "Glycerolipids"
            Case "DGCC"
                Return "Glycerolipids"
            Case "DGGA"
                Return "Glycerolipids"
            Case "ADGGA"
                Return "Glycerolipids"
            Case "EtherMGDG"
                Return "Glycerolipids"
            Case "EtherDGDG"
                Return "Glycerolipids"
            Case "EtherSDGDG"
                Return "Glycerolipids"

            Case "LPC"
                Return "Glycerophospholipids"
            Case "LPA"
                Return "Glycerophospholipids"
            Case "LPE"
                Return "Glycerophospholipids"
            Case "LPG"
                Return "Glycerophospholipids"
            Case "LPI"
                Return "Glycerophospholipids"
            Case "LPS"
                Return "Glycerophospholipids"
            Case "BisMeLPA"
                Return "Glycerophospholipids"

            Case "PC"
                Return "Glycerophospholipids"
            Case "PA"
                Return "Glycerophospholipids"
            Case "PE"
                Return "Glycerophospholipids"
            Case "PG"
                Return "Glycerophospholipids"
            Case "PI"
                Return "Glycerophospholipids"
            Case "PS"
                Return "Glycerophospholipids"
            Case "PT"
                Return "Glycerophospholipids"
            Case "BMP"
                Return "Glycerophospholipids"
            Case "HBMP"
                Return "Glycerophospholipids"
            Case "CL"
                Return "Glycerophospholipids"
            Case "DLCL"
                Return "Glycerophospholipids"
            Case "MLCL"
                Return "Glycerophospholipids"
            Case "SMGDG"
                Return "Glycerolipids"
            Case "EtherSMGDG"
                Return "Glycerolipids"

            Case "EtherPC"
                Return "Glycerophospholipids"
            Case "EtherPE"
                Return "Glycerophospholipids"
            Case "EtherPE_O"
                Return "Glycerophospholipids"
            Case "EtherPE_P"
                Return "Glycerophospholipids"
            Case "EtherPS"
                Return "Glycerophospholipids"
            Case "EtherPG"
                Return "Glycerophospholipids"
            Case "EtherPI"
                Return "Glycerophospholipids"

            Case "EtherLPC"
                Return "Glycerophospholipids"
            Case "EtherLPE"
                Return "Glycerophospholipids"
            Case "EtherLPS"
                Return "Glycerophospholipids"
            Case "EtherLPG"
                Return "Glycerophospholipids"
            Case "EtherLPI"
                Return "Glycerophospholipids"

            Case "OxPC"
                Return "Glycerophospholipids"
            Case "OxPE"
                Return "Glycerophospholipids"
            Case "OxPG"
                Return "Glycerophospholipids"
            Case "OxPI"
                Return "Glycerophospholipids"
            Case "OxPS"
                Return "Glycerophospholipids"

            Case "EtherOxPC"
                Return "Glycerophospholipids"
            Case "EtherOxPE"
                Return "Glycerophospholipids"

            Case "PMeOH"
                Return "Glycerophospholipids"
            Case "PEtOH"
                Return "Glycerophospholipids"
            Case "PBtOH"
                Return "Glycerophospholipids"
            Case "MMPE"
                Return "Glycerophospholipids"
            Case "DMPE"
                Return "Glycerophospholipids"

            Case "LNAPE"
                Return "Glycerophospholipids"
            Case "LNAPS"
                Return "Glycerophospholipids"

            Case "Ac2PIM1"
                Return "Glycerophospholipids"
            Case "Ac2PIM2"
                Return "Glycerophospholipids"
            Case "Ac3PIM2"
                Return "Glycerophospholipids"
            Case "Ac4PIM2"
                Return "Glycerophospholipids"

            Case "BRSLPHex"
                Return "SterolLipids"
            Case "BRSPHex"
                Return "SterolLipids"
            Case "CASLPHex"
                Return "SterolLipids"
            Case "CASPHex"
                Return "SterolLipids"
            Case "CSLPHex"
                Return "SterolLipids"
            Case "CSPHex"
                Return "SterolLipids"
            Case "SISLPHex"
                Return "SterolLipids"
            Case "SISPHex"
                Return "SterolLipids"
            Case "STSLPHex"
                Return "SterolLipids"
            Case "STSPHex"
                Return "SterolLipids"

            Case "SPE"
                Return "SterolLipids"
            Case "SPEHex"
                Return "SterolLipids"
            Case "SPGHex"
                Return "SterolLipids"

            Case "CE"
                Return "SterolLipids"
            Case "Cholesterol"
                Return "SterolLipids"
            Case "CholesterolSulfate"
                Return "SterolLipids"

            Case "SHex"
                Return "SterolLipids"
            Case "SSulfate"
                Return "SterolLipids"
            Case "BAHex"
                Return "SterolLipids"
            Case "BASulfate"
                Return "SterolLipids"

            Case "BRSE"
                Return "SterolLipids"
            Case "CASE"
                Return "SterolLipids"
            Case "SISE"
                Return "SterolLipids"
            Case "STSE"
                Return "SterolLipids"
            Case "EGSE"
                Return "SterolLipids"
            Case "DEGSE"
                Return "SterolLipids"
            Case "DSMSE"
                Return "SterolLipids"
            Case "AHexCS"
                Return "SterolLipids"
            Case "AHexBRS"
                Return "SterolLipids"
            Case "AHexCAS"
                Return "SterolLipids"
            Case "AHexSIS"
                Return "SterolLipids"
            Case "AHexSTS"
                Return "SterolLipids"
            Case "ST"
                Return "SterolLipids"

            Case "DCAE"
                Return "SterolLipids"
            Case "GDCAE"
                Return "SterolLipids"
            Case "GLCAE"
                Return "SterolLipids"
            Case "TDCAE"
                Return "SterolLipids"
            Case "TLCAE"
                Return "SterolLipids"
            Case "LCAE"
                Return "SterolLipids"
            Case "KLCAE"
                Return "SterolLipids"
            Case "KDCAE"
                Return "SterolLipids"
            Case "Vitamin_D"
                Return "SterolLipids"
            Case "Vitamin D"
                Return "SterolLipids"
            Case "BileAcid"
                Return "SterolLipids"

            Case "CoQ"
                Return "PrenolLipids"
            Case "Vitamin_E"
                Return "PrenolLipids"
            Case "Vitamin E"
                Return "PrenolLipids"
            Case "VAE"
                Return "PrenolLipids"

            Case "PhytoSph"
                Return "Sphingolipids"
            Case "DHSph"
                Return "Sphingolipids"
            Case "Sph"
                Return "Sphingolipids"

            Case "Cer_ADS"
                Return "Sphingolipids"
            Case "Cer_AS"
                Return "Sphingolipids"
            Case "Cer_BDS"
                Return "Sphingolipids"
            Case "Cer_BS"
                Return "Sphingolipids"
            Case "Cer_NDS"
                Return "Sphingolipids"
            Case "Cer_NS"
                Return "Sphingolipids"
            Case "Cer_NP"
                Return "Sphingolipids"
            Case "Cer_AP"
                Return "Sphingolipids"
            Case "Cer_OS"
                Return "Sphingolipids"
            Case "Cer_HS"
                Return "Sphingolipids"
            Case "Cer_HDS"
                Return "Sphingolipids"
            Case "Cer_NDOS"
                Return "Sphingolipids"
            Case "Cer_EODS"
                Return "Sphingolipids"
            Case "Cer_EOS"
                Return "Sphingolipids"

            Case "Cer-ADS"
                Return "Sphingolipids"
            Case "Cer-AS"
                Return "Sphingolipids"
            Case "Cer-BDS"
                Return "Sphingolipids"
            Case "Cer-BS"
                Return "Sphingolipids"
            Case "Cer-NDS"
                Return "Sphingolipids"
            Case "Cer-NS"
                Return "Sphingolipids"
            Case "Cer-NP"
                Return "Sphingolipids"
            Case "Cer-AP"
                Return "Sphingolipids"
            Case "Cer-OS"
                Return "Sphingolipids"
            Case "Cer-HS"
                Return "Sphingolipids"
            Case "Cer-HDS"
                Return "Sphingolipids"
            Case "Cer-NDOS"
                Return "Sphingolipids"
            Case "CerP"
                Return "Sphingolipids"
            Case "Cer-EODS"
                Return "Sphingolipids"
            Case "Cer-EOS"
                Return "Sphingolipids"

            Case "HexCer-NS"
                Return "Sphingolipids"
            Case "HexCer-NDS"
                Return "Sphingolipids"
            Case "HexCer-AP"
                Return "Sphingolipids"
            Case "HexCer-HS"
                Return "Sphingolipids"
            Case "HexCer-HDS"
                Return "Sphingolipids"
            Case "HexCer-EOS"
                Return "Sphingolipids"
            Case "HexCer_NS"
                Return "Sphingolipids"
            Case "HexCer_NDS"
                Return "Sphingolipids"
            Case "HexCer_AP"
                Return "Sphingolipids"
            Case "HexCer_HS"
                Return "Sphingolipids"
            Case "HexCer_HDS"
                Return "Sphingolipids"
            Case "HexCer_EOS"
                Return "Sphingolipids"
            Case "Hex2Cer"
                Return "Sphingolipids"
            Case "Hex3Cer"
                Return "Sphingolipids"
            Case "Cer_EBDS"
                Return "Sphingolipids"
            Case "Cer-EBDS"
                Return "Sphingolipids"
            Case "AHexCer"
                Return "Sphingolipids"
            Case "ASHexCer"
                Return "Sphingolipids"
            Case "ASM"
                Return "Sphingolipids"

            Case "PI-Cer"
                Return "Sphingolipids"
            Case "PE-Cer"
                Return "Sphingolipids"
            Case "PI_Cer"
                Return "Sphingolipids"
            Case "PE_Cer"
                Return "Sphingolipids"
            Case "PI-Cer+O"
                Return "Sphingolipids"
            Case "PE-Cer+O"
                Return "Sphingolipids"
            Case "PI_Cer+O"
                Return "Sphingolipids"
            Case "PE_Cer+O"
                Return "Sphingolipids"
            Case "MIPC"
                Return "Sphingolipids"

            Case "SM"
                Return "Sphingolipids"
            Case "SHexCer"
                Return "Sphingolipids"
            Case "SL"
                Return "Sphingolipids"
            Case "SM+O"
                Return "Sphingolipids"
            Case "SHexCer+O"
                Return "Sphingolipids"
            Case "SL+O"
                Return "Sphingolipids"
            Case "GM3"
                Return "Sphingolipids"
            Case "GM3[NeuAc]"
                Return "Sphingolipids"

            Case "GPNAE"
                Return "Glycerophospholipids"
            Case "MGMG"
                Return "Glycerolipids"
            Case "DGMG"
                Return "Glycerolipids"

            Case "GD1a"
                Return "Sphingolipids"
            Case "GD1b"
                Return "Sphingolipids"
            Case "GD2"
                Return "Sphingolipids"
            Case "GD3"
                Return "Sphingolipids"
            Case "GM1"
                Return "Sphingolipids"
            Case "GT1b"
                Return "Sphingolipids"
            Case "GQ1b"
                Return "Sphingolipids"
            Case "NGcGM3"
                Return "Sphingolipids"

            Case "LPC_d5"
                Return "Glycerophospholipids"
            Case "LPE_d5"
                Return "Glycerophospholipids"
            Case "LPG_d5"
                Return "Glycerophospholipids"
            Case "LPI_d5"
                Return "Glycerophospholipids"
            Case "LPS_d5"
                Return "Glycerophospholipids"

            Case "PC_d5"
                Return "Glycerophospholipids"
            Case "PE_d5"
                Return "Glycerophospholipids"
            Case "PG_d5"
                Return "Glycerophospholipids"
            Case "PI_d5"
                Return "Glycerophospholipids"
            Case "PS_d5"
                Return "Glycerophospholipids"

            Case "DG_d5"
                Return "Glycerolipids"
            Case "TG_d5"
                Return "Glycerolipids"

            Case "CE_d7"
                Return "SterolLipids"
            Case "Cer_NS_d7"
                Return "Sphingolipids"
            Case "SM_d9"
                Return "Sphingolipids"

            Case "bmPC"
                Return "Glycerophospholipids"
            Case Else
                Return "Unassigned lipid"
        End Select
    End Function
End Module
