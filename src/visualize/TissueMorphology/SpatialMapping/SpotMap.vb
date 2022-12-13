Imports System.Xml.Serialization

''' <summary>
''' A spot of spatial transcriptomics mapping to a 
''' collection of spatial metabolism pixels
''' </summary>
Public Class SpotMap

    ''' <summary>
    ''' the spot barcode
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property barcode As String
    ''' <summary>
    ''' tissue flag
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property flag As Integer

    ''' <summary>
    ''' the physical xy
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property physicalXY As Integer()
    ''' <summary>
    ''' the original raw spot xy
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property spotXY As Integer()

    ''' <summary>
    ''' pixel x of the spatial transcriptomics spot.
    ''' (after spatial transform)
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property STX As Double
    ''' <summary>
    ''' pixel y of the spatial transcriptomics spot.
    ''' (after spatial transform)
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property STY As Double

    <XmlAttribute> Public Property SMX As Integer()
    <XmlAttribute> Public Property SMY As Integer()

End Class
