Imports System.Xml.Serialization

Public Class SpatialMapping

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
