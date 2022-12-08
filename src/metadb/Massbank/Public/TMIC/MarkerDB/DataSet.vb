Imports System.Xml.Serialization

Namespace TMIC.MarkerDB.XML

    Public Class DataSet

        Public Property version As String
        Public Property biomarker_type As String
        ''' <summary>
        ''' all_chemicals.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property chemicals As chemical()
        Public Property biomarkers As biomarkers
        ''' <summary>
        ''' all_karyotypes.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property karyotypes As karyotype()
        ''' <summary>
        ''' all_sequence_variants.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property sequence_variants As sequence_variant()
        ''' <summary>
        ''' all_proteins.xml
        ''' </summary>
        ''' <returns></returns>
        Public Property proteins As protein()
    End Class

    Public Class biomarkers
        ''' <summary>
        ''' + all_exposure_chemicals.xml
        ''' + all_diagnostic_chemicals.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property chemical As chemical()
        ''' <summary>
        ''' all_predictive_genetics.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property gene As gene()
        ''' <summary>
        ''' all_diagnostic_proteins.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property protein As protein()
        ''' <summary>
        ''' all_diagnostic_karyotypes.xml
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property karyotype As karyotype()
    End Class

    Public Class gene
        Public Property biomarker_type As String
        Public Property id As String
        Public Property variation As String
        Public Property position As String
        Public Property external_link As String
        Public Property gene_symbol As String
        Public Property entrez_gene_id As String
        Public Property conditions As String
        Public Property indication_types As String
    End Class

    Public Class protein
        Public Property id As String
        Public Property creation_date As String
        Public Property update_date As String
        Public Property name As String
        Public Property gene_name As String
        Public Property uniprot_id As String
        Public Property conditions As condition()
    End Class

    Public Class sequence_variant : Inherits condition
        Public Property id As String
        Public Property variation As String
        Public Property position As String
        Public Property external_link As String
        Public Property reference As String
        Public Property sequence_variant_measurements As sequence_variant_measurement()
    End Class

    Public Class sequence_variant_measurement : Inherits condition
        Public Property reference As String
    End Class

    Public Class karyotype : Inherits condition

        Public Property biomarker_type As String
        Public Property id As String
        Public Property karyotype As String
        Public Property description As String

    End Class

    Public Class chemical : Inherits condition

        Public Property biomarker_type As String
        Public Property id As String
        Public Property creation_date As String
        Public Property update_date As String
        Public Property name As String
        Public Property hmdb_id As String
        Public Property conditions As condition()

    End Class

    Public Class condition

        Public Property concentration As String
        Public Property age As String
        Public Property sex As String
        Public Property biofluid As String
        Public Property condition As String
        Public Property indication_type As String
        Public Property citation As String

    End Class

End Namespace