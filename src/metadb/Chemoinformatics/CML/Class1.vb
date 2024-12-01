Imports System.Xml.Serialization

Namespace ChemicalMarkupLanguage

    Public Class bond

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property atomRefs2 As String()
        <XmlAttribute> Public Property order As Integer

    End Class
End Namespace