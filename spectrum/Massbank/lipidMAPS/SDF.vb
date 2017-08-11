Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace LipidMaps

    ''' <summary>
    ''' http://www.lipidmaps.org/resources/downloads/index.html
    ''' </summary>
    Public Class SDF : Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property [Class] As String
        Public Property Molecule As String
        Public Property MetaData As MetaData

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
        Public Property PUBCHEM_SID As String
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