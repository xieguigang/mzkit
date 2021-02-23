
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace MarkupData.mzXML

    Public Class index

        <XmlAttribute>
        Public Property name As String

        <XmlElement("offset")>
        Public Property offsets As offset()

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class

    Public Structure offset

        <XmlAttribute>
        Public Property id As Integer
        <XmlText>
        Public Property value As Long

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace