Imports System.Xml.Serialization

Namespace mzXML

    ''' <summary>
    ''' ``*.mzXML`` raw data
    ''' </summary>
    ''' 
    <XmlType("mzXML", [Namespace]:="http://sashimi.sourceforge.net/schema_revision/mzXML_3.2")> Public Class XML


        Public Property index As index
        Public Property indexOffset As Long
        Public Property shal As String

    End Class
End Namespace