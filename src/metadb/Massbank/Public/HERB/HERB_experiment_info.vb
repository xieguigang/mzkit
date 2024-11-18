Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace HERB

    Public Class HERB_experiment_info

        Public Property EXP_id As String
        <Column("Herb/ingredient_id")> Public Property Herb_ingredient_id As String
        <Column("Herb/ingredient_name")> Public Property Herb_ingredient_name As String
        <Column("Herb/ingredient")> Public Property Herb_ingredient As String
        Public Property GSE_id As String
        Public Property Organism As String
        Public Property Strain As String
        Public Property Tissue As String
        Public Property Cell_Type As String
        Public Property Cell_line As String
        Public Property Experiment_type As String
        Public Property Sequence_type As String
        Public Property Experiment_subject As String
        Public Property Experiment_subject_detail As String
        Public Property Experiment_special_pretreatment As String
        Public Property Control_condition As String
        Public Property Control_samples As String
        Public Property Treatment_condition As String
        Public Property Treatment_samples As String
        Public Property Drug_delivery As String
        Public Property Plat_info As String

    End Class
End Namespace