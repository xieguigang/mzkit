Imports System.Xml.Serialization

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
End Namespace