Imports System.Xml.Serialization

Namespace ChemicalMarkupLanguage

    ''' <summary>
    ''' Chemical Markup Language
    ''' 
    ''' https://www.xml-cml.org/
    ''' </summary>
    <XmlRoot("cml", [Namespace]:=MarkupFile.xmlns)>
    <XmlType("cml", [Namespace]:=MarkupFile.xmlns)>
    Public Class MarkupFile

        Public Const xmlns As String = "http://www.xml-cml.org/schema"

        <XmlAttribute>
        Public Property title As String
        Public Property molecule As molecule

    End Class
End Namespace