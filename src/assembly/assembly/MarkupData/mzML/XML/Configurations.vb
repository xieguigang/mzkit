
Imports System.Xml.Serialization

Namespace MarkupData.mzML

    Public Class instrumentConfiguration

        <XmlAttribute> Public Property id As String

        Public Property componentList As componentList

    End Class

    Public Class componentList : Inherits List

        Public Property source As component

        <XmlElement>
        Public Property analyzer As component()
        <XmlElement>
        Public Property detector As component()

    End Class

    Public Class component : Inherits Params

        <XmlAttribute> Public Property order As Integer

    End Class
End Namespace