Imports System.Xml.Serialization

Namespace TMIC.MarkerDB

    Public Class XML

        Public Property version As String
        Public Property biomarker_type As String
        Public Property chemicals As chemical()
        Public Property biomarkers As biomarkers
        Public Property karyotypes As karyotype()
    End Class

    Public Class biomarkers
        <XmlElement> Public Property chemical As chemical()
        <XmlElement> Public Property sequence_variants As sequence_variant()
        <XmlElement> Public Property proteins As protein()
        <XmlElement> Public Property gene As gene()

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