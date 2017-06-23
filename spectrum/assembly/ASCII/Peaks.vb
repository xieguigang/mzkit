Imports System.Xml.Serialization

Public Structure MSMSPeak

    <XmlAttribute> Public Property mz As Double
    <XmlAttribute> Public Property intensity As Double

    Public Overrides Function ToString() As String
        Return $"{mz} ({intensity})"
    End Function
End Structure
