Imports System.Xml.Serialization

Namespace HMDB

    Public Structure [property]

        Public Property kind As String
        Public Property value As String
        Public Property source As String

        Public Overrides Function ToString() As String
            Return $"{value} [{kind}]"
        End Function
    End Structure

    Public Structure Properties
        <XmlElement("property")>
        Public Property PropertyList As [property]()

        Public Overrides Function ToString() As String
            Return $"{PropertyList.Length} properties..."
        End Function
    End Structure
End Namespace