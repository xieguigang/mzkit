Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace HERB

    Public Class HERB_reference_info

        Public Property REF_id As String
        <Column("Herb/ingredient_id")> Public Property Herb_ingredient_id As String
        <Column("Herb/ingredient_name")> Public Property Herb_ingredient_name As String
        <Column("Herb/ingredient")> Public Property Herb_ingredient As String
        Public Property Reference_title As String
        Public Property PubMed_id As String
        Public Property Journal As String
        <Column("Publish.Date")> Public Property Publish_Date As String
        Public Property DOI As String
        Public Property Experiment_subject As String
        Public Property Experiment_type As String
        Public Property Animal_Experiment As String
        Public Property Cell_Experiment As String
        Public Property Clinical_Experiment As String
        Public Property Not_Mentioned As String

    End Class
End Namespace