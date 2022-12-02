Imports System.Xml.Serialization

Public Class SpotMapping

    <XmlAttribute>
    Public Property barcode As String
    <XmlAttribute>
    Public Property flag As Integer
    <XmlAttribute>
    Public Property physicalXY As Integer()

    ''' <summary>
    ''' pixel x of the spatial transcriptomics spot
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property STX As Integer
    ''' <summary>
    ''' pixel y of the spatial transcriptomics spot
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property STY As Integer

    <XmlAttribute> Public Property SMX As Integer()
    <XmlAttribute> Public Property SMY As Integer()

End Class
