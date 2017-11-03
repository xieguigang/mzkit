Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace HMDB

    Public Class taxonomy

        Public Property description As String
        Public Property direct_parent As String
        Public Property kingdom As String
        Public Property super_class As String
        Public Property [class] As String
        Public Property sub_class As String
        Public Property molecular_framework As String
        Public Property alternative_parents As alternative_parents
        Public Property substituents As substituents

    End Class

    Public Structure alternative_parents
        <XmlElement> Public Property alternative_parent As String()
    End Structure

    Public Structure substituents
        <XmlElement> Public Property substituent As String()
    End Structure

    Public Class ontology

        Public Property status As String
        Public Property origins As origins
        Public Property biofunctions As biofunctions
        Public Property applications As applications
        Public Property cellular_locations As cellular_locations

        Public Overrides Function ToString() As String
            Return status
        End Function
    End Class

    Public Structure origins

        <XmlElement(NameOf(origin))>
        Public Property origin As String()

        Public Overrides Function ToString() As String
            Return origin.GetJson
        End Function
    End Structure

    Public Structure biofunctions

        <XmlElement(NameOf(biofunction))>
        Public Property biofunction As String()

        Public Overrides Function ToString() As String
            Return biofunction.GetJson
        End Function
    End Structure

    Public Structure applications

        <XmlElement(NameOf(application))>
        Public Property application As String()

        Public Overrides Function ToString() As String
            Return application.GetJson
        End Function
    End Structure

    Public Structure cellular_locations

        <XmlElement(NameOf(cellular_location))>
        Public Property cellular_location As String()

        Public Overrides Function ToString() As String
            Return cellular_location.GetJson
        End Function
    End Structure
End Namespace