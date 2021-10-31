Public Class FoodData : Inherits Food

    Public Property contents As Content()
    Public Property compoundFlavors As Dictionary(Of String, String())
    Public Property compounds As Compound()

    Sub New(food As Food)
        With Me
            .category = food.category
            .description = food.description
            .food_group = food.food_group
            .food_subgroup = food.food_subgroup
            .food_type = food.food_type
            .id = food.id
            .itis_id = food.itis_id
            .legacy_id = food.legacy_id
            .name = food.name
            .name_scientific = food.name_scientific
            .ncbi_taxonomy_id = food.ncbi_taxonomy_id
            .public_id = food.public_id
            .wikipedia_id = food.wikipedia_id
        End With
    End Sub

    Sub New()
    End Sub

End Class
