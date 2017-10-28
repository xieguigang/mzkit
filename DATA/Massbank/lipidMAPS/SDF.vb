Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

Namespace LipidMaps

    ''' <summary>
    ''' http://www.lipidmaps.org/resources/downloads/index.html
    ''' </summary>
    Public Class SDF
        Implements INamedValue
        Implements Value(Of MetaData).IValueOf

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property [Class] As String
        Public Property Molecule As String
        Public Property MetaData As MetaData Implements Value(Of MetaData).IValueOf.Value

        Public Shared Iterator Function IterateParser(path$) As IEnumerable(Of SDF)
            Dim o As SDF

            For Each block As String() In path _
                .IterateAllLines _
                .Split(Function(s) s = "$$$$", includes:=False)

                o = SDF.StreamParser(block)
                Yield o
            Next
        End Function

        Const molEnds$ = "M  END"

        Private Shared Function StreamParser(block$()) As SDF
            Dim ID$ = block(0), class$ = block(1)
            Dim metas$()
            Dim mol$

            With block _
                .Skip(2) _
                .Split(Function(s) s = molEnds, includes:=False)

                metas = .Last
                mol = .First.Join({molEnds}).JoinBy(vbLf)
            End With

            Return New SDF With {
                .ID = ID.Trim,
                .Molecule = mol,
                .Class = [class].Trim,
                .MetaData = MetaData.Data(metas)
            }
        End Function
    End Class

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

        Friend Shared Function Data(metaData$()) As MetaData
            Dim table As Dictionary(Of String, String) =
                metaData _
                .Split(Function(s) s.StringEmpty, includes:=False) _
                .Where(Function(t) Not t.IsNullOrEmpty) _
                .ToDictionary(Function(t) Mid(t(0), 4, t(0).Length - 4),
                              Function(t) If(t.Length = 1, "", t(1)))
            Dim meta As Object = New MetaData

            For Each key As String In table.Keys
                Call properties(key).SetValue(meta, table(key))
            Next

            Return DirectCast(meta, MetaData)
        End Function
    End Class
End Namespace