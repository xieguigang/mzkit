#Region "Microsoft.VisualBasic::a900265cd235b2b820d100e5ee685623, G:/mzkit/src/metadb/Lipidomics//LipoqualityDatabaseManagerUtility.vb"

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

    '   Total Lines: 802
    '    Code Lines: 624
    ' Comment Lines: 80
    '   Blank Lines: 98
    '     File Size: 32.34 KB


    ' Class LipoqualityDatabaseManagerUtility
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ConvertMsdialClassDefinitionToSuperClass, ConvertMsdialClassDefinitionToTraditionalClassDefinition, ConvertMsdialLipidnameToLipidAnnotation, getLipidChainsInformation
    ' 
    '     Sub: setDoubleAcylChainsLipidAnnotation, setQuadAcylChainsLipidAnnotation, setSingleAcylChainsLipidAnnotation, setTripleAcylChainsLipidAnnotation
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.MSEngine

Public NotInheritable Class LipoqualityDatabaseManagerUtility
    Private Sub New()
    End Sub

    Public Shared Function ConvertMsdialLipidnameToLipidAnnotation(query As MoleculeMsReference, metaboliteName As String) As LipoqualityAnnotation
        Dim lipidannotation = New LipoqualityAnnotation()

        Select Case query.CompoundClass

                'Glycerolipid
            Case "MAG"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "DAG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "TAG"
                setTripleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Lyso phospholipid
            Case "LPC"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LPE"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LPG"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LPI"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LPS"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LPA"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LDGTS"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LDGTA"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "LPC_d5", "LPE_d5", "LPG_d5", "LPI_d5", "LPS_d5"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Phospholipid
            Case "PC"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PE"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PI"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PA"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PC_d5", "PE_d5", "PG_d5", "PI_d5", "PS_d5"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "bmPC"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "BMP"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "HBMP"
                setTripleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "CL"
                setQuadAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Ether linked phospholipid
            Case "EtherPC"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "EtherPE"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Oxidized phospholipid
            Case "OxPC"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "OxPE"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "OxPG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "OxPI"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "OxPS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)


                'Oxidized ether linked phospholipid
            Case "EtherOxPC"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "EtherOxPE"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Oxidized ether linked phospholipid
            Case "PMeOH"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PEtOH"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "PBtOH"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Plantlipid
            Case "MGDG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "DGDG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "SQDG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "DGTS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "DGTA"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "GlcADG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "AcylGlcADG"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Others
            Case "CE", "CE_d7"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "ACar"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "FA", "DMEDFA", "OxFA", "DMEDOxFA"
                setSingleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "FAHFA", "DMEDFAHFA"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Sphingomyelin
            Case "SM", "SM_d9"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

                'Ceramide
            Case "Cer_ADS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_AS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_BDS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_BS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_EODS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_EOS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_NDS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_NS", "Cer_NS_d7"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_NP"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "GlcCer_NS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "GlcCer_NDS"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "Cer_AP"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "GlcCer_AP"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

            Case "SHexCer"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)
            Case "GM3"
                setDoubleAcylChainsLipidAnnotation(lipidannotation, query, metaboliteName)

        End Select
        Return lipidannotation
    End Function

    'private static void setSingleAcylChainslipidAnnotation(LipidAnnotation lipidannotation, MspFormatCompoundInformationBean query)
    '{
    '    var name = query.Name;
    '    var nameArray = query.Name.Split(';').ToArray();

    '    var lipidinfo = nameArray[0].Trim();

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass);
    '    var totalChain = lipidinfo.Split(' ')[1]; if (query.CompoundClass == "CE") totalChain = lipidinfo.Split(' ')[0];
    '    var sn1AcylChain = lipidinfo.Split(' ')[1]; if (query.CompoundClass == "CE") sn1AcylChain = lipidinfo.Split(' ')[0];

    '    lipidannotation.Name = lipidinfo;
    '    lipidannotation.IonMode = query.IonMode;
    '    lipidannotation.LipidSuperClass = lipidSuperClass;
    '    lipidannotation.LipidClass = lipidclass;
    '    lipidannotation.Adduct = AdductIonParcer.GetAdductIonBean(query.AdductIonBean.AdductIonName);
    '    lipidannotation.Sn1AcylChain = sn1AcylChain;
    '    lipidannotation.Smiles = query.Smiles;
    '    lipidannotation.Inchikey = query.InchiKey;
    '    lipidannotation.Formula = query.Formula;
    '}

    Private Shared Sub setSingleAcylChainsLipidAnnotation(lipidannotation As LipoqualityAnnotation, query As MoleculeMsReference, metabolitename As String)
        Dim name = metabolitename
        Dim nameArray = metabolitename.Split(";"c).ToArray()

        Dim lipidinfo = nameArray(0).Trim()

        Dim lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass)
        Dim lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass)
        Dim totalChain = lipidinfo.Split(" "c)(1)
        Dim sn1AcylChain = lipidinfo.Split(" "c)(1)

        lipidannotation.Name = lipidinfo
        lipidannotation.IonMode = query.IonMode
        lipidannotation.LipidSuperClass = lipidSuperClass
        lipidannotation.LipidClass = lipidclass
        lipidannotation.Adduct = query.AdductType
        lipidannotation.TotalChain = totalChain
        lipidannotation.Sn1AcylChain = sn1AcylChain
        lipidannotation.Smiles = query.SMILES
        lipidannotation.Inchikey = query.InChIKey
        lipidannotation.Formula = query.Formula.EmpiricalFormula
    End Sub

    Private Shared Sub setDoubleAcylChainsLipidAnnotation(lipidannotation As LipoqualityAnnotation, query As MoleculeMsReference, metabolitename As String)

        Dim nameArray = metabolitename.Split(";"c).ToArray()
        Dim lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass)
        Dim lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass)
        Dim totalLipidInfo = nameArray(0).Trim()
        Dim totalChain = totalLipidInfo.Split(" "c)(1)

        If nameArray.Length = 2 Then
            Dim adductInfo = nameArray(1).Trim()
            lipidannotation.Name = totalLipidInfo
            lipidannotation.IonMode = query.IonMode
            lipidannotation.LipidSuperClass = lipidSuperClass
            lipidannotation.LipidClass = lipidclass
            lipidannotation.Adduct = query.AdductType
            lipidannotation.TotalChain = totalChain
            lipidannotation.Smiles = query.SMILES
            lipidannotation.Inchikey = query.InChIKey
            lipidannotation.Formula = query.Formula.EmpiricalFormula

        ElseIf nameArray.Length = 3 Then

            Dim detailLipidInfo = nameArray(1).Trim()
            Dim adductInfo = nameArray(2).Trim()

            Dim chainsInfo = getLipidChainsInformation(detailLipidInfo)
            Dim sn1AcylChain = chainsInfo(0)
            Dim sn2AcylChain = chainsInfo(1)

            lipidannotation.Name = detailLipidInfo
            lipidannotation.IonMode = query.IonMode
            lipidannotation.LipidSuperClass = lipidSuperClass
            lipidannotation.LipidClass = lipidclass
            lipidannotation.Adduct = query.AdductType
            lipidannotation.TotalChain = totalChain
            lipidannotation.Sn1AcylChain = sn1AcylChain
            lipidannotation.Sn2AcylChain = sn2AcylChain
            lipidannotation.Smiles = query.SMILES
            lipidannotation.Inchikey = query.InChIKey
            lipidannotation.Formula = query.Formula.EmpiricalFormula
        End If
    End Sub

    Private Shared Sub setTripleAcylChainsLipidAnnotation(lipidannotation As LipoqualityAnnotation, query As MoleculeMsReference, metabolitename As String)

        Dim nameArray = metabolitename.Split(";"c).ToArray()
        Dim totalLipidInfo = nameArray(0).Trim()
        Dim lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass)
        Dim lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass)
        Dim totalChain = totalLipidInfo.Split(" "c)(1)

        If nameArray.Length = 2 Then
            Dim adductInfo = nameArray(1).Trim()

            lipidannotation.Name = totalLipidInfo
            lipidannotation.IonMode = query.IonMode
            lipidannotation.LipidSuperClass = lipidSuperClass
            lipidannotation.LipidClass = lipidclass
            lipidannotation.Adduct = query.AdductType
            lipidannotation.TotalChain = totalChain
            lipidannotation.Smiles = query.SMILES
            lipidannotation.Inchikey = query.InChIKey
            lipidannotation.Formula = query.Formula.EmpiricalFormula
        ElseIf nameArray.Length = 3 Then

            Dim detailLipidInfo = nameArray(1).Trim()
            Dim adductInfo = nameArray(2).Trim()

            Dim chainsInfo = getLipidChainsInformation(detailLipidInfo)
            Dim sn1AcylChain = chainsInfo(0)
            Dim sn2AcylChain = chainsInfo(1)
            Dim sn3AcylChain = chainsInfo(2)

            lipidannotation.Name = detailLipidInfo
            lipidannotation.IonMode = query.IonMode
            lipidannotation.LipidSuperClass = lipidSuperClass
            lipidannotation.LipidClass = lipidclass
            lipidannotation.Adduct = query.AdductType
            lipidannotation.TotalChain = totalChain
            lipidannotation.Sn1AcylChain = sn1AcylChain
            lipidannotation.Sn2AcylChain = sn2AcylChain
            lipidannotation.Sn3AcylChain = sn3AcylChain
            lipidannotation.Smiles = query.SMILES
            lipidannotation.Inchikey = query.InChIKey
            lipidannotation.Formula = query.Formula.EmpiricalFormula
        End If
    End Sub

    Private Shared Sub setQuadAcylChainsLipidAnnotation(lipidannotation As LipoqualityAnnotation, query As MoleculeMsReference, metabolitename As String)

        Dim nameArray = metabolitename.Split(";"c).ToArray()
        Dim totalLipidInfo = nameArray(0).Trim()
        Dim lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass)
        Dim lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass)
        Dim totalChain = totalLipidInfo.Split(" "c)(1)

        If nameArray.Length = 2 Then
            Dim adductInfo = nameArray(1).Trim()

            lipidannotation.Name = totalLipidInfo
            lipidannotation.IonMode = query.IonMode
            lipidannotation.LipidSuperClass = lipidSuperClass
            lipidannotation.LipidClass = lipidclass
            lipidannotation.Adduct = query.AdductType
            lipidannotation.TotalChain = totalChain
            lipidannotation.Smiles = query.SMILES
            lipidannotation.Inchikey = query.InChIKey
            lipidannotation.Formula = query.Formula.EmpiricalFormula
        ElseIf nameArray.Length = 3 Then

            Dim detailLipidInfo = nameArray(1).Trim()
            Dim adductInfo = nameArray(2).Trim()

            Dim chainsInfo = getLipidChainsInformation(detailLipidInfo)
            Dim sn1AcylChain = chainsInfo(0)
            Dim sn2AcylChain = chainsInfo(1)
            Dim sn3AcylChain = chainsInfo(2)
            Dim sn4AcylChain = chainsInfo(3)

            lipidannotation.Name = detailLipidInfo
            lipidannotation.IonMode = query.IonMode
            lipidannotation.LipidSuperClass = lipidSuperClass
            lipidannotation.LipidClass = lipidclass
            lipidannotation.Adduct = query.AdductType
            lipidannotation.TotalChain = totalChain
            lipidannotation.Sn1AcylChain = sn1AcylChain
            lipidannotation.Sn2AcylChain = sn2AcylChain
            lipidannotation.Sn3AcylChain = sn3AcylChain
            lipidannotation.Sn4AcylChain = sn4AcylChain
            lipidannotation.Smiles = query.SMILES
            lipidannotation.Inchikey = query.InChIKey
            lipidannotation.Formula = query.Formula.EmpiricalFormula
        End If
    End Sub


    'private static void setDoubleAcylChainslipidAnnotation(LipidAnnotation lipidannotation, 
    '    MspFormatCompoundInformationBean query)
    '{
    '    var nameArray = query.Name.Split(';').ToArray();

    '    var totalLipidInfo = nameArray[0].Trim();
    '    var detailLipidInfo = nameArray[1].Trim();
    '    var adductInfo = nameArray[2].Trim();

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass);
    '    var totalChain = totalLipidInfo.Split(' ')[1];

    '    var chainsInfo = getLipidChainsInformation(detailLipidInfo);
    '    var sn1AcylChain = chainsInfo[0];
    '    var sn2AcylChain = chainsInfo[1];
    '    if (sn1AcylChain.Contains("P-")) sn1AcylChain = sn1AcylChain.Replace("P-", "e").Replace(":0", ":1");

    '    lipidannotation.Name = detailLipidInfo;
    '    lipidannotation.IonMode = query.IonMode;
    '    lipidannotation.LipidSuperClass = lipidSuperClass;
    '    lipidannotation.LipidClass = lipidclass;
    '    lipidannotation.Adduct = AdductIonParcer.GetAdductIonBean(query.AdductIonBean.AdductIonName);
    '    lipidannotation.Sn1AcylChain = sn1AcylChain;
    '    lipidannotation.Sn2AcylChain = sn2AcylChain;
    '    lipidannotation.Smiles = query.Smiles;
    '    lipidannotation.Inchikey = query.InchiKey;
    '    lipidannotation.Formula = query.Formula;
    '}

    'private static void setTripleAcylChainslipidAnnotation(LipidAnnotation lipidannotation, MspFormatCompoundInformationBean query)
    '{
    '    var nameArray = query.Name.Split(';').ToArray();

    '    var totalLipidInfo = nameArray[0].Trim();
    '    var detailLipidInfo = nameArray[1].Trim();
    '    var adductInfo = nameArray[2].Trim();

    '    var lipidSuperClass = ConvertMsdialClassDefinitionToSuperClass(query.CompoundClass);
    '    var lipidclass = ConvertMsdialClassDefinitionToTraditionalClassDefinition(query.CompoundClass);
    '    var totalChain = totalLipidInfo.Split(' ')[1];

    '    var chainsInfo = getLipidChainsInformation(detailLipidInfo);
    '    var sn1AcylChain = chainsInfo[0];
    '    var sn2AcylChain = chainsInfo[1];
    '    var sn3AcylChain = chainsInfo[2];

    '    lipidannotation.Name = detailLipidInfo;
    '    lipidannotation.IonMode = query.IonMode;
    '    lipidannotation.LipidSuperClass = lipidSuperClass;
    '    lipidannotation.LipidClass = lipidclass;
    '    lipidannotation.Adduct = AdductIonParcer.GetAdductIonBean(query.AdductIonBean.AdductIonName);
    '    lipidannotation.Sn1AcylChain = sn1AcylChain;
    '    lipidannotation.Sn2AcylChain = sn2AcylChain;
    '    lipidannotation.Sn3AcylChain = sn3AcylChain;
    '    lipidannotation.Smiles = query.Smiles;
    '    lipidannotation.Inchikey = query.InchiKey;
    '    lipidannotation.Formula = query.Formula;
    '}

    Private Shared Function getLipidChainsInformation(detailLipidInfo As String) As List(Of String)
        Dim chains = New List(Of String)()
        Dim acylArray As String() = Nothing

        If detailLipidInfo.Contains("(") AndAlso Not detailLipidInfo.Contains("Cyc") Then

            If detailLipidInfo.Contains("/") Then
                acylArray = detailLipidInfo.Split("("c)(1).Split(")"c)(0).Split("/"c)
            Else
                acylArray = detailLipidInfo.Split("("c)(1).Split(")"c)(0).Split("-"c)
            End If
        Else
            If detailLipidInfo.Contains("/") Then
                acylArray = detailLipidInfo.Split(" "c)(1).Split("/"c)
            Else
                acylArray = detailLipidInfo.Split(" "c)(1).Split("-"c)
            End If
        End If

        For i = 0 To acylArray.Length - 1
            If i = 0 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
            If i = 1 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
            If i = 2 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
            If i = 3 AndAlso Not Equals(acylArray(i), String.Empty) Then chains.Add(acylArray(i))
        Next
        Return chains
    End Function

    Public Shared Function ConvertMsdialClassDefinitionToTraditionalClassDefinition(lipidclass As String) As String
        Select Case lipidclass
            Case "MAG"
                Return "MAG"
            Case "DAG"
                Return "DAG"
            Case "TAG"
                Return "TAG"

            Case "LPC"
                Return "LPC"
            Case "LPA"
                Return "LPA"
            Case "LPE"
                Return "LPE"
            Case "LPG"
                Return "LPG"
            Case "LPI"
                Return "LPI"
            Case "LPS"
                Return "LPS"
            Case "LDGTS"
                Return "LDGTS"
            Case "LDGTA"
                Return "LDGTA"

            Case "PC"
                Return "PC"
            Case "PA"
                Return "PA"
            Case "PE"
                Return "PE"
            Case "PG"
                Return "PG"
            Case "PI"
                Return "PI"
            Case "PS"
                Return "PS"
            Case "BMP"
                Return "BMP"
            Case "HBMP"
                Return "HBMP"
            Case "CL"
                Return "CL"

            Case "EtherPC"
                Return "EtherPC"
            Case "EtherPE"
                Return "EtherPE"

            Case "OxPC"
                Return "OxPC"
            Case "OxPE"
                Return "OxPE"
            Case "OxPG"
                Return "OxPG"
            Case "OxPI"
                Return "OxPI"
            Case "OxPS"
                Return "OxPS"

            Case "EtherOxPC"
                Return "EtherOxPC"
            Case "EtherOxPE"
                Return "EtherOxPE"

            Case "PMeOH"
                Return "PMeOH"
            Case "PEtOH"
                Return "PEtOH"
            Case "PBtOH"
                Return "PBtOH"

            Case "DGDG"
                Return "DGDG"
            Case "MGDG"
                Return "MGDG"
            Case "SQDG"
                Return "SQDG"
            Case "DGTS"
                Return "DGTS"
            Case "DGTA"
                Return "DGTA"
            Case "GlcADG"
                Return "GlcADG"
            Case "AcylGlcADG"
                Return "AcylGlcADG"

            Case "CE"
                Return "CE"
            Case "ACar"
                Return "ACar"
            Case "FA"
                Return "FA"
            Case "OxFA"
                Return "OxFA"
            Case "FAHFA"
                Return "FAHFA"
            Case "DMEDFA"
                Return "DMEDFA"
            Case "DMEDOxFA"
                Return "DMEDOxFA"
            Case "DMEDFAHFA"
                Return "DMEDFAHFA"

            Case "Cer_ADS"
                Return "Cer-ADS"
            Case "Cer_AS"
                Return "Cer-AS"
            Case "Cer_BDS"
                Return "Cer-BDS"
            Case "Cer_BS"
                Return "Cer-BS"
            Case "Cer_NDS"
                Return "Cer-NDS"
            Case "Cer_NS"
                Return "Cer-NS"
            Case "Cer_NP"
                Return "Cer-NP"
            Case "Cer_AP"
                Return "Cer-AP"
            Case "Cer_EODS"
                Return "Cer-EODS"
            Case "Cer_EOS"
                Return "Cer-EOS"

            Case "GlcCer_NS"
                Return "HexCer-NS"
            Case "GlcCer_NDS"
                Return "HexCer-NDS"
            Case "GlcCer_AP"
                Return "HexCer-AP"

            Case "SM"
                Return "SM"

            Case "SHexCer"
                Return "SHexCer"
            Case "GM3"
                Return "GM3"
            Case "GM3[NeuAc]"
                Return "GM3"

            Case "LPC_d5"
                Return "LPC_d5"
            Case "LPE_d5"
                Return "LPE_d5"
            Case "LPG_d5"
                Return "LPG_d5"
            Case "LPI_d5"
                Return "LPI_d5"
            Case "LPS_d5"
                Return "LPS_d5"

            Case "PC_d5"
                Return "PC_d5"
            Case "PE_d5"
                Return "PE_d5"
            Case "PG_d5"
                Return "PG_d5"
            Case "PI_d5"
                Return "PI_d5"
            Case "PS_d5"
                Return "PS_d5"

            Case "DG_d5"
                Return "DG_d5"
            Case "TG_d5"
                Return "TG_d5"

            Case "CE_d7"
                Return "CE_d7"
            Case "Cer_NS_d7"
                Return "Cer_NS_d7"
            Case "SM_d9"
                Return "SM_d9"

            Case "bmPC"
                Return "bmPC"
            Case Else
                Return "Unassigned lipid"
        End Select
    End Function

    Public Shared Function ConvertMsdialClassDefinitionToSuperClass(lipidclass As String) As String
        Select Case lipidclass
            Case "MAG"
                Return "Glycerolipid"
            Case "DAG"
                Return "Glycerolipid"
            Case "TAG"
                Return "Glycerolipid"

            Case "LPC"
                Return "Lyso phospholipid"
            Case "LPA"
                Return "Lyso phospholipid"
            Case "LPE"
                Return "Lyso phospholipid"
            Case "LPG"
                Return "Lyso phospholipid"
            Case "LPI"
                Return "Lyso phospholipid"
            Case "LPS"
                Return "Lyso phospholipid"
            Case "LDGTS"
                Return "Lyso algal lipid"
            Case "LDGTA"
                Return "Lyso algal lipid"

            Case "PC"
                Return "Phospholipid"
            Case "PA"
                Return "Phospholipid"
            Case "PE"
                Return "Phospholipid"
            Case "PG"
                Return "Phospholipid"
            Case "PI"
                Return "Phospholipid"
            Case "PS"
                Return "Phospholipid"
            Case "BMP"
                Return "Phospholipid"
            Case "HBMP"
                Return "Phospholipid"
            Case "CL"
                Return "Phospholipid"

            Case "EtherPC"
                Return "Ether linked phospholipid"
            Case "EtherPE"
                Return "Ether linked phospholipid"

            Case "OxPC"
                Return "Oxidized phospholipid"
            Case "OxPE"
                Return "Oxidized phospholipid"
            Case "OxPG"
                Return "Oxidized phospholipid"
            Case "OxPI"
                Return "Oxidized phospholipid"
            Case "OxPS"
                Return "Oxidized phospholipid"

            Case "EtherOxPC"
                Return "Oxidized ether linked phospholipid"
            Case "EtherOxPE"
                Return "Oxidized ether linked phospholipid"

            Case "PMeOH"
                Return "Header modified phospholipid"
            Case "PEtOH"
                Return "Header modified phospholipid"
            Case "PBtOH"
                Return "Header modified phospholipid"

            Case "DGDG"
                Return "Plant lipid"
            Case "MGDG"
                Return "Plant lipid"
            Case "SQDG"
                Return "Plant lipid"
            Case "DGTS"
                Return "Algal lipid"
            Case "DGTA"
                Return "Algal lipid"
            Case "GlcADG"
                Return "Plant lipid"
            Case "AcylGlcADG"
                Return "Plant lipid"

            Case "CE"
                Return "Cholesterol ester"
            Case "ACar"
                Return "Acyl carnitine"
            Case "FA"
                Return "Free fatty acid"
            Case "OxFA"
                Return "Free fatty acid"
            Case "FAHFA"
                Return "Fatty acid ester of hydroxyl fatty acid"
            Case "DMEDFAHFA"
                Return "Fatty acid ester of hydroxyl fatty acid"
            Case "DMEDFA"
                Return "Free fatty acid"
            Case "DMEDOxFA"
                Return "Free fatty acid"

            Case "Cer_ADS"
                Return "Ceramide"
            Case "Cer_AS"
                Return "Ceramide"
            Case "Cer_BDS"
                Return "Ceramide"
            Case "Cer_BS"
                Return "Ceramide"
            Case "Cer_NDS"
                Return "Ceramide"
            Case "Cer_NS"
                Return "Ceramide"
            Case "Cer_NP"
                Return "Ceramide"
            Case "Cer_AP"
                Return "Ceramide"

            Case "Cer_EODS"
                Return "Acyl ceramide"
            Case "Cer_EOS"
                Return "Acyl ceramide"

            Case "GlcCer_NS"
                Return "Hexosyl ceramide"
            Case "GlcCer_NDS"
                Return "Hexosyl ceramide"
            Case "GlcCer_AP"
                Return "Hexosyl ceramide"

            Case "SM"
                Return "Sphingomyelin"
            Case "SHexCer"
                Return "Sulfatide"
            Case "GM3"
                Return "Ganglioside"
            Case "GM3[NeuAc]"
                Return "Ganglioside"

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
End Class

