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
        Public Property Molecule
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

        Private Shared Function StreamParser(block$()) As SDF
            Dim ID$ = block(0), class$ = block(1)
            Dim metas = block _
                .Split(Function(s) s = "M  END", includes:=False) _
                .Last

            Return New SDF With {
                .ID = ID.Trim,
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

        Shared ReadOnly properties As Dictionary(Of String, PropertyInfo) =
            DataFramework.Schema(Of MetaData)(PropertyAccess.Writeable, True)

        Friend Shared Function Data(metaData$()) As MetaData
            Dim table As Dictionary(Of String, String) =
                metaData _
                .Split(Function(s) s.StringEmpty, includes:=False) _
                .ToDictionary(Function(t) Mid(t(0), 4, t(0).Length - 5),
                              Function(t) t(1))
            Dim meta As Object = New MetaData

            For Each key As String In table.Keys
                Call properties(key).SetValue(meta, table(key))
            Next

            Return DirectCast(meta, MetaData)
        End Function
    End Class
End Namespace