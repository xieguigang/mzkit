Public Class Food
    Public Property id As String
    Public Property name As String
    Public Property name_scientific As String
    Public Property description As String
    Public Property itis_id As String
    Public Property wikipedia_id As String
    Public Property legacy_id As String
    Public Property food_group As String
    Public Property food_subgroup As String
    Public Property food_type As String
    Public Property category As String
    Public Property ncbi_taxonomy_id As String
    Public Property public_id As String

    Public Overrides Function ToString() As String
        Return $"[{id}] {name}({name_scientific})"
    End Function

End Class
