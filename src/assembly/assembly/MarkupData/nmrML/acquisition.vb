Imports System.Xml.Serialization

Namespace MarkupData.nmrML

    Public Class acquisition

        Public Property acquisitionMultiD As acquisitionMultiD

    End Class

    Public Class acquisitionMultiD

        Public Property fidData As fidData

    End Class

    Public Class fidData

        <XmlAttribute> Public Property byteFormat As String
        <XmlAttribute> Public Property compressed As String
        <XmlAttribute> Public Property encodedLength As Integer

        <XmlText>
        Public Property base64 As String

    End Class
End Namespace