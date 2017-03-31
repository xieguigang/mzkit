Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace mzXML

    Public Class index

        <XmlAttribute>
        Public Property name As String
        <XmlElement("offset")>
        Public Property offsets As offset()

    End Class

    Public Structure offset

        <XmlAttribute>
        Public Property id As Integer
        <XmlText>
        Public Property value As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

End Namespace