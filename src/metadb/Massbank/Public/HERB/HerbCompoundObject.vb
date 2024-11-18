Namespace HERB

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class HerbCompoundObject : Inherits HERB_ingredient_info

        Public Property reference As HERB_reference_info()

        Sub New()
        End Sub

        Sub New(base As HERB_ingredient_info)
            Ingredient_id = base.Ingredient_id
            Ingredient_name = base.Ingredient_name
            [Alias] = base.Alias
            Ingredient_formula = base.Ingredient_formula
            Ingredient_Smile = base.Ingredient_Smile
            Ingredient_weight = base.Ingredient_weight
            OB_score = base.OB_score
            CAS_id = base.CAS_id
            SymMap_id = base.SymMap_id
            TCMID_id = base.TCMID_id
            TCMSP_id = base.TCMSP_id
            TCM_ID_id = base.TCM_ID_id
            PubChem_id = base.PubChem_id
            DrugBank_id = base.DrugBank_id
        End Sub

    End Class
End Namespace