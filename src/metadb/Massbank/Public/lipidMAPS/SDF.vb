#Region "Microsoft.VisualBasic::023ae9545fd66a40006c68e60f1a22d2, src\metadb\Massbank\Public\lipidMAPS\SDF.vb"

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

'     Class MetaData
' 
'         Properties: CATEGORY, CHEBI_ID, CLASS_LEVEL4, COMMON_NAME, EXACT_MASS
'                     FORMULA, HMDBID, INCHI, INCHI_KEY, KEGG_ID
'                     LIPID_MAPS_CMPD_URL, LIPIDBANK_ID, LM_ID, MAIN_CLASS, METABOLOMICS_ID
'                     PUBCHEM_CID, PUBCHEM_SID, PUBCHEM_SUBSTANCE_URL, STATUS, SUB_CLASS
'                     SYNONYMS, SYSTEMATIC_NAME
' 
'         Function: Data, EqualsAny, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports BioNovoGene.BioDeep.Chemistry.File
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace LipidMaps

    ''' <summary>
    ''' 物质的注释信息
    ''' </summary>
    Public Class MetaData

        Public Property PUBCHEM_SUBSTANCE_URL As String
        Public Property LIPID_MAPS_CMPD_URL As String
        Public Property LM_ID As String
        Public Property COMMON_NAME As String
        Public Property SYSTEMATIC_NAME As String
        Public Property SYNONYMS As String
        Public Property CATEGORY As String
        Public Property MAIN_CLASS As String
        Public Property SUB_CLASS As String
        Public Property EXACT_MASS As String
        Public Property FORMULA As String
        Public Property LIPIDBANK_ID As String
        ''' <summary>
        ''' PubChem Substance accession identifier (SID)
        ''' </summary>
        ''' <returns></returns>
        Public Property PUBCHEM_SID As String
        ''' <summary>
        ''' PubChem Compound accession identifier (CID)
        ''' </summary>
        ''' <returns></returns>
        Public Property PUBCHEM_CID As String
        Public Property KEGG_ID As String
        Public Property HMDBID As String
        Public Property CHEBI_ID As String
        Public Property INCHI_KEY As String
        Public Property INCHI As String
        Public Property STATUS As String
        Public Property CLASS_LEVEL4 As String
        Public Property METABOLOMICS_ID As String

        Shared ReadOnly properties As Dictionary(Of String, PropertyInfo) =
            DataFramework.Schema(Of MetaData)(PropertyAccess.Writeable, True)

        Public Shared Function Data(sdf As SDF) As MetaData
            Return sdf.Data(Of MetaData)(properties)
        End Function

        ''' <summary>
        ''' 只要任意一个编号对象相等，就认为两个对象是同一种物质？
        ''' </summary>
        ''' <param name="name$"></param>
        ''' <param name="systematicName$"></param>
        ''' <param name="kegg$"></param>
        ''' <param name="chebi$"></param>
        ''' <param name="hmdb$"></param>
        ''' <param name="inchiKey$"></param>
        ''' <param name="inchi$"></param>
        ''' <param name="metabolomicsID$"></param>
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
            Return COMMON_NAME
        End Function
    End Class
End Namespace
