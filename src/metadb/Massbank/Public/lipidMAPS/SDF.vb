#Region "Microsoft.VisualBasic::1cdf82aab90617aef8346539f2498c57, metadb\Massbank\Public\lipidMAPS\SDF.vb"

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

    '   Total Lines: 131
    '    Code Lines: 90 (68.70%)
    ' Comment Lines: 31 (23.66%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (7.63%)
    '     File Size: 7.70 KB


    '     Class MetaData
    ' 
    '         Properties: ABBREVIATION, CATEGORY, CHEBI_ID, CLASS_LEVEL4, COMMON_NAME
    '                     EXACT_MASS, FORMULA, HMDB_ID, HMDBID, INCHI
    '                     INCHI_KEY, KEGG_ID, LIPID_MAPS_CMPD_URL, LIPIDBANK_ID, LM_ID
    '                     MAIN_CLASS, METABOLOMICS_ID, NAME, PLANTFA_ID, PUBCHEM_CID
    '                     PUBCHEM_SID, PUBCHEM_SUBSTANCE_URL, SMILES, STATUS, SUB_CLASS
    '                     SWISSLIPIDS_ID, SYNONYMS, SYSTEMATIC_NAME
    ' 
    '         Function: Data, EqualsAny, GetAnnotation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace LipidMaps

    ''' <summary>
    ''' lipidmaps annotation data model, which is original extract from the SDF data objects.
    ''' </summary>
    Public Class MetaData : Implements IExactMassProvider, IReadOnlyId, ICompoundNameProvider, IFormulaProvider ' , ICompoundClass

        ''' <summary>
        ''' The lipidmaps unique reference id
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0, NilImplication:=NilImplication.MemberDefault)> Public Property LM_ID As String Implements IReadOnlyId.Identity
        <MessagePackMember(1, NilImplication:=NilImplication.MemberDefault)> Public Property NAME As String Implements ICompoundNameProvider.CommonName
        <MessagePackMember(2, NilImplication:=NilImplication.MemberDefault)> Public Property PUBCHEM_SUBSTANCE_URL As String
        <MessagePackMember(3, NilImplication:=NilImplication.MemberDefault)> Public Property LIPID_MAPS_CMPD_URL As String
        <MessagePackMember(4, NilImplication:=NilImplication.MemberDefault)> Public Property PLANTFA_ID As String
        <MessagePackMember(5, NilImplication:=NilImplication.MemberDefault)> Public Property COMMON_NAME As String
        <MessagePackMember(6, NilImplication:=NilImplication.MemberDefault)> Public Property SYSTEMATIC_NAME As String
        <MessagePackMember(7, NilImplication:=NilImplication.MemberDefault)> Public Property SYNONYMS As String()
        <MessagePackMember(8, NilImplication:=NilImplication.MemberDefault)> Public Property ABBREVIATION As String
        <MessagePackMember(9, NilImplication:=NilImplication.MemberDefault)> Public Property CATEGORY As String 'Implements ICompoundClass.kingdom
        <MessagePackMember(10, NilImplication:=NilImplication.MemberDefault)> Public Property MAIN_CLASS As String 'Implements ICompoundClass.super_class
        <MessagePackMember(11, NilImplication:=NilImplication.MemberDefault)> Public Property SUB_CLASS As String 'Implements ICompoundClass.class
        <MessagePackMember(12, NilImplication:=NilImplication.MemberDefault)> Public Property EXACT_MASS As Double Implements IExactMassProvider.ExactMass
        <MessagePackMember(13, NilImplication:=NilImplication.MemberDefault)> Public Property FORMULA As String Implements IFormulaProvider.Formula
        <MessagePackMember(14, NilImplication:=NilImplication.MemberDefault)> Public Property LIPIDBANK_ID As String
        <MessagePackMember(15, NilImplication:=NilImplication.MemberDefault)> Public Property SWISSLIPIDS_ID As String
        <MessagePackMember(16, NilImplication:=NilImplication.MemberDefault)> Public Property HMDB_ID As String
        ''' <summary>
        ''' PubChem Substance accession identifier (SID)
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(17, NilImplication:=NilImplication.MemberDefault)> Public Property PUBCHEM_SID As String
        ''' <summary>
        ''' PubChem Compound accession identifier (CID)
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(18, NilImplication:=NilImplication.MemberDefault)> Public Property PUBCHEM_CID As String
        <MessagePackMember(19, NilImplication:=NilImplication.MemberDefault)> Public Property KEGG_ID As String
        <MessagePackMember(20, NilImplication:=NilImplication.MemberDefault)> Public Property HMDBID As String
        <MessagePackMember(21, NilImplication:=NilImplication.MemberDefault)> Public Property CHEBI_ID As String
        <MessagePackMember(22, NilImplication:=NilImplication.MemberDefault)> Public Property INCHI_KEY As String
        <MessagePackMember(23, NilImplication:=NilImplication.MemberDefault)> Public Property INCHI As String
        <MessagePackMember(24, NilImplication:=NilImplication.MemberDefault)> Public Property SMILES As String
        <MessagePackMember(25, NilImplication:=NilImplication.MemberDefault)> Public Property STATUS As String
        <MessagePackMember(26, NilImplication:=NilImplication.MemberDefault)> Public Property CLASS_LEVEL4 As String 'Implements ICompoundClass.sub_class
        <MessagePackMember(27, NilImplication:=NilImplication.MemberDefault)> Public Property METABOLOMICS_ID As String

        ''' <summary>
        ''' extract annotation information from the SDF metadata object
        ''' </summary>
        ''' <param name="sdf"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Data(sdf As SDF) As MetaData
            Dim obj As MetaData = sdf.Data(Of MetaData)(properties)
            obj.SYNONYMS = obj.SYNONYMS.ElementAtOrDefault(Scan0).StringSplit(";\s+")
            Return obj
        End Function

        ''' <summary>
        ''' 只要任意一个编号对象相等，就认为两个对象是同一种物质？
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="systematicName"></param>
        ''' <param name="kegg"></param>
        ''' <param name="chebi"></param>
        ''' <param name="hmdb"></param>
        ''' <param name="inchiKey"></param>
        ''' <param name="inchi"></param>
        ''' <param name="metabolomicsID"></param>
        ''' <returns></returns>
        Public Function EqualsAny(Optional name$ = Nothing,
                                  Optional systematicName$ = Nothing,
                                  Optional kegg$ = Nothing,
                                  Optional chebi$ = Nothing,
                                  Optional hmdb$ = Nothing,
                                  Optional inchiKey$ = Nothing,
                                  Optional inchi$ = Nothing,
                                  Optional metabolomicsID$ = Nothing) As Boolean

            If Not name.StringEmpty AndAlso name.TextEquals(COMMON_NAME) Then
                Return True
            End If
            If Not systematicName.StringEmpty AndAlso SYSTEMATIC_NAME.TextEquals(systematicName) Then
                Return True
            End If
            If Not kegg.StringEmpty AndAlso KEGG_ID.TextEquals(kegg) Then
                Return True
            End If
            If Not chebi.StringEmpty AndAlso CHEBI_ID.Split(":"c).Last = chebi.Split(":"c).Last Then
                Return True
            End If
            If Not hmdb.StringEmpty AndAlso hmdb.TextEquals(HMDBID) Then
                Return True
            End If
            If Not inchiKey.StringEmpty AndAlso inchiKey.TextEquals(INCHI_KEY) Then
                Return True
            End If
            If Not inchi.StringEmpty AndAlso inchi.TextEquals(Me.INCHI) Then
                Return True
            End If
            If Not metabolomicsID.StringEmpty AndAlso metabolomicsID.TextEquals(METABOLOMICS_ID) Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' <see cref="COMMON_NAME"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"[{CATEGORY}] {SYSTEMATIC_NAME}"
        End Function

        Public Function GetAnnotation() As MetaboliteAnnotation
            Return New MetaboliteAnnotation With {
                .CommonName = NAME,
                .ExactMass = FormulaScanner.EvaluateExactMass(FORMULA),
                .Formula = FORMULA,
                .Id = Me.LM_ID
            }
        End Function
    End Class
End Namespace
