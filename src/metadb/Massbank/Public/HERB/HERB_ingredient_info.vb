Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace HERB

    Public Class HERB_ingredient_info

        Public Property Ingredient_id As String
        Public Property Ingredient_name As String
        Public Property [Alias] As String
        Public Property Ingredient_formula As String
        Public Property Ingredient_Smile As String
        Public Property Ingredient_weight As String
        Public Property OB_score As String
        Public Property CAS_id As String
        Public Property SymMap_id As String
        Public Property TCMID_id As String
        Public Property TCMSP_id As String
        <Column("TCM-ID_id")>
        Public Property TCM_ID_id As String
        Public Property PubChem_id As String
        Public Property DrugBank_id As String

        Public Overrides Function ToString() As String
            Return $"{Ingredient_name} ({Ingredient_formula})"
        End Function

    End Class
End Namespace